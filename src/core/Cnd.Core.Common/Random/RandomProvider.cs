using System;

namespace Cnd.Core.Common
{
    public interface IRandomProvider
    {
        int GetRandomNumber();
        int GetRandomNumber(int min, int max);
    }

    public class RandomProvider : IRandomProvider
    {
        private static readonly Random Random = new Random();

        public int GetRandomNumber() => this.GetRandomNumber(0, int.MaxValue);

        public int GetRandomNumber(int min, int max)
        {
            lock (Random)
            {
                return Random.Next(min, max);
            }
        }
    }
}
