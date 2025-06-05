// -----------------------------------------------------------------------
// <copyright file="BitwiseExtensions.cs" company="Redforce04">
// Copyright (c) Redforce04. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace AdvancedCommandLibrary.Extensions;

using System;
using System.Globalization;

/// <summary>
/// A collection of extensions for Bitwise Operations (Enum Flags).
/// </summary>
public static class BitwiseExtensions
{
    /*public static T AppendSeparateEnumValue<T>(this T value, Enum append) where T : Enum
    {
        Enum[] valuesToAppend = (Enum[])Enum.GetValues(append.GetType());
        Enum[] currentValues = (Enum[])Enum.GetValues(value.GetType());
        object result = value;
        for (int i = 0; i < currentValues.Length && i < valuesToAppend.Length; i++)
        {
            if (!append.HasFlag(valuesToAppend[i]))
            {
                continue;
            }

            if (!Enum.IsDefined(((Enum)valuesToAppend[i]).GetType(), result))
                continue;
            if (((Enum)result).HasFlag(valuesToAppend[i]))
            {
                continue;
            }

            // ((Enum)result).SetFlag(valuesToAppend[i], true);
        }
    }*/

    /// <summary>
    /// Checks to see if the provided enum has specific flags.
    /// </summary>
    /// <param name="value">The enum value to check for flags.</param>
    /// <param name="requiredValues">The enum containing the flags that will be checked.</param>
    /// <returns>True if value has the required flags specified in requiredValues. Otherwise, false.</returns>
    public static bool HasRequiredFlags(this Enum value, Enum requiredValues)
    {
        foreach (Enum flag in Enum.GetValues(requiredValues.GetType()))
        {
            if (requiredValues.HasFlag(flag) && !value.HasFlag(flag))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Returns an enum with all flags set to true.
    /// </summary>
    /// <param name="value">The item with the enum type that will be returned with full flags.</param>
    /// <typeparam name="T">The enum type that will be returned with full flags.</typeparam>
    /// <returns>An enum of type T with all flags set to full.</returns>
    public static T IncludeAll<T>(this T value)
        where T : Enum
    {
        Type type = value.GetType();
        object result = value;
        string[] names = Enum.GetNames(type);
        foreach (string name in names)
        {
            ((Enum)result).Include(Enum.Parse(type, name));
        }

        return (T)result;

        // Enum.Parse(type, result.ToString());
    }

    /// <summary>
    /// Includes an enumerated type and returns the new value.
    /// </summary>
    /// <param name="value">The original enum to use as a base value.</param>
    /// <param name="append">The enum to append to value.</param>
    /// <typeparam name="T">The enum type that will be returned.</typeparam>
    /// <returns>The combined values of value and append returned in the type of T.</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static T Include<T>(this Enum value, T append)
    {
        if (append is null)
        {
            throw new ArgumentNullException(nameof(append));
        }

        Type type = value.GetType();

        // Determine the values.
        object result = value;
        Value parsed = new Value(append, type);

        if (parsed.Signed is { } signedLong)
        {
            result = Convert.ToInt64(value) | signedLong;
        }
        else if (parsed.Unsigned is { } unsignedLong)
        {
            result = Convert.ToUInt64(value) | unsignedLong;
        }

        // Return the final value.
        return (T)Enum.Parse(type, result.ToString());
    }

    /// <summary>
    /// Check to see if a flags enumeration has a specific flag set.
    /// </summary>
    /// <param name="valueToCheck">The enum value to check.</param>
    /// <param name="checkingFor">The enum flag being checked for.</param>
    /// <returns>True if valueToCheck has the flag checkingFor.</returns>
    public static bool HasFlag(this Enum valueToCheck, Enum checkingFor)
    {
        // Not as good as the .NET 4 version of this function but should be good enough.
        if (!Enum.IsDefined(valueToCheck.GetType(), checkingFor))
        {
            throw new ArgumentException(string.Format(
                "Enumeration type mismatch.  The flag is of type '{0}', " +
                "was expecting '{1}'.",
                checkingFor.GetType(),
                valueToCheck.GetType()));
        }

        ulong num = Convert.ToUInt64(checkingFor);
        return (Convert.ToUInt64(valueToCheck) & num) == num;
    }

    /// <summary>
    /// Sets the value of a specific enum flag to true or false, and returns the resulting enum.
    /// </summary>
    /// <param name="originalValue">The original enum to modify.</param>
    /// <param name="flagToUpdate">The enum flag that will be modified.</param>
    /// <param name="newFlagValue">The new value of the flagToUpdate.</param>
    /// <typeparam name="T">The type of the enums.</typeparam>
    /// <returns>OriginalValue with the flagToUpdate set to newFlagValue type-casted to T.</returns>
    public static T SetFlag<T>(this T originalValue, T flagToUpdate, bool newFlagValue)
        where T : struct, IComparable, IFormattable, IConvertible
    {
        int flagsInt = originalValue.ToInt32(NumberFormatInfo.CurrentInfo);
        int flagInt = flagToUpdate.ToInt32(NumberFormatInfo.CurrentInfo);
        if (newFlagValue)
        {
            flagsInt |= flagInt;
        }
        else
        {
            flagsInt &= ~flagInt;
        }

        return (T)(object)flagsInt;
    }

    /// <summary>
    /// Removes an enumerated type and returns the new value.
    /// </summary>
    /// <param name="originalValue">The original value to be modified.</param>
    /// <param name="flagToRemove">The flag or value to remove from originalValue.</param>
    /// <typeparam name="T">The type to be returned.</typeparam>
    /// <returns>OriginalValue with the flagToRemove removed, returned as T.</returns>
    public static T Remove<T>(this Enum originalValue, T flagToRemove)
    {
        if (flagToRemove is null)
        {
            throw new ArgumentNullException(nameof(flagToRemove));
        }

        Type type = originalValue.GetType();

        // Determine the values.
        object result = originalValue;
        Value parsed = new Value(flagToRemove, type);
        if (parsed.Signed is { } signedLong)
        {
            result = Convert.ToInt64(originalValue) & ~signedLong;
        }
        else if (parsed.Unsigned is { } unsignedLong)
        {
            result = Convert.ToUInt64(originalValue) & ~unsignedLong;
        }

        // Return the final value.
        return (T)Enum.Parse(type, result.ToString());
    }

    /// <summary>
    /// A class to simplify narrowing values between an ulong and long, since either value should cover any lesser value.
    /// </summary>
    private class Value
    {
        /// <summary>
        /// The resulting signed value.
        /// </summary>
        #pragma warning disable SA1401
        internal readonly long? Signed;

        /// <summary>
        /// The resulting unsigned value.
        /// </summary>
        internal readonly ulong? Unsigned;
        #pragma warning restore SA1401

        // Cached comparisons for tye to use
        private static readonly Type UInt32 = typeof(long);
        private static readonly Type UInt64 = typeof(ulong);

        public Value(object value, Type type)
        {
            // Make sure it is even an enum to work with.
            if (!type.IsEnum)
            {
                throw new ArgumentException("Value provided is not an enumerated type!");
            }

            // Then check for the enumerated value.
            Type compare = Enum.GetUnderlyingType(type);

            // If this is an unsigned long then the only value that can hold it would be an ulong.
            if (compare == UInt32 || compare == UInt64)
            {
                this.Unsigned = Convert.ToUInt64(value);
            }

            // Otherwise, a long should cover anything else.
            else
            {
                this.Signed = Convert.ToInt64(value);
            }
        }
    }
}