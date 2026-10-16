// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.Text.Json;
using System.Text.Json.Serialization;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    /// <summary>
    /// The configurable options used by the <see cref="EnumConverter{TEnum}"/>.
    /// </summary>
    public sealed record EnumConverterOptions(
        bool AllowNumbers,
        bool AllowStrings = true,
        EnumValueSerializer? EnumValueSerializer = null
    )
    {
        /// <summary>
        /// The default options.
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
        public static EnumConverterOptions Default { get; } = new(AllowNumbers: true);

        /// <summary>
        /// Gets whether number format is allowed during both serialization and deserialization.
        /// </summary>
        /// <remarks>
        /// If <see langword="false" /> then <see cref="JsonException"/> will be thrown when serializing undefined enum values or deserializing a JSON number.
        /// <para>Either <see cref="AllowNumbers"/> or <see cref="AllowStrings"/> must be <see langword="true"/>; otherwise, <see cref="JsonException"/> will be thrown during both serialization and deserialization.</para>
        /// </remarks>
        public bool AllowNumbers { get; init; } = AllowNumbers;

        /// <summary>
        /// Gets whether string format is allowed during both serialization and deserialization.
        /// </summary>
        /// <remarks>
        /// If <see langword="false"/> then <see cref="JsonException"/> will be thrown when deserializing a JSON string.
        /// <para>Either <see cref="AllowNumbers"/> or <see cref="AllowStrings"/> must be <see langword="true"/>; otherwise, <see cref="JsonException"/> will be thrown during both serialization and deserialization.</para>
        /// </remarks>
        public bool AllowStrings { get; init; } = AllowStrings;

        /// <summary>
        /// The serializer for enum values.
        /// </summary>
        public EnumValueSerializer EnumValueSerializer { get; init; } = EnumValueSerializer ?? JsonEnumValueSerializer.CreateDefault();
    }
}
