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
using System.Linq;
using System.Collections.Generic;
using ScopeLib.Utilities;
using ScopeLib.Sampling;
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
            // === Channels configuration ===

            var channelVMs = new[]
            {
//                new ChannelViewModel("V", new Position(1.0, 1.0), 0.5, 0.333, new Color(1, 1, 0)),
                new ChannelViewModel("V", new Position(0.0, 1.0), 1, 1, new Color(1, 1, 0)),
//                new ChannelViewModel("V", new Position(-Math.PI, -2), 1, 2, new Color(0, 1, 0)),
                new ChannelViewModel("V", new Position(0, -2), 1, 2, new Color(0, 1, 0)),
            };
            channelVMs[0].MeasurementCursorA.Visible = true;
            channelVMs[0].MeasurementCursorB.Visible = true;
            channelVMs[1].MeasurementCursorA.Visible = true;
            channelVMs[1].MeasurementCursorB.Visible = true;
            channelVMs[0].MeasurementCursorA.Value = 2.0;
            channelVMs[0].MeasurementCursorB.Value = 3.0;
            channelVMs[1].MeasurementCursorA.Value = -0.5;
            channelVMs[1].MeasurementCursorB.Value = 0.5;
            _scopeScreenVM.ChannelVMs = channelVMs;

            // === Timebase configuration ===

            var timebaseVM = new TimebaseViewModel ("s", 1, new Color(0.5, 0.8, 1.0));

            var trigger = new LevelTrigger(LevelTriggerMode.RisingEdge, 0.5);
            var triggerChannelIndex = 0;

            timebaseVM.TriggerVM =
                new LevelTriggerViewModel(trigger, channelVMs[triggerChannelIndex]);
            timebaseVM.MeasurementCursorA.Visible = true;
            timebaseVM.MeasurementCursorB.Visible = true;
            timebaseVM.MeasurementCursorA.Value = 2.0;
            timebaseVM.MeasurementCursorB.Value = 3.0;
            _scopeScreenVM.TimebaseVM = timebaseVM;

            // === Sample Sequences ===

            var channel2TimeIncrement = 2 * Math.PI / 40;
            var channel2values =
                FunctionValueGenerator.GenerateSineValuesForAngles(0.0, 2 * Math.PI, channel2TimeIncrement,
                    (x, y) => y);

            var sampleSequenceProviders = new Func<SampleSequence>[]
            {
                () => new SampleSequence(1, new []{ -1d, 0d, 2d, 3d }),
                () => new SampleSequence(channel2TimeIncrement, channel2values),
            };

            var sampler = new Sampler(sampleSequenceProviders, trigger, triggerChannelIndex);

            _scopeScreenVM.SampleSequenceProviders = sampler.SampleSequenceProviders;
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

