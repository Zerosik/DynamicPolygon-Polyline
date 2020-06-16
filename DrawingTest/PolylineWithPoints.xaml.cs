using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace DrawingTest
{
    /// <summary>
    /// Interaction logic for PolylineWithPoints.xaml
    /// </summary>
    public partial class PolylineWithPoints : UserControl
    {
        int selectedPointIndex = -1;
        Polyline PLine;
        bool PointClicked = false;

        public PolylineWithPoints()
        {
            InitializeComponent();
            PLine = Canvas.Children.OfType<Polyline>().FirstOrDefault();

        }



        private void PolyLine_PointsLeftUp(object sender, MouseButtonEventArgs e)
        {
            //클릭
            PointClicked = false;
        }

        private void PolyLine_PointsLeftDown(object sender, MouseButtonEventArgs e)
        {
            //저장(추가)

            selectedPointIndex = GetClosePointIndex(PLine, e.GetPosition(this));
            Console.WriteLine("selectedPointIndex Changed To :{0},", selectedPointIndex);
            Line l = sender as Line;
            l.StrokeThickness = 20;
            PointClicked = true;
        }

        private void PolyLine_PointsMove(object sender, MouseEventArgs e)
        {
            //움직임
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point p = e.GetPosition(this);

                PLine.Points[selectedPointIndex] = p;
                Console.WriteLine("idx:{0},  {1}, {2}", selectedPointIndex, p.X, p.Y);

                PointsControl.ItemsSource = null;
                PointsControl.ItemsSource = PLine.Points;
                Line l = sender as Line;
                l.StrokeThickness = 20;
            }
        }

        private void PolyLine_PointsRightDown(object sender, MouseButtonEventArgs e)
        {
            //포인트 삭제
            Polyline PLine = Canvas.Children.OfType<Polyline>().FirstOrDefault();
            if (PLine.Points.Count <= 2)
                return;


            Point p = GetClosePoint(PLine, e.GetPosition(this));
            PLine.Points.Remove(p);
            PLine.UpdateLayout();

            //Line Point = sender as Line;
            //Point.Visibility = Visibility.Collapsed;
            //Canvas.Children.Remove(Point);
            PointsControl.ItemsSource = null;
            PointsControl.ItemsSource = PLine.Points;
        }

        private Point GetClosePoint(Polyline PLine, Point point)
        {
            Point MousePoint = point;
            Point ClosestPoint = PLine.Points.Select(p => new { Point = p, Distance = getDistance(p, point) }).Aggregate((p1, p2) => p1.Distance < p2.Distance ? p1 : p2).Point;
            return ClosestPoint;
        }
        private int GetClosePointIndex(Polyline PLine, Point point)
        {
            Point MousePoint = point;
            Point ClosestPoint = PLine.Points.Select(p => new { Point = p, Distance = getDistance(p, point) }).Aggregate((p1, p2) => p1.Distance < p2.Distance ? p1 : p2).Point;
            int idx = PLine.Points.IndexOf(ClosestPoint);
            return idx;
        }

        private double getDistance(Point p1, Point p2)
        {
            double distance = 0;
            distance = Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2);

            return distance;
        }








        private void Window_Initialized(object sender, EventArgs e)
        {

        }




        private void PolyLine_LineLeftDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                //더블클릭시  해당 위치에 포인트 추가
            }
        }

        private void PolyLine_LineLeftUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void PolyLine_LineLeftUp(object sender, MouseEventArgs e)
        {

        }



        

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {

            //움직임
            if (e.LeftButton == MouseButtonState.Pressed && PointClicked == true)
            {
                Point p = e.GetPosition(this);

                PLine.Points[selectedPointIndex] = p;
                Console.WriteLine("idx:{0},  {1}, {2}", selectedPointIndex, p.X, p.Y);


                PointsControl.ItemsSource = null;
                PointsControl.ItemsSource = PLine.Points;
            }
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PointClicked = false;
        }
    }
}
