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
