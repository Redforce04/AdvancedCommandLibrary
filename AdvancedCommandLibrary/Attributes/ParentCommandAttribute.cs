// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         ParentCommandAttribute.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 17:03
//    Created Date:     05/19/2025 17:05
// -----------------------------------------

namespace AdvancedCommandLibrary;

using System;

[AttributeUsage(AttributeTargets.Method)]
public class ParentCommandAttribute(
    string commandName,
    string commandDescription,
    string[]? commandAliases = null,
    string[]? commandUsages = null,
    CommandHandlerType commandHandlerType = CommandHandlerType.RemoteAdmin | CommandHandlerType.GameConsole)
    : CommandAttribute(commandName,
        commandDescription,
        commandAliases,
        commandUsages,
        commandHandlerType);