// -----------------------------------------------------------------------
// <copyright file="CommandContext.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Contexts;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using CommandSystem;
using Enums;
using JetBrains.Annotations;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using Trackers;

/// <summary>
/// The primary context to respond to a command.
/// </summary>
public class CommandContext : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandContext"/> class.
    /// </summary>
    /// <param name="args">The arguments provided by the sender.</param>
    /// <param name="sender">The CommandSender who sent the message.</param>
    /// <param name="response">The default response to show.</param>
    /// <param name="command">The command instance that is being executed.</param>
    /// <param name="parentCommand">The parent command of the command instance (if applicable).</param>
    /// <param name="permissionsTracker">A permission tracker if the command has a permission tracker.</param>
    internal CommandContext(ArraySegment<string> args, ICommandSender sender, string response, ICommand command, ParentCommand? parentCommand = null, PermissionsTracker? permissionsTracker = null)
    {
        this.Args = args.ToArray();
        this.Sender = sender;
        this.Response = response == string.Empty ? "No response from command." : response;
        this.CommandInstance = command;
        this.ParentCommand = parentCommand;
        this.PermissionsTracker = permissionsTracker;
    }

    /// <summary>
    /// Gets a list of arguments provided by the sender.
    /// </summary>
    [PublicAPI]
    public string[] Args { get; }

    /// <summary>
    /// Gets or sets the response to show the sender.
    /// </summary>
    [PublicAPI]
    public string Response { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the command is allowed to execute.
    /// </summary>
    [PublicAPI]
    public bool Allowed { get; set; }

    /// <summary>
    /// Gets the default command instance being executed.
    /// </summary>
    [PublicAPI]
    public ICommand CommandInstance { get; }

    /// <summary>
    /// Gets the parent command of the executing command if the command has a parent command.
    /// </summary>
    [PublicAPI]
    public ParentCommand? ParentCommand { get; }

    /// <summary>
    /// Gets the command sender.
    /// </summary>
    [PublicAPI]
    public ICommandSender Sender { get; }

    /// <summary>
    /// Gets the Permissions tracker if the command has one.
    /// </summary>
    private PermissionsTracker? PermissionsTracker { get; }

    /// <summary>
    /// Allows the command to execute and provides an optional response.
    /// </summary>
    /// <param name="response">The response to show the sender.</param>
    public void Allow(string response = "")
    {
        this.Allowed = true;
        if(response != string.Empty)
        {
            this.Response = response;
        }
    }

    /// <summary>
    /// Denies the command from executing and provides an optional response.
    /// </summary>
    /// <param name="response">The response to show the sender.</param>
    public void Deny(string response = "")
    {
        this.Allowed = false;
        if (response != string.Empty)
        {
            this.Response = response;
        }
    }

    /// <summary>
    /// Checks the permissions of the sender and sets the response from the provided permission tracker response if not allowed to execute.
    /// </summary>
    /// <returns>True if the command is allowed to execute otherwise false.</returns>
    public bool CheckPermissions()
    {
        bool debug = CommandManager.DebugMode >= LoggingMode.Ludicrous;
        try
        {
            if (this.PermissionsTracker is null)
            {
                Logger.Debug($"Permissions for command {this.CommandInstance.Command} is null. Skipping Permissions Check.", debug);
                return true;
            }

            ProcessPermissionsArgs args = new(this.Sender);
            this.PermissionsTracker.ProcessPermissions(args);
            if (!args.IsAllowed)
            {
                this.Response = args.Response;
                return false;
            }
        }
        catch (Exception)
        {
            this.Response = "An error has occured.";
            this.Allowed = false;
            return false;
        }

        Logger.Debug($"Sender has permissions for command {this.CommandInstance.Command}.", debug);
        this.Response = "You have permission to execute this command.";
        this.Allowed = true;
        return true;
    }

    /// <summary>
    /// Tries to get the <see cref="Sender"/> as a <see cref="Player"/>.
    /// </summary>
    /// <param name="ply">The sender as a player.</param>
    /// <returns>True if the sender is a player, otherwise false.</returns>
    [MemberNotNullWhen(true)]
    public bool TryGetSenderAsPlayer(out Player? ply)
    {
        ply = null;
        if (this.Sender is PlayerCommandSender playerSender)
        {
            ply = Player.Get(playerSender.ReferenceHub);
            return true;
        }

        return false;
    }
}