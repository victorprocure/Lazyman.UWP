// // --------------------------------------------------------------------------------------------------------------------
// // <copyright file="AppExecutionState.cs" company="Procure Software Development">
// //   Copyright (c) Procure Software Development
// // </copyright>
// // <author>Victor Procure</author>
// // --------------------------------------------------------------------------------------------------------------------

namespace UI.Framework
{
    /// <summary>
    /// The application execution state
    /// </summary>
    public enum AppExecutionState
    {
        /// <summary>
        /// Application execution is suspended
        /// </summary>
        Suspended,

        /// <summary>
        /// Application execution is terminated
        /// </summary>
        Terminated,

        /// <summary>
        /// Application execution is performing prelaunch
        /// </summary>
        Prelaunch
    }
}