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
        private const bool _captureContinuously = false;
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

            // === Do any additional stuff. ===

            _scopeGraphics = new ScopeGraphics (ScopeStretchMode.Stretch,
                _xMinimumGraticuleUnits, _yMinimumGraticuleUnits);

            InitializeGraphics();
            RefreshData();
        }

        /// <summary>
        /// Performs actions whenever the scope drawing area has been drawn.
        /// </summary>
        private void ScopeDrawingArea_DrawnEventHandler (object o, DrawnArgs args)
        {
            _scopeGraphics.Draw(scopeDrawingArea.Window);

            if (_captureContinuously)
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
        /// Creates a scope graph from a channel configuration and a signal frame.
        /// </summary>
        private ScopeGraph CreateScopeGraph(ChannelConfiguration channelConfiguration, SignalFrame signalFrame)
        {
            var timebaseConfiguration = _viewModel.TimebaseConfiguration;
            var triggerPointPosition = timebaseConfiguration.TriggerConfiguration.HorizontalPosition;

            return new ScopeGraph
            {
                LineType = _graphLineType,
                ReferencePointPosition =
                    new Cairo.PointD(
                        channelConfiguration.ReferencePointPosition.X + triggerPointPosition,
                        channelConfiguration.ReferencePointPosition.Y),
                Color = CairoHelpers.ToCairoColor(channelConfiguration.Color),
                XScaleFactor = channelConfiguration.TimeScaleFactor,
                YScaleFactor = channelConfiguration.ValueScaleFactor,
                ReferencePoint =
                    new Cairo.PointD(signalFrame.ReferenceTime, _referenceLevel),
                Vertices = signalFrame.Values
                    .Select((value, counter) => new Cairo.PointD (counter * signalFrame.TimeIncrement * timebaseConfiguration.TimeScaleFactor, value)),
            };
        }

        /// <summary>
        /// Creates the timebase cursors.
        /// </summary>
        private IEnumerable<BoundCursor> CreateTimebaseCursors()
        {
            var timebaseConfig = _viewModel.TimebaseConfiguration;
            var triggerConfig = timebaseConfig.TriggerConfiguration;
            var channelConfig = triggerConfig.ChannelConfiguration;

            var cursors = new List<BoundCursor>();

            if (channelConfig == null)
            {
                ; // intentionally left blank
            }
            else if (triggerConfig is LevelTriggerConfiguration)
            {
                cursors.Add(TriggerCursorFactory.CreateTriggerCriteriaCursor(
                    triggerConfig as LevelTriggerConfiguration, channelConfig,
                    () => _referenceLevel));
            }
            // Add more cases for other types of triggers here.
            // ...

            cursors.Add(TriggerCursorFactory.CreateTriggerPointCursor(timebaseConfig));

            bool bothCursorsVisible =
                channelConfig.MeasurementCursorA.Visible &&
                channelConfig.MeasurementCursorB.Visible;

            if (timebaseConfig.MeasurementCursorA.Visible)
            {
                cursors.Add(MeasurementCursorFactory.CreateTimeMeasurementCursor(
                    timebaseConfig.MeasurementCursorA, timebaseConfig, bothCursorsVisible,
                    null,
                    () => _referenceTime));
            }

            if (timebaseConfig.MeasurementCursorB.Visible)
            {
                cursors.Add(MeasurementCursorFactory.CreateTimeMeasurementCursor(
                    timebaseConfig.MeasurementCursorB, timebaseConfig, false,
                    bothCursorsVisible ? () => timebaseConfig.MeasurementCursorA.Value : (Func<double>)null,
                    () => _referenceTime));

            }

            return cursors;
        }

        /// <summary>
        /// Creates the cursors for all channels.
        /// </summary>
        private IEnumerable<BoundCursor> CreateChannelCursors()
        {
            var channelConfig = _viewModel.ChannelConfigurations;

            // Note that the last cursor in the list has the highest priority when 
            // searching them after a click.

            var cursors = new List<BoundCursor>();

            cursors.AddRange(channelConfig
                .Select((channelConf, index) => ChannelCursorFactory.CreateChannelReferenceCursor(channelConf, index)));

            channelConfig.ForEach(chConfig =>
                {
                    bool bothCursorsVisible =
                        chConfig.MeasurementCursorA.Visible &&
                        chConfig.MeasurementCursorB.Visible;

                    if (chConfig.MeasurementCursorA.Visible)
                    {
                        cursors.Add(MeasurementCursorFactory.CreateLevelMeasurementCursor(
                            chConfig.MeasurementCursorA, chConfig, bothCursorsVisible,
                            null,
                            () => _referenceLevel));
                    }

                    if (chConfig.MeasurementCursorB.Visible)
                    {
                        cursors.Add(MeasurementCursorFactory.CreateLevelMeasurementCursor(
                            chConfig.MeasurementCursorB, chConfig, false,
                            bothCursorsVisible ? () => chConfig.MeasurementCursorA.Value : (Func<double>)null,
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

            _scopeGraphics.Graphs = CollectionUtilities.Zip(
                objects => CreateScopeGraph(objects[0] as ChannelConfiguration, objects[1] as SignalFrame),
                _viewModel.ChannelConfigurations, _viewModel.CurrentSignalFrames);

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
            var boundCursors = CreateChannelCursors().Concat(CreateTimebaseCursors());
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
                        _captureContinuously ? string.Format ("{0} fps", _framesPerSecond) : "hold",
                },
            };
        }
    }
}

