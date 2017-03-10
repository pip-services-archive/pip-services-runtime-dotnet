using System;

namespace PipServices.Runtime
{
    /// <summary>
    ///     Callback interface to complete measuring time interval.
    /// </summary>
    public interface ITiming : IDisposable
    {
        /// <summary>
        ///     Completes measuring time interval and updates counter.
        /// </summary>
        void EndTiming();
    }
}