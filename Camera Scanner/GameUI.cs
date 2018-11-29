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

    public class GameUI
{
    public static Sandbox.ModAPI.IMyGridProgram Prog;
    public List<IMyTextPanel> GetLCDs()
    {
        List<IMyTextPanel> output = new List<IMyTextPanel>();
        List<IMyTerminalBlock> allBlocks = new List<IMyTerminalBlock>();

        Prog.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(allBlocks);
        for (int i = 0; i < allBlocks.Count; i++)
        {
            output.Add((IMyTextPanel)allBlocks[i]);
        }
        
        return output;
    }

    //0.5 = 53.5 / 35.5
    //1 = 26.5 / 17.75
    //2 = 13.25 / 9
    //3 = 9 / 6
    //4 = 6.75 / 4.5
    //y = a/(x-h) + k

    
        void Test()
        {
            
        }

        
        
}

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
