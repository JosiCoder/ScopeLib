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
using System.Reflection;
using Cairo;

namespace ScopeLib.Display.Graphics
{
    /// <summary>
    /// Provides some extension methods used with Cairo.
    /// </summary>
    public static class CairoExtensions
    {
        /// <summary>
        /// Disposes a Cairo surface, considering a bug related to internal reference counting
        /// that causes a memory leak (already fixed on GitHub). The reference count is increased
        /// once too often. This considers the current reference count and thus it should still
        /// work with Cairo versions having that bug fixed.
        /// See here for more information:
        /// https://stackoverflow.com/questions/27106466/cairo-surface-is-leaking-how-to-debug-it-with-monodevelop
        /// <param name="surface">The Cairo surface to dispose.</param>
        public static void DisposeHard (this Surface surface)
        {
            var handle = surface.Handle;
            var refCount = surface.ReferenceCount;
            surface.Dispose ();
            refCount--;
            if (refCount <= 0)
            {
                return;
            }

            var assembly = typeof (Surface).Assembly;
            var nativeMethods = assembly.GetType ("Cairo.NativeMethods");
            var surfaceDestroy = nativeMethods.GetMethod ("cairo_surface_destroy",
                BindingFlags.Static | BindingFlags.NonPublic);
            for (var i = refCount; i > 0; i--)
            {
                surfaceDestroy.Invoke (null, new object [] { handle });
            }
        }

        /// <summary>
        /// Transforms the specified point according to the specified transformation matrix.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="x">The x coordinate to transform.</param>
        /// <param name="y">The y coordinate to transform.</param>
        /// <returns>The transformed point.</returns>
        public static PointD TransformPoint (this Matrix matrix, double x, double y)
        {
            matrix.TransformPoint (ref x, ref y);
            return new PointD(x, y);
        }

        /// <summary>
        /// Transforms the specified point according to the specified transformation matrix.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="point">The point to transform.</param>
        /// <returns>The transformed point.</returns>
        public static PointD TransformPoint(this Matrix matrix, PointD point)
        {
            var x = point.X;
            var y = point.Y;
            return matrix.TransformPoint (x, y);
        }

        /// <summary>
        /// Transforms the specified point according to the specified transformation matrix,
        /// in the reverse direction.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="x">The x coordinate to transform.</param>
        /// <param name="y">The y coordinate to transform.</param>
        /// <returns>The transformed point.</returns>
        public static PointD TransformPointInverse(this Matrix matrix, double x, double y)
        {
            var invertedMatrix = (Matrix)matrix.Clone();
            var status = invertedMatrix.Invert ();
            return status == Status.Success
                ? invertedMatrix.TransformPoint (x, y) : new PointD();
        }

        /// <summary>
        /// Transforms the specified point according to the specified transformation matrix,
        /// in the reverse direction.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="point">The point to transform.</param>
        /// <returns>The transformed point.</returns>
        public static PointD TransformPointInverse(this Matrix matrix, PointD point)
        {
            return matrix.TransformPointInverse(point.X, point.Y);
        }

        /// <summary>
        /// Transforms the specified distance according to the specified transformation matrix.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="x">The x distance to transform.</param>
        /// <param name="y">The y distance to transform.</param>
        /// <returns>The transformed point.</returns>
        public static Distance TransformDistance (this Matrix matrix, double dx, double dy)
        {
            matrix.TransformDistance (ref dx, ref dy);
            return new Distance (dx, dy);
        }

        /// <summary>
        /// Transforms the specified distance according to the specified transformation matrix.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="distance">The distance to transform.</param>
        /// <returns>The transformed distance.</returns>
        public static Distance TransformDistance(this Matrix matrix, Distance distance)
        {
            var dx = distance.Dx;
            var dy = distance.Dy;
            return matrix.TransformDistance (dx, dy);
        }

        /// <summary>
        /// Transforms the specified distance according to the specified transformation matrix,
        /// in the reverse direction.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="x">The x distance to transform.</param>
        /// <param name="y">The y distance to transform.</param>
        /// <returns>The transformed distance.</returns>
        public static Distance TransformDistanceInverse(this Matrix matrix, double dx, double dy)
        {
            var invertedMatrix = (Matrix)matrix.Clone();
            var status = invertedMatrix.Invert();
            return status == Status.Success
                ? invertedMatrix.TransformDistance (dx, dy) : new Distance();
        }

        /// <summary>
        /// Transforms the specified distance according to the specified transformation matrix,
        /// in the reverse direction.
        /// </summary>
        /// <param name="matrix">The transformation matrix to use.</param>
        /// <param name="distance">The distance to transform.</param>
        /// <returns>The transformed distance.</returns>
        public static Distance TransformDistanceInverse(this Matrix matrix, Distance distance)
        {
            return matrix.TransformDistanceInverse(distance.Dx, distance.Dy);
        }
    }
}

