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
using Attributes;
using Contexts;
using LabApi.Features.Console;

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
            this.OnProcessingPermissions?.Invoke(args);
        }
        catch (Exception e)
        {
            Logger.Warn($"An exception occured while processing permissions.");
            Logger.Debug($"Exception: {e}");
            // Unused.
        }
    }
}