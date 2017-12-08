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
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Gtk;
using Cairo;
using UI = Gtk.Builder.ObjectAttribute;
using System.Collections.Specialized;
using ScopeLib.Display;

namespace ScopeLib.Demo
{
    public partial class DemoWindowView: Gtk.Window
    {
        private readonly double _xMinimumGraticuleUnits = 10.0;
        private readonly double _yMinimumGraticuleUnits = 8.0;
        private readonly ScopeGraphics _scopeGraphics;

        private uint _currentMouseButtons;
        [UI] Gtk.DrawingArea scopeDrawingArea;

        public static DemoWindowView Create()
        {
            // TODO: load ScopeView.glade
            var builder = new Builder (null, "DemoWindowView.glade", null);
            return new DemoWindowView (builder, builder.GetObject ("window1").Handle);
        }

        protected DemoWindowView(Builder builder, IntPtr handle) : base (handle)
        {
            _scopeGraphics = new ScopeGraphics (ScopeStretchMode.Stretch,
                _xMinimumGraticuleUnits, _yMinimumGraticuleUnits);
            
            builder.Autoconnect(this);

            DeleteEvent += OnDeleteEvent;

            // Raise motion events even if no button is pressed.
            Events |= Gdk.EventMask.PointerMotionMask;

            scopeDrawingArea.Drawn += (o, args) =>
            {
                _scopeGraphics.Draw(scopeDrawingArea.Window);
    //            OnDrawn1 (o, args);
    //            OnDrawn2 (o, args);
            };

            InitializeScopeDemo ();

            MotionNotifyEvent += MainWindow_MotionNotifyEventHandler;
                
            ButtonPressEvent += MainWindow_ButtonPressEventHandler;
            ButtonReleaseEvent += MainWindow_ButtonReleaseEventHandler;
        }

        private void MainWindow_ButtonPressEventHandler (object o, ButtonPressEventArgs args)
        {
            _currentMouseButtons = args.Event.Button;
            FindAndSelectCursorLines (new PointD(args.Event.X, args.Event.Y));
            Redraw ();
        }

        private void MainWindow_ButtonReleaseEventHandler (object o, ButtonReleaseEventArgs args)
        {
            _currentMouseButtons = 0;
            _scopeGraphics.DeselectScopeCursorLines ();
            FindAndHighlightCursorLines (new PointD(args.Event.X, args.Event.Y));
            Redraw ();
        }

        private void MainWindow_MotionNotifyEventHandler (object o, MotionNotifyEventArgs args)
        {
            if (_currentMouseButtons != 0)
            {
                SetSelectedCursorLinesToPosition (new PointD (args.Event.X, args.Event.Y));
            }
            else
            {
                FindAndHighlightCursorLines(new PointD(args.Event.X, args.Event.Y));
            }

            Redraw ();
        }
      
        private void SetSelectedCursorLinesToPosition(PointD pointerPosition)
        {
            _scopeGraphics.SetSelectedCursorLinesToPosition (pointerPosition);
        }

        private void FindAndSelectCursorLines(PointD pointerPosition)
        {
            _scopeGraphics.FindAndSelectCursorLines (pointerPosition);
        }

        private void FindAndHighlightCursorLines(PointD pointerPosition)
        {
            _scopeGraphics.FindAndHighlightCursorLines (pointerPosition);
        }

        private void Redraw()
        {
            scopeDrawingArea.Window.InvalidateRect(new Gdk.Rectangle(0, 0, scopeDrawingArea.Window.Width, scopeDrawingArea.Window.Height), false);
        }

        private IEnumerable<PointD> GenerateSine()
        {
            for (var x = 0.0; x <= 2 * Math.PI; x += 0.1)
            {
                yield return new PointD (x, Math.Sin (x));
            }
        }

        private void InitializeScopeDemo()
        {
            Color textColor = new Color (1, 1, 0);
            Color cursor1Color = new Color (1, 0.5, 0.5);
            Color cursor2Color = new Color (0.5, 1, 0.5);
            Color cursor3Color = new Color (0.5, 0.5, 1);
            Color triggerCursorColor = new Color (0.5, 0.5, 0.0);

            //TODO: demo only
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
                    Vertices = GenerateSine(),
                    LineType = ScopeLineType.LineAndDots,
                },
            };

            //TODO: demo only
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

            //TODO: demo only
            _scopeGraphics.Readouts = new []
            {
                new ScopeReadout
                {
                    Line = 0,
                    Column = 4,
                    TextProvider = () => DateTime.Now.ToString(),
                },
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
            };
        }

        protected void OnDrawn1 (object sender, DrawnArgs a)
        {
            using (var ctx = Gdk.CairoHelper.Create (scopeDrawingArea.Window))
            {
                using (var target = ctx.GetTarget ())
                {
                    PointD p1,p2,p3,p4;
                    p1 = new PointD (10,10);
                    p2 = new PointD (100,10);
                    p3 = new PointD (100,100);
                    p4 = new PointD (10,100);

                    ctx.MoveTo (p1);
                    ctx.LineTo (p2);
                    ctx.LineTo (p3);
                    ctx.LineTo (p4);
                    ctx.LineTo (p1);
                    ctx.ClosePath ();

                    ctx.SetSourceColor(new Color (0,0,0));
                    ctx.FillPreserve ();
                    ctx.SetSourceColor(new Color (1,0,0));
                    ctx.Stroke ();

    //                ctx.Scale(120, 120);
                    ctx.SelectFontFace("Noto Sans", FontSlant.Normal, FontWeight.Normal);
                    ctx.SetFontSize(12);
                    const string testText = "Dies ist ein Texttext.";
                    TextExtents te = ctx.TextExtents(testText);
    //                ctx.MoveTo(0.5 - te.Width  / 2 - te.XBearing,
    //                    0.5 - te.Height / 2 - te.YBearing);
                    ctx.MoveTo(0, te.Height);
                    ctx.ShowText(testText);
                }
            }
        }

        protected void OnDrawn2 (object sender, EventArgs a)
        {
            using (var ctx = Gdk.CairoHelper.Create(scopeDrawingArea.Window))
            {
                using (var target = ctx.GetTarget ())
                {
                    // Save the state to restore it later. That will NOT save the path
                    ctx.Save ();

                    // Shape
                    ctx.MoveTo (new PointD (100,200));
                    ctx.CurveTo (new PointD (100,100), new PointD (100,100), new PointD (200,100));
                    ctx.CurveTo (new PointD (200,200), new PointD (200,200), new PointD (100,200));
                    ctx.ClosePath ();

                    var pat = new Cairo.LinearGradient (100, 200, 200, 100);
                    pat.AddColorStop (0, new Cairo.Color (0,0,0,1));
                    pat.AddColorStop (1, new Cairo.Color (1,0,0,1));
                    ctx.SetSource(pat);

                    // Fill the path with pattern
                    ctx.FillPreserve ();

                    // Color for the stroke
                    ctx.SetSourceColor(new Color (0,0,0));

                    ctx.LineWidth = 3;
                    ctx.Stroke ();

                    // We "undo" the pattern setting here
                    ctx.Restore ();
                }
            }
        }

        protected void OnDeleteEvent (object sender, DeleteEventArgs a)
        {
            Application.Quit ();
            a.RetVal = true;
        }
    }
}