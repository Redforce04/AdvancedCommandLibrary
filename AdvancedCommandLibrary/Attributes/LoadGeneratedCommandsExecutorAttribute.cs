// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         AdvancedCommandLibrary
//    Project:          AdvancedCommandLibrary
//    FileName:         LoadGeneratedCommandsExecutorAttribute.cs
//    Author:           Redforce04#4091
//    Revision Date:    05/23/2025 13:01
//    Created Date:     05/23/2025 13:05
// -----------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;
using System.Reflection;
using Contexts.Helpers;
using LabApi.Features.Console;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class LoadGeneratedCommandsExecutorAttribute : Attribute
{
    public LoadGeneratedCommandsExecutorAttribute(Type type, string methodName)
    {
        this.Method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
        if (this.Method is null)
        {
            Logger.Warn("Could not find LoadGeneratedCommandsExecutorAttribute method. Please ensure it is a Public Static method that use LoadGeneratedCommandsContext.");
            return;
        }
        var parameters = this.Method.GetParameters();
        if (parameters.Length != 1 || parameters[0].ParameterType != typeof(LoadGeneratedCommandsContext))
        {
            this.Method = null;
            Logger.Warn("A LoadGeneratedCommandsExecutor method was found however it had incorrect method parameters. The method parameters should look like public static void OnLoadGeneratedCommands(LoadGeneratedCommandsContext context).");
            return;
        }
    }
    public MethodInfo? Method { get; } 
    
}