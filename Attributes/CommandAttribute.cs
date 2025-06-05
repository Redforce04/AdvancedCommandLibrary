// -----------------------------------------------------------------------
// <copyright file="CommandAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;

#pragma warning disable SA1135
using Enums;
#pragma warning restore SA1135

/// <summary>
/// Used to define a command from a public static method.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandAttribute"/> class.
    /// </summary>
    /// <param name="commandName">The name for the command.</param>
    /// <param name="commandDescription">The description for the command.</param>
    /// <param name="commandAliases">The aliases for the command.</param>
    /// <param name="commandUsages">The usages for the command.</param>
    /// <param name="commandHandlerType">The command handlers for the command.</param>
    public CommandAttribute(string commandName, string commandDescription, string[]? commandAliases = null, string[]? commandUsages = null, CommandHandlerType commandHandlerType = CommandHandlerType.RemoteAdmin | CommandHandlerType.GameConsole)
    {
        this.Name = commandName;
        this.Description = commandDescription;
        this.Aliases = commandAliases ?? [];
        this.Usages = commandUsages ?? [];
        this.HandlerType = commandHandlerType;
    }

    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// gets the aliases of the command.
    /// </summary>
    public string[] Aliases { get; }

    /// <summary>
    /// Gets the usages of the command.
    /// </summary>
    public string[] Usages { get; }

    /// <summary>
    /// Gets the handlers of the command.
    /// </summary>
    public CommandHandlerType HandlerType { get; }
}