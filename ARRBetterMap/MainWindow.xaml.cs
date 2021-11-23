﻿using ARRSaveFormat;
using ARRSaveFormat.Types;
using GvasFormat;
using GvasFormat.Serialization;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ARRBetterMap
{
    public class ListboxVehiclesType
    {
        public string Name { get; set; }
        public Point Location { get; set; }
        public ListboxVehiclesType(string name, Point location)
        {
            Name = name;
            Location = location;
        }
    }

    public enum ShapeType
    {
        Rectangle,
        Ellipse
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Game Map
        //private const int MAP_HEIGHT = 194000;
        //private const int MAP_WIDTH = 203404;
        //private const int MAP_OFFSET_X = 0;
        //private const int MAP_OFFSET_Y = -4000;
        //private const double MAP_PIXALS = 850.0;

        // Terrain Map
        private const int MAP_HEIGHT = 201000;  // Lowering the number stretches the lines further out
        private const int MAP_WIDTH = 201000;
        private const int MAP_OFFSET_X = 0;
        private const int MAP_OFFSET_Y = -1000;
        private const double MAP_PIXALS = 2000.0;

        private const int LEN_SWITCH_MAIN = 1950;
        private const int LEN_SWITCH_SIDE = 1950;

        private const int SWITCH_SIDE_ROT = 6;
        private const int LEN_CROSS = 385;
        private const int TURNABLE_SIZE = 570;


        private double MinSize { get; set; }
        private bool Holding { get; set; }

        private double MapBgrSize { get; set; } = MAP_PIXALS;
        private double MapBgrScale { get; set; } = 1;
        private Point MapBgrStart { get; set; } = new Point(0, 0);

        private Point? LastPoint { get; set; }

        private ARRSaveFormat.Properties Properties { get; set; }

        private List<(Point, Shape, double, double, double)> Shapes { get; set; }

        private List<(Point, System.Windows.Shapes.Path, SplineSegment[], SplineType)> Splines { get; set; }

        private List<(Point, Shape, double, double, double)> RemovedVegetation { get; set; }

        private List<(Point, Shape)> Players { get; set; }

        public List<ListboxVehiclesType> ListboxVehicles { get; set; }

        private string SaveFile { get; set; }

        private FileSystemWatcher SaveFileWatcher { get; set; }

        public MainWindow()
        {
            string arrDir = string.Empty;
            string localAppData = string.Empty;
            try
            {
                localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            }
            catch (Exception e)
            {
                Trace.Assert(false, $"Getting localAppData failed with {e.Message}");
            }
            try
            {
                arrDir = System.IO.Path.Combine(localAppData, "arr\\saved\\savegames\\");
            }
            catch (Exception e)
            {
                Trace.Assert(false, $"Joining localAppData {localAppData} with {arrDir} failed with {e.Message}");
            }
            try
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.DefaultExt = ".sav";
                dlg.Filter = "RailroadsOnline save file (*.sav)|*.sav";
                if (Directory.Exists(arrDir))
                {
                    dlg.InitialDirectory = arrDir;
                }

                bool? result = dlg.ShowDialog();
                if (result != true)
                {
                    Close();
                    return;
                }

                SaveFile = dlg.FileName;

                InitializeComponent();

                SaveFileWatcher = RunWatcher();
            }
            catch (Exception e)
            {
                Trace.Assert(false, $"Opening dialog with path \"{arrDir}\" failed with {e.Message}");
            }
        }

        public FileSystemWatcher RunWatcher()
        {
            Init();

            FileSystemWatcher watcher = new FileSystemWatcher
            {
                Path = System.IO.Path.GetDirectoryName(SaveFile),

                Filter = "slot*.sav",

                NotifyFilter = NotifyFilters.LastWrite
            };

            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;

            return watcher;
        }

        private void Init()
        {
            Shapes = new List<(Point, Shape, double, double, double)>();
            RemovedVegetation = new List<(Point, Shape, double, double, double)>();
            ListboxVehicles = new List<ListboxVehiclesType>();
            Splines = new List<(Point, System.Windows.Shapes.Path, SplineSegment[], SplineType)>();
            Players = new List<(Point, Shape)>();

            Gvas save;
            using (FileStream stream = File.Open(SaveFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                save = UESerializer.Read(stream);
            }
            Properties = new ARRSaveFormat.Properties(save);

            Industry[] firewoods = Properties.Industries.Where(x => x.Type == IndustryType.industry_firewooddepot).ToArray();
            Dispatcher.Invoke(() =>
            {
                Canvas.Children.RemoveRange(1, Canvas.Children.Count - 1);
                //InitStaticShapes(Properties.Industries.Except(firewoods).ToArray(), ShapeType.Rectangle, Colors.Gray, 2, 0, 10);
                InitSplines();
                InitSwitches();
                InitStaticShapes(Properties.Watertowers, ShapeType.Ellipse, Colors.Blue, 1, 1.5, 1.5);
                InitStaticShapes(Properties.Sandhouses, ShapeType.Rectangle, Colors.Yellow, 2, 0.5, 2);
                InitStaticShapes(firewoods, ShapeType.Rectangle, Colors.Brown, 2, 0.5, 2);
                InitStaticShapes(Properties.Turntables, ShapeType.Ellipse, Colors.Red, 1, 0, 3, true);
                InitVegetation(Properties.RemovedVegetationAssets, Colors.Green, 1, 0, 1);
                InitVehicles();
                InitPlayers(Properties.Players);
                InfoRow.Content = $"Succesfully loaded {Properties.Vehicles?.Length ?? 0} vehicles, {Properties.Watertowers?.Length ?? 0} watertowers, {Properties.Sandhouses?.Length ?? 0} sandhouses, {Properties.Industries?.Length ?? 0} industries, {Properties.Turntables?.Length ?? 0} turntables, {Properties.Switches?.Length ?? 0} switches and {Properties.Splines?.Length ?? 0} splines from save {SaveFile} - {Properties.SaveGameDate}!";

                DrawMap();
            });
        }

        private void InitVehicles()
        {
            for (int i = 0; i < Properties.Vehicles.Length; i++)
            {
                Vehicle vehicle = Properties.Vehicles[i];

                Ellipse ellipse = new Ellipse();
                ellipse.Stroke = new SolidColorBrush(Colors.Black);
                ellipse.Fill = new SolidColorBrush(Colors.Black);
                ellipse.Width = 9 + MapBgrScale * 1;
                ellipse.Height = 9 + MapBgrScale * 1;

                string printableName = $"[{i}] {vehicle.Type} - {vehicle.Name}:{vehicle.Number}";
                ToolTip tt = new ToolTip();
                tt.Content = printableName;
                ellipse.ToolTip = tt;

                Point location = new Point(vehicle.Location.X, vehicle.Location.Y);
                Shapes.Add((location, ellipse, 1, 1, 1));

                ListboxVehicles.Add(new ListboxVehiclesType(printableName, location));

                Canvas.Children.Add(ellipse);
            }

            VehiclesList.ItemsSource = ListboxVehicles;
        }

        private void InitPlayers(Player[] players)
        {
            if (players == null)
                return;

            for (int i = 0; i < players.Length; i++)
            {
                Player player = players[i];

                Polygon shape = new Polygon();

                Type colorsType = typeof(Colors);
                PropertyInfo[] properties = colorsType.GetProperties();
                Color color = (Color)properties[i * 10].GetValue(null, null);
                shape.Stroke = new SolidColorBrush(color);
                shape.Fill = new SolidColorBrush(color);
                shape.Width = 20;
                shape.Height = 20;
                TransformGroup tg = new TransformGroup();
                tg.Children.Add(new RotateTransform(player.Rotation, shape.Width / 2.0, shape.Height / 2.0));
                //tg.Children.Add(new ScaleTransform(MapBgrScale / 3, MapBgrScale / 3, shape.Width / 2.0, shape.Height / 2.0));
                shape.LayoutTransform = tg;
                PointCollection pc = new PointCollection(new List<Point> { new Point(-10, 10), new Point(-10, -10), new Point(10, -10) });
                shape.Points = pc;

                string printableName = $"[{i}] {player.Name} | ${player.Money} | XP: {player.XP} | X/Y/Z: {player.Location.X} / {player.Location.Y} / {player.Location.Z}";
                ToolTip tt = new ToolTip();
                tt.Content = printableName;
                shape.ToolTip = tt;

                Point location = new Point(player.Location.X, player.Location.Y);
                Players.Add((location, shape));

                // CRASH
                // ListboxVehicles.Add(new ListboxVehiclesType(printableName, location));

                Canvas.Children.Add(shape);
            }
        }

        private void InitStaticShapes(StaticObject[] objects, ShapeType shapeType, Color color, double ratio, double baseScale, double scaledScale, bool notCentered = false)
        {
            if (objects == null)
                return;

            for (int i = 0; i < objects.Length; i++)
            {
                StaticObject staticObject = objects[i];

                Shape shape;
                if (shapeType == ShapeType.Rectangle)
                {
                    shape = new Rectangle();
                }
                else if (shapeType == ShapeType.Ellipse)
                {
                    shape = new Ellipse();
                }
                else
                {
                    throw new Exception("Unsupported shape type!");
                }

                shape.Stroke = new SolidColorBrush(color);
                shape.Fill = new SolidColorBrush(color);
                shape.Width = 9 * baseScale + MapBgrScale * scaledScale;
                shape.Height = 9 * baseScale * ratio + MapBgrScale * scaledScale * ratio;
                if (!notCentered)
                {
                    shape.LayoutTransform = new RotateTransform(staticObject.Rotation.Y, shape.Width / 2.0, shape.Height / 2.0);
                }

                float xOffset = 0;
                float yOffset = 0;
                if (notCentered)
                {
                    xOffset = -(float)Math.Sin(staticObject.Rotation.Y / 180 * Math.PI) * TURNABLE_SIZE;
                    yOffset = (float)Math.Cos(staticObject.Rotation.Y / 180 * Math.PI) * TURNABLE_SIZE;
                }

                Point location = new Point(staticObject.Location.X + xOffset, staticObject.Location.Y + yOffset);
                Shapes.Add((location, shape, ratio, baseScale, scaledScale));

                Canvas.Children.Add(shape);
            }
        }

        private void InitVegetation(Location[] locations, Color color, double ratio, double baseScale, double scaledScale)
        {
            if (locations == null)
                return;

            for (int i = 0; i < locations.Length; i++)
            {
                Location loc = locations[i];

                Shape shape = new Ellipse();
                shape.Stroke = new SolidColorBrush(color);
                shape.Fill = new SolidColorBrush(color);
                shape.Width = 9 * baseScale + MapBgrScale * scaledScale;
                shape.Height = 9 * baseScale * ratio + MapBgrScale * scaledScale * ratio;

                Point location = new Point(loc.X, loc.Y);
                RemovedVegetation.Add((location, shape, ratio, baseScale, scaledScale));

                Canvas.Children.Add(shape);
            }
        }

        private void InitSplines()
        {
            if (Properties.Splines == null)
            {
                return;
            }

            Properties.Splines = Properties.Splines.OrderBy(x => x.Type == SplineType.RailNG || x.Type == SplineType.TrestleDeck).ThenByDescending(x => x.Type).ThenBy(x => x.Location.Z).ToArray();
            for (int i = 0; i < Properties.Splines.Length; i++)
            {
                Spline spline = Properties.Splines[i];

                System.Windows.Shapes.Path curve = new System.Windows.Shapes.Path();

                //Color color = spline.Type == SplineType.Grade || spline.Type == SplineType.GradeConstant ? Colors.Green :
                //    spline.Type == SplineType.StonewallVariable || spline.Type == SplineType.StonewallConstant ? Colors.LightGray :
                //    spline.Type == SplineType.Trestle ? Colors.Yellow :
                //    spline.Type == SplineType.SplineTrestleSteel ? Colors.Blue : Colors.Red;

                Color color;
                switch (spline.Type)
                {
                    // Grades
                    case SplineType.Grade:
                    case SplineType.GradeConstant:
                        color = Colors.LightGray;
                        break;

                    // Stone Walls
                    case SplineType.StonewallVariable:
                    case SplineType.StonewallConstant:
                        color = Colors.Gray;
                        break;

                    // Wood Trestle
                    case SplineType.Trestle:
                        color = Colors.Orange;
                        break;

                    // Steel Trestle
                    case SplineType.SplineTrestleSteel:
                        color = Colors.LightBlue;
                        break;

                    // Trestle Rail
                    case SplineType.TrestleDeck:
                        color = Colors.DarkGray;
                        break;

                    // Normal Rail
                    case SplineType.RailNG:
                        color = Colors.Black;
                        break;

                    default:
                        color = Colors.Red;
                        break;
                }

                curve.Stroke = new SolidColorBrush(color);

                Splines.Add((new Point(spline.Location.X, spline.Location.Y), curve, spline.Segments, spline.Type));

                Canvas.Children.Add(curve);
            }
        }

        private void InitSwitches()
        {
            if (Properties.Switches == null)
                return;

            for (int i = 0; i < Properties.Switches.Length; i++)
            {
                ARRSaveFormat.Types.Switch @switch = Properties.Switches[i];

                System.Windows.Shapes.Path main = new System.Windows.Shapes.Path();
                System.Windows.Shapes.Path side = new System.Windows.Shapes.Path();

                switch (@switch.Type)
                {
                case SwitchType.SwitchCross90:
                    {
                        main.Stroke = new SolidColorBrush(Colors.Blue);
                        side.Stroke = new SolidColorBrush(Colors.Blue);

                        float xOffsetMain = (float)Math.Cos((@switch.Rotation.Y) / 180 * Math.PI) * LEN_CROSS;
                        float yOffsetMain = (float)Math.Sin((@switch.Rotation.Y) / 180 * Math.PI) * LEN_CROSS;
                        Location mainEndLocation = new Location(@switch.Location.X + xOffsetMain, @switch.Location.Y + yOffsetMain, 0);
                        SplineSegment mainSegment = new SplineSegment(@switch.Location, mainEndLocation, true);

                        float xOffsetSide = -(float)Math.Cos((@switch.Rotation.Y - 90) / 180 * Math.PI) * LEN_CROSS;
                        float yOffsetSide = -(float)Math.Sin((@switch.Rotation.Y - 90) / 180 * Math.PI) * LEN_CROSS;
                        Location sideStartLocation = new Location(@switch.Location.X - xOffsetSide / 2 + xOffsetMain / 2, @switch.Location.Y - yOffsetSide / 2 + yOffsetMain / 2, 0);
                        Location sideEndLocation = new Location(@switch.Location.X + xOffsetSide / 2 + xOffsetMain / 2, @switch.Location.Y + yOffsetSide / 2 + yOffsetMain / 2, 0);
                        SplineSegment sideSegment = new SplineSegment(sideStartLocation, sideEndLocation, true);

                        Splines.Add((new Point(@switch.Location.X, @switch.Location.Y), main, new SplineSegment[] { mainSegment }, SplineType.RailNG));
                        Splines.Add((new Point(sideStartLocation.X, sideStartLocation.Y), side, new SplineSegment[] { sideSegment }, SplineType.RailNG));

                        Canvas.Children.Add(main);
                        Canvas.Children.Add(side);
                    }
                    break;

                case SwitchType.SwitchRight:
                case SwitchType.SwitchRightMirror:
                case SwitchType.SwitchLeft:
                case SwitchType.SwitchLeftMirror:
                    {
                        Color colorMain = Colors.Purple;
                        Color colorSide = Colors.Pink;

                        switch (@switch.Type)
                        {
                            case SwitchType.SwitchRight:
                            case SwitchType.SwitchRightMirror:
                                colorMain = Colors.Black;
                                colorSide = Colors.Red;
                                break;

                            case SwitchType.SwitchLeft:
                            case SwitchType.SwitchLeftMirror:
                                colorMain = Colors.Red;
                                colorSide = Colors.Black;
                                break;
                        }

                        if (@switch.State != SwitchState.Main)
                        {
                            Color colorTemp = colorMain;
                            colorMain = colorSide;
                            colorSide = colorTemp;
                        }

                        main.Stroke = new SolidColorBrush(colorMain);
                        side.Stroke = new SolidColorBrush(colorSide);

                        float xOffsetMain = -(float)Math.Cos((@switch.Rotation.Y - 90) / 180 * Math.PI) * LEN_SWITCH_MAIN;
                        float yOffsetMain = -(float)Math.Sin((@switch.Rotation.Y - 90) / 180 * Math.PI) * LEN_SWITCH_MAIN;
                        Location mainEndLocation = new Location(@switch.Location.X + xOffsetMain, @switch.Location.Y + yOffsetMain, 0);
                        SplineSegment mainSegment = new SplineSegment(@switch.Location, mainEndLocation, true);

                        int rotationOffset = @switch.Type == SwitchType.SwitchLeft || @switch.Type == SwitchType.SwitchLeftMirror ? -SWITCH_SIDE_ROT : SWITCH_SIDE_ROT;
                        float xOffsetSide = -(float)Math.Cos((@switch.Rotation.Y - 90 + rotationOffset) / 180 * Math.PI) * LEN_SWITCH_SIDE;
                        float yOffsetSide = -(float)Math.Sin((@switch.Rotation.Y - 90 + rotationOffset) / 180 * Math.PI) * LEN_SWITCH_SIDE;
                        Location sideEndLocation = new Location(@switch.Location.X + xOffsetSide, @switch.Location.Y + yOffsetSide, 0);
                        SplineSegment sideSegment = new SplineSegment(@switch.Location, sideEndLocation, true);

                        Splines.Add((new Point(@switch.Location.X, @switch.Location.Y), main, new SplineSegment[] { mainSegment }, SplineType.RailNG));
                        Splines.Add((new Point(@switch.Location.X, @switch.Location.Y), side, new SplineSegment[] { sideSegment }, SplineType.RailNG));

                        Canvas.Children.Add(main);
                        Canvas.Children.Add(side);
                    }
                    break;

                case SwitchType.SwitchY:
                case SwitchType.SwitchYMirror:
                    {
                        // TODO
                    }
                    break;
                }
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            System.Threading.Thread.Sleep(50);

            SaveFile = e.FullPath;
            Init();
        }

        private void DrawMap()
        {
            Players.ForEach(player =>
            {
                Point translatedPoint = TranslatePoint(player.Item1);
                /*player.Item2.Width = 20 * MapBgrScale;
                player.Item2.Height = 20 * MapBgrScale;
                /*ScaleTransform st = (ScaleTransform)((TransformGroup)player.Item2.LayoutTransform).Children[1];
                st.CenterX = player.Item2.Width / 2;
                st.CenterY = player.Item2.Height / 2;
                st.ScaleX = MapBgrScale / 3;
                st.ScaleY = MapBgrScale / 3;*/
                Canvas.SetLeft(player.Item2, translatedPoint.X - player.Item2.Width / 2);
                Canvas.SetTop(player.Item2, translatedPoint.Y - player.Item2.Height / 2);
            });

            Shapes.ForEach(rect =>
            {
                Point translatedPoint = TranslatePoint(rect.Item1);
                rect.Item2.Width = 9 * rect.Item4 + MapBgrScale * rect.Item5;
                rect.Item2.Height = 9 * rect.Item4 * rect.Item3 + MapBgrScale * rect.Item5 * rect.Item3;
                Canvas.SetLeft(rect.Item2, translatedPoint.X - rect.Item2.Width / 2);
                Canvas.SetTop(rect.Item2, translatedPoint.Y - rect.Item2.Height / 2);
            });

            Splines.ForEach(spline =>
            {
                PathSegment[] pathSegments = new PathSegment[spline.Item3.Length];
                for (int i = 0; i < spline.Item3.Length; i++)
                {
                    SplineSegment splineSegment = spline.Item3[i];
                    pathSegments[i] = new QuadraticBezierSegment(TranslatePoint(splineSegment.LocationStart.X, splineSegment.LocationStart.Y), TranslatePoint(splineSegment.LocationEnd.X, splineSegment.LocationEnd.Y), splineSegment.Visible || (displayInvisible.IsChecked ?? false));
                    //pathSegments[i] = new QuadraticBezierSegment(TranslatePoint(splineSegment.LocationStart.X, splineSegment.LocationStart.Y), TranslatePoint(splineSegment.LocationEnd.X, splineSegment.LocationEnd.Y), splineSegment.Visible);
                }
                PathFigureCollection pfc = new PathFigureCollection();
                pfc.Add(new PathFigure(TranslatePoint(spline.Item1.X, spline.Item1.Y), pathSegments, false));
                float mult = spline.Item4 == SplineType.RailNG || spline.Item4 == SplineType.TrestleDeck ? 0.3f : 1;
                spline.Item2.StrokeThickness = MapBgrScale * mult + 5 / MapBgrScale * mult;
                spline.Item2.Data = new PathGeometry(pfc);
            });

            //if (displayRemovedVegetation?.IsChecked ?? false)
            {
                RemovedVegetation.ForEach(rect =>
                {
                    Point translatedPoint = TranslatePoint(rect.Item1);
                    rect.Item2.Width = 9 * rect.Item4 + MapBgrScale * rect.Item5;
                    rect.Item2.Height = 9 * rect.Item4 * rect.Item3 + MapBgrScale * rect.Item5 * rect.Item3;
                    Canvas.SetLeft(rect.Item2, translatedPoint.X - rect.Item2.Width / 2);
                    Canvas.SetTop(rect.Item2, translatedPoint.Y - rect.Item2.Height / 2);
                });
            }
        }

        private void ZoomBackground(double scrollDelta, Point center)
        {
            double size = MapBgrSize;

            double delta = scrollDelta / 1000.0 * size;
            size += delta;

            ResizeBackground(size, center);
        }

        private void ScaleBackground(double scale, Point center)
        {
            // mapBgr.png dimensions
            ResizeBackground(scale * MAP_PIXALS, center);
        }

        private void ResizeBackground(double size, Point center)
        {
            double oldSize = MapBgrSize;
            if (size < MinSize)
            {
                size = MinSize;
            }

            MapBgr.Height = size;
            MapBgr.Width = size;

            MapBgrSize = size;
            MapBgrScale = size / MAP_PIXALS;

            double deltaX = -(center.X / MapBgr.ActualWidth) * (size - oldSize);
            double deltaY = -(center.Y / MapBgr.ActualHeight) * (size - oldSize);

            TranslateBackground(deltaX, deltaY);

            MapBgr.UpdateLayout();
        }

        private void ShowLocation(Point targetLocation)
        {
            ScaleBackground(10.0, new Point(0, 0));
            Point pointInImage = TranslatePoint(targetLocation);
            double xShift = 0.4 * ActualWidth - pointInImage.X;
            double yShift = 0.4 * ActualHeight - pointInImage.Y;
            TranslateBackground(xShift, yShift);
        }

        private void TranslateBackground(Point currPoint)
        {
            double xDelta = 0;
            double yDelta = 0;

            if (LastPoint != null)
            {
                xDelta = currPoint.X - ((Point)LastPoint).X;
                yDelta = currPoint.Y - ((Point)LastPoint).Y;
            }

            TranslateBackground(xDelta, yDelta);

            LastPoint = currPoint;
        }

        private void TranslateBackground(double xDelta, double yDelta)
        {
            double newX = MapBgrStart.X + xDelta;
            if (newX + MapBgrSize < 0.8 * ActualWidth)
            {
                newX = 0.8 * ActualWidth - MapBgrSize;
            }

            if (newX > 0)
            {
                newX = 0;
            }

            double newY = MapBgrStart.Y + yDelta;
            if (newY + MapBgrSize < ActualHeight)
            {
                newY = ActualHeight - MapBgrSize;
            }

            if (newY > 0)
            {
                newY = 0;
            }

            MapBgr.Margin = new Thickness(newX, newY, 0, 0);
            MapBgrStart = new Point(newX, newY);

            DrawMap();
        }

        private Point TranslatePoint(double x, double y)
        {
            Point imageRelativePoint = new Point((-x + MAP_WIDTH + MAP_OFFSET_X) / (2 * MAP_WIDTH), (-y + MAP_HEIGHT + MAP_OFFSET_Y) / (2 * MAP_HEIGHT));
            Point windowRelativePoint = new Point(imageRelativePoint.X * MapBgrSize + MapBgrStart.X, imageRelativePoint.Y * MapBgrSize + MapBgrStart.Y);

            return windowRelativePoint;
        }

        private Point TranslatePoint(Point point)
        {
            return TranslatePoint(point.X, point.Y);
        }

        private void MapBgr_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ZoomBackground(e.Delta, e.GetPosition(MapBgr));
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MinSize = Math.Max(0.8 * ActualWidth, ActualHeight);

            if (MapBgr.Height < MinSize || MapBgr.Width < MinSize)
            {
                MapBgr.Height = MinSize;
                MapBgr.Width = MinSize;
                MapBgrSize = MinSize;
            }

            TranslateBackground(new Point());
        }

        private void MapBgr_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Holding = true;
        }

        private void MapBgr_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Holding = false;
            LastPoint = null;
        }

        private void MapBgr_MouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(this);

            Holding &= e.LeftButton == MouseButtonState.Pressed;
            if (!Holding)
                LastPoint = null;

            if (position != LastPoint && Holding)
            {
                TranslateBackground(position);
            }
        }

        private void VehiclesList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowLocation(((ListboxVehiclesType)VehiclesList.SelectedItem).Location);
        }

        private void displayInvisible_Checked(object sender, RoutedEventArgs e)
        {
            DrawMap();
        }

        private void displayInvisible_Unchecked(object sender, RoutedEventArgs e)
        {
            DrawMap();
        }
    }
}
