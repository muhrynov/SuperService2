﻿using System;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;

namespace Test
{
    public static class Navigation
    {
        private const string DefaultStyle = @"Style\style.css";
        private static readonly Stack ScreenInfoStack = new Stack();
        private static readonly Stack ScreenStack = new Stack();

        public static ScreenInfo CurrentScreenInfo { get; private set; }

        public static void Back(bool reload = false)
        {
            if (ScreenInfoStack.Count == 0)
            {
                DConsole.WriteLine("Can't go back when stack is empty");
                return;
            }
            var screenInfo = (ScreenInfo) ScreenInfoStack.Pop();
            if (reload)
                screenInfo.Screen = (Screen) Application.CreateInstance($"Test.{screenInfo.Name}");
            ModalMove(screenInfo);
        }

        public static void MoveTo(string name, string css = null, Dictionary<string, object> args = null)
        {
            DConsole.WriteLine("Loading screen info");
            var screenInfo = CreateScreenInfoFromName(name, css);
            DConsole.WriteLine("Really moving somewhere");
            MoveTo(screenInfo, args);
        }

        private static ScreenInfo CreateScreenInfoFromName(string name, string css)
        {
            var screen = (Screen) Application.CreateInstance($"Test.{name}");
            DConsole.WriteLine($"Created instance of {name}");
            DConsole.WriteLine($"ORLY? : {screen == null}");
            return new ScreenInfo
            {
                Name = name,
                Screen = screen,
                Xml = $@"Screen\{name}.xml",
                Css = css ?? $@"Style\{name}.css"
            };
        }

        public static void MoveTo(ScreenInfo screenInfo, Dictionary<string, object> args = null)
        {
            DConsole.WriteLine("Is there CSI?");
            if (CurrentScreenInfo != null)
                ScreenInfoStack.Push(CurrentScreenInfo);
            DConsole.WriteLine("Ok, now really moving");
            ModalMove(screenInfo, args);
        }

        public static void ModalMove(string name, string css = null, Dictionary<string, object> args = null)
        {
            var screenInfo = CreateScreenInfoFromName(name, css);
            ModalMove(screenInfo, args);
        }

        public static void ModalMove(ScreenInfo screenInfo, Dictionary<string, object> args = null)
        {
            DConsole.WriteLine("Trying to get screen from info");
            var screen = screenInfo.Screen;
            DConsole.WriteLine("Basic screen interaction");
            screen.SetData(args);
            try
            {
                screen.LoadFromStream(Application.GetResourceStream(screenInfo.Xml));
            }
            catch
            {
                DConsole.WriteLine($"Can't find xml file for {screenInfo.Name}");
            }
            try
            {
                screen.LoadStyleSheet(Application.GetResourceStream(screenInfo.Css));
            }
            catch
            {
                screen.LoadStyleSheet(Application.GetResourceStream(DefaultStyle));
            }
            DConsole.WriteLine("Now we can show this");
            screen.Show();
            DConsole.WriteLine("Oh, and don't forget to save new info");
            CurrentScreenInfo = screenInfo;
        }
    }

    public class ScreenInfo
    {
        public string Name { get; set; }
        public Screen Screen { get; set; }
        public string Xml { get; set; }
        public string Css { get; set; }
    }
}