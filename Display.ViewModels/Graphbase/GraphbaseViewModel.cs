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
    /// Provides the viewmodel of a scope graphbase (e.g. timebase).
    /// </summary>
    public class GraphbaseViewModel : ViewModelBase
    {
        private readonly Color _defaultColor = new Color (1, 1, 1);

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public GraphbaseViewModel ()
        {
            BaseUnitString = "s";
            ScaleFactor = 1.0;
            Color = _defaultColor;
            Initialize();
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="baseUnitString">The string representing the base unit.</param>
        /// <param name="scaleFactor">The scaling factor for the base axis.</param>
        /// <param name="color">The graph color.</param>
        public GraphbaseViewModel (string baseUnitString, double scaleFactor, Color color)
        {
            BaseUnitString = baseUnitString;
            ScaleFactor = scaleFactor;
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
            TriggerVM = new NullTriggerViewModel ();
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

        private double _scaleFactor;
        /// <summary>
        /// Gets or sets the scaling factor for the base axis.
        /// </summary>
        public double ScaleFactor
        {
            get
            {
                return _scaleFactor;
            }
            set
            {
                _scaleFactor = value;
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
        /// Gets or sets the viewmodel of the second measurement cursor.
        /// </summary>
        public MeasurementCursorViewModel MeasurementCursor2VM
        { get; private set; }

        private ITriggerViewModel _triggerVM;
        /// <summary>
        /// Gets or sets the trigger viewmodel.
        /// </summary>
        public ITriggerViewModel TriggerVM
        {
            get
            {
                return _triggerVM;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentException ("null not allowed for trigger");
                }
                _triggerVM = value;
                RaisePropertyChanged();
            }
        }
    }
}

