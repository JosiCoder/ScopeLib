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
using System.Text;
using System.IO;

namespace ScopeLib.Signal
{
    /// <summary>
    /// Reads and writes values to and from a stream representing a waveform file.
    /// </summary>
    public static class WaveformFileStreamExtensions
    {
        //== Write values to stream ==//

        /// <summary>
        /// Writes a string value to the specified output stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        public static void WriteString(this Stream stream, string value)
        {
            stream.WriteBytes(new ASCIIEncoding().GetBytes(value));
        }

        /// <summary>
        /// Writes a 16 bit value to the specified output stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        public static void WriteInt16(this Stream stream, short value)
        {
            stream.WriteNumber(value, val => BitConverter.GetBytes(val));
        }

        /// <summary>
        /// Writes a 32 bit value to the specified output stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        public static void WriteInt32(this Stream stream, int value)
        {
            stream.WriteNumber(value, val => BitConverter.GetBytes(val));
        }

        /// <summary>
        /// Writes a numerical value to the specified output stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        public static void WriteNumber<T>(this Stream stream, T value, Func<T, byte[]> valueConverter)
        {
            stream.WriteBytes(value.NumberToBytes(valueConverter));
        }

        /// <summary>
        /// Writes bytes to the specified output stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="value">The value to write.</param>
        public static void WriteBytes(this Stream stream, byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
        }

        //== Read values from stream ==//

        /// <summary>
        /// Reads a string value from the specified input stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The value read.</returns>
        public static string ReadAsString(this Stream stream, int byteCount)
        {
            var streamBytes = stream.ReadBytes(byteCount);
            return new ASCIIEncoding().GetString(streamBytes);
        }

        /// <summary>
        /// Reads a 16 bit value from the specified input stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The value read.</returns>
        public static short ReadAsInt16(this Stream stream)
        {
            return ReadAsNumber(stream, 2, bytes => BitConverter.ToInt16(bytes, 0));
        }

        /// <summary>
        /// Reads a 32 bit value from the specified input stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The value read.</returns>
        public static int ReadAsInt32(this Stream stream)
        {
            return ReadAsNumber(stream, 4, bytes => BitConverter.ToInt32(bytes, 0));
        }

        /// <summary>
        /// Reads numerical value from the specified input stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The value read.</returns>
        public static T ReadAsNumber<T>(this Stream stream, int byteCount, Func<byte[], T> valueConverter)
        {
            var streamBytes = stream.ReadBytes(byteCount);
            return streamBytes.BytesToNumber(valueConverter);
        }

        /// <summary>
        /// Skips bytes from the specified input stream.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        /// <param name="count">The number of bytes to skip.</param>
        public static void SkipBytes(this Stream stream, int count)
        {
            stream.Seek(count, SeekOrigin.Current);
        }

        /// <summary>
        /// Reads bytes from the specified input stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <returns>The bytes read.</returns>
        public static byte[] ReadBytes(this Stream stream, int count)
        {
            var bytes = new byte[count];
            var countRead = stream.Read(bytes, 0, count);
            if (countRead < count)
            {
                throw new EndOfStreamException("End of stream reached.");
            }
            return bytes;
        }
    }
}

