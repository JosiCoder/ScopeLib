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
        private const char _channelCaptionBaseSymbol = '\u278A';// one of '\u2460', '\u2776', '\u278A';

        private readonly Color _baseColor = new Color(0.5, 0.8, 1.0);
        private readonly Color[] _channelColors = new Color[]
        {
            new Color(1, 1, 0),
            new Color(0, 1, 0),
        };

        private readonly IScopeScreenViewModel _masterScopeScreenVM = new ScopeScreenViewModel();
        private readonly IScopeScreenViewModel _slaveScopeScreenVM = new ScopeScreenViewModel();

        //TODO: comments (see also below)
        public DemoViewModel ()
        {
            var sampleSequences = CreateSampleSequences();
            ConfigureMainScopeScreenVM(_masterScopeScreenVM, sampleSequences);
            //ConfigureZoomScopeScreenVM(_slaveScopeScreenVM);
            ConfigureFFTScopeScreenVM(_slaveScopeScreenVM, sampleSequences.First());
        }

        private IEnumerable<SampleSequence> CreateSampleSequences()
        {
            var channel1SampleFrequency = 64;
            var channel1XInterval = 1/(double)channel1SampleFrequency;
            var channel1values =
                FunctionValueGenerator.GenerateSineValuesForFrequency(1, channel1SampleFrequency, 4,
                    (x, y) => y).Take(256); //TODO: must be a power of 2 for FFT

            var channel2SampleFrequency = 1;
            var channel2XInterval = 1/(double)channel2SampleFrequency;
            var channel2values =  new []{ -1d, 0d, 2d, 3d };

            // LogDeferredAccess shows us some details about how the values are accessed (see there).
//            yield return new SampleSequence(channel1XInterval, channel1values);
            yield return new SampleSequence(channel1XInterval, LogDeferredAccess(channel1values));
            yield return new SampleSequence(channel2XInterval, channel2values);
//            yield return new SampleSequence(channel2XInterval, LogDeferredAccess(channel2values));
        }

        private void ConfigureMainScopeScreenVM (IScopeScreenViewModel scopeScreenVM,
            IEnumerable<SampleSequence> sampleSequences)
        {
            // === Channels configuration ===

            var timeScaleFactor = 1;
            var channelVMs = new[]
            {
                new ChannelViewModel("V", new Position(0.0, 1.0), timeScaleFactor, 1,
                    BuildChannelCaptionFromIndex(0), _channelColors[0]),
                new ChannelViewModel("V", new Position(0, -2), timeScaleFactor, 1,
                    BuildChannelCaptionFromIndex(1), _channelColors[1]),
            };

            var index = 0;
            channelVMs[index].MeasurementCursor1VM.Visible = true;
            channelVMs[index].MeasurementCursor2VM.Visible = true;
            channelVMs[index].MeasurementCursor1VM.Value = 2.0;
            channelVMs[index].MeasurementCursor2VM.Value = 3.0;
            index++;
            channelVMs[index].MeasurementCursor1VM.Visible = true;
            channelVMs[index].MeasurementCursor2VM.Visible = true;
            channelVMs[index].MeasurementCursor1VM.Value = -0.5;
            channelVMs[index].MeasurementCursor2VM.Value = 0.5;
            scopeScreenVM.ChannelVMs = channelVMs;

            // === Graphbase configuration ===

            var graphbaseVM = new GraphbaseViewModel ("s", 1, _baseColor);

            var trigger = new LevelTrigger(LevelTriggerMode.RisingEdge, 0.5);
            var triggerChannelIndex = 0;

            graphbaseVM.TriggerVM =
                new LevelTriggerViewModel(trigger, channelVMs[triggerChannelIndex]);
            graphbaseVM.MeasurementCursor1VM.Visible = true;
            graphbaseVM.MeasurementCursor2VM.Visible = true;
            graphbaseVM.MeasurementCursor1VM.Value = 2.0;
            graphbaseVM.MeasurementCursor2VM.Value = 3.0;
            scopeScreenVM.GraphbaseVM = graphbaseVM;

            // === Sample Sequences ===

            var sampleSequenceProviders =
                sampleSequences.Select(ss => new Func<SampleSequence>(() => ss));

            var sampler = new Sampler(sampleSequenceProviders, trigger, triggerChannelIndex);
            scopeScreenVM.SampleSequenceProviders = sampler.SampleSequenceProviders;
        }

        private void ConfigureFFTScopeScreenVM (IScopeScreenViewModel scopeScreenVM,
            SampleSequence sampleSequence)
        {
            // === Channels configuration ===

            var frequencyScaleFactor = 1;
            var channelVMs = new[]
            {
                new ChannelViewModel("dB?", new Position(0, -2), frequencyScaleFactor, 0.5,
                    BuildChannelCaptionFromIndex(0), _channelColors[0]),//TODO Unit
            };

            var index = 0;
            channelVMs[index].MeasurementCursor1VM.Visible = true;
            channelVMs[index].MeasurementCursor2VM.Visible = true;
            channelVMs[index].MeasurementCursor1VM.Value = -0.5;
            channelVMs[index].MeasurementCursor2VM.Value = 0.5;
            scopeScreenVM.ChannelVMs = channelVMs;

            // === Graphbase configuration ===

            var graphbaseVM = new GraphbaseViewModel ("Hz", 0.2, _baseColor);

            graphbaseVM.MeasurementCursor1VM.Visible = true;
            graphbaseVM.MeasurementCursor2VM.Visible = true;
            graphbaseVM.MeasurementCursor1VM.Value = 2.0;
            graphbaseVM.MeasurementCursor2VM.Value = 3.0;
            scopeScreenVM.GraphbaseVM = graphbaseVM;

            // === Sample Sequences ===

            var channel1FourierSamples =
                new Fourier().TransformForward(sampleSequence);

            var sampleSequenceProviders = new Func<SampleSequence>[]
            {
                () => channel1FourierSamples,
            };

            scopeScreenVM.SampleSequenceProviders = sampleSequenceProviders;
        }

        /// <summary>
        /// Creates a channel caption for the specified channel index.
        /// </summary>
        private string BuildChannelCaptionFromIndex(int channelIndex)
        {
            return ((char)(_channelCaptionBaseSymbol + channelIndex)).ToString();
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
        /// Gets the master scope screen viewmodel.
        /// </summary>
        public IScopeScreenViewModel MasterScopeScreenVM
        {
            get { return _masterScopeScreenVM; }
        }

        /// <summary>
        /// Gets the slave scope screen viewmodel.
        /// </summary>
        public IScopeScreenViewModel SlaveScopeScreenVM
        {
            get { return _slaveScopeScreenVM; }
        }
    }
}

