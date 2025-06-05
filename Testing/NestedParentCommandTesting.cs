// -----------------------------------------------------------------------
// <copyright file="NestedParentCommandTesting.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Testing;

using Attributes;
using Contexts;

/// <summary>
/// A nested parent testing command.
/// </summary>
public class NestedParentCommandTesting
{
    /// <summary>
    /// A Base Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
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
    /// A sub command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [Parent(typeof(NestedParentCommandTesting), nameof(BaseCommand))]
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