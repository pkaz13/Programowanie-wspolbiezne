using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Semafory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point startPoint;
        private const int startPointX = 0;
        private const int startPointY = 300;
        private const int stepX = 50;
        private const int stepY = -50;
        int iteration = 5;

        Stack<Point> points;
        Stack<Point> newPoints;

        Thread thread1;
        Thread thread2;

        Semaphore semaphore;

        List<KeyValuePair<Point, Point>> listOfPoints;
        public MainWindow()
        {
            InitializeComponent();
            listOfPoints = new List<KeyValuePair<Point, Point>>();
            semaphore = new Semaphore(1, 1);
            startPoint = new Point(startPointX, startPointY);
            points = new Stack<Point>();
            newPoints = new Stack<Point>();
        }

        private void DrawLine(Point from, Point to, Canvas targetCanvas)
        {
            Line line = new Line();
            line.Stroke = Brushes.Blue;
            line.StrokeThickness = 1.0;
            line.X1 = from.X;
            line.Y1 = from.Y;
            line.X2 = to.X;
            line.Y2 = to.Y;
            targetCanvas.Children.Add(line);
        }

        private void Draw()
        {
            while (true)
            {
                semaphore.WaitOne();
                if (iteration > 0)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} startuje");

                    if (points.Count > 0)
                    {

                        Point newPointUp;
                        Point newPointRight;

                        Point currentPoint = points.Pop();
                        newPointUp = new Point(currentPoint.X, currentPoint.Y + stepY);
                        newPointRight = new Point(currentPoint.X + stepX, currentPoint.Y);
                        newPoints.Push(newPointUp);
                        listOfPoints.Add(new KeyValuePair<Point, Point>(currentPoint, newPointUp));
                        newPoints.Push(newPointRight);
                        listOfPoints.Add(new KeyValuePair<Point, Point>(currentPoint, newPointRight));
                        listOfPoints.Add(new KeyValuePair<Point, Point>(newPointRight, newPointUp));

                        Console.WriteLine($"{Thread.CurrentThread.Name} wszedł");
                        Console.WriteLine($"{Thread.CurrentThread.Name} kończy");

                    }
                    else
                    {
                        Console.WriteLine($"{Thread.CurrentThread.Name} nie ma punktów.");
                        points = newPoints;
                        newPoints = new Stack<Point>();
                        iteration--;
                    }
                }
                else
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name} zatrzymany.");
                    Console.WriteLine($"{Thread.CurrentThread.Name} zatrzymany.");
                    return;
                }
                semaphore.Release();
            }

        }

        private void btnRysuj_Click(object sender, RoutedEventArgs e)
        {
            List<KeyValuePair<Point, Point>> listOfPointsWtihoutDuplicates = listOfPoints.Distinct().ToList();
            foreach (var item in listOfPointsWtihoutDuplicates)
            {
                DrawLine(item.Key, item.Value, canvas);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            
            canvas.Children.Clear();
            listOfPoints.Clear();
            iteration = Convert.ToInt32(txtIteration.Text);

            points.Push(startPoint);
            thread1 = new Thread(new ThreadStart(Draw));
            thread1.Name = "Thread1";
            thread2 = new Thread(new ThreadStart(Draw));
            thread2.Name = "Thread2";
            thread1.Start();
            thread2.Start();
        }
    }
}
