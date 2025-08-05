# Temu Wordle Solver 🎯

A simple C# Wordle solver that simulates solving 5-letter Wordle puzzles using filtered random guessing and feedback from previous attempts. Not smart, just stubborn.

## 🧠 How It Works

1. Randomly guesses a word from the list.
2. Evaluates feedback (⬜ = not in word, 🟨 = wrong position, 🟩 = correct).
3. Filters the word list based on feedback.
4. Repeats until the correct word is found (or not in 6 tries).

## 💡 Key Features

- 📄 Uses a preloaded 5-letter word list
- 🌀 Guesses are random but filtered
- 📊 Tracks:
  - Total words attempted
  - Solved vs failed (within 6 tries)
  - Average guesses per word
  - Time per solve
  - Full guess distribution

## 🧪 Sample Output
```
========== Summary ==========
Total words attempted: 10000
Successfully solved: 10000 | 100%
Failed to solve within 6 guesses: 1800 | 18%
Average guesses per solved word: 4.28
Average solve time: 8 ms

Guess Distribution:
1 Guess -> 120 times
2 Guesses -> 1345 times
3 Guesses -> 3010 times
...
```
## 🚀 Getting Started

1. Clone the repo:
   ```bash
   git clone https://github.com/st10443220/Temu_Wordle_Solver.git
   ```
2. Open the project in Visual Studio.
3. Make sure your word list is present as an embedded resource (or adapt as needed).
4. Run Program.cs and watch it go brrr.

# 📂 Project Structure

- **Program.cs** – Simulation controller + statistics  
- **Solver.cs** – Random guessing logic with feedback filtering  
- **Config.cs** – Stores run-specific data  
- *(Future Implementation)* **StrategyEngine.cs** – For future smarter logic  

# 📌 Notes

- This isn't a "smart" solver (yet) — it's mostly random with filtering.  
- Expect some fails within 6 guesses due to luck.  

# 🛠 Future Plans

- 🔍 Add actual strategy for guess selection  
- 📈 Visualize stats with charts  
- 🎮 Build a playable Wordle mode
- 🎨 Create a more visual appealing app
    -> perhaps a .NET CORE MVC App for furthering my proficiency in it!

