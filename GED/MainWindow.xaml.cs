using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using static System.Media.SystemSounds;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using Microsoft.Win32;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows.Threading;

namespace GED
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            create_DG();
            canvas_SizeChanged(null, null);
            panel_options.Width = 0;
            panel_options_fr.Width = 0;
            gs = new Group_shape();
            list_canvas.Add(gs);
            count_list_canvas.Text = "1";
        }

        Line line = new Line(); //линия
        Ellipse el = new Ellipse(); //эллипс
        Point p; //точка
        bool draw_ok = false; //рисовать
        bool canmove = false; //двигать

        List<Line> grid_art = new List<Line>(); //коллекция сетки
        Group_shape gs/* = new Group_shape()*/; //коллекция групп
        List<Group_shape> list_canvas = new List<Group_shape>(); //коллекция листов
        List<Shape> group = new List<Shape>(); //коллекция передвигаемых линий
        List<Shape> gr_morf = new List<Shape>(); //коллекция линий для морфинга
        Shape[] figure; //старые позиции морфинга

        #region Base_methods
        private void std_bt_Click(object sender, RoutedEventArgs e)
        {
            draw_ok = false;
            gs.morf_ok = false;
            canvas.Cursor = Cursors.Arrow;
            md.Content = "контроль";

            if (gr_morf != null)
                foreach (Line obj in gr_morf)
                    obj.Stroke = Brushes.Black;
            if (group != null)
                foreach (Line obj in group)
                    obj.Stroke = Brushes.Green;
        } //курсор взаимодействия
        private void line_bt_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveimg = new Microsoft.Win32.SaveFileDialog();
            draw_ok = true;
            gs.morf_ok = false;
            canvas.Cursor = Cursors.Cross;
            md.Content = "отрисовка";
            if (gr_morf != null)
                foreach (Line obj in gr_morf)
                    obj.Stroke = Brushes.Black;
            if (group != null)
                foreach (Line obj in group)
                    obj.Stroke = Brushes.Black;

        } //курсор рисования
        private void bt_morf_Click(object sender, RoutedEventArgs e)
        {
            canvas.Cursor = Cursors.Arrow;
            gs.morf_ok = true;
            md.Content = "морфинг";
            if (gr_morf != null)
                foreach (Line obj in gr_morf)
                    obj.Stroke = Brushes.Violet;
            if (group != null)
                foreach (Line obj in group)
                    obj.Stroke = Brushes.Black;
        } //выбран морфиг
        private void save_morf_Click(object sender, RoutedEventArgs e)
        {
            List<Shape> tmp = new List<Shape>();
            if (gr_morf != null)
                foreach (Line obj in gr_morf)
                {
                    if (gs.Contains(obj))
                        obj.Stroke = Brushes.Black;
                    else
                        tmp.Add(obj);
                }
            gr_morf.Clear();
            gr_morf = tmp;
            list_in_mas();
            Slider_morf.Value = 0;

        } //очистить морфинг с сохранением
        private void cancle_morf_Click(object sender, RoutedEventArgs e)
        {
            List<Shape> tmp = new List<Shape>();
            Morfing(0);
            if (gr_morf != null)
                foreach (Line obj in gr_morf)
                {
                    if (gs.Contains(obj))
                        obj.Stroke = Brushes.Black;
                    else
                        tmp.Add(obj);
                }
            gr_morf.Clear();
            gr_morf = tmp;
            list_in_mas();
            Slider_morf.Value = 0;
        } //очистить морфинг без сохранения
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.D1)
                std_bt_Click(sender, e);
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.D2)
                line_bt_Click(sender, e);
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Z && gs.Count() != 0)
            {
                if (group.Contains(gs[gs.Count() - 1] as Line))
                    group.Remove(gs[gs.Count() - 1] as Line);
                gs.RemoveAt(gs.Count() - 1); //стирает последнюю линиб
                gs_in_canvas();
            }
            if (e.Key == Key.NumPad4)
            {
                bt_backward_Click(sender, e);
            }
            if (e.Key == Key.NumPad6)
            {
                bt_forward_Click(sender, e);
            }
            if (e.Key == Key.V && canvas.Cursor == Cursors.Arrow)
            {
                md.Content = "выделение";
                gs.select_ok = true; //переход в режим выделения
            }
        } //режимы управления
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift)
            {
                md.Content = "контроль";
                gs.select_ok = false;
                Mouse.Capture(null);
                //foreach (var obj in group)
                //{
                //    (obj as Line).Tag = 0;
                //    (obj as Line).Stroke = Brushes.Black;
                //}
                foreach (var obj in gs)
                {
                    if ((int)(obj as Line).Tag != -10)
                    {
                        (obj as Line).Tag = 0;
                        (obj as Line).Stroke = Brushes.Black;
                    }
                }
                group.Clear();
                //figure = null;
            }
        } //освобождение захваченных линий

        public void test_all_line_border()
        {
            foreach (Line obj in gs)
                if ((int)obj.Tag != -10)
                    test_border(obj.X1, obj.Y1, obj.X2, obj.Y2);
        } //обход всех линий канвы и определение границ
        public void AddLineToBackground(double x1, double y1, double x2, double y2)
        {
            var line = new Line()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                Stroke = Brushes.LightGray,
                StrokeThickness = 1,
                SnapsToDevicePixels = true
            };
            if (x1 == 0 && y1 == 0)
                line.Stroke = Brushes.IndianRed;
            line.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            line.Tag = -10;
            Canvas.SetZIndex(line, -1);
            grid_art.Add(line);
            canvas.Children.Add(line);
        } //рисоване сетки а канве
        public void test_border(double x1, double y1, double x2, double y2)
        {

            //scr_view.ActualHeight
            if (x1 > canvas.ActualWidth || x2 > canvas.ActualWidth)
            {
                if (x1 > x2)
                    canvas.Width = x1;
                else
                    canvas.Width = x2;
            }
            else if (x1 < scr_view.ActualWidth && x2 < scr_view.ActualWidth)
                canvas.Width = scr_view.Width;

            if (y1 > canvas.ActualHeight || y2 > canvas.ActualHeight)
            {
                if (y1 > y2)
                    canvas.Height = y1;
                else
                    canvas.Height = y2;
            }
            else if (y1 < scr_view.ActualHeight && y2 < scr_view.ActualHeight)
                canvas.Height = scr_view.Height;
        } //определение выхода фигуры за гранцу канвы
        public void create_DG()
        {

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    TextBox t = new TextBox();
                    t.Text = "0";
                    t.TextAlignment = TextAlignment.Center;
                    t.Name = "DG_items_" + (i + 1) + (j + 1);
                    t.PreviewTextInput += Scl_PreviewTextInput;
                    t.PreviewKeyDown += Scl_PreviewKeyDown;
                    DG.Children.Add(t);
                    Grid.SetRow(t, i);
                    Grid.SetColumn(t, j);
                }
        } //создание матрицы преобразований
        public void list_in_mas()
        {
            if (gr_morf != null)
            {
                figure = new Shape[gr_morf.Count];
                for (int i = 0; i < gr_morf.Count; i++)
                {
                    Line tmp = new Line();
                    figure[i] = tmp;
                    (figure[i] as Line).X1 = (gr_morf[i] as Line).X1;
                    (figure[i] as Line).X2 = (gr_morf[i] as Line).X2;
                    (figure[i] as Line).Y1 = (gr_morf[i] as Line).Y1;
                    (figure[i] as Line).Y2 = (gr_morf[i] as Line).Y2;
                }
            }
        } //массив старых положений в морфинге
        public void Morfing(double now)
        {
            if (gr_morf != null)
                for (int i = 0; i < gr_morf.Count - 1; i++)
                {
                    if (i % 2 == 0 && gs.Contains(gr_morf[i]))
                    {
                        (gr_morf[i] as Line).X1 = (gr_morf[i + 1] as Line).X1 * now + (1 - now) * (figure[i] as Line).X1;
                        (gr_morf[i] as Line).X2 = (gr_morf[i + 1] as Line).X2 * now + (1 - now) * (figure[i] as Line).X2;
                        (gr_morf[i] as Line).Y1 = (gr_morf[i + 1] as Line).Y1 * now + (1 - now) * (figure[i] as Line).Y1;
                        (gr_morf[i] as Line).Y2 = (gr_morf[i + 1] as Line).Y2 * now + (1 - now) * (figure[i] as Line).Y2;
                        //coordinate.Content = now;
                    }
                }
            //Title = (figure[0] as Line).X1 + "  " + last;
        } //морфинг
        public void gs_in_canvas()
        {
            canvas.Children.Clear();
            canvas_SizeChanged(null, null);

            foreach (Shape obj in gs)
            {
                //Title = (obj as Line).X1 + " " + (obj as Line).X2;
                //Title = (obj as Line).Stroke + "";
                canvas.Children.Add(obj);
                bind_el_line(obj as Line);
            }
            //canvas.UpdateLayout();
        } //заполнение канвы из листа
        public void remove_mg_gr(Group_shape figuses)
        {
            foreach (Shape obj in figuses)
            {
                group.Remove(obj);
                gr_morf.Remove(obj);
            }
            elem_group.Content = "    Canvas: " + gs.Count() +
                       "    CountLC: " + list_canvas.Count +
                       "    GroupL: " + group.Count +
                       "    GroupM: " + gr_morf.Count;
        } //очистка гуппы линий и морфина после удаления листа
        public void bind_el_line(Line obj)
        {
            Ellipse e1 = new Ellipse() { Width = 8, Height = 8, Stroke = Brushes.Black, Fill = Brushes.Red };
            Ellipse e2 = new Ellipse() { Width = 8, Height = 8, Stroke = Brushes.Black, Fill = Brushes.Cyan };

            Binding x1 = new Binding(); x1.Mode = BindingMode.TwoWay; x1.Path = new PropertyPath(Line.X1Property); x1.Converter = new MyConverter(); x1.ConverterParameter = e1;
            Binding y1 = new Binding(); y1.Mode = BindingMode.TwoWay; y1.Path = new PropertyPath(Line.Y1Property); y1.Converter = new MyConverter(); y1.ConverterParameter = e2;
            Binding x2 = new Binding(); x2.Mode = BindingMode.TwoWay; x2.Path = new PropertyPath(Line.X2Property); x2.Converter = new MyConverter(); x2.ConverterParameter = e1;
            Binding y2 = new Binding(); y2.Mode = BindingMode.TwoWay; y2.Path = new PropertyPath(Line.Y2Property); y2.Converter = new MyConverter(); y2.ConverterParameter = e2;

            x1.Source = y1.Source = obj;
            x2.Source = y2.Source = obj;
            //line.ma

            e1.SetBinding(Canvas.LeftProperty, x1);
            e1.SetBinding(Canvas.TopProperty, y1);
            e2.SetBinding(Canvas.LeftProperty, x2);
            e2.SetBinding(Canvas.TopProperty, y2);

            e1.PreviewMouseDown += Line_PreviewMouseDown;
            e1.MouseMove += Line_MouseMove;
            e1.PreviewMouseUp += Line_PreviewMouseUp;
            e2.PreviewMouseDown += Line_PreviewMouseDown;
            e2.MouseMove += Line_MouseMove;
            e2.PreviewMouseUp += Line_PreviewMouseUp;

            canvas.Children.Add(e1);
            canvas.Children.Add(e2);
        }
        public void dell_el_in_canvas()
        {
            List<Ellipse> ellist = new List<Ellipse>();
            foreach (Shape obj in canvas.Children)
                if (obj is Ellipse)
                    ellist.Add(obj as Ellipse);
            foreach (Shape obj in ellist)
                if (obj is Ellipse)
                    canvas.Children.Remove(obj);
        }

        #endregion
        //методы управления ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        private void canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (canvas.Cursor == Cursors.Cross) //начало рисования нововй линии
            {
                draw_ok = true;
                p = e.GetPosition(canvas);
                line = new Line();
                line.Stroke = Brushes.Black;
                line.StrokeThickness = 3;
                line.X1 = line.X2 = p.X;
                line.Y1 = line.Y2 = p.Y;
                //Canvas.SetZIndex(line, 1);
                //gs.Add(line);
                //canvas.Children = gs;
                //gs_in_canvas();
                canvas.Children.Add(line);
                bind_el_line(line);

                //SizeChangedEventHandler act = (Object s, SizeChangedEventArgs args) =>
                //{
                //    BindingOperations.GetBindingExpressionBase(e1, Ellipse.WidthProperty).UpdateTarget();
                //    BindingOperations.GetBindingExpressionBase(e2, Ellipse.HeightProperty).UpdateTarget();
                //};
                //Ellipse el = new Ellipse();
                //el.Fill = Brushes.Red;
                //el.Width = el.Height = 7;
                //////el.RenderTransform = 
                ////el.RenderTransformOrigin
                //Binding bd = new Binding();
                //bd.Source = line;
                //bd.Path = new PropertyPath("RenderTransformOrigin");
                //bd.Mode = BindingMode.TwoWay;
                //el.SetBinding(Line.RenderTransformOriginProperty, bd);

                 //добавляем на канву
                //canvas.Children.Add(el);
            }
        } //нажате на канве
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (draw_ok && canvas.Cursor == Cursors.Cross && line != null) //рисуем (еще в движнии)
            {
                Point pointEnd = e.GetPosition(canvas);
                line.X2 = pointEnd.X;
                line.Y2 = pointEnd.Y;
            }

            data_xy.Content = "X: " + Convert.ToInt32(e.GetPosition(canvas).X) +
                            "  Y:" + Convert.ToInt32(e.GetPosition(canvas).Y);
            elem_group.Content = "    Canvas: " + gs.Count() +
                                "    CountLC: " + list_canvas.Count +
                                "    GroupL: " + group.Count +
                                "    GroupM: " + gr_morf.Count;

        } //мышь зажата на канве
        private void canvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (canvas.Cursor == Cursors.Cross) //нарисовали линию
            {
                draw_ok = false;
                //if (line.X1 == line.X2 && line.Y1 == line.Y2)
                //    gs.RemoveAt(gs.Count() - 1);
                //is_ok = false;
                line.Tag = 0; //присвоили тег бездействия

                line.MouseRightButtonDown += Line_MouseRightButtonDown; //подписались на события
                line.PreviewMouseDown += Line_PreviewMouseDown;
                line.MouseMove += Line_MouseMove;
                line.PreviewMouseUp += Line_PreviewMouseUp;
                gs.Add(line);
                line = null;

            }
        } //мышь отпущена на канве

        //методы для формирования линии ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        private void Line_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Title = canvas.Children.Count+"";
            if (!gs.morf_ok && !gs.select_ok && canvas.Cursor == Cursors.Arrow) //простой режим
            {
                line = sender as Line;
                //line.CommandBindings.RemoveAt(0);
                //line.CommandBindings.RemoveAt(1);
                dell_el_in_canvas();
                gs.Remove(line);
                canmove = false;
                gs_in_canvas();
                canvas.UpdateLayout(); //обнавление канвы
            }
            if (gs.select_ok) //режим выделения
            {
                line = sender as Line;
                line.Tag = 0; //присвоили тег бездействия

                line.Stroke = Brushes.Black;
                //canvas.UpdateLayout(); //обнавляем канву
                group.Remove(line); //удалили линию из группы выделеных линий
                //list_in_mas();
            }
            if (gs.morf_ok)
            {
                line = sender as Line;
                line.Stroke = Brushes.Black;
                gr_morf.Remove(line);
                list_in_mas();
            }
            //line = null;
        } //нажали ПКМ по линии
        private void Line_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

            if (canvas.Cursor == Cursors.Arrow)
            {
                p = e.GetPosition(canvas);
                if (sender is Line)
                {
                    line = sender as Line;
                    line.Tag = 0;
                    line.Stroke = Brushes.Red;
                    Mouse.Capture(line); //захват мыши линией
                }
                if (sender is Ellipse && !gs.morf_ok && !gs.select_ok)
                {
                    el = sender as Ellipse;

                    ////Canvas.SetLeft(el, Canvas.GetLeft(el)); //передвижения линии по канвасу 
                    ////Canvas.SetTop(el, Canvas.GetLeft(el));

                    Mouse.Capture(el); //захват мыши линией
                    el.SetValue(Canvas.LeftProperty, e.GetPosition(canvas).X + (el.Width / 2));
                    el.SetValue(Canvas.TopProperty, e.GetPosition(canvas).Y + (el.Width / 2));
                    //canvas.UpdateLayout();
                }
                //p = Mouse.GetPosition(line);
                canmove = true;

                //запомнили позицию нажатия

                //MessageBox.Show(select_ok + "");

                if (gs.select_ok && e.LeftButton == MouseButtonState.Pressed) //режим выделения
                {
                    line = sender as Line;
                    line.Tag = 1;  //присвоили тег выделения
                    line.Stroke = Brushes.Green;
                    canvas.UpdateLayout(); //обнавляем канву
                    if (!group.Contains(line))
                    {
                        group.Add(line); //добавили в коллекцию ыделяемых элементов
                    }
                    //MessageBox.Show( group.Count+ "");
                }
                if (gs.morf_ok && e.LeftButton == MouseButtonState.Pressed && sender is Line)
                {
                    line = sender as Line;
                    line.Stroke = Brushes.Violet;
                    if (gr_morf != null && !gr_morf.Contains(line))
                    {
                        gr_morf.Add(line); //добавили в коллекцию ыделяемых элементов
                    }
                }
            }
        } //нажатие по линии
        private void Line_MouseMove(object sender, MouseEventArgs e)
        {
            if (canmove && canvas.Cursor == Cursors.Arrow && !gs.select_ok && !gs.morf_ok)
            {
                Ellipse tmpel = new Ellipse();
                Line tmp = new Line();
                if (Mouse.Captured is Line)
                {
                    tmp = Mouse.Captured as Line; //присвоили временной линии линию захваченную мышкой
                    Canvas.SetLeft(tmp, e.GetPosition(canvas).X - p.X); //передвижения линии по канвасу 
                    Canvas.SetTop(tmp, e.GetPosition(canvas).Y - p.Y);
                }
                if (Mouse.Captured is Ellipse)
                {
                    tmpel = Mouse.Captured as Ellipse;
                    Canvas.SetLeft(tmpel, e.GetPosition(canvas).X + (el.Width / 2)/*- p.X + 100*/); //передвижения линии по канвасу 
                    Canvas.SetTop(tmpel, e.GetPosition(canvas).Y + (el.Width / 2)/* - p.Y +100*/);
                }
                canvas.UpdateLayout();
            }
            if (gs.select_ok && e.LeftButton == MouseButtonState.Pressed && Mouse.Captured != null && !gs.morf_ok && sender is Line) //режим выделения, и мышь зажата
            {
                foreach (Line obj in gs) //перебираем элементы на канве
                {
                    if (obj.Tag != null && (int)obj.Tag == 1) //если тег не пустой и в режиме выделения 
                    {
                        Canvas.SetLeft(obj as Line, e.GetPosition(canvas).X - p.X); //перемещаем выделеннные линии
                        Canvas.SetTop(obj as Line, e.GetPosition(canvas).Y - p.Y);
                        canvas.UpdateLayout();
                    }
                }
            }
        } //мышь зажата на линии
        private void Line_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!gs.morf_ok && !gs.select_ok && Mouse.Captured != null) //не режим выделеня и мышь не захвачена
            {
                if (Mouse.Captured is Line)
                {
                    line = Mouse.Captured as Line; //получаем захваченную линию
                    Canvas.SetLeft(line, 0); //передвижения линии по канвасу 
                    Canvas.SetTop(line, 0);

                    line.X1 = line.X1 + (e.GetPosition(canvas).X - p.X);
                    line.X2 = line.X2 + (e.GetPosition(canvas).X - p.X);
                    line.Y1 = line.Y1 + (e.GetPosition(canvas).Y - p.Y);
                    line.Y2 = line.Y2 + (e.GetPosition(canvas).Y - p.Y);
                    test_border(line.X1, line.Y1, line.X2, line.Y2);
                    line.Stroke = Brushes.Black;
                }
                //if (Mouse.Captured is Ellipse && !gs.morf_ok && !gs.select_ok)
                //{
                //    el = Mouse.Captured as Ellipse;
                //    Canvas.SetLeft(el, (e.GetPosition(canvas).X - p.X)); //передвижения линии по канвасу 
                //    Canvas.SetTop(el, (e.GetPosition(canvas).Y - p.Y));
                //}
                //canvas.UpdateLayout();
                Mouse.Capture(null); //освобождаем мышь
                canmove = false;
            }
            if (gs.select_ok && line != null && sender is Line)
            {

                for (int i = 0; i < gs.Count(); i++)
                {
                    if ((gs[i] as Line).Tag != null && (int)(gs[i] as Line).Tag == 1)
                    {
                        Line tmp = gs[i] as Line;
                        Canvas.SetLeft(tmp, 0); //передвижения линии по канвасу 
                        Canvas.SetTop(tmp, 0);
                        tmp.X1 = tmp.X1 + (e.GetPosition(canvas).X - p.X);
                        tmp.X2 = tmp.X2 + (e.GetPosition(canvas).X - p.X);
                        tmp.Y1 = tmp.Y1 + (e.GetPosition(canvas).Y - p.Y);
                        tmp.Y2 = tmp.Y2 + (e.GetPosition(canvas).Y - p.Y);
                        test_border(tmp.X1, tmp.Y1, tmp.X2, tmp.Y2);
                    }
                }
                Mouse.Capture(null);
                canvas.UpdateLayout(); //обнавляем канву
            }
            if (gs.morf_ok)
                Mouse.Capture(null);
            list_in_mas();
            line = null;
        } //мышь отпущена с линии 

        //методы взаимодействия с линией ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (tab_control.SelectedIndex == 0)
                if (panel_options.ActualWidth != 0)
                {
                    DoubleAnimation panel_anim = new DoubleAnimation();
                    panel_anim.From = panel_options.ActualWidth;
                    panel_anim.To = 0;
                    panel_anim.Duration = TimeSpan.FromSeconds(0.3);
                    panel_options.BeginAnimation(StackPanel.WidthProperty, panel_anim);
                    MatrixTransform mir = new MatrixTransform(1, 0, 0, -1, 0, arrow_img.Width);
                    arrow_img.RenderTransform = mir;
           
                }
                else
                {
                    DoubleAnimation panel_anim = new DoubleAnimation();
                    panel_anim.From = panel_options.ActualWidth;
                    panel_anim.To = 100;
                    panel_anim.Duration = TimeSpan.FromSeconds(0.3);
                    panel_options.BeginAnimation(StackPanel.WidthProperty, panel_anim);
                    MatrixTransform mir = new MatrixTransform(-1, 0, 0, 1, arrow_img.Width, 0);
                    arrow_img.RenderTransform = mir;
            
                }
            if (tab_control.SelectedIndex == 1)
                if (panel_options_fr.ActualWidth != 0)
                {
                    DoubleAnimation panel_anim = new DoubleAnimation();
                    panel_anim.From = panel_options_fr.ActualWidth;
                    panel_anim.To = 0;
                    panel_anim.Duration = TimeSpan.FromSeconds(0.3);
                    panel_options_fr.BeginAnimation(StackPanel.WidthProperty, panel_anim);
                    MatrixTransform mir = new MatrixTransform(1, 0, 0, -1, 0, arrow_img_fr.Width);
                
                    arrow_img_fr.RenderTransform = mir;
                }
                else
                {
                    DoubleAnimation panel_anim = new DoubleAnimation();
                    panel_anim.From = panel_options_fr.ActualWidth;
                    panel_anim.To = 200;
                    panel_anim.Duration = TimeSpan.FromSeconds(0.3);
                    panel_options_fr.BeginAnimation(StackPanel.WidthProperty, panel_anim);
                    MatrixTransform mir = new MatrixTransform(-1, 0, 0, 1, arrow_img.Width, 0);
                
                    arrow_img_fr.RenderTransform = mir;
                }
        } //анимация раскрытия боковой панели
        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var w = canvas.ActualWidth; //текущие высота и ширина канвы
            var h = canvas.ActualHeight;

            foreach (Line obj in grid_art)
                canvas.Children.Remove(obj); //удаляем старые линии сетки
            grid_art.Clear();
            //canvas.Children.Clear();
            for (int x = 0; x < w; x += 20) //добавляем новые линии сетки
                AddLineToBackground(x, 0, x, h);
            for (int y = 0; y < h; y += 20)
                AddLineToBackground(0, y, w, y);
            //test_all_line_border();
        } //изменение размеров канвы и отрисовка сетки
        private void scr_view_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            canvas.Width = scr_view.Width; //возврат ширины и высоты канвы
            canvas.Height = scr_view.Height;
        } //изменение размеров скрола и отрисовка сетки
        private void Slider_morf_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Morfing(e.NewValue); //передача параметров морфингу через слайдер
            gs.pos_slider = e.NewValue;
        } //изменение значений слайдера для морфинга
        private void go_transform_Click(object sender, RoutedEventArgs e)
        {
            double[,] tr = new double[3, 3];

            int n = 0;
            int m = 0;

            foreach (TextBox obj in DG.Children)
            {
                tr[n, m] = Convert.ToDouble(obj.Text);
                m++;
                if (m == 3)
                {
                    m = 0;
                    n++;
                }
            }
            Matrix3D fut = new Matrix3D(tr[0, 0], tr[0, 1], 0, tr[0, 2],
                                            tr[1, 0], tr[1, 1], 0, tr[1, 2],
                                            0, 0, 0, 0,
                                            tr[2, 0], tr[2, 1], 0, tr[2, 2]);
            for (int i = 0; i < group.Count; i++)
            {
                Matrix3D now_one = new Matrix3D((group[i] as Line).X1, (group[i] as Line).Y1, 0, 1,
                                   0, 0, 0, 0,
                                   0, 0, 0, 0,
                                   0, 0, 0, 0);
                Matrix3D now_two = new Matrix3D((group[i] as Line).X2, (group[i] as Line).Y2, 0, 1,
                                 0, 0, 0, 0,
                                 0, 0, 0, 0,
                                 0, 0, 0, 0);
                Matrix3D res_one = Matrix3D.Multiply(now_one, fut);
                Matrix3D res_two = Matrix3D.Multiply(now_two, fut);
                if (res_one.M14 != 0)
                {
                    (group[i] as Line).X1 = res_one.M11 / res_one.M14;
                    (group[i] as Line).Y1 = res_one.M12 / res_one.M14;
                    test_border((group[i] as Line).X1, (group[i] as Line).Y1, (group[i] as Line).X2, (group[i] as Line).Y2);
                }
                if (res_two.M14 != 0)
                {
                    (group[i] as Line).X2 = res_two.M11 / res_two.M14;
                    (group[i] as Line).Y2 = res_two.M12 / res_two.M14;
                    test_border((group[i] as Line).X1, (group[i] as Line).Y1, (group[i] as Line).X2, (group[i] as Line).Y2);
                }
            }
        } //матрица преобразований    

        private void count_list_canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gs = new Group_shape();
            list_canvas.Add(gs);
            count_list_canvas.Text = list_canvas.Count + "";
            gs = list_canvas[Convert.ToInt32(count_list_canvas.Text) - 1];
            gs_in_canvas();
            elem_group.Content = "    Canvas: " + gs.Count() +
                           "    CountLC: " + list_canvas.Count +
                           "    GroupL: " + group.Count +
                           "    GroupM: " + gr_morf.Count;
            Slider_morf.Value = 0;
        } //добавление листа
        private void count_list_canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (list_canvas.Count > 1)
            {
                remove_mg_gr(gs);
                list_canvas.RemoveAt(Convert.ToInt32(count_list_canvas.Text) - 1);
                if (Convert.ToInt32(count_list_canvas.Text) != 1)
                    count_list_canvas.Text = Convert.ToInt32(count_list_canvas.Text) - 1 + "";
                gs = list_canvas[Convert.ToInt32(count_list_canvas.Text) - 1];
                gs_in_canvas();
                elem_group.Content = "    Canvas: " + gs.Count() +
                              "    CountLC: " + list_canvas.Count +
                              "    GroupL: " + group.Count +
                              "    GroupM: " + gr_morf.Count;

                Slider_morf.Value = 0;
                //MessageBox.Show(gs.Count() + "");
            }
        } //удаление листа
        private void bt_backward_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(count_list_canvas.Text) > 1)
            {
                //canvas.Children.Clear();
                count_list_canvas.Text = Convert.ToInt32(count_list_canvas.Text) - 1 + "";
                //Title = Convert.ToInt32(count_list_canvas.Text) - 1 + "";
                gs = list_canvas[Convert.ToInt32(count_list_canvas.Text) - 1];
                gs_in_canvas();
                elem_group.Content = "    Canvas: " + gs.Count() +
                         "    CountLC: " + list_canvas.Count +
                         "    GroupL: " + group.Count +
                         "    GroupM: " + gr_morf.Count;
                Slider_morf.Value = gs.pos_slider;
            }
        } //перемещение на предыдущий лист
        private void bt_forward_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToInt32(count_list_canvas.Text) < list_canvas.Count)
            {
                //canvas.Children.Clear();
                count_list_canvas.Text = Convert.ToInt32(count_list_canvas.Text) + 1 + "";
                //Title = Convert.ToInt32(count_list_canvas.Text) - 1 + "";
                gs = list_canvas[Convert.ToInt32(count_list_canvas.Text) - 1];
                gs_in_canvas();
                elem_group.Content = "    Canvas: " + gs.Count() +
                         "    CountLC: " + list_canvas.Count +
                         "    GroupL: " + group.Count +
                         "    GroupM: " + gr_morf.Count;
                Slider_morf.Value = gs.pos_slider;
            }
        } //перемещение на следующий лист

        private void Save_file_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text files(*.ged)|*.ged|All files(*.*)|*.*";
            Nullable<bool> result = sf.ShowDialog();
            if (result == false)
                return;
            string fn = sf.FileName;

            //gs[0].Tag = 5555;
            Save_shape new_save = new Save_shape();
            int number_group = 0;
            foreach (var obj in list_canvas)
            {
                for (int i = 0; i < obj.Count(); i++)
                {
                    Data_for_serialize tmp = new Data_for_serialize();
                    tmp.X1 = (obj[i] as Line).X1;
                    tmp.X2 = (obj[i] as Line).X2;
                    tmp.Y1 = (obj[i] as Line).Y1;
                    tmp.Y2 = (obj[i] as Line).Y2;
                    tmp.Stroke = (obj[i] as Line).Stroke;
                    tmp.StrokeThickness = (obj[i] as Line).StrokeThickness;
                    tmp.Tag = (obj[i] as Line).Tag;
                    tmp.Type = (obj[i] as Line).GetType();
                    if (group.Contains((obj[i] as Line)))
                        tmp.Group = true;
                    if (gr_morf.Contains((obj[i] as Line)))
                    {
                        tmp.Morf = true;
                        tmp.Num_morf = gr_morf.IndexOf((obj[i] as Line));
                        tmp.Count_morf = gr_morf.Count();
                    }
                    tmp.Number_group = number_group;
                    new_save.Add(tmp);
                }
                number_group++;
            }
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            using (FileStream fs = File.Create(sf.FileName))
            {
                serializer.Serialize(fs, new_save);
            }
            sf.FileName = null;
        }
        private void Open_file_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Filter = "Text files(*.ged)|*.ged";
            Nullable<bool> result = of.ShowDialog();
            if (result == false)
                return;
            string fn = of.FileName;

            Save_shape open_obj = new Save_shape();
            NetDataContractSerializer serializer = new NetDataContractSerializer();
            using (FileStream fs = File.OpenRead(of.FileName))
            {
                open_obj = (Save_shape)serializer.Deserialize(fs);
            }
            int num_gr = 0;
            Group_shape gsh = new Group_shape();
            list_canvas.Clear();
            group.Clear();
            gr_morf.Clear();
            figure = null;
            Shape[] gr_morf_tmp = null;
            int cnt = 0;
            int real_cnt = 0;
            foreach (Data_for_serialize obj in open_obj)
            {
                Line tmp = new Line();
                tmp.X1 = obj.X1;
                tmp.X2 = obj.X2;
                tmp.Y1 = obj.Y1;
                tmp.Y2 = obj.Y2;
                tmp.Stroke = obj.Stroke;
                tmp.StrokeThickness = obj.StrokeThickness;
                tmp.Tag = obj.Tag;
                
                if (obj.Group)
                {
                    group.Add(tmp);
                    gsh.select_ok = true;
                }
                if (obj.Morf)
                {
                    if (cnt == 0)
                    {
                        gr_morf_tmp = new Shape[obj.Count_morf];
                        real_cnt = obj.Count_morf;
                    }
                    cnt = 1;
                    gr_morf_tmp[obj.Num_morf] = tmp;
                    //gr_morf.Add(tmp);
                }
                list_in_mas();

                

                if (obj.Number_group > num_gr )
                {
                    list_canvas.Add(gsh);
                    
                    gsh = new Group_shape();
                    num_gr++;
                }

                tmp.MouseRightButtonDown += Line_MouseRightButtonDown; //подписались на события
                tmp.PreviewMouseDown += Line_PreviewMouseDown;
                tmp.MouseMove += Line_MouseMove;
                tmp.PreviewMouseUp += Line_PreviewMouseUp;

                gsh.Add(tmp);                //canvas.Children.Add(tmp);
                if (num_gr == 0)
                    gs = gsh;
            }
            for (int i = 0; i < real_cnt; i++)
                gr_morf.Add(gr_morf_tmp[i]);
            list_in_mas();
            list_canvas.Add(gsh);
            gs_in_canvas();
            std_bt_Click(sender, e);
        }


        //фрактал VVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVVV


        Random rnd = new Random();
        DispatcherTimer tm = new DispatcherTimer();
        //Line l;
        public void create_fr(double ag, double proba, int num_iter, double x, double y, double thickness)
        {
            if (num_iter > 0)
            {
                byte R = 160;
                byte G = 82;
                byte B = 45;

                

                double _x = x + num_iter * (proba /*+ rnd.Next(-5, 5)*/) * Math.Sin(ag),
                        _y = y - num_iter * (proba /*+ rnd.Next(-5, 5)*/) * Math.Cos(ag);
                Line l = new Line();
                l.X1 = x;
                l.Y1 = y;
                l.X2 = _x;
                l.Y2 = _y;
                if (num_iter < Convert.ToInt32(Cnt_iter.Text) / 1.5)
                {

                    R = 34;
                    G = 139;
                    B = 34;
                    l.StrokeThickness = thickness - 0.8;
                }
                l.Stroke = new SolidColorBrush(Color.FromRgb(R, G, B));
                l.StrokeThickness = thickness - 0.8;

                x = _x;
                y = _y;            

                canvas_fr.Children.Add(l);

                //tm.Tick += Tm_Tick; ;
                //tm.Interval = new TimeSpan(0, 0, 0, 0, 500);
                //tm.Start();

                if (rnd_crt.IsChecked ?? true)
                {
                    if (rnd.Next(0, 2) == 0)
                    {
                        if (rnd.NextDouble() <= Convert.ToDouble(Rand.Text) / 100)
                            create_fr(ag + Math.PI * Convert.ToDouble(Rot.Text) / 180 + Math.PI * rnd.Next(-45, 45) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                        create_fr(ag - Math.PI * Convert.ToDouble(Rot.Text) / 180 - Math.PI * rnd.Next(-45, 45) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                    }
                    else
                    {
                        create_fr(ag + Math.PI * Convert.ToDouble(Rot.Text) / 180 + Math.PI * rnd.Next(-45, 45) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                        if (rnd.NextDouble() <= Convert.ToDouble(Rand.Text) / 100)
                            create_fr(ag - Math.PI * Convert.ToDouble(Rot.Text) / 180 - Math.PI * rnd.Next(-45, 45) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                    }

                  
                }
                else
                {
                    if (rnd.Next(0,2) == 0)
                    {
                        if (rnd.NextDouble() <= Convert.ToDouble(Rand.Text) / 100)
                            create_fr(ag + Math.PI * Convert.ToDouble(Rot.Text) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                        create_fr(ag - Math.PI * Convert.ToDouble(Rot.Text) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                    }
                    else
                    {
                        create_fr(ag + Math.PI * Convert.ToDouble(Rot.Text) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                        if (rnd.NextDouble() <= Convert.ToDouble(Rand.Text) / 100)
                            create_fr(ag - Math.PI * Convert.ToDouble(Rot.Text) / 180, proba, num_iter - 1, x, y, l.StrokeThickness);
                    }
                }

            }
        }
        //int tt = 5;
        //private void Tm_Tick(object sender, EventArgs e)
        //{

        //    //create_fr(tt/10-1, 5, 10 - 1, 500, 500, l.StrokeThickness);
        //    //create_fr(tt/10-1, 5, 10 - 1, 500, 500, l.StrokeThickness);

        //    //canvas_fr.Children.Add(l);
        //    //tm.Stop();
        //}

        private void canvas_fr_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Scl.Text == "" || Rot.Text == "" || Rand.Text == "" || Cnt_iter.Text == "")
            {
                MessageBox.Show("Введите необходимые данне!");
                return;
            }
            if ((Convert.ToDouble(Scl.Text) < 1 || (Convert.ToDouble(Scl.Text) > 15))
                || (Convert.ToDouble(Rot.Text) < 0 || (Convert.ToDouble(Rot.Text) > 360))
                || (Convert.ToDouble(Rand.Text) < 0 || (Convert.ToDouble(Rand.Text) > 100)))
            {
                MessageBox.Show("Введенные значения не попадают\r\nв допустимые диапазоны!");
                return;
            }
          
                create_fr(0, Convert.ToInt32(Scl.Text), Convert.ToInt32(Cnt_iter.Text), e.GetPosition(canvas_fr).X, e.GetPosition(canvas_fr).Y, 10);
        
        }
        private void canvas_fr_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            canvas_fr.Children.Clear();
        }
        private void bt_save_fr_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text files(*.png)|*.png|All files(*.*)|*.*";
            Nullable<bool> result = sf.ShowDialog();
            if (result == false)
                return;
            string fn = sf.FileName;
            canvas_fr.Width = scr_view_fr.ActualWidth;
            canvas_fr.Height = scr_view_fr.ActualHeight;

            var rtb = new RenderTargetBitmap((int)canvas_fr.Width, (int)canvas_fr.Height, 96d, 96d, PixelFormats.Pbgra32);
            canvas_fr.Measure(new Size((int)canvas_fr.Width, (int)canvas_fr.Height));
            canvas_fr.Arrange(new Rect(new Size((int)canvas_fr.Width, (int)canvas_fr.Height)));
            rtb.Render(canvas_fr);

            PngBitmapEncoder BufferSave = new PngBitmapEncoder();
            BufferSave.Frames.Add((BitmapFrame.Create(rtb)));
            using (var fs = File.OpenWrite(sf.FileName))
            {
                BufferSave.Save(fs);
            }
        }

        private void Scl_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (tab_control.SelectedIndex == 0)
            {
                Title = e.Text + " " + (sender as TextBox).Text.Length;
                if (e.Text == "-" && (sender as TextBox).Text.IndexOf("-") == -1)
                {
                   
                    return;
                }
                else 
                if (!(Char.IsDigit(e.Text, 0)
                    || (e.Text == ",")
                    && (!(sender as TextBox).Text.Contains(",")
                    && (sender as TextBox).Text.Length != 0)))
                {
                    e.Handled = true;
                    (sender as TextBox).Text = (sender as TextBox).Text.Trim('-');
                }
                else e.Handled = false;
            }
            if (tab_control.SelectedIndex == 1)
            {
                if (!(Char.IsDigit(e.Text, 0)
                    || (e.Text == ",")
                    && (!(sender as TextBox).Text.Contains(",")
                    && (sender as TextBox).Text.Length != 0)))
                {
                    e.Handled = true;
                }
                else e.Handled = false;
            }
        }
        private void Scl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

      
    }
}


