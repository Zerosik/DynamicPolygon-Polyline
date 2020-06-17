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
        public readonly List<Point> Points = new List<Point>(); //값
        private readonly List<Line> Pointcontrols = new List<Line>();//ui내 그려진 점 관리를 위함., points와 인덱스 공유
        private readonly List<Line> Lines = new List<Line>(); //ui내 그려진 선 관리를 위함.
        private readonly List<Path> Paths = new List<Path>(); //ui내 그려진 화살표 관리를 위함.
        private int SelectedPointIndex = -1;//선택된 점의 인덱스값
        private readonly Path Arrow;//화살표 표기를 위함.



        public DynamicPolyline()
        {
            InitializeComponent();
            Points.Add(new Point(100,300));
            Points.Add(new Point(300,300));

            Arrow = CloneElement(canvas.Children.OfType<Path>().FirstOrDefault()) as Path;

            DrawPoint(Points);
            DrawAllLine(Points);
        }

        private void DrawPoint(List<Point> points)
        {
            int i;
            int count = points.Count;
            ClearPoint();
            for (i = 0; i < count; i++)
            {
                Line myline = new Line();
                myline.Stroke = Brushes.Blue;
                myline.StrokeThickness = 10;
                myline.StrokeStartLineCap = PenLineCap.Round;
                myline.StrokeEndLineCap = PenLineCap.Round;
                myline.X1 = points[i].X; myline.Y1 = points[i].Y;
                myline.X2 = points[i].X; myline.Y2 = points[i].Y;
                myline.MouseLeftButtonDown += SelectPoint;
                myline.MouseLeftButtonUp += ReleasePoint;
                myline.MouseRightButtonDown += RemovePoint;
                canvas.Children.Add(myline);
                Canvas.SetZIndex(myline, 2);
                Pointcontrols.Add(myline);

            }
        }


        private void RemovePoint(object sender, MouseButtonEventArgs e)
        {
            if (Points.Count <= 2)
                return;
            Line SelectedLine = sender as Line;
            int idx = Pointcontrols.IndexOf(SelectedLine);
            Points.RemoveAt(idx);
            DrawPoint(Points);
            DrawAllLine(Points);
        }

        private void ClearPoint()
        {
            //???
            foreach (var line in Pointcontrols)
            {
                canvas.Children.Remove(line);
            }
            Pointcontrols.Clear();
        }
        private void ClearArrow()
        {
            List<Path> arrows = canvas.Children.OfType<Path>().ToList();
            foreach(var arrow in arrows)
            {
                canvas.Children.Remove(arrow);
            }
        }

        private void DrawAllLine(List<Point> points)
        {
            int i;
            int count = points.Count;

            int First = 0;
            int Last = points.Count - 1;
            ClearLine();
            ClearArrow();

            for (i = First; i < Last; i++)
            {
                Line myline = new Line();
                myline.StrokeThickness = 6;
                myline.Stroke = new SolidColorBrush(Color.FromArgb(88, 251, 40, 34));
                myline.X1 = points[i].X;
                myline.Y1 = points[i].Y;
                myline.X2 = points[i + 1].X;
                myline.Y2 = points[i + 1].Y;
                myline.MouseLeftButtonDown += InsertPoint;
                canvas.Children.Add(myline);
                Lines.Insert(i, myline);
                Canvas.SetZIndex(myline, 1);

                //https://stackoverflow.com/questions/12891516/math-calculation-to-retrieve-angle-between-two-points
                double xDiff = points[i + 1].X - points[i].X;
                double yDiff = points[i + 1].Y - points[i].Y;
                var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
                Point Between = new Point(((points[i + 1].X + points[i].X) / 2), ((points[i + 1].Y + points[i].Y) / 2));
                Path C_arrow = CloneElement(Arrow) as Path;
                C_arrow.RenderTransform = new RotateTransform(angle);
                C_arrow.Visibility = Visibility.Visible;
                Canvas.SetLeft(C_arrow, Between.X - 15);
                Canvas.SetTop(C_arrow, Between.Y - 40);
                canvas.Children.Add(C_arrow);
                Paths.Insert(i, C_arrow);

                var distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                if (distance < 100)
                {
                    C_arrow.Visibility = Visibility.Hidden;
                }

            }
        }

        private void DrawLine(List<Point> points)
        {
            int i;
            int count = points.Count;

            int First = SelectedPointIndex - 1 >= 0 ? SelectedPointIndex - 1 : SelectedPointIndex;
            int Last = SelectedPointIndex + 1 < count ? SelectedPointIndex + 1 : SelectedPointIndex;
            for(i =First; i<Last; i++)
            {
                Lines[i].X1 = points[i].X;
                Lines[i].X2 = points[i+1].X;
                Lines[i].Y1 = points[i].Y;
                Lines[i].Y2 = points[i+1].Y;

                Path C_arrow = Paths[i];
                //https://stackoverflow.com/questions/12891516/math-calculation-to-retrieve-angle-between-two-points
                //각도 구하기
                double xDiff = points[i + 1].X - points[i].X;
                double yDiff = points[i + 1].Y - points[i].Y;
                var angle = Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
                C_arrow.RenderTransform = new RotateTransform(angle);
                Point Between = new Point(((points[i + 1].X + points[i].X) / 2), ((points[i + 1].Y + points[i].Y) / 2));
                C_arrow.Visibility = Visibility.Visible;
                Canvas.SetLeft(C_arrow, Between.X - 15);
                Canvas.SetTop(C_arrow, Between.Y - 40);

                var distance = Math.Sqrt(Math.Pow(xDiff, 2) + Math.Pow(yDiff, 2));
                if (distance < 100)
                {
                    C_arrow.Visibility = Visibility.Hidden;
                }
            }


        }

        private void InsertPoint(object sender, MouseButtonEventArgs e)
        {
            int idx = Lines.IndexOf(sender as Line);
            Points.Insert(idx+1, e.GetPosition(this));
            DrawPoint(Points);
            DrawAllLine(Points);
            SelectPoint(Pointcontrols[idx + 1], e);
        }

        private void ClearLine()
        {
            foreach(var line in Lines)
            {
                canvas.Children.Remove(line);
            }
            Lines.Clear();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if(e.LeftButton == MouseButtonState.Pressed && SelectedPointIndex != -1)
            {
                //선택한 점의 좌표를 변경
                var p = e.GetPosition(this);
                Points[SelectedPointIndex] = p;
                Pointcontrols[SelectedPointIndex].X1 = p.X;
                Pointcontrols[SelectedPointIndex].X2 = p.X;
                Pointcontrols[SelectedPointIndex].Y1 = p.Y;
                Pointcontrols[SelectedPointIndex].Y2 = p.Y;
                DrawLine(Points);
            }
        }
        private void SelectPoint(object sender, MouseButtonEventArgs e)
        {
            Line SelectedLine = sender as Line;
            SelectedPointIndex = Pointcontrols.IndexOf(SelectedLine);
        }
        private void ReleasePoint(object sender, MouseButtonEventArgs e)
        {
            SelectedPointIndex = -1;
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
