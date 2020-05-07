using System;

namespace Cnd.Core.Common
{
    public interface IGuidGenerator
    {
        Guid Generate();
        Guid Parse(string text);
    }
}
