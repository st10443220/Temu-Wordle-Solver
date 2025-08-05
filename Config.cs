namespace Temu_Wordle_Solver
{
    internal class Config
    {
        private string? _actualWord;
        private int _guessCount;
        public bool EnableSleep { get; set; } = true;
        public string ActualWord
        {
            get { return _actualWord!; }
            set { _actualWord = value; }
        }
        public int GuessCount
        {
            get { return _guessCount; }
            set { _guessCount = value; }
        }
        public Config(string actualWord, Boolean enableSleep)
        {
            _actualWord = actualWord;
            EnableSleep = enableSleep;
        }
    }
}