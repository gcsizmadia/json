// Copyright © 2023-2026 Gabor Csizmadia
// This code is licensed under MIT license (see LICENSE for details)

using EgonsoftHU.Text.Json.UnitTests;

using FluentAssertions;
using FluentAssertions.Extensibility;

[assembly: AssertionEngineInitializer(typeof(AssertionEngineInitializer), nameof(AssertionEngineInitializer.AcknowledgeSoftWarning))]

namespace EgonsoftHU.Text.Json.UnitTests
{
    public static class AssertionEngineInitializer
    {
        public static void AcknowledgeSoftWarning()
        {
            License.Accepted = true;
        }
    }
}
