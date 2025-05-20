// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         CommandTracker.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 17:05
//    Created Date:     05/19/2025 17:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Trackers;

using System.Collections.Generic;
using System.Reflection;

internal class ParentCommandTracker : CommandTracker
{
    internal ParentCommandTracker() : base()
    {
    }

    internal List<CommandTracker> ChildCommands { get; set; } = new();
}