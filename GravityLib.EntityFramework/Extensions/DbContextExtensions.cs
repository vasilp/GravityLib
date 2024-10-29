using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GravityLib.EntityFramework.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// Deletes on batches by 2000 items with raw SQL from the given table with the given where statement.
        ///
        /// NOTE: This method is compatible with SQL Server, using raw SQL execution and does not follow SaveChanges() calls!
        /// As a infinite loop prevention the maximum amount if items that could be deleted is 1 000 000.
        /// If the function returns the above number of deleted items, there might be more to be deleted and should be called again.
        /// </summary>
        /// <returns>The total items affected (deleted)</returns>
        public static async Task<int> DeleteOnBatchesSqlAsync(
            this DbContext context,
            string table,
            string where,
            params object[] parameters)
        {
            const int batchSize = 2000;
            var affectedRows = 1;
            var counter = 0;
            var totalRows = 0;

            while (affectedRows > 0 && counter <= 500)
            {
                var command = $"DELETE TOP ({batchSize}) FROM [{table}] WHERE {where}";

                affectedRows = await context.Database.ExecuteSqlRawAsync(command, parameters);

                totalRows += affectedRows;
                counter += 1;

                // Wait a bit if we hit the top limit of the delete (this means there might be more data left for deletion),
                // so not to flood the SQL server with requests
                if (affectedRows == batchSize)
                {
                    await Task.Delay(50);
                }
            }

            return totalRows;
        }
    }
}
