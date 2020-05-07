using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;

namespace Cnd.Core.Common
{
    public interface ISecureRandomProvider : IDisposable
    {
        bool GenerateBool();
        double GenerateDouble();
        float GenerateFloat();
        int GenerateInt();
        int GenerateInt(uint minimum, uint maximum);
        long GenerateLong();
        long GenerateLong(ulong minimum, ulong maximum);
        string GenerateString(int length);
    }

    public class SecureRandomProvider : ISecureRandomProvider
    {
        public const string STRING_CHARSET = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";

        public const string BYTES_EXCEEDS_BUFFER_ERROR = "Number of bytes required exceeds the size of buffer. Try with a larger buffer.";
        public const string MINIMUM_EXCEEDS_MAXIMUM_ERROR = "Minimum cannot exceed maximum";
        public const string DETERMINABLE_OUTPUT_ERROR = "Only one output is possible";

        public const string INVALID_BUFFER_SIZE_WARNING = "Invalid buffer size requested. Initialising with 1024-byte buffer.";
        public const string ENTROPY_COMPROMISED_WARNING = "The number of valid characters supplied cannot be evenly divided by 256. The entropy of the output is compromised.";

        private const int DEFAULT_SHARED_BUFFER_LENGTH = 1024;
        private static RNGCryptoServiceProvider _prng;
        private byte[] _sharedBuffer;
        private uint _currentBufferIndex;
        private readonly int _bufferLength;

        public SecureRandomProvider()
            : this(DEFAULT_SHARED_BUFFER_LENGTH)
        {
        }

        public SecureRandomProvider(int sharedBufferSize)
        {
            if (sharedBufferSize < 1)
            {
                sharedBufferSize = DEFAULT_SHARED_BUFFER_LENGTH;
            }

            _prng = new RNGCryptoServiceProvider();
            _bufferLength = sharedBufferSize;
        }

        private void InitSharedBuffer()
        {
            if (_sharedBuffer == null || _sharedBuffer.Length != _bufferLength)
                _sharedBuffer = new byte[_bufferLength];

            _prng.GetBytes(_sharedBuffer);
            _currentBufferIndex = 0;
        }

        private void RequestBuffer(uint bytesRequired)
        {
            if (_sharedBuffer == null)
                InitSharedBuffer();

            if (bytesRequired > _bufferLength)
                throw new ArgumentOutOfRangeException(nameof(bytesRequired), BYTES_EXCEEDS_BUFFER_ERROR);

            if ((_sharedBuffer.Length - _currentBufferIndex) < bytesRequired)
                InitSharedBuffer();
        }

        public bool GenerateBool()
            => GenerateInt() % 2 == 0;

        public int GenerateInt()
        {
            lock (this)
            {
                RequestBuffer(sizeof(uint));

                var result = BitConverter.ToUInt32(_sharedBuffer, (int)_currentBufferIndex);

                _currentBufferIndex += sizeof(uint);

                return (int)result >= 0 ? (int)result : (int)result * -1;
            }
        }

        public int GenerateInt(uint minimum, uint maximum)
        {
            if (minimum > maximum)
                throw new ArgumentOutOfRangeException(nameof(minimum), MINIMUM_EXCEEDS_MAXIMUM_ERROR);

            if (minimum == maximum)
                throw new ArgumentException(DETERMINABLE_OUTPUT_ERROR);

            ulong difference = maximum - minimum;

            lock (this)
            {
                RequestBuffer(sizeof(uint));

                var randomInteger = BitConverter.ToUInt32(_sharedBuffer, (int)_currentBufferIndex);

                const ulong max = (ulong)uint.MaxValue + 1;
                var remainder = max % difference;

                var result = (uint)(minimum + (randomInteger % difference));

                _currentBufferIndex += sizeof(uint);

                return (int)result >= 0
                    ? (int)result
                    : (int)result * -1;
            }
        }

        public long GenerateLong()
        {
            lock (this)
            {
                RequestBuffer(sizeof(ulong));

                var result = BitConverter.ToUInt64(_sharedBuffer, (int)_currentBufferIndex);

                _currentBufferIndex += sizeof(ulong);

                return (long)result >= 0
                    ? (long)result
                    : (long)result * -1;
            }
        }

        public long GenerateLong(ulong minimum, ulong maximum)
        {
            if (minimum > maximum)
                throw new ArgumentOutOfRangeException(nameof(minimum), MINIMUM_EXCEEDS_MAXIMUM_ERROR);

            if (minimum == maximum)
                throw new ArgumentException(DETERMINABLE_OUTPUT_ERROR);

            var difference = maximum - minimum;

            lock (this)
            {
                RequestBuffer(sizeof(ulong));

                var randomInteger = BitConverter.ToUInt64(_sharedBuffer, (int)_currentBufferIndex);

                var remainder = ulong.MaxValue % difference;

                var result = minimum + (randomInteger % difference);

                _currentBufferIndex += sizeof(ulong);

                return (long)result >= 0
                    ? (long)result
                    : (long)result * -1;
            }
        }


        public float GenerateFloat()
            => (float)GenerateDouble();

        public double GenerateDouble()
            => GenerateInt() / (uint.MaxValue + 1d);

        public string GenerateString(uint length)
        {
            const string validCharacters = STRING_CHARSET;
            return GenerateString(length, validCharacters);
        }

        public string GenerateString(int length)
        {
            if (length < 0)
                length = length * -1;

            return GenerateString((uint)length);
        }

        private string GenerateString(uint length, string validCharacters, bool removeDuplicates = true)
        {
            var validCharacterArray = validCharacters.ToCharArray();

            return GenerateString(length, validCharacterArray, removeDuplicates);
        }

        private string GenerateString(int length, string validCharacters, bool removeDuplicates = true)
        {
            if (length < 0)
                length = length * -1;

            return GenerateString((uint)length, validCharacters, removeDuplicates);
        }

        private string GenerateString(uint length, char[] validCharacters, bool removeDuplicates = true)
        {
            var distinctValidCharacterArray = validCharacters.Distinct().ToArray();

            if (distinctValidCharacterArray.Length <= 1)
                throw new ArgumentException(DETERMINABLE_OUTPUT_ERROR, nameof(validCharacters));

            if (length == 0)
                throw new ArgumentException(DETERMINABLE_OUTPUT_ERROR, nameof(length));

            if (removeDuplicates)
                validCharacters = distinctValidCharacterArray;

            var validCharactersLength = validCharacters.Length;

            if (256 % validCharactersLength != 0)
                throw new Exception(ENTROPY_COMPROMISED_WARNING);

            var stringBuilder = new StringBuilder();

            var tempLength = length;

            lock (this)
            {
                RequestBuffer(tempLength);

                while (length-- > 0)
                {
                    _prng.GetBytes(_sharedBuffer);
                    var index = BitConverter.ToUInt32(_sharedBuffer, (int)_currentBufferIndex);
                    stringBuilder.Append(validCharacters[(int)(index % (uint)validCharactersLength)]);
                }

                var result = stringBuilder.ToString();

                _currentBufferIndex += tempLength;

                return result;
            }
        }

        public string GenerateString(int length, char[] validCharacters, bool removeDuplicates = true)
        {
            if (length < 0)
                length = length * -1;

            return GenerateString((uint)length, validCharacters, removeDuplicates);
        }

        public void Dispose()
            => GC.SuppressFinalize(this);

    }
}
