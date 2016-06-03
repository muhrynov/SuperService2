﻿using System;
using System.Net;
using System.Text;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class BagListScreen : Screen
    {
        private TabEventsComponent _tabEventsComponent;

        public override void OnLoading()
        {
            _tabEventsComponent = new TabEventsComponent(this);
            DConsole.WriteLine("BagListScreen init");
        }

        internal void TabEventsButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabEventsComponent.Events_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Events");

        }

        internal void TabBagButton_OnClick(object sender, EventArgs eventArgs)
        {
            //_tabEventsComponent.Bag_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Bag");

        }

        internal void TabClientsButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabEventsComponent.Clients_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Clients");

        }

        internal void TabSettingsButton_OnClick(object sender, EventArgs eventArgs)
        {
            _tabEventsComponent.Settings_OnClick(sender, eventArgs);
            DConsole.WriteLine("Bag Settings");

        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }
    }
}
