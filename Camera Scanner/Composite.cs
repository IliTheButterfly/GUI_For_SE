using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Camera_Scanner
{
    

    public class Program
    {




        public static void Main()
        {
            var mainMenu = new UIMenu("Main Menu") { Parent = Component.MainComponent };
                var mainTitle = new UITitle("LIDAR - MENU PRINCIPAL"); mainMenu.Add(mainTitle);
                var manageScans = new UIMenu("Gérer les scans"); mainMenu.Add(manageScans);
                    var manageScansTitle = new UITitle("LIDAR - GÉRER SCANS"); manageScans.Add(manageScansTitle);
                    var addScan = new UIMenu("Ajouter un scan"); manageScans.Add(addScan);
                        var addScanTitle = new UITitle("LIDAR - AJOUTER UN SCAN"); addScan.Add(addScanTitle);
                        var addExposition = new UIMenu("Ajouter une exposition"); addScan.Add(addExposition);
                            var addExpTitle = new UITitle("LIDAR - AJOUTER EXPOSITION"); addExposition.Add(addExpTitle);
                            var addExpTime = new UIValueChangerInt("Temps", 1); addExposition.Add(addExpTime);
                        var addStream = new UIMenu("Ajouter une raffale"); addScan.Add(addStream);
                        var addStrmTitle = new UITitle("LIDAR - AJOUTER RAFFALE"); addStream.Add(addStrmTitle);
                    var modifyScan = new UIMenu("Modifier un scan"); manageScans.Add(modifyScan);
                        var modifScanTitle = new UITitle("LIDAR - MODIFIER SCAN"); modifyScan.Add(modifScanTitle);


            Component.Current = manageScans;

            
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

        public abstract class UINotSelectable : UIItem
        {
            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 3, 0 }, 1); } }

            protected UINotSelectable(string name) : base(name) { }

            public override DisplayLine GetText()
            {
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
            protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties(new List<int> { 2, 1, 0, 1, 5, 1 }, 2); } }

            public UIValueChangerIntChild Child;
            public volatile int Value;

            public UIValueChangerInt(string name, int val) : base(name)
            {
                Value = val;
            }

            protected override void _supplementText()
            {
                Display.Texts[4] = new DisplayText(Value.ToString(), 5);
            }

            public override void Select()
            {
                if (Child == null) Child = new UIValueChangerIntChild(Name, this);
                Current = Child;
            }
        }

        public class UIValueChangerIntChild : UISelectable
        {
            private int _lastValue;

            public UIValueChangerInt MyParent;

            public UIValueChangerIntChild(string name, UIValueChangerInt myParent) : base(name)
            {
                _lastValue = myParent.Value;
                MyParent = myParent;
            }

            protected override void _supplementText()
            {
                if (IsHighlighted)
                {
                    MyParent.Display.Texts[3] = new DisplayText("<", 1);
                    MyParent.Display.Texts[5] = new DisplayText(">", 1);
                }
                else
                {
                    MyParent.Display.Texts[3] = new DisplayText(" ", 1);
                    MyParent.Display.Texts[5] = new DisplayText(" ", 1);
                }
            }

            public override void Select()
            {
                Current = MyParent;
            }

            public override void Back()
            {
                MyParent.Value = _lastValue;
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

    }
}



