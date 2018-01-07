// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICachingService.cs" company="Procure Software Development">
//   Copyright (c) 2018 Procure Software Development
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UI.Framework.Caching
{
    using System;

    /// <summary>
    /// Defines a caching service
    /// </summary>
    public interface ICachingService
    {
        /// <summary>
        /// Gets or sets the cache max duration.
        /// </summary>
        TimeSpan CacheMaxDuration { get; set; }

        /// <summary>
        /// Gets the cache date key.
        /// </summary>
        string CacheDateKey { get; }
    }
}