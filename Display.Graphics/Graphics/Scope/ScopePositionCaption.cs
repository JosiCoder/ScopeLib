﻿//------------------------------------------------------------------------------
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
using Cairo;

namespace ScopeLib.Display.Graphics
{
    /// <summary>
    /// Provides a caption applied to a scope position.
    /// </summary>
    public class ScopePositionCaption
    {
        private readonly Color _defaultColor = new Color (1, 1, 1);

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        public ScopePositionCaption ()
        {
            Color = _defaultColor;
        }

        /// <summary>
        /// Initializes an instance of this class.
        /// </summary>
        /// <param name="textProvider">A function returning the caption text.</param>
        /// <param name="horizontalAlignment">
        /// The horizontal alignment with respect to the specified alignment reference.
        /// </param>
        /// <param name="verticalAlignment">
        /// The vertical alignment with respect to the specified alignment reference.
        /// </param>
        /// <param name="alignmentReference">
        /// The reference used for horizontal and vertical alignment.</param>
        /// <param name="yieldToMarker">
        /// A value indicating whether to provide additional space for the cursor marker.
        /// </param>
        /// <param name="color">The caption color.</param>
        public ScopePositionCaption (Func<string> textProvider, ScopeHorizontalAlignment horizontalAlignment,
            ScopeVerticalAlignment verticalAlignment, ScopeAlignmentReference alignmentReference, bool yieldToMarker,
            Color color)
        {
            TextProvider = textProvider;
            HorizontalAlignment = horizontalAlignment;
            VerticalAlignment = verticalAlignment;
            AlignmentReference = alignmentReference;
            YieldToMarker = yieldToMarker;
            Color = color;
        }

        /// <summary>
        /// Gets or sets a function returning the caption text.
        /// </summary>
        public Func<string> TextProvider
        { get; set; }

        /// <summary>
        /// Gets or sets the horizontal alignment with respect to the specified alignment reference.
        /// </summary>
        public ScopeHorizontalAlignment HorizontalAlignment
        { get; set; }

        /// <summary>
        /// Gets or sets the vertical alignment with respect to the specified alignment reference.
        /// </summary>
        public ScopeVerticalAlignment VerticalAlignment
        { get; set; }

        /// <summary>
        /// Gets or sets the reference used for horizontal and vertical alignment.
        /// </summary>
        public ScopeAlignmentReference AlignmentReference
        { get; set; }

        /// <summary>
        /// Gets or sets the caption color.
        /// </summary>
        public Color Color
        { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to provide additional space for the cursor marker.
        /// </summary>
        public bool YieldToMarker
        { get; set; }

        /// <summary>
        /// Gets the current caption text.
        /// </summary>
        public string CurrentText
        {
            get{ return (TextProvider != null) ? TextProvider() : null; }
        }

        /// <summary>
        /// Gets or sets a fixed caption text.
        /// </summary>
        /// <param name="text">The text to use.</param>
        public void SetFixedText(string text)
        {
            TextProvider = () => text;
        }
    }
}

