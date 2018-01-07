// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ActivateWindowSources.cs" company="Procure Software Development">
//    Copyright (c) Procure Software Development
// </copyright>
// <author>Victor Procure</author>
// --------------------------------------------------------------------------------------------------------------------

namespace UI.Framework.Display
{
    /// <summary>
    /// The reason a window source is activated.
    /// </summary>
    public enum ActivateWindowSources
    {
        /// <summary>
        /// Activated by launching
        /// </summary>
        Launching,

        /// <summary>
        /// Activated sources
        /// </summary>
        Activating,

        /// <summary>
        /// Activated by splash screen
        /// </summary>
        SplashScreen,

        /// <summary>
        /// Activated by resuming
        /// </summary>
        Resuming
    }
}