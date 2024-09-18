using Egorozh.ColorPicker.Dialog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media;
using System.Windows.Media.Animation;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Point? startPoint = null;
        private System.Windows.Point? endPoint = null;
        private Line line;
        private Line strline;
        private Ellipse ellipse;
        private Rectangle rectangle;
        private ToolType toolType = ToolType.Pensil;
        private System.Windows.Media.Brush C;
        //System.Windows.Media.Brush dc = System.Windows.Media.Brushes.White;
        private System.Windows.Media.Color color;
        int t = 3;
        int index = 0;
       
        public System.Windows.Media.Brush c { get => new SolidColorBrush(ColorPickerButton.Color); set => c = C; }
        public MainWindow()
        {
            InitializeComponent();
            this.Width = 900;
            this.Height = 700;
        }
        private void InkCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition((InkCanvas)sender);
            if (toolType == ToolType.Line)
            {
                ClearPreviousLine();
                strline = new Line
                {
                    Stroke = c,
                    StrokeThickness = t,
                    X1 = startPoint.Value.X,
                    Y1 = startPoint.Value.Y,
                    X2 = startPoint.Value.X,
                    Y2 = startPoint.Value.Y
                };
                MainCanvas.Children.Add(strline);
            }
            System.Windows.Media.Color color1 = ColorPickerButton.Color;
            MainColor.Background = new SolidColorBrush(color1);
        }
        private void InkCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {

            if (toolType == ToolType.Line && startPoint != null && strline != null)
            {
                endPoint = e.GetPosition((IInputElement)sender); 

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    double angle = Math.Atan2(endPoint.Value.Y - startPoint.Value.Y, endPoint.Value.X - startPoint.Value.X);
                    double angleInDegrees = angle * (180 / Math.PI);
                    double roundedAngle = Math.Round(angleInDegrees / 45.0) * 45.0;

                    double radians = roundedAngle * (Math.PI / 180);
                    double distance = Math.Sqrt(Math.Pow(endPoint.Value.X - startPoint.Value.X, 2) + Math.Pow(endPoint.Value.Y - startPoint.Value.Y, 2));

                    endPoint = new System.Windows.Point(startPoint.Value.X + distance * Math.Cos(radians), startPoint.Value.Y + distance * Math.Sin(radians));
                }

                UpdateLine();
                startPoint = null;
                endPoint = null; 
                strline = null; 
            }

            C = MainColor.Background;
            startPoint = null;
            ellipse = null;
            rectangle = null;
            strline = null;
            
        }
        void Draw_line(System.Windows.Media.Brush c, int t, object sender, MouseEventArgs e)
        {
            System.Windows.Point currentPoint = e.GetPosition((IInputElement)sender);
            line = new Line();
            line.Stroke = c;
            line.StrokeThickness = t;
            line.X1 = startPoint?.X ?? 0.0;
            line.Y1 = startPoint?.Y ?? 0.0;
            line.X2 = currentPoint.X;
            line.Y2 = currentPoint.Y;
            MainCanvas.Children.Add(line);
            startPoint = currentPoint;
        }
        void Draw_Ellipse(System.Windows.Media.Brush c1, System.Windows.Media.Brush c2, int t, object sender, MouseEventArgs e)
        {
            System.Windows.Point currentPoint = e.GetPosition((IInputElement)sender);
            if (startPoint is { } point)
            {
                if (ellipse == null)
                {
                    ellipse = new Ellipse();
                    ellipse.Stroke = c1;
                    ellipse.Fill = c2;
                    ellipse.StrokeThickness = t;
                    MainCanvas.Children.Add(ellipse);
                }
                var r = new Rect(point, currentPoint);
                ellipse.Margin = new Thickness(r.X, r.Y, 0, 0);
                ellipse.Width = r.Width;
                ellipse.Height = r.Heigth;

            }
        }
        void Draw_Square(System.Windows.Media.Brush c1, System.Windows.Media.Brush c2, int t, object sender, MouseEventArgs e)
        {
            System.Windows.Point currentPoint = e.GetPosition((IInputElement)sender);
            if (startPoint is { } point)
            {
                if (rectangle == null)
                {
                    rectangle = new Rectangle();
                    rectangle.Stroke = c1;
                    rectangle.Fill = c2;
                    rectangle.StrokeThickness = t;
                    MainCanvas.Children.Add(rectangle);
                }
                var r = new Rect(point, currentPoint);
                rectangle.Margin = new Thickness(r.X, r.Y, 0, 0);
                rectangle.Width = r.Width;
                rectangle.Height = r.Heigth;

            }
        }
        void UpdateLine()
        {
            if (startPoint != null && endPoint != null && strline != null)
            {
                strline.X2 = endPoint.Value.X;
                strline.Y2 = endPoint.Value.Y;
            }
        }
        void ClearPreviousLine()
        {
            if (strline != null)
            {
                MainCanvas.Children.Remove(strline);
                strline = null;
            }
        }
        private void MainCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            startPoint = null;
            ellipse = null;
            rectangle = null;
            line = null;
        }
        private void InkCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            
            System.Windows.Point currentPoint = e.GetPosition((IInputElement)sender);

            if (toolType == ToolType.InkPen)
            {
                if (startPoint != null)
                {
                    Draw_line(c, t, sender, e);
                }
            }
            if (toolType == ToolType.Pensil)
            {
                if (startPoint != null)
                {
                    Draw_line(c, 1, sender, e);
                }
            }
            if (toolType == ToolType.Erase)
            {
                if (startPoint != null)
                {
                    Draw_line(System.Windows.Media.Brushes.White, t, sender, e);
                }
            }
            if (toolType == ToolType.Line && startPoint != null && strline != null)
            {
                endPoint = e.GetPosition((IInputElement)sender); 

                if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                {
                    double angle = Math.Atan2(endPoint.Value.Y - startPoint.Value.Y, endPoint.Value.X - startPoint.Value.X);
                    double angleInDegrees = angle * (180 / Math.PI);
                    double roundedAngle = Math.Round(angleInDegrees / 45.0) * 45.0;

                    double radians = roundedAngle * (Math.PI / 180);
                    double distance = Math.Sqrt(Math.Pow(endPoint.Value.X - startPoint.Value.X, 2) + Math.Pow(endPoint.Value.Y - startPoint.Value.Y, 2));

                    endPoint = new System.Windows.Point(startPoint.Value.X + distance * Math.Cos(radians), startPoint.Value.Y + distance * Math.Sin(radians));
                }

                UpdateLine(); 
            }
            if (toolType == ToolType.Square)
            {
                if (startPoint != null)
                {
                    Draw_Square(c, null, t, sender, e);
                }
            }
            if (toolType == ToolType.FillSquare)
            {
                if (startPoint != null)
                {
                    Draw_Square(c, c, t, sender, e);
                }
            }
            if (toolType == ToolType.Ellipse)
            {
                if (startPoint != null)
                {
                    Draw_Ellipse(c, null, t, sender, e);
                }
            }
            if (toolType == ToolType.FillEllipse)
            {
                if (startPoint != null)
                {
                    Draw_Ellipse(c, c, t, sender, e);
                }
            }
            
        }
        
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpg;*.jpeg;*.gif;*.bmp)|*.png;*.jpg;*.jpeg;*.gif;*.bmp|All files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                System.Windows.Media.Imaging.BitmapImage image = new System.Windows.Media.Imaging.BitmapImage(new Uri(openFileDialog.FileName));
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Source = image;
                MainCanvas.Children.Add(img);
                
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e) 
        {
            var rtb = GetRenderTargetBitmapFromControl(MainCanvas);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*";

            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                using (var fileStream = File.OpenWrite(saveFileDialog.FileName))
                {
                    encoder.Save(fileStream);
                }
            }
        }

        private static BitmapSource GetRenderTargetBitmapFromControl(Visual targetControl, double dpi = 96d)
        {
            if (targetControl == null) return null;

            var bounds = VisualTreeHelper.GetDescendantBounds(targetControl);
            var renderTargetBitmap = new RenderTargetBitmap((int)(bounds.Width * dpi / 96.0),
                                                            (int)(bounds.Height * dpi / 96.0),
                                                            dpi,
                                                            dpi,
                                                            PixelFormats.Pbgra32);

            var drawingVisual = new DrawingVisual();

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                var visualBrush = new VisualBrush(targetControl);
                drawingContext.DrawRectangle(visualBrush, null, new System.Windows.Rect(new System.Windows.Point(), (System.Windows.Point)bounds.Size));
            }

            renderTargetBitmap.Render(drawingVisual);
            return renderTargetBitmap;
        }
        
        private void Pensil_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.Pensil;
        }
        private void InkPen_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.InkPen;
        }
        private void Erase_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.Erase;
        }

        private void WhiteButton(object sender, RoutedEventArgs e)
        {

            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.White.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;
            
            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.White;
            }
            index = 0;
        }
        private void BlackButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Black.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Black;
            }
            index = 0;
        }
        private void RedButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Red.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Red;
            }
            index = 0;
        }
        private void GreenButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Green.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Green;
            }
            index = 0;
        }
        private void BlueButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Blue.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Blue;
            }
            index = 0;
        }
        private void YellowButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Yellow.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Yellow;
            }
            index = 0;
        }
        private void OrangeButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Orange.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Orange;
            }
            index = 0;
        }
        private void PurpleButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Purple.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Purple;
            }
            index = 0;
        }
        private void LightBlueButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.LightSkyBlue.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.LightSkyBlue;
            }
            index = 0;
        }
        private void PinkButton(object sender, RoutedEventArgs e)
        {
            System.Windows.Media.Color color1 = System.Windows.Media.Brushes.Pink.Color;
            MainColor.Background = new SolidColorBrush(color1);
            ColorPickerButton.Color = color1;

            if (index == 2)
            {
                DopColor.Background = System.Windows.Media.Brushes.Pink;
            }
            index = 0;
        }

        private void t1_Click(object sender, RoutedEventArgs e)
        {
            t = 1;
        }
        private void t2_Click(object sender, RoutedEventArgs e)
        {
            t = 3;
        }
        private void t3_Click(object sender, RoutedEventArgs e)
        {
            t = 7;
        }
        private void t4_Click(object sender, RoutedEventArgs e)
        {
            t = 10;
        }
        private void t5_Click(object sender, RoutedEventArgs e)
        {
            t = 15;
        }

        private void MainColorButton(object sender, RoutedEventArgs e)
        {
            
            
            index = 1;
        }
        private void DopColorButton(object sender, RoutedEventArgs e)
        {
            index = 2;
        }




        private void Line_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.Line;
        }
        private void Square_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.Square;
        }
        private void FillSquare_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.FillSquare;
        }
        private void Ellipse_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.Ellipse;
        }
        private void FillEllipse_Click(object sender, RoutedEventArgs e)
        {
            toolType = ToolType.FillEllipse;
        }

        private void MainCanvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            c = DopColor.Background;
        }
        private void MainCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            c = MainColor.Background;
        }



        enum ToolType
        {
            InkPen, Pensil, Line, Square, FillSquare, Ellipse, FillEllipse, Erase, Text
        }

        
    }
}
