﻿using System;
using BitMobile.ClientModel3.UI;

namespace Test.Components
{
    public class TabBarComponent
    {
        private readonly Screen _parentScreen;
        private Image _image;
        private TextView _textView;

        public TabBarComponent(Screen parentScreen)
        {
            _parentScreen = parentScreen;
            SwitchActiveTab(parentScreen);
        }

        internal void SwitchActiveTab(Screen parentScreen)
        {
            var screenName = Navigation.CurrentScreenInfo.Name;
            switch (screenName)
            {
                case nameof(EventListScreen):
                    _textView = (TextView) _parentScreen.GetControl("TabBarFirstTabTextView", true);
                    _image = (Image) parentScreen.GetControl("TabBarFirstTabImage", true);

                    _textView.CssClass = "TabTextViewActive";
                    _image.Source = ResourceManager.GetImage("tabbar_events_active");
                    break;

                case nameof(ClientListScreen):
                    _textView = (TextView) parentScreen.GetControl("TabBarThirdTabTextView", true);
                    _image = (Image) parentScreen.GetControl("TabBarThirdTabImage", true);

                    _textView.CssClass = "TabTextViewActive";
                    _image.Source = ResourceManager.GetImage("tabbar_clients_active");
                    break;

                case nameof(FiscalRegistratorSettingsScreen):
                    _textView = (TextView) parentScreen.GetControl("TabBarSecondTabTextView", true);
                    _image = (Image) parentScreen.GetControl("TabBarSecondTabImage", true);

                    _textView.CssClass = "TabTextViewActive";
                    _image.Source = ResourceManager.GetImage("tabbar_fr_active");
                    break;

                case nameof(SettingsScreen):
                    _textView = (TextView) parentScreen.GetControl("TabBarFourthTabTextView", true);
                    _image = (Image) parentScreen.GetControl("TabBarFourthTabImage", true);

                    _textView.CssClass = "TabTextViewActive";
                    _image.Source = ResourceManager.GetImage("tabbar_settings_active");
                    break;
            }
        }

        internal void Events_OnClick(object sender, EventArgs e) 
            => Navigation.ModalMove(nameof(EventListScreen));

        internal void FrSettings_OnClick(object sender, EventArgs e)
            => Navigation.ModalMove(nameof(FiscalRegistratorSettingsScreen));

        internal void Clients_OnClick(object sender, EventArgs e) 
            => Navigation.ModalMove(nameof(ClientListScreen));

        internal void Settings_OnClick(object sender, EventArgs e) 
            => Navigation.ModalMove(nameof(SettingsScreen));
    }
}