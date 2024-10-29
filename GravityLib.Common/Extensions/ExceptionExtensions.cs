using System;

namespace GravityLib.Common.Extensions;

/// <summary> </summary>
public static class ExceptionExtensions
{
    /// <summary> </summary>
    public static Exception GetInnerException(this Exception ex)
    {
        var inner = ex;
        while (inner?.InnerException != null)
        {
            inner = inner.InnerException;
        }
        return inner;
    }
}