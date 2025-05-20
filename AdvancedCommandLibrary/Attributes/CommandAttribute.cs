// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         VirtualizedCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:37
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

namespace AdvancedCommandLibrary;

using System;

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public CommandAttribute(string commandName, string commandDescription, string[]? commandAliases = null, string[]? commandUsages = null, CommandHandlerType commandHandlerType = CommandHandlerType.RemoteAdmin | CommandHandlerType.GameConsole)
    {
        this.Name = commandName;
        this.Description = commandDescription;
        this.Aliases = commandAliases ?? [];
        this.Usages = commandUsages ?? [];
        this.HandlerType = commandHandlerType;
    }

    public string Name { get; }
    public string Description { get; }
    public string[] Aliases { get; }
    public string[] Usages { get; }
    public CommandHandlerType HandlerType { get; }

}