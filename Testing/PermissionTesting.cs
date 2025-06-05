// -----------------------------------------------------------------------
// <copyright file="PermissionTesting.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Testing;

using Attributes;
using Contexts;

/// <summary>
/// A permission testing command.
/// </summary>
public class PermissionTesting
{
    /// <summary>
    /// A Base Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [ParentCommand("TestPerms", "Test various permissions related commands", [], [])]
    public static void BaseCommand(ParentCommandContext context)
    {
        context.RespondWithSubCommands();
    }

    /// <summary>
    /// A Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [Parent(typeof(PermissionTesting), nameof(BaseCommand))]
    [ParentCommand("nodes", "Tests the permissions node checker.", [], [])]
    public static void PermissionNodesBaseCommand(ParentCommandContext context)
    {
        context.RespondWithSubCommands();
    }

    /// <summary>
    /// A Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [Parent(typeof(PermissionTesting), nameof(BaseCommand))]
    [ParentCommand("playerPerms", "Tests the player permissions checker.", [], [])]
    public static void PlayerPermissionsBaseCommand(ParentCommandContext context)
    {
        context.RespondWithSubCommands();
    }

    /// <summary>
    /// A Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [RequirePermissionNodes("developer")]
    [Parent(typeof(PermissionTesting), nameof(PermissionNodesBaseCommand))]
    [Command("pass", "Passes the player permissions checker.", [], [])]
    public static void PermissionNodesPass(CommandContext context)
    {
        if (!context.CheckPermissions())
        {
            context.Deny("Test Failed. [Command Failed]");
            return;
        }

        context.Allow("Test Passed. [Command Passed]");
    }

    /// <summary>
    /// A Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [RequirePermissionNodes("MissingPermissionsNode")]
    [Parent(typeof(PermissionTesting), nameof(PermissionNodesBaseCommand))]
    [Command("fail", "Fails the player permissions checker.", [], [])]
    public static void PermissionNodesFail(CommandContext context)
    {
        if (!context.CheckPermissions())
        {
            context.Deny("Test Passed. [Command Failed]");
            return;
        }

        context.Allow("Test Failed. [Command Passed]");
    }

    /// <summary>
    /// A Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [RequirePlayerPermissions(PlayerPermissions.AdminChat)]
    [Parent(typeof(PermissionTesting), nameof(PlayerPermissionsBaseCommand))]
    [Command("pass", "Passes the player permissions checker.", [], [])]
    public static void PlayerPermissionPass(CommandContext context)
    {
        if (!context.CheckPermissions())
        {
            context.Deny("Test Failed. [Command Failed]");
            return;
        }

        context.Allow("Test Passed. [Command Passed]");
    }

    /// <summary>
    /// A Command.
    /// </summary>
    /// <param name="context">The executing command context.</param>
    [RequirePlayerPermissions(PlayerPermissions.SetGroup | PlayerPermissions.AdminChat)]
    [Parent(typeof(PermissionTesting), nameof(PlayerPermissionsBaseCommand))]
    [Command("fail", "Fails the player permissions checker.", [], [])]
    public static void PlayerPermissionFail(CommandContext context)
    {
        if (!context.CheckPermissions())
        {
            context.Deny("Test Passed. [Command Failed]");
            return;
        }

        context.Allow("Test Failed. [Command Passed]");
    }
}