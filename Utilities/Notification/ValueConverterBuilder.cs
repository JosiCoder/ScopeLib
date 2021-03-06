﻿//------------------------------------------------------------------------------
// Copyright (C) 2017 Josi Coder

// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.

// This program is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.

// You should have received a copy of the GNU General Public License along with
// this program. If not, see <http://www.gnu.org/licenses/>.
//--------------------------------------------------------------------------------

using System;

namespace ScopeLib.Utilities
{
    /// <summary>
    /// Builds value converters.
    /// </summary>
    public static class ValueConverterBuilder
    {
        /// <summary>
        /// Builds a value converter used to convert an enumeration value from its
        /// integer to its string representation and vice versa.
        /// </summary>
        /// <typeparam name="TEnum">
        /// The type of the enumeration the converter is intended to be used for.
        /// </typeparam>
        /// <returns>A new enumeration value converter.</returns>
        public static ValueConverter<int, string> BuildEnumValueConverter<TEnum>()
        {
            return new ValueConverter<int, string>(
                val =>
            {
                var enumValue = Enum.ToObject(typeof(TEnum), val);
                return enumValue.ToString();
            },
                val =>
            {
                var enumValue = (TEnum)Enum.Parse(typeof(TEnum), val);
                return Convert.ToInt32(enumValue);
            });
        }

        /// <summary>
        /// Builds a value converter used to convert an integer value to its string
        /// representation and vice versa.
        /// </summary>
        /// <returns>A new integer value converter.</returns>
        public static ValueConverter<int, string> BuildInt32ValueConverter()
        {
            return new ValueConverter<int, string>(
                val => val.ToString(),
                val => Convert.ToInt32(val));
        }
    }
}

