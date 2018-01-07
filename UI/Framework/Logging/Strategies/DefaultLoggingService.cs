// -----------------------------------------------------------------------
// <copyright file="DefaultLoggingService.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>


namespace UI.Framework.Logging.Strategies
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.CompilerServices;

    /// <inheritdoc />
    /// <summary>
    /// Represents a default logging service
    /// </summary>
    public class DefaultLoggingService : ILoggingService
    {
        /// <inheritdoc />
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        public DebugWriteDelegate WriteLine => this.WriteLineInternal;

        /// <summary>
        /// The write line internal delegate.
        /// </summary>
        /// <param name="text">
        /// The log message.
        /// </param>
        /// <param name="severity">
        /// The severity of the log message.
        /// </param>
        /// <param name="target">
        /// The target of the log message.
        /// </param>
        /// <param name="caller">
        /// The caller of the log writer.
        /// </param>
        /// <exception cref="NotImplementedException">
        /// Thrown when target is not debug and delegate has not been overridden
        /// </exception>
        private void WriteLineInternal(
            string text = null,
            Severities severity = Severities.Info,
            Targets target = Targets.Debug,
            [CallerMemberName] string caller = null)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (target)
            {
                case Targets.Debug:
                    if (this.IsEnabled)
                    {
                        Debug.WriteLine($"{DateTime.Now.TimeOfDay.ToString()} {severity} {caller} {text}");
                    }

                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}