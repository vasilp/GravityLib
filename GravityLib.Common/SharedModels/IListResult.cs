using System.Collections.Generic;

namespace GravityLib.Common.SharedModels
{
    public interface IListResult<T>
    {
        IList<T> Items { get; }

        long Count { get; }
    }
}
