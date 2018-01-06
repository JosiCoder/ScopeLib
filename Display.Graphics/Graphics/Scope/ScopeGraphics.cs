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
using System.Linq;
using Cairo;

namespace ScopeLib.Display.Graphics
{
    /// <summary>
    /// Provides the extents of a rectangular device area, in device units.
    /// </summary>
    public class DeviceAreaExtents
    {
        public int Width;
        public int Height;
        public int MinX;
        public int MaxX;
        public int MinY;
        public int MaxY;
    }

    /// <summary>
    /// Manages the graphics display of a scope, including its cursors, captions and ticks.
    /// </summary>
    public class ScopeGraphics
    {
        private readonly ScopeStretchMode _stretchMode;
        private readonly double _xMinimumGraticuleUnits;
        private readonly double _yMinimumGraticuleUnits;
        private readonly double _maxSnapDistance = 10.0;

        private DeviceAreaExtents _currentScopeGraphicsRangeExtents;

        /// <summary>
        /// Specifies the available modes for stretching user-specific units to fit device units.
        /// </summary>
        private enum UserToDeviceStretchMode
        {
            Fill,
            UniformToWidth,
            UniformToHeight,
            UniformToAny,
            UniformToFill,
        }

        Distance _userOriginOffset = new Distance ();// (1.5f, 1.0f);

        private ScopeCursorSelection _cursorSelection;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="stretchMode">The stretch mode of the scope.</param>
        /// <param name="xMinimumGraticuleUnits">
        /// The minimum number of graticule units in the horizontal (X) direction.
        /// </param>
        /// <param name="xMinimumGraticuleUnits">
        /// The minimum number of graticule units in the vertical (Y) direction.
        /// </param>
        public ScopeGraphics (ScopeStretchMode stretchMode,
            double xMinimumGraticuleUnits, double yMinimumGraticuleUnits)
        {
            _stretchMode = stretchMode;
            _xMinimumGraticuleUnits = xMinimumGraticuleUnits;
            _yMinimumGraticuleUnits = yMinimumGraticuleUnits;
        }

        /// <summary>
        /// Gets or sets the scope cursors.
        /// </summary>
        public IEnumerable<ScopeCursor> Cursors
        { get; set; }

        /// <summary>
        /// Gets or sets the scope graphs.
        /// </summary>
        public IEnumerable<ScopeGraph> Graphs
        { get; set; }

        /// <summary>
        /// Gets or sets the scope readouts.
        /// </summary>
        public IEnumerable<ScopeReadout> Readouts
        { get; set; }

        /// <summary>
        /// Attempts to find a scope cursor's lines at the the specified position and
        /// highlights them accordingly.
        /// </summary>
        /// <param name="searchPosition">The position to search at.</param>
        public void FindAndHighlightCursorLines(PointD searchPosition)
        {
            Cursors.ToList().ForEach(s => s.HighlightedLines = ScopeCursorLines.None);
            var cursorSelection = FindScopeCursorLines(searchPosition, true);
            if (cursorSelection != null && cursorSelection.Cursor != null)
            {
                cursorSelection.Cursor.HighlightedLines = cursorSelection.SelectedLines;
            }
        }

        /// <summary>
        /// Attempts to find a scope cursor's lines at the the specified position and
        /// selects them accordingly.
        /// </summary>
        /// <param name="searchPosition">The position to search at.</param>
        public void FindAndSelectCursorLines(PointD searchPosition)
        {
            _cursorSelection = FindScopeCursorLines(searchPosition, true);
        }

        /// <summary>
        /// Deselects any selected scope cursor lines.
        /// </summary>
        public void DeselectScopeCursorLines()
        {
            _cursorSelection = null;
        }

