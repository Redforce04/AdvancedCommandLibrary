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
using System.Linq;
using System.Reflection;

internal class ParentCommandTracker : CommandTracker
{
    internal ParentCommandTracker() : base()
    {
    }
    internal MethodInfo? LoadGeneratedCommandsExecutor { get; set; }

    internal List<CommandTracker> ChildCommands => this._childCommandsIds.Select(childId => CommandManager.Instance.RegisteredCommands[childId]).ToList();

    private List<int> _childCommandsIds { get; set; } = new();
    internal void UpdateChildren(List<CommandTracker> childIds) => this._childCommandsIds = childIds.Select(x => x.Id).ToList();
    internal void AddChild(CommandTracker child) => this._childCommandsIds.Add(child.Id);
}