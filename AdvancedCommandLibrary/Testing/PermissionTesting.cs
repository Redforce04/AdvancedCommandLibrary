// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         Testing.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 20:26
//    Created Date:     05/19/2025 20:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Testing;

using System.Runtime.Remoting.Contexts;
using Attributes;
using Contexts;

public class PermissionTesting
{
    [ParentCommand("TestPerms", "Test various permissions related commands", [], [], CommandHandlerType.RemoteAdmin | CommandHandlerType.GameConsole)]
    public static void BaseCommand(ParentCommandContext context)
    {
        context.RespondWithSubCommands();
    }
    
    [Parent(typeof(PermissionTesting),nameof(BaseCommand))]
    [ParentCommand("nodes", "Tests the permissions node checker.", [], [])]
    public static void PermissionNodesBaseCommand(ParentCommandContext context)
    {
        context.RespondWithSubCommands();
    }
    
    [Parent(typeof(PermissionTesting),nameof(BaseCommand))]
    [ParentCommand("playerPerms", "Tests the player permissions checker.", [], [])]
    public static void PlayerPermissionsBaseCommand(ParentCommandContext context)
    {
        context.RespondWithSubCommands();
    }
    
    [RequirePermissionNodes("developer")]
    [Parent(typeof(PermissionTesting),nameof(PermissionNodesBaseCommand))]
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
    
    [RequirePermissionNodes("MissingPermissionsNode")]
    [Parent(typeof(PermissionTesting),nameof(PermissionNodesBaseCommand))]
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
    
    [RequirePlayerPermissions(PlayerPermissions.AdminChat)]
    [Parent(typeof(PermissionTesting),nameof(PlayerPermissionsBaseCommand))]
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
    
    [RequirePlayerPermissions(PlayerPermissions.SetGroup | PlayerPermissions.AdminChat)]
    [Parent(typeof(PermissionTesting),nameof(PlayerPermissionsBaseCommand))]
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