# TypeScript to C# Code Generator

## Overview
This system exports types and functions from TypeScript to C#.
It runs only manually to ensure changes happened only when it is intended.

To run generation select in top menu Tools/Code Generation/Generate C# From TypeScript.

It will delete all old classes marked with TsGenerated or TsGeneratedFunc, if they are gone from Typescript,
and create all required ones, marking them with these attributes.


## Interfaces
To export TypeScript interface as C# class, mark it with special annotation:
```typescript
/**
 * @cs-export Game.Generated.Models.Player
 */
interface IPlayerParams { ... }
```
Parameter specifies class namespace and folder(it will match the namespace)


### Functions
To export TypeScript interface as C# class, mark it with special annotation:
```typescript
/**
 * @cs-export Game.Generated.Query.Player:PlayerQuery
 */
function getPlayerParams() { ... }
```
First parameter specifies namespace - similar to classes. 
The second parameter specifies outer partial class, to make it easier to organize in proejct.
Fir the example above the result will be
```c#
/**
 * @cs-export Game.Generated.Query.Player:PlayerQuery
 */
function getPlayerParams() { ... }
```

## Jint Type Conversions

### From JavaScript to C#
- `JsValue.AsString()` → `string`
- `JsValue.AsNumber()` → `double` (cast to `int` or `float` as needed)
- `JsValue.AsBoolean()` → `bool`
- `JsValue.AsArray()` → Array processing
- `JsValue.As{CustomType}()` → Custom type converter

### Converter Methods
Each custom type gets an extension method:
```csharp
public static CustomType AsCustomType(this JsValue jsValue)
{
    // Conversion logic
}
```

## File Structure

Generated files are placed in:
- Base path: `Assets/Game/Generated/`
- Folder mirrors namespace after `Game.Generated.`
- Example: `Game.Generated.Models.Player` → `Assets/Game/Generated/Models/Player/`

## Attributes

### For Structs (from interfaces)
```csharp
[TsGenerated]
public readonly struct TypeName { ... }
```

### For Functions
```csharp
[TsGeneratedFunc]
public class FunctionName : BaseQuery { ... }
```

These attributes are used to identify generated files for cleanup when TypeScript definitions change.
