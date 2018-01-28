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
    /// Creates cursors used on the scope screen for trigger-related values.
    /// </summary>
    internal static class TriggerCursorFactory
    {
        private const char _triggerSymbol = 'T';
        private const char _triggerTypeRisingSymbol = '\u2191';
        private const char _triggerTypeFallingSymbol = '\u2193';

        /// <summary>
        /// Creates a trigger criteria cursor for a level-based trigger.
        /// </summary>
        internal static BoundCursor CreateTriggerCriteriaCursor(
            LevelTriggerConfiguration triggerConfiguration,
            ChannelConfiguration triggerChannelConfiguration,
            Func<double> referenceLevel)
        {
            var triggerModeSymbol =
                triggerConfiguration.Mode == LevelTriggerMode.RisingEdge ? _triggerTypeRisingSymbol
                : triggerConfiguration.Mode == LevelTriggerMode.FallingEdge ? _triggerTypeFallingSymbol
                : '?';

            var influencingObjects = new INotifyPropertyChanged[]
            {
                triggerChannelConfiguration,
                triggerChannelConfiguration.ReferencePointPosition
            };

            return  CreateTriggerCriteriaCursor(triggerConfiguration,
                triggerModeSymbol,
                () => triggerChannelConfiguration.ValueScaleFactor,
                () => triggerChannelConfiguration.ReferencePointPosition.Y,
                referenceLevel,
                triggerConfiguration.ChannelConfiguration.BaseUnitString,
                triggerChannelConfiguration.Color,
                influencingObjects);
        }

        /// <summary>
        /// Creates a trigger criteria cursor for a level-based trigger.
        /// </summary>
        internal static BoundCursor CreateTriggerCriteriaCursor(
            LevelTriggerConfiguration triggerConfiguration,
            char triggerModeSymbol,
            Func<double> valueScaleFactor,
            Func<double> triggerReferenceValue,
            Func<double> referenceLevel,
            string baseUnitString,
            Color levelColor,
            IEnumerable<INotifyPropertyChanged> influencingObjects)
        {
            var triggerCaption = string.Format("{0}{1}", _triggerSymbol, triggerModeSymbol);
            Func<String> levelTextProvider = () =>
                UnitHelper.BuildValueText(baseUnitString, triggerConfiguration.Level);

            var cairoColor = CairoHelpers.ToCairoColor(levelColor);

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.Y,
                LineWeight = ScopeCursorLineWeight.Low,
                SelectableLines = ScopeCursorLines.Y,
                Markers = ScopeCursorMarkers.YFull,
                Color = cairoColor,
                Captions = new []
                {
                    new ScopePositionCaption(() => triggerCaption, ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, cairoColor),
                    new ScopePositionCaption(() => triggerCaption, ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, cairoColor),
                    new ScopePositionCaption(levelTextProvider, ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, cairoColor),
                },
            };

            // === Create value converters. ===

            var triggerLevelConverter = new ValueConverter<double, double>(
                val => (val - referenceLevel()) * valueScaleFactor() + triggerReferenceValue(),
                val => ((val - triggerReferenceValue()) / valueScaleFactor()) + referenceLevel());

            // === Create bindings. ===

            // Bind the cursor's position.
            var binding = PB.Binding.Create (() =>
                cursor.Position.Y == triggerLevelConverter.DerivedValue &&
                triggerLevelConverter.OriginalValue == triggerConfiguration.Level);

            // The trigger cursor's position depends on some additional values (except the primary value
            // it is bound to). Update it if any of these values changes. ===
            influencingObjects.ForEach(influencingObject =>
            {
                influencingObject.PropertyChanged += (sender, e) =>
                {
                    PB.Binding.InvalidateMember(() => triggerLevelConverter.DerivedValue);
                };
            });

            return new BoundCursor(cursor, binding);
        }

        /// <summary>
        /// Creates a trigger point cursor.
        /// </summary>
        internal static BoundCursor CreateTriggerPointCursor(TriggerConfigurationBase triggerConfiguration)
        {
            var triggerCaption = _triggerSymbol.ToString();
            Func<String> positionTextProvider = () =>
                string.Format("{0:F2}", triggerConfiguration.HorizontalPosition);

            var markerColor = new Cairo.Color (0.5, 0.8, 1.0);

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.X,
                LineWeight = ScopeCursorLineWeight.Low,
                SelectableLines = ScopeCursorLines.X,
                Markers = ScopeCursorMarkers.XFull,
                Color = markerColor,
                Captions = new []
                {
                    new ScopePositionCaption(() => triggerCaption, ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Top, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, markerColor),
                    new ScopePositionCaption(() => triggerCaption, ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, markerColor),
                    new ScopePositionCaption(positionTextProvider, ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, markerColor),
                },
            };

            // === Create bindings. ===

            // Bind the cursor's position.
            var binding = PB.Binding.Create (() => cursor.Position.X == triggerConfiguration.HorizontalPosition);

            return new BoundCursor(cursor, binding);
        }
    }
}

