# Dodgeball_Project

## Overview
Welcome to our AI Dodgeball repo.
## Features

## Document Overview
- **Assets**
  - Prefabs: Folder containing all generated objects.
  - Scenes/Dodgeball.unity: The environment used to train the agents.
  - Scripts:
    - DodgeballAgent.cs: Logic that controls how the agent interacts with the environment.
    - DodgeballController.cs: Logic that controls how the ball reacts to events from interacting with the agent and environment.
    - DodgeballEnvController.cs: Logic that controls how the environment reacts to events that occur from agent and ball actions
    - All Dodgeball.onx files: Brain of the agents after being trained, the number after dash corresponds to the number of steps the brain has been trained on.
  - config/Dodgeball.yaml: Configurations of hyperparameters to train agents
- **Packages**: Contains packages required by the unity project
- **ProjectSettings**: Contains configurations for the unity project settings
  
## Getting Started

### Prerequisites
## Installation
Follow the Installation Guide provided by Unity Technologies for ML-AGENTS https://github.com/Unity-Technologies/ml-agents/blob/develop/docs/Installation.md
- Install Unity Hub
- Install Unity
- Install ML-Agents
- Clone this repo
- Add Dodgeball_Project from disk in Unity Hub
- Open Dodgeball_Project from Unity Hub
- Press the play button
