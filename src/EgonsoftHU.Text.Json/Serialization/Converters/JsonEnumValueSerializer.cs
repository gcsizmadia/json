// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
#if !NET9_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

using EgonsoftHU.Extensions.Bcl;
using EgonsoftHU.Extensions.Bcl.Constants;
using EgonsoftHU.Extensions.Bcl.Enumerations;
using EgonsoftHU.Text.Json.Exceptions;

namespace EgonsoftHU.Text.Json.Serialization.Converters
{
    /// <summary>
    /// An implementation of the <see cref="EnumValueSerializer"/> base class that uses attributes and a JSON naming policy
    /// to convert/retrieve an enum member value to/from a string.
    /// </summary>
    public class JsonEnumValueSerializer : EnumValueSerializer
    {
        private readonly JsonNamingPolicy? jsonNamingPolicy;
        private readonly IReadOnlyCollection<EnumMemberAttributeSelectorOption> enumMemberAttributeSelectorOptions;

#if !LANGVERSION13_0_OR_GREATER
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEnumValueSerializer"/> class
        /// with a specified <see cref="JsonNamingPolicy"/> and
        /// a read-only collection of <see cref="EnumMemberAttributeSelectorOption"/>.
        /// </summary>
        public JsonEnumValueSerializer(
            JsonNamingPolicy? jsonNamingPolicy,
            params
            EnumMemberAttributeSelectorOption[] enumMemberAttributeSelectorOptions
        )
        {
            this.jsonNamingPolicy = jsonNamingPolicy;
            this.enumMemberAttributeSelectorOptions = enumMemberAttributeSelectorOptions;

            ThrowIfInvalidEnumValues(enumMemberAttributeSelectorOptions);
        }
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonEnumValueSerializer"/> class
        /// with a specified <see cref="JsonNamingPolicy"/> and
        /// a read-only collection of <see cref="EnumMemberAttributeSelectorOption"/>.
        /// </summary>
        public JsonEnumValueSerializer(
            JsonNamingPolicy? jsonNamingPolicy,
#if LANGVERSION13_0_OR_GREATER
            params
#endif
            IReadOnlyCollection<EnumMemberAttributeSelectorOption> enumMemberAttributeSelectorOptions
        )
        {
            this.jsonNamingPolicy = jsonNamingPolicy;
            this.enumMemberAttributeSelectorOptions = enumMemberAttributeSelectorOptions;

            ThrowIfInvalidEnumValues(enumMemberAttributeSelectorOptions);
        }

        internal static EnumValueSerializer CreateDefault()
        {
            return new JsonEnumValueSerializer(null, EnumMemberAttributeSelectorOption.UseJsonStringEnumMemberNameAttribute);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The conversion methods in order of priority:
        /// <list type="bullet">
        /// <item>
        /// The value of the attribute selected by <see cref="EnumMemberAttributeSelectorOption"/>,
        /// if that attribute is applied to the current enum member.
        /// </item>
        /// <item><see cref="JsonNamingPolicy"/>, if specified.</item>
        /// <item>The enum member's name.</item>
        /// </list>
        /// </remarks>
        public override string Serialize<TEnum>(EnumInfo<TEnum> enumeration)
        {
            var serializedValues =
                enumeration
                    .Select(
                        flag =>
                            GetEnumMemberAttributeValue(flag.Attributes)
                            ??
                            jsonNamingPolicy?.ConvertName(flag.Name)
                            ??
                            flag.Name
                    )
                    .ToList();

            string serializedValue =
                serializedValues.Count == 1
                    ? serializedValues[0]
                    : String.Join(Strings.CommaSpaceSeparator, serializedValues);

            return serializedValue;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The <paramref name="serializedValue"/> will be compared to the following values, in order of priority:
        /// <list type="bullet">
        /// <item>
        /// The value of the attribute selected by <see cref="EnumMemberAttributeSelectorOption"/>,
        /// if that attribute is applied to the current enum member.
        /// </item>
        /// <item>The value produced by the specified <see cref="JsonNamingPolicy"/>.</item>
        /// <item>The enum member's name. The comparing is case sensitive.</item>
        /// </list>
        /// </remarks>
        public override EnumInfo<TEnum>? Deserialize<TEnum>(string serializedValue)
        {
            string[] serializedSingleValues = GetNames(serializedValue);

            var membersFound =
                serializedSingleValues
                    .Select(
                        serializedSingleValue =>
                            EnumInfo<TEnum>.DeclaredMembers.FirstOrDefault(
                                member =>
                                (
                                    GetEnumMemberAttributeValue(member.Attributes) is string attributeValue
                                    &&
                                    String.Equals(serializedSingleValue, attributeValue, StringComparison.Ordinal)
                                )
                                ||
                                (
                                    jsonNamingPolicy is not null
                                    &&
                                    String.Equals(serializedSingleValue, jsonNamingPolicy.ConvertName(member.Name), StringComparison.Ordinal)
                                )
                                ||
                                String.Equals(serializedSingleValue, member.Name, StringComparison.Ordinal))
                    )
                    .OfType<EnumInfo<TEnum>>()
                    .ToList();

            EnumInfo<TEnum>? enumeration =
                (membersFound.Count, serializedSingleValues.Length) switch
                {
                    (1, 1) => membersFound[0],
                    var (found, serialized) when found == serialized && found > 1 => membersFound.Aggregate((first, second) => first | second),
                    _ => throw JsonExceptions.InvalidEnumValue()
                };

            return enumeration;
        }

        private static void ThrowIfInvalidEnumValues(IReadOnlyCollection<EnumMemberAttributeSelectorOption> enumMemberAttributeSelectorOptions)
        {
            if (enumMemberAttributeSelectorOptions.All(value => EnumInfo.TryFromValue(value, out _)))
            {
                return;
            }

            throw new ArgumentException("Invalid options.", nameof(enumMemberAttributeSelectorOptions));
        }

        private string? GetEnumMemberAttributeValue(IEnumerationAttributes attributes)
        {
            foreach (EnumMemberAttributeSelectorOption nameSelectorOption in enumMemberAttributeSelectorOptions)
            {
                string? name = nameSelectorOption switch
                {
                    EnumMemberAttributeSelectorOption.UseJsonStringEnumMemberNameAttribute => GetJsonStringEnumMemberName(attributes),
                    EnumMemberAttributeSelectorOption.UseEnumMemberAttribute => attributes.EnumMember?.Value,
                    EnumMemberAttributeSelectorOption.UseDescriptionAttribute => attributes.Description?.Description,
                    EnumMemberAttributeSelectorOption.UseDisplayNameAttribute => attributes.DisplayName?.DisplayName,
                    EnumMemberAttributeSelectorOption.UseDisplayAttributeGetName => attributes.Display?.GetName(),
                    EnumMemberAttributeSelectorOption.UseDisplayAttributeGetShortName => attributes.Display?.GetShortName(),
                    EnumMemberAttributeSelectorOption.UseDisplayAttributeGetDescription => attributes.Display?.GetDescription(),
                    _ => null
                };

                if (!name.IsNullOrWhiteSpace())
                {
                    return name;
                }
            }

            return null;
        }

        private static string? GetJsonStringEnumMemberName(IEnumerationAttributes attributes)
        {
            return
#if NET9_0_OR_GREATER
                attributes.JsonStringEnumMemberName?.Name
#else
                attributes.GetAttribute<JsonStringEnumMemberNameAttribute>()?.Name
#endif
                ;
        }
    }
}
