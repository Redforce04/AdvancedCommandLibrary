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
using CommandSystem;
using Contexts;
using Extensions;
using LabApi.Features.Wrappers;

internal class PermissionsTracker
{
    internal PermissionsTracker(List<RequirePermissionsAttribute> commandAttribute)
    {
        foreach (RequirePermissionsAttribute attribute in commandAttribute)
        {
            this.OnProcessingPermissions += attribute.CheckPermissions;
        }
    }
    public event ProcessingPermissions? OnProcessingPermissions;
    public delegate void ProcessingPermissions(ProcessPermissionsArgs args);

    internal void ProcessPermissions(ProcessPermissionsArgs args)
    {
        try
        {
            OnProcessingPermissions?.Invoke(args);
        }
        catch (Exception)
        {
            // Unused.
        }
    }
}