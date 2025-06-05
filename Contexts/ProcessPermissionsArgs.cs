// -----------------------------------------------------------------------
// <copyright file="ProcessPermissionsArgs.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Contexts;

using CommandSystem;

/// <summary>
/// Arguments provided for processing permissions.
/// </summary>
/// <param name="sender">The command sender.</param>
public class ProcessPermissionsArgs(ICommandSender sender)
{
    /// <summary>
    /// Gets the command sender.
    /// </summary>
    public ICommandSender Sender { get; } = sender;

    /// <summary>
    /// Gets or sets a value indicating whether the permissions are allowed.
    /// </summary>
    public bool IsAllowed { get; set; } = true;

    /// <summary>
    /// Gets or sets a response to show the player.
    /// </summary>
    public string Response { get; set; } = string.Empty;
}