// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         PowerupAPI
//    Project:          PowerupAPI
//    FileName:         ParentCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/18/2025 11:17
//    Created Date:     05/18/2025 11:05
// -----------------------------------------

namespace AdvancedCommandLibrary.GameCommandModules;

using System;
using System.Diagnostics.CodeAnalysis;
using CommandSystem;

public abstract class AdvancedParentCommand : ParentCommand
{
    public override abstract string Command { get; }
    public override abstract string[] Aliases { get; }
    public override abstract string Description { get; }
    
    public override void LoadGeneratedCommands()
    {
    }

    protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        response = "Please enter a valid subcommand: \n";
        return false;
    }
    
}
