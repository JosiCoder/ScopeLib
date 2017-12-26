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
using NUnit.Framework;
using SpecsFor;
using Should;
using SpecsFor.ShouldExtensions;
using Moq;

namespace ScopeLib.Signal.Specs
{
    public abstract class WaveformFileStreamExtensionsSpecs
        : SpecsFor<object>
    {
        protected Mock<Stream> _streamMock;

        protected override void Given()
        {
            base.Given();

            _streamMock = GetMockFor<Stream> ();
        }
    }


    public class When_writing_a_string_value_to_a_waveform_file_stream
        : WaveformFileStreamExtensionsSpecs
    {
        protected byte[] _bytes;

        protected override void When()
        {
            string value = "ABC";
            _streamMock.Object.WriteString(value);
        }

        [Test]
        public void then_the_SUT_should_write_the_correct_ASCII_representation()
        {
            _streamMock.Verify(str => str.Write(new []{(byte)65, (byte)66, (byte)67}, 0, 3));
        }
    }


    public class When_writing_a_16bit_value_to_a_waveform_file_stream
        : WaveformFileStreamExtensionsSpecs
    {
        protected byte[] _bytes;

        protected override void When()
        {
            short value = 256*2 + 1;
            _streamMock.Object.WriteInt16(value);
        }

        [Test]
        public void then_the_SUT_should_write_the_correct_little_endian_byte_representation()
        {
            _streamMock.Verify(str => str.Write(new []{(byte)1, (byte)2}, 0, 2));
        }
    }


    public class When_writing_a_32bit_value_to_a_waveform_file_stream
        : WaveformFileStreamExtensionsSpecs
    {
        protected byte[] _bytes;

        protected override void When()
        {
            int value = 256*256*256*4 + 256*256*3 + 256*2 + 1;
            _streamMock.Object.WriteInt32(value);
        }

        [Test]
        public void then_the_SUT_should_write_the_correct_little_endian_byte_representation()
        {
            _streamMock.Verify(str => str.Write(new []{(byte)1, (byte)2, (byte)3, (byte)4}, 0, 4));
        }
    }


    public class When_reading_a_string_value_from_a_waveform_file_stream
        : WaveformFileStreamExtensionsSpecs
    {
        protected string _value;

        protected override void When()
        {
            var bytes = new []{ (byte)65, (byte)66, (byte)67};

            _streamMock
                .Setup(str => str.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<byte[], int, int>((buffer, offset, count) =>
                {
                    bytes.CopyTo(buffer, 0);
                })
                .Returns(bytes.Length);

            _value = _streamMock.Object.ReadAsString(bytes.Length);
        }

        [Test]
        public void then_the_SUT_should_return_the_according_value()
        {
            _value.ShouldEqual("ABC");
        }
    }


    public class When_reading_a_16bit_value_from_a_waveform_file_stream
        : WaveformFileStreamExtensionsSpecs
    {
        protected short _value;

        protected override void When()
        {
            var bytes = new []{ (byte)1, (byte)2};

            _streamMock
                .Setup(str => str.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<byte[], int, int>((buffer, offset, count) =>
                {
                    bytes.CopyTo(buffer, 0);
                })
                .Returns(bytes.Length);

            _value = _streamMock.Object.ReadAsInt16();
        }

        [Test]
        public void then_the_SUT_should_return_the_according_value()
        {
            _value.ShouldEqual((short)(256*2 + 1));
        }
    }


    public class When_reading_a_32bit_value_from_a_waveform_file_stream
        : WaveformFileStreamExtensionsSpecs
    {
        protected int _value;

        protected override void When()
        {
            var bytes = new []{ (byte)1, (byte)2, (byte)3, (byte)4};

            _streamMock
                .Setup(str => str.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback<byte[], int, int>((buffer, offset, count) =>
                {
                    bytes.CopyTo(buffer, 0);
                })
                .Returns(bytes.Length);

            _value = _streamMock.Object.ReadAsInt32();
        }

        [Test]
        public void then_the_SUT_should_return_the_according_value()
        {
            _value.ShouldEqual(256*256*256*4 + 256*256*3 + 256*2 + 1);
        }
    }
}

