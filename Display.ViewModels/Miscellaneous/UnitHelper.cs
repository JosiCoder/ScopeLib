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

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides scaling information for a specific scale factor.
    /// </summary>
    public class ScaleFactorInfo
    {
        public string ValueSuffix;
        public string UnitPrefix;
    }

    /// <summary>
    /// Provides utilities for handling physical units.
    /// </summary>
    public static class UnitHelper
    {
        private struct PrefixFactor
        {
            public readonly int Exponent;
            public readonly string Prefix;

            public PrefixFactor (int exponent, string prefix)
            {
                Exponent = exponent;
                Prefix = prefix;
            }
        }

        private static PrefixFactor[] _prefixFactors = new PrefixFactor[]
        {
            new PrefixFactor(9, "G"),
            new PrefixFactor(6, "M"),
            new PrefixFactor(3, "k"),
            new PrefixFactor(0, ""),
            new PrefixFactor(-3, "m"),
            new PrefixFactor(-6, "μ"),
            new PrefixFactor(-9, "n"),
        };

        /// <summary>
        /// Returns scaling information for the specified scale factor.
        /// </summary>
        /// <param name="scaleFactorExponent">
        /// The exponent of the scale factor to get scaling information for.
        /// </param>
        /// <returns>The scaling information for the specified scale factor exponent.</returns>
        public static ScaleFactorInfo GetScaleFactorInfo(int scaleFactorExponent)
        {
            var unitPrefix = "";
            foreach(var factor in _prefixFactors)
            {
                if (scaleFactorExponent >= factor.Exponent)
                {
                    scaleFactorExponent -= factor.Exponent;
                    unitPrefix = factor.Prefix;
                    break;
                }
            }

            return new ScaleFactorInfo
            {
                ValueSuffix = scaleFactorExponent != 0
                    ? string.Format("e{0}", scaleFactorExponent)
                    : "",
                UnitPrefix = unitPrefix,
            };
        }

        /// <summary>
        /// Creates a value text according to the current value.
        /// </summary>
        public static string BuildValueText(string baseUnitString, double value)
        {
            Func<double, int, int> getEffectiveExponent = (val, snap) =>
            {
                var exp = Math.Log10(Math.Abs(val));
                var f = Math.Floor(exp/snap);
                var i = f >= 0 ? (int)(f+0.5) : (int)(f-0.5);
                return snap * i;
            };

            try
            {
                int decimalPlaces = 0;
                int scaleExponent = 0;
                if (Math.Abs(value) > double.Epsilon)
                {
                    scaleExponent = getEffectiveExponent(value, 3);
                    var leastSignificantDigitExponent = getEffectiveExponent(value, 1);
                    decimalPlaces = Math.Max(0, 2 - (leastSignificantDigitExponent - scaleExponent));
                }

                var formatString = string.Format("{{0:#,0.{0}}}{{1}} {{2}}", "".PadRight(decimalPlaces, '0'));

                var scaleFactorInfo = GetScaleFactorInfo (scaleExponent);

                return string.Format(formatString, value / Math.Pow(10, scaleExponent),
                    scaleFactorInfo.ValueSuffix, scaleFactorInfo.UnitPrefix + baseUnitString);
            }
            catch (Exception)
            {
                return ("-");
            }
        }
    }
}

