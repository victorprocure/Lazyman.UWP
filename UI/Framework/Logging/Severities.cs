// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="Severities.cs" company="Procure Software Development">
// //   Copyright (c) Procure Software Development
// // </copyright>
// // <author>Victor Procure</author>
// // <summary>
// // 
// //   </summary>
// // --------------------------------------------------------------------------------------------------------------------

namespace UI.Framework.Logging
{
    /// <summary>
    /// Representation of log message severities
    /// </summary>
    public enum Severities
    {
        /// <summary>
        /// The message shown is a debug message
        /// </summary>
        Debug,

        /// <summary>
        /// The message shown is informational
        /// </summary>
        Info,

        /// <summary>
        /// The message shown was a handled error
        /// </summary>
        Warning,

        /// <summary>
        /// The message shown is an unhandled error
        /// </summary>
        Error,

        /// <summary>
        /// The messaged shown caused application to crash
        /// </summary>
        Critical
    }
}