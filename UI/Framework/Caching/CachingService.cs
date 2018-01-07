// -----------------------------------------------------------------------
// <copyright file="CachingService.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>

namespace UI.Framework.Caching
{
    using System;

    /// <inheritdoc />
    /// <summary>Abstract caching service that most services should derive from</summary>
    public abstract class CachingService : ICachingService
    {
        /// <inheritdoc />
        public virtual TimeSpan CacheMaxDuration { get; set; } = TimeSpan.MaxValue;

        /// <inheritdoc />
        public virtual string CacheDateKey { get; } = "Setting-Cache-Date";

    }
}