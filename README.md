# Connect Four — Blazor Web App (CSE 325 Week 04)

An interactive Connect Four game built with **.NET 10 Blazor Web App** (Interactive Server render mode) following the Microsoft Learn *Build a Connect Four game with Blazor* module, extended with custom gameplay and accessibility features.

## Core Features (Microsoft Learn Module)
- **7×6 Interactive Board (`Board.razor` & `Board.razor.css`)**: CSS-isolated board with circular cutouts and smooth gravity-bounce drop animations (`drop1`–`drop6`).
- **Game State Engine (`GameState.cs`)**: Tracks turns, board occupancy, full-column validation, and checks all 69 horizontal, vertical, and diagonal four-in-a-row win conditions as well as ties.
- **Parameterized Styling**: `Board` accepts `System.Drawing.Color` parameters (`BoardColor`, `Player1Color`, `Player2Color`) from `Home.razor`.

## Additional Features Implemented
1. **Consecutive Win Streak & Scoreboard Tracker**: Tracks total wins for Player 1 and Player 2, ties, current consecutive win streak (with a streak highlight badge), and best streak across rounds, plus a separate **Reset Stats** control.
2. **Interactive Piece & Board Color Customizer**: Live color pickers for Player 1, Player 2, and Board colors, duplicate-color warning, and four one-click accessible theme presets (*Classic*, *Neon Dark*, *High Contrast*, *Berry Mint*).
3. **Live Move History & Winning Piece Highlight**: Displays an ordered list of every move made during the current game (move number, player color swatch, column, and row) and highlights the winning four-in-a-row pieces on the board.
4. **Keyboard Navigation & Accessibility**: Full keyboard play (`←`/`→` to aim, `Enter`/`Space`/`↓` to drop, `1`–`7` for direct column drop, `R` to reset), real `<button>` controls, `aria-label` descriptions, and `aria-live="polite"` status announcements.

## Running Locally
```bash
dotnet run
```
Then open the URL printed in the terminal (e.g., `http://localhost:5123`).
