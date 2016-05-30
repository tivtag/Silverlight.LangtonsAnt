// <copyright file="Direction4.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Ant.Direction4 enumeration.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Ant
{
    /// <summary>
    /// Enumerates the four main direction on the 2d plane.
    /// </summary>
    internal enum Direction4
    {
        /// <summary>
        /// No specified direction.
        /// </summary>
        None=0,

        /// <summary> Left (west). </summary>
        Left,

        /// <summary> Right (east). </summary>
        Right,

        /// <summary> Up (north). </summary>
        Up,

        /// <summary> Down (south). </summary>
        Down
    }
}
