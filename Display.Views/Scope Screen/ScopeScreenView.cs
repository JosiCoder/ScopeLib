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
using System.Collections.Specialized;
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

        private const double _referenceLevel = 0.0;
        private const bool _captureContinuously = true;
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
        /// Converts a view model point to a Cairo point.
        /// </summary>
        private static Cairo.PointD ToCairoPointD(ScopeLib.Display.ViewModels.Point point)
        {
            return new Cairo.PointD(point.X, point.Y);
        }

        /// <summary>
        /// Converts a view model color to a Cairo color.
        /// </summary>
        private static Cairo.Color ToCairoColor(ScopeLib.Display.ViewModels.Color color)
        {
            return new Cairo.Color(color.R, color.G, color.B);
        }

        /// <summary>
        /// Creates a scope graph from a channel configuration and a signal frame.
        /// </summary>
        private static ScopeGraph CreateScopeGraph(
            double triggerPointPosition,
            ChannelConfiguration channelConfiguration,
            SignalFrame signalFrame)
        {
            return new ScopeGraph
            {
                LineType = _graphLineType,
                ReferencePointPosition =
                    new Cairo.PointD(
                        channelConfiguration.ReferencePointPosition.X + triggerPointPosition,
                        channelConfiguration.ReferencePointPosition.Y),
                Color = ToCairoColor(channelConfiguration.Color),
                XScaleFactor = channelConfiguration.TimeScaleFactor,
                YScaleFactor = channelConfiguration.ValueScaleFactor,
                ReferencePoint =
                    new Cairo.PointD(signalFrame.ReferenceTime, _referenceLevel),
                Vertices = signalFrame.Values
                    .Select((value, counter) => new Cairo.PointD (counter * signalFrame.TimeIncrement, value)),
            };
        }

        /// <summary>
        /// Creates the trigger cursors.
        /// </summary>
        private IEnumerable<ScopeCursor> CreateTriggerCursors()
        {
            var triggerConfiguration = _viewModel.TriggerConfiguration;

            var channelConfig = _viewModel.ChannelConfigurations
                .Skip(triggerConfiguration.ChannelNumber)
                .FirstOrDefault();

            IEnumerable<ScopeCursor> triggerCriteriaCursors = new ScopeCursor[0];

            if (channelConfig == null)
            {
                ; // intentionally left blank
            }
            else if (triggerConfiguration is LevelTriggerConfiguration)
            {
                triggerCriteriaCursors = CreateTriggerCriteriaCursors(
                    triggerConfiguration as LevelTriggerConfiguration, channelConfig);
            }
            // Add more cases for other types of triggers here.

            var timeReferenceCursors = CreateTriggerPointCursors(triggerConfiguration);

            return triggerCriteriaCursors.Concat(timeReferenceCursors);
        }

        /// <summary>
        /// Creates the trigger criteria cursors for a level-based trigger.
        /// </summary>
        private IEnumerable<ScopeCursor> CreateTriggerCriteriaCursors(
            LevelTriggerConfiguration triggerConfiguration,
            ChannelConfiguration triggerChannelConfiguration)
        {
            const string triggerCaption = "T";
            Func<String> levelTextProvider = () =>
                string.Format("{0:F2}", triggerConfiguration.Level);

            var levelColor = ToCairoColor(triggerChannelConfiguration.Color);

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.Y,
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
                val => (val - _referenceLevel) * valueScaleFactor() + triggerLevelReferencePosition(),
                val => ((val - triggerLevelReferencePosition()) / valueScaleFactor()) + _referenceLevel);

            // === Create bindings. ===

            // Bind the cursor's position.
            PB.Binding.Create (() => cursor.Position.Y == triggerLevelConverter.DerivedValue);
            PB.Binding.Create (() => triggerLevelConverter.OriginalValue == triggerConfiguration.Level);

            return new []
            {
                cursor,
            };
        }

        /// <summary>
        /// Creates the trigger point cursors.
        /// </summary>
        private IEnumerable<ScopeCursor> CreateTriggerPointCursors(
            TriggerConfigurationBase triggerConfiguration)
        {
            const string triggerCaption = "T";
            Func<String> positionTextProvider = () =>
                string.Format("{0:F2}", triggerConfiguration.HorizontalPosition);

            var markerColor = new Cairo.Color (0.0, 0.5, 1.0);

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.X,
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
            PB.Binding.Create (() => cursor.Position.X == triggerConfiguration.HorizontalPosition);

            return new []
            {
                cursor,
            };
        }

        /// <summary>
        /// Creates the reference line cursors for all channels.
        /// </summary>
        private IEnumerable<ScopeCursor> CreateChannelCursors()
        {
            // Create the cursors immediately and just once.
            return _viewModel.ChannelConfigurations
                .SelectMany((channelConf, index) => CreateChannelCursors(channelConf, index+1))
                .ToArray();
        }

        /// <summary>
        /// Creates the reference line cursors for a single channel.
        /// </summary>
        private IEnumerable<ScopeCursor> CreateChannelCursors(ChannelConfiguration channelConfiguration,
            int channelNumber)
        {
            var channelCaption = channelNumber.ToString();
            var channelColor = ToCairoColor(channelConfiguration.Color);

            var cursor = new ScopeCursor
            {
                Lines = ScopeCursorLines.Y,
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
            PB.Binding.Create (() => cursor.Position.Y == channelConfiguration.ReferencePointPosition.Y);

            return new []
            {
                cursor,
            };
        }

        /// <summary>
        /// Initializes the scope graphics.
        /// </summary>
        private void InitializeGraphics()
        {
            var textColor = new Cairo.Color (1, 1, 0);
            var cursor1Color = new Cairo.Color (1, 0.5, 0.5);
//            var cursor2Color = new Cairo.Color (0.5, 1, 0.5);
//            var cursor3Color = new Cairo.Color (0.5, 0.5, 1);

            _scopeGraphics.Graphs = CollectionUtilities.Zip(
                objects => CreateScopeGraph(_viewModel.TriggerConfiguration.HorizontalPosition,
                    objects[0] as ChannelConfiguration, objects[1] as SignalFrame),
                _viewModel.ChannelConfigurations, _viewModel.CurrentSignalFrames);

            var demoCursors = new []
            {
                new ScopeCursor
                {
                    Position = new ScopePosition(3, 3),
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

//                new ScopeCursor
//                {
//                    Position = new PointD (-2.5, -2.7),
//                    Lines = ScopeCursorLines.X,
//                    Markers = ScopeCursorMarkers.XRight,
//                    Color = cursor2Color,
//                    Captions = new []
//                    {
//                        new ScopePositionCaption(() => "yLB", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, textColor),
//                        new ScopePositionCaption(() => "yLT", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Top, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, true, textColor),
//                        new ScopePositionCaption(() => "yRT", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top, ScopeAlignmentReference.XPositionAndVerticalRangeEdge, false, textColor),
//                    },
//                    YTicks = new []
//                    {
//                        new ScopeCursorValueTick(-0.5),
//                        new ScopeCursorValueTick(0.5),
//                        new ScopeCursorValueTick(3.0)
//                    },
//                },
//                new ScopeCursor
//                {
//                    Position = new PointD (-3.5, -3.7),
//                    Lines = ScopeCursorLines.Y,
//                    Markers = ScopeCursorMarkers.YLower,
//                    Color = cursor3Color,
//                    Captions = new []
//                    {
//                        new ScopePositionCaption(() => "xRT", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Top, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, true, textColor),
//                        new ScopePositionCaption(() => "xRB", ScopeHorizontalAlignment.Right, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, false, textColor),
//                        new ScopePositionCaption(() => "xLB", ScopeHorizontalAlignment.Left, ScopeVerticalAlignment.Bottom, ScopeAlignmentReference.YPositionAndHorizontalRangeEdge, false, textColor),
//                    },
//                    XTicks = new []
//                    {
//                        new ScopeCursorValueTick(-0.3),
//                        new ScopeCursorValueTick(0.3),
//                        new ScopeCursorValueTick(2.8)
//                    },
//                },
            };

            _scopeGraphics.Cursors =
                CreateTriggerCursors()
                    .Concat(CreateChannelCursors())
                    .Concat(demoCursors);

//            _scopeGraphics.Cursors.First().Captions.First().TextProvider = () =>
//                string.Format("x={0:F2}/y={1:F2}", _scopeGraphics.Cursors.First().Position.X,  _scopeGraphics.Cursors.First().Position.Y);
//
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

