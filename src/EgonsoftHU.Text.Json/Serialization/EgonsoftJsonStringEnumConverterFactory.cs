// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

using EgonsoftHU.Extensions.Bcl;
using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json.Serialization
{
    /// <summary>
    /// Converter to convert enums to and from strings.
    /// </summary>
    public class EgonsoftJsonStringEnumConverterFactory : JsonConverterFactory
    {
        private readonly EnumConverterOptions converterOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="EgonsoftJsonStringEnumConverterFactory"/> class with the default options.
        /// </summary>
        /// <remarks>
        /// The default options are:
        /// <list type="bullet">
        /// <item>when serializing, defined enum values will be serialized as a JSON string</item>
        /// <item>when serializing, undefined enum values will be serialized as a JSON number</item>
        /// <item>when deserializing, both JSON strings and JSON numbers are accepted</item>
        /// <item>no <see cref="JsonNamingPolicy"/> will be used during both serialization and deserialization</item>
        /// <item>
        /// the serialized value will be determined in the following order, from highest to lowest priority:
        /// <list type="bullet">
        /// <item>the value of the <see cref="JsonStringEnumMemberNameAttribute.Name"/> property</item>
        /// <item>the member name in its original form</item>
        /// <item>the undefined enum value in number format</item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        public EgonsoftJsonStringEnumConverterFactory()
            : this(EnumConverterOptions.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgonsoftJsonStringEnumConverterFactory"/> class with a specified naming policy.
        /// </summary>
        /// <param name="jsonNamingPolicy">The optional naming policy for reading and writing enum values.</param>
        /// <remarks>
        /// The following options will be used:
        /// <list type="bullet">
        /// <item>when serializing, defined enum values will be serialized as a JSON string</item>
        /// <item>when serializing, undefined enum values will be serialized as a JSON number</item>
        /// <item>when deserializing, both JSON strings and JSON numbers are accepted</item>
        /// <item>
        /// the serialized value will be determined in the following order, from highest to lowest priority:
        /// <list type="bullet">
        /// <item>the value of the <see cref="JsonStringEnumMemberNameAttribute.Name"/> property</item>
        /// <item>the result of calling <see cref="JsonNamingPolicy.ConvertName(String)"/> method on the specified <paramref name="jsonNamingPolicy"/></item>
        /// <item>the member name in its original form</item>
        /// <item>the undefined enum value in number format</item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        public EgonsoftJsonStringEnumConverterFactory(JsonNamingPolicy jsonNamingPolicy)
            : this(
                EnumConverterOptions.Default with
                {
                    EnumValueSerializer =
                        new JsonEnumValueSerializer(
                            jsonNamingPolicy,
                            EnumMemberAttributeSelectorOption.UseJsonStringEnumMemberNameAttribute
                        )
                }
            )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgonsoftJsonStringEnumConverterFactory"/> class
        /// with a value that indicates whether integer values are allowed.
        /// </summary>
        /// <param name="allowIntegerValues">
        /// <see langword="true"/> to allow undefined enum values; otherwise, <see langword="false"/>.
        /// When <see langword="true"/>, if an enum value isn't defined, it will output as a number rather than a string.
        /// </param>
        /// <remarks>
        /// The following options will be used:
        /// <list type="bullet">
        /// <item>when serializing, defined enum values will be serialized as a JSON string,</item>
        /// <item>
        /// when serializing, if <paramref name="allowIntegerValues"/> is <see langword="true"/>,
        /// undefined enum values will be serialized as a JSON number; otherwise, a <see cref="JsonException"/> will be thrown,
        /// </item>
        /// <item>
        /// when deserializing, if <paramref name="allowIntegerValues"/> is <see langword="true"/>,
        /// both JSON string and JSON number are accepted; otherwise, only JSON string is accepted,
        /// </item>
        /// <item>
        /// the serialized value will be determined in the following order, from highest to lowest priority:
        /// <list type="bullet">
        /// <item>the value of the <see cref="JsonStringEnumMemberNameAttribute.Name"/> property</item>
        /// <item>the member name in its original form</item>
        /// <item>the undefined enum value in number format, if <paramref name="allowIntegerValues"/> is <see langword="true"/></item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        public EgonsoftJsonStringEnumConverterFactory(bool allowIntegerValues)
            : this(
                EnumConverterOptions.Default with
                {
                    AllowNumbers = allowIntegerValues
                }
            )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgonsoftJsonStringEnumConverterFactory"/> class
        /// with a specified naming policy and a value that indicates whether integer values are allowed.
        /// </summary>
        /// <param name="jsonNamingPolicy">The optional naming policy for reading and writing enum values.</param>
        /// <param name="allowIntegerValues">
        /// <see langword="true"/> to allow undefined enum values; otherwise, <see langword="false"/>.
        /// When <see langword="true"/>, if an enum value isn't defined, it will output as a number rather than a string.
        /// </param>
        /// <remarks>
        /// The following options will be used:
        /// <list type="bullet">
        /// <item>when serializing, defined enum values will be serialized as a JSON string,</item>
        /// <item>
        /// when serializing, if <paramref name="allowIntegerValues"/> is <see langword="true"/>,
        /// undefined enum values will be serialized as a JSON number; otherwise, a <see cref="JsonException"/> will be thrown,
        /// </item>
        /// <item>
        /// when deserializing, if <paramref name="allowIntegerValues"/> is <see langword="true"/>,
        /// both JSON string and JSON number are accepted; otherwise, only JSON string is accepted,
        /// </item>
        /// <item>
        /// the serialized value will be determined in the following order, from highest to lowest priority:
        /// <list type="bullet">
        /// <item>the value of the <see cref="JsonStringEnumMemberNameAttribute.Name"/> property</item>
        /// <item>the result of calling <see cref="JsonNamingPolicy.ConvertName(String)"/> method on the specified <paramref name="jsonNamingPolicy"/></item>
        /// <item>the member name in its original form</item>
        /// <item>the undefined enum value in number format, if <paramref name="allowIntegerValues"/> is <see langword="true"/></item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        public EgonsoftJsonStringEnumConverterFactory(JsonNamingPolicy? jsonNamingPolicy, bool allowIntegerValues)
            : this(
                EnumConverterOptions.Default with
                {
                    AllowNumbers = allowIntegerValues,
                    EnumValueSerializer =
                        new JsonEnumValueSerializer(
                            jsonNamingPolicy,
                            EnumMemberAttributeSelectorOption.UseJsonStringEnumMemberNameAttribute
                        )
                }
            )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EgonsoftJsonStringEnumConverterFactory"/> class with the specified options.
        /// </summary>
        /// <param name="converterOptions">The converter options to be used.</param>
        public EgonsoftJsonStringEnumConverterFactory(EnumConverterOptions converterOptions)
        {
            converterOptions.ThrowIfNull();

            this.converterOptions = converterOptions;
        }

        /// <inheritdoc/>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        /// <inheritdoc/>
        public sealed override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return EnumConverterFactory.Create(typeToConvert, options, converterOptions);
        }
    }
}
