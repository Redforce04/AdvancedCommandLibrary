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

using Attributes;
using Contexts;

public class DoubleNestedParentCommandTesting
{

    [ParentCommand("doubleNestedBase", "The base command for the plugin", ["basealias"], [], CommandHandlerType.RemoteAdmin | CommandHandlerType.GameConsole)]
    public static void BaseBaseCommand(ParentCommandContext context)
    {
        if(!context.CheckPermissions())
            return;
        context.RespondWithSubCommands();
    }
    
    [Parent(typeof(DoubleNestedParentCommandTesting),nameof(BaseBaseCommand))]
    [ParentCommand("nestedBase", "The Second base command for the plugin", [], [])]
    public static void BaseCommand(ParentCommandContext context)
    {
        if(!context.CheckPermissions())
            return;
        context.RespondWithSubCommands();
    }
            
    [Parent(typeof(DoubleNestedParentCommandTesting),nameof(BaseCommand))]
    [Command("subCommand", "An example subcommand.", ["sub"], [])]
    public static void Run(CommandContext context)
    {
        if(!context.CheckPermissions())
            return;
        context.Allow("it worked!");
    }
}