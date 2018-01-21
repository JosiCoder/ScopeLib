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

namespace ScopeLib.Display.Graphics
{
    /// <summary>
    /// Specifies the resize modes available for a scope.
    /// </summary>
    public enum ScopeStretchMode : short
    {
        Expand,
        Stretch,
    }

    /// <summary>
    /// Specifies the line types available for a scope graph.
    /// </summary>
    [Flags]
    public enum ScopeLineType : short
    {
        None = 0,
        Line = 1,
        Dots = 2,
        LineAndDots = 1 + 2,
    }

    /// <summary>
    /// Specifies the lines available for scope cursors.
    /// </summary>
    [Flags]
    public enum ScopeCursorLines : short
    {
        None = 0,
        X = 1,
        Y = 2,
        Both = 1 + 2,
    }

    /// <summary>
    /// Specifies the weight of a cursor line.
    /// </summary>
    public enum ScopeCursorLineWeight : short
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    /// <summary>
    /// Specifies the markers available for scope cursors.
    /// </summary>
    [Flags]
    public enum ScopeCursorMarkers : short
    {
        None = 0,
        XLeft = 1,
        XRight = 2,
        YUpper = 4,
        YLower = 8,
        XFull = 1 + 2,
        YFull = 4 + 8,
        Full = 1 + 2 + 4 + 8,
    }

    /// <summary>
    /// Specifies the alignment references available for items of a scope.
    /// </summary>
    public enum ScopeAlignmentReference : short
    {
        Position,
        XPositionAndVerticalRangeEdge,
        YPositionAndHorizontalRangeEdge,
    }

    /// <summary>
    /// Specifies the horizontal alignment alternatives available for items of a scope.
    /// </summary>
    public enum ScopeHorizontalAlignment : short
    {
        Left,
        Right
    }

    /// <summary>
    /// Specifies the vertical alignment alternatives available for items of a scope.
    /// </summary>
    public enum ScopeVerticalAlignment : short
    {
        Top,
        Bottom
    }
}

