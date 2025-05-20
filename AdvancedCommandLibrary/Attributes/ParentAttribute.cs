
using System;

namespace AdvancedCommandLibrary.Attributes;

using System.Reflection;

// Instead of parsing here, I want to parse this in the loader so it is safer.
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class ParentAttribute(Type parentBaseType, string parentName) : Attribute
{
    public Type ParentBaseType { get; } = parentBaseType;

    public string ParentName { get; } = parentName;
}