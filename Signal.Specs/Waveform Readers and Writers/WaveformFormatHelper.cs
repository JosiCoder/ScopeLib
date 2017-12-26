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

using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;

namespace ScopeLib.Signal.Specs
{
    public static class WaveformFormatHelper
    {
        public static void CheckFormat(WaveformFormat format,
            short expectedChannelsCount, int expectedSamplesPerSecond, short expectedBitsPerSample) 
        {
            var sampleSize = (short)((expectedBitsPerSample + 7) / 8);
            var frameSize = (short)(expectedChannelsCount * sampleSize);
            var bytesPerSecond = frameSize * expectedSamplesPerSecond;

            format.ChannelsCount.ShouldEqual(expectedChannelsCount);
            format.SamplesPerSecond.ShouldEqual(expectedSamplesPerSecond);
            format.BitsPerSample.ShouldEqual(expectedBitsPerSample);
            format.SampleSize.ShouldEqual(sampleSize);
            format.FrameSize.ShouldEqual(frameSize);
            format.BytesPerSecond.ShouldEqual(bytesPerSecond);
        }
    }
}

