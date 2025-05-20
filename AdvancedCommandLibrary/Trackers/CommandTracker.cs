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

using System;
using System.Collections;
using System.Reflection;
using CommandSystem;

internal class CommandTracker
{
    internal CommandTracker(int id = -1)
    {
        if (id == -1)
        {
            this.Id = LastId;
            LastId = LastId++;
        }
        else
            this.Id = id;
    }

    protected private static int LastId { get; set; } = 0;
    internal int Id { get; set; }
    internal ICommand GameCommandInstance { get; set; }
    internal MethodInfo Method { get; init; }
    internal Assembly Assembly { get; init; }
    internal ParentCommandTracker? ParentTrackerInstance { get; set; } 
    internal PermissionsTracker? PermissionsRequirementTracker { get; init; }

    internal string Name { get; init; } 
    internal string Description { get; init; }
    internal string[] Aliases { get; init; }
    internal string[] Usages { get; init; }
    internal CommandHandlerType HandlerType { get; init; }

    public bool CompareAgainstMethod(CommandTracker commandTracker)
    {
        return commandTracker.Method == this.Method;
    }

    public bool CompareAgainstId(CommandTracker commandTracker)
    {
        return commandTracker.Id == this.Id;
    }
    
    protected bool Equals(CommandTracker other)
    {
        return this.Id == other.Id;
    }

    public override int GetHashCode()
    {
        return this.Id;
    }
}