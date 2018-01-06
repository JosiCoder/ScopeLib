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
using System.Collections.Generic;
using Cairo;

namespace ScopeLib.Display.Graphics
{
    /// <summary>
    /// Provides a value tick used with scope cursors.
    /// </summary>
    public class ScopeCursorValueTick
    {
        private readonly Color _defaultCaptionColor = new Color(1,1,1);

        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        public ScopeCursorValueTick ()
        {
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="captions">A list of captions.</param>
        public ScopeCursorValueTick (double value, IEnumerable<ScopePositionCaption> captions = null)
        {
            Value = value;

            // By default, the tick has one caption showing its value.
            Captions = captions ?? new []
            {
                new ScopePositionCaption(() => this.Value.ToString(),
                    ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top,
                    ScopeAlignmentReference.Position, false, _defaultCaptionColor)
            };
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public double Value
        { get; set; }

        /// <summary>
        /// Gets or sets a list of captions.
        /// </summary>
        public IEnumerable<ScopePositionCaption> Captions
        { get; set; }
    }
}

