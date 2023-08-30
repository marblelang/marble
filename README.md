<img alt="Marble logo" src="logo-with-text.svg" width="400" />

# Marble programming language compiler

[![License Badge](https://img.shields.io/badge/license-Apache_2.0-orange)](LICENSE.txt)

## Introduction

Marble is a programming language that takes inspiration from C# and Kotlin. It is a statically typed language that
compiles to CIL bytecode. It is designed to be a general purpose language that can be used with the .NET ecosystem.
This is the compiler for the language, which is written in C#. It follows the
[Marble language specification](https://github.com/marblelang/marble-spec).

This project is still in its early stages, so it is not ready for production use. Contributions are welcome.

## Building

To build the compiler, you will need the .NET 7 SDK. You can build the compiler by running `dotnet build` in the root
directory of the repository. You can also run `dotnet test` to run the unit tests.

## Usage

The compiler is not yet ready for use. However, you can run the compiler by running `dotnet run` to load a sample program
and print the lexical tokens to the console. As the compiler is developed, this will be updated to compile the program
and print the resulting bytecode.

## License

This project is licensed under the Apache 2.0 license. See the [LICENSE](LICENSE.txt) file for more information.
