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
    /// Renders the contents of a scope to a Cairo context.
    /// </summary>
    public class ScopeRenderer : ContextRendererBase
    {
        private class DummyDisposable : IDisposable
        {
            public void Dispose(){}
        }

        private const int _outerMargin = 5;
        private const int _numberOfReadoutLines = 2;
        private const uint _numberOfReadoutColumns = 5;
        private const int _interAreaSpacing = 5;
        private readonly Distance _defaultReadoutsAlignmentDistance = new Distance (5, 10);
        private readonly Distance _defaultCaptionsAlignmentDistance = new Distance (3, 3);
        private const string _standardFontFamily = "Noto Sans";
        private const double _standardFontSize = 12;

        private const double _graticuleLineWidth = 0.5;
        private const double _graticuleAxesLineWidth = 1.0;
        private const double _graticuleAxesTicksLineWidth = 0.5;
        private const double _cursorLineWidth = 0.5;
        private readonly double[] _cursorLineLowWeightDashes = new []{ 1d, 3d };
        private readonly double[] _cursorLineMediumWeightDashes = new []{ 1d, 1d };
        private const double _cursorHighlightLineWidth = 1.0;
        private const double _cursorTickLineWidth = 0.5;
        private const double _cursorMarkersLineWidth = 0.5;
        private const int _longitudinalCursorMarkerSize = 10;
        private const int _lateralCursorMarkerSize = 3;
        private const double _cursorTickLength = 10.0;
        private const double _ticksBaseLength = 0.1;
        private const double _graphStandardLineWidth = 1.0;
        private const double _dotsGraphLineWidth = 1.5;
        private const double _reducedGraphLineWidth = 0.5;

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="context">The Cairo context to use.</param>
        public ScopeRenderer (Context context)
            : base(context)
        {
        }

        /// <summary>
        /// Draws the main background.
        /// </summary>
        /// <param name="deviceWidth">The device width, in device units.</param>
        /// <param name="deviceHeight">The device height, in device units.</param>
        public void DrawMainBackground(int deviceWidth, int deviceHeight)
        {
            var backGroundColor = new Color (0.2, 0.2, 0.2);

            using (CreateContextState())
            {
                Context.MoveTo (0, 0);
                Context.LineTo (deviceWidth-1, 0);
                Context.LineTo (deviceWidth-1, deviceHeight-1);
                Context.LineTo (0, deviceHeight-1);
                Context.ClosePath ();

                Context.SetSourceColor (backGroundColor);
                Context.Fill ();
            }
        }

        /// <summary>
        /// Draws the readouts (i.e. some additional information regarding the scope settings and signals).
        /// </summary>
        /// <param name="extents">The extents of the device area to draw the readouts to.</param>
        /// <param name="readouts">The readouts to draw.</param>
        public void DrawReadouts(DeviceAreaExtents extents, IEnumerable<ScopeReadout> readouts)
        {
            readouts = readouts ?? new ScopeReadout[0];
            foreach (var readout in readouts)
            {
                DrawReadout(extents, readout);
            }
        }

        /// <summary>
        /// Draws a scope readout.
        /// </summary>
        private void DrawReadout(DeviceAreaExtents extents, ScopeReadout readout)
        {
            var fontHeight = GetFontExtents ().Height;

            var lineHeight = fontHeight;
            var columnWidth = (extents.Width - 2 * _defaultReadoutsAlignmentDistance.Dx) / _numberOfReadoutColumns;

            var readoutPosition = new PointD (
                extents.MinX + readout.Column * columnWidth,
                extents.MinY + readout.Line * lineHeight
            );

            DrawText(readout.CurrentText, readoutPosition, readout.Color,
                ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Top, _defaultReadoutsAlignmentDistance);
        }

        /// <summary>
        /// Draws the scope graphics (i.e. the graticule and all its contents) to a rectangle range.
        /// </summary>
        /// <param name="userRange">The rectangle range to draw the contents to.</param>
        /// <param name="cursors">The cursors to draw.</param>
        public void DrawScopeGraphics(RectangleRange userRange, IEnumerable<ScopeCursor> cursors,
            IEnumerable<ScopeGraph> graphs)
        {
            var backGroundColor = new Color (0, 0.1, 0);
            var graticuleColor = new Color (0.5, 0.5, 0.5);

            DrawGraticuleBackground (userRange, backGroundColor);
            DrawGraticule (userRange, 1.0, 1.0, graticuleColor);

            graphs = graphs ?? new ScopeGraph[0];
            foreach (var graph in graphs)
            {
                DrawGraph(userRange, graph);
            }

            cursors = cursors ?? new ScopeCursor[0];
            foreach (var cursor in cursors)
            {
                DrawCursor (userRange, cursor);
            }
        }

        /// <summary>
        /// Draws a scope graticule.
        /// </summary>
        private void DrawGraticule(RectangleRange rectangleRange,
            double xUserStepWidth, double yUserStepWidth, Color graticuleColor)
        {
            // Set ticks depending on axes style, double.NaN suppresses ticks.
            double minorAxesTicksPerUserStep = 10f; // double.NaN tu suppress ticks
            double majorAxesTicksPerUserStep = 2f; // double.NaN tu suppress ticks

            if (true) // true for ticks @ 0.20 intervals, no major ticks
            {
                minorAxesTicksPerUserStep = 5f;
                majorAxesTicksPerUserStep = double.NaN;
            }

            var minX = rectangleRange.MinX;
            var maxX = rectangleRange.MaxX;
            var minY = rectangleRange.MinY;
            var maxY = rectangleRange.MaxY;
            var userToDeviceMatrix = rectangleRange.Matrix;

            using (CreateContextState())
            {
                using (CreateContextState(userToDeviceMatrix))
                {
                    var xTicksBaseLength = _ticksBaseLength;
                    var yTicksBaseLength = _ticksBaseLength;

                    if (!double.IsNaN (minorAxesTicksPerUserStep))
                    {
                        var xMinorTicksStepWidth = xUserStepWidth / minorAxesTicksPerUserStep;
                        ProcessSteps (minX, maxX, xMinorTicksStepWidth,
                            current => AddLinePath (new PointD (current, -xTicksBaseLength), new PointD (current, xTicksBaseLength)));

                        var yMinorTicksStepWidth = yUserStepWidth / minorAxesTicksPerUserStep;
                        ProcessSteps (minY, maxY, yMinorTicksStepWidth,
                            current => AddLinePath (new PointD (-yTicksBaseLength, current), new PointD (yTicksBaseLength, current)));
                    }

                    if (!double.IsNaN (majorAxesTicksPerUserStep))
                    {
                        var xMajorTicksStepWidth = xUserStepWidth / majorAxesTicksPerUserStep;
                        ProcessSteps (minX, maxX, xMajorTicksStepWidth,
                            current => AddLinePath (new PointD (current, 2 * -xTicksBaseLength), new PointD (current, 2 * xTicksBaseLength)));

                        var yMajorTicksStepWidth = yUserStepWidth / majorAxesTicksPerUserStep;
                        ProcessSteps (minY, maxY, yMajorTicksStepWidth,
                            current => AddLinePath (new PointD (2 * -yTicksBaseLength, current), new PointD (2 * yTicksBaseLength, current)));
                    }
                }

                Context.SetSourceColor (graticuleColor);
                Context.LineWidth = _graticuleAxesTicksLineWidth;
                Context.Stroke ();

                using (CreateContextState(userToDeviceMatrix))
                {
                    ProcessSteps (minX, maxX, xUserStepWidth,
                        current => AddLinePath(new PointD (current, minY), new PointD (current, maxY)));

                    ProcessSteps (minY, maxY, yUserStepWidth, 
                        current => AddLinePath(new PointD (minX, current), new PointD (maxX, current)));
                }

                Context.LineWidth = _graticuleLineWidth;
                Context.Stroke ();

                using (CreateContextState(userToDeviceMatrix))
                {
                    AddLinePath (new PointD (0f, minY), new PointD (0f, maxY));
                    AddLinePath (new PointD (minX, 0f), new PointD (maxX, 0f));
                }

                Context.LineWidth = _graticuleAxesLineWidth;
                Context.Stroke ();
            }
        }

        /// <summary>
        /// Draws a scope graph.
        /// </summary>
        private void DrawGraph(RectangleRange rectangleRange, ScopeGraph graph)
        {
            var lineType = graph.LineType;

            using (CreateContextState())
            {
                ClipToRange(rectangleRange);

                switch (lineType)
                {
                    case ScopeLineType.Dots:
                        DrawGraph (rectangleRange, graph, ScopeLineType.Dots, _dotsGraphLineWidth);
                        break;
                    case ScopeLineType.LineAndDots:
                        DrawGraph (rectangleRange, graph, ScopeLineType.Line, _reducedGraphLineWidth);
                        DrawGraph (rectangleRange, graph, ScopeLineType.Dots, _dotsGraphLineWidth);
                        break;
                    default:
                        DrawGraph (rectangleRange, graph, lineType, _graphStandardLineWidth);
                        break;
                }
            }
        }
                
        /// <summary>
        /// Draws a scope graph.
        /// </summary>
        private void DrawGraph(RectangleRange rectangleRange, ScopeGraph graph,
            ScopeLineType lineType, double lineWidth)
        {
            var userToDeviceMatrix = rectangleRange.Matrix;

            var vertices = graph.Vertices ?? new PointD[0];
            var referencePoint = graph.ReferencePoint;
            var referencePointPosition = graph.ReferencePointPosition;
            var xScaleFactor = graph.XScaleFactor;
            var yScaleFactor = graph.YScaleFactor;
            var color = graph.Color;

            if (vertices.Any())
            {
                using (CreateContextState())
                {
                    Context.SetSourceColor (color);

                    // Create a new transformation matrix considering scale factors
                    // and reference point position.
                    var vertexMatrix = (Matrix)userToDeviceMatrix.Clone ();
                    vertexMatrix.Translate (referencePointPosition.X, referencePointPosition.Y );
                    vertexMatrix.Scale (xScaleFactor, yScaleFactor);
                    vertexMatrix.Translate (-referencePoint.X, -referencePoint.Y);

                    using (CreateContextState(vertexMatrix))
                    {
                        Context.MoveTo (vertices.First());
                        foreach (var vertex in vertices)
                        {
                            if (lineType == ScopeLineType.Dots)
                            {
                                Context.MoveTo (vertex);
                            }
                            Context.LineTo (vertex);
                        }
                    }

                    Context.LineWidth = lineWidth;
                    if (lineType == ScopeLineType.Dots)
                    {
                        Context.LineCap = LineCap.Round;
                    }
                    Context.Stroke ();
                }
            }
        }

        /// <summary>
        /// Draws a scope cursor.
        /// </summary>
        private void DrawCursor(RectangleRange rectangleRange, ScopeCursor cursor)
        {
            DrawCursorLinesAndMarkers (rectangleRange, cursor);

            foreach (var tick in cursor.XTicks ?? new ScopeCursorValueTick[0])
            {
                var tickPosition = new PointD (tick.Value, cursor.Position.Y);
                DrawCursorTickLines (rectangleRange, tick, tickPosition, ScopeCursorLines.X, cursor.Color);
                DrawCaptions (rectangleRange, tick.Captions, tickPosition);
            }

            foreach (var tick in cursor.YTicks ?? new ScopeCursorValueTick[0])
            {
                var tickPosition = new PointD (cursor.Position.X, tick.Value);
                DrawCursorTickLines (rectangleRange, tick, tickPosition, ScopeCursorLines.Y, cursor.Color);
                DrawCaptions (rectangleRange, tick.Captions, tickPosition);
            }

            using (CreateContextState())
            {
                ClipToRange(rectangleRange);

                DrawCaptions (rectangleRange, cursor.Captions, cursor.Position.CairoPoint);
            }
        }

        /// <summary>
        /// Draws the lines and markers of a scope cursor.
        /// </summary>
        private void DrawCursorLinesAndMarkers(RectangleRange rectangleRange, ScopeCursor cursor)
        {
            var userToDeviceMatrix = rectangleRange.Matrix;

            var position = cursor.Position;
            var lines = cursor.Lines;
            var markers = cursor.Markers;
            var color = cursor.Color;

            using (CreateContextState())
            {
                Context.SetSourceColor (color);

                using (CreateContextState(userToDeviceMatrix))
                {
                    if ((lines & ScopeCursorLines.X) != ScopeCursorLines.None)
                    {
                        AddLinePath (new PointD (position.X, rectangleRange.MinY), new PointD (position.X, rectangleRange.MaxY));
                    }
                }

                Context.LineWidth = (cursor.HighlightedLines & ScopeCursorLines.X) != ScopeCursorLines.None
                    ? _cursorHighlightLineWidth : _cursorLineWidth;
                Context.SetDash(
                    cursor.LineWeight == ScopeCursorLineWeight.Low ? _cursorLineLowWeightDashes
                    : cursor.LineWeight == ScopeCursorLineWeight.Medium ? _cursorLineMediumWeightDashes
                    : new double[0],
                    0);
                Context.Stroke ();

                using (CreateContextState(userToDeviceMatrix))
                {
                    if ((lines & ScopeCursorLines.Y) != ScopeCursorLines.None)
                    {
                        AddLinePath (new PointD (rectangleRange.MinX, position.Y), new PointD (rectangleRange.MaxX, position.Y));
                    }
                }

                Context.LineWidth = (cursor.HighlightedLines & ScopeCursorLines.Y) != ScopeCursorLines.None
                    ? _cursorHighlightLineWidth : _cursorLineWidth;
                Context.Stroke ();

                if ((markers & ScopeCursorMarkers.XLeft) != ScopeCursorMarkers.None)
                {
                    AddXCursorMarkerPairPathFromCurrentPosition (
                        userToDeviceMatrix.TransformPoint (position.X, rectangleRange.MaxY),
                        userToDeviceMatrix.TransformPoint (position.X, rectangleRange.MinY),
                        ScopeHorizontalAlignment.Right);
                }

                if ((markers & ScopeCursorMarkers.XRight) != ScopeCursorMarkers.None)
                {
                    AddXCursorMarkerPairPathFromCurrentPosition (
                        userToDeviceMatrix.TransformPoint (position.X, rectangleRange.MaxY),
                        userToDeviceMatrix.TransformPoint (position.X, rectangleRange.MinY),
                        ScopeHorizontalAlignment.Left);
                }

                if ((markers & ScopeCursorMarkers.YUpper) != ScopeCursorMarkers.None)
                {
                    AddYCursorMarkerPairPathFromCurrentPosition (
                        userToDeviceMatrix.TransformPoint (rectangleRange.MinX, position.Y),
                        userToDeviceMatrix.TransformPoint (rectangleRange.MaxX, position.Y),
                        ScopeVerticalAlignment.Bottom);
                }

                if ((markers & ScopeCursorMarkers.YLower) != ScopeCursorMarkers.None)
                {
                    AddYCursorMarkerPairPathFromCurrentPosition (
                        userToDeviceMatrix.TransformPoint (rectangleRange.MinX, position.Y),
                        userToDeviceMatrix.TransformPoint (rectangleRange.MaxX, position.Y),
                        ScopeVerticalAlignment.Top);
                }

                Context.LineWidth = _cursorMarkersLineWidth;
                Context.FillPreserve ();
                Context.Stroke ();
            }
        }

        /// <summary>
        /// Adds a Cairo path that represents a pair of X cursor markers aligned at the current Cairo position.
        /// </summary>
        private void AddXCursorMarkerPairPathFromCurrentPosition(PointD topMarkerPosition, PointD bottomMarkerPosition,
            ScopeHorizontalAlignment horizontalAlignment)
        {
            Context.MoveTo (bottomMarkerPosition);
            AddXCursorMarkerPathFromCurrentPosition (horizontalAlignment, ScopeVerticalAlignment.Bottom);
            Context.MoveTo (topMarkerPosition);
            AddXCursorMarkerPathFromCurrentPosition (horizontalAlignment, ScopeVerticalAlignment.Top);
        }

        /// <summary>
        /// Adds a Cairo path that represents a pair of Y cursor markers aligned at the current Cairo position.
        /// </summary>
        private void AddYCursorMarkerPairPathFromCurrentPosition(PointD leftMarkerPosition, PointD rightMarkerPosition,
            ScopeVerticalAlignment verticalAlignment)
        {
            Context.MoveTo (leftMarkerPosition);
            AddYCursorMarkerPathFromCurrentPosition (ScopeHorizontalAlignment.Left, verticalAlignment);
            Context.MoveTo (rightMarkerPosition);
            AddYCursorMarkerPathFromCurrentPosition (ScopeHorizontalAlignment.Right, verticalAlignment);
        }

        /// <summary>
        /// Adds a Cairo path that represents an X cursor marker aligned at the current Cairo position.
        /// </summary>
        private void AddXCursorMarkerPathFromCurrentPosition(ScopeHorizontalAlignment horizontalAlignment,
            ScopeVerticalAlignment verticalAlignment)
        {
            var horizontalSign = horizontalAlignment == ScopeHorizontalAlignment.Right
                ? -1 : +1;

            var verticalSign = verticalAlignment == ScopeVerticalAlignment.Bottom
                ? -1 : +1;

            Context.RelLineTo (0, verticalSign * _longitudinalCursorMarkerSize);
            Context.RelLineTo (horizontalSign * _lateralCursorMarkerSize, verticalSign * -_longitudinalCursorMarkerSize/2);
            Context.RelLineTo (0, verticalSign * -_longitudinalCursorMarkerSize/2);
            Context.ClosePath ();
        }

        /// <summary>
        /// Adds a Cairo path that represents a Y cursor marker aligned at the current Cairo position.
        /// </summary>
        private void AddYCursorMarkerPathFromCurrentPosition(ScopeHorizontalAlignment horizontalAlignment,
            ScopeVerticalAlignment verticalAlignment)
        {
            var horizontalSign = horizontalAlignment == ScopeHorizontalAlignment.Right
                ? -1 : +1;

            var verticalSign = verticalAlignment == ScopeVerticalAlignment.Bottom
                ? -1 : +1;

            Context.RelLineTo (horizontalSign * _longitudinalCursorMarkerSize, 0);
            Context.RelLineTo (horizontalSign * -_longitudinalCursorMarkerSize/2, verticalSign * _lateralCursorMarkerSize);
            Context.RelLineTo (horizontalSign * -_longitudinalCursorMarkerSize/2, 0);
            Context.ClosePath ();
        }

        /// <summary>
        /// Draws the lines of a cursor tick.
        /// </summary>
        private void DrawCursorTickLines(RectangleRange rectangleRange, ScopeCursorValueTick tick,
            PointD tickPosition, ScopeCursorLines tickLines, Color color)
        {
            var userToDeviceMatrix = rectangleRange.Matrix;
            var cursorTickHalfUserLengths = userToDeviceMatrix.TransformDistanceInverse (_cursorTickLength/2, _cursorTickLength/2);

            using (CreateContextState())
            {
                Context.SetSourceColor (color);

                using (CreateContextState(userToDeviceMatrix))
                {
                    if ((tickLines & ScopeCursorLines.X) != ScopeCursorLines.None)
                    {
                        AddLinePath (new PointD (tickPosition.X, tickPosition.Y - cursorTickHalfUserLengths.Dy), new PointD (tickPosition.X, tickPosition.Y + cursorTickHalfUserLengths.Dy));
                    }
                }

                using (CreateContextState(userToDeviceMatrix))
                {
                    if ((tickLines & ScopeCursorLines.Y) != ScopeCursorLines.None)
                    {
                        AddLinePath (new PointD (tickPosition.X - cursorTickHalfUserLengths.Dx, tickPosition.Y), new PointD (tickPosition.X + cursorTickHalfUserLengths.Dx, tickPosition.Y));
                    }
                }

                Context.LineWidth = _cursorTickLineWidth;
                Context.Stroke ();
            }
        }

        /// <summary>
        /// Draws the specified captions according to the specified user-specific position.
        /// </summary>
        private void DrawCaptions(RectangleRange rectangleRange, IEnumerable<ScopePositionCaption> captions,
            PointD position)
        {
            captions = captions ?? new ScopePositionCaption[0];
            foreach (var caption in captions)
            {
                var alignmentDistance = _defaultCaptionsAlignmentDistance;

                switch (caption.AlignmentReference)
                {
                    case ScopeAlignmentReference.YPositionAndHorizontalRangeEdge:
                        alignmentDistance = caption.YieldToMarker
                            ? new Distance(alignmentDistance.Dx, alignmentDistance.Dy + _lateralCursorMarkerSize)
                            : alignmentDistance;
                        DrawTextAtVerticalEdge (rectangleRange, caption.CurrentText, position.Y, caption.Color,
                            caption.HorizontalAlignment, caption.VerticalAlignment, alignmentDistance);
                        break;

                    case ScopeAlignmentReference.XPositionAndVerticalRangeEdge:
                        alignmentDistance = caption.YieldToMarker
                            ? new Distance(alignmentDistance.Dx + _lateralCursorMarkerSize, alignmentDistance.Dy)
                            : alignmentDistance;
                        DrawTextAtHorizontalEdge (rectangleRange, caption.CurrentText, position.X, caption.Color,
                            caption.HorizontalAlignment, caption.VerticalAlignment, alignmentDistance);
                        break;

                    default:
                        DrawText (rectangleRange, caption.CurrentText, position, caption.Color,
                            caption.HorizontalAlignment, caption.VerticalAlignment, alignmentDistance);
                        break;
                }
            }
        }

        /// <summary>
        /// Draws a text aligned with a certain distance to the specified user-specific position
        /// at the vertical edge.
        /// </summary>
        private void DrawTextAtVerticalEdge(RectangleRange rectangleRange,
            string text, double yPosition, Color textColor,
            ScopeHorizontalAlignment horizontalAlignment, ScopeVerticalAlignment verticalAlignment,
            Distance alignmentDistance)
        {
            var xPosition = horizontalAlignment == ScopeHorizontalAlignment.Right
                ? rectangleRange.MaxX : rectangleRange.MinX;

            DrawText (rectangleRange, text, new PointD(xPosition, yPosition), textColor,
                horizontalAlignment, verticalAlignment, alignmentDistance);
        }

        /// <summary>
        /// Draws a text aligned with a certain distance to the specified user-specific position
        /// at the horizontal edge.
        /// </summary>
        private void DrawTextAtHorizontalEdge(RectangleRange rectangleRange,
            string text, double xPosition, Color textColor,
            ScopeHorizontalAlignment horizontalAlignment, ScopeVerticalAlignment verticalAlignment,
            Distance alignmentDistance)
        {
            var yPosition = verticalAlignment == ScopeVerticalAlignment.Bottom
                ? rectangleRange.MinY : rectangleRange.MaxY;

            DrawText (rectangleRange, text, new PointD(xPosition, yPosition), textColor,
                horizontalAlignment, verticalAlignment, alignmentDistance);
        }

        /// <summary>
        /// Draws a text aligned with a certain distance to the specified user-specific position.
        /// </summary>
        private void DrawText(RectangleRange rectangleRange,
            string text, PointD position, Color textColor,
            ScopeHorizontalAlignment horizontalAlignment, ScopeVerticalAlignment verticalAlignment,
            Distance alignmentDistance)
        {
            var devicePosition = rectangleRange.Matrix.TransformPoint (position);
            DrawText (text, devicePosition, textColor, horizontalAlignment, verticalAlignment,
                alignmentDistance);
        }

        /// <summary>
        /// Draws a text aligned with a certain distance to the specified device position.
        /// </summary>
        private void DrawText(string text, PointD position, Color textColor,
            ScopeHorizontalAlignment horizontalAlignment, ScopeVerticalAlignment verticalAlignment,
            Distance alignmentDistance)
        {
            using (CreateContextState())
            {
                SetFontToStandardSettings();
                var extents = Context.TextExtents(text);

                var deviceDistance = new Distance (
                    horizontalAlignment == ScopeHorizontalAlignment.Right
                    ? -alignmentDistance.Dx - extents.Width : alignmentDistance.Dx,
                    verticalAlignment == ScopeVerticalAlignment.Bottom
                    ? -alignmentDistance.Dy : alignmentDistance.Dy + extents.Height
                );

                Context.MoveTo(position);
                Context.RelMoveTo (deviceDistance);

                Context.SetSourceColor (textColor);
                Context.ShowText(text);
            }
        }

        /// <summary>
        /// Gets the extents of the font used.
        /// </summary>
        /// <returns>The extents of the current font.</returns>
        private FontExtents GetFontExtents()
        {
            using (CreateContextState ())
            {
                SetFontToStandardSettings ();
                return Context.FontExtents;
            }
        }

        /// <summary>
        /// Sets the current font using the standard settings.
        /// </summary>
        private void SetFontToStandardSettings()
        {
            SetFont(_standardFontFamily, _standardFontSize, FontSlant.Normal, FontWeight.Normal);
        }

        /// <summary>
        /// Sets the current font.
        /// </summary>
        private void SetFont(string family, double scale, FontSlant slant, FontWeight weight)
        {
            Context.SelectFontFace(family, slant, weight);
            Context.SetFontSize(scale);
        }

        /// <summary>
        /// Adds a Cairo path that represents a single line.
        /// </summary>
        private void AddLinePath(PointD startPoint, PointD endPoint)
        {
            Context.MoveTo (startPoint);
            Context.LineTo (endPoint);
        }

        /// <summary>
        /// Iterates from a minimum to a maximum value with a certain step width and executes
        /// an action for each step, passing it the current step value. The steps are aligned
        /// so that partial steps appear at both sides.
        /// </summary>
        private void ProcessSteps(double startValue, double endValue, double stepWidth, Action<double> stepAction)
        {
            var start = Math.Ceiling(startValue / stepWidth) * stepWidth;

            for (var current = start; current <= endValue; current += stepWidth)
            {
                stepAction(current);
            }
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        private void DrawGraticuleBackground(RectangleRange rectangleRange, Color backGroundColor)
        {
            var userToDeviceMatrix = rectangleRange.Matrix;

            using (CreateContextState())
            {
                using (CreateContextState(userToDeviceMatrix))
                {
                    Context.MoveTo (rectangleRange.MinX, rectangleRange.MinY);
                    Context.LineTo (rectangleRange.MaxX, rectangleRange.MinY);
                    Context.LineTo (rectangleRange.MaxX, rectangleRange.MaxY);
                    Context.LineTo (rectangleRange.MinX, rectangleRange.MaxY);
                    Context.ClosePath ();
                }

                Context.SetSourceColor (backGroundColor);
                Context.Fill ();
            }
        }

        /// <summary>
        /// Clips any rendering to the specified range.
        /// </summary>
        private void ClipToRange(RectangleRange rectangleRange)
        {
            var userToDeviceMatrix = rectangleRange.Matrix;

            using (CreateContextState(userToDeviceMatrix))
            {
                Context.MoveTo (rectangleRange.MinX, rectangleRange.MinY);
                Context.LineTo (rectangleRange.MaxX, rectangleRange.MinY);
                Context.LineTo (rectangleRange.MaxX, rectangleRange.MaxY);
                Context.LineTo (rectangleRange.MinX, rectangleRange.MaxY);
                Context.ClosePath ();
            }

            Context.Clip ();
        }

        /// <summary>
        /// Gets the width of the area to the left of the scope graphics area.
        /// </summary>
        public int LeftAreaWidth
        {
            get { return _outerMargin; }
        }

        /// <summary>
        /// Gets the width of the area to the right of the scope graphics area.
        /// </summary>
        public int RightAreaWidth
        {
            get { return _outerMargin; }
        }

        /// <summary>
        /// Gets the height of the area above the scope graphics area.
        /// </summary>
        public int TopAreaHeight
        {
            get { return _outerMargin; }
        }

        /// <summary>
        /// Gets the height of the area below the scope graphics area.
        /// </summary>
        public int BottomAreaHeight
        {
            get
            {
                return _interAreaSpacing +
                    (int) (_numberOfReadoutLines * GetFontExtents().Height + _defaultReadoutsAlignmentDistance.Dy);
            }
        }

        /// <summary>
        /// Returns the device area extents related to the rectangle range the scope contents
        /// are rendered to.
        /// </summary>
        /// <param name="deviceWidth">The device width, in device units.</param>
        /// <param name="deviceHeight">The device height, in device units.</param>
        /// <returns>The device area extents.</returns>
        public DeviceAreaExtents GetScopeGraphicsRangeDeviceAreaExtents(int deviceWidth, int deviceHeight)
        {
            var rangeWidth = deviceWidth - LeftAreaWidth - RightAreaWidth;
            var rangeHeight = deviceHeight - TopAreaHeight - BottomAreaHeight;

            return new DeviceAreaExtents
            {
                Width = rangeWidth,
                Height = rangeHeight,
                MinX = LeftAreaWidth,
                MaxX = rangeWidth-1 + LeftAreaWidth,
                MinY = TopAreaHeight,
                MaxY = rangeHeight-1 + TopAreaHeight,
            };
        }

        /// <summary>
        /// Returns the device area extents readout values are rendered to.
        /// </summary>
        /// <param name="deviceWidth">The device width, in device units.</param>
        /// <param name="deviceHeight">The device height, in device units.</param>
        /// <returns>The device area extents.</returns>
        public DeviceAreaExtents GetReadoutsDeviceAreaExtents(int deviceWidth, int deviceHeight)
        {
            return new DeviceAreaExtents
            {
                Width = deviceWidth,
                Height = BottomAreaHeight,
                MinX = 0,
                MaxX = deviceWidth-1,
                MinY = deviceHeight-1 - BottomAreaHeight,
                MaxY = deviceHeight-1,
            };
        }
    }
}