        /// <summary>
        /// Attempts to find a scope cursor's lines at the the specified position.
        /// </summary>
        /// <param name="searchPosition">The position to search at.</param>
        /// <param name="selectableOnly">A value indicating whether to restrict the search to selectable cursor lines.</param>
        /// <returns>A scope cursor selection at the position or <c>null</c>.</returns>
        public ScopeCursorSelection FindScopeCursorLines(PointD searchPosition, bool selectableOnly)
        {
            var extents = _currentScopeGraphicsRangeExtents;
            var userRange = extents != null
                ? CreateScopeGraphicsRange (extents,
                    _xMinimumGraticuleUnits, _yMinimumGraticuleUnits, _userOriginOffset)
                : null;

            if (userRange == null)
            {
                return null;
            }

            return Cursors
                .Select (cursor =>
                {
                    var deviceCursorPosition = userRange.Matrix.TransformPoint (cursor.Position);

                    // Select each axis we are nearby and which a visible line exists for.
                    return new ScopeCursorSelection (cursor,
                        // X axis
                        ((cursor.Lines & ScopeCursorLines.X) != ScopeCursorLines.None &&
                            ((cursor.SelectableLines & ScopeCursorLines.X) != ScopeCursorLines.None || !selectableOnly) &&
                            Math.Abs (searchPosition.X - deviceCursorPosition.X) < _maxSnapDistance
                            ? ScopeCursorLines.X : ScopeCursorLines.None)
                        |
                        // Y axis
                        ((cursor.Lines & ScopeCursorLines.Y) != ScopeCursorLines.None &&
                            ((cursor.SelectableLines & ScopeCursorLines.Y) != ScopeCursorLines.None || !selectableOnly) &&
                            Math.Abs (searchPosition.Y - deviceCursorPosition.Y) < _maxSnapDistance
                            ? ScopeCursorLines.Y : ScopeCursorLines.None));
                })
                .FirstOrDefault (cursorSelection => cursorSelection.SelectedLines != ScopeCursorLines.None);
        }

        /// <summary>
        /// Sets the position of the selected scope cursor lines to the specified position.
        /// </summary>
        /// <param name="window">The Gdk window to work with.</param>
        /// <param name="position">The position to set the cursor lines to.</param>
        public void SetSelectedCursorLinesToPosition(/*Gdk.Window window, */PointD position)
        {
            var extents = _currentScopeGraphicsRangeExtents;
            var userRange = extents != null
                ? CreateScopeGraphicsRange (extents,
                    _xMinimumGraticuleUnits, _yMinimumGraticuleUnits, _userOriginOffset)
                : null;

            if (userRange == null)
            {
                return;
            }

            // Prevent moving the cursor off the screen.
            position = new PointD (
                Math.Max(extents.MinX, Math.Min(extents.MaxX, position.X)),
                Math.Max(extents.MinY, Math.Min(extents.MaxY, position.Y)));

            var userPointerPosition = userRange.Matrix.TransformPointInverse (position);

            if (_cursorSelection != null && _cursorSelection.Cursor != null)
            {
                var newX = (_cursorSelection.SelectedLines & ScopeCursorLines.X) != ScopeCursorLines.None
                    ? userPointerPosition.X : _cursorSelection.Cursor.Position.X;

                var newY = (_cursorSelection.SelectedLines & ScopeCursorLines.Y) != ScopeCursorLines.None
                    ? userPointerPosition.Y : _cursorSelection.Cursor.Position.Y;

                _cursorSelection.Cursor.Position = new PointD(newX, newY);
            }
        }

        /// <summary>
        /// Draws the specified contents to a Gdk window.
        /// </summary>
        /// <param name="window">The Gdk window to work with.</param>
        public void Draw(Gdk.Window window)
        {
            using (var context = Gdk.CairoHelper.Create(window))
            {
                using (var surface = context.GetTarget())
                {
                    var renderer = new ScopeRenderer (context);
                    _currentScopeGraphicsRangeExtents = renderer.GetScopeGraphicsRangeDeviceAreaExtents (window.Width, window.Height);
                    var currentReadoutsDeviceExtents = renderer.GetReadoutsDeviceAreaExtents (window.Width, window.Height);

                    var userRange = _currentScopeGraphicsRangeExtents != null
                        ? CreateScopeGraphicsRange (_currentScopeGraphicsRangeExtents,
                            _xMinimumGraticuleUnits, _yMinimumGraticuleUnits, _userOriginOffset)
                        : null;

                    if (userRange == null)
                    {
                        return;
                    }

                    renderer.DrawMainBackground (window.Width, window.Height);
                    renderer.DrawScopeGraphics (userRange, Cursors, Graphs);
                    renderer.DrawReadouts (currentReadoutsDeviceExtents, Readouts);

                    // There's a bug related to internal reference counting (already fixed on GitHub)
                    // that causes a memory leak. Thus we use our workaround dispose method here.
                    //surface.Dispose ();
                    surface.DisposeHard();
                }
                context.Dispose();
            }
        }

