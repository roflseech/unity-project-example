# Survival Game

This is showcasing project, that demonstrates production-ready practices for Unity game development, including project structure and architecture, asset management, config management, various domains inter-communications (there is even compute shader cloud system!)

## Who might be interested in this project:

- Beginners who would like to see an example of well-structured Unity project
- Experienced developers who want to see new ideas

## Key Features

- **Domain-split architecture**
- **Model-View-Presenter** for UI
- **Component-based approach** for gameplay
- **TypeScript configs**
- **Code-Generation**
- **Compute shaders**

## Technologies

- **VContainer**
- **UniRX**
- **UniTask**
- **Addressables**
- **ZString**
- **Compute Shaders, HLSL**
- **TypeScript**

---

## Domain-Split Architecture

Project is split into different domains, each containing strict set of responsibilities. It makes it easier to find specific code in project, or place where to put new classes. each domain has it's own assembly definition and references only the ones that are necessary. 

AppFlow - entry points and containers that build all required classes and depebdencies for game to work

Models - data domain of game - classes that store data, including generated classes and functions from TS to query data, and including runtime session data and logic

UI.Presenters - presentation domain of ui - presenters, which depend on various data and models from UI.Models

UI.Models - model domain of ui - models and business logic related to ui. Has no knowledge regarding prefabs or predenters that use them.

Gameplay - entities and components required for game, all the game logic that doesn't fit into ui logic or session-long logic




## Model-View-Presenter for UI

There are many variations of MV* pattern, for example MVP has common Passive-View and Supervising-Controller subtypes, and even inside one definition - there are still uncertainties. What is Model - specific class or whole Model domain of your app? What is View - special class or some provided elements by the program environment? What is Presenter - pure C# class or MonoBehavior?

Here this approach is used: **MVP with Passive-View**.

- **Presenter** is MonoBehavor that is bound to UI Model
- **Everything controlled by Presenter** (Unity UI components or other helper components in most cases) - is View

## Component-Based Approach for Gameplay

In many cases (but not always - sometimes it works great) gameplay doesn't align well with MV* approaches - it often leads to boilerplate code, difficulties with fast prototyping and sometimes.

Therefore, for this project classic component-based approach was chosen, but with addition of OOP layer for better maintenance - there are abstractions, factories, not very deep inheritance - only things that actually make it easier to maintain.

## TypeScript Configs

TypeScript is used to configure every in-game entity, that does not have to be inside scriptable object.

**Such as:**
- Character stats
- UI icon paths
- Loot generation

### Benefits

What benefits does this approach has compared to JSON and other format types? It allows you to work with strongly-typed language and define logic right inside your config - create templates, create loot-generation rules, anything. And you can ship it to players even without client update.

In this project single compiled config file is used, but for cloud-based delivery it would be better to split it.

### Performance

Performance won't be an issue - even in large scale project whole config would take a few MB of RAM, and function execution would take such few that it can be ignored, unleass you are dealing with hundreds of calls - however even in this case it can be optimized with functions taht return data arrays.

For even higher performance you can use a native JS engine, but in this project for simplicity reason I went with **JINT** that works completely inside C# runtime and therefore compatible with any platform.

## Code-Generation

To streamline TS types to C# types mapping and avoid writing similar code, project has a code generation functionality, that creates C# classes based on TS classes annotated with `@csharp-namespace`

## Compute Shaders

This project also has compute shader driven cloud system, that shows how separated feature that involves other technologies and specific approach can be integrated.
