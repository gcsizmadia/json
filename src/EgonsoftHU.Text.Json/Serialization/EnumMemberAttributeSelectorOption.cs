// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using EgonsoftHU.Text.Json.Serialization.Converters;

namespace EgonsoftHU.Text.Json.Serialization
{
    /// <summary>
    /// Specifies options for <see cref="JsonEnumValueSerializer"/> which attribute to use.
    /// </summary>
    public enum EnumMemberAttributeSelectorOption
    {
        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="JsonStringEnumMemberNameAttribute"/> attribute.
        /// </summary>
        UseJsonStringEnumMemberNameAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="EnumMemberAttribute"/> attribute.
        /// </summary>
        UseEnumMemberAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DescriptionAttribute"/> attribute.
        /// </summary>
        UseDescriptionAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayNameAttribute"/> attribute.
        /// </summary>
        UseDisplayNameAttribute,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayAttribute.GetName"/> method.
        /// </summary>
        UseDisplayAttributeGetName,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayAttribute.GetShortName"/> method.
        /// </summary>
        UseDisplayAttributeGetShortName,

        /// <summary>
        /// The <see cref="JsonEnumValueSerializer"/> will use the <see cref="DisplayAttribute.GetDescription"/> method.
        /// </summary>
        UseDisplayAttributeGetDescription
    }
}
