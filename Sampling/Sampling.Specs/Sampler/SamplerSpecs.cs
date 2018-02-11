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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using ExpectedObjects;
using Moq;
using ScopeLib.Utilities;

namespace ScopeLib.Sampling.Specs
{
    public abstract class SamplerSpecs
        : SpecsFor<Sampler>
    {
        protected const int _triggerChannelIndex = 0;
        protected Mock<ITrigger> _triggerMock;
        protected IEnumerable<Func<SampleSequence>> _sampleSequenceProviders;
        protected readonly double[] _channel1values = new []{0d, 1d, 2d, 3d, 4d, 5d, 6d};
        protected readonly double[] _channel2values = new []{10d, 11d, 12d, 13d};
        protected readonly List<double> _accessedValues = new List<double>();

        protected override void InitializeClassUnderTest ()
        {
            _triggerMock = GetMockFor<ITrigger>();

            _sampleSequenceProviders = new Func<SampleSequence>[]
            {
                () => new SampleSequence(10, UseDeferred(_channel1values)),
                () => new SampleSequence(20, UseDeferred(_channel2values)),
            };

            SUT = new Sampler(_sampleSequenceProviders, _triggerMock.Object, _triggerChannelIndex);
        }

        private IEnumerable<double> UseDeferred(IEnumerable<double> values)
        {
            return values.ForEachDoDeferred(value =>
            {
                _accessedValues.Add(value);    
            });
        }
    }


    public class Given_the_trigger_does_not_trigger_and_no_timerange_is_given
    {
        public abstract class NonTriggeredSamplerSpecs
            : SamplerSpecs
        {
            protected override void Given()
            {
                base.Given();

                _triggerMock
                    .Setup (tr => tr.Check(It.IsAny<double>()))
                    .Returns (false);

                _triggerMock
                    .SetupGet(tr => tr.State)
                    .Returns(TriggerState.Armed);
            }
        }


        public class when_fetching_the_sample_sequences_for_all_channels
            : NonTriggeredSamplerSpecs
        {
            private SampleSequence[] _sequences;

            protected override void When()
            {
                var providers = SUT.SampleSequenceProviders.ToArray();
                _sequences = providers.Select(provider => provider()).ToArray();
            }

            [Test]
            public void then_the_SUT_should_access_all_sample_values_of_the_trigger_channel_but_none_else ()
            {
                _channel1values.ForEachDo(value => _accessedValues.ShouldContain(value));

                _accessedValues.Count.ShouldEqual(_channel1values.Count());
            }

            [Test]
            public void then_each_sample_sequence_should_have_a_reference_time_of_zero ()
            {
                _sequences.ForEachDo(sequence => sequence.ReferenceTime.ShouldEqual(0));
            }
        }


        public class when_fetching_the_sample_values_of_all_channels_multiple_times
            : NonTriggeredSamplerSpecs
        {
            private SampleSequence[] _sequences;

            protected override void When()
            {
                var providers = SUT.SampleSequenceProviders.ToArray();
                _sequences = providers.Select(provider => provider()).ToArray();

                // Fetching values twice.
                _sequences.ForEachDo(sequence => sequence.Values.ToList());
                _sequences.ForEachDo(sequence => sequence.Values.ToList());
            }

            [Test]
            public void then_the_SUT_should_access_all_sample_values_exactly_once ()
            {
                _channel1values.Concat(_channel2values)
                    .ForEachDo(value => _accessedValues.ShouldContain(value));
                
                _accessedValues.Count.ShouldEqual(_channel1values.Count() + _channel2values.Count());
            }
        }
    }


    public class Given_the_trigger_triggers_after_some_items_and_no_timerange_is_given
    {
        public abstract class TriggeredSamplerSpecs
            : SamplerSpecs
        {
            protected override void Given()
            {
                base.Given();

                // Triggers after 5th value.
                _triggerMock
                    .SetupSequence (tr => tr.Check(It.IsAny<double>()))
                    .Returns (false)
                    .Returns (false)
                    .Returns (false)
                    .Returns (false)
                    .Returns(true);

                _triggerMock
                    .SetupGet(tr => tr.State)
                    .Returns(TriggerState.Triggered);
            }
        }


        public class when_fetching_the_sample_sequences_for_all_channels
            : TriggeredSamplerSpecs
        {
            private const int _indexOfTriggeringItem = 4;

            private SampleSequence[] _sequences;

            protected override void When()
            {
                var providers = SUT.SampleSequenceProviders.ToArray();
                _sequences = providers.Select(provider => provider()).ToArray();
            }

            [Test]
            public void then_the_SUT_should_access_all_pre_trigger_sample_values_but_none_else ()
            {
                _channel1values
                    .Take(_indexOfTriggeringItem + 1)
                    .ForEachDo(value => _accessedValues.ShouldContain(value));

                _accessedValues.Count.ShouldEqual(_indexOfTriggeringItem + 1);
            }

            [Test]
            public void then_each_sample_sequence_should_have_a_reference_time_corresponding_to_the_trigger_reference_time ()
            {
                var triggerReferenceTime = _indexOfTriggeringItem * _sequences[_triggerChannelIndex].TimeIncrement;
                _sequences.ForEachDo(sequence => sequence.ReferenceTime.ShouldEqual(triggerReferenceTime));
            }
        }
    }
}

