using System.Diagnostics;

namespace Temu_Wordle_Solver
{
    internal class Solver
    {
        private readonly Config _config;
        private readonly string[] _wordList = Properties.Resources.WordList.Split('\n');
        private readonly List<string> _usedWords = new();
        private readonly Random _random = new();
        private readonly List<string> _bestStartingWords = new List<string>
        {
            "slate",
            "crane",
            "soare",
            "raise",
            "arise",
            "audio",
            "adieu",
            "react",
        };

        private Stopwatch _stopwatch = new Stopwatch();
        private int _totalWordsTried = 0;

        public SolverStats Stats { get; private set; } = new SolverStats();

        public Solver(Config config)
        {
            _config = config;
        }

        public void SolveWordle()
        {
            bool failedOnSixth = false;
            PrintHeader("TEMU WORDLE SOLVER");
            string actualWord = _config.ActualWord.ToLower();
            bool solved = false;
            _totalWordsTried = 0;
            _stopwatch.Restart();

            char[] greens = new char[5];
            HashSet<char> yellowLetters = new();
            HashSet<char> greyLetters = new();
            Dictionary<int, HashSet<char>> positionExclusions = new();

            Console.WriteLine($"Actual word: {actualWord.ToUpper()}");
            Console.WriteLine(new string('=', 40));

            Boolean isFirstGuess = true;

            while (!solved)
            {
                string word;
                if (isFirstGuess)
                {
                    word = _bestStartingWords[_random.Next(_bestStartingWords.Count)];
                    isFirstGuess = false;
                }
                else
                {
                    word = GetRandomWord(greens, yellowLetters, greyLetters, positionExclusions);
                }

                ThinkingAnimation();
                _totalWordsTried++;

                var feedback = GetFeedback(word, actualWord);

                for (int i = 0; i < 5; i++)
                {
                    char guessChar = word[i];

                    switch (feedback[i])
                    {
                        case LetterState.Green:
                            greens[i] = guessChar;
                            yellowLetters.Remove(guessChar);
                            break;

                        case LetterState.Yellow:
                            yellowLetters.Add(guessChar);
                            if (!positionExclusions.ContainsKey(i))
                                positionExclusions[i] = new HashSet<char>();
                            positionExclusions[i].Add(guessChar);
                            break;

                        case LetterState.Grey:
                            if (!IsLetterInWord(guessChar, actualWord, word, feedback))
                            {
                                greyLetters.Add(guessChar);
                            }
                            else
                            {
                                if (!positionExclusions.ContainsKey(i))
                                    positionExclusions[i] = new HashSet<char>();
                                positionExclusions[i].Add(guessChar);
                            }
                            break;
                    }
                }

                Display(feedback, word, _totalWordsTried);

                if (word == actualWord)
                {
                    _stopwatch.Stop();
                    Stats.TotalGuesses = _usedWords.Count;
                    Stats.ElapsedMilliseconds = _stopwatch.ElapsedMilliseconds;

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("🎉 Word solved!");
                    Console.ResetColor();
                    PrintStats();
                    Console.WriteLine(new string('=', 40));
                    solved = true;
                    if (_config.EnableSleep)
                        Thread.Sleep(1500);
                    break;
                }
                else if (_totalWordsTried == 6)
                {
                    failedOnSixth = true;
                }
            }

            Stats.FailedOnSixth = failedOnSixth;
        }

        #region Display & UI

        private void PrintHeader(string title)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(new string('=', 40));
            Console.WriteLine(title.PadLeft((40 + title.Length) / 2).PadRight(40));
            Console.WriteLine(new string('=', 40));
            Console.ResetColor();
        }

