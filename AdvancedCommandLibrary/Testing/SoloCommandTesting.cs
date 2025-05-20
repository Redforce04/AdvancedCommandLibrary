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

public class SoloCommandTesting
{
    [Command("SoloCommand", "An example subcommand.", ["solo"], [])]
    public static void Run(CommandContext context)
    {
        if(!context.CheckPermissions())
            return;
        context.Allow("it worked!");
    }
}