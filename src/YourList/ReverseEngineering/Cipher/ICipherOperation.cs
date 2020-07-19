using System.Collections.Generic;
using System.Linq;

namespace YourList.ReverseEngineering.Cipher
{
    internal interface ICipherOperation
    {
        string Decipher(string input);
    }

    internal static class CipherOperationExtensions
    {
        public static string Decipher(this IEnumerable<ICipherOperation> operations, string input)
        {
            return operations.Aggregate(input, (acc, op) => op.Decipher(acc));
        }
    }
}