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
using Gtk;
using Cairo;

namespace ScopeLib.Display
{
    /// <summary>
    /// Provides information about a cursor and the selection state of its cursor lines.
    /// </summary>
    public class ScopeCursorSelection
    {
        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="cursor">The cursor this object belongs to.</param>
        /// <param name="selectedLines">A value indicating which lines are selected.</param>
        public ScopeCursorSelection (ScopeCursor cursor, ScopeCursorLines selectedLines)
        {
            Cursor = cursor;
            SelectedLines = selectedLines;
        }

        /// <summary>
        /// Gets or sets the cursor this object belongs to.
        /// </summary>
        /// <value>The cursor.</value>
        public ScopeCursor Cursor
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating which lines are selected.
        /// </summary>
        public ScopeCursorLines SelectedLines
        { get; set; }
    }
}

