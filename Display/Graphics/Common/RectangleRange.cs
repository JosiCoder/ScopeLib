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
using Cairo;

namespace ScopeLib.Display
{
    /// <summary>
    /// Provides a rectangular range graphics is rendered to, based on device and user-specific
    /// units and a transformation matrix.
    /// </summary>
    public class RectangleRange
    {
        private readonly Matrix _matrix = null;
        private readonly double _xSpan;
        private readonly double _ySpan;
        private readonly Distance _originOffset;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="deviceWidth">The range width, in device units.</param>
        /// <param name="deviceHeight">The range height, in device units.</param>
        /// <param name="xSpan">The horizontal (X) span, in user-specific units.</param>
        /// <param name="ySpan">The vertical (Y) span, in user-specific units.</param>
        /// <param name="originOffset">The offset of the origin to the range center, in user-specific units.</param>
        /// <param name="initialMatrix">The matrix used to transform user-specific to device units.</param>
        public RectangleRange (int deviceWidth, int deviceHeight, double xSpan, double ySpan, Distance originOffset, Matrix matrix)
        {
            DeviceWidth = deviceWidth;
            DeviceHeight = deviceHeight;
            _xSpan = xSpan;
            _ySpan = ySpan;
            _originOffset = originOffset;
            _matrix = matrix;
        }

        /// <summary>
        /// Gets the range width, in device units.
        /// </summary>
        public int DeviceWidth
        { get; private set; }

        /// <summary>
        /// Gets the range height, in device units.
        /// </summary>
        public int DeviceHeight
        { get; private set; }

        /// <summary>
        /// Gets the minimum X value, in user-specific units.
        /// </summary>
        public double MinX
        {
            get { return -_xSpan / 2f - _originOffset.Dx; }
        }

        /// <summary>
        /// Gets the maximum X value, in user-specific units.
        /// </summary>
        public double MaxX
        {
            get { return _xSpan / 2f - _originOffset.Dx; }
        }

        /// <summary>
        /// Gets the minimum Y value, in user-specific units.
        /// </summary>
        public double MinY
        {
            get { return -_ySpan / 2f - _originOffset.Dy; }
        }

        /// <summary>
        /// Gets the maximum Y value, in user-specific units.
        /// </summary>
        public double MaxY
        {
            get { return _ySpan / 2f - _originOffset.Dy; }
        }

        /// <summary>
        /// Gets the matrix used to transform user-specific to device units. This matrix
        /// already considers the <c>originOffset</c>.
        /// </summary>
        public Matrix Matrix
        {
            get { return _matrix; }
        }
    }
}

