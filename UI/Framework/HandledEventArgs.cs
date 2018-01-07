// -----------------------------------------------------------------------
// <copyright file="HandledEventArgs.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Represents handled event arguments
// </summary>


namespace UI.Framework
{
    using System;

    /// <inheritdoc />
    /// <summary>
    /// Represents handled event arguments
    /// </summary>
    public class HandledEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether handled.
        /// </summary>
        public bool Handled { get; set; }
    }
}