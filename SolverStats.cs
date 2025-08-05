namespace Temu_Wordle_Solver
{
    public class SolverStats
    {
        public int TotalGuesses { get; set; }
        public long ElapsedMilliseconds { get; set; }
        public bool FailedOnSixth { get; set; }
    }
}