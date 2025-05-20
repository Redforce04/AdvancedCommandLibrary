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
using System.Linq;
using System.Reflection;
using Attributes;

internal class PermissionsTracker
{
    internal PermissionsTracker(RequirePermissionsAttribute commandAttribute)
    {
        this.RequiredPermissionNodes = commandAttribute.RequiredPermissionNodes ?? [];
        this.RequiredPlayerPermissions = commandAttribute.RequiredPlayerPermissions ?? (PlayerPermissions) 0;
    }
        
    internal string[] RequiredPermissionNodes { get; init; }
    internal PlayerPermissions RequiredPlayerPermissions { get; init; }
    
}