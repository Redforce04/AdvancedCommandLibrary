// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         CommandContext.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:46
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Contexts.Helpers;

using System;
using CommandSystem;

public class LoadGeneratedCommandsContext : EventArgs
{
    internal LoadGeneratedCommandsContext(ICommand command, ParentCommand? parentCommand = null)
    {
        this.CommandInstance = command;
        this.ParentCommand = parentCommand;
    }
    public ICommand CommandInstance { get; }
    public ParentCommand? ParentCommand { get; set; }
}