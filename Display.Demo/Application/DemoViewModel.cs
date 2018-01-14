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
using ScopeLib.Utilities;
using ScopeLib.Display.ViewModels;

namespace ScopeLib.Display.Demo
{
    /// <summary>
    /// Provides the demo viewmodel that acts as the entry point.
    /// </summary>
    public class DemoViewModel
    {
        private readonly IScopeScreenViewModel _scopeScreenVM = new ScopeScreenViewModel();

        public DemoViewModel ()
        {
            var channel2TimeIncrement = 2 * Math.PI / 40;
            var channel2values =
                FunctionValueGenerator.GenerateSineValuesForAngles(0.0, 2 * Math.PI, channel2TimeIncrement,
                (x, y) => y);

            _scopeScreenVM.TriggerConfiguration =
                new LevelTriggerConfiguration(1, 0.5);

            _scopeScreenVM.ChannelConfigurations = new[]
            {
                new ChannelConfiguration(new Point(1.0, 1.0), 0.5, 0.3, new Color(1, 1, 0)),
                new ChannelConfiguration(new Point(-Math.PI, 0), 1, 1, new Color(0, 1, 0)),
            };

            _scopeScreenVM.CurrentSignalFrames = new[]
            {
                new SignalFrame(1, 2, new []{-1d, 0d, 2d, 3d}),
                new SignalFrame(channel2TimeIncrement, 0,  channel2values),
            };
        }

        /// <summary>
        /// Gets the scope screen viewmodel.
        /// </summary>
        public IScopeScreenViewModel ScopeScreenVM
        {
            get { return _scopeScreenVM; }
        }
    }
}

