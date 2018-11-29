using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;



    public class Program
    {
        

        public static void Main()
        {
            var mainMenu = new UIMenu("Main Menu") { Parent = Component.MainComponent };
            var menu1 = new UIMenu("Menu 1") { Parent = mainMenu };
			var command1_1 = new UICommand("Command 1", new SayHelloCommand()) { Parent = menu1 };
			var menu2 = new UIMenu("Menu 2") { Parent = mainMenu };
			var chexkbox2_1 = new UIToggleable("Checkbox 1", "O", "X") { Parent = menu2 };
			
			Component.Current = command1_1;
			Component.Current.Show();
			Component.Current.Show();

			
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
				Display.Texts[_prop.DynamicIndex] = new DisplayText(name, 0);//err
            }
			public abstract DisplayLine GetText();

        }

        public abstract class UISelectable : UIItem
        {
			protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties( new List<int>{2,1,0}, 2); } }
			
            public bool IsHighlighted { get { return this == Current; } }

            public static UISelectable Default = new UIMenu("Default") { Parent = MainComponent };
			
			
            protected UISelectable(string name) : base(name) { }
			
			
			protected virtual void _supplementText(){}
			
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
                Current = Parent == MainComponent ? Current : Parent;
            }

            public virtual void Up()
            {
                for (int i = 0; i < Parent.GetSelectable().Count; i++)
                {
                    if (Parent.GetSelectable()[i].IsHighlighted)
                    {
                        if (i != 0) Parent.GetSelectable()[i - 1].Highlight();
                    }
                }
            }

            public virtual void Down()
            {
                for (int i = 0; i < Parent.GetSelectable().Count; i++)
                {
                    if (Parent.GetSelectable()[i].IsHighlighted)
                    {
                        if (i + 1 != Parent.GetSelectable().Count - 1) (Parent).GetSelectable()[i + 1].Highlight();
                    }
                }
            }

            public virtual void Show()
            {
				Console.WriteLine("Parent will show");
                if (Parent != MainComponent)
                {
                    Parent.Show();

                }
            }
        }

        public abstract class UINotSelectable : UIItem
        {
			protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties( new List<int>{3,0}, 1); } }
			
            protected UINotSelectable(string name) : base(name) { }
			
			public override DisplayLine GetText()
			{
				return Display;
			}
        }
		
		
		

        public class UIMenu : UISelectable
        {
            public List<Component> Items = new List<Component>();
            public ComplexDisplay MyComplexDisplay = new ComplexDisplay();
			

            public UIMenu(string name) : base(name) { }

            public void Add(Component item)
            {
                item.Parent = this;
                Items.Add(item);
                MyComplexDisplay.Add(item.Display);
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
                return output;
            }

            public override void Show()
            {
                if (MyComplexDisplay.Lines.Count == 0) 
				{
					foreach (var item in Items)
					{
						MyComplexDisplay.Add(item.Display);
					}
				}
				Console.WriteLine(MyComplexDisplay.ToString());
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

        public class UIValueChanger<ValueT> : UISelectable
        {
			protected override DisplayCanvasProperties _prop { get { return new DisplayCanvasProperties( new List<int>{2,1,0,1,5,1}, 2); } }
			
            public UIValueChangerChild<ValueT> Child;
            public ValueT Value;

            public UIValueChanger(string name, ValueT val) : base(name)
            {
                Value = val;
                Child = new UIValueChangerChild<ValueT>(name, val)
                {
                    MyParent = this
                };
            }
			
			protected override void _supplementText()
			{
				Display.Texts[4] = new DisplayText(Value.ToString(), 5);
			}

            public override void Select()
            {
                Current = Child;
            }
        }

        public class UIValueChangerChild<ValueT> : UIValueChanger<ValueT>
        {
            private ValueT _lastValue;

            public UIValueChanger<ValueT> MyParent;

            public UIValueChangerChild(string name, ValueT val) : base(name, val)
            {
                _lastValue = Value;
            }
			
			public override DisplayLine GetText()
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
				return MyParent.GetText();
			}

            public override void Select()
            {
                Current = Parent;
            }

            public override void Back()
            {
                Value = _lastValue;
                Current = Parent;
            }

            public override void Show()
            {
                MyParent.Show();
            }
        }

        public class UITitle : UINotSelectable
        {
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


public struct DisplayText
    {
        
        private string _outputText;
        private int _length;
        public int Length { get { return Math.Max(_length, Text.Length); } }

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
            sb.Append(' ', (Length - Text.Length)/2);
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

    public struct ValueGroup<T1,T2>
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

                for (int ii = 0; ii < 2; ii++)
                {
                    for (int i = 0; i < Texts.Count; i++)
                    {
                        if (i == Canvas.DynamicIndex)
                        {
                            Canvas.DynamicSize = Math.Max(Texts[i].Length, Canvas.DynamicSize);
                        }
                        output += Canvas.Sizes[i];
                    }
                }
                return output;
            }
            set
            {
                Canvas.DynamicSize = value;
            }
        }

        public override string ToString()
        {
            var output = new StringBuilder();
            foreach (var section in Texts)
            {
                output.Append(section.ToString());
            }
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
				Console.WriteLine(line.ToString());
                output.AppendLine(line.ToString());
            }
            return output.ToString();
        }
    }




