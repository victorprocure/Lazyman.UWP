// -----------------------------------------------------------------------
// <copyright file="Deferral.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Represents a deferral and a deferral manager
// </summary>

namespace UI.Framework
{
    using System;

    /// <summary>
    /// Represents a deferral and a deferral manager
    /// </summary>
    public sealed class Deferral
    {
        /// <summary>
        /// The deferral callback.
        /// </summary>
        private readonly Action callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deferral"/> class.
        /// </summary>
        /// <param name="callback">
        /// The callback.
        /// </param>
        public Deferral(Action callback)
        {
            this.callback = callback;
        }

        /// <summary>
        /// Complete the deferral
        /// </summary>
        public void Complete()
        {
            this.callback.Invoke();
        }
    }
}