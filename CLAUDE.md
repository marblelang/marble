# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Marble is a statically typed programming language compiler inspired by C# and Kotlin that compiles to CIL bytecode for the .NET ecosystem. The compiler is written in C# (currently targeting .NET 10) and follows the [Marble language specification](https://github.com/marblelang/marble-spec).

**Current Status**: The compiler is in early development, currently implementing the lexical analysis phase. Parser, semantic analysis, and code generation phases are not yet implemented.

## Build and Test Commands

```bash
# Build the entire solution
dotnet build

# Run the compiler (currently outputs lexical tokens to console)
dotnet run --project Compiler

# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test class
dotnet test --filter "FullyQualifiedName~LexerTests"

# Build in Release mode
dotnet build -c Release
```

## Architecture Overview

### Project Structure

- **Compiler**: Main compiler executable with lexical analysis implementation
- **Compiler.Tests**: NUnit-based test suite

### Compilation Pipeline (Current Implementation)

```
Source Code
    ↓
SourceReader (character-level access with lookahead)
    ↓
Lexer (state machine-based tokenizer)
    ↓
SyntaxToken stream (with trivia and diagnostics)
```

**Planned future stages**: Parser → Semantic Analyzer → Code Generator → CIL Bytecode

### Core Components

#### 1. Syntax System (Compiler/Syntax/)

**SourceReader** (`SourceReader.cs`):
- Encapsulates character-by-character source code access
- Uses `ReadOnlyMemory<char>` for efficient, allocation-free operations
- Supports lookahead via `Peek(offset)` without consuming characters
- Returns `\0` at end-of-file

**Lexer** (`Lexer.cs`):
- State machine-based tokenizer with stack-based state management
- Handles complex string interpolation with nested expression parsing
- Preserves all trivia (whitespace, comments) for round-trip compilation
- Collects diagnostics without throwing exceptions for error recovery

**LexerState enum**:
- `Normal`: Regular code tokenization
- `LineString`: Inside single-line string literal
- `MultilineString`: Inside triple-quoted string
- `InterpolatedString`: Processing string with interpolation
- `Interpolation`: Inside `${...}` expression within interpolated string

State transitions enable context-dependent tokenization (e.g., `}` closes interpolation or is a regular brace depending on state).

**SyntaxFacts** (`SyntaxFacts.cs`):
- Centralized metadata for token kinds
- `GetKeywordKind(string)`: Keyword recognition (O(1) switch expression)
- `GetTokenText(SyntaxKind)`: Reverse lookup for punctuation/operator text

#### 2. Token Hierarchy

```
IToken (interface)
├── SyntaxToken (record) - Main tokens with trivia
│   └── LiteralToken (sealed record) - Extends SyntaxToken with parsed Value
└── TriviaToken (record) - Whitespace, comments, newlines
```

**SyntaxToken structure**:
- `Kind` (SyntaxKind): Token classification (80+ types including keywords, operators, literals)
- `Text` (string): Original source text
- `LeadingTrivia` / `TrailingTrivia`: Attached whitespace/comments
- `Diagnostics`: Errors/warnings associated with this token

**LiteralToken**:
- Adds `Value` (object): Parsed literal value (int, float, double, char, string)

#### 3. Diagnostic System (Compiler/Diagnostics/)

**Design**: Immutable records with structured error codes and severity levels.

**Diagnostic** (definition):
- `Code`: Format `MB{Category}{Index:D3}` (e.g., MB1001 = Syntax category, error 001)
- `Title`: Short error description
- `Format`: Message template with placeholders
- `Category`: Syntax, InternalCompiler, etc.
- `Severity`: Error, Warning, Info, Hint

**DiagnosticInfo** (instance):
- Links to Diagnostic definition
- `Offset` and `Width`: Source location span
- `Arguments`: Values for format placeholders

