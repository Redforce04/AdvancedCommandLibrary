// -----------------------------------------------------------------------
// <copyright file="DoubleNestedParentCommandTesting.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Testing;

using Attributes;
using Contexts;

/// <summary>
/// Used to test commands.
/// </summary>
public class DoubleNestedParentCommandTesting
{
    /// <summary>
    /// A Base Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [ParentCommand("doubleNestedBase", "The base command for the plugin", ["basealias"], [])]
    public static void BaseBaseCommand(ParentCommandContext context)
    {
        if(!context.CheckPermissions())
        {
            return;
        }

        context.RespondWithSubCommands();
    }

    /// <summary>
    /// A nested base command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [Parent(typeof(DoubleNestedParentCommandTesting), nameof(BaseBaseCommand))]
    [ParentCommand("nestedBase", "The Second base command for the plugin", [], [])]
    public static void BaseCommand(ParentCommandContext context)
    {
        if(!context.CheckPermissions())
        {
            return;
        }

        context.RespondWithSubCommands();
    }

    /// <summary>
    /// A nested sub command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [Parent(typeof(DoubleNestedParentCommandTesting), nameof(BaseCommand))]
    [Command("subCommand", "An example subcommand.", ["sub"], [])]
    public static void Run(CommandContext context)
    {
        if(!context.CheckPermissions())
        {
            return;
        }

        context.Allow("it worked!");
    }
}