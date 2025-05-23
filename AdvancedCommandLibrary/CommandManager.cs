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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Attributes;
    using CommandSystem;
    using Contexts.Helpers;
    using Enums;
    using GameCommandModules.Processors;
    using LabApi.Features.Console;
    using Mirror;
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

        internal Dictionary<int, CommandTracker> RegisteredCommands { get; private set; } = new();

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
            foreach (var cmdTracker in Instance.RegisteredCommands.Values.Where(x => x.ParentTrackerInstance is null).ToList())
            {
                try
                {
                    Instance._unregisterCommandsRecursive(cmdTracker, 0);
                }
                catch (Exception ex)
                {
                    Logger.Warn($"Could not unregister command {cmdTracker.Name} because an error occured while trying to unregister the command from a game command handler.");
                    Logger.Debug($"Exception: {ex}", DebugMode >= LoggingMode.Debug);
                }
            }
            Instance.RegisteredCommands = null;
            Instance = null;
        }

        private void _unregisterCommandsRecursive(CommandTracker commandTracker, int depth)
        {
            if (depth >= 10)
            {
                Logger.Warn("Unregister search has reached a depth of 10 or more. This is bad and likely a bug.");
                return;
            }
            if (commandTracker is ParentCommandTracker parentCommandTracker)
            {
                try
                {

                    if (parentCommandTracker.GameCommandInstance is ParentCommandProcessor parentProcessor)
                    {
                        foreach (CommandTracker childCommandTracker in parentCommandTracker.ChildCommands)
                        {
                            try
                            {

                                if (childCommandTracker is ParentCommandTracker parentChildCommandTracker)
                                    this._unregisterCommandsRecursive(parentChildCommandTracker, depth + 1);
                                
                                switch (childCommandTracker.GameCommandInstance)
                                {
                                    case ParentCommandProcessor childParentProcessor:
                                        parentProcessor.UnregisterCommand(childParentProcessor);
                                        break;
                                    case ChildCommandProcessor childCommandProcessor:
                                        parentProcessor.UnregisterCommand(childCommandProcessor);
                                        break;
                                }
                                childCommandTracker.UpdateParentTracker(null);
                                RegisteredCommands.Remove(childCommandTracker.Id);
                            }
                            catch (Exception)
                            {
                                //
                            }
                        }
                    }

                    parentCommandTracker.UpdateChildren([]);
                    parentCommandTracker.UpdateParentTracker(null);
                }
                catch (Exception ex)
                {
                    //
                }
            }
            if (depth != 0)
                return;

            try
            {

                if (commandTracker.HandlerType.HasFlag(CommandHandlerType.ClientConsole))
                    ClientCommandHandler.UnregisterCommand(commandTracker.GameCommandInstance);

                if (commandTracker.HandlerType.HasFlag(CommandHandlerType.RemoteAdmin))
                    RemoteAdminCommandHandler.UnregisterCommand(commandTracker.GameCommandInstance);

                if (commandTracker.HandlerType.HasFlag(CommandHandlerType.GameConsole))
                    GameConsoleCommandHandler.UnregisterCommand(commandTracker.GameCommandInstance);
            }
            catch (Exception)
            {
                //
            }
        }
        
        private void _logCommandTree()
        {
            if (DebugMode < LoggingMode.Insanity)
                return;
            try
            {
                StringBuilder builder = new();
                builder.AppendLine($"Registered Commands:");
                foreach (CommandTracker tracker in RegisteredCommands.Values.Where(x => x.ParentTrackerInstance is null))
                {
                    var searchResult = CommandTrackerTreeSearcher.RecursivelySearchCommand(tracker, 0);
                    CommandTrackerTreeSearcher.RecursivelyBuildString(ref builder, searchResult, 0);
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
                        PermissionsTracker? permTracker = this._findAttributes<RequirePermissionsAttribute>(method) is { Count: > 0 } permsAtr ? new PermissionsTracker(permsAtr) : null;

                        // Check the method for CommandAttribute
                        if (this._findAttribute<CommandAttribute>(method) is not { } cmdAtr)
                            continue;
                        
                        // Create a tracker to track the command.
                        CommandTracker tracker = cmdAtr is ParentCommandAttribute ? new ParentCommandTracker()
                            {
                                Name = cmdAtr.Name, Description = cmdAtr.Description, Aliases = cmdAtr.Aliases ?? [], Usages = cmdAtr.Usages ?? [],
                                HandlerType = cmdAtr.HandlerType, PermissionsRequirementTracker = permTracker, Method = method, Assembly = pluginAssembly,
                            }
                            : new CommandTracker()
                            {
                                Name = cmdAtr.Name, Description = cmdAtr.Description, Aliases = cmdAtr.Aliases ?? [], Usages = cmdAtr.Usages ?? [],
                                HandlerType = cmdAtr.HandlerType, PermissionsRequirementTracker = permTracker, Method = method, Assembly = pluginAssembly,
                            };

                        if (tracker is ParentCommandTracker parentTracker && this._findAttribute<LoadGeneratedCommandsExecutorAttribute>(method) is { Method: not null } loadExecutor)
                            parentTracker.LoadGeneratedCommandsExecutor = loadExecutor.Method;
                        
                        
                        // Then register the tracker in our tracking lists.
                        this.RegisteredCommands[tracker.Id] = tracker;

                        Logger.Debug($"[Found {(tracker is ParentCommandTracker ? "Parent" : "Child ")} Command] Name: \"{tracker.Name}\" [{tracker.Id}]", DebugMode >= LoggingMode.Ludicrous);
                    }
                    catch (Exception e)
                    {
                        Logger.Warn($"An error has occured ");
                    }

                }
            }
            Logger.Debug($"Resulting SearchAssemblies: [Commands: {RegisteredCommands.Count}]", DebugMode >= LoggingMode.Ludicrous);
        }
        
        private T? _findAttribute<T>(MethodInfo method) where T : Attribute
        {
            if (Attribute.GetCustomAttribute(method, typeof(T)) is T attribute)
            {
                return attribute;
            }
            return null;
        }
        
        [MemberNotNull]
        private List<T> _findAttributes<T>(MethodInfo method) where T : Attribute
        {
            if (Attribute.GetCustomAttributes(method, typeof(T)) is T[] attributes)
            {
                return attributes.ToList();
            }
            return new List<T>();
        }

        private void _assignChildCommands()
        {
            Logger.Debug($"[===== Assign Child Command Module =====] Assigning Parents & Children to any eligible commands.", DebugMode >= LoggingMode.Ludicrous);
            
            // Foreach Command Check if it has a ParentAttribute, then ensure the parent is actually valid. Then associate both the parents and the children with each other.
            foreach (CommandTracker immutableTracker in RegisteredCommands.Values.ToList())
            {
                CommandTracker tracker = immutableTracker;
                Logger.Debug($"[Checking {(tracker is ParentCommandTracker ? "Parent" : "Child")}] \"{tracker.Name}\".", DebugMode >= LoggingMode.Ludicrous);
                if (_findAttribute<ParentAttribute>(tracker.Method) is { } parentAtr)
                {
                    if (!_tryGetParentCommand(ref tracker, parentAtr, out ParentCommandTracker? parent))
                        continue;
                    
                    tracker.UpdateParentTracker(parent!);
                    parent!.AddChild(tracker);
                    
                    RegisteredCommands[tracker.Id] = tracker;
                    RegisteredCommands[parent!.Id] = parent;
                    continue;
                }
                // If no parents, then add to list as a base parent command.
                RegisteredCommands[tracker.Id] = tracker;

                Logger.Debug($"====> No Parents Found. Base Command.", DebugMode >= LoggingMode.Ludicrous);
            }

        }

        [MemberNotNullWhen(true)]
        private bool _tryGetParentCommand(ref CommandTracker tracker, ParentAttribute parentAtr, out ParentCommandTracker? parentCommandTracker)
        {
            parentCommandTracker = null;
            MethodInfo? method = parentAtr.ParentBaseType.GetMethod(name: parentAtr.ParentName, bindingAttr: BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static);
            if (method is null)
            {
                Logger.Warn($"Could not find the specified parent command for the command \"{tracker.Name}\". The command and any children will not be registered.");
                Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}(ParentContext ctx)\" could not be found. Remember that it must be a public, static, method with only ParentContext as the parameters.",
                    DebugMode >= LoggingMode.Debug);
                return false;
            }

            if (Attribute.GetCustomAttribute(method, typeof(ParentCommandAttribute)) is not ParentCommandAttribute parentCmd)
            {
                Logger.Warn($"The parent command for command \"{tracker.Name}\" is missing the ParentCommandAttribute. The command and any children will not be registered.");
                Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}(ParentContext ctx)\" did not have the [ParentCommand] Attribute.", DebugMode >= LoggingMode.Debug);
                return false;
            }
            ParentCommandTracker? parent = RegisteredCommands.Values.FirstOrDefault(x => x is ParentCommandTracker && x.Method == method) as ParentCommandTracker;
            if (parent is null)
            {
                Logger.Warn($"The parent command for command \"{tracker.Name}\" Could not be found. This is likely due to a bug. The command and any children will not be registered.");
                Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}(ParentContext ctx)\" was found, however it was not registered which should have already occured by this point. This is likely a framework bug.",
                    DebugMode >= LoggingMode.Debug);
                return false;
            }
            parentCommandTracker = parent;
            Logger.Debug($"====> Parent Found: \"{parent.Name}\".", DebugMode >= LoggingMode.Ludicrous);
            return true;
        }

        private void _initializeGameCommands()
        {
            Logger.Debug($"[===== Initialize Game Commands Module =====] Initializing Child & Parent Command Processors.", DebugMode >= LoggingMode.Ludicrous);
            try
            {
                foreach (CommandTracker cmd in RegisteredCommands.Values.ToList())
                    if(cmd is ParentCommandTracker parent)
                        this.RegisteredCommands[cmd.Id].GameCommandInstance = new ParentCommandProcessor(parent);
                    else
                        this.RegisteredCommands[cmd.Id].GameCommandInstance = new ChildCommandProcessor(cmd);
                // Load generated commands after all the instances are fully initialized.
                foreach (CommandTracker x in RegisteredCommands.Values)
                {
                    if(x.GameCommandInstance is not ParentCommandProcessor parent)
                        continue;
                    parent.LoadGeneratedCommands();
                }
            }
            catch (Exception e)
            {
                Logger.Error("Could not initialize game commands because of an error.");
                Logger.Debug($"Exception: \n{e}", DebugMode >= LoggingMode.Debug);
            }
            Logger.Debug($"Resulting InitializeGameCommands: [Commands: {RegisteredCommands.Count}]", DebugMode >= LoggingMode.Ludicrous);
        }

        private void _registerGameCommands()
        {
            Logger.Debug($"[===== Register Game Commands Module =====] Registering Parent & Child Command Processors to their relevant game command handler modules.", DebugMode >= LoggingMode.Ludicrous);

            string[] gameConsoleAliases = GameConsoleCommandHandler.AllCommands.SelectMany(x => x.Aliases ?? []).ToArray();
            string[] gameConsoleCommands = GameConsoleCommandHandler.AllCommands.Select(x => x.Command ?? "").ToArray();
            string[] clientConsoleAliases = ClientCommandHandler.AllCommands.SelectMany(x => x.Aliases ?? []).ToArray();
            string[] clientConsoleCommands = ClientCommandHandler.AllCommands.Select(x => x.Command ?? "").ToArray();
            string[] remoteAdminAliases = RemoteAdminCommandHandler.AllCommands.SelectMany(x => x.Aliases ?? []).ToArray();
            string[] remoteAdminCommands = RemoteAdminCommandHandler.AllCommands.Select(x => x.Command ?? "").ToArray();
            Logger.Debug($"All currently registered commands:", DebugMode >= LoggingMode.Ludicrous);

            foreach (CommandTracker cmd in RegisteredCommands.Values)
                Logger.Debug($"====> [{(cmd is ParentCommandTracker { } parent ? $"Parent - {parent.ChildCommands.Count} Children" : "Child")}] {cmd.Name} - {(cmd.ParentTrackerInstance is null ? "[Base Command]" : $"(Nested Command - Parent: {cmd.ParentTrackerInstance?.Name})")} ", DebugMode >= LoggingMode.Ludicrous);

            Logger.Debug($"Registering Base Commands:", DebugMode >= LoggingMode.Ludicrous);
            foreach (CommandTracker cmd in RegisteredCommands.Values.Where(x => x.ParentTrackerInstance is null))
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
                Logger.Debug($"====> \"{cmd.Name}\"{modes}", DebugMode >= LoggingMode.Ludicrous);

            }
        }
    }
}
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
