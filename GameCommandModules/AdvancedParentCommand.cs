// -----------------------------------------------------------------------
// <copyright file="AdvancedParentCommand.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules;

using System;
using System.Diagnostics.CodeAnalysis;

using CommandSystem;

/// <summary>
/// The better base game parent command implementation.
/// </summary>
public abstract class AdvancedParentCommand : ParentCommand
{
    /// <summary>
    /// Gets the name of the command.
    /// </summary>
    public override abstract string Command { get; }

    /// <summary>
    /// Gets a list of aliases for the command.
    /// </summary>
    public override abstract string[] Aliases { get; }

    /// <summary>
    /// Gets the description of the command.
    /// </summary>
    public override abstract string Description { get; }

    /// <summary>
    /// Loads the generated commands.
    /// </summary>
    public override void LoadGeneratedCommands()
    {
    }

    /// <summary>
    /// Executed when the command is called without a subcommand.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <param name="sender">The sender.</param>
    /// <param name="response">The response out.</param>
    /// <returns>True if successful, false otherwise.</returns>
    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = "Please enter a valid subcommand: \n";
        return false;
    }
}