        private void PrintStats()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"Total guesses: {Stats.TotalGuesses}");
            Console.WriteLine($"Time taken: {Stats.ElapsedMilliseconds} ms");
            Console.ResetColor();
        }

        private void ThinkingAnimation()
        {
            if (!_config.EnableSleep)
                return;

            string thinkingText = "Thinking";
            Console.Write(thinkingText);
            for (int i = 0; i < 3; i++)
            {
                Thread.Sleep(200);
                Console.Write(".");
            }
            Thread.Sleep(200);

            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, currentLineCursor - 0);
            Console.Write(new string(' ', thinkingText.Length + 3));
            Console.SetCursorPosition(0, currentLineCursor);
        }

        public void Display(LetterState[] feedback, string word, int attempt)
        {
            Console.WriteLine($"Guess {attempt}:");
            for (int i = 0; i < 5; i++)
            {
                switch (feedback[i])
                {
                    case LetterState.Green:
                        Console.BackgroundColor = ConsoleColor.Green;
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case LetterState.Yellow:
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Black;
                        break;
                    case LetterState.Grey:
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                Console.Write($" {char.ToUpper(word[i])} ");
                Console.ResetColor();
                Console.Write(" ");
            }
            Console.WriteLine("\n");
        }

        #endregion

        #region Solver Logic

        private bool IsLetterInWord(
            char letter,
            string actualWord,
            string guessWord,
            LetterState[] feedback
        )
        {
            for (int i = 0; i < 5; i++)
            {
                if (
                    guessWord[i] == letter
                    && (feedback[i] == LetterState.Green || feedback[i] == LetterState.Yellow)
                )
                {
                    return true;
                }
            }
            return false;
        }

        private LetterState[] GetFeedback(string guess, string actual)
        {
            LetterState[] feedback = new LetterState[5];
            Dictionary<char, int> actualCounts = CountLetters(actual);
            Dictionary<char, int> usedCounts = new();

            // Green
            for (int i = 0; i < 5; i++)
            {
                if (guess[i] == actual[i])
                {
                    feedback[i] = LetterState.Green;
                    usedCounts[guess[i]] = usedCounts.GetValueOrDefault(guess[i], 0) + 1;
                }
            }

            // Yellow, Grey
            for (int i = 0; i < 5; i++)
            {
                if (feedback[i] == LetterState.Green)
                    continue;

                char guessChar = guess[i];
                int totalInActual = actualCounts.GetValueOrDefault(guessChar, 0);
                int alreadyUsed = usedCounts.GetValueOrDefault(guessChar, 0);

                if (totalInActual > alreadyUsed && actual.Contains(guessChar))
                {
                    feedback[i] = LetterState.Yellow;
                    usedCounts[guessChar] = alreadyUsed + 1;
                }
                else
                {
                    feedback[i] = LetterState.Grey;
                }
            }

            return feedback;
        }

        public string GetRandomWord(
            char[] greens,
            HashSet<char> yellowLetters,
            HashSet<char> greyLetters,
            Dictionary<int, HashSet<char>> positionExclusions
        )
        {
            var filteredWords = _wordList
                .Select(w => w.Trim().ToLower())
                .Where(w =>
                    w.Length == 5
                    && !_usedWords.Contains(w)
                    && MatchesConstraints(w, greens, yellowLetters, greyLetters, positionExclusions)
                )
                .ToList();

            if (filteredWords.Count == 0)
            {
                Console.WriteLine("DEBUG: No valid words found!");
                Console.WriteLine(
                    $"Greens: {string.Join("", greens.Select(c => c == '\0' ? '_' : c))}"
                );
                Console.WriteLine($"Yellow letters: {string.Join(", ", yellowLetters)}");
                Console.WriteLine($"Grey letters: {string.Join(", ", greyLetters)}");
                Console.WriteLine($"Used words: {_usedWords.Count}");
                throw new Exception("Temu Solver Got Stuck!");
            }

            string word = filteredWords[_random.Next(filteredWords.Count)];
            _usedWords.Add(word);
            return word;
        }

        private bool MatchesConstraints(
            string word,
            char[] greens,
            HashSet<char> yellowLetters,
            HashSet<char> greyLetters,
            Dictionary<int, HashSet<char>> positionExclusions
        )
        {
            for (int i = 0; i < 5; i++)
            {
                if (greens[i] != '\0' && word[i] != greens[i])
                    return false;
            }

            foreach (char yellowChar in yellowLetters)
            {
                if (!word.Contains(yellowChar))
                    return false;
            }

            foreach (char greyChar in greyLetters)
            {
                if (word.Contains(greyChar))
                    return false;
            }

            foreach (var kvp in positionExclusions)
            {
                int position = kvp.Key;
                foreach (char excludedChar in kvp.Value)
                {
                    if (word[position] == excludedChar)
                        return false;
                }
            }

            return true;
        }

        private static Dictionary<char, int> CountLetters(string word)
        {
            Dictionary<char, int> letterCounts = new();
            foreach (char c in word)
            {
                letterCounts[c] = letterCounts.GetValueOrDefault(c, 0) + 1;
            }
            return letterCounts;
        }

        #endregion
    }
}
