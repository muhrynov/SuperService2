﻿using BitMobile.ClientModel3;

namespace Test
{
    public class Solution : Application
    {
        public override void OnCreate()
        {
            DConsole.WriteLine("Starting application...");
            BusinessProcess.Init();
        }
    }
}
