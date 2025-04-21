# Reinforcement Learning for Simplified Pacman 🟡👻

## Overview
This project implements **Q-learning** and **SARSA** reinforcement learning algorithms on a simplified version of the classic Pacman game. The goal is for the agent (Pacman) to reach an exit while avoiding ghosts and collecting edible dots for bonus rewards.

This assignment showcases active reinforcement learning techniques in a non-deterministic grid world environment.

---
## 🔧 Features & Highlights

### 🎯 Environment
- Grid-based world with static walls, ghosts, edible dots, and exit states
- Stochastic transitions: 90% intended action, 10% opposite action
- Reward shaping:
  - Exit: `+1` (terminal)
  - Ghost: `-1` (terminal)
  - Dots: `+d` once, then treated as empty
  - Empty: `−0.04` default

### 🤖 Agent Capabilities
- **Q-learning**: Off-policy temporal difference control
- **SARSA**: On-policy temporal difference control
- ε-greedy action selection for exploration/exploitation balance
- Maintains Q-tables and visitation counts per state-action pair

### 📁 File Breakdown
- `activeRL.py`: Core logic and RL algorithm implementation
- `visualize_run.py`: Step-by-step visual simulation of learned policy
- `requirements.txt`: Python dependencies for environment setup

---

## 🧠 Skills Demonstrated

- **Reinforcement Learning Algorithms**: Implemented Q-learning and SARSA from scratch
- **Stochastic Environments**: Handled non-deterministic transitions in policy training
- **Value Function Approximation**: Learned state-action values over episodes
- **Debugging & Testing**: Wrote and ran custom test cases to validate training behavior
- **Modular Design**: Used clean class abstractions for `State`, `Grid`, and `RLAgent`

---

## 🖥️ How to Run

### Training
```bash
python activeRL.py test1.txt

