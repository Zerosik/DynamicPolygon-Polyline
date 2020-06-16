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
    /// Interaction logic for CustomPolyline.xaml
    /// </summary>
    public partial class DynamicPolyline : UserControl
    {
        List<Point> points = new List<Point>(); //값
        List<Line> pointcontrols = new List<Line>();//ui에 그려지는 point값, points와 인덱스 공유
        List<Line> lines = new List<Line>(); //그려진 선들
        List<Path> paths = new List<Path>();
        int SelectedPointIndex = -1;
        Path Arrow;



        public DynamicPolyline()
        {
            InitializeComponent();
            points.Add(new Point(100,300));
            points.Add(new Point(300,300));

            Arrow = CloneElement(canvas.Children.OfType<Path>().FirstOrDefault()) as Path;

            DrawPoint(points);
            DrawLine(points, true);
        }

        private void DrawPoint(List<Point> points)
        {
            int i;
            int count = points.Count;
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
        private  void ClearArrow()
        {
            List<Path> arrows = canvas.Children.OfType<Path>().ToList();
            foreach(var arrow in arrows)
            {
                canvas.Children.Remove(arrow);
            }
        }
        private void DrawLine(List<Point> points, bool drawAll)
        {
            int i;
            int count = points.Count;
            
            int back = 0;
            int forth = points.Count -1;
            if (drawAll == true)
            {
                ClearLine();
                ClearArrow();
            }
            else
            {
                back = SelectedPointIndex - 1 >= 0 ? SelectedPointIndex - 1 : SelectedPointIndex;
                forth = SelectedPointIndex+1 < count ? SelectedPointIndex+1 : SelectedPointIndex;
                for(int n = forth-1; n>= back; n--)
                {
                    canvas.Children.Remove(paths[n]);
                    paths.RemoveAt(n);
                    canvas.Children.Remove(lines[n]);
                    lines.RemoveAt(n);
                }
            }
            for (i = back; i < forth; i++)
            {
                Line myline = new Line();
                myline.MouseLeftButtonDown += InsertPoint;
                myline.StrokeThickness = 6;
                myline.Stroke = new SolidColorBrush( Color.FromArgb(88, 251, 40, 34));
                myline.X1 = points[i].X;
                myline.Y1 = points[i].Y;
                myline.X2 = points[i + 1].X;
                myline.Y2 = points[i + 1].Y;

                double xDiff = points[i + 1].X - points[i].X;
                double yDiff = points[i + 1].Y - points[i].Y;
                var distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
                Point Between = new Point(((points[i + 1].X + points[i].X) / 2), ((points[i + 1].Y + points[i].Y) / 2));
                Path C_arrow = CloneElement(Arrow) as Path;
                C_arrow.RenderTransform = new RotateTransform(angle);
                C_arrow.Visibility = Visibility.Visible;
                Canvas.SetLeft(C_arrow, Between.X - 15);
                Canvas.SetTop(C_arrow, Between.Y - 40);
                canvas.Children.Add(C_arrow);

                paths.Insert(i, C_arrow);
                if (distance < 100)
                {
                    C_arrow.Visibility = Visibility.Hidden;
                }
                canvas.Children.Add(myline);
                lines.Insert(i,myline);
                Canvas.SetZIndex(myline, 1);
            }
        }

        private void InsertPoint(object sender, MouseButtonEventArgs e)
        {
            int idx = lines.IndexOf(sender as Line);
            points.Insert(idx+1, e.GetPosition(this));
            DrawPoint(points);
            DrawLine(points, true);
            SelectPoint(pointcontrols[idx + 1], e);
        }

        private void ClearLine()
        {
            foreach(var line in lines)
            {
                canvas.Children.Remove(line);
            }
            lines.Clear();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && SelectedPointIndex != -1)
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
