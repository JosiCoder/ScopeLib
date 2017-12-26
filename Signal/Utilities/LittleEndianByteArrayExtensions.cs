//------------------------------------------------------------------------------
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

namespace ScopeLib.Signal
{
    /// <summary>
    /// Converts numeric values to their little-endian byte representations
    /// and vice versa.
    /// </summary>
    public static class LittleEndianByteArrayExtensions
    {
        //== Convert values to byte arrays ==//

        /// <summary>
        /// Converts a 16 bit value to its little-endian byte representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] Int16ToBytes(this short value)
        {
            return NumberToBytes(value, val => BitConverter.GetBytes(val));
        }

        /// <summary>
        /// Converts a 32 bit value to its little-endian byte representation.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] Int32ToBytes(this int value)
        {
            return NumberToBytes(value, val => BitConverter.GetBytes(val));
        }

        /// <summary>
        /// Converts a numerical value to its little-endian byte representation.
        /// </summary>
        /// <typeparam name="T">The type of the numerical value.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The little-endian byte representation.</returns>
        public static byte[] NumberToBytes<T>(this T value, Func<T, byte[]> valueConverter)
        {
            var bytes = valueConverter(value);
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return bytes;
        }

        //== Convert byte arrays to values ==//

        /// <summary>
        /// Converts the little-endian byte representation of a 16 bit value to the
        /// according value.
        /// </summary>
        /// <param name="bytes">The byte representation to convert.</param>
        /// <returns>The value.</returns>
        public static short BytesToInt16(this byte[] bytes)
        {
            return BytesToNumber(bytes, inBytes => BitConverter.ToInt16(inBytes, 0));
        }

        /// <summary>
        /// Converts the little-endian byte representation of a 32 bit value to the
        /// according value.
        /// </summary>
        /// <param name="bytes">The byte representation to convert.</param>
        /// <returns>The value.</returns>
        public static int BytesToInt32(this byte[] bytes)
        {
            return BytesToNumber(bytes, inBytes => BitConverter.ToInt32(inBytes, 0));
        }

        /// <summary>
        /// Converts the little-endian byte representation of a numerical value to the
        /// according value.
        /// </summary>
        /// <typeparam name="T">The type of the numerical value.</typeparam>
        /// <param name="bytes">The byte representation to convert.</param>
        /// <returns>The value.</returns>
        public static T BytesToNumber<T>(this byte[] bytes, Func<byte[], T> valueConverter)
        {
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return valueConverter(bytes);
        }
    }
}

