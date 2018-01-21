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
using PB = Praeclarum.Bind;
using ScopeLib.Display.Graphics;

namespace ScopeLib.Display.Views
{
    /// <summary>
    /// Provides a scope cursor based on data binding.
    /// </summary>
    public class BoundCursor : IDisposable
    {
        private PB.Binding _binding;

        /// <summary>
        /// Gets the embedded cursor.
        /// </summary>
        public ScopeCursor EmbeddedCursor
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="embeddedCursor">The cursor embedded in this object.</param>
        /// <param name="binding">The binding used by this object.</param>
        public BoundCursor (ScopeCursor embeddedCursor, PB.Binding binding)
        {
            EmbeddedCursor = embeddedCursor;
            _binding = binding;
        }

        /// <summary>
        /// Releases the underlying binding.
        /// </summary>
        public void Dispose()
        {
            if (_binding != null)
            {
                _binding.Unbind();
            }
        }
    }
}

