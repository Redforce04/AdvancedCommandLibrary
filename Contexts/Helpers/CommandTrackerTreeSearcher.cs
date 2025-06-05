// -----------------------------------------------------------------------
// <copyright file="CommandTrackerTreeSearcher.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Contexts.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using LabApi.Features.Console;
using Trackers;

/// <summary>
/// The command tree searcher used to find and list subcommands.
/// </summary>
internal static class CommandTrackerTreeSearcher
{
    /// <summary>
    /// Finds and builds a search string recursively.
    /// </summary>
    /// <param name="priorString">The previous string builder.</param>
    /// <param name="search">The previous search result.</param>
    /// <param name="depth">The current search depth.</param>
    /// <returns>A string builder with the results.</returns>
    internal static StringBuilder RecursivelyBuildString(ref StringBuilder priorString, SearchResult search, int depth)
    {
        if (depth == 10)
        {
            Logger.Warn("Command iterative string builder has reached a depth of 10 or more. This is bad and likely a bug.");
            return priorString;
        }

        string padding = string.Empty.PadRight(depth * 4, '-');
        priorString.AppendLine($"{(depth > 0 ? " |" : string.Empty)}{padding}{(depth > 0 ? ">" : string.Empty)} {search.Name} {search.Args}-> {search.Description}");

        foreach (SearchResult child in search.Children)
        {
            RecursivelyBuildString(ref priorString, child, depth + 1);
        }

        return priorString;
    }

    /// <summary>
    /// Recursively searches a command.
    /// </summary>
    /// <param name="command">The command to search.</param>
    /// <param name="currentDepth">The current search depth.</param>
    /// <returns>A SearchResult with any subcommands and command info.</returns>
    internal static SearchResult RecursivelySearchCommand(CommandTracker command, int currentDepth)
    {
        SearchResult currentCmd = new SearchResult(command, currentDepth);
        if (currentDepth >= 10)
        {
            Logger.Warn("Command iterative search has reached a depth of 10 or more. This is bad and likely a bug.");
            return currentCmd;
        }

        if (command is not ParentCommandTracker parent)
        {
            return currentCmd;
        }

        foreach (CommandTracker childCmd in parent.ChildCommands)
        {
            currentCmd.Children.Add(RecursivelySearchCommand(childCmd, currentDepth + 1));
        }

        return currentCmd;
    }

    /// <summary>
    /// Provides a search result for command trees.
    /// </summary>
    internal struct SearchResult()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResult"/> struct.
        /// </summary>
        /// <param name="cmd">The command to search.</param>
        /// <param name="depth">The current search depth.</param>
        internal SearchResult(CommandTracker cmd, int depth)
            : this()
        {
            this.Tracker = cmd;
            this.Name = cmd.Name;
            this.Description = cmd.Description;
            this.Depth = depth;

            if (cmd.Usages.Length > 0)
            {
                this.Args = string.Empty;
            }
            else
            {
                this.Args1 = cmd.Usages.ToList();
            }

            if (cmd.Aliases.Length == 0)
            {
                this.Aliases = string.Empty;
            }
            else
            {
                this.Aliases1 = cmd.Aliases.ToList();
            }
        }

        /// <summary>
        /// Gets the command tracker representing this search.
        /// </summary>
        internal CommandTracker Tracker { get; init; } = null!;

        /// <summary>
        /// Gets or sets a list of children search results.
        /// </summary>
        internal List<SearchResult> Children { get; set; } = new();

        /// <summary>
        /// Gets the current search depth.
        /// </summary>
        internal int Depth { get; init; }

        /// <summary>
        /// Gets the current command's Name.
        /// </summary>
        internal string Name { get; init; } = null!;

        /// <summary>
        /// Gets the current command's arguments as a single string.
        /// </summary>
        internal string Args { get; private set; } = null!;

        /// <summary>
        /// Gets the current command's aliases as a single string.
        /// </summary>
        internal string Aliases { get; private set; } = null!;

        /// <summary>
        /// Gets the current command's description.
        /// </summary>
        internal string Description { get; init; } = null!;

        private List<string> Args1
        {
            init
            {
                foreach (string arg in value)
                {
                    this.Args += $"[{arg}] ";
                }
            }
        }

        private List<string> Aliases1
        {
            init
            {
                if (value.Count == 0)
                {
                    return;
                }

                this.Aliases += "[";
                for (int i = 0; i < value.Count; i++)
                {
                    string alias = value[i];
                    this.Aliases += $"{alias}";
                    if (i == value.Count - 1)
                    {
                        continue;
                    }

                    this.Aliases += " / ";
                }

                this.Aliases += "]";
            }
        }
    }
}