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
    public partial class DemoWindowView: Gtk.Window
    {
        private const bool _captureContinuously = true;
        [UI] Gtk.Container graphicsContainer;

        public static DemoWindowView Create()
        {
            var builder = new Builder (null, "DemoWindowView.glade", null);
            return new DemoWindowView (builder, builder.GetObject ("mainWidget").Handle);
        }

        protected DemoWindowView(Builder builder, IntPtr handle) : base (handle)
        {
            builder.Autoconnect(this);

            //  === Create sub-views. ===

            var scopeScreenView = ScopeScreenView.Create(new ScopeScreenViewModel());
            graphicsContainer.Add(scopeScreenView);

            // === Register event handlers. ===

            DeleteEvent += OnDeleteEvent;
        }

        protected void OnDeleteEvent (object sender, DeleteEventArgs a)
        {
            Application.Quit ();
            a.RetVal = true;
        }
    }
}