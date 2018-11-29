using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRageMath;
using VRage.Game;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Ingame;
//using Sandbox.ModAPI;
using Sandbox.Game.EntityComponents;
using VRage.Game.Components;
using VRage.Collections;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game.ModAPI.Ingame;
using SpaceEngineers.Game.ModAPI.Ingame;



namespace Camera_Scanner
{




    public class Program
    {
        public static Sandbox.ModAPI.IMyGridProgram Prog;

        public static TimeUnit MinRefresh = (TimeUnit)10;

        static void Main(string[] args)
        {
            var mainMenu = new UIMenu("Main Menu") { Parent = Component.MainComponent };
            {
                var mainTitle = new UITitle("LIDAR - MENU PRINCIPAL"); mainMenu.Add(mainTitle);
                var manageScans = new UIMenu("Gérer les scans"); mainMenu.Add(manageScans);
                {
                    var manageScansTitle = new UITitle("LIDAR - GÉRER SCANS"); manageScans.Add(manageScansTitle);
                    var addScan = new UIMenu("Ajouter un scan"); manageScans.Add(addScan);
                    {
                        var addScanTitle = new UITitle("LIDAR - AJOUTER UN SCAN"); addScan.Add(addScanTitle);
                        var addExposition = new UIMenu("Ajouter une exposition"); addScan.Add(addExposition);
                        {
                            var addExpTitle = new UITitle("LIDAR - AJOUTER EXPOSITION"); addExposition.Add(addExpTitle);
                            var addExpTime = new UIValueChangerInt("Temps", 1); addExposition.Add(addExpTime);
                        }
                        var addStream = new UIMenu("Ajouter une raffale"); addScan.Add(addStream);
                        {
                            var addStrmTitle = new UITitle("LIDAR - AJOUTER RAFFALE"); addStream.Add(addStrmTitle);
                        }
                    }
                    var modifyScan = new UIMenu("Modifier un scan"); manageScans.Add(modifyScan);
                    {
                        var modifScanTitle = new UITitle("LIDAR - MODIFIER SCAN"); modifyScan.Add(modifScanTitle);
                    }
                }
                var detected = new UIMenu("Détectés"); mainMenu.Add(detected);
                {
                    var detectedTitle = new UITitle("LIDAR - OBJETS DÉTECTÉS"); detected.Add(detectedTitle);
                }
                var autre = new UIMenu("AutreMenu"); mainMenu.Add(autre);
                {
                    var autreTitle = new UITitle("AUTRE TITRE"); autre.Add(autreTitle);
                }
                var text = new UINotSelectable("Hello"); mainMenu.Add(text);
            }





            Component.Current = (UISelectable)mainMenu.Items[1];


            while (true)
            {
                //Console.WriteLine(Component.Current.Name);
                Component.Current.Show();
                ConsoleKeyInfo key = Console.ReadKey();
                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        Component.Current.Up();
                        break;
                    case ConsoleKey.DownArrow:
                        Component.Current.Down();
                        break;
                    case ConsoleKey.LeftArrow:
                        Component.Current.Back();
                        break;
                    case ConsoleKey.RightArrow:
                        Component.Current.Select();
                        break;
                    case ConsoleKey.Enter:
                        return;
                    default:
                        break;
                }
                Console.Clear();
            }


        }

        public class GridDraw
        {
            public List<Vector2> Coords = new List<Vector2>();
            public int RangeX { get; set; }
            public int RangeY { get; set; }

            public GridDraw(int rangeX = 2, int rangeY = 2)
            {
                RangeX = rangeX;
                RangeY = rangeY;
            }


            private Vector2 _getMiddle()
            {
                return new Vector2(RangeX / 2, RangeY / 2);
            }

            public void SetSize(int size)
            {
                RangeX = size;
                RangeY = size;
            }

            public void Add(Vector2 coord)
            {
                Coords.Add(new Vector2(coord.X + _getMiddle().X, coord.Y + _getMiddle().Y));
            }

            public void Draw()
            {
                Console.Clear();
                for (int i = 0; i < RangeX; i++)
                {
                    for (int ii = 0; ii < RangeY; ii++)
                    {
                        bool selected = false;
                        if (Coords.Count > 0)
                        {
                            foreach (Vector2 p in Coords)
                            {
                                if ((p.X == i) && (p.Y == ii))
                                {
                                    selected = true;
                                }
                            }
                        }

                        if (selected)
                        {
                            Console.Write("X");
                        }
                        else
                        {
                            Console.Write("O");
                        }

                    }
                    Console.Write("\n");
                }
            }
        }

