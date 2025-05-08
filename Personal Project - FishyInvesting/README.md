## 🐟 Automated Stock Trading Personal Project

**Fishy Investing** is my personal investing platform that I’ve been developing in collaboration with [Ann Stone](mailto:ann@thestones.me) since the summer of 2024.

We currently host the platform on [Kamatera](https://www.kamatera.com/) and manage our development backlog through Git.

## Investment Platform Architecture Overview

The trading system is structured into **seven distinct microservices**, each corresponding to a core aspect of the stock trading lifecycle. This modular design enhances scalability, maintainability, and clarity across the platform.

### Microservices Breakdown

#### 1. `WebApi.Brokers.CharlesSchwab`
Handles all interactions with the Charles Schwab brokerage API, including placing and managing trades.

#### 2. `WebApi.DB.Access`
Provides CRUD operations for all major database tables. Primarily used by the frontend to access and manipulate data.

#### 3. `WebApi.GetData`
Maintains and retrieves historical and current market data. Supports both backtesting and live strategy execution.

#### 4. `WebApi.MakeTrade`
Converts strategy signals (e.g., in/out of market) into optimized trades and places them through the Charles Schwab service.

#### 5. `WebApi.Scheduler`
Orchestrates the execution of all active trading strategies at their scheduled intervals.

#### 6. `WebApi.StrategyService`
Executes specific trading strategies, triggered by the scheduler or manually for testing and development.

#### 7. `WebApi.UpdateTrades`
Monitors and updates the status of all incomplete or pending trades, ensuring execution integrity.

---

### Roadmap for Future Enhancements

While this is the current architecture, I am excited to continue growing and improving the architecture. On my list of improvements is:
- **Implementing multithreading** for concurrent strategy execution, reducing latency and enabling parallelism.
- **Expanding data timeframes** to include broader historical context for both analysis and trade decisions.
- **Adjusting for corporate actions**, such as stock splits and reverse stock splits, to improve data accuracy and consistency.

### ⚠️ Reminder

Please **do not copy, share, or redistribute** any part of this project.

- Only **one strategy** is included here; the others have been removed.
- Any **secrets or sensitive data** have been excluded from this public version.
- If you happen to come across something that seems like it shouldn’t be included, please reach out and let me know. I’d really appreciate it!

