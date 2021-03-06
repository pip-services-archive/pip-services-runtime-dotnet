﻿using System;
using PipServices.Runtime.Config;

namespace PipServices.Runtime.Counters
{
    public class NullCounters : AbstractComponent, ICounters
    {
        /// <summary>
        ///     Unique descriptor for the NullCounters component
        /// </summary>
        public static readonly ComponentDescriptor ClassDescriptor = new ComponentDescriptor(
            Category.Counters, "pip-services-runtime-counters", "null", "*"
            );

        /// <summary>
        ///     Creates instance of null counter that doesn't do anything.
        /// </summary>
        public NullCounters()
            : base(ClassDescriptor)
        {
        }

        /// <summary>
        ///     Starts measurement of execution time interval.
        ///     The method returns ITiming object that provides EndTiming()
        ///     method that shall be called when execution is completed
        ///     to calculate elapsed time and update the counter.
        /// </summary>
        /// <param name="name">the name of interval counter.</param>
        /// <returns>
        ///     callback interface with EndTiming() method
        ///     that shall be called at the end of execution.
        /// </returns>
        public override ITiming BeginTiming(string name)
        {
            return new Timing();
        }

        /// <summary>
        ///     Calculates rolling statistics: minimum, maximum, average
        ///     and updates Statistics counter.
        ///     This counter can be used to measure various non-functional
        ///     characteristics, such as amount stored or transmitted data,
        ///     customer feedback, etc.
        /// </summary>
        /// <param name="name">the name of statistics counter.</param>
        /// <param name="value">the value to add to statistics calculations.</param>
        public override void Stats(string name, float value)
        {
        }

        /// <summary>
        ///     Records the last reported value.
        ///     This counter can be used to store performance values reported
        ///     by clients or current numeric characteristics such as number
        ///     of values stored in cache.
        /// </summary>
        /// <param name="name">the name of last value counter</param>
        /// <param name="value">the value to be stored as the last one</param>
        public override void Last(string name, float value)
        {
        }

        /// <summary>
        ///     Records the current time.
        ///     This counter can be used to track timing of key business transactions.
        /// </summary>
        /// <param name="name">the name of timing counter</param>
        public override void TimestampNow(string name)
        {
        }

        /// <summary>
        ///     Records specified time.
        ///     This counter can be used to tack timing of key
        ///     business transactions as reported by clients.
        /// </summary>
        /// <param name="name">the name of timing counter.</param>
        /// <param name="value">the reported timing to be recorded.</param>
        public override void Timestamp(string name, DateTime value)
        {
        }

        /// <summary>
        ///     Increments counter by value of 1.
        ///     This counter is often used to calculate
        ///     number of client calls or performed transactions.
        /// </summary>
        /// <param name="name">the name of counter counter.</param>
        public override void IncrementOne(string name)
        {
        }

        /// <summary>
        ///     Increments counter by specified value.
        ///     This counter can be used to track various numeric characteristics
        /// </summary>
        /// <param name="name">the name of counter counter.</param>
        /// <param name="value">number to increase the counter.</param>
        public override void Increment(string name, int value)
        {
        }
    }
}