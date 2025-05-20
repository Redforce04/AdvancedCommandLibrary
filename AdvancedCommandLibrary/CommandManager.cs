// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         CommandManager.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/19/2025 16:33
//    Created Date:     05/19/2025 16:05
// -----------------------------------------

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
namespace AdvancedCommandLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Attributes;
    using CommandSystem;
    using Contexts.Helpers;
    using Enums;
    using GameCommandModules.Processors;
    using LabApi.Features.Console;
    using RemoteAdmin;
    using Trackers;

    public class CommandManager
    {
        private CommandManager() { }

        /// <summary>
        /// The main instance of the Command Manager. Make sure to initialize by calling <see cref="LoadCommandManager"/> otherwise it will be null.
        /// </summary>
        public static CommandManager Instance { get; private set; } = null!;
        /// <summary>
        /// Gets or sets a value that indicates if debug logs should be shown.
        /// </summary>
        public static LoggingMode DebugMode { get; set; }
        
        /// <summary>
        /// Gets or sets a value that indicates if the missing required permission branches will be shown when a user doesn't have permission to execute the command.
        /// </summary>
        public static bool ShowPermissionsBranches { get; set; }

        internal List<CommandTracker> RegisteredCommands { get; private set; } = new();
        private static RemoteAdminCommandHandler RemoteAdminCommandHandler => CommandProcessor.RemoteAdminCommandHandler; 
        private static GameConsoleCommandHandler GameConsoleCommandHandler => GameCore.Console.singleton.ConsoleCommandHandler; 
        private static ClientCommandHandler ClientCommandHandler => QueryProcessor.DotCommandHandler; 

        /// <summary>
        /// Loads the command manager and registers all commands.
        /// </summary>
        public static void LoadCommandManager()
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (Instance is not null)
            {
                Logger.Warn("The CommandManager has already been loaded.");
                return;
            }
            Instance = new();
            Logger.Debug("Loading All Commands.", DebugMode >= LoggingMode.Debug);
            Instance._loadAllCommands();
        }
        
        /// <summary>
        /// Unloads the command manager and unregisters all loaded commands.
        /// </summary>
        public static void UnloadCommandManager()
        {
            Instance = null;
        }

        private void _logCommandTree()
        {
            if (DebugMode < LoggingMode.Insanity)
                return;
            try
            {

                StringBuilder builder = new();
                builder.AppendLine($"Registered Commands:");
                foreach (CommandTracker tracker in RegisteredCommands.Where(x => x.ParentTrackerInstance is null))
                {
                    var searchResult = CommandTrackerTreeSearcher.RecursivelySearchCommand(tracker, 0);
                    CommandTrackerTreeSearcher.RecursivelyBuildString(ref builder, searchResult, 0).ToString();
                }
                Logger.Debug(builder.ToString());
            }
            catch (Exception e)
            {
                Logger.Debug($"An error has occured while trying to log the command tree.\nException: {e}");
            }
        }
        private void _loadAllCommands()
        {
            Logger.Debug("Searching Assemblies.", DebugMode >= LoggingMode.Debug);
            try
            {
                this._searchAssemblies();
            }
            catch (Exception e)
            {
                Logger.Warn("An error has occured while searching assemblies.");
                Logger.Debug($"Exception: {e}", DebugMode >= LoggingMode.Insanity);
            }
            Logger.Debug("Assigning Child Commands.", DebugMode >= LoggingMode.Debug);
            try
            {
                this._assignChildCommands();
            }
            catch (Exception e)
            {
                Logger.Warn("An error has occured while assigning child commands.");
                Logger.Debug($"Exception: {e}", DebugMode >= LoggingMode.Insanity);
            }
            this._logCommandTree();
            Logger.Debug("Initializing Game Commands.", DebugMode >= LoggingMode.Debug);
            try
            {
                this._initializeGameCommands();
            }
            catch (Exception e)
            {
                Logger.Warn("An error has occured while Initializing game commands.");
                Logger.Debug($"Exception: {e}", DebugMode >= LoggingMode.Insanity);
            }
            Logger.Debug("Registering Game Commands.", DebugMode >= LoggingMode.Debug);
            try
            {
                this._registerGameCommands();
            }
            catch (Exception e)
            {
                Logger.Warn("An error has occured while registering game commands.");
                Logger.Debug($"Exception: {e}", DebugMode >= LoggingMode.Insanity);
            }
            Logger.Info($"{RegisteredCommands.Count} Commands Loaded.");
        }

        private void _searchAssemblies()
        {
            Logger.Debug($"[===== Search Assembly Module =====] Searching for all eligible commands.", DebugMode >= LoggingMode.Ludicrous);

            // Foreach loaded plugin assembly we check all eligible methods for either a ParentCommandAttribute and a CommandAttribute.
            foreach (Assembly pluginAssembly in LabApi.Loader.PluginLoader.Plugins.Values)
            {
                // Get all methods that are Public, Static, and Invokable
                var search = pluginAssembly.GetTypes().SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod));
                foreach (MethodInfo method in search)
                {
                    try
                    {
                        // Check to see if it has a RequirePermissionsAttribute and if it does, register a PermissionsTracker to track the required permissions.
                        PermissionsTracker? permTracker = Attribute.GetCustomAttribute(method, typeof(RequirePermissionsAttribute)) is RequirePermissionsAttribute permsAtr ? new PermissionsTracker(permsAtr) : null;

                        // Check the method for CommandAttribute
                        if (Attribute.IsDefined(method, typeof(CommandAttribute)) && Attribute.GetCustomAttribute(method, typeof(CommandAttribute)) is CommandAttribute cmdAtr)
                        {
                            // Create a tracker to track the command.
                            CommandTracker tracker = cmdAtr is ParentCommandAttribute ? new CommandTracker() : new ParentCommandTracker()
                            {
                                Name = cmdAtr.Name, Description = cmdAtr.Description, Aliases = cmdAtr.Aliases ?? [], Usages = cmdAtr.Usages ?? [],
                                HandlerType = cmdAtr.HandlerType, PermissionsRequirementTracker = permTracker, Method = method, Assembly = pluginAssembly,
                            };

                            // Then register the tracker in our tracking lists.
                            RegisteredCommands.Add(tracker);

                            Logger.Debug($"[Found {(tracker is ParentCommandTracker ? "Parent" : "Child ")} Command] Name: \"{tracker.Name}\"", DebugMode >= LoggingMode.Ludicrous);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Warn($"An error has occured ");
                    }

                }
            }
            Logger.Debug($"Resulting SearchAssemblies: [Commands: {RegisteredCommands.Count}]", DebugMode >= LoggingMode.Ludicrous);
        }

        private void _assignChildCommands()
        {
            bool debug = DebugMode >= LoggingMode.Ludicrous;
            Logger.Debug($"[===== Assign Child Command Module =====] Assigning Parents & Children to any eligible commands.", debug);

            List<CommandTracker> updatedCommands = new();
            
            foreach (CommandTracker tracker in RegisteredCommands)
            {
                Logger.Debug($"[Checking Child ] \"{tracker.Name}\".", debug);
                if (Attribute.GetCustomAttribute(tracker.Method, typeof(ParentAttribute)) is ParentAttribute parentAtr)
                {
                    MethodInfo? method = parentAtr.ParentBaseType.GetMethod(name: parentAtr.ParentName, bindingAttr: BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static );
                    if (method is null)
                    {
                        Logger.Warn($"Could not find the specified parent command for the command \"{tracker.Name}\". The command will not be registered.");
                        Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}(ParentContext ctx)\" could not be found. Remember that it must be a public, static, method with only ParentContext as the parameters.", DebugMode >= LoggingMode.Debug);
                        continue;
                    }

                    if (Attribute.GetCustomAttribute(method, typeof(ParentCommandAttribute)) is not ParentCommandAttribute parentCmd)
                    {
                        Logger.Warn($"The parent command for command \"{tracker.Name}\" is missing the ParentCommandAttribute. The command will not be registered.");
                        Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}(ParentContext ctx)\" did not have the [ParentCommand] Attribute.", DebugMode >= LoggingMode.Debug);
                        continue;
                    }
                    ParentCommandTracker? parent = RegisteredParentCommands.FirstOrDefault(x => x.Method == method);
                    if (parent is null)
                    {
                        Logger.Warn($"The parent command for command \"{tracker.Name}\" Could not be found. This is likely due to a bug. The command will not be registered.");
                        Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}(ParentContext ctx)\" was found, however it was not registered which should have already occured by this point. This is likely a framework bug.", DebugMode >= LoggingMode.Debug);
                        continue;
                    }
                    Logger.Debug($"====> Parent Found: \"{parent.Name}\".", debug);
                    tracker.ParentTrackerInstance = parent;
                    parent.ChildCommands.Add(tracker);
                    if(!updatedCommands.Contains(tracker))
                        updatedParentCommands.Add(parent);
                    if(!updatedParentCommands.Contains(tracker))
                        updatedCommands.Add(tracker);
                    continue;
                }
                // If no parents, then add to list as a base parent command.
                Logger.Debug($"====> No Parents Found. Base Command.", debug);
                updatedCommands.Add(tracker);
            }
            foreach (ParentCommandTracker tracker in RegisteredParentCommands)
            {
                Logger.Debug($"[Checking Parent] \"{tracker.Name}\".", debug);
                if (Attribute.GetCustomAttribute(tracker.Method, typeof(ParentAttribute)) is ParentAttribute parentAtr)
                {
                    MethodInfo? method = parentAtr.ParentBaseType.GetMethod(name: parentAtr.ParentName, bindingAttr: BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static );
                    if (method is null)
                    {
                        Logger.Warn($"Could not find the specified parent command for the sub-parent command \"{tracker.Name}\". The sub-parent command and it's children will not be registered.");
                        Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}\"(ParentContext ctx) could not be found. Remember that it must be a public, static, method with only ParentContext as the parameters.", DebugMode >= LoggingMode.Debug);
                        if(updatedParentCommands.Contains(tracker))
                            updatedParentCommands.Remove(tracker);
                        foreach(var childCmd in tracker.ChildCommands)
                            if(updatedCommands.Contains(childCmd))
                                updatedCommands.Remove(childCmd);
                        continue;
                    }

                    if (Attribute.GetCustomAttribute(method, typeof(ParentCommandAttribute)) is not ParentCommandAttribute parentCmd)
                    {
                        Logger.Warn($"The parent command for sub-parent command \"{tracker.Name}\" is missing the ParentCommandAttribute. The sub-parent command and it's children will not be registered.");
                        Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}\"(ParentContext ctx) did not have the [ParentCommand] Attribute.", DebugMode >= LoggingMode.Debug);
                        if(updatedParentCommands.Contains(tracker))
                            updatedParentCommands.Remove(tracker);
                        foreach(var childCmd in tracker.ChildCommands)
                            if(updatedCommands.Contains(childCmd))
                                updatedCommands.Remove(childCmd);
                        continue;
                    } 
                    ParentCommandTracker? parent = RegisteredParentCommands.FirstOrDefault(x => x.Method == method);
                    if (parent is null)
                    {
                        Logger.Warn($"The parent command for sub-parent command \"{tracker.Name}\" Could not be found. This is likely due to a bug. The sub-parent command and it's children will not be registered.");
                        Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}\"(ParentContext ctx) was found, however it was not registered which should have already occured by this point. This is likely a framework bug.", DebugMode >= LoggingMode.Debug);
                        if(updatedParentCommands.Contains(tracker))
                            updatedParentCommands.Remove(tracker);
                        foreach(var childCmd in tracker.ChildCommands)
                            if(updatedCommands.Contains(childCmd))
                                updatedCommands.Remove(childCmd);
                        continue;
                    }
                    Logger.Debug($"====> Parent Found: \"{parent.Name}\".", debug);
                    tracker.ParentTrackerInstance = parent;
                    parent.ChildCommands.Add(tracker);
                    if(!updatedParentCommands.Contains(tracker))
                        updatedParentCommands.Add(parent);
                    if(!updatedParentCommands.Contains(tracker))
                        updatedParentCommands.Add(tracker);
                    continue;
                }
                // If no parents, then add to list as a base parent command.
                updatedParentCommands.Add(tracker);
                Logger.Debug($"====> No Parents Found. Base Command.", debug);
            }
            // Outright replace original lists to prevent collection modified exceptions.
             RegisteredCommands = updatedCommands;
             RegisteredParentCommands = updatedParentCommands;
             Logger.Debug($"Resulting AssignChildCommands: [Child: {RegisteredCommands.Count}], [Parents: {RegisteredParentCommands.Count}]", debug);
        }

        private void _initializeGameCommands()
        {
            bool debug = DebugMode >= LoggingMode.Ludicrous;
            Logger.Debug($"[===== Initialize Game Commands Module =====] Initializing Child & Parent Command Processors.", debug);

            try
            {
                List<ParentCommandTracker> newParentCommands = new();
                List<CommandTracker> newCommands = new();
                foreach (CommandTracker cmd in RegisteredCommands)
                {
                    
                    cmd.GameCommandInstance = new ChildCommandProcessor(cmd);
                    newCommands.Add(cmd);
                }
                foreach (ParentCommandTracker parentCmd in RegisteredParentCommands)
                {
                    parentCmd.GameCommandInstance = new ParentCommandProcessor(parentCmd);
                    newParentCommands.Add(parentCmd);
                }
                this.RegisteredCommands = newCommands;
                this.RegisteredParentCommands = newParentCommands;
            }
            catch (Exception e)
            {
                Logger.Error("Could not initialize game commands because of an error.");
                Logger.Debug($"Exception: \n{e}", DebugMode >= LoggingMode.Debug);
            }
            Logger.Debug($"Resulting InitializeGameCommands: [Child: {RegisteredCommands.Count}], [Parents: {RegisteredParentCommands.Count}]", debug);
        }

        private void _registerGameCommands()
        {   
            bool debug = DebugMode >= LoggingMode.Ludicrous;
            Logger.Debug($"[===== Register Game Commands Module =====] Registering Parent & Child Command Processors to their relevant game command handler modules.", debug);

            string[] gameConsoleAliases = GameConsoleCommandHandler.AllCommands.SelectMany(x => x.Aliases ?? []).ToArray();
            string[] gameConsoleCommands = GameConsoleCommandHandler.AllCommands.Select(x => x.Command ?? "").ToArray();
            string[] clientConsoleAliases = ClientCommandHandler.AllCommands.SelectMany(x => x.Aliases ?? []).ToArray();
            string[] clientConsoleCommands= ClientCommandHandler.AllCommands.Select(x => x.Command ?? "").ToArray();
            string[] remoteAdminAliases = RemoteAdminCommandHandler.AllCommands.SelectMany(x => x.Aliases ?? []).ToArray();
            string[] remoteAdminCommands = RemoteAdminCommandHandler.AllCommands.Select(x => x.Command ?? "").ToArray(); 
            Logger.Debug($"All currently registered commands:", debug);
            foreach (ParentCommandTracker cmd in RegisteredParentCommands)
            {
                Logger.Debug($"====> [Parent {(cmd is { } parent ? $"- {parent.ChildCommands.Count} Children" : "" )}] {cmd.Name} - {(cmd.ParentTrackerInstance?.Name is null ? $"(Nested Command - Parent: {cmd.ParentTrackerInstance?.Name})" : "[Base Command]")} ", debug);
            }
            foreach (CommandTracker cmd in RegisteredCommands)
            {
                Logger.Debug($"====> [Child] {cmd.Name} - {(cmd.ParentTrackerInstance?.Name is null ? $"(Nested Command - Parent: {cmd.ParentTrackerInstance?.Name})" : "[Base Command]")} ", debug);
            }
            
            Logger.Debug($"Registering Base Child Commands:", debug);
            foreach (CommandTracker cmd in RegisteredCommands.Where(x => x.ParentTrackerInstance is null))
            {
                string modes = "";
                try
                { 
                    if (cmd.HandlerType.HasFlag(CommandHandlerType.ClientConsole))
                    {
                        if (clientConsoleCommands.Any(x => string.Equals(cmd.Name, x, StringComparison.CurrentCultureIgnoreCase)))
                            Logger.Warn($"Could not register child command \"{cmd.Name}\" to ClientConsoleCommandHandler because a command with a similar name already exists.");
                        else if (clientConsoleAliases.Any(x => cmd.Aliases.Any(y => string.Equals(y, x, StringComparison.CurrentCultureIgnoreCase))))
                            Logger.Warn($"Could not register child command \"{cmd.Name}\" to ClientConsoleCommandHandler because a command with a similar alias already exists.");
                        else
                        {
                            modes += " [Client Console]";
                            ClientCommandHandler.RegisterCommand(cmd.GameCommandInstance);
                        }
                    }

                    if (cmd.HandlerType.HasFlag(CommandHandlerType.RemoteAdmin))
                    {
                        if (remoteAdminCommands.Any(x => string.Equals(cmd.Name, x, StringComparison.CurrentCultureIgnoreCase)))
                            Logger.Warn($"Could not register child command \"{cmd.Name}\" to RemoteAdminCommandHandler because a command with a similar name already exists.");
                        else if (remoteAdminAliases.Any(x => cmd.Aliases.Any(y => string.Equals(y, x, StringComparison.CurrentCultureIgnoreCase))))
                            Logger.Warn($"Could not register child command \"{cmd.Name}\" to RemoteAdminCommandHandler because a command with a similar alias already exists.");
                        else
                        {
                            modes += " [Remote Admin]";
                            RemoteAdminCommandHandler.RegisterCommand(cmd.GameCommandInstance);
                        }
                    }

                    if (cmd.HandlerType.HasFlag(CommandHandlerType.GameConsole))
                    {
                        if (gameConsoleCommands.Any(x => string.Equals(cmd.Name, x, StringComparison.CurrentCultureIgnoreCase)))
                            Logger.Warn($"Could not register child command \"{cmd.Name}\" to GameConsoleCommandHandler because a command with a similar name already exists.");
                        else if (gameConsoleAliases.Any(x => cmd.Aliases.Any(y => string.Equals(y, x, StringComparison.CurrentCultureIgnoreCase))))
                            Logger.Warn($"Could not register child command \"{cmd.Name}\" to GameConsoleCommandHandler because a command with a similar alias already exists.");
                        else
                        {
                            modes += " [Game Console]";
                            GameConsoleCommandHandler.RegisterCommand(cmd.GameCommandInstance);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Warn($"Could not register game command {cmd.Name} due to an error.");
                    Logger.Debug($"Error: \n{e}", DebugMode >= LoggingMode.Debug);
                }
                Logger.Debug($"====> \"{cmd.Name}\"{modes}", debug);
                
            }
            
            Logger.Debug($"Registering Base Parent Commands:", debug);
            foreach (ParentCommandTracker parentCmd in RegisteredParentCommands.Where(x => x.ParentTrackerInstance is null))
            {
                string parentModes = "";
                try
                {
                    if (parentCmd.HandlerType.HasFlag(CommandHandlerType.ClientConsole))
                    {
                        if (clientConsoleCommands.Any(x => string.Equals(parentCmd.Name, x, StringComparison.CurrentCultureIgnoreCase)))
                            Logger.Warn($"Could not register parent command \"{parentCmd.Name}\" to ClientConsoleCommandHandler because a command with a similar name already exists.");
                        else if (clientConsoleAliases.Any(x => parentCmd.Aliases.Any(y => string.Equals(y, x, StringComparison.CurrentCultureIgnoreCase))))
                            Logger.Warn($"Could not register parent command \"{parentCmd.Name}\" to ClientConsoleCommandHandler because a command with a similar alias already exists.");
                        else
                        {
                            ClientCommandHandler.RegisterCommand(parentCmd.GameCommandInstance);
                            parentModes += " [Client Console]";
                        }
                    }

                    if (parentCmd.HandlerType.HasFlag(CommandHandlerType.RemoteAdmin))
                    {
                        if (remoteAdminCommands.Any(x => string.Equals(parentCmd.Name, x, StringComparison.CurrentCultureIgnoreCase)))
                            Logger.Warn($"Could not register parent command \"{parentCmd.Name}\" to RemoteAdminCommandHandler because a command with a similar name already exists.");
                        else if (remoteAdminAliases.Any(x => parentCmd.Aliases.Any(y => string.Equals(y, x, StringComparison.CurrentCultureIgnoreCase))))
                            Logger.Warn($"Could not register parent command \"{parentCmd.Name}\" to RemoteAdminCommandHandler because a command with a similar alias already exists.");
                        else
                        {
                            RemoteAdminCommandHandler.RegisterCommand(parentCmd.GameCommandInstance);
                            parentModes += " [Remote Admin]";
                        }
                    }

                    if (parentCmd.HandlerType.HasFlag(CommandHandlerType.GameConsole))
                    {
                        if (gameConsoleCommands.Any(x => string.Equals(parentCmd.Name, x, StringComparison.CurrentCultureIgnoreCase)))
                            Logger.Warn($"Could not register parent command \"{parentCmd.Name}\" to GameConsoleCommandHandler because a command with a similar name already exists.");
                        else if (gameConsoleAliases.Any(x => parentCmd.Aliases.Any(y => string.Equals(y, x, StringComparison.CurrentCultureIgnoreCase))))
                            Logger.Warn($"Could not register parent command \"{parentCmd.Name}\" to GameConsoleCommandHandler because a command with a similar alias already exists.");
                        else
                        {
                            GameConsoleCommandHandler.RegisterCommand(parentCmd.GameCommandInstance);
                            parentModes += " [Game Console]";
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.Warn($"Could not register game command {parentCmd.Name} due to an error.");
                    Logger.Debug($"Error: \n{e}", DebugMode >= LoggingMode.Debug);
                }
                Logger.Debug($"====> \"{parentCmd.Name}\"{parentModes}", debug);
            }
        }
    }
}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
