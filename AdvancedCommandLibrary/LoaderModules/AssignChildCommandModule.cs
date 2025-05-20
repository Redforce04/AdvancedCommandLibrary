// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         AssignChildCommandModule.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/20/2025 17:07
//    Created Date:     05/20/2025 17:05
// -----------------------------------------

namespace AdvancedCommandLibrary.LoaderModules;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Attributes;
using Enums;
using LabApi.Features.Console;
using Trackers;

public class AssignChildCommandModule
{
    private List<CommandTracker> RegisteredCommands => CommandManager.Instance.RegisteredCommands;
    private LoggingMode DebugMode => CommandManager.DebugMode;
    private void _assignChildCommands()
        {
            Logger.Debug($"[===== Assign Child Command Module =====] Assigning Parents & Children to any eligible commands.", DebugMode >= LoggingMode.Ludicrous);

            Dictionary<int, CommandTracker> updatedCommands = new();
            
            // Foreach Command Check if it has a ParentAttribute, then ensure the parent is actually valid. Then associate both the parents and the children with each other.
            foreach (CommandTracker registeredTracker in RegisteredCommands)
            {
                
                CommandTracker tracker = updatedCommands.TryGetValue(registeredTracker.Id, out CommandTracker trackerUpdated) ? trackerUpdated : registeredTracker;
                Logger.Debug($"[Checking {(tracker is ParentCommandTracker ? "Parent" : "Child")}] \"{tracker.Name}\".", DebugMode >= LoggingMode.Ludicrous);
                if (Attribute.GetCustomAttribute(tracker.Method, typeof(ParentAttribute)) is ParentAttribute parentAtr)
                {
                    if (!_tryGetParentCommand(ref tracker, parentAtr, out ParentCommandTracker? parent))
                    {
                        continue;
                    }

                    if (updatedCommands.ContainsKey(tracker.Id))
                        updatedCommands[tracker.Id] = tracker;
                    updatedCommands.Add(tracker.Id, tracker);
                    if (updatedCommands.ContainsKey(parent!.Id))
                        updatedCommands[parent.Id] = parent;
                    updatedCommands.Add(parent.Id, parent);
                    continue;
                }
                // If no parents, then add to list as a base parent command.
                updatedCommands[tracker.Id] = tracker;
                
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
        ParentCommandTracker? parent = RegisteredCommands.FirstOrDefault(x => x is ParentCommandTracker && x.Method == method) as ParentCommandTracker;
        if (parent is null)
        {
            Logger.Warn($"The parent command for command \"{tracker.Name}\" Could not be found. This is likely due to a bug. The command and any children will not be registered.");
            Logger.Debug($"Method \"{parentAtr.ParentBaseType.FullName}.{parentAtr.ParentName}(ParentContext ctx)\" was found, however it was not registered which should have already occured by this point. This is likely a framework bug.", DebugMode >= LoggingMode.Debug);
            return false;
        }
        Logger.Debug($"====> Parent Found: \"{parent.Name}\".", DebugMode >= LoggingMode.Ludicrous);
        tracker.ParentTrackerInstance = parent;
        parent.ChildCommands.Add(tracker);
        parentCommandTracker = tracker.ParentTrackerInstance;
        return true;
    }
}