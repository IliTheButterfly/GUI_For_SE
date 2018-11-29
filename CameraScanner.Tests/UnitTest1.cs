using System;
using Camera_Scanner;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using FluentAssertions.Collections;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace CameraScanner.Tests
{
    [TestClass]
    public class UnitTest1 : Program
    {
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    var distance = new DistanceUnit();
        //    distance.Meters = 10000;

        //    distance.Kilometers.Should().Be(10);
        //}

        //[TestMethod]
        //public void TestExposure()
        //{
        //    CamExposure camEposure = new CamExposure(5, 640, 20, (AngleUnit)20);
        //    camEposure.Time.Should().Be((TimeUnit)10);
        //}
        [TestClass]
        public class TestTotalSize
        {
            [TestMethod]
            public void Test_TotalSize_and_Length()
            {
                DisplayLine line1 = new DisplayLine
                {
                    Canvas = new DisplayCanvas
                    {
                        Sizes = new List<int> { 0 },
                        DynamicIndex = 0
                    },
                    Texts = new List<DisplayText>
                {
                    new DisplayText("test", 4)
                }
                };//Total size = 4

                line1.TotalSize.Should().Be(line1.ToString().Length);
            }

            [TestClass]
            public class Test_complex_display_dynamic_dimensions_with_components
            {
                [TestMethod]
                public void Test_title1_base_size()
                {
                    UITitle title1 = new UITitle("title 1");

                    title1.Display.TotalSize.Should().Be(title1.GetText().ToString().Length);
                }

                [TestMethod]
                public void Test_menu1_base_size()
                {
                    UIMenu menu1 = new UIMenu("menu 1");

                    menu1.Display.TotalSize.Should().Be(menu1.GetText().ToString().Length);
                }

                [TestMethod]
                public void Test_equal_TotalSize()
                {
                    var testDisplay = new TestDisplay();
                    UIMenu mainMenu = new UIMenu("mainMenu", testDisplay);
                    UITitle title1 = new UITitle("title 1");
                    UIMenu menu1 = new UIMenu("menu 1");
                    
                    mainMenu.Add(title1);
                    mainMenu.Add(menu1);
                    mainMenu.Print();
                    title1.Display.TotalSize.Should().Be(menu1.Display.TotalSize);
                }

                [TestMethod]
                public void Test_equal_Length()
                {
                    var testDisplay = new TestDisplay();
                    UIMenu mainMenu = new UIMenu("mainMenu", testDisplay);
                    UITitle title1 = new UITitle("title 1");
                    UIMenu menu1 = new UIMenu("menu 1");

                    mainMenu.Add(title1);
                    mainMenu.Add(menu1);
                    mainMenu.Print();

                    title1.GetText().ToString().Length.Should().Be(menu1.GetText().ToString().Length);
                }

                [TestMethod]
                public void Test_TotalSize_and_Length_of_title1()
                {
                    UIMenu mainMenu = new UIMenu("mainMenu");
                    UITitle title1 = new UITitle("title 1");
                    UIMenu menu1 = new UIMenu("menu 1");

                    mainMenu.Add(title1);
                    mainMenu.Add(menu1);
                    title1.Display.TotalSize.Should().Be(title1.GetText().ToString().Length);
                }

                [TestMethod]
                public void Test_TotalSize_and_Length_of_menu1()
                {
                    var testDisplay = new TestDisplay();
                    UIMenu mainMenu = new UIMenu("mainMenu", testDisplay);
                    UITitle title1 = new UITitle("title 1");
                    UIMenu menu1 = new UIMenu("menu 1");
                    UIMenu menu2 = new UIMenu("me 1");

                    mainMenu.Add(title1);
                    mainMenu.Add(menu1);
                    mainMenu.Add(menu2);

                    mainMenu.Print();

                    //testDisplay.Lines[0].Length.Should().Be(9);
                    testDisplay.Lines[1].Length.Should().Be(9);
                    testDisplay.Lines[2].Length.Should().Be(9);
                }
            }

            public class TestDisplay : IDisplay
            {
                private ComplexDisplay _display = new ComplexDisplay();
                private string[] _lines;

                public void Print(IEnumerable<UIItem> items)
                {
                    foreach (var item in items)
                    {
                        _display.Add(item.GetText());
                    }
                    _lines = items.Select(l => l.GetText().ToString()).ToArray();

                    foreach (var item in _display.Lines)
                    {

                        Console.WriteLine(item.ToString());
                    }
                }

                public string[] Lines => _lines;
            }

            [TestMethod]
            public void Test_complex_display_dynamic_dimensions_with_lines()
            {
                ComplexDisplay display = new ComplexDisplay();

                DisplayLine line1 = new DisplayLine
                {
                    Canvas = new DisplayCanvas
                    {
                        Sizes = new List<int> { 0 },
                        DynamicIndex = 0
                    },
                    Texts = new List<DisplayText>
                {
                    new DisplayText("test", 4)
                }
                };//Total size = 4

                DisplayLine line2 = new DisplayLine
                {
                    Canvas = new DisplayCanvas
                    {
                        Sizes = new List<int> { 0 },
                        DynamicIndex = 0
                    },
                    Texts = new List<DisplayText>
                {
                    new DisplayText("test", 5)
                }
                };//Total size = 5

                display.Add(line1);//base size = 4
                display.Add(line2);//base size = 5
                //display.Add() modifies the size of each line so their size is equal to the longest line.
                //This is necessary so that lines with text after their name align.
                line1.TotalSize.Should().Be(5);
            }
        }
        

        //[TestMethod]
        //public void TestWithClass()
        //{
        //    var unit = new Meter(10000);

        //    ((Kilometer)unit).Value.Should().Be(10);
        //}

        //[TestMethod]
        //public void TestWithImplicitConvertion()
        //{
        //    DistanceUnit unit = new Meter(6000) + new Meter(4000);

            
        //    unit.Kilometers.Should().Be(10);
        //}

        //[TestMethod]
        //public void Test()
        //{

        //}

    }
}
