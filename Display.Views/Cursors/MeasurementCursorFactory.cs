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
using System.Collections.Generic;
using PB = Praeclarum.Bind;
using ScopeLib.Utilities;
using ScopeLib.Display.ViewModels;
using ScopeLib.Display.Graphics;

namespace ScopeLib.Display.Views
{
    /// <summary>
    /// Creates cursors used on the scope screen for measuring values.
    /// </summary>
    internal static class MeasurementCursorFactory
    {
        private const char _deltaSymbol = '\u2206';

        /// <summary>
        /// Creates a cursor used for level measurements.
        /// </summary>
        internal static BoundCursor CreateLevelMeasurementCursor(
            MeasurementCursorConfiguration cursorConfiguration,
            ChannelConfiguration cursorChannelConfiguration,
            bool isReferenceCursor, Func<double> deltaReferenceLevelProvider,
            Func<double> referenceLevel)
        {
            return CreateMeasurementCursor(cursorConfiguration, cursorChannelConfiguration,
                isReferenceCursor, deltaReferenceLevelProvider, referenceLevel);
        }

        /// <summary>
        /// Creates a measurement cursor.
        /// </summary>
        internal static BoundCursor CreateMeasurementCursor(
            MeasurementCursorConfiguration cursorConfiguration,
            ChannelConfiguration cursorChannelConfiguration,
            bool isReferenceCursor, Func<double> deltaReferenceLevelProvider,
            Func<double> referenceLevel)
        {
            Func<String> basicLevelTextProvider = () =>
                UnitHelper.BuildValueText(cursorChannelConfiguration.BaseUnitString, cursorConfiguration.Level);

            var cursorColor = CairoHelpers.ToCairoColor(cursorChannelConfiguration.Color);

            ScopeCursorMarkers markers;
            ScopeVerticalAlignment valueAlignment;
            if (isReferenceCursor)
            {
                markers = ScopeCursorMarkers.YLower;
                valueAlignment = ScopeVerticalAlignment.Top;
            }
            else
            {
                markers = ScopeCursorMarkers.YUpper;
                valueAlignment = ScopeVerticalAlignment.Bottom;
            }

            Func<String> levelTextProvider;
            if (deltaReferenceLevelProvider == null)
            {
                levelTextProvider = basicLevelTextProvider;
            }
            else
            {
                levelTextProvider = () =>
                    string.Format("{0} / {1} = {2}", basicLevelTextProvider(), _deltaSymbol,
                        UnitHelper.BuildValueText(cursorChannelConfiguration.BaseUnitString,
                            cursorConfiguration.Level - deltaReferenceLevelProvider()));
            }

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.Y,
                LineWeight = ScopeCursorLineWeight.Medium,
                SelectableLines = ScopeCursorLines.Y,
                Markers = markers,
                Color = cursorColor,
                Captions = new []
                {
                    new ScopePositionCaption(levelTextProvider, ScopeHorizontalAlignment.Right, valueAlignment, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, cursorColor),
                },
            };

            // === Create value converters. ===

            Func<double> valueScaleFactor = () =>
                cursorChannelConfiguration.ValueScaleFactor;

            Func<double> triggerLevelReferencePosition = () =>
                cursorChannelConfiguration.ReferencePointPosition.Y;

            var cursorLevelConverter = new ValueConverter<double, double>(
                val => (val - referenceLevel()) * valueScaleFactor() + triggerLevelReferencePosition(),
                val => ((val - triggerLevelReferencePosition()) / valueScaleFactor()) + referenceLevel());

            // === Create bindings. ===

            // Bind the cursor's position.
            var binding = PB.Binding.Create (() =>
                cursor.Position.Y == cursorLevelConverter.DerivedValue &&
                cursorLevelConverter.OriginalValue == cursorConfiguration.Level);

            // The measurement cursor's Y position depends on some additional values (except the primary value
            // it is bound to). Update it if any of these values changes. ===
            cursorChannelConfiguration.PropertyChanged += (sender, e) =>
            {
                PB.Binding.InvalidateMember(() => cursorLevelConverter.DerivedValue);
            };
            cursorChannelConfiguration.ReferencePointPosition.PropertyChanged += (sender, e) =>
            {
                PB.Binding.InvalidateMember(() => cursorLevelConverter.DerivedValue);
            };

            return new BoundCursor(cursor, binding);
        }
    }
}

