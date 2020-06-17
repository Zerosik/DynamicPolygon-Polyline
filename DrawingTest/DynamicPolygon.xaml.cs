using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DrawingTest
{
    /// <summary>
    /// Interaction logic for DynamicPolygon.xaml
    /// </summary>
    public partial class DynamicPolygon : UserControl
    {
        List<Point> points = new List<Point>(); //값
        List<Line> pointcontrols = new List<Line>();//ui에 그려지는 point값, points와 인덱스 공유
        List<Line> lines = new List<Line>(); //그려진 선들
        Polygon polygon = new Polygon();
        int SelectedPointIndex = -1;
        Point PreviewPoint;
        bool PolygonSelected = false;


        public DynamicPolygon()
        {
            InitializeComponent();
            points.Add(new Point(100, 100));
            points.Add(new Point(100, 300));
            points.Add(new Point(300, 300));
            points.Add(new Point(300, 100));

            canvas.Children.Add(polygon);
            polygon.Fill = new SolidColorBrush(Color.FromArgb(80, 251, 40, 34));
            polygon.MouseLeftButtonDown += Polygon_MouseLeftButtonDown; 
            polygon.MouseLeftButtonUp += Polygon_MouseLeftButtonUp;
            polygon.MouseMove += Polygon_MouseMove;
            Canvas.SetZIndex(polygon, 0);

            DrawPoint(points);
            DrawLine(points, true);
            DrawPolygon();
        }

        private void Polygon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PolygonSelected = false;
        }

        private void Polygon_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PolygonSelected = true;
            PreviewPoint = e.GetPosition(this);
        }

        private void Polygon_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && PolygonSelected)
            {
                if (PreviewPoint.X == 0 && PreviewPoint.Y == 0)
                {
                    PreviewPoint = e.GetPosition(this);
                    return;

                }
                Point newPoint = e.GetPosition(this);
                double MoveX = newPoint.X - PreviewPoint.X;
                double MoveY = newPoint.Y - PreviewPoint.Y;

                points = points.Select(p =>
                {
                    p.X = p.X + MoveX;
                    p.Y = p.Y + MoveY;
                    return p;
                }).ToList();
                PreviewPoint = e.GetPosition(this);
                DrawPoint(points);
                DrawLine(points, true);
            }
        }

        private void DrawPolygon()
        {
            PointCollection pc = new PointCollection(points);
            polygon.Points = pc;
        }

        private void DrawPoint(List<Point> points)
        {
            int i;
            int count = points.Count;
            DrawPolygon();
            ClearPoint();
            for (i = 0; i < count; i++)
            {
                Line myline = new Line();
                myline.MouseLeftButtonDown += SelectPoint;
                myline.MouseLeftButtonUp += ReleasePoint;
                myline.MouseRightButtonDown += RemovePoint;
                myline.Stroke = Brushes.Blue;
                myline.StrokeThickness = 10;
                myline.StrokeStartLineCap = PenLineCap.Round;
                myline.StrokeEndLineCap = PenLineCap.Round;
                myline.X1 = points[i].X;
                myline.Y1 = points[i].Y;
                myline.X2 = points[i].X;
                myline.Y2 = points[i].Y;
                canvas.Children.Add(myline);
                Canvas.SetZIndex(myline, 2);
                pointcontrols.Add(myline);
            }
        }

        private void RemovePoint(object sender, MouseButtonEventArgs e)
        {
            if (points.Count <= 3)
                return;
            Line SelectedLine = sender as Line;
            int idx = pointcontrols.IndexOf(SelectedLine);
            points.RemoveAt(idx);
            DrawPoint(points);
            DrawLine(points, true);


        }

        private void ReleasePoint(object sender, MouseButtonEventArgs e)
        {
            SelectedPointIndex = -1;
        }

        private void ClearPoint()
        {
            //???
            foreach (var line in pointcontrols)
            {
                canvas.Children.Remove(line);
            }
            pointcontrols.Clear();
        }
        private void ClearArrow()
        {
            List<Path> arrows = canvas.Children.OfType<Path>().ToList();
            foreach (var arrow in arrows)
            {
                canvas.Children.Remove(arrow);
            }
        }
        private void DrawLine(List<Point> points, bool drawAll)
        {
            int i;
            int count = points.Count;

            int back = 0;
            int forth = points.Count - 1;
            if (drawAll == true)
            {
                ClearLine();
                ClearArrow();
            }
            else
            {
                back = SelectedPointIndex - 1;
                forth = SelectedPointIndex;
                
                for (int n = forth; n >= back; n--)
                {
                    if(n == -1)
                    {

                        canvas.Children.Remove(lines[count-2]);
                        lines.RemoveAt(count - 2);
                        continue;
                    }
                    canvas.Children.Remove(lines[n]);
                    lines.RemoveAt(n);
                    
                }
            }
            for (i = back; i <= forth; i++)
            {
                Line myline = new Line();
                myline.MouseLeftButtonDown += InsertPoint;
                myline.StrokeThickness = 6;
                myline.Stroke = new SolidColorBrush(Color.FromArgb(88, 251, 40, 34));
                
                if (i < points.Count - 1 && i>=0)
                {
                    myline.X1 = points[i].X;
                    myline.Y1 = points[i].Y;
                    myline.X2 = points[i + 1].X;
                    myline.Y2 = points[i + 1].Y;
                    lines.Insert(i, myline);
                }
                else
                {
                    myline.X1 = points[count-1].X;
                    myline.Y1 = points[count-1].Y;
                    myline.X2 = points[0].X;
                    myline.Y2 = points[0].Y;
                    lines.Add(myline);
                }

                //double xDiff = points[i + 1].X - points[i].X;
                //double yDiff = points[i + 1].Y - points[i].Y;
                //var distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                //var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
                //Point Between = new Point(((points[i + 1].X + points[i].X) / 2), ((points[i + 1].Y + points[i].Y) / 2));


                canvas.Children.Add(myline);
                //lines.Insert(i, myline);
                Canvas.SetZIndex(myline, 1);
            }

            



        }

        private void InsertPoint(object sender, MouseButtonEventArgs e)
        {
            int idx = lines.IndexOf(sender as Line);
            points.Insert(idx + 1, e.GetPosition(this));
            DrawPoint(points);
            DrawLine(points, true);
            SelectPoint(pointcontrols[idx + 1], e);
        }

        private void ClearLine()
        {
            foreach (var line in lines)
            {
                canvas.Children.Remove(line);
            }
            lines.Clear();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && SelectedPointIndex != -1)
            {
                //int Cidx = canvas.Children.IndexOf(SelectedLine);
                //int Pidx = pointcontrols.IndexOf(SelectedLine);
                points[SelectedPointIndex] = e.GetPosition(this);
                DrawPoint(points);
                DrawLine(points, false);
            }
        }
        private void SelectPoint(object sender, MouseButtonEventArgs e)
        {
            Line SelectedLine = sender as Line;
            SelectedPointIndex = pointcontrols.IndexOf(SelectedLine);
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PolygonSelected = false;
            SelectedPointIndex = -1;
        }


        public static UIElement CloneElement(UIElement Source)
        {

            if (Source == null) return null;

            string XAML = System.Windows.Markup.XamlWriter.Save(Source);

            System.IO.StringReader StringReader = new System.IO.StringReader(XAML);
            System.Xml.XmlReader xmlReader = System.Xml.XmlTextReader.Create(StringReader);

            return (UIElement)System.Windows.Markup.XamlReader.Load(xmlReader);

        }
    }
}
