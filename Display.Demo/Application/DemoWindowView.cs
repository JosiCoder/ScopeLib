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
using ScopeLib.Utilities;
using ScopeLib.Display.ViewModels;
using ScopeLib.Display.Views;
using ScopeLib.Display.Graphics;

namespace ScopeLib.Display.Demo
{
    /// <summary>
    /// Provides the Gtk# view of the demo window.
    /// </summary>
    public partial class DemoWindowView: Gtk.Window
    {
        private readonly DemoViewModel _viewModel;
        [UI] Gtk.Paned graphicsContainerPane;
        [UI] Gtk.Container masterGraphicsContainer;
        [UI] Gtk.Container slaveGraphicsContainer;

        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by the instance created.</param>
        public static DemoWindowView Create(DemoViewModel viewModel)
        {
            var builder = new Builder (null, "DemoWindowView.glade", null);
            return new DemoWindowView (viewModel, builder, builder.GetObject ("mainWidget").Handle);
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="viewModel">The viewmodel represented by this view.</param>
        /// <param name="builder">The Gtk# builder used to build this view.</param>
        /// <param name="handle">The handle of the main widget.</param>
        protected DemoWindowView(DemoViewModel viewModel, Builder builder, IntPtr handle) : base (handle)
        {
            _viewModel = viewModel;
            builder.Autoconnect(this);

            //  === Create sub-views. ===

            var masterScopeScreenView = ScopeScreenView.Create(_viewModel.MasterScopeScreenVM);
            masterGraphicsContainer.Add(masterScopeScreenView);

            var slaveScopeScreenView = ScopeScreenView.Create(_viewModel.SlaveScopeScreenVM);
            slaveGraphicsContainer.Add(slaveScopeScreenView);

            // === Register event handlers. ===

            DeleteEvent += OnDeleteEvent;

            // === Do some additional stuff. ===

            graphicsContainerPane.Orientation = Orientation.Vertical;
            graphicsContainerPane.DragMotion += GraphicsContainerPane_DragMotionEventHandler;
        }

        /// <summary>
        /// Performs actions whenever the scope drawing area has been drawn.
        /// </summary>
        private void GraphicsContainerPane_DragMotionEventHandler (object o, DragMotionArgs args)
        {
        }

        /// <summary>
        /// Performs actions whenever the window has been closed.
        /// </summary>
        protected void OnDeleteEvent (object sender, DeleteEventArgs a)
        {
            Application.Quit ();
            a.RetVal = true;
        }
    }
}