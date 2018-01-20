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
    /// Provides a graph used on scope displays.
    /// </summary>
    public class ScopeGraph
    {
        private readonly Color _defaultColor = new Color (1, 1, 1);

        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        public ScopeGraph ()
        {
            ReferencePoint = new PointD ();
            ReferencePointPosition = new PointD ();
            Vertices = new PointD[0];
            XScaleFactor = 1.0;
            YScaleFactor = 1.0;
            LineType = ScopeLineType.Line;
            Color = _defaultColor;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="referencePoint">
        /// The reference point. The X and Y values refer to the units used for the vertices.
        /// </param>
        /// <param name="referencePointPosition">
        /// The position of the reference point on the scope display.
        /// The X and Y values refer to the screen graticule.
        /// </param>
        /// <param name="vertices">The vertices describing the graph.</param>
        /// <param name="xScaleFactor">The scaling factor for the horizontal (X) direction.</param>
        /// <param name="yScaleFactor">The scaling factor for the vertical (Y) direction.</param>
        /// <param name="graphType">The graph line type.</param>
        /// <param name="color">The graph color.</param>
        public ScopeGraph (PointD referencePoint, PointD referencePointPosition, IEnumerable<PointD> vertices,
            double xScaleFactor, double yScaleFactor, ScopeLineType lineType, Color color)
        {
            ReferencePoint = referencePoint;
            ReferencePointPosition = referencePointPosition;
            Vertices = vertices;
            XScaleFactor = xScaleFactor;
            YScaleFactor = yScaleFactor;
            LineType = lineType;
            Color = color;
        }

        /// <summary>
        /// Gets or sets the reference point. The X and Y values refer to the units used
        /// for the vertices.
        /// </summary>
        public PointD ReferencePoint
        { get; set; }

        /// <summary>
        /// Gets or sets the position of the reference point on the scope display.
        /// The X and Y values refer to the screen graticule.
        /// </summary>
        public PointD ReferencePointPosition
        { get; set; }

        /// <summary>
        /// Gets or sets the vertices describing the graph.
        /// </summary>
        public IEnumerable<PointD> Vertices
        { get; set; }

        /// <summary>
        /// Gets or sets the scaling factor for the horizontal (X) direction.
        /// </summary>
        public double XScaleFactor
        { get; set; }

        /// <summary>
        /// Gets or sets the scaling factor for the vertical (Y) direction.
        /// </summary>
        public double YScaleFactor
        { get; set; }

        /// <summary>
        /// Gets or sets the graph line type.
        /// </summary>
        public ScopeLineType LineType
        { get; set; }

        /// <summary>
        /// Gets or sets the graph color.
        /// </summary>
        public Color Color
        { get; set; }
    }
}

