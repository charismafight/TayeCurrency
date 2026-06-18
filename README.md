# ⛏️ Taye's Craft

> A Minecraft-style behavior incentive system that turns real-life growth into an adventure for kids

---

## 📖 The Story Behind Taye's Craft

Taye's Craft is a custom-built growth incentive application, created by a father for his son, Taye. It transforms daily routines—studying, life habits, and behaviors—into a Minecraft-style adventure journey.

In this world, every good deed is an "achievement unlocked," and every reward star is a precious "emerald." Taye can save up his emeralds to exchange for game time, toys, or snacks. The Web Dashboard serves as his "adventure map"—a place where he can clearly see his growth trajectory, achievement progress, and the next challenge waiting to be conquered.

**This is more than just a tracking tool; it is a unique gift of growing up.**

---

## 🎯 Project Goals

- **Incentivize positive behavior**: Transform abstract behavioral expectations into a visual achievement system, giving Taye a sense of accomplishment through "leveling up"
- **Cultivate self-management skills**: Through the concept of "emerald income and expenses," help Taye understand the relationship between effort and reward, and learn planning and delayed gratification
- **Build a growth archive**: Document Taye's development journey completely, including every improvement and every memorable moment
- **Enhance parent-child interaction**: Through the "Crafting Table" exchange mechanism and "proof photo" uploads, create more opportunities for communication and shared reflection

---

## 🏗️ Technical Architecture

The project is built on the .NET 9 technology stack, with a layered architecture that unifies the MAUI client and Web Dashboard.

| Project                | Description                                                          | Tech Stack                      |
| ---------------------- | -------------------------------------------------------------------- | ------------------------------- |
| **Taye.MAUI**          | Pad-based recording app for daily behavior logging                   | .NET MAUI, PGsql                |
| **Taye.Shared**        | Shared class library containing core data models and business logic  | .NET 9 Class Library            |
| **Taye.WebAPI**        | Backend API service providing data aggregation and interfaces        | ASP.NET Core 9 Web API, EF Core |
| **Taye.Web** (Planned) | Web dashboard frontend presenting Minecraft-style data visualization | Blazor WebAssembly / React      |

### Shared Core (Taye.Shared)

- Data Models
- Business Logic
- Contracts / Interfaces

### Taye.MAUI (Recorder)

- Pad-based behavior logging
- Emerald transaction recording
- Photo evidence upload
- Real-time balance checking

### Taye.WebAPI (Backend)

- RESTful API endpoints
- Data aggregation
- Database access via EF Core
- CORS enabled for web frontend

### Taye.Web (Dashboard)

- Minecraft-themed UI
- Data visualization with charts
- Achievement tracking
- Exchange request management

---

## 🎮 Core Functional Modules

### 📱 MAUI Recorder (Taye.MAUI)

Taye's daily behaviors are recorded through the Pad app—each entry is an "adventure log":

- **Behavior Logging**: Select behavior type (Study / Life / School / Gaming / Shopping), record specific events
- **Emerald Transactions**: Reward or penalty amounts are automatically calculated based on predefined rules
- **Photo Evidence**: Upload proof photos (homework, awards, clean plates, etc.) to validate every reward
- **Real-time Balance**: Check current emerald balance at any time to know "how rich you are"

### 🌐 Web Dashboard (Taye.Web - Planned)

A Minecraft-themed "adventure map" with five core modules:

#### 1. Hero Profile — "Who I Am, How Strong I've Become"

- Avatar, player name, current emerald balance
- Weekly income/expense badges
- Achievement rank (e.g., "Dragon Slayer")
- Experience bar showing progress to the next level

#### 2. Achievement Wall — "My Glorious Weekly Records"

Achievement challenges are displayed as a grid of "quests," organized by category:

- **Academic**: "Perfect Score Hunter" (Test 100), "Dictation Master", "Homework Elite"
- **School**: "Campus Star" (Teacher's Commendation)
- **Life**: "Early Bird" (Bed by 21:30), "Clean Plate Champion"

Each achievement card shows: icon, name, weekly completion count, and emeralds earned.

#### 3. Survival Challenges — "Quests to Conquer"

Displays areas for improvement as "challenges" rather than "mistakes," with a positive, motivational tone:

- **Classroom Focus**: Reduce school behavior violations
- **Homework Discipline**: Reduce incomplete assignments
- **Backpack Management**: Reduce forgotten supplies
- **Hygiene Routine**: Reduce missed face-washing / flushing
- **Bedtime Prep**: Reduce late bedtime incidents

Each challenge shows weekly frequency and consecutive streak days.

#### 4. Adventure Timeline — "Every Step of the Journey"

A chronological log of recent behavioral records:

- Time, event icon, event name, emerald change (+N / -N)
- Photo evidence thumbnails (click to enlarge)
- Filter by "All / Good Deeds / Challenges"

#### 5. Crafting Table — "Exchange Your Loot"

Convert emeralds into real-world rewards using a "crafting recipe" interface:

| Item            |   Cost    |
| :-------------- | :-------: |
| Game Time  (5m) | 1 emerald |
| Toy (￥1)        | 1 emerald |
| Snack (￥1)      | 1 emerald |

- Displays recent exchange requests and their status (Pending / Approved / Completed)
- Includes a "Wish List" for Taye to add items he's saving up for

---

## 🛠️ Behavior Scoring Rules

The following table shows how daily behaviors are mapped to emerald rewards or penalties:

| Category | Behavior                            | Emerald Change |
| :------- | :---------------------------------- | :------------: |
| Academic | Test score 100                      |       +3       |
| Academic | Chinese Dictation A+                |       +2       |
| Academic | Teacher's praise in homework        |       +2       |
| Academic | Complete Khan Academy unit          |       +2       |
| Academic | Homework grade A- or below          |       -1       |
| School   | Teacher commends school performance |       +3       |
| School   | Violation/misconduct at school      |       -2       |
| Life     | Bed by 21:30 (good)                 |       +2       |
| Life     | Not in bed by 22:00                 |       -1       |
| Life     | Clean plate at meals                |       +1       |
| Life     | Forgot to wash face / flush         |       -1       |
| Play     | Gaming (per 5 minutes)              |       -1       |
| Play     | Toy purchase (per 1 yuan)           |       -1       |
| Play     | Snack purchase (per 1 yuan)         |       -1       |

> Note: Custom events can be added, such as "Hire Dad to Read (per chapter)" for +1 emerald, making everyday moments part of the adventure.

---

## 🚀 Roadmap

- [x] **Phase 1**: MAUI recording app with behavior logging, photo upload, and balance tracking
- [x] **Phase 2**: Shared library and Web API foundation
- [ ] **Phase 3**: Web Dashboard with Minecraft-themed visualization (in progress)
- [ ] **Phase 4**: Achievement system with unlock animations and notifications

---

## 💡 Philosophy

> *"Every child deserves to be the hero of their own story. Taye's Craft turns the ordinary moments of growing up—homework, chores, good habits—into a grand adventure, one emerald at a time."*

---

## 📄 License

This project is for personal and educational use.

---

Made with ❤️ for Taye
