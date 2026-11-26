<!-- markdownlint-disable-file MD033 -->

# Technical Documentation

> **Purpose:** This document explains how the project is organised, how the team works with it, and how it can be maintained and evolved.
>
> This is practical production documentation, not a summary of the source code or the Game Design Document (GDD). Keep it useful to programmers, artists, designers, producers, sound designers, and any other team members.

## Document Information

- **Project:** Altershot
- **Last updated:** 20/11/2026
- **Project version / build:** Sprint 3
- **Repository:** [Github](https://github.com/ArthurSantosLuso/AP_Project_FPS)
- **Related documentation:** [Links, e.g. GDD, art bible, production plan]

## Table of Contents

1. [Team Organisation and Responsibilities](#1-team-organisation-and-responsibilities)
2. [Project Overview and Architecture](#2-project-overview-and-architecture)
3. [Onboarding and Development Environment Setup](#3-onboarding-and-development-environment-setup)
4. [Production Pipelines and Workflows](#4-production-pipelines-and-workflows)
5. [Conventions, Standards, and File Organisation](#5-conventions-standards-and-file-organisation)
6. [Tools, Testing, and Validation](#6-tools-testing-and-validation)
7. [Production Decisions, Current Limitations, and Known Risks](#7-production-decisions-current-limitations-and-known-risks)
8. [Project Maintenance and Evolution](#8-project-maintenance-and-evolution)

---

## 1. Team Organisation and Responsibilities

### 1.1 Team Structure
<!-- Describe the team structure and the areas of work covered by the project. -->
The team is composed of 5 offical members and 2 outsources. The work covers various parts of developing a game:

- Programming
- Art
- Game Design
- Sound Design
- Production
- Marketing

### 1.2 Roles and Responsibilities
<!-- Identify each team role and explain its responsibilities. Include programming, art, design, production, audio, QA, and other relevant roles. -->

The roles below describe the main responsibilities within the team. Depending on the task and the team's workload, members may collaborate across disciplines or contribute outside their primary role.

- **Programmer**
  - Maintain a well-structured and understandable codebase, making it straightforward to extend existing systems and add content such as enemies and effects.
  - Ensure that C# files and in-code elements (fields, methods, parameters, etc.) follow the project's naming conventions consistently.
  - Implement and maintain the systems defined by the design, including gameplay systems for the player and enemies, as well as manager systems.
  - Work with designers and artists to integrate gameplay requirements, assets, animations, and effects into the game.

- **Game Designer**
  - Imagine, plan, specify, and refine the game's mechanics and systems.
  - Define gameplay behaviour and parameters, such as player movement, animation timing, weapon fire rate, and other details that affect how each system works.
  - Create and develop the level designs for the game's scenarios.
  - Design and test puzzles, ensuring that their rules and solutions are understandable and work as intended.
  - Define enemy behaviours and communicate their requirements to the programming and art teams.
  - Playtest mechanics and levels, using observations and feedback to improve the gameplay experience.

- **Artist**
  - Create and prepare the visual assets required by the game, following the project's intended art direction.
  - Develop and maintain the visual identity, mood, and feel of the game in collaboration with the rest of the team.
  - Produce or contribute to assets such as environments, props, textures, lighting, visual effects, and user-interface elements, according to the division of work within the art team.
  - Prepare assets for integration and collaborate with programmers and designers to ensure that they work correctly and communicate the intended gameplay information.

- **Character Artist**
  - Focus on the visual creation and refinement of characters, including their appearance, animation, shape, materials, and visual details.
  - Prepare character assets for use in the game and coordinate with the programming work where technical requirements affect the final result.
  - This is currently a specialised responsibility handled by one team member; it is listed separately to distinguish character work from the broader art responsibilities.

- **Sound Designer**
  - Create, edit, and prepare sound effects that support gameplay, feedback, and the atmosphere of the game.
  - Identify the events and interactions that require audio feedback, and coordinate with designers and programmers on when and how sounds should play.
  - Import, organise, and validate sound assets in the project, checking that they are clear and correctly triggered in-game.

- **Music Designer**
  - Create or select the musical content that supports the game's atmosphere, pacing, and identity.
  - Coordinate music requirements with the game design and production work, including where music should start, stop, or change during gameplay.
  - Prepare and deliver music assets in a format suitable for integration, and review their implementation in-game.

- **Producer**
  - Coordinate the team's work, priorities, milestones, and deadlines.
  - Help maintain visibility over task progress, dependencies, and potential production risks.
  - Facilitate communication and handoffs between disciplines so that work can be planned and integrated effectively.
  - Keep production information organised and communicate changes to priorities or schedules to the team.

- **Quality Assurance (QA)**
  - Test gameplay, levels, assets, and builds to identify defects, inconsistencies, and usability issues.
  - Record bugs with clear reproduction steps and relevant context, and communicate findings to the responsible team members.
  - Verify fixes and perform regression checks to ensure that changes have not introduced new problems.

### 1.3 Ownership and Points of Contact
<!-- Indicate who is responsible for each major system, pipeline, or production area, and who should be contacted when help is needed. -->

#### Arthur Santos - Programmer

Responsible* for Game design, UI implementation, <u>some</u> Art work:

- Models
- Shaders
- VFX
- Particles.

...and Game systems:

- Bullets
- Damage Effects
- Game State
- Game Manager

*Responsible does not mean that other member have not done anything to that specific part of the project, just indicates that the member is the one in **charge** of that part.

#### Cátia Nascimento - Artist

Responsible* for Lights, VFX, Props and Textures.

Auxiliates in the creation of all artistic parts of the game, including:

- Game Mood and Feel
- Game Art Style
- Character Design
- Level Design
- Game UI

#### Frederico Carvalho - Programmer

#### Rafael Canhoto - Artist

#### Rafael Martins - Artist

### 1.4 Collaboration and Handoffs
<!-- Explain how work is passed between team members and how dependencies, reviews, and approvals are handled. -->

## 2. Project Overview and Architecture

### 2.1 Technical Overview
<!-- Give a concise, high-level explanation of the project from a technical perspective. Help a new team member understand the structure quickly. -->

### 2.2 High-Level Architecture Diagram
<!-- Add a diagram showing the main parts of the project and how they relate. -->

### 2.3 Main Systems and Subsystems
<!-- Describe the purpose and responsibility of each major system. Focus on how the project is organised; do not list every class or source file. -->

### 2.4 System Communication and Dependencies
<!-- Explain how the main systems communicate, what they depend on, and any important dependency constraints. -->

### 2.5 Main Game Flow (Technical Perspective)
<!-- Describe the main technical flow of the game, from startup and initialisation through the relevant gameplay and shutdown or transition stages. -->

### 2.6 Project Structure: Folders, Modules, and Resources
<!-- Show the main folder and module structure and explain what belongs in each important area. -->

### 2.7 Plugins, Packages, and External Dependencies
<!-- List the plugins, packages, libraries, or external services used and explain why each one is needed. -->

## 3. Onboarding and Development Environment Setup

### 3.1 Requirements and Prerequisites
<!-- List the required hardware, operating system, software, engine version, SDKs, accounts, and access permissions. -->

### 3.2 Accessing the Project and Tools
<!-- Explain how a new team member obtains repository access and accesses the tools and shared resources needed for their role. -->

### 3.3 Installing and Configuring the Environment
<!-- Provide the steps to install and configure the development environment, including required settings and dependencies. -->

### 3.4 Getting the Project
<!-- Explain how to clone, download, or otherwise obtain the project, including any required branches or submodules. -->

### 3.5 Running the Project
<!-- Give the steps required to open, build, and run the project for the first time. Include the expected result and common setup issues. -->

### 3.6 Finding Relevant Systems, Assets, and Tools
<!-- Explain where team members can find the systems, assets, editors, templates, and other resources relevant to their work. -->

### 3.7 Making and Integrating a First Change
<!-- Describe how a new team member can create or modify something in their area and integrate that work into the project. -->

### 3.8 Role-Specific Onboarding
<!-- Add practical onboarding notes for each relevant role. For example: programmers—run the project and locate systems; artists—find asset specifications and import a model; designers—edit gameplay values and configure a level; sound designers—import audio and connect it to game events; producers—find milestones, priorities, and task status. -->

## 4. Production Pipelines and Workflows

> A workflow describes the process for completing and integrating a task, not merely how to use a particular tool.

### 4.1 Asset Production and Integration Pipeline
<!-- Explain how assets are created, reviewed, prepared, imported, configured, and integrated into the game. -->

### 4.2 Creating and Integrating a New Enemy
<!-- Document the steps and handoffs between design, art, audio, programming, and QA, from the initial specification to validation in-game. -->

### 4.3 Creating a New Arena or Level
<!-- Explain the complete process for creating, configuring, testing, reviewing, and integrating a new level or arena. -->

### 4.4 Preparing and Importing Models, Textures, and Animations
<!-- Describe the workflow and required handoffs, formats, settings, and validation checks for these assets. -->

### 4.5 Adding Audio and Connecting Game Events
<!-- Explain how audio is prepared, imported, configured, connected to gameplay events, and checked in-game. -->

### 4.6 Creating and Configuring Items, Weapons, or Abilities
<!-- Describe how new gameplay content is specified, configured, implemented if necessary, tested, and integrated. -->

### 4.7 Changing Gameplay Values and Balancing
<!-- Explain where gameplay values are stored and how authorised team members can change them. State whether values are data-driven or hardcoded and describe the relevant workflow. -->

### 4.8 Preparing and Running Playtests
<!-- Explain how to prepare a playtest, select and distribute a build, collect observations, and record results. -->

### 4.9 Collecting and Processing Feedback
<!-- Describe how feedback is collected, documented, reviewed, prioritised, and turned into follow-up work. -->

### 4.10 Reporting and Reproducing Bugs
<!-- Specify the bug-reporting process and the information required to reproduce and investigate an issue. -->

### 4.11 Submitting Changes and Integrating Team Work
<!-- Explain the process for submitting changes, requesting reviews, resolving conflicts, and integrating work into the shared project. -->

### 4.12 Creating, Testing, and Distributing a Build
<!-- Document how to create a build, perform checks, identify the build, and distribute it to the intended recipients. -->

### 4.13 Deployment and Game Updates
<!-- Explain how a build is deployed or delivered and how updates are prepared and released. -->

## 5. Conventions, Standards, and File Organisation

### 5.1 Project Folder and File Structure
<!-- Document the agreed organisation for folders, files, scenes, source files, and assets. -->

### 5.2 Naming Conventions
<!-- Define naming rules for classes, variables, functions, scenes, models, textures, animations, audio, and other relevant resources. -->

The following conventions must be followed when writing or modifying C# code. Consistent naming makes the codebase easier to read, maintain, and extend.

- **Classes, properties, and methods** must use `PascalCase`.
  - <badge color="success">Correct</badge> `PlayerController.cs`, `HealPlayer()`, `PlayerHealth`
  - <badge color="danger">Incorrect</badge> `Player_Controller.cs`, `playercontroller.cs`, `playerController.cs`, `healplayer()`
- **Class and file names** must match: the name of a class must be the same as its `.cs` file name.
  - Example: the class `PlayerController` must be declared in `PlayerController.cs`.
- **Local variables, method parameters, and fields** must use `camelCase`.
  - <badge color="success">Correct</badge> `float playerMoveSpeed`
  - <badge color="danger">Incorrect</badge> `float PlayerMoveSpeed`, `float player_move_speed`
- **Private fields** must begin with an underscore (`_`), followed by a `camelCase` name.
  - Example: `private int _playerHealth;`
- **Constants** must use uppercase letters, with words separated by underscores (`UPPER_SNAKE_CASE`).
  - <badge color="success">Correct</badge> `const int PI_NUMBER`
- **Interfaces** must begin with an uppercase `I`, followed by a `PascalCase` name.
  - Examples: `IConsumable`, `IInteractable`
- **Enums and enum values** must use `PascalCase`.
  - <badge color="success">Correct</badge> `PlayerState` and `PlayerState.Idle`
  - <badge color="danger">Incorrect</badge> `player_state`, `PLAYER_STATE`
- **Classes that inherit from Unity's `MonoBehaviour`** must follow the same `PascalCase` naming convention as other classes.
  - Examples: `PlayerController`, `EnemySpawner`
- **Unity event methods** (such as `Awake()`, `Start()`, `Update()`, and `OnTriggerEnter()`) must retain their original Unity naming and capitalisation.
  - <badge color="success">Correct</badge> `private void Awake()`
  - <badge color="danger">Incorrect</badge> `private void awake()`
- **Fields exposed in the Unity Inspector** should use `[SerializeField]` rather than being made public without a specific reason.
  - <badge color="success">Correct</badge> `[SerializeField] private float _moveSpeed`
  - <badge color="danger">Incorrect</badge> `public float moveSpeed`
- **References to Unity components** should be stored in private fields whenever other classes do not need direct access to them.
  - <badge color="success">Correct</badge> `[SerializeField] private Rigidbody _rigidbody`
  - <badge color="danger">Incorrect</badge> `public Rigidbody rigidbody`
- **Public properties** should be used when other classes need access to a value without exposing the underlying field directly.
  - Example: `public int PlayerHealth => _playerHealth;`
- **Methods** must have names that clearly describe the action they perform.
  - <badge color="success">Correct</badge> `TakeDamage()`, `UpdatePlayerHealth()`
  - <badge color="danger">Incorrect</badge> `DoThing()`, `Handle()`
- **Avoid unnecessary abbreviations** in class, method, and variable names.
  - <badge color="success">Correct</badge> `playerHealth`, `movementSpeed`
  - <badge color="danger">Incorrect</badge> `pHealth`, `movSpd`
- **Each class should have one clear responsibility.** Avoid combining unrelated functionality in a single class.
  - <badge color="success">Correct</badge> `PlayerController` handles player movement.
  - <badge color="danger">Incorrect</badge> `PlayerController` handles movement, UI, audio, inventory, and the save system.
- **Use `var` only when the type is obvious from the context.**
  - <badge color="success">Correct</badge> `var player = GetComponent<PlayerController>();`
  - <badge color="danger">Incorrect</badge> `var playerHealth = 100;` when the intended type is not clear.

### 5.3 Asset Formats and Technical Specifications
<!-- Specify required formats, resolutions, dimensions, scales, orientations, limits, import settings, and other asset requirements. -->

### 5.4 Code and Design Standards
<!-- Describe the coding standards, design patterns, architectural rules, and reusable approaches the team should follow. -->

### 5.5 Reusable Resources and Templates
<!-- Explain how and when to use project-specific prefabs, templates, Scriptable Objects, presets, or equivalent reusable resources. -->

### 5.6 Version Control Rules
<!-- Document repository usage, branch strategy, commit message conventions, merge or pull request procedures, and rules for handling shared files. -->

## 6. Tools, Testing, and Validation

### 6.1 Development and Production Tools
<!-- List the tools used by the different disciplines and explain their purpose in the project workflow. -->

### 6.2 Tool and Plugin Configuration
<!-- Record important configuration requirements, versions, integrations, and known setup constraints. -->

### 6.3 Testing Strategy
<!-- Explain what is tested, at which stages, by whom, and how test results are recorded. -->

### 6.4 Validation Checklists
<!-- Add the checks required before integrating assets, systems, levels, audio, or other deliverables. -->

### 6.5 Performance and Compatibility Checks
<!-- Describe how performance, target-platform compatibility, and other technical requirements are evaluated. -->

### 6.6 Quality Assurance and Issue Tracking
<!-- Explain the QA process, issue tracking conventions, and how fixes are verified. -->

## 7. Production Decisions, Current Limitations, and Known Risks

### 7.1 Important Production Decisions
<!-- Record significant technical and production decisions, including architecture, programming patterns, art pipelines, mechanics implementation, tools, and plugins. Explain the reasoning rather than only stating what was chosen. -->

#### Decision Record Template

- **Context:** [What problem or requirement led to this decision?]
- **Decision:** [What was chosen?]
- **Justification:** [Why was this option selected?]
- **Alternatives considered:** [Which alternatives were considered, and why were they rejected?]
- **Consequences and trade-offs:** [What benefits, limitations, or compromises resulted?]

### 7.2 Current Technical Limitations
<!-- Describe known limitations in systems, tools, pipelines, assets, or content. Distinguish temporary solutions from intended long-term solutions. -->

### 7.3 Known Risks and Technical Debt
<!-- Identify unstable systems, performance issues, refactoring needs, incomplete solutions, dependencies, and other risks that could affect the project. This section is an analysis, not a replacement for the task-tracking system. -->

#### Risk Record Template

- **Problem:** [Describe the issue.]
- **Impact:** [Explain what the issue affects or limits.]
- **Priority:** [High / Medium / Low]
- **Possible solution:** [Describe a possible mitigation or fix.]
- **Responsible person or team:** [Identify who owns or monitors the issue.]

### 7.4 External and Internal Dependencies
<!-- Record dependencies that may block work or affect delivery, including ownership and relevant constraints. -->

## 8. Project Maintenance and Evolution

### 8.1 Maintaining Project Systems and Content
<!-- Explain how existing systems, assets, tools, and pipelines should be maintained as the project changes. -->

### 8.2 Updating and Maintaining Documentation
<!-- State who is responsible for each documentation area and when updates are required. Update documentation whenever systems, tools, conventions, or workflows change. -->

### 8.3 Versioning and Change History
<!-- Explain how documentation changes are tracked and how the documentation relates to project versions and releases. -->

### 8.4 Periodic Review
<!-- Describe how the team checks that the documentation remains accurate, useful, consistent, and easy to navigate. -->

### 8.5 Future Improvements and Evolution
<!-- Record relevant areas for future improvement and explain how the project can be maintained and extended sustainably. -->

---

## Appendix

### A. Glossary
<!-- Define project-specific technical terms, abbreviations, and naming used throughout this documentation. -->

### B. Reference Links
<!-- Add links to relevant internal resources, the repository, the GDD, art bible, production plan, and tool documentation. Ensure essential documentation can still be consulted in the delivered project snapshot. -->

### C. Supporting Diagrams and Images
<!-- Include or link to architecture diagrams, pipeline diagrams, folder-structure diagrams, and other resources needed to understand the project. -->

### D. Document Change Log
<!-- Record significant documentation updates, including date, author, and a short description of the change. -->
