﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Point> _bonusPoints = new List<Point>();

        private readonly List<Point> _snakePoints = new List<Point>();

        private readonly Brush _snakeColor = Brushes.Green;
        private enum SnakeSize
        {
            Thin = 4,
            Normal = 6,
            Thick = 8
        };
        private enum Movingdirection
        {
            Upwards = 8,
            Downwards = 2,
            Toleft = 4,
            Toright = 6
        };

        private enum GameSpeed
        {
            Fast = 1,
            Moderate = 10000,
            Slow = 50000,
            DamnSlow = 500000
        };

        private readonly Point _startingPoint = new Point(100, 100);
        private Point _currentPosition = new Point();


        private int _direction = 0;


        private int _previousDirection = 0;


        private readonly int _headSize = (int)SnakeSize.Thick;

        private int _length = 100;
        private int _score = 0;
        private readonly Random _rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);

            timer.Interval = new TimeSpan((int)GameSpeed.Moderate);
            timer.Start();
            
            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            PaintSnake(_startingPoint);
            _currentPosition = _startingPoint;

            for (var n = 0; n < 10; n++)
            {
                PaintBonus(n);
            }
        }

        private void PaintSnake(Point currentposition)
        {
            
            Ellipse newEllipse = new Ellipse
            {
                Fill = _snakeColor,
                Width = _headSize,
                Height = _headSize
            };

            Canvas.SetTop(newEllipse, currentposition.Y);
            Canvas.SetLeft(newEllipse, currentposition.X);

            int count = PaintCanvas.Children.Count;
            PaintCanvas.Children.Add(newEllipse);
            _snakePoints.Add(currentposition);

            if (count > _length)
            {
                PaintCanvas.Children.RemoveAt(count - _length + 9);
                _snakePoints.RemoveAt(count - _length);
            }
        }
        
        private void PaintBonus(int index)
        {
            Point bonusPoint = new Point(_rnd.Next(5, 780), _rnd.Next(5, 480));

            Ellipse newEllipse = new Ellipse
            {
                Fill = Brushes.Red,
                Width = _headSize,
                Height = _headSize
            };

            Canvas.SetTop(newEllipse, bonusPoint.Y);
            Canvas.SetLeft(newEllipse, bonusPoint.X);
            PaintCanvas.Children.Insert(index, newEllipse);
            _bonusPoints.Insert(index, bonusPoint);

        }


        private void timer_Tick(object sender, EventArgs e)
        {

            switch (_direction)
            {
                case (int)Movingdirection.Downwards:
                    _currentPosition.Y += 1;
                    PaintSnake(_currentPosition);
                    break;
                case (int)Movingdirection.Upwards:
                    _currentPosition.Y -= 1;
                    PaintSnake(_currentPosition);
                    break;
                case (int)Movingdirection.Toleft:
                    _currentPosition.X -= 1;
                    PaintSnake(_currentPosition);
                    break;
                case (int)Movingdirection.Toright:
                    _currentPosition.X += 1;
                    PaintSnake(_currentPosition);
                    break;
            }

            if ((_currentPosition.X < 5) || (_currentPosition.X > 780) ||
                (_currentPosition.Y < 5) || (_currentPosition.Y > 480))
                GameOver();

            int n = 0;
            foreach (Point point in _bonusPoints)
            {

                if ((Math.Abs(point.X - _currentPosition.X) < _headSize) &&
                    (Math.Abs(point.Y - _currentPosition.Y) < _headSize))
                {
                    _length += 10;
                    _score += 10;

                    _bonusPoints.RemoveAt(n);
                    PaintCanvas.Children.RemoveAt(n);
                    PaintBonus(n);
                    break;
                }
                n++;
            }

            for (int q = 0; q < (_snakePoints.Count - _headSize * 2); q++)
            {
                Point point = new Point(_snakePoints[q].X, _snakePoints[q].Y);
                if ((Math.Abs(point.X - _currentPosition.X) < (_headSize)) &&
                     (Math.Abs(point.Y - _currentPosition.Y) < (_headSize)))
                {
                    GameOver();
                    break;
                }

            }
            
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                    if (_previousDirection != (int)Movingdirection.Upwards)
                        _direction = (int)Movingdirection.Downwards;
                    break;
                case Key.Up:
                    if (_previousDirection != (int)Movingdirection.Downwards)
                        _direction = (int)Movingdirection.Upwards;
                    break;
                case Key.Left:
                    if (_previousDirection != (int)Movingdirection.Toright)
                        _direction = (int)Movingdirection.Toleft;
                    break;
                case Key.Right:
                    if (_previousDirection != (int)Movingdirection.Toleft)
                        _direction = (int)Movingdirection.Toright;
                    break;

            }
            _previousDirection = _direction;

        }



        private void GameOver()
        {
            MessageBox.Show($@"Pathetic! Your score is only { _score}", "My granny does better", MessageBoxButton.OK, MessageBoxImage.Hand);
            this.Close();
        }
    }
}
