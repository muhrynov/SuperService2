﻿using System;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class CheckInfoScreen : Screen
    {
        private string _eventId;
        private List<string> _fiscalList;
        private bool _readonly;
        private TopInfoComponent _topInfoComponent;
        private bool _wasStarted;

        public override void OnLoading()
        {
            base.OnLoading();
            _readonly = (bool) Variables.GetValueOrDefault(Parameters.IdIsReadonly, false);
            _topInfoComponent = new TopInfoComponent(this)
            {
                Header = Translator.Translate("cashbox_check"),
                LeftButtonControl = new Image {Source = ResourceManager.GetImage("topheading_back")},
                RightButtonControl = _fiscalList.Count == 0
                    ? _readonly
                        ? new Image {Source = ResourceManager.GetImage("print_icon_disabel")}
                        : new Image {Source = ResourceManager.GetImage("print_icon")}
                    : new Image {Source = ResourceManager.GetImage("print_icon_disabel")},
                ArrowVisible = false
            };

            _topInfoComponent.ActivateBackButton();
            _eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            _wasStarted = (bool) Variables.GetValueOrDefault(Parameters.IdWasEventStarted, true);
        }

        public override void OnShow()
        {
            base.OnShow();
        }


        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
            => Navigation.ModalMove(nameof(COCScreen), new Dictionary<string, object>
            {
                {Parameters.IdCurrentEventId, _eventId},
                {Parameters.IdIsReadonly, _readonly},
                {Parameters.IdWasEventStarted, _wasStarted}
            });

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
            if (_fiscalList.Count != 0) return;
            if (!_readonly)
                Navigation.ModalMove(nameof(PrintCheckScreen), new Dictionary<string, object>
                {
                    {Parameters.IdCurrentEventId, _eventId},
                    {Parameters.IdIsReadonly, _readonly},
                    {Parameters.IdWasEventStarted, _wasStarted}
                });
        }

        internal void TopInfo_LeftButton_OnPressDown(object sender, EventArgs e)
        {
            ((Image)_topInfoComponent.LeftButtonControl).Source = ResourceManager.GetImage("topheading_back_active");
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_LeftButton_OnPressUp(object sender, EventArgs e)
        {
            ((Image)_topInfoComponent.LeftButtonControl).Source = ResourceManager.GetImage("topheading_back");
            _topInfoComponent.Refresh();
        }

        internal void TopInfo_RightButton_OnPressDown(object sender, EventArgs e)
        {
            Image image = (Image)_topInfoComponent.RightButtonControl;
            image.Source = _fiscalList.Count == 0
                    ? _readonly
                        ? ResourceManager.GetImage("print_icon_disabel")
                        : ResourceManager.GetImage("print_icon_active")
                    : ResourceManager.GetImage("print_icon_disabel");
            image.Refresh();
        }

        internal void TopInfo_RightButton_OnPressUp(object sender, EventArgs e)
        {
            Image image = (Image)_topInfoComponent.RightButtonControl;
            image.Source = _fiscalList.Count == 0
                    ? _readonly
                        ? ResourceManager.GetImage("print_icon_disabel")
                        : ResourceManager.GetImage("print_icon")
                    : ResourceManager.GetImage("print_icon_disabel");
            image.Refresh();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        internal string GetResourceImage(string tag) => ResourceManager.GetImage(tag);

        internal string GetNameVAT(string vatEnum)
        {
            var strStart = Translator.Translate("VAT");
            switch (vatEnum)
            {
                case "Percent18":
                    return strStart + " 18%";
                case "Percent0":
                    return strStart + " 0%";
                case "PercentWithoOut":
                    return Translator.Translate("percent_witho_out");
                case "Percent10":
                    return strStart + " 10%";

                default:
                    return "";
            }
        }

        internal string GetSumCheck()
        {
            var totalSum =
                DBHelper.GetCheckSKUSum((string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty));
            var decimalSum = Converter.ToDecimal(totalSum);

            return $"{decimalSum:N}";
        }

        internal DbRecordset GetFiscalProp()
        {
            _fiscalList = new List<string>();
            var eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            var fiscalRecordSet = DBHelper.GetFiscalEvent(eventId);
            while (fiscalRecordSet.Next())
            {
                _fiscalList.Add(fiscalRecordSet["CheckNumber"].ToString());
                _fiscalList.Add(fiscalRecordSet["Date"].ToString());
                _fiscalList.Add(fiscalRecordSet["NumberFtpr"].ToString());
                _fiscalList.Add(fiscalRecordSet["ShiftNumber"].ToString());
            }
            return fiscalRecordSet;
        }

        internal string GetCheckNumber() => _fiscalList[0];
        internal string GetDate() => _fiscalList[1];
        internal string GetNumberFtpr() => _fiscalList[2];
        internal string GetShiftNumber() => _fiscalList[3];

        internal bool CheckFiscalEvent() => _fiscalList.Count != 0;

        internal string ConvertToDec(object price)
        {
            return $"{Converter.ToDecimal(price):N}";
        }


        internal DbRecordset GetRIMList()
        {
            var eventId = (string) Variables.GetValueOrDefault(Parameters.IdCurrentEventId, string.Empty);
            return DBHelper.GetCheckSKU(eventId);
        }
    }
}