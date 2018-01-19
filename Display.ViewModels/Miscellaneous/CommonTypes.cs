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

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides a point on the scope display.
    /// </summary>
    public struct Point
    {
        public Point (double x, double y)
        {
            X = x;
            Y = y;
        }

        public double X
        { get; set; }

        public double Y
        { get; set; }
    }

    /// <summary>
    /// Provides a color.
    /// </summary>
    public struct Color
    {
        public Color (double r, double g, double b)
        {
            R = r;
            G = g;
            B = b;
        }

        public double R
        { get; set; }

        public double G
        { get; set; }

        public double B
        { get; set; }
    }

}

