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
    /// Provides the viewmodel of a scope channel.
    /// </summary>
    public class ChannelViewModel : NotifyingBase
    {
        private readonly Color _defaultColor = new Color (1, 1, 1);

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public ChannelViewModel ()
        {
            BaseUnitString = "V";
            ReferencePointPosition = new Position ();
            XScaleFactor = 1.0;
            YScaleFactor = 1.0;
            Color = _defaultColor;
            Initialize();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="baseUnitString">The string representing the base unit.</param>
        /// <param name="referencePointPosition">
        /// The the position of the reference point on the scope display.
        /// </param>
        /// <param name="xScaleFactor">The scaling factor in the horizontal direction.</param>
        /// <param name="yScaleFactor">The scaling factor in the vertical direction.</param>
        /// <param name="color">The graph color.</param>
        public ChannelViewModel (string baseUnitString, Position referencePointPosition,
            double xScaleFactor, double yScaleFactor, Color color)
        {
            BaseUnitString = baseUnitString;
            ReferencePointPosition = referencePointPosition;
            XScaleFactor = xScaleFactor;
            YScaleFactor = yScaleFactor;
            Color = color;
            Initialize();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        private void Initialize()
        {
            MeasurementCursor1VM = new MeasurementCursorViewModel();
            MeasurementCursor2VM = new MeasurementCursorViewModel();
        }

        private string _baseUnitString;
        /// <summary>
        /// Gets or sets the string representing the base unit.
        /// </summary>
        public string BaseUnitString
        {
            get
            {
                return _baseUnitString;
            }
            set
            {
                _baseUnitString = value;
                RaisePropertyChanged();
            }
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

        private double _xScaleFactor;
        /// <summary>
        /// Gets or sets the scaling factor in the horizontal direction.
        /// </summary>
        public double XScaleFactor
        {
            get
            {
                return _xScaleFactor;
            }
            set
            {
                _xScaleFactor = value;
                RaisePropertyChanged();
            }
        }

        private double _yScaleFactor;
        /// <summary>
        /// Gets or sets the scaling factor in the vertical direction.
        /// </summary>
        public double YScaleFactor
        {
            get
            {
                return _yScaleFactor;
            }
            set
            {
                _yScaleFactor = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the graph color.
        /// </summary>
        public Color Color
        { get; set; }

        /// <summary>
        /// Gets the viewmodel of the first measurement cursor.
        /// </summary>
        public MeasurementCursorViewModel MeasurementCursor1VM
        { get; private set; }

        /// <summary>
        /// Gets the viewmodel of the second measurement cursor.
        /// </summary>
        public MeasurementCursorViewModel MeasurementCursor2VM
        { get; private set; }
    }
}

