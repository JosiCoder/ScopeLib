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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Gtk;
using Cairo;
using UI = Gtk.Builder.ObjectAttribute;
//using PB = Praeclarum.Bind;
using System.Collections.Specialized;
using ScopeLib.Display.ViewModels;
using ScopeLib.Display.Graphics;

namespace ScopeLib.Display.Views
{
    /// <summary>
    /// Provides the Gtk# view of a scope screen.
    /// </summary>
    public class ScopeScreenView : Gtk.Bin
    {
        private readonly IScopeScreenViewModel _viewModel;

        private const bool _captureContinuously = true;
        private const double _xMinimumGraticuleUnits = 10.0;
        private const double _yMinimumGraticuleUnits = 8.0;
        private readonly ScopeGraphics _scopeGraphics;

        private uint _currentMouseButtons;
        private DateTime _captureDateTime;
        private int _frameCounter;
        private int _framesPerSecond;
        private int _lastDrawSecond;

        [UI] Gtk.EventBox scopeEventBox;
        [UI] Gtk.DrawingArea scopeDrawingArea;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static ScopeScreenView Create(IScopeScreenViewModel viewModel)
        {
            var builder = new Builder (null, "ScopeScreenView.glade", null);
            return new ScopeScreenView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        private ScopeScreenView(IScopeScreenViewModel viewModel, Builder builder, IntPtr handle)
            : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            // === Create sub-views. ===

            // === Register event handlers. ===

            // === Create value converters. ===

            // === Create bindings. ===

            //  === Do any additional stuff. ===

            _scopeGraphics = new ScopeGraphics (ScopeStretchMode.Stretch,
                _xMinimumGraticuleUnits, _yMinimumGraticuleUnits);

            InitializeGraphics();
        }

        private void InitializeGraphics()
        {
            // Raise motion events even if no button is pressed.
            Events |= Gdk.EventMask.PointerMotionMask;

            scopeDrawingArea.Drawn += ScopeDrawingArea_DrawnEventHandler;

            scopeEventBox.Events =
                Gdk.EventMask.PointerMotionMask |
                Gdk.EventMask.ButtonPressMask |
                Gdk.EventMask.ButtonReleaseMask;

            scopeEventBox.MotionNotifyEvent += ScopeEventBox_MotionNotifyEventHandler;
            scopeEventBox.ButtonPressEvent += ScopeEventBox_ButtonPressEventHandler;
            scopeEventBox.ButtonReleaseEvent += ScopeEventBox_ButtonReleaseEventHandler;

            ShowScopeDemo ();

            Capture();
        }

        private void ScopeDrawingArea_DrawnEventHandler (object o, DrawnArgs args)
        {
            _scopeGraphics.Draw(scopeDrawingArea.Window);

            if (_captureContinuously)
            {
                Capture();
                TriggerRedraw();
            }
        }

        private void ScopeEventBox_MotionNotifyEventHandler (object o, MotionNotifyEventArgs args)
        {
            if (_currentMouseButtons != 0)
            {
                _scopeGraphics.SetSelectedCursorLinesToPosition (new PointD (args.Event.X, args.Event.Y));
            }
            else
            {
                _scopeGraphics.FindAndHighlightCursorLines(new PointD(args.Event.X, args.Event.Y));
            }

            TriggerRedraw ();
        }

        private void ScopeEventBox_ButtonPressEventHandler (object o, ButtonPressEventArgs args)
        {
            _currentMouseButtons = args.Event.Button;
            _scopeGraphics.FindAndSelectCursorLines (new PointD(args.Event.X, args.Event.Y));
            TriggerRedraw ();
        }

        private void ScopeEventBox_ButtonReleaseEventHandler (object o, ButtonReleaseEventArgs args)
        {
            _currentMouseButtons = 0;
            _scopeGraphics.DeselectScopeCursorLines ();
            _scopeGraphics.FindAndHighlightCursorLines (new PointD(args.Event.X, args.Event.Y));
            TriggerRedraw ();
        }

        private void TriggerRedraw()
        {
            var currentDrawSecond = _captureDateTime.Second;
            if (currentDrawSecond == _lastDrawSecond)
            {
                _frameCounter++;
            }
            else
            {
                _framesPerSecond = _frameCounter;
                _frameCounter = 0;
                _lastDrawSecond = currentDrawSecond;
            }
            scopeDrawingArea.Window.InvalidateRect(new Gdk.Rectangle(0, 0, scopeDrawingArea.Window.Width, scopeDrawingArea.Window.Height), false);
        }

        private void Capture()
        {
            _captureDateTime = DateTime.Now;

            // Here we could capture more data.
        }

