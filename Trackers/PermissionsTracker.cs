// -----------------------------------------------------------------------
// <copyright file="PermissionsTracker.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Trackers;

using System;
using System.Collections.Generic;

using Attributes;
using Contexts;
using LabApi.Features.Console;

/// <summary>
/// A permission tracking instance.
/// </summary>
public class PermissionsTracker
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionsTracker"/> class.
    /// </summary>
    /// <param name="commandAttribute">A list of permission tracking instances to consider.</param>
    internal PermissionsTracker(List<RequirePermissionsAttribute> commandAttribute)
    {
        foreach (RequirePermissionsAttribute attribute in commandAttribute)
        {
            this.OnProcessingPermissions += attribute.CheckPermissions;
        }
    }

    /// <summary>
    /// The delegate used to call <see cref="PermissionsTracker.OnProcessingPermissions"/>.
    /// </summary>
    /// <param name="args">The processing arguments to pass through.</param>
    public delegate void ProcessingPermissions(ProcessPermissionsArgs args);

    /// <summary>
    /// Called when this permissions tracker is processing permissions.
    /// </summary>
    public event ProcessingPermissions? OnProcessingPermissions;

    /// <summary>
    /// Invokes <see cref="OnProcessingPermissions"/>.
    /// </summary>
    /// <param name="args">The processing arguments to pass through.</param>
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
        }
    }
}