**Current Syntax Diagnostics (MB1xxx)**:
- MB1000: Invalid token
- MB1001: Unclosed string literal
- MB1002: Unclosed multiline comment
- MB1003: Unclosed character literal
- MB1004: Invalid character literal (empty or multiple chars)
- MB1005: Invalid unicode codepoint in escape
- MB1006: Invalid escape character

### String Literal Support

The lexer handles sophisticated string literal types:

- **Line strings**: `"simple string"` with escape sequences
- **Multiline strings**: `"""text across\nlines"""`
- **Interpolated strings**: `$"value is {expr}"` with nested expression parsing
- **Escape sequences**: Standard (`\n`, `\t`, `\\`, `\"`) and unicode (`\uXXXX`)

String interpolation uses state stack to track nesting depth and properly handle `{` and `}` characters.

## Key Design Patterns

1. **Trivia Preservation**: Unlike traditional compilers, Marble preserves all whitespace and comments as trivia attached to tokens. This enables perfect source reconstruction and supports IDE features (formatting, refactoring).

2. **Immutability**: All data structures use C# records for value-based equality and immutability. No mutation after construction.

3. **Error Recovery**: Lexer never throws exceptions. Instead, it produces tokens (possibly `Unknown` kind) with attached diagnostics, allowing compilation to continue and report multiple errors.

4. **Separation of Concerns**:
   - SourceReader: Character access abstraction
   - Lexer: Token generation logic
   - SyntaxFacts: Metadata and lookup tables
   - Diagnostics: Error reporting (decoupled from lexing)

5. **State Pattern**: Stack-based state machine for context-dependent tokenization (essential for string interpolation).

## Testing Conventions

- Use NUnit framework with `[Test]` and `[TestCase]` attributes
- Test files mirror source structure (e.g., `LexerTests.cs` tests `Lexer.cs`)
- Create focused test methods for each feature area
- Use helper methods to reduce boilerplate (e.g., asserting token properties)
- Test both happy paths and error cases (verify diagnostic generation)
- Parameterized tests with `[TestCase]` for similar test variations

**Example test pattern**:
```csharp
[TestCase("input", SyntaxKind.ExpectedKind)]
public void TestFeature(string input, SyntaxKind expectedKind)
{
    var reader = new SourceReader(input);
    var lexer = new Lexer(reader);
    var token = lexer.Lex();
    Assert.That(token.Kind, Is.EqualTo(expectedKind));
}
```

## Important Implementation Notes

- **InternalsVisibleTo**: Compiler internals are exposed to Compiler.Tests (see Compiler.csproj)
- **Nullable Reference Types**: Enabled project-wide; use nullable annotations consistently
- **Implicit Usings**: Enabled; common namespaces are automatically imported
- **SyntaxKind Enum**: Single source of truth for all token types; add new token types here first
- **Lexer Token Creation**: Use helper methods (`TakeBasic`, `TakeWithText`, `TakeTrivia`) for consistent token construction

## Working with the Lexer

When modifying lexical analysis:

1. **Adding new token types**:
   - Add to `SyntaxKind` enum
   - Update `SyntaxFacts.GetTokenText()` if it's punctuation/operator
   - Update `SyntaxFacts.GetKeywordKind()` if it's a keyword
   - Add lexing logic in appropriate `Lexer` method

2. **Adding diagnostics**:
   - Define in `Diagnostics/Syntax.cs` with next available MB1xxx code
   - Create diagnostic in lexer when error condition detected
   - Attach via `SyntaxToken.Diagnostics` property

3. **State-dependent tokenization**:
   - Use `_state` stack for context management
   - Push new state when entering context (e.g., string interpolation)
   - Pop state when exiting context
   - Check current state with `_state.Peek()`

## Language Specification

The compiler follows the [Marble language specification](https://github.com/marblelang/marble-spec). When implementing new features, refer to the spec for grammar and semantics.

## Issue Tracking

Issues are tracked on [Taiga](https://tree.taiga.io/project/luluvia-marble-language-compiler/issues) (primary) and [GitHub](https://github.com/marblelang/marble/issues) (synchronized).
