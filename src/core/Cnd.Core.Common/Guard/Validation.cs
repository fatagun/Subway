namespace Cnd.Core.Common
{
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    [DebuggerStepThrough]
    public struct Validation<T>
    {
        public readonly string Name;
        public readonly T Value;

        public Validation(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public Validation<T> Is => this;
    }
}
