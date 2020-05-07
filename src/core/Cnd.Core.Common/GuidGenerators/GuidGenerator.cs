using Cnd.Core.ServiceLifetime;
using JetBrains.Annotations;
using System;

namespace Cnd.Core.Common
{
    public class GuidGenerator : IGuidGenerator, ISingletonService
    {
        // For direct access with out DI.
        public static GuidGenerator Instance { get; } = new GuidGenerator();

        public Guid Generate() => Guid.NewGuid();

        public Guid Parse([NotNull]string text) => Guid.Parse(text);
    }
}
