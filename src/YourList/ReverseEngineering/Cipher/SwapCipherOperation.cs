using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Cipher
{
    internal class SwapCipherOperation : ICipherOperation
    {
        private readonly int _index;

        public SwapCipherOperation(int index)
        {
            _index = index;
        }

        public string Decipher(string input)
        {
            return input.SwapChars(0, _index);
        }

        public override string ToString()
        {
            return $"Swap ({_index})";
        }
    }
}