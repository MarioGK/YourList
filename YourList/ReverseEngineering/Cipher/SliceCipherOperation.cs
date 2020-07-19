namespace YourList.ReverseEngineering.Cipher
{
    internal class SliceCipherOperation : ICipherOperation
    {
        private readonly int _index;

        public SliceCipherOperation(int index)
        {
            _index = index;
        }

        public string Decipher(string input)
        {
            return input.Substring(_index);
        }

        public override string ToString()
        {
            return $"Slice ({_index})";
        }
    }
}