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

            var channelConfigurations = new[]
            {
                new ChannelConfiguration("V", new Position(1.0, 1.0), 0.5, 0.333, new Color(1, 1, 0)),
//                new ChannelConfiguration("V", new Position(0.0, 1.0), 1, 1, new Color(1, 1, 0)),
                new ChannelConfiguration("V", new Position(-Math.PI, -2), 1, 2, new Color(0, 1, 0)),
            };
            channelConfigurations[0].MeasurementCursorA.Visible = true;
            channelConfigurations[0].MeasurementCursorB.Visible = true;
            channelConfigurations[1].MeasurementCursorA.Visible = true;
            channelConfigurations[1].MeasurementCursorB.Visible = true;
            channelConfigurations[0].MeasurementCursorA.Value = 2.0;
            channelConfigurations[0].MeasurementCursorB.Value = 3.0;
            channelConfigurations[1].MeasurementCursorA.Value = -0.5;
            channelConfigurations[1].MeasurementCursorB.Value = 0.5;
            _scopeScreenVM.ChannelConfigurations = channelConfigurations;

            // === Timebase configuration ===

            var triggerChannelIndex = 0;

            var timebaseConfiguration = new TimebaseConfiguration ("s", 1, new Color(0.5, 0.8, 1.0));
            timebaseConfiguration.TriggerConfiguration =
                new LevelTriggerConfiguration(
                    new LevelTrigger(LevelTriggerMode.RisingEdge, 0.5),
                    channelConfigurations[triggerChannelIndex]);
            timebaseConfiguration.MeasurementCursorA.Visible = true;
            timebaseConfiguration.MeasurementCursorB.Visible = true;
            timebaseConfiguration.MeasurementCursorA.Value = 2.0;
            timebaseConfiguration.MeasurementCursorB.Value = 3.0;
            _scopeScreenVM.TimebaseConfiguration = timebaseConfiguration;

            // === Sample Sequences ===

            var channel2TimeIncrement = 2 * Math.PI / 40;
            var channel2values =
                FunctionValueGenerator.GenerateSineValuesForAngles(0.0, 2 * Math.PI, channel2TimeIncrement,
                    (x, y) => y);

            var sampleSequenceProviders = new Func<SampleSequence>[]
            {
                () => GetSampleSequence(1, 2, new []{ -1d, 0d, 2d, 3d }),
                () => GetSampleSequence(channel2TimeIncrement, 0, channel2values),
            };
            var xxx = sampleSequenceProviders
                .Select((provider, index) => new
                {
                    SequenceProvider = provider,
                    IsTriggerChannel = index == triggerChannelIndex,
                });

            _scopeScreenVM.SampleSequenceProviders = sampleSequenceProviders;
        }

        private SampleSequence GetSampleSequence(double timeIncrement, double referenceTime, IEnumerable<double> values)
        {
            return new SampleSequence(timeIncrement, referenceTime, values);
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

