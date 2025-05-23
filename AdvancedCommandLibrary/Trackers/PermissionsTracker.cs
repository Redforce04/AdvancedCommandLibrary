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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Attributes;
using Extensions;

internal class PermissionsTracker
{
    internal PermissionsTracker(List<RequirePermissionsAttribute> commandAttribute)
    {
        this.RequiredPlayerPermissions = (PlayerPermissions) 0;
        List<string> permsNodes = new ();
        foreach (RequirePermissionsAttribute attribute in commandAttribute)
        {
            if(attribute.RequiredPermissionNodes is not null)
                permsNodes.AddRange(attribute.RequiredPermissionNodes);
            PlayerPermissions newPerms = attribute.RequiredPlayerPermissions ?? 0;
            if(attribute.RequiredPlayerPermissions is not null)
                this.RequiredPlayerPermissions = this.RequiredPlayerPermissions.Include(newPerms);
        }
        this.RequiredPermissionNodes = permsNodes.ToArray();
    }
        
    internal string[] RequiredPermissionNodes { get; init; }
    internal PlayerPermissions RequiredPlayerPermissions { get; init; }
    
}