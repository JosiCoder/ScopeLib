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

using System.IO;
using System.Linq;
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;

namespace ScopeLib.Signal.Specs
{
    public abstract class WaveformFileWaveformWriterSpecs
        : SpecsFor<object>
    {
        protected const string _filePath = @"TempTestWaveFile.wav";
        protected const short _channelsCount = 2;
        protected const int _samplesPerSecond = 48000;
        protected const short _bitsPerSample = 16;
        protected const short _fileSize = 52;

        protected WaveForm16BitFrame[] _frames;
    }


    public class When_writing_a_waveform_to_a_waveform_file
        : WaveformFileWaveformWriterSpecs
    {
        protected FileInfo _file = new FileInfo(_filePath);

        protected override void Given ()
        {
            base.Given();

            if (_file.Exists)
            {
                _file.Delete();
            }
        }

        protected override void When()
        {
            _frames = new WaveForm16BitFrame[]
            {
                new WaveForm16BitFrame(new []{(short)1,(short)2}),
                new WaveForm16BitFrame(new []{(short)3,(short)4}),
            };

            var format = new WaveformFormat(_channelsCount, _samplesPerSecond, _bitsPerSample);
            var waveForm = new MemoryWaveform(format, _frames);

            using (var waveFormWriter = new WaveformFileWaveformWriter(_filePath))
            {
                waveFormWriter.Write(waveForm);
            }
        }

        [Test]
        public void then_the_SUT_should_write_a_file_of_the_expected_size()
        {
            _file.Exists.ShouldBeTrue();
            _file.Length.ShouldEqual(_fileSize);
        }

        [Test]
        public void then_the_SUT_should_write_the_file_format_specified()
        {
            using (var waveForm = new WaveformFileWaveformReader(_filePath).Read())
            {
                WaveformFormatHelper.CheckFormat(waveForm.Format, _channelsCount, _samplesPerSecond, _bitsPerSample);
            }
        }

        [Test]
        public void then_the_SUT_should_write_the_corrent_frames_count()
        {
            using (var waveForm = new WaveformFileWaveformReader(_filePath).Read())
            {
                waveForm.FrameCount.ShouldEqual(_frames.Length);
            }
        }

        [Test]
        public void then_the_SUT_should_write_the_frames_specified()
        {
            using (var waveForm = new WaveformFileWaveformReader(_filePath).Read())
            {
                var framesFromFile = waveForm.GetFrames().ToArray();
                var expectedFrames = _frames.Cast<IWaveformFrame>().ToArray();
                framesFromFile.ShouldLookLike(expectedFrames);
            }
        }
    }
}

