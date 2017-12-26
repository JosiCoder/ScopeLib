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
    public abstract class LittleEndianByteArrayExtensionsSpecs
        : SpecsFor<object>
    {
    }


    public class When_coverting_a_16bit_value_to_its_little_endian_byte_representation
        : LittleEndianByteArrayExtensionsSpecs
    {
        protected byte[] _bytes;

        protected override void When()
        {
            short value = 256*2 + 1;
            _bytes = value.Int16ToBytes();
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_little_endian_byte_representation()
        {
            _bytes.ShouldEqual(new []{(byte)1, (byte)2});
        }
    }


    public class When_coverting_a_32bit_value_to_its_little_endian_byte_representation
        : LittleEndianByteArrayExtensionsSpecs
    {
        protected byte[] _bytes;

        protected override void When()
        {
            int value = 256*256*256*4 + 256*256*3 + 256*2 + 1;
            _bytes = value.Int32ToBytes();
        }

        [Test]
        public void then_the_SUT_should_return_the_correct_little_endian_byte_representation()
        {
            _bytes.ShouldEqual(new []{(byte)1, (byte)2, (byte)3, (byte)4});
        }
    }


    public class When_coverting_the_little_endian_byte_representation_of_a_16bit_value
        : LittleEndianByteArrayExtensionsSpecs
    {
        protected short _value;

        protected override void When()
        {
            var bytes = new []{ (byte)1, (byte)2};
            _value = bytes.BytesToInt16();
        }

        [Test]
        public void then_the_SUT_should_return_the_according_value()
        {
            _value.ShouldEqual((short)(256*2 + 1));
        }
    }


    public class When_coverting_the_little_endian_byte_representation_of_32bit_value
        : LittleEndianByteArrayExtensionsSpecs
    {
        protected int _value;

        protected override void When()
        {
            var bytes = new []{ (byte)1, (byte)2, (byte)3, (byte)4};
            _value = bytes.BytesToInt32();
        }

        [Test]
        public void then_the_SUT_should_return_the_according_value()
        {
            _value.ShouldEqual(256*256*256*4 + 256*256*3 + 256*2 + 1);
        }
    }
}

