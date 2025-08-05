namespace Temu_Wordle_Solver
{
    internal class Program
    {
        private static Config config;
        private static Solver solver;
        private static Random random = new Random();
        private static string[] WordList = Properties.Resources.WordList.Split("\n").Select(w => w.Trim().ToLower()).ToArray();
        private static int failedWithinSix = 0;
        private static int totalWords = WordList.Length - 1;
        private static Dictionary<int, int> guessDistribution = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            int solvedCount = 0;
            int totalGuesses = 0;
            long totalTimeMs = 0;

            var wordsToTry = WordList.OrderBy(_ => random.Next()).Distinct().Take(totalWords).ToList();

            for (int i = 0; i < wordsToTry.Count; i++)
            {
                try
                {
                    string word = wordsToTry[i];
                    config = new Config(word, false);
                    solver = new Solver(config);
                    Console.WriteLine(
                        "\U00002B1C = Not in word   \U0001F7E8 = Wrong position   \U0001F7E9 = Correct\n"
                    );
                    solver.SolveWordle();
                    solvedCount++;
                    int guesses = solver.Stats.TotalGuesses;
                    if (guesses == 0)
                    {
                        Console.WriteLine($"Warning: 0 guesses for word '{config.ActualWord}'.");
                        continue;
                    }

                    totalGuesses += guesses;

                    if (!guessDistribution.ContainsKey(guesses))
                        guessDistribution[guesses] = 0;
                    guessDistribution[guesses]++;

                    totalTimeMs += solver.Stats.ElapsedMilliseconds;
                    if (solver.Stats.FailedOnSixth)
                        failedWithinSix++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to solve: {config.ActualWord.ToUpper()}");
                    Console.WriteLine($"Reason: {ex.Message}");
                    Console.WriteLine("=====================");
                }
            }

            // Summary
            Console.WriteLine("\n========== Summary ==========");
            Console.WriteLine($"Total words attempted: \t\t\t{totalWords}");
            Console.WriteLine($"Successfully solved: \t\t\t{solvedCount} | {(solvedCount / (double)totalWords) * 100:F0}%");
            Console.WriteLine($"Failed to solve within 6 guesses: \t{failedWithinSix} | {(int)Math.Round((double)(100 * failedWithinSix) / totalWords)}%");
            Console.WriteLine($"Average guesses per solved word: \t{(solvedCount == 0 ? 0 : (double)totalGuesses / solvedCount):F2}");
            Console.WriteLine($"Average solve time: \t\t\t{(solvedCount == 0 ? 0 : (double)totalTimeMs / solvedCount):F0} ms");
            Console.WriteLine("\nGuess Distribution:");
            foreach (var kvp in guessDistribution.OrderBy(k => k.Key))
            {
                Console.WriteLine($"{kvp.Key} Guess{(kvp.Key == 1 ? "" : "es")} -> {kvp.Value} time{(kvp.Value == 1 ? "" : "s")}");
            }
            Console.WriteLine("=============================");
        }
    }
}
