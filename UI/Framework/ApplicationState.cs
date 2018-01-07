// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationState.cs" company="Procure Software Development">
//   Copyright (c) Procure Software Development
// </copyright>
// <author>Victor Procure</author>
// --------------------------------------------------------------------------------------------------------------------

namespace UI.Framework
{
    /// <summary>
    /// The current application state
    /// </summary>
    public enum ApplicationState
    {
        /// <summary>
        /// Application has no current state
        /// </summary>
        None,

        /// <summary>
        /// Application running state
        /// </summary>
        Running,

        /// <summary>
        /// Application before initialization state
        /// </summary>
        BeforeInit,

        /// <summary>
        /// Application after initialization state
        /// </summary>
        AfterInit,

        /// <summary>
        /// Application before launch state
        /// </summary>
        BeforeLaunch,

        /// <summary>
        /// Application after launch state
        /// </summary>
        AfterLaunch,

        /// <summary>
        /// Application before activate state
        /// </summary>
        BeforeActivate,

        /// <summary>
        /// Application after activate state
        /// </summary>
        AfterActivate,

        /// <summary>
        /// Application before start state
        /// </summary>
        BeforeStart,

        /// <summary>
        /// Application after start state
        /// </summary>
        AfterStart
    }
}