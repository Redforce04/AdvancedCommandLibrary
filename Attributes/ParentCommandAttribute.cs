// -----------------------------------------------------------------------
// <copyright file="ParentCommandAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;

using Enums;

/// <summary>
/// Used to define a command from a public static method.
/// </summary>
/// <param name="commandName">The name for the command.</param>
/// <param name="commandDescription">The description for the command.</param>
/// <param name="commandAliases">The aliases for the command.</param>
/// <param name="commandUsages">The usages for the command.</param>
/// <param name="commandHandlerType">The command handlers for the command.</param>
[AttributeUsage(AttributeTargets.Method)]
public class ParentCommandAttribute(
    string commandName,
    string commandDescription,
    string[]? commandAliases = null,
    string[]? commandUsages = null,
    CommandHandlerType commandHandlerType = CommandHandlerType.RemoteAdmin | CommandHandlerType.GameConsole)
    : CommandAttribute(
        commandName,
        commandDescription,
        commandAliases,
        commandUsages,
        commandHandlerType);