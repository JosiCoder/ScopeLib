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
    /// Creates cursors used on the scope screen.
    /// </summary>
    public static class CursorFactory
    {
        /// <summary>
        /// Creates a trigger criteria cursor for a level-based trigger.
        /// </summary>
        public static BoundCursor CreateTriggerCriteriaCursor(
            LevelTriggerConfiguration triggerConfiguration,
            ChannelConfiguration triggerChannelConfiguration,
            Func<double> referenceLevel)
        {
            const string triggerCaption = "T";
            Func<String> levelTextProvider = () =>
                UnitHelper.BuildValueText(triggerConfiguration.BaseUnitString,
                    triggerConfiguration.Level);

            var levelColor = CairoHelpers.ToCairoColor(triggerChannelConfiguration.Color);

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.Y,
                LineWeight = ScopeCursorLineWeight.Low,
                SelectableLines = ScopeCursorLines.Y,
                Markers = ScopeCursorMarkers.YFull,
                Color = levelColor,
                Captions = new []
                {
                    new ScopePositionCaption(() => triggerCaption, ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, levelColor),
                    new ScopePositionCaption(() => triggerCaption, ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, levelColor),
                    new ScopePositionCaption(levelTextProvider, ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, levelColor),
                },
            };

            // === Create value converters. ===

            Func<double> valueScaleFactor = () =>
                triggerChannelConfiguration.ValueScaleFactor;

            Func<double> triggerLevelReferencePosition = () =>
                triggerChannelConfiguration.ReferencePointPosition.Y;

            var triggerLevelConverter = new ValueConverter<double, double>(
                val => (val - referenceLevel()) * valueScaleFactor() + triggerLevelReferencePosition(),
                val => ((val - triggerLevelReferencePosition()) / valueScaleFactor()) + referenceLevel());

            // === Create bindings. ===

            // Bind the cursor's position.
            var binding = PB.Binding.Create (() =>
                cursor.Position.Y == triggerLevelConverter.DerivedValue &&
                triggerLevelConverter.OriginalValue == triggerConfiguration.Level);

            // The trigger cursor's Y position depends on some additional values (except the primary value
            // it is bound to). Update it if any of these values changes. ===
            triggerChannelConfiguration.PropertyChanged += (sender, e) =>
            {
                PB.Binding.InvalidateMember(() => triggerLevelConverter.DerivedValue);
            };
            triggerChannelConfiguration.ReferencePointPosition.PropertyChanged += (sender, e) =>
            {
                PB.Binding.InvalidateMember(() => triggerLevelConverter.DerivedValue);
            };

            return new BoundCursor(cursor, binding);
        }

        /// <summary>
        /// Creates a trigger point cursor.
        /// </summary>
        public static BoundCursor CreateTriggerPointCursor(TriggerConfigurationBase triggerConfiguration)
        {
            const string triggerCaption = "T";
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

        /// <summary>
        /// Creates a reference line cursor for a single channel.
        /// </summary>
        public static BoundCursor CreateChannelCursor(ChannelConfiguration channelConfiguration,
            int channelNumber)
        {
            var channelCaption = channelNumber.ToString();
            var channelColor = CairoHelpers.ToCairoColor(channelConfiguration.Color);

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.Y,
                LineWeight = ScopeCursorLineWeight.Low,
                SelectableLines = ScopeCursorLines.Y,
                Markers = ScopeCursorMarkers.YFull,
                Color = channelColor,
                Captions = new []
                {
                    new ScopePositionCaption(() => channelCaption, ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, channelColor),
                    new ScopePositionCaption(() => channelCaption, ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, channelColor),
                },
            };

            // === Create bindings. ===

            // Bind the cursor's position.
            var binding = PB.Binding.Create (() => cursor.Position.Y == channelConfiguration.ReferencePointPosition.Y);

            return new BoundCursor(cursor, binding);
        }
    }
}

