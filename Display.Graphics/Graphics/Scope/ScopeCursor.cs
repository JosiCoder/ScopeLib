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
using ScopeLib.Utilities;

namespace ScopeLib.Display.Graphics
{
    /// <summary>
    /// Provides a cursor used on scope displays.
    /// </summary>
    public class ScopeCursor : NotifyingBase
    {
        private readonly Color _defaultColor = new Color (1, 1, 1);

        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        public ScopeCursor ()
        {
            Position = new ScopePosition();
            Lines = ScopeCursorLines.Both;
            HighlightedLines = ScopeCursorLines.None;
            SelectableLines = ScopeCursorLines.Both;
            Color = _defaultColor;
            Markers = ScopeCursorMarkers.None;
            Captions = new ScopePositionCaption[0];
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="lines">A value indicating which lines are visible.</param>
        /// <param name="lineWeight">The weight of the cursor lines.</param>
        /// <param name="selectableLines">A value indicating which lines are selectable.</param>
        /// <param name="color">The cursor color.</param>
        /// <param name="markers">A value indicating which markers are visible.</param>
        /// <param name="captions">A list of captions.</param>
        public ScopeCursor (ScopePosition position, ScopeCursorLines lines, ScopeCursorLineWeight lineWeight,
            ScopeCursorLines selectableLines, Color color,
            ScopeCursorMarkers markers, IEnumerable<ScopePositionCaption> captions)
        {
            Position = position;
            Lines = lines;
            LineWeight = lineWeight;
            HighlightedLines = ScopeCursorLines.None;
            SelectableLines = selectableLines;
            Color = color;
            Markers = markers;
            Captions = captions;
        }

        private ScopePosition _position;
        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public ScopePosition Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating which lines are visible.
        /// </summary>
        public ScopeCursorLines Lines
        { get; set; }

        /// <summary>
        /// Gets or sets the weight of the cursor lines.
        /// </summary>
        public ScopeCursorLineWeight LineWeight
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which lines are highlighted.
        /// </summary>
        public ScopeCursorLines HighlightedLines
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which lines are selectable.
        /// </summary>
        public ScopeCursorLines SelectableLines
        { get; set; }

        /// <summary>
        /// Gets or sets the cursor color.
        /// </summary>
        public Color Color
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which markers are visible.
        /// </summary>
        public ScopeCursorMarkers Markers
        { get; set; }

        /// <summary>
        /// Gets or sets a list of captions.
        /// </summary>
        public IEnumerable<ScopePositionCaption> Captions
        { get; set; }

        /// <summary>
        /// Gets or sets a list of values to draw ticks for on the horizontal (X) axis.
        /// </summary>
        public IEnumerable<ScopeCursorValueTick> XTicks
        { get; set; }

        /// <summary>
        /// Gets or sets a list of values to draw ticks for on the vertical (Y) axis.
        /// </summary>
        public IEnumerable<ScopeCursorValueTick> YTicks
        { get; set; }
    }
}

