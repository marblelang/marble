namespace Compiler.Syntax;

/// <summary>
/// The type of token.
/// </summary>
public enum SyntaxKind
{
    // General tokens

    /// <summary>
    /// The end of the file.
    /// </summary>
    EndOfFile,

    /// <summary>
    /// An unknown token.
    /// </summary>
    Unknown,

    /// <summary>
    /// A new line sequence.
    /// </summary>
    NewLine,

    /// <summary>
    /// Horizontal whitespace.
    /// </summary>
    Whitespace,

    /// <summary>
    /// A single line comment.
    /// </summary>
    LineComment,

    // Literals

    /// <summary>
    /// A numeric literal.
    /// </summary>
    NumberLiteral,

    /// <summary>
    /// A string literal.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// A character literal.
    /// </summary>
    CharacterLiteral,

    /// <summary>
    /// A non-keyword identifier.
    /// </summary>
    Identifier,

    // Punctuation

    /// <summary>
    /// An open parenthesis. <c>(</c>
    /// </summary>
    OpenParenthesis,

    /// <summary>
    /// A close parenthesis. <c>)</c>
    /// </summary>
    CloseParenthesis,

    /// <summary>
    /// An open bracket. <c>[</c>
    /// </summary>
    OpenBracket,

    /// <summary>
    /// A close bracket. <c>]</c>
    /// </summary>
    CloseBracket,

    /// <summary>
    /// An open brace. <c>{</c>
    /// </summary>
    OpenBrace,

    /// <summary>
    /// A close brace. <c>}</c>
    /// </summary>
    CloseBrace,

    /// <summary>
    /// A comma. <c>,</c>
    /// </summary>
    Comma,

    /// <summary>
    /// A colon. <c>:</c>
    /// </summary>
    Colon,

    /// <summary>
    /// A semicolon. <c>;</c>
    /// </summary>
    Semicolon,

    /// <summary>
    /// A period. <c>.</c>
    /// </summary>
    Period,

    /// <summary>
    /// A question mark. <c>?</c>
    /// </summary>
    QuestionMark,

    /// <summary>
    /// An exclamation mark. <c>!</c>
    /// </summary>
    ExclamationMark,

    /// <summary>
    /// A plus sign. <c>+</c>
    /// </summary>
    Plus,

    /// <summary>
    /// A minus sign. <c>-</c>
    /// </summary>
    Minus,

    /// <summary>
    /// A multiplication sign. <c>*</c>
    /// </summary>
    Asterisk,

    /// <summary>
    /// A division sign. <c>/</c>
    /// </summary>
    Slash,

    /// <summary>
    /// A percent sign. <c>%</c>
    /// </summary>
    Percent,

    /// <summary>
    /// A caret. <c>^</c>
    /// </summary>
    Caret,

    /// <summary>
    /// An ampersand. <c>&amp;</c>
    /// </summary>
    Ampersand,

    /// <summary>
    /// A pipe. <c>|</c>
    /// </summary>
    Pipe,

    /// <summary>
    /// An equals sign. <c>=</c>
    /// </summary>
    Equals,

    /// <summary>
    /// A less than sign. <c>&lt;</c>
    /// </summary>
    LessThan,

    /// <summary>
    /// A greater than sign. <c>&gt;</c>
    /// </summary>
    GreaterThan,

    /// <summary>
    /// A single quote. <c>'</c>
    /// </summary>
    SingleQuote,

    /// <summary>
    /// A double quote. <c>"</c>
    /// </summary>
    DoubleQuote,

    /// <summary>
    /// A backslash. <c>\</c>
    /// </summary>
    Backslash,

    // Compound punctuation

    /// <summary>
    /// A double equals sign. <c>==</c>
    /// </summary>
    DoubleEquals,

    /// <summary>
    /// A not equals sign. <c>!=</c>
    /// </summary>
    NotEquals,

    /// <summary>
    /// A less than or equal to sign. <c>&lt;=</c>
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// A greater than or equal to sign. <c>&gt;=</c>
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// A plus equals sign. <c>+=</c>
    /// </summary>
    PlusEquals,

    /// <summary>
    /// A minus equals sign. <c>-=</c>
    /// </summary>
    MinusEquals,

    /// <summary>
    /// A multiplication equals sign. <c>*=</c>
    /// </summary>
    AsteriskEquals,

    /// <summary>
    /// A division equals sign. <c>/=</c>
    /// </summary>
    SlashEquals,

    /// <summary>
    /// A percent equals sign. <c>%=</c>
    /// </summary>
    PercentEquals,

    /// <summary>
    /// A caret equals sign. <c>^=</c>
    /// </summary>
    CaretEquals,

    /// <summary>
    /// A double ampersand sign. <c>&amp;&amp;</c>
    /// </summary>
    DoubleAmpersand,

    /// <summary>
    /// A double pipe sign. <c>||</c>
    /// </summary>
    DoublePipe,

    /// <summary>
    /// A double plus sign. <c>++</c>
    /// </summary>
    DoublePlus,

    /// <summary>
    /// A double minus sign. <c>--</c>
    /// </summary>
    DoubleMinus,

    // Keywords

    /// <summary>
    /// The <c>and</c> keyword.
    /// </summary>
    AndKeyword,

    /// <summary>
    /// The <c>do</c> keyword.
    /// </summary>
    DoKeyword,

    /// <summary>
    /// The <c>else</c> keyword.
    /// </summary>
    ElseKeyword,

    /// <summary>
    /// The <c>false</c> keyword.
    /// </summary>
    FalseKeyword,

    /// <summary>
    /// The <c>for</c> keyword.
    /// </summary>
    ForKeyword,

    /// <summary>
    /// The <c>fun</c> keyword.
    /// </summary>
    FunKeyword,

    /// <summary>
    /// The <c>if</c> keyword.
    /// </summary>
    IfKeyword,

    /// <summary>
    /// The <c>or</c> keyword.
    /// </summary>
    OrKeyword,

    /// <summary>
    /// The <c>return</c> keyword.
    /// </summary>
    ReturnKeyword,

    /// <summary>
    /// The <c>true</c> keyword.
    /// </summary>
    TrueKeyword,

    /// <summary>
    /// The <c>use</c> keyword.
    /// </summary>
    UseKeyword,

    /// <summary>
    /// The <c>val</c> keyword.
    /// </summary>
    ValKeyword,

    /// <summary>
    /// The <c>var</c> keyword.
    /// </summary>
    VarKeyword,

    /// <summary>
    /// The <c>while</c> keyword.
    /// </summary>
    WhileKeyword
}
