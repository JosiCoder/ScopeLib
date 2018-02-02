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
    /// Provides the configuration of the scope timebase.
    /// </summary>
    public class TimebaseConfiguration : NotifyingBase
    {
        private readonly Color _defaultColor = new Color (1, 1, 1);

        /// <summary>
        /// Initializes an instance of this class with default settings.
        /// </summary>
        public TimebaseConfiguration ()
        {
            BaseUnitString = "s";
            TimeScaleFactor = 1.0;
            Color = _defaultColor;
            Initialize();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="baseUnitString">The string representing the base unit.</param>
        /// <param name="xScaleFactor">The scaling factor for the time axis.</param>
        /// <param name="yScaleFactor">The scaling factor for the value axis.</param>
        /// <param name="color">The graph color.</param>
        public TimebaseConfiguration (string baseUnitString, double timeScaleFactor, Color color)
        {
            BaseUnitString = baseUnitString;
            TimeScaleFactor = timeScaleFactor;
            Color = color;
            Initialize();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        private void Initialize()
        {
            MeasurementCursorA = new MeasurementCursorConfiguration();
            MeasurementCursorB = new MeasurementCursorConfiguration();
            TriggerConfiguration = new NullTriggerConfiguration ();
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

        /// <summary>
        /// Gets or sets the graph color.
        /// </summary>
        public Color Color
        { get; set; }

        /// <summary>
        /// Gets or sets the first measurement cursor.
        /// </summary>
        public MeasurementCursorConfiguration MeasurementCursorA
        { get; private set; }

        /// <summary>
        /// Gets or sets the second measurement cursor.
        /// </summary>
        public MeasurementCursorConfiguration MeasurementCursorB
        { get; private set; }

        private ITriggerViewModel _triggerConfiguration;
        /// <summary>
        /// Gets or sets the trigger configuration.
        /// </summary>
        public ITriggerViewModel TriggerConfiguration
        {
            get
            {
                return _triggerConfiguration;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException ("null trigger not allowed");
                }
                _triggerConfiguration = value;
                RaisePropertyChanged();
            }
        }
    }
}

