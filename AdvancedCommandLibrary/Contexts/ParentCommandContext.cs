// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         ParentCommandContext.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:46
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Contexts;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using CommandSystem;
using Enums;
using GameCommandModules.Processors;
using Helpers;
using LabApi.Features.Console;
using LabApi.Features.Permissions;
using LabApi.Features.Wrappers;
using RemoteAdmin;
using Trackers;

public class ParentCommandContext : CommandContext
{
    internal ParentCommandContext(ArraySegment<string> args, ICommandSender sender, string response, ParentCommand command, ParentCommand? parentCommand = null, PermissionsTracker? permissionsTracker = null) : base(args, sender, response, command, parentCommand, permissionsTracker)
    {
    }
    
    public void RespondWithSubCommands()
    {
        if (this.CommandInstance is not ParentCommandProcessor cmd)
            return;
        Logger.Debug($"Command \"{cmd.Command}\" children: {cmd.AllCommands.Count()}", CommandManager.DebugMode >= LoggingMode.Ludicrous);
        StringBuilder builder = new ();
        builder.AppendLine($"Command Usage:");
        var searchResult = CommandTrackerTreeSearcher.RecursivelySearchCommand(cmd.Tracker, 0);
        CommandTrackerTreeSearcher.RecursivelyBuildString(ref builder, searchResult, 0);
        this.Response = builder.ToString();
    }
}