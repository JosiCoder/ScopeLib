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

            var timeScaleFactor = 1;
            var channelVMs = new[]
            {
                new ChannelViewModel("V", new Position(0.0, 1.0), timeScaleFactor, 2, new Color(1, 1, 0)),
                new ChannelViewModel("V", new Position(0, -2), timeScaleFactor, 1, new Color(0, 1, 0)),
            };
            channelVMs[0].MeasurementCursor1VM.Visible = true;
            channelVMs[0].MeasurementCursor2VM.Visible = true;
            channelVMs[1].MeasurementCursor1VM.Visible = true;
            channelVMs[1].MeasurementCursor2VM.Visible = true;
            channelVMs[0].MeasurementCursor1VM.Value = 2.0;
            channelVMs[0].MeasurementCursor2VM.Value = 3.0;
            channelVMs[1].MeasurementCursor1VM.Value = -0.5;
            channelVMs[1].MeasurementCursor2VM.Value = 0.5;
            _scopeScreenVM.ChannelVMs = channelVMs;

            // === Timebase configuration ===

            var timebaseVM = new TimebaseViewModel ("s", 1, new Color(0.5, 0.8, 1.0));

            var trigger = new LevelTrigger(LevelTriggerMode.RisingEdge, 0.5);
            var triggerChannelIndex = 0;

            timebaseVM.TriggerVM =
                new LevelTriggerViewModel(trigger, channelVMs[triggerChannelIndex]);
            timebaseVM.MeasurementCursor1VM.Visible = true;
            timebaseVM.MeasurementCursor2VM.Visible = true;
            timebaseVM.MeasurementCursor1VM.Value = 2.0;
            timebaseVM.MeasurementCursor2VM.Value = 3.0;
            _scopeScreenVM.TimebaseVM = timebaseVM;

            // === Sample Sequences ===

            var channel1SampleFrequency = 10;
            var channel1XInterval = 1/(double)channel1SampleFrequency;
            var channel1values =
                FunctionValueGenerator.GenerateSineValuesForFrequency(1, channel1SampleFrequency, 4,
                    (x, y) => y);

            var channel2SampleFrequency = 1;
            var channel2XInterval = 1/(double)channel2SampleFrequency;
            var channel2values =  new []{ -1d, 0d, 2d, 3d };

            // UseDeferred shows us some details about how the values are accessed (see there).
            var sampleSequenceProviders = new Func<SampleSequence>[]
            {
//                () => new SampleSequence(channel1SampleInterval, channel1values),
                () => new SampleSequence(channel1XInterval, LogDeferredAccess(channel1values)),
                () => new SampleSequence(channel2XInterval, channel2values),
//                () => new SampleSequence(channel2SampleInterval, UseDeferred(channel2values)),
            };

            var sampler = new Sampler(sampleSequenceProviders, trigger, triggerChannelIndex);

            _scopeScreenVM.SampleSequenceProviders = sampler.SampleSequenceProviders;
        }

        /// <summary>
        /// Log the (deferred) access to the values in the specified enumerable. This shows
        /// us which values are accessed as well as when and how often they are accessed.
        /// </summary>
        private IEnumerable<T> LogDeferredAccess<T>(IEnumerable<T> values)
        {
            return values.ForEachDoDeferred(element => Console.WriteLine(element));
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

