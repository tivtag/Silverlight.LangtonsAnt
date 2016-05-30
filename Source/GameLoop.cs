// <copyright file="GameLoop.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines various Ant.GameLoop class.</summary>
// <author>Paul Ennemoser</author>

namespace Ant
{
    using System;
    using System.Windows;
    using System.Windows.Media.Animation;

    /// <summary>
    /// Defines the delegate that gets executed every 'frame'.
    /// </summary>
    /// <param name="elapsedTime">
    /// The time the last frame took.
    /// </param>
    internal delegate void GameLoopUpdateEventHandler( TimeSpan elapsedTime );

    /// <summary>
    /// The GameLoop class allows the usage of a standart game-loop
    /// in a Silverlight application.
    /// This class can't be inherited.
    /// </summary>
    internal sealed class GameLoop
    {
        #region [ Events ]

        /// <summary>
        /// Fired when this GameLoop is updating.
        /// </summary>
        public event GameLoopUpdateEventHandler Update;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets the (unique) name that identifies this GameLoop.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        #endregion

        #region [ Constructors ]

        /// <summary>
        /// Initializes a new instance of the <see cref="GameLoop"/> class.
        /// </summary>
        /// <param name="name">
        /// The (unique) name that identifies the new GameLoop.
        /// </param>
        public GameLoop( string name )
        {
            if( name == null )
                throw new ArgumentNullException( "name" );

            this.Name       = name;

            this.storyboard = new Storyboard();
            this.storyboard.Completed += new EventHandler( OnStoryboardCompleted );
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Attaches this GameLoop to the given <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The element to attach at.</param>
        /// <exception cref="InvalidOperationException">
        /// If this GameLoop is already attached to a <see cref="FrameworkElement"/>. Detach it first.
        /// </exception>
        public void Attach( FrameworkElement element )
        {
            if( this.targetElement != null )
            {
                throw new InvalidOperationException(
                    @"Can't attach this GameLoop to the given FrameworkElement.
                      The GameLoop is already attached to a FrameworkElement."
                );
            }

            this.targetElement = element;
            this.targetElement.Resources.Add( this.Name, storyboard );

            this.lastUpdateTime = DateTime.Now;
            this.storyboard.Begin();
        }

        ///// <summary>
        ///// Detachs this GameLoop from the current <see cref="FrameworkElement"/>.
        ///// </summary>
        ///// <exception cref="InvalidOperationException">
        ///// If this GameLoop currently is not attached to any <see cref="FrameworkElement"/>.
        ///// </exception>
        //public void Detach()
        //{
        //    if( this.targetElement == null )
        //    {
        //        throw new InvalidOperationException(
        //            "Can't detach the GameLoop. It's not attached to any FrameworkElement."
        //        );
        //    }

        //    this.storyboard.Stop();

        //    this.targetElement.Resources.Remove( this.Name );
        //    this.targetElement = null;
        //}

        /// <summary>
        /// Gets called when one frame has ended.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contains the event data.</param>
        private void OnStoryboardCompleted( object sender, EventArgs e )
        {
            this.elapsedTime    = DateTime.Now - this.lastUpdateTime;
            this.lastUpdateTime = DateTime.Now;

            if( this.Update != null )
                this.Update( elapsedTime );

            // Restart the frame:
            if( this.storyboard != null )
                this.storyboard.Begin();
        }

        #endregion

        #region [ Fields ]

        /// <summary>
        /// Stores the TimeSpan the last frame took to execute.
        /// </summary>
        private TimeSpan elapsedTime;

        /// <summary>
        /// Stores the time at which the last frame has ended.
        /// </summary>
        private DateTime lastUpdateTime = DateTime.MinValue;

        /// <summary>
        /// Identifies the <see cref="FrameworkElement"/> this GameLoop is currently attached to.
        /// </summary>
        private FrameworkElement targetElement;

        /// <summary>
        /// The Storyboard which drives the GameLoop.
        /// </summary>
        private readonly Storyboard storyboard;

        #endregion
    }
}
