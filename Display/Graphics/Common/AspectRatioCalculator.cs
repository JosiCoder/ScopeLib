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

namespace ScopeLib.Display
{
    /// <summary>
    /// Calculates the aspect ratios of the specified user-specific and device-related
    /// rectangular ranges as well as the ratio between these aspect ratios.
    /// </summary>
    public class AspectRatioCalculator
    {
        private readonly double _deviceWidth;
        private readonly double _deviceHeight;
        private readonly double _userWidth;
        private readonly double _userHeight;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="deviceWidth">The width of the device range, in device units.</param>
        /// <param name="deviceHeight">The height of the device range, in device units.</param>
        /// <param name="userWidth">The width of the user-specific range, in user-specific units.</param>
        /// <param name="userHeight">The height of the user-specific range, in user-specific units.</param>
        public AspectRatioCalculator (double deviceWidth, double deviceHeight, double userWidth, double userHeight)
        {
            _deviceWidth = deviceWidth;
            _deviceHeight = deviceHeight;
            _userWidth = userWidth;
            _userHeight = userHeight;
        }

        /// <summary>
        /// Gets the device-related aspect ratio, in device units.
        /// </summary>
        public double DeviceAspectRatio
        {
            get { return _deviceWidth / _deviceHeight; }
        }

        /// <summary>
        /// Gets the user-specific aspect ratio, in user-specific units.
        /// </summary>
        public double UserAspectRatio
        {
            get { return _userWidth / _userHeight; }
        }

        /// <summary>
        /// Gets the ratio between the device-related and user-specific aspect ratios.
        /// </summary>
        public double UserToDeviceAspectRatioFactor
        {
            get { return DeviceAspectRatio / UserAspectRatio; }
        }
    }
}