        public class Camera
        {
            public GridDraw Grid { get; set; }
            public IMyCameraBlock Cam { get; set; }

            public Camera(GridDraw grid, IMyCameraBlock cam)
            {
                Grid = grid;
                Cam = cam;
            }



            public void ScanAt(Vector2 coord, double range = 0)
            {

                Grid.Add(coord);

                Cam.Raycast(range, coord.X, coord.Y);
            }

        }



        public class ScanPattern
        {
            public Vector2 Coord { get; set; }
            public int Index { get; set; }
            public List<Camera> Cameras { get; set; }
            public int CurrentDirection { get; set; }
            public int Direction()
            {
                if (CurrentDirection == 4) CurrentDirection = 0;

                return (CurrentDirection++);
            }

            public ScanPattern(List<Camera> cameras)
            {
                Cameras = cameras;
            }



            private void _move()
            {

                switch (CurrentDirection)
                {
                    case 1:
                        Coord = new Vector2(Coord.X, Coord.Y + 1);
                        break;
                    case 2:
                        Coord = new Vector2(Coord.X + 1, Coord.Y);
                        break;
                    case 3:
                        Coord = new Vector2(Coord.X, Coord.Y - 1);
                        break;
                    case 4:
                        Coord = new Vector2(Coord.X - 1, Coord.Y);
                        break;
                    default:
                        break;
                }
            }

            private void _scan()
            {
                //Console.ReadKey();
                //Cameras[Index].Grid.Draw();
                if (Index < Cameras.Count) Cameras[Index].ScanAt(Coord);
                Index++;
            }

            private void _side(int sideLength)
            {
                for (int i = 0; i < sideLength; i++)
                {
                    _move();
                    _scan();
                }
            }

            public void Scan(int sideLength)
            {
                Coord = new Vector2(0, 0);
                Cameras[0].ScanAt(Coord);
                CurrentDirection = 0;
                Index = 0;
                int length = 0;
                for (int i = 0; i < sideLength; i++)
                {

                    Direction();
                    _side(i);

                    Direction();
                    _side(i);

                    length = i;
                }
                Direction();
                _side(length);
            }
        }


        public class CamExposure
        {
            public int Captures;
            public DistanceUnit Range;
            public DistanceUnit Size;
            public AngleUnit Definedfov;
            public CamGroupUnit MaxCams;
            public AngleUnit FOV;

            public TimeUnit Time
            {
                get { return (TimeUnit)Math.Floor((double)((Range / MinRefresh * (DistanceUnit)1).Ticks)); }
            }

            public AngleUnit MaxFOV
            {
                get { return new AngleUnit { Radians = (new AngleOperations { Size = Size, Range = Range }.Angle) * Math.Sqrt(Math.Floor((double)(MaxCams.NCams * Captures))) }; }
            }

            public CamGroupUnit NCams
            {
                get { return new CamGroupUnit { NGroups = 1, CamsSide = (int)(FOV / (new AngleOperations { Size = Size, Range = Range }.Angle)) }; }
            }
        }



        public class CamGroup
        {
            private List<Camera> _cams;

            public void Add(Camera cam)
            {
                _cams.Add(cam);
            }

            public void Clear()
            {
                _cams.Clear();
            }

            public List<Camera> Get()
            {
                return _cams;
            }
        }

        public struct AngleOperations
        {
            private AngleUnit _angle;
            private DistanceUnit _size;
            private DistanceUnit _range;

            public DistanceUnit Size
            {
                get { return _size = _range * Math.Sin((double)(_angle.Radians)); }
                set { _size = value; }
            }

            public DistanceUnit Range
            {
                get { return _range = (_size * Math.Sin((double)(_angle.Radians))); }
                set { _range = value; }
            }

            public AngleUnit Angle
            {
                get { return _angle = (AngleUnit)Math.Asin((double)(_size / _range)); }
                set { _angle = value; }
            }
        }

        public struct CamGroupUnit
        {
            private int _nCams;
            private int _nGroups;

            public int NCamsPerGroup
            {
                get { return _nCams / _nGroups; }
                set { _nCams = value * _nGroups; }
            }

