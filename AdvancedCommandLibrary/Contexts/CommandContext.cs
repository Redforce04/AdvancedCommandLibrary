// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         CommandContext.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:46
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Contexts;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using Enums;
using GameCommandModules.Processors;
using LabApi.Features.Console;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using Trackers;

public class CommandContext : EventArgs
{
    internal CommandContext(ArraySegment<string> args, ICommandSender sender, string response, ICommand command, ParentCommand? parentCommand = null, PermissionsTracker? permissionsTracker = null)
    {
        this.Args = args.ToArray();
        this.Sender = sender;
        this.Response = response == "" ? "No response from command." : response;
        this.CommandInstance = command;
        this.ParentCommand = parentCommand;
        this.PermissionsTracker = permissionsTracker;
    }
    private PermissionsTracker? PermissionsTracker { get; }
    public string[] Args { get; }
    public string Response { get; set; }
    public bool Allowed { get; set; } = false;
    public ICommand CommandInstance { get; }
    public ParentCommand? ParentCommand { get; set; }
    public ICommandSender Sender { get; }

    public void Allow(string response = "")
    {
        this.Allowed = true;
        if(response != "")
            this.Response = response;
    }

    public void Deny(string response = "")
    {
        this.Allowed = false;
        if(response != "")
            this.Response = response;
    }

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
            bool hasRequiredPermissionNodes = this.PermissionsTracker.RequiredPermissionNodes.Length == 0 || Sender.HasPermissions(this.PermissionsTracker.RequiredPermissionNodes);
            bool hasRequiredPlayerPermissions = this.PermissionsTracker.RequiredPlayerPermissions == 0 || Sender.CheckPermission(this.PermissionsTracker.RequiredPlayerPermissions);
            if (!hasRequiredPlayerPermissions || !hasRequiredPermissionNodes)
            { 
                Logger.Debug($"Sender doesn't have permissions for command {this.CommandInstance.Command}. " +
                    $"\n[PermissionNodesCheck: {hasRequiredPermissionNodes}, PlayerPermissionsCheck: {hasRequiredPlayerPermissions}] " +
                    $"\n[Permission Nodes: {String.Join(", ", this.PermissionsTracker.RequiredPermissionNodes)} | PlayerPermissions: {this.PermissionsTracker.RequiredPlayerPermissions}]", debug);
                this.Response = $"You do not have permission to execute this command.";
                if (CommandManager.ShowPermissionsBranches || debug)
                {
                    this.Response += " [Required Permissions: ";
                    foreach (var requiredPermissionsNode in this.PermissionsTracker.RequiredPermissionNodes)
                        this.Response += $" {requiredPermissionsNode}, ";
                    if (this.PermissionsTracker.RequiredPermissionNodes.Length > 0)
                        this.Response = this.Response.Substring(0, this.Response.Length - 2);
                    foreach (Enum enumValue in Enum.GetValues(typeof(PlayerPermissions)))
                    {
                        if (this.PermissionsTracker.RequiredPlayerPermissions.HasFlag(enumValue))
                            this.Response += $"{enumValue}, ";
                    }
                    if ((int)this.PermissionsTracker.RequiredPlayerPermissions > 0)
                        this.Response = this.Response.Substring(0, this.Response.Length - 2);
                    this.Response += "]";
                }
                this.Allowed = false;
                return false;
            }
        }
        catch (Exception e)
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