﻿using System;
using System.Collections;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Stack = BitMobile.ClientModel3.Stack;

namespace Test
{
    public static class Navigation
    {
        private const string DefaultStyle = @"Style\style.css";
        private static readonly Stack ScreenInfoStack = new Stack();
        private static readonly Stack ScreenStack = new Stack();

        // ReSharper disable once NotAccessedField.Local
        private static Screen _lastScreen;

        private static readonly ArrayList CurrentScreenInfoRef = new ArrayList();

        private static readonly ArrayList CurrentScreenRef = new ArrayList();

        /// <summary>
        ///     Информация о текущем отображаемом экране
        /// </summary>
        public static ScreenInfo CurrentScreenInfo
        {
            get { return CurrentScreenInfoRef.Count == 0 ? null : (ScreenInfo) CurrentScreenInfoRef[0]; }
            private set
            {
                if (CurrentScreenInfoRef.Count == 0)
                {
                    CurrentScreenInfoRef.Add(value);
                }
                else
                {
                    CurrentScreenInfoRef[0] = value;
                }
            }
        }

        /// <summary>
        ///     Ссылка на отображемый экран
        /// </summary>
        public static Screen CurrentScreen
        {
            get { return CurrentScreenRef.Count == 0 ? null : (Screen) CurrentScreenRef[0]; }
            private set
            {
                if (CurrentScreenRef.Count == 0)
                {
                    CurrentScreenRef.Add(value);
                }
                else
                {
                    CurrentScreenRef[0] = value;
                }
            }
        }

        /// <summary>
        ///     Перейти на один экран по стеку назад
        /// </summary>
        /// <param name="reload">Перезагржать ли целевой экран</param>
        public static void Back(bool reload = false)
        {
            if (ScreenInfoStack.Count == 0)
            {
                DConsole.WriteLine("Can't go back when stack is empty");
                return;
            }
            var screenInfo = (ScreenInfo) ScreenInfoStack.Pop();
            var nextScreen = (Screen) ScreenStack.Pop();
            _lastScreen = CurrentScreen;
            if (!reload)
                ModalMove(screenInfo, screen: nextScreen);
            else
                ModalMove(screenInfo);
        }

        /// <summary>
        ///     Передвинуться на экран, записав текущий экран на стек
        /// </summary>
        /// <param name="name">Имя целевого экрана</param>
        /// <param name="args">Словарь аргументов</param>
        /// <param name="css">Путь к файлу стиля</param>
        public static void Move(string name, IDictionary<string, object> args = null, string css = null)
        {
            var screenInfo = CreateScreenInfoFromName(name, css);
            Move(screenInfo, args);
        }

        private static ScreenInfo CreateScreenInfoFromName(string name, string css)
        {
            return new ScreenInfo
            {
                Name = name,
                Xml = $@"Screen\{name}.xml",
                Css = css ?? $@"Style\{name}.css"
            };
        }

        /// <summary>
        ///     Передвинуться на экран, записав текущий экран на стек
        /// </summary>
        /// <param name="screenInfo">Информация о следующем экране</param>
        /// <param name="args">Словарь аргументов</param>
        public static void Move(ScreenInfo screenInfo, IDictionary<string, object> args = null)
        {
            if (CurrentScreenInfo != null)
            {
                ScreenInfoStack.Push(CurrentScreenInfo);
                ScreenStack.Push(CurrentScreen);
            }
            ModalMove(screenInfo, args);
        }

        /// <summary>
        ///     Изменить отображаемый экран, не меняя стек.
        /// </summary>
        /// <param name="name">Имя экрана</param>
        /// <param name="args">Словарь аргументов</param>
        /// <param name="css">Путь к файлу стилей</param>
        public static void ModalMove(string name, IDictionary<string, object> args = null, string css = null)
        {
            var screenInfo = CreateScreenInfoFromName(name, css);
            ModalMove(screenInfo, args);
        }

        /// <summary>
        ///     Изменить отображаемый экран, не меняя стек.
        /// </summary>
        /// <param name="screenInfo">Информация об экране</param>
        /// <param name="args">Словарь аргументов</param>
        /// <param name="screen">Ссылка на целевой экран</param>
        public static void ModalMove(ScreenInfo screenInfo, IDictionary<string, object> args = null,
            Screen screen = null)
        {
            DConsole.WriteLine($"Moving to {screenInfo.Name}");
            screen = screen ?? (Screen) Application.CreateInstance($"Test.{screenInfo.Name}");
            screen.SetData(args);
            try
            {
                screen.LoadFromStream(Application.GetResourceStream(screenInfo.Xml));
            }
            catch (Exception e)
            {
                DConsole.WriteLine($"Can't find xml file for {screenInfo.Name} ({screenInfo.Xml})");
                DConsole.WriteLine($"{e.Message}");
            }
            try
            {
                screen.LoadStyleSheet(Application.GetResourceStream(screenInfo.Css));
            }
            catch
            {
                screen.LoadStyleSheet(Application.GetResourceStream(DefaultStyle));
            }
            CurrentScreenInfo = screenInfo;
            CurrentScreen = screen;
            screen.Show();
        }

        /// <summary>
        ///     Очистить стек
        /// </summary>
        public static void CleanStack()
        {
            ScreenInfoStack.Clear();
            ScreenStack.Clear();
        }
    }

    public class ScreenInfo
    {
        public string Name { get; set; }
        public string Xml { get; set; }
        public string Css { get; set; }
    }
}