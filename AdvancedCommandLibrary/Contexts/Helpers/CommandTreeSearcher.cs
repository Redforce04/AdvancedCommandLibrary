// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         ParentCommandSearcher.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:59
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Contexts.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using AdvancedCommandLibrary.GameCommandModules;
using AdvancedCommandLibrary.GameCommandModules.Processors;
using CommandSystem;
using LabApi.Features.Console;
using LabApi.Features.Permissions;

internal class CommandTreeSearcher
{
    internal static StringBuilder RecursivelyBuildString(ref StringBuilder priorString, SearchResult search, ICommandSender sender, int depth)
        {
            if (depth == 10)
            {
                Logger.Warn("Command iterative string builder has reached a depth of 10 or more. This is bad and likely a bug.");
                return priorString;
            }
            string padding = string.Empty.PadRight(depth * 4, '-');
            if (sender is ServerConsoleSender)
                priorString.AppendLine($"{(depth > 0 ? " |" : "")}{padding}{(depth > 0 ? ">" : "")} {search.Name} {search.Args}-> {search.Description}");
            else
                priorString.AppendLine($"{(depth > 0 ? " <color=orange>|" : "")}{padding}{(depth > 0 ? ">" : "")}<color=yellow> {search.Name} {search.Args}<color=white>-> {search.Description}");
            
            foreach (SearchResult child in search.Children)
            {
                if (child.Command is ChildCommandProcessor { Tracker.PermissionsRequirementTracker: { } tracker })
                {
                    if (tracker.RequiredPermissionNodes.Length > 0 && !sender.HasPermissions(tracker.RequiredPermissionNodes))
                        continue;
                    if (tracker.RequiredPlayerPermissions > 0 && !sender.CheckPermission(tracker.RequiredPlayerPermissions))
                        continue;
                }
                RecursivelyBuildString(ref priorString, child, sender, depth + 1);
            }
            return priorString;
        }

        internal static SearchResult RecursivelySearchCommand(ICommand command, int currentDepth)
        {
            var currentCmd = new SearchResult(command, currentDepth);
            if (currentDepth >= 10)
            {
                Logger.Warn("Command iterative search has reached a depth of 10 or more. This is bad and likely a bug.");
                return currentCmd;
            }
    
            if (command is not ParentCommand parent)
            {
                return currentCmd;
            }
            foreach (ICommand childCmd in parent.AllCommands)
            {
                currentCmd.Children.Add(RecursivelySearchCommand(childCmd, currentDepth + 1));
            }
            return currentCmd;
        }
    internal struct SearchResult()
    {
        internal SearchResult(ICommand cmd, int depth) : this()
        {
            this.Command = cmd;
            this.Name = cmd.Command;
            this.Description = cmd.Description;
            this.PermissionNode = (cmd is ICommandPermission perms) ? perms.PermissionNode : "";
            this.Depth = depth;
            
            if (cmd is not IUsageProvider usage)
                this.Args = "";
            else
                this._args = usage.Usage.ToList();
            
            if (cmd.Aliases == null || cmd.Aliases.Length == 0)
                this.Aliases = "";
            else
                this._aliases = cmd.Aliases.ToList();
        }
        internal ICommand Command { get; init; }
        internal List<SearchResult> Children { get; set; } = new();
        internal int Depth { get; init; }
        internal string PermissionNode { get; init; }
        internal string Name { get; init; }
        internal string Args { get; private set; }
        internal string Aliases { get; private set; }
        internal string Description { get; init; }

        private List<string> _args
        {
            init
            {
                foreach (string arg in value)
                {
                    this.Args += $"[{arg}] ";
                }
            }
        }
        private List<string> _aliases
        {
            init
            {
                if (value.Count == 0)
                    return;
                this.Aliases += "[";
                for (int i = 0; i < value.Count; i++)
                {
                    string aliase = value[i];
                    this.Aliases += $"{aliase}";
                    if (i == value.Count - 1)
                        continue;
                    this.Aliases += " / ";
                }
                this.Aliases += "]";
            }
        }
    }
}