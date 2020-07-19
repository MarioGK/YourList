using YourList.Internal.Extensions;

namespace YourList.ReverseEngineering.Cipher
{
    internal class ReverseCipherOperation : ICipherOperation
    {
        public string Decipher(string input)
        {
            return input.Reverse();
        }

        public override string ToString()
        {
            return "Reverse";
        }
    }
}