using System.Collections.Generic;

namespace GravityLib.Common.SharedModels
{
    public class ListResult<T> : IListResult<T>
    {
        public IList<T> Items { get; set; }

        public long Count { get; set; }
    }
}
