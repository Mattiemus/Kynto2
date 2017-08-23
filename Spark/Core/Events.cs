namespace Spark.Core
{
    using System;

    /// <summary>
    /// Represents a method that takes a strongly typed sender and event argument.
    /// </summary>
    /// <typeparam name="TSender">Sender object type</typeparam>
    /// <typeparam name="TEventArgs">Event args type</typeparam>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">Event arguments</param>
    [Serializable]
    public delegate void TypedEventHandler<in TSender, in TEventArgs>(TSender sender, TEventArgs e) where TEventArgs : EventArgs;

    /// <summary>
    /// Represents a method that takes a strongly typed sender.
    /// </summary>
    /// <typeparam name="TSender">Sender object type</typeparam>
    /// <param name="sender">Source of the event</param>
    /// <param name="e">Event arguments</param>
    [Serializable]
    public delegate void TypedEventHandler<in TSender>(TSender sender, EventArgs e);
}
