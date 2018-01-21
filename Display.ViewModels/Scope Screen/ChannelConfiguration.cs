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
using ScopeLib.Utilities;

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides the configuration of a scope channel.
    /// </summary>
    public class ChannelConfiguration : NotifyingBase
    {
        private readonly Color _defaultColor = new Color (1, 1, 1);

        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        public ChannelConfiguration ()
        {
            ReferencePointPosition = new Position ();
            TimeScaleFactor = 1.0;
            ValueScaleFactor = 1.0;
            Color = _defaultColor;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="referencePointPosition">
        /// The the position of the reference point on the scope display.
        /// </param>
        /// <param name="xScaleFactor">The scaling factor for the time axis.</param>
        /// <param name="yScaleFactor">The scaling factor for the value axis.</param>
        /// <param name="color">The graph color.</param>
        public ChannelConfiguration (Position referencePointPosition, double timeScaleFactor, double valueScaleFactor,
            Color color)
        {
            ReferencePointPosition = referencePointPosition;
            TimeScaleFactor = timeScaleFactor;
            ValueScaleFactor = valueScaleFactor;
            Color = color;
        }

        /// <summary>
        /// Gets or sets the position of the reference point on the scope display.
        /// The X value specifies the horizontal distance of the reference point from
        /// trigger point position.
        /// The Y value specifies the vertical distance of the reference point from
        /// the horizontal center line.
        /// </summary>
        public Position ReferencePointPosition
        { get; set; }

        private double _timeScaleFactor;
        /// <summary>
        /// Gets or sets the scaling factor for the time axis.
        /// </summary>
        public double TimeScaleFactor
        {
            get
            {
                return _timeScaleFactor;
            }
            set
            {
                _timeScaleFactor = value;
                RaisePropertyChanged();
            }
        }

        private double _valueScaleFactor;
        /// <summary>
        /// Gets or sets the scaling factor for the value axis.
        /// </summary>
        public double ValueScaleFactor
        {
            get
            {
                return _valueScaleFactor;
            }
            set
            {
                _valueScaleFactor = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the graph color.
        /// </summary>
        public Color Color
        { get; set; }
    }
}