        // === From here to the end for demo purposes only ===

//        private IEnumerable<PointD> GenerateSine()
//        {
//            return FunctionValueGenerator.GenerateSineValuesForAngles(0.0, 2 * Math.PI, 2 * Math.PI / 40,
//                (x, y) => new PointD (x, y));
//        }
//
        private void ShowScopeDemo()
        {
            Color textColor = new Color (1, 1, 0);
            Color cursor1Color = new Color (1, 0.5, 0.5);
            Color cursor2Color = new Color (0.5, 1, 0.5);
            Color cursor3Color = new Color (0.5, 0.5, 1);
            Color triggerCursorColor = new Color (0.5, 0.5, 0.0);

            _scopeGraphics.Graphs = new []
            {
                new ScopeGraph
                {
                    ReferencePoint = new PointD(1, 2),
                    ReferencePointPosition = new PointD(1.0, 1.0),
                    XScaleFactor = 0.5,
                    YScaleFactor = 0.3,
                    Vertices = new []
                    {
                        new PointD (-1, -1), 
                        new PointD (0, -0),
                        new PointD (1, 2),
                        new PointD (2, 3),
                    },
                    LineType = ScopeLineType.LineAndDots,
                },
                new ScopeGraph
                {
                    ReferencePoint = new PointD(0, 0),
                    ReferencePointPosition = new PointD(-Math.PI, 0),
                    XScaleFactor = 1,
                    YScaleFactor = 2,
//                    Vertices = GenerateSine(),
                    LineType = ScopeLineType.LineAndDots,
                },
            };

            _scopeGraphics.Cursors = new []
            {
                new ScopeCursor
                {
                    Position = new PointD(1, 1),
                    Lines = ScopeCursorLines.Both,
                    Markers = ScopeCursorMarkers.Full,
                    Color = cursor1Color,
                    Captions = new []
                    {
                        new ScopePositionCaption(() => "LB", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.Position, false, textColor),
                        new ScopePositionCaption(() => "RB", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.Position, false, textColor),
                        new ScopePositionCaption(() => "LT", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Top, ScopeAlignmentReference.Position, false, textColor),
                    },
                    XTicks = new []
                    {
                        new ScopeCursorValueTick(-1.5), 
                        new ScopeCursorValueTick(1.5),
                        new ScopeCursorValueTick(2.0)
                    },
                    YTicks = new []
                    {
                        new ScopeCursorValueTick(-2.5),
                        new ScopeCursorValueTick(1.5),
                        new ScopeCursorValueTick(2.5),
                    },
                },
                new ScopeCursor
                {
                    Position = new PointD (-2.5, -2.7),
                    Lines = ScopeCursorLines.X,
                    Markers = ScopeCursorMarkers.XRight,
                    Color = cursor2Color,
                    Captions = new []
                    {
                        new ScopePositionCaption(() => "yLB", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, textColor),
                        new ScopePositionCaption(() => "yLT", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Top, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, textColor),
                        new ScopePositionCaption(() => "yRT", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, false, textColor),
                    },
                    YTicks = new []
                    {
                        new ScopeCursorValueTick(-0.5),
                        new ScopeCursorValueTick(0.5),
                        new ScopeCursorValueTick(3.0)
                    },
                },
                new ScopeCursor
                {
                    Position = new PointD (-3.5, -3.7),
                    Lines = ScopeCursorLines.Y,
                    Markers = ScopeCursorMarkers.YLower,
                    Color = cursor3Color,
                    Captions = new []
                    {
                        new ScopePositionCaption(() => "xRT", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, textColor),
                        new ScopePositionCaption(() => "xRB", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, false, textColor),
                        new ScopePositionCaption(() => "xLB", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, false, textColor),
                    },
                    XTicks = new []
                    {
                        new ScopeCursorValueTick(-0.3),
                        new ScopeCursorValueTick(0.3),
                        new ScopeCursorValueTick(2.8)
                    },
                },
                new ScopeCursor
                {
                    Position = new PointD (1.5, 1.5),
                    Lines = ScopeCursorLines.Both,
                    SelectableLines = ScopeCursorLines.None,
                    Markers = ScopeCursorMarkers.Full,
                    Color = triggerCursorColor,
                    Captions = new []
                    {
                        new ScopePositionCaption(() => "T", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, triggerCursorColor),
                        new ScopePositionCaption(() => "T", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, triggerCursorColor),
                        new ScopePositionCaption(() => "T", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Top, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, triggerCursorColor),
                        new ScopePositionCaption(() => "T", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, triggerCursorColor),
                    },
                },
            };

            _scopeGraphics.Cursors.First().Captions.First().TextProvider = () =>
                string.Format("x={0:F2}/y={1:F2}", _scopeGraphics.Cursors.First().Position.X,  _scopeGraphics.Cursors.First().Position.Y);

            _scopeGraphics.Readouts = new []
            {
                new ScopeReadout
                {
                    Line = 0,
                    Column = 0,
                    TextProvider = () => "hor: 1 ms/div",
                },
                new ScopeReadout
                {
                    Line = 1,
                    Column = 0,
                    TextProvider = () => "Ch1: 1 mV/div",
                },
                new ScopeReadout
                {
                    Line = 1,
                    Column = 1,
                    TextProvider = () => "Ch2: 1 mV/div",
                    Color = new Color (.5,1,1),
                },
                new ScopeReadout
                {
                    Line = 0,
                    Column = 4,
                    TextProvider = () => _captureDateTime.ToString(),
                },
                new ScopeReadout
                {
                    Line = 1,
                    Column = 4,
                    TextProvider = () =>
                        _captureContinuously ? string.Format ("{0} fps", _framesPerSecond) : "hold",
                },
            };
        }
    }
}

