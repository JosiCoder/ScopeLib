//------------------------------------------------------------------------------
// Copyright (C) 2016 Josi Coder

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

using System.Linq;
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;

namespace ScopeLib.Signal.Specs
{
    public abstract class WaveformFileWaveformReaderSpecs
        : SpecsFor<WaveformFileWaveformReader>
    {
        protected const string _filePath = @"../../Test Data/TestWaveformFile.wav";
        protected const short _channelsCount = 2;
        protected const int _samplesPerSecond = 48000;
        protected const short _bitsPerSample = 16;
        protected const int _numberOfSamplesInFile = 2;

        protected override void InitializeClassUnderTest()
        {
            SUT = new WaveformFileWaveformReader(_filePath);
        }
    }


    public class When_reading_a_waveform_from_a_waveform_file
        : WaveformFileWaveformReaderSpecs
    {

        protected StreamWaveform _waveForm;

        protected override void When()
        {
            _waveForm = SUT.Read();
        }

        protected override void AfterSpec ()
        {
            base.AfterSpec();

            _waveForm.Dispose();
        }

        [Test]
        public void then_the_SUT_should_return_a_waveform_in_a_format_corresponding_to_the_file()
        {
            WaveformFormatHelper.CheckFormat(_waveForm.Format, _channelsCount, _samplesPerSecond, _bitsPerSample);
        }

        [Test]
        public void then_the_SUT_should_return_the_corrent_frames_count()
        {
            _waveForm.FrameCount.ShouldEqual(_numberOfSamplesInFile);
        }

        [Test]
        public void then_the_SUT_should_return_exactly_as_many_frames_as_specified()
        {
            _waveForm.GetFrames().Count().ShouldEqual(_numberOfSamplesInFile);
        }
    }
}

