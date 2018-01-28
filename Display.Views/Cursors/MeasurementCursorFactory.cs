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
using System.ComponentModel;
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
            var influencingObjects = new INotifyPropertyChanged[]
            {
                cursorChannelConfiguration,
                cursorChannelConfiguration.ReferencePointPosition
            };

            return CreateMeasurementCursor(cursorConfiguration,
                isReferenceCursor, deltaReferenceLevelProvider,
                () => cursorChannelConfiguration.ValueScaleFactor,
                () => cursorChannelConfiguration.ReferencePointPosition.Y,
                referenceLevel,
                cursorChannelConfiguration.BaseUnitString,
                cursorChannelConfiguration.Color,
                influencingObjects);
        }

        /// <summary>
        /// Creates a measurement cursor.
        /// </summary>
        internal static BoundCursor CreateMeasurementCursor(
            MeasurementCursorConfiguration cursorConfiguration,
            bool isReferenceCursor,
            Func<double> deltaReferenceValueProvider,
            Func<double> valueScaleFactor,
            Func<double> triggerReferenceValue,
            Func<double> referenceValue,
            string baseUnitString,
            Color cursorColor,
            IEnumerable<INotifyPropertyChanged> influencingObjects)
        {
            Func<String> basicValueTextProvider = () =>
                UnitHelper.BuildValueText(baseUnitString, cursorConfiguration.Value);

            var cairoColor = CairoHelpers.ToCairoColor(cursorColor);

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

            Func<String> valueTextProvider;
            if (deltaReferenceValueProvider == null)
            {
                valueTextProvider = basicValueTextProvider;
            }
            else
            {
                valueTextProvider = () =>
                    string.Format("{0} / {1} = {2}", basicValueTextProvider(), _deltaSymbol,
                        UnitHelper.BuildValueText(baseUnitString,
                            cursorConfiguration.Value - deltaReferenceValueProvider()));
            }

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.Y,
                LineWeight = ScopeCursorLineWeight.Medium,
                SelectableLines = ScopeCursorLines.Y,
                Markers = markers,
                Color = cairoColor,
                Captions = new []
                {
                    new ScopePositionCaption(valueTextProvider, ScopeHorizontalAlignment.Right, valueAlignment, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, cairoColor),
                },
            };

            // === Create value converters. ===

            var cursorLevelConverter = new ValueConverter<double, double>(
                val => (val - referenceValue()) * valueScaleFactor() + triggerReferenceValue(),
                val => ((val - triggerReferenceValue()) / valueScaleFactor()) + referenceValue());

            // === Create bindings. ===

            // Bind the cursor's position.
            var binding = PB.Binding.Create (() =>
                cursor.Position.Y == cursorLevelConverter.DerivedValue &&
                cursorLevelConverter.OriginalValue == cursorConfiguration.Value);

            // The measurement cursor's position depends on some additional values (except the primary value
            // it is bound to). Update it if any of these values changes. ===
            influencingObjects.ForEach(influencingObject =>
            {
                influencingObject.PropertyChanged += (sender, e) =>
                {
                    PB.Binding.InvalidateMember(() => cursorLevelConverter.DerivedValue);
                };
            });

            return new BoundCursor(cursor, binding);
        }
    }
}

