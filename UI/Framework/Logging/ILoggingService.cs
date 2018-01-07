// -----------------------------------------------------------------------
// <copyright file="ILoggingService.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>


namespace UI.Framework.Logging
{
    using System;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// The debug write delegate.
    /// </summary>
    /// <param name="text">
    /// The log message
    /// </param>
    /// <param name="severity">
    /// The severity of message.
    /// </param>
    /// <param name="target">
    /// The target of the message.
    /// </param>
    /// <param name="caller">
    /// The caller of the logger.
    /// </param>
    public delegate void DebugWriteDelegate(
        string text = null,
        Severities severity = Severities.Info,
        Targets target = Targets.Debug,
        [CallerMemberName] string caller = null);

    /// <summary>
    /// Represents a logging service
    /// </summary>
    public interface ILoggingService
    {
        /// <summary>
        /// Gets or sets a value indicating whether logging service is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the write line delegate.
        /// </summary>
        DebugWriteDelegate WriteLine { get; }
    }
}