        /// <summary>
        /// Creates a transformation matrix that can be used to transform the user-specific
        /// to device units.
        /// </summary>
        /// <param name="deviceWidth">The device width, in device units.</param>
        /// <param name="deviceHeight">The device height, in device units.</param>
        /// <param name="userWidth">The device width, in user-specific units.</param>
        /// <param name="userHeight">The device heigh, in user-specific units.</param>
        /// <param name="stretchMode">
        /// A value indicating how to stretch user-specific units to fit the device units.
        /// </param>
        /// <returns>The created transformation matrix.</returns>
        private Matrix CreateUserToDeviceTransformationMatrix(int deviceWidth, int deviceHeight,
            double userWidth, double userHeight, UserToDeviceStretchMode stretchMode)
        {
            var extents = _currentScopeGraphicsRangeExtents;
            var deviceCenter = new PointD (deviceWidth / 2 + extents.MinX, deviceHeight / 2 + extents.MinY);

            var aspectRatioCalculator = new AspectRatioCalculator (deviceWidth, deviceHeight, userWidth, userHeight);
            var aspectRatioFactor = aspectRatioCalculator.UserToDeviceAspectRatioFactor;

            switch (stretchMode)
            {
                case UserToDeviceStretchMode.UniformToAny:
                    stretchMode = aspectRatioFactor > 1 ? UserToDeviceStretchMode.UniformToHeight : UserToDeviceStretchMode.UniformToWidth;
                    break;
                case UserToDeviceStretchMode.UniformToFill:
                    stretchMode = aspectRatioFactor < 1 ? UserToDeviceStretchMode.UniformToHeight : UserToDeviceStretchMode.UniformToWidth;
                    break;
            }

            switch (stretchMode)
            {
                case UserToDeviceStretchMode.UniformToHeight:
                    userWidth = userHeight;
                    deviceWidth = (int) (deviceWidth / aspectRatioCalculator.DeviceAspectRatio);
                    break;
                case UserToDeviceStretchMode.UniformToWidth:
                    userHeight = userWidth;
                    deviceHeight = (int) (deviceHeight * aspectRatioCalculator.DeviceAspectRatio);
                    break;
            }

            return new Matrix(deviceWidth/userWidth, 0, 0, -deviceHeight/userHeight, deviceCenter.X, deviceCenter.Y);
        }

        /// <summary>
        /// Creates a rectangle range the scope contents are rendered to, according to the specified values.
        /// </summary>
        /// <returns>The user range.</returns>
        /// <param name="extents">The extents of the device area to render to.</param>
        /// <param name="xUserMinSpan">The minimum horizontal (X) span requeste, in user-specific units.</param>
        /// <param name="yUserMinSpan">The minimum vertical (Y) span requeste, in user-specific units.</param>
        /// <param name="originOffset">The offset of the origin to the range center, in user-specific units.</param>
        /// <returns>The created rectangle range or <c>null</c> if no range could be created.</returns>
        private RectangleRange CreateScopeGraphicsRange(DeviceAreaExtents extents,
            double xUserMinSpan, double yUserMinSpan, Distance userOriginOffset)
        {
            var rangeWidth = extents.Width;
            var rangeHeight = extents.Height;

            // As this could result in division by zero or infinite values,
            // we don't create a range in this case.
            if (rangeWidth <= 0 || rangeHeight <= 0)
            {
                return null;
            }

            double aspectRatioFactor;
            UserToDeviceStretchMode userToDeviceStretchMode;

            if (_stretchMode == ScopeStretchMode.Expand)
            {
                aspectRatioFactor = new AspectRatioCalculator (rangeWidth, rangeHeight, xUserMinSpan, yUserMinSpan)
                    .UserToDeviceAspectRatioFactor;
                userToDeviceStretchMode = UserToDeviceStretchMode.UniformToAny;
            }
            else
            {
                aspectRatioFactor = 1;
                userToDeviceStretchMode = UserToDeviceStretchMode.Fill;
            }

            // Expand the width or height range to utilize any extra space available.
            var xUserSpan = xUserMinSpan;
            var yUserSpan = yUserMinSpan;
            if (aspectRatioFactor > 1)
            {
                xUserSpan *= aspectRatioFactor;
            }
            else
            {
                yUserSpan /= aspectRatioFactor;
            }

            var userToDeviceMatrix = CreateUserToDeviceTransformationMatrix (rangeWidth, rangeHeight, xUserSpan, yUserSpan, userToDeviceStretchMode);
            // Note: "if (matrix != null)" fails. Bug in the equality operator of Matrix?

            // Consider the origin offset (in user-specific units).
            userToDeviceMatrix.Translate (+userOriginOffset.Dx, +userOriginOffset.Dy);

            return new RectangleRange (rangeWidth, rangeHeight, xUserSpan, yUserSpan, userOriginOffset, userToDeviceMatrix);
        }
    }
}

