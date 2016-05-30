// <copyright file="Page.xaml.cs" company="federrot Software">
//     Copyright (c) federrot Software. All rights reserved.
// </copyright>
// <summary>
//     Defines the Ant.MainPage class.
// </summary>
// <author>
//     Paul Ennemoser
// </author>

namespace Ant
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Shapes;

    /// <summary>
    /// Imagine an ant-field made out of boxes, called cells.
    /// Each cell may be either colored black or white.
    /// At the start of the journey every cell is white,
    /// but the cell of our little ant is black.
    /// The algorithm, which moves our ant over the ant-field,
    /// is as followed:
    /// 1. Move Ant.
    /// 2. The cell the Ant moved onto changes color (black -> white, white -> black)
    /// 3. If the cell was black, then turn the Ant 90° left.
    /// If the cell was white, then turn the Ant 90° right.
    /// 4. Start again at 1.
    /// If we follow the ant on her journey she seems to generate a quite chaotic structure.
    /// But suddenly at around 10000 steps the structure the ant is creating
    /// turns into a non-chaotic, for-ever repeating structure.
    /// This proves that a simple algorithm that produces a chaotic structure may infact change its behaviour to be non-chaotic.
    /// See http://mathworld.wolfram.com/LangtonsAnt.html for a more in-depth explanation.
    /// </summary>
    public sealed partial class MainPage : UserControl, INotifyPropertyChanged
    {
        #region [ Events ]

        /// <summary>
        /// Called when a property of this Page has changed.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region [ Properties ]

        /// <summary>
        /// Gets or sets the speed the simulation is running at.
        /// </summary>
        public double SimulationSpeed
        {
            get
            {
                return tickTime.Milliseconds / 1000.0;
            }

            set
            {                
                tickTime = TimeSpan.FromMilliseconds( value * 1000.0 );

                if( PropertyChanged != null )
                {
                    PropertyChanged( this, new PropertyChangedEventArgs( "SimulationSpeed" ) );
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation
        /// is currently running.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }

            set
            {
                isRunning = value;

                if( value )
                {
                    buttonStart.IsEnabled = false;
                    buttonPause.IsEnabled = true;
                }
                else
                {
                    buttonStart.IsEnabled = true;
                    buttonPause.IsEnabled = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the simulation
        /// is currently paused.
        /// </summary>
        public bool IsPaused
        {
            get
            {
                return isPaused;
            }

            set
            {
                isPaused = value;

                if( value )
                {
                    buttonStart.IsEnabled = true;
                    buttonPause.IsEnabled = false;
                }
                else
                {
                    buttonPause.IsEnabled = true;
                }
            }
        }

        #endregion

        #region [ Initialization ]

        /// <summary>
        /// Initializes a new instance of the MainPage class.
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.CreateField( 45 * 16, 45 * 16, 8 );

            this.gameLoop = new GameLoop( "Ant Loop" );
            this.gameLoop.Update += UpdateSimulation;
            this.gameLoop.Attach( this );
        }

        /// <summary>
        /// Creates the ant field on which the simulation runs on.
        /// </summary>
        /// <param name="fieldWidth">The width of the field (in tile-space).</param>
        /// <param name="fieldHeight">The height of the field (in tile-space).</param>
        /// <param name="cellSize">The size of a single cell.</param>
        private void CreateField( int fieldWidth, int fieldHeight, int cellSize )
        {
            antGrid.Children.Clear();
            antGrid.RowDefinitions.Clear();
            antGrid.ColumnDefinitions.Clear();

            // Create rows.
            this.rows = fieldWidth / cellSize;
            for( int i = 0; i < rows; ++i )
            {
                antGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(cellSize) });
            }

            // Create columns.
            this.columns = fieldHeight / cellSize;
            for( int i = 0; i < columns; ++i )
            {
                antGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(cellSize) });
            }

            // Create field.
            this.antField = new Rectangle[rows, columns];

            // And finally.. create the cells :)
            for( int row = 0; row < rows; ++row )
            {
                for( int column = 0; column < columns; ++column )
                {
                    Rectangle cell = new Rectangle() {
                        Fill = brushA
                    };

                    Grid.SetRow( cell, row );
                    Grid.SetColumn( cell, column );

                    antField[column, row] = cell;
                    antGrid.Children.Add( cell );
                }
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Updates the Ant simulation.
        /// </summary>
        /// <param name="elapsedTime">
        /// The time the last frame took to execute.
        /// </param>
        private void UpdateSimulation( TimeSpan elapsedTime )
        {
            if( !this.IsRunning || this.IsPaused )
            {
                return;
            }

            tickTimeLeft -= elapsedTime;
            
            if( tickTimeLeft <= TimeSpan.Zero )
            {
                SimulateAnt();
                tickTimeLeft = tickTime;
            }
        }

        /// <summary>
        /// Simulates the next step of the Ant.
        /// </summary>
        private void SimulateAnt()
        {
            switch( antDirection )
            {
                case Direction4.Up:
                    antPositionY -= 1;
                    break;

                case Direction4.Down:
                    antPositionY += 1;
                    break;

                case Direction4.Left:
                    antPositionX -= 1;
                    break;

                case Direction4.Right:
                    antPositionX += 1;
                    break;
            }

            if( antPositionX < 0 )
            {
                antPositionX = columns - 1;
            }
            else if( antPositionX >= columns )
            {
                antPositionX = 0;
            }

            if( antPositionY < 0 )
            {
                antPositionY = rows - 1;
            }
            else if( antPositionY >= rows )
            {
                antPositionY = 0;
            }

            Rectangle cell = antField[antPositionX, antPositionY];

            if( cell.Fill == brushA )
            {
                cell.Fill = brushB;
                TurnAntRight();
            }
            else
            {
                cell.Fill = brushA;
                TurnAntLeft();
            }

            ++step;
            textBoxStep.Text = "Step: " + step.ToString();
        }

        /// <summary>
        /// Turns the ant left.
        /// </summary>
        private void TurnAntLeft()
        {
            switch( antDirection )
            {
                case Direction4.Left:
                    antDirection = Direction4.Down;
                    break;

                case Direction4.Down:
                    antDirection = Direction4.Right;
                    break;

                case Direction4.Right:
                    antDirection = Direction4.Up;
                    break;

                case Direction4.Up:
                    antDirection = Direction4.Left;
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Turns the ant right.
        /// </summary>
        private void TurnAntRight()
        {
            switch( antDirection )
            {
                case Direction4.Left:
                    antDirection = Direction4.Up;
                    break;

                case Direction4.Up:
                    antDirection = Direction4.Right;
                    break;

                case Direction4.Right:
                    antDirection = Direction4.Down;
                    break;

                case Direction4.Down:
                    antDirection = Direction4.Left;
                    break;

                default:
                    break;
            }
        }

        #region - Events -

        /// <summary>
        /// Called when the user clicks on the Start button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnStartButtonClicked( object sender, RoutedEventArgs e )
        {
            if( !this.IsPaused )
            {
                antPositionX = rows    / 2 - 8;
                antPositionY = columns / 2;
                tickTimeLeft = tickTime;
                step = 0;
            }

            this.IsPaused  = false;
            this.IsRunning = true;
        }

        /// <summary>
        /// Called when the user clicks on the Pause button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnPauseButtonClicked( object sender, RoutedEventArgs e )
        {
            this.IsPaused = true;
        }

        /// <summary>
        /// Called when the user clicks on the Fill button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnFillButtonClicked( object sender, RoutedEventArgs e )
        {
            int count = (rows * columns) / 20;

            if( random == null )
            {
                random = new Random();
            }

            for( int i = 0; i < count; ++i )
            {
                int row    = random.Next( 0, rows );
                int column = random.Next( 0, columns );

                antField[column, row].Fill = brushB;
            }
        }

        /// <summary>
        /// Called when the user clicks on the Reset button.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The RoutedEventArgs that contain the event data.</param>
        private void OnResetButtonClicked( object sender, RoutedEventArgs e )
        {
            this.IsPaused  = false;
            this.IsRunning = false;

            foreach( Rectangle cell in antField )
            {
                cell.Fill = brushA;
            }

            textBoxStep.Text = string.Empty;
        }

        /// <summary>
        /// Gets called when the value of the speed slider control has changed.
        /// </summary>
        /// <param name="sender">The sender fo the event.</param>
        /// <param name="e">The RoutedPropertyChangedEventArgs{Double} that contain the event data.</param>
        private void OnSliderSpeedValueChanged( object sender, RoutedPropertyChangedEventArgs<double> e )
        {
            // Workaround, Binding is still somewhat bugged in SL B2
            this.SimulationSpeed = sliderSpeed.Value;
        }

        #endregion

        #endregion

        #region [ Fields ]

        #region - Ant Simulation -

        /// <summary>
        /// The current position of the ant on the x-axis.
        /// </summary>
        private int antPositionX;

        /// <summary>
        /// The current position of the ant on the y-axis.
        /// </summary>
        private int antPositionY;

        /// <summary>
        /// The current direction the ant is facing.
        /// </summary>
        private Direction4 antDirection = Direction4.Down;

        #endregion

        #region - Ant Field -

        /// <summary>
        /// The number of rows the ant field has.
        /// </summary>
        private int rows;

        /// <summary>
        /// The number of columns the ant field has.
        /// </summary>
        private int columns;

        /// <summary>
        /// The cellular field the ant is running over.
        /// </summary>
        private Rectangle[,] antField;


        #endregion

        #region - Simulation -

        /// <summary>
        /// States whether the simulation is currently running.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// States whether a running simulation has been paused by the user.
        /// </summary>
        private bool isPaused;

        /// <summary>
        /// The time between two simulation steps.
        /// </summary>
        private TimeSpan tickTime;

        /// <summary>
        /// The time left until the next simulation step is done.
        /// </summary>
        private TimeSpan tickTimeLeft;

        /// <summary>
        /// The number of steps since the start of the simulation.
        /// </summary>
        private int step;

        #endregion

        #region - Other -

        /// <summary>
        /// A random number generator.
        /// </summary>
        private Random random;

        /// <summary>
        /// The GameLoop that runs the simulation.
        /// </summary>
        private readonly GameLoop gameLoop;

        #endregion

        #endregion
    }
}
