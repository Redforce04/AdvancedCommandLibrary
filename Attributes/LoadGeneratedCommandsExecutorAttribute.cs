// -----------------------------------------------------------------------
// <copyright file="LoadGeneratedCommandsExecutorAttribute.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Attributes;

using System;
using System.Reflection;

using Contexts.Helpers;
using LabApi.Features.Console;

/// <summary>
/// Used to indicate a method to call when a parent command has LoadGeneratedCommands Invoked.
/// </summary>
// ReSharper disable ClassNeverInstantiated.Global
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class LoadGeneratedCommandsExecutorAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadGeneratedCommandsExecutorAttribute"/> class.
    /// </summary>
    /// <param name="type">The type that declares the method to invoke.</param>
    /// <param name="methodName">The name of the method to invoke.</param>
    public LoadGeneratedCommandsExecutorAttribute(Type type, string methodName)
    {
        this.Method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod);
        if (this.Method is null)
        {
            Logger.Warn("Could not find LoadGeneratedCommandsExecutorAttribute method. Please ensure it is a Public Static method that use LoadGeneratedCommandsContext.");
            return;
        }

        ParameterInfo[] parameters = this.Method.GetParameters();
        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(LoadGeneratedCommandsContext))
        {
            return;
        }

        this.Method = null;
        Logger.Warn("A LoadGeneratedCommandsExecutor method was found however it had incorrect method parameters. The method parameters should look like public static void OnLoadGeneratedCommands(LoadGeneratedCommandsContext context).");
    }

    /// <summary>
    /// Gets the method to invoke.
    /// </summary>
    public MethodInfo? Method { get; }
}