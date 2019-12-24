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
using UI = Gtk.Builder.ObjectAttribute;
using PB = Praeclarum.Bind;
using ScopeLib.Utilities;
using ScopeLib.Sampling;
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

        private const double _referenceTime = 0.0;
        private const double _referenceLevel = 0.0;
        private const bool _drawContinuously = false;
        private const double _xMinimumGraticuleUnits = 10.0;
        private const double _yMinimumGraticuleUnits = 8.0;

        private const ScopeLineType _graphLineType = ScopeLineType.LineAndDots;
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

            scopeDrawingArea.Drawn += ScopeDrawingArea_DrawnEventHandler;

            // Raise motion events even if no button is pressed.
            scopeEventBox.Events |= Gdk.EventMask.PointerMotionMask;

            scopeEventBox.MotionNotifyEvent += ScopeEventBox_MotionNotifyEventHandler;
            scopeEventBox.ButtonPressEvent += ScopeEventBox_ButtonPressEventHandler;
            scopeEventBox.ButtonReleaseEvent += ScopeEventBox_ButtonReleaseEventHandler;

            // === Create value converters. ===

            // === Create bindings. ===

            // === Do some additional stuff. ===

            _scopeGraphics = new ScopeGraphics (ScopeStretchMode.Stretch,
                _xMinimumGraticuleUnits, _yMinimumGraticuleUnits);

            // Allow shrinking the drawing area completely.
            scopeDrawingArea.HeightRequest = 0;
            scopeDrawingArea.WidthRequest = 0;

            InitializeGraphics();
            RefreshData();
        }

        /// <summary>
        /// Performs actions whenever the scope drawing area has been drawn.
        /// </summary>
        private void ScopeDrawingArea_DrawnEventHandler (object o, DrawnArgs args)
        {
            _scopeGraphics.Draw(scopeDrawingArea.Window);

            if (_drawContinuously)
            {
                RefreshData();
                RefreshGraphics();
            }
        }

        /// <summary>
        /// Performs actions whenever the mouse has been moved within the scope drawing area.
        /// </summary>
        private void ScopeEventBox_MotionNotifyEventHandler (object o, MotionNotifyEventArgs args)
        {
            if (_currentMouseButtons != 0)
            {
                _scopeGraphics.SetSelectedCursorLinesToPosition (new Cairo.PointD (args.Event.X, args.Event.Y));
            }
            else
            {
                _scopeGraphics.FindAndHighlightCursorLines(new Cairo.PointD(args.Event.X, args.Event.Y));
            }
            RefreshGraphics ();
        }

        /// <summary>
        /// Performs actions whenever a mouse button has been pressed within the scope drawing area.
        /// </summary>
        private void ScopeEventBox_ButtonPressEventHandler (object o, ButtonPressEventArgs args)
        {
            _currentMouseButtons = args.Event.Button;
            _scopeGraphics.FindAndSelectCursorLines (new Cairo.PointD(args.Event.X, args.Event.Y));
            RefreshGraphics ();
        }

        /// <summary>
        /// Performs actions whenever a mouse button has been released within the scope drawing area.
        /// </summary>
        private void ScopeEventBox_ButtonReleaseEventHandler (object o, ButtonReleaseEventArgs args)
        {
            _currentMouseButtons = 0;
            _scopeGraphics.DeselectScopeCursorLines ();
            _scopeGraphics.FindAndHighlightCursorLines (new Cairo.PointD(args.Event.X, args.Event.Y));
            RefreshGraphics ();
        }

        /// <summary>
        /// Initiates a refresh of the scope graphics.
        /// </summary>
        private void RefreshGraphics()
        {
            var currentDrawSecond = DateTime.Now.Second;
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

        /// <summary>
        /// Refreshes the data shown on the scope screen.
        /// </summary>
        private void RefreshData()
        {
            _captureDateTime = DateTime.Now;

            // Here we could capture more data.
        }

        /// <summary>
        /// Creates a scope graph from a channel configuration and a sample sequence provider.
        /// </summary>
        private ScopeGraph CreateScopeGraph(ChannelViewModel channelVM, Func<SampleSequence> sampleSequenceProvider)
        {
            var graphbaseVM = _viewModel.GraphbaseVM;
            var triggerPointPosition = graphbaseVM.TriggerVM.HorizontalPosition;

            var sampleSequence = sampleSequenceProvider();

            return new ScopeGraph
            {
                LineType = _graphLineType,
                ReferencePointPosition =
                    new Cairo.PointD(
                        channelVM.ReferencePointPosition.X + triggerPointPosition,
                        channelVM.ReferencePointPosition.Y),
                Color = CairoHelpers.ToCairoColor(channelVM.Color),
                XScaleFactor = channelVM.XScaleFactor,
                YScaleFactor = channelVM.YScaleFactor,
                ReferencePoint =
                    new Cairo.PointD(sampleSequence.ReferenceX, _referenceLevel),
                Vertices = sampleSequence.Values
                    .Select((value, counter) => new Cairo.PointD (counter * sampleSequence.SampleInterval * graphbaseVM.ScaleFactor, value)),
            };
        }

        /// <summary>
        /// Creates the graphbase (e.g. timebase) cursors.
        /// </summary>
        private IEnumerable<BoundCursor> CreateGraphbaseCursors()
        {
            var graphbaseVM = _viewModel.GraphbaseVM;
            var triggerVM = graphbaseVM.TriggerVM;
            var channelVM = triggerVM.ChannelVM;

            var cursors = new List<BoundCursor>();

            if (channelVM == null)
            {
                ; // intentionally left blank
            }
            else if (triggerVM is LevelTriggerViewModel)
            {
                cursors.Add(TriggerCursorFactory.CreateTriggerCriteriaCursor(
                    triggerVM as LevelTriggerViewModel, channelVM,
                    () => _referenceLevel));
            }
            // Add more cases for other types of triggers here.
            // ...

            cursors.Add(TriggerCursorFactory.CreateTriggerPointCursor(graphbaseVM));

            bool bothCursorsVisible =
                graphbaseVM.MeasurementCursor1VM.Visible &&
                graphbaseVM.MeasurementCursor2VM.Visible;

            if (graphbaseVM.MeasurementCursor1VM.Visible)
            {
                cursors.Add(MeasurementCursorFactory.CreateTimeMeasurementCursor(
                    graphbaseVM.MeasurementCursor1VM, graphbaseVM, bothCursorsVisible,
                    null,
                    () => _referenceTime));
            }

            if (graphbaseVM.MeasurementCursor2VM.Visible)
            {
                cursors.Add(MeasurementCursorFactory.CreateTimeMeasurementCursor(
                    graphbaseVM.MeasurementCursor2VM, graphbaseVM, false,
                    bothCursorsVisible ? () => graphbaseVM.MeasurementCursor1VM.Value : (Func<double>)null,
                    () => _referenceTime));

            }

            return cursors;
        }

        /// <summary>
        /// Creates the cursors for all channels.
        /// </summary>
        private IEnumerable<BoundCursor> CreateChannelCursors()
        {
            var channelConfig = _viewModel.ChannelVMs;

            // Note that the last cursor in the list has the highest priority when 
            // searching them after a click.

            var cursors = new List<BoundCursor>();

            cursors.AddRange(channelConfig
                .Select(channelConf => ChannelCursorFactory.CreateChannelReferenceCursor(channelConf)));

            channelConfig.ForEachDo(chConfig =>
                {
                    bool bothCursorsVisible =
                        chConfig.MeasurementCursor1VM.Visible &&
                        chConfig.MeasurementCursor2VM.Visible;

                    if (chConfig.MeasurementCursor1VM.Visible)
                    {
                        cursors.Add(MeasurementCursorFactory.CreateLevelMeasurementCursor(
                            chConfig.MeasurementCursor1VM, chConfig, bothCursorsVisible,
                            null,
                            () => _referenceLevel));
                    }

                    if (chConfig.MeasurementCursor2VM.Visible)
                    {
                        cursors.Add(MeasurementCursorFactory.CreateLevelMeasurementCursor(
                            chConfig.MeasurementCursor2VM, chConfig, false,
                            bothCursorsVisible ? () => chConfig.MeasurementCursor1VM.Value : (Func<double>)null,
                            () => _referenceLevel));
                    }
                });

            return cursors;
        }

        /// <summary>
        /// Initializes the scope graphics.
        /// </summary>
        private void InitializeGraphics()
        {
            var textColor = new Cairo.Color (1, 1, 0);
            var cursor1Color = new Cairo.Color (1, 0.5, 0.5);

            // Reorder the channel information and sample sequence providers so that the trigger channel comes first.
            var channelDataWithTriggerChannelFirst = CollectionUtilities.Zip(objects =>
                new
                {
                    ChannelVM = objects[0] as ChannelViewModel,
                    SampleSequence = objects[1] as Func<SampleSequence>
                },
                _viewModel.ChannelVMs, _viewModel.SampleSequenceProviders)
                .OrderByDescending(item => item.ChannelVM == _viewModel.GraphbaseVM.TriggerVM.ChannelVM);

            // Create the scope graphs (essentially consisting of channel information and sample sequence providers).
            // Note that the trigger channel comes first.
            _scopeGraphics.Graphs = CollectionUtilities.Zip(
                objects => CreateScopeGraph(objects[0] as ChannelViewModel, objects[1] as Func<SampleSequence>),
                channelDataWithTriggerChannelFirst.Select(a => a.ChannelVM),
                channelDataWithTriggerChannelFirst.Select(a => a.SampleSequence));

            var demoCursors = new []
            {
                new ScopeCursor
                {
                    Position = new ScopePosition(-4.5, -3.5),
                    Lines = ScopeCursorLines.Both,
                    LineWeight = ScopeCursorLineWeight.Medium,
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
            };

            // Note that the last cursor in the list has the highest priority when 
            // searching them after a click.
            var boundCursors = CreateChannelCursors().Concat(CreateGraphbaseCursors());
            _scopeGraphics.Cursors =
                boundCursors.Select(cursor => cursor.EmbeddedCursor).Concat(demoCursors);

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
                    Color = new Cairo.Color (.5,1,1),
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
                        string.Format ("{0} fps ({1})", _framesPerSecond, _drawContinuously ? "cont" : "evnt"),
                },
            };
        }
    }
}

