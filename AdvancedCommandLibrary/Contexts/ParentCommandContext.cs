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
        var searchResult = CommandTreeSearcher.RecursivelySearchCommand(this.CommandInstance, 0);
        StringBuilder builder = new();
        this.Response = CommandTreeSearcher.RecursivelyBuildString(ref builder, searchResult, this.Sender, 0).ToString();
    }
}