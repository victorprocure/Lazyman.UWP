// -----------------------------------------------------------------------
// <copyright file="JsonSerializationService.cs" company="Procure Software Development">
// Copyright (c) 2018 Procure Software Development
// </copyright>
// <summary>
// Represents a JSON serializer service
// </summary>

namespace UI.Framework.Serialization
{
    using System;

    using Newtonsoft.Json;

    /// <inheritdoc />
    /// <summary>
    ///     Represents a JSON serializer service
    /// </summary>
    public sealed class JsonSerializationService : ISerializationService
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonSerializationService" /> class.
        /// </summary>
        internal JsonSerializationService()
        {
            this.Settings = new JsonSerializerSettings
                                {
                                    Formatting = Formatting.None,
                                    TypeNameHandling = TypeNameHandling.Auto,
                                    TypeNameAssemblyFormatHandling =
                                        TypeNameAssemblyFormatHandling.Simple,
                                    PreserveReferencesHandling = PreserveReferencesHandling.All,
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                                    ObjectCreationHandling = ObjectCreationHandling.Auto,
                                    ConstructorHandling =
                                        ConstructorHandling.AllowNonPublicDefaultConstructor
                                };
        }

        /// <summary>
        ///     Gets JSON serializer settings.
        /// </summary>
        public JsonSerializerSettings Settings { get; }

        /// <inheritdoc />
        public object Deserialize(string value)
        {
            if (value == null)

                return null;

            if (value == string.Empty)

                return string.Empty;

            // Deserialize from json
            var container = JsonConvert.DeserializeObject<Container>(value);

            var type = Type.GetType(container.Type);

            return JsonConvert.DeserializeObject(container.Data, type, this.Settings);
        }

        /// <inheritdoc />
        public T Deserialize<T>(string value)
        {
            var result = this.Deserialize(value);

            if (result != null) return (T)result;

            return default(T);
        }

        /// <inheritdoc />
        public string Serialize(object value)
        {
            if (value == null)

                return null;

            if (value as string == string.Empty)

                return string.Empty;

            // Serialize to json
            var container = new Container
                                {
                                    Type = value.GetType().AssemblyQualifiedName,
                                    Data = JsonConvert.SerializeObject(value, Formatting.None, this.Settings)
                                };

            return JsonConvert.SerializeObject(container);
        }

        /// <inheritdoc />
        public bool TryDeserialize<T>(string value, out T result)
        {
            var r = this.Deserialize(value);

            if (r == null)
            {
                result = default(T);

                return false;
            }

            try
            {
                result = (T)r;

                return true;
            }
            catch
            {
                result = default(T);

                return false;
            }
        }

        /// <summary>
        /// The container representation
        /// </summary>
        internal sealed class Container
        {
            /// <summary>
            /// Gets or sets the data.
            /// </summary>
            public string Data { get; set; }

            /// <summary>
            /// Gets or sets the type.
            /// </summary>
            public string Type { get; set; }
        }
    }
}