            public int CamsSide
            {
                get { return (int)(Math.Ceiling(Math.Sqrt(NCamsPerGroup))); }
                set { NCamsPerGroup = (int)(Math.Pow(value, 2)); }
            }

            public int NCams
            {
                get { return _nCams; }
                set { _nCams = value; }
            }

            public int NGroups
            {
                get { return _nGroups; }
                set { _nGroups = value; }
            }

            public static implicit operator CamGroupUnit(int nCams)
            {
                return new CamGroupUnit
                {
                    _nCams = nCams,
                    _nGroups = 1
                };
            }
        }





        public struct AngleUnit
        {
            private float _radians;

            public object Degrees
            {
                get { return float.Parse((Convert.ToDouble(_radians) * 180 / Math.PI).ToString()); }
                set { _radians = float.Parse((Convert.ToDouble(value) * Math.PI / 180).ToString()); }
            }

            public object Radians
            {
                get { return _radians; }
                set { _radians = float.Parse(value.ToString()); }
            }

            public static explicit operator AngleUnit(float angle)
            {
                return new AngleUnit()
                {
                    Degrees = angle
                };
            }
            public static explicit operator AngleUnit(int angle)
            {
                return new AngleUnit()
                {
                    Degrees = float.Parse(angle.ToString())
                };
            }
            public static explicit operator AngleUnit(double angle)
            {
                return new AngleUnit()
                {
                    Degrees = float.Parse(angle.ToString())
                };
            }
            public static float operator /(AngleUnit angle1, AngleUnit angle2)
            {
                return float.Parse(angle1.Radians.ToString()) / float.Parse(angle2.Radians.ToString());
            }
            //public static AngleUnit operator *(AngleUnit angle1, AngleUnit angle2)
            //{
            //    return new AngleUnit()
            //    {
            //        _radians = float.Parse(angle1.Radians.ToString()) * float.Parse(angle2.Radians.ToString())
            //    };
            //}
            public static AngleUnit operator *(AngleUnit angle1, int val)
            {
                return new AngleUnit()
                {
                    _radians = float.Parse(angle1.Radians.ToString()) * float.Parse(val.ToString())
                };
            }
            public static AngleUnit operator *(AngleUnit angle1, double val)
            {
                return new AngleUnit()
                {
                    _radians = float.Parse(angle1.Radians.ToString()) * float.Parse(val.ToString())
                };
            }
            public static AngleUnit operator *(AngleUnit angle1, float val)
            {
                return new AngleUnit()
                {
                    _radians = float.Parse(angle1.Radians.ToString()) * float.Parse(val.ToString())
                };
            }
            public static AngleUnit operator -(AngleUnit angle1, AngleUnit angle2)
            {
                return new AngleUnit()
                {
                    _radians = float.Parse(angle1.Radians.ToString()) - float.Parse(angle2.Radians.ToString())
                };
            }
            public static AngleUnit operator +(AngleUnit angle1, AngleUnit angle2)
            {
                return new AngleUnit()
                {
                    _radians = float.Parse(angle1.Radians.ToString()) + float.Parse(angle2.Radians.ToString())
                };
            }
        }

        public struct TimeUnit
        {
            private float _miliseconds;

            public object Miliseconds
            {
                get { return float.Parse(_miliseconds.ToString()); }
                set { _miliseconds = float.Parse(value.ToString()); }
            }

            public object Seconds
            {
                get { return float.Parse(_miliseconds.ToString()) / 1000; }
                set { _miliseconds = float.Parse(value.ToString()) * 1000; }
            }

            public object Ticks
            {
                get { return float.Parse(_miliseconds.ToString()) / 16; }
                set { _miliseconds = float.Parse(value.ToString()) * 16; }
            }

            public object Ticks10
            {
                get { return float.Parse(Ticks.ToString()) / 10; }
                set { Ticks = float.Parse(value.ToString()) * 10; }
            }


            public static explicit operator TimeUnit(float time)
            {
                return new TimeUnit()
                {
                    Ticks = time
                };
            }
            public static explicit operator TimeUnit(int time)
            {
                return new TimeUnit()
                {
                    Ticks = time
                };
            }
            public static explicit operator TimeUnit(double time)
            {
                return new TimeUnit()
                {
                    Ticks = time
                };
            }
            /// <summary>
            /// Converts a distance to its equivalent time for a camera to scan at that distance
            /// </summary>
            /// <param name="distance"></param>
            public static implicit operator TimeUnit(DistanceUnit distance)
            {
                return new TimeUnit()
                {
                    _miliseconds = float.Parse(distance.Meters.ToString()) * 2
                };
            }
            public static TimeUnit operator /(TimeUnit time1, TimeUnit time2)
            {
                return new TimeUnit()
                {
                    _miliseconds = float.Parse(time1.Miliseconds.ToString()) / float.Parse(time2.Miliseconds.ToString())
                };
            }
            public static TimeUnit operator *(TimeUnit time1, TimeUnit time2)
            {
                return new TimeUnit()
                {
                    _miliseconds = float.Parse(time1.Miliseconds.ToString()) * float.Parse(time2.Miliseconds.ToString())
                };
            }
            public static TimeUnit operator +(TimeUnit time1, TimeUnit time2)
            {
                return new TimeUnit()
                {
                    _miliseconds = float.Parse(time1.Miliseconds.ToString()) + float.Parse(time2.Miliseconds.ToString())
                };
            }
            public static TimeUnit operator -(TimeUnit time1, TimeUnit time2)
            {
                return new TimeUnit()
                {
                    _miliseconds = float.Parse(time1.Miliseconds.ToString()) - float.Parse(time2.Miliseconds.ToString())
                };
            }
        }

        public struct DistanceUnit
        {
            private float _meters;

            public object Meters
            {
                get { return _meters; }
                set { _meters = float.Parse(value.ToString()); }
            }

            public object Kilometers
            {
                get { return _meters / 1000; }
                set { _meters = float.Parse(value.ToString()) * 1000; }
            }

            public object CamTicks
            {
                get { return float.Parse(_meters.ToString()) / 32; }
                set { _meters = float.Parse(value.ToString()) * 32; }
            }

            public object CamTicks10
            {
                get { return float.Parse(CamTicks.ToString()) / 10; }
                set { CamTicks10 = float.Parse(value.ToString()) * 10; }
            }

            public static explicit operator DistanceUnit(float distance)
            {
                return new DistanceUnit()
                {
                    _meters = distance
                };
            }
            public static explicit operator DistanceUnit(int distance)
            {
                return new DistanceUnit()
                {
                    _meters = float.Parse(distance.ToString())
                };
            }
            public static explicit operator DistanceUnit(double distance)
            {
                return new DistanceUnit()
                {
                    _meters = float.Parse(distance.ToString())
                };
            }
            public static float operator /(DistanceUnit distance1, DistanceUnit distance2)
            {
                return float.Parse(distance1.Meters.ToString()) / float.Parse(distance2.Meters.ToString());
            }
            //public static DistanceUnit operator *(DistanceUnit distance1, DistanceUnit distance2)
            //{
            //    return new DistanceUnit()
            //    {
            //        _meters = float.Parse(distance1.Meters.ToString()) * float.Parse(distance2.Meters.ToString())
            //    };
            //}
            public static DistanceUnit operator *(DistanceUnit distance1, int val)
            {
                return new DistanceUnit()
                {
                    _meters = float.Parse(distance1.Meters.ToString()) * float.Parse(val.ToString())
                };
            }
            public static DistanceUnit operator *(DistanceUnit distance1, double val)
            {
                return new DistanceUnit()
                {
                    _meters = float.Parse(distance1.Meters.ToString()) * float.Parse(val.ToString())
                };
            }
            public static DistanceUnit operator *(DistanceUnit distance1, float val)
            {
                return new DistanceUnit()
                {
                    _meters = float.Parse(distance1.Meters.ToString()) * float.Parse(val.ToString())
                };
            }
            public static DistanceUnit operator +(DistanceUnit distance1, DistanceUnit distance2)
            {
                return new DistanceUnit()
                {
                    _meters = float.Parse(distance1.Meters.ToString()) + float.Parse(distance2.Meters.ToString())
                };
            }
            public static DistanceUnit operator -(DistanceUnit distance1, DistanceUnit distance2)
            {
                return new DistanceUnit()
                {
                    _meters = float.Parse(distance1.Meters.ToString()) - float.Parse(distance2.Meters.ToString())
                };
            }
            public static AngleOperations operator &(DistanceUnit size, DistanceUnit range)
            {
                return new AngleOperations()
                {
                    Size = size,
                    Range = range
                };
            }
        }

        public class Component
        {
            public string Name;
            public UIMenu Parent;
            public DisplayLine Display;

            public static UIMenu MainComponent = new UIMenu(String.Empty);

            public static UISelectable Current = UISelectable.Default;
            public static Dictionary<string, Component> Components = new Dictionary<string, Component>();

            protected Component(string name)
            {
                Name = name;
            }
        }

        public abstract class UIItem : Component
        {
            protected abstract DisplayCanvasProperties _prop { get; }

            protected UIItem(string name) : base(name)
            {
                var texts = new List<DisplayText>();
                for (int i = 0; i < _prop.Sizes.Count; i++)
                {
                    texts.Add(new DisplayText());
                }
                Display = new DisplayLine
                {
                    Canvas = _prop,
                    Texts = texts
                };
                Display.Texts[_prop.DynamicIndex] = new DisplayText(name, 0);
            }
            public abstract DisplayLine GetText();

        }

        public abstract class UISelectable : UIItem
        {
            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 2, 1, 0 }, 2); } }

            public bool IsHighlighted { get { return this == Current; } }

            public static UISelectable Default = new UIMenu("Default") { Parent = MainComponent };


            protected UISelectable(string name) : base(name) { }


            protected virtual void _supplementText()
            {
                Display.Texts[1] = new DisplayText(" ", 1);
            }

            public override DisplayLine GetText()
            {
                if (IsHighlighted)
                {

                    Display.Texts[0] = new DisplayText("->", 2);
                }
                else
                {
                    Display.Texts[0] = new DisplayText("~ ", 2);
                }
                _supplementText();
                return Display;
            }

            public virtual void Highlight()
            {
                Current = this;
            }

            public virtual void Select()
            {

            }

            public virtual void Back()
            {
                Current = Parent.Parent == MainComponent ? Current : Parent;
            }

            public virtual void Up()
            {
                var selectable = Parent.GetSelectable();
                for (int i = 0; i < selectable.Count; i++)
                {
                    if (selectable[i].IsHighlighted)
                    {
                        if (i != 0)
                        {
                            selectable[i - 1].Highlight();
                            return;
                        }
                    }
                }
            }

            public virtual void Down()
            {
                var selectable = Parent.GetSelectable();
                for (int i = 0; i < selectable.Count; i++)
                {
                    if (selectable[i].IsHighlighted)
                    {
                        if (i + 1 != selectable.Count)
                        {
                            selectable[i + 1].Highlight();
                            return;
                        }
                    }
                }
            }

            public virtual void Show()
            {
                //Console.WriteLine("Parent will show");
                if (Parent != MainComponent)
                {
                    Parent.Print();
                }
            }
        }

        public class UINotSelectable : UIItem
        {
            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 3, 0 }, 1); } }

            public UINotSelectable(string name) : base(name) { }

            

            public override DisplayLine GetText()
            {
                Display.Texts[0] = new DisplayText("", 3);
                return Display;
            }
        }


        public interface IDisplay
        {
            void Print(IEnumerable<UIItem> items);
        }

        public class ConsoleDisplay : IDisplay
        {
            public ComplexDisplay MyComplexDisplay = new ComplexDisplay();

            public void Print(IEnumerable<UIItem> items)
            {
                MyComplexDisplay.Lines.Clear();
                foreach (var item in items)
                {
                    MyComplexDisplay.Add(item.GetText());
                }
                Console.WriteLine(MyComplexDisplay.ToString());
            }
        }

        public class UIMenu : UISelectable
        {
            public List<UIItem> Items = new List<UIItem>();
            public IDisplay _display;

            public UIMenu(string name, IDisplay display = null) : base(name)
            {
                _display = display ?? new ConsoleDisplay();
            }

            public void Add(UIItem item)
            {
                item.Parent = this;
                Items.Add(item);
            }

            public List<UISelectable> GetSelectable()
            {
                var output = new List<UISelectable>();
                foreach (var item in Items)
                {
                    if (item is UISelectable)
                    {
                        output.Add((UISelectable)item);
                    }
                }
                if (output.Count == 0)
                {
                    var empty = new UIEmpty(string.Empty);
                    empty.Parent = this;
                    output.Add(empty);
                }
                return output;
            }

            public override void Select()
            {
                Current = GetSelectable()[0];
            }

            public void Print()
            {
                _display.Print(Items);
            }
        }


        public class UICommand : UISelectable
        {
            private IUICommand _command;

            public UICommand(string name, IUICommand command) : base(name)
            {
                _command = command;
            }



            public override void Select()
            {
                _command.Execute();
                Console.ReadKey();
            }


        }

        public class UIToggleable : UISelectable
        {
            private bool _state;
            private string _textTrue;
            private string _textFalse;

            public UIToggleable(string name, string textTrue, string textFalse, bool state = false) : base(name)
            {
                _textTrue = textTrue;
                _textFalse = textFalse;
                _state = state;
            }

            public override void Select()
            {
                _state = !_state;
            }


        }



        public class UIValueChangerInt : UISelectable
        {
            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 2, 1, 0, 1, 5, 1, 2 }, 2); } }

            public UIValueChangerIntChild Child;
            public int Value;
            public DisplayText Unit = new DisplayText(string.Empty, 2);

            public UIValueChangerInt(string name, int val) : base(name)
            {
                Value = val;
            }
            public UIValueChangerInt(string name, int val, DisplayText unit) : this(name, val)
            {
                Unit = unit;
            }

            protected override void _supplementText()
            {
                Display.Texts[6] = Unit;
                Display.Texts[1] = new DisplayText(" ", 1);
                Child = Child ?? new UIValueChangerIntChild(string.Empty, this);
                Display.Texts[4] = new DisplayText(Value.ToString(), 5);
                if (Child.IsHighlighted)
                {
                    Display.Texts[3] = new DisplayText("<", 1);
                    Display.Texts[5] = new DisplayText(">", 1);
                }
                else
                {
                    Display.Texts[3] = new DisplayText(" ", 1);
                    Display.Texts[5] = new DisplayText(" ", 1);
                }
            }

            public override void Select()
            {
                Child = Child ?? new UIValueChangerIntChild(string.Empty, this);
                Child.LastValue = Value;
                Current = Child;
            }
        }

        public class UIValueChangerIntChild : UISelectable
        {
            public int LastValue;

            public UIValueChangerInt MyParent;

            public UIValueChangerIntChild(string name, UIValueChangerInt myParent) : base(name)
            {
                MyParent = myParent;
                Display = MyParent.Display;
            }

            public override void Select()
            {
                Current = MyParent;
            }

            public override void Back()
            {
                MyParent.Value = LastValue;
                Current = MyParent;
            }

            public override void Up()
            {
                MyParent.Value++;
            }

            public override void Down()
            {
                MyParent.Value--;
            }

            public override void Show()
            {
                MyParent.Show();
            }
        }

        public class UIValueShower<T> : UINotSelectable
        {
            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 0 }, 0); } }
            public DisplayText Unit = new DisplayText(string.Empty, 2);
            public T Value;

            public UIValueShower(string name) : base(name)
            {
            }


        }

        public class UIEmpty : UISelectable
        {
            public UIEmpty(string name) : base(name) { }

            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 0 }, 0); } }

            public override DisplayLine GetText()
            {

                Display.Texts[0] = new DisplayText(string.Empty, 0);
                return Display;
            }
        }

        public class UITitle : UINotSelectable
        {
            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 0 }, 0); } }
            public UITitle(string name) : base(name)
            {
            }

            public override DisplayLine GetText()
            {
                return Display;
            }
        }

        public interface IUICommand
        {
            void Execute();
        }

        public class SayHelloCommand : IUICommand
        {
            public void Execute()
            {
                Console.WriteLine("Hello");
            }
        }





        /*
        public override void Select()
        {

        }

        public override void Back()
        {

        }

        public override void Up()
        {

        }

        public override void Down()
        {

        }
        */

        public struct DisplayText
        {
            private string _outputText;
            private int _length;
            public int Length
            {
                get { return Math.Max(_length, Text.Length); }
                set { _length = value; Length = value; }
            }

            public string Text;
            public DisplayText(string text, int length)
            {
                Text = text;
                _outputText = String.Empty;
                _length = length;
            }

            public void ToLeft()
            {
                var sb = new StringBuilder();
                sb.Append(Text);
                sb.Append(' ', Length - Text.Length);
                _outputText = sb.ToString();
            }

            public void ToCenter()
            {
                var sb = new StringBuilder();
                sb.Append(' ', (Length - Text.Length) / 2);
                sb.Append(Text);
                sb.Append(' ', Length - sb.Length);
                _outputText = sb.ToString();
            }

            public void ToRight()
            {
                var sb = new StringBuilder();
                sb.Append(' ', Length - Text.Length);
                sb.Append(Text);
                _outputText = sb.ToString();
            }

            public override string ToString()
            {
                if (_outputText == String.Empty) ToLeft();
                return _outputText;
            }
        }

        public class DisplayUnit
        {
            private float _size;

            public int Length
            {
                get { return (int)(35 / _size); }
                set { _size = (value - 1) * 35; }
            }

            public int Lines
            {
                get { return (int)(18 / _size); }
                set { _size = (value * 18); }
            }

            public static implicit operator DisplayUnit(float size)
            {
                return new DisplayUnit
                {
                    _size = size
                };
            }
        }

        public struct ValueGroup<T1, T2>
        {
            public T1 Val1;
            public T2 Val2;

            public ValueGroup(T1 val1, T2 val2)
            {
                Val1 = val1;
                Val2 = val2;
            }


        }



        public struct DisplayCanvasProperties
        {
            public List<int> Sizes;
            public int DynamicIndex;

            public DisplayCanvasProperties(List<int> sizes, int dynamicIndex = 2)
            {
                Sizes = sizes;
                DynamicIndex = dynamicIndex;
            }

            public void Add(int size, bool dynamic = false)
            {
                Sizes = Sizes ?? new List<int>();
                Sizes.Add(size);
                if (dynamic) { DynamicIndex = Sizes.Count - 1; }
            }
        }

        public class DisplayCanvas
        {
            public List<int> Sizes = new List<int>();
            public int DynamicIndex;



            public void SetSize(int index, int size)
            {
                Sizes[index] = size;
            }

            public int DynamicSize
            {
                get { return Sizes[DynamicIndex]; }
                set { Sizes[DynamicIndex] = value; }
            }

            public static implicit operator DisplayCanvas(DisplayCanvasProperties prop)
            {
                return new DisplayCanvas
                {
                    Sizes = prop.Sizes,
                    DynamicIndex = prop.DynamicIndex
                };
            }
        }



        public class DisplayLine
        {
            public DisplayCanvas Canvas;
            public List<DisplayText> Texts = new List<DisplayText>();

            public int TotalSize
            {
                get
                {
                    int output = 0;


                    for (int i = 0; i < Texts.Count; i++)
                    {
                        if (i == Canvas.DynamicIndex)
                        {
                            Canvas.DynamicSize = Math.Max(Texts[i].Length, Canvas.DynamicSize);
                        }
                        output += Canvas.Sizes[i];
                    }

                    return output;
                }
                set
                {
                    int val = value - TotalSize;
                    Canvas.DynamicSize += val;
                    int index = Canvas.DynamicIndex;
                    Texts[index] = new DisplayText(Texts[index].Text, Texts[index].Length + val);
                }
            }

            public override string ToString()
            {
                var output = new StringBuilder();
                foreach (var section in Texts)
                {
                    output.Append(section.ToString());
                }
                //Console.WriteLine(output.ToString());
                return output.ToString();
            }

            public void Add(DisplayText text)
            {
                Texts.Add(text);
            }


            public static implicit operator DisplayLine(DisplayCanvas canvas)
            {
                List<DisplayText> texts = new List<DisplayText>();
                for (int i = 0; i < canvas.Sizes.Count; i++)
                {
                    texts.Add(new DisplayText(String.Empty, canvas.Sizes[i]));
                }
                return new DisplayLine
                {
                    Canvas = canvas,
                    Texts = texts
                };
            }
        }

        public class ComplexDisplay
        {
            public List<DisplayLine> Lines = new List<DisplayLine>();

            public void Add(DisplayLine line)
            {
                Lines.Add(line);
                int max = 0;
                foreach (var linec in Lines)
                {
                    max = Math.Max(linec.TotalSize, max);
                }
                foreach (var linec in Lines)
                {
                    linec.TotalSize = max;
                }
            }

            public override string ToString()
            {
                var output = new StringBuilder();
                foreach (var line in Lines)
                {
                    //Console.WriteLine(line.ToString());
                    output.AppendLine(line.ToString() + "|");
                }
                return output.ToString();
            }
        }

    }


    
}
