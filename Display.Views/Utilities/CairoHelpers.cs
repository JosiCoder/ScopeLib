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

namespace ScopeLib.Display.Views
{
    /// <summary>
    /// Provides some helpers for Cairo graphics.
    /// </summary>
    public static class CairoHelpers
    {
        /// <summary>
        /// Converts a native point to a Cairo point.
        /// </summary>
        public static Cairo.PointD ToCairoPointD(ScopeLib.Display.ViewModels.Point point)
        {
            return new Cairo.PointD(point.X, point.Y);
        }

        /// <summary>
        /// Converts a native color to a Cairo color.
        /// </summary>
        public static Cairo.Color ToCairoColor(ScopeLib.Display.ViewModels.Color color)
        {
            return new Cairo.Color(color.R, color.G, color.B);
        }
    }
}

