// <copyright file="App.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>Defines the Silverlight application class that runs the Ants Cellular Automat.</summary>
// <author>Paul Ennemoser</author>

namespace Ant
{
    using System;
    using System.Windows;
    using System.Windows.Browser;

    /// <summary>
    /// Defines the Silverlight application class that runs the Ants Cellular Automaton.
    /// </summary>
    public sealed partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class.
        /// </summary>
        public App()
        {
            // Hook events.
            this.Startup            += this.OnApplicationStartup;
            this.Exit               += this.OnApplicationExit;
            this.UnhandledException += this.OnApplicationUnhandledException;

            InitializeComponent();
        }

        /// <summary>
        /// Called when Application is firing up for the first time.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The StartupEventArgs that contain the event data.</param>
        private void OnApplicationStartup( object sender, StartupEventArgs e )
        {
            // Load the main control.
            this.RootVisual = new MainPage();
        }

        /// <summary>
        /// Called when Application is closing for good.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The EventArgs that contain the event data.</param>
        private void OnApplicationExit( object sender, EventArgs e )
        {
        }

        /// <summary>
        /// Called when an exception that has been raised during the execution of 
        /// this TickWebpage.App has not been handled.
        /// </summary>
        /// <param name="sender">
        /// The sender of the event.
        /// </param>
        /// <param name="e">
        /// The ApplicationUnhandledExceptionEventArgs that contains the event data.
        /// </param>
        private void OnApplicationUnhandledException( object sender, ApplicationUnhandledExceptionEventArgs e )
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if( !System.Diagnostics.Debugger.IsAttached )
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;

                // Inform User:
                try
                {
                    var culture = System.Globalization.CultureInfo.InvariantCulture;

                    string error         = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                    string formatedError = error.Replace( '"', '\'' ).Replace( "\r\n", @"\n" );

                    string message = string.Format(
                        culture,
                        "An unhandled error has ocurred in the Silverlight application {0}.",
                        formatedError
                    );

                    string code = string.Format( culture, "throw new Error(\"{0}\");", message );
                    HtmlPage.Window.Eval( code );
                }
                catch( Exception ) { }
            }
        }
    }
}
