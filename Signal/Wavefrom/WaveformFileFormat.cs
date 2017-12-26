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
    /// Desribes the format of a waveform file.
    /// </summary>
    internal static class WaveformFileFormat
    {
        /// <summary>
        /// Represents a header of a data chunk of a waveform file.
        /// </summary>
        public class ChunkHeader
        {
            /// <summary>
            /// Initializes an instance of this class.
            /// </summary>
            /// <param name="id">The chunk's identifier.</param>
            /// <param name="payloadSize">
            /// The size of the chunk's payload, i.e. excluding the chunk's header.
            /// </param>
            public ChunkHeader (string id, int payloadSize)
            {
                Id = id;
                PayloadSize = payloadSize;
            }

            /// <summary>
            /// Gets or sets the chunk's identifier.
            /// </summary>
            public string Id
            { get; set; }

            /// <summary>
            /// Gets or sets the size of the chunk's payload, i.e. excluding the chunk's header.
            /// </summary>
            public int PayloadSize
            { get; set; }
        }

        public const string RootChunkId = "RIFF";
        public const string FormatChunkId = "fmt ";
        public const string DataChunkId = "data";
        public const string RiffType = "WAVE";
        public const int ChunkHeaderSize = 8;
        public const int FormatChunkPayloadSize = 16;
        public const short PcmFormatTag = 1;
    }
}

