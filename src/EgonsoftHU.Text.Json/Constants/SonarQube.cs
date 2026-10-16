// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace System.Diagnostics.CodeAnalysis
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    internal static class SonarQube
    {
        internal const string Category = nameof(SonarQube);

        /// <summary>
        /// Static fields should not be used in generic types
        /// </summary>
        internal const string S2743 = nameof(S2743);

        /// <summary>
        /// Inner class members should not shadow outer class "static" or type members
        /// </summary>
        internal const string S3218 = nameof(S3218);
    }
}
