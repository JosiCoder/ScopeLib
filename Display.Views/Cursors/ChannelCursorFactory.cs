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
    /// Creates cursors used on the scope screen for channel-related values.
    /// </summary>
    internal static class ChannelCursorFactory
    {
        private const char _channelCaptionBaseSymbol = '\u278A';// one of '\u2460', '\u2776', '\u278A';

        /// <summary>
        /// Creates a reference line cursor for a single channel.
        /// </summary>
        internal static BoundCursor CreateChannelReferenceCursor(ChannelConfiguration channelConfiguration,
            int channelNumber)
        {
            var channelCaption = ((char)(_channelCaptionBaseSymbol+channelNumber)).ToString();
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
