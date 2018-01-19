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
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ScopeLib.Utilities;

namespace ScopeLib.Display.ViewModels
{
    /// <summary>
    /// Provides a base implementation for viewmodels.
    /// </summary>
    public abstract class ViewModelBase : NotifyingBase
    {
        private readonly TaskScheduler _scheduler;
        private readonly Thread _uiThread;

        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        public ViewModelBase()
        {
            _uiThread = Thread.CurrentThread;
            try
            {
                // Sometimes we don't have a proper synchronization context (e.g. during unit tests).
                _scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            }
            catch
            {
                ; // intentionally left empty
            }
        }

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has a new value.
        /// This defaults to the name of the calling member.</param>
        protected override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (Thread.CurrentThread != _uiThread)
            {
                System.Diagnostics.Debug.WriteLine (
                    "RaisePropertyChanged for {0} was called on thread id {1}, ignored.",
                    propertyName, Thread.CurrentThread.ManagedThreadId);
                return;
            }

            base.RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Schedules the specified action using the task scheduler obtained from the
        /// synchronization context of the thread this object was created on. This causes
        /// the action to be dispatched on the UI thread if this object was created on the
        /// UI thread).
        /// The action is called directly if no task scheduler could be obtained from
        /// the synchronization context.
        /// </summary>
        /// <param name="action">The action to dispatch.</param>
        protected virtual void DispatchOnUIThread(System.Action action)
        {
            if (_scheduler != null)
            {
                Task.Factory.StartNew (() => { action (); },
                    Task.Factory.CancellationToken, TaskCreationOptions.None, _scheduler);
            }
            else
            {
                action ();
            }
        }
    }
}