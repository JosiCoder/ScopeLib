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
using Cairo;

namespace ScopeLib.Display.Graphics
{
    /// <summary>
    /// Provides a base implementation for Cairo-based renderers, i.e. classes that render
    /// graphics to a Cairo context.
    /// </summary>
    public class ContextRendererBase
    {
        /// <summary>
        /// Maintains the state of a Cairo context. Creating an instance of this class
        /// saves a copy of the context's current state on an internal stack, disposing
        /// an instance restores the context's previous state. Instances of this class
        /// are intended to be used with try/finally or within using blocks.
        /// </summary>
        protected class ContextState : IDisposable
        {
            private readonly Context _context;

            /// <summary>
            /// Initializes a new instance of this class.
            /// </summary>
            /// <param name="context">The context to maintain the state for.</param>
            public ContextState(Context context)
            {
                _context = context;
                _context.Save();
            }

            /// <summary>
            /// Restores the context's previous state.
            /// </summary>
            public void Dispose()
            {
                _context.Restore();
            }
        }

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="context">The context to render to.</param>
        protected ContextRendererBase (Context context)
        {
            Context = context;
        }

        /// <summary>
        /// Gets the context to render to.
        /// </summary>
        protected Context Context
        { get; private set; }

        /// <summary>
        /// Creates a new state for the context this object renders to.
        /// </summary>
        /// <returns>The new context state.</returns>
        protected ContextState CreateContextState()
        {
            return new ContextState(Context);
        }

        /// <summary>
        /// Creates a new state for the context this object renders to and
        /// applies a transformation matrix to the context within this state.
        /// </summary>
        /// <param name="matrix">The transformation matrix to apply to the context.</param>
        /// <returns>The new context state.</returns>
        protected ContextState CreateContextState(Matrix matrix)
        {
            var contextState = new ContextState(Context);
            Context.Transform (matrix);
            return contextState;
        }
    }
}

