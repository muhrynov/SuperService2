﻿using System;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Controls;
using Test.Components;

namespace Test
{
    public class COCScreen : Screen
    {
        private DbRecordset _sums;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            DConsole.WriteLine("In to: " + nameof(OnLoading));
            _topInfoComponent = new TopInfoComponent(this)
            {
                ExtraLayoutVisible = true,
                HeadingTextView = {Text = Translator.Translate("coc")},
                RightButtonImage = {Visible = false},
                LeftButtonImage = {Source = ResourceManager.GetImage("topheading_back")},
                CommentTextView =
                {
                    Text = Translator.Translate("total") + Environment.NewLine + Convert.ToDouble((double) _sums["Sum"])
                           + Translator.Translate("currency")
                },
                BigArrowActive = false
            };
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back(true);
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal void AddService_OnClick(object sender, EventArgs e)
        {
            BusinessProcess.GlobalVariables["isService"] = true;
            Navigation.Move("AddServicesOrMaterialsScreen");
        }

        internal void AddMaterial_OnClick(object sender, EventArgs e)
        {
            BusinessProcess.GlobalVariables["isService"] = false;
            Navigation.Move("AddServicesOrMaterialsScreen");
        }

        internal void EditServicesOrMaterials_OnClick(object sender, EventArgs e)
        {
            var vl = (VerticalLayout) sender;
            BusinessProcess.GlobalVariables["currentServicesMaterialsId"] = vl.Id;
            Navigation.Move("EditServicesOrMaterialsScreen");
        }

        internal void ApplicatioMaterials_OnClick(object sender, EventArgs e)
        {
            Navigation.Move("MeterialsRequestScreen");
        }

        internal void OpenDeleteButton_OnClick(object sender, EventArgs e)
        {
            //TODO: Обходной путь получения парента. Внимание!!!!! .
            var vl = (VerticalLayout) sender;
            var hl = (IHorizontalLayout3) vl.Parent;
            var shl = (ISwipeHorizontalLayout3) hl.Parent;
            ++shl.Index;
        }

        internal void DeleteButton_OnClick(object sender, EventArgs e)
        {
            //TODO: Обходной путь получения парента. Внимание!!!!!.
            var vl = (VerticalLayout) sender;
            DBHelper.DeleteServiceOrMaterialById(vl.Id);
            var shl = (ISwipeHorizontalLayout3) vl.Parent;
            shl.CssClass = "NoHeight";
            shl.Refresh();
        }

        internal string GetFormatStringForSums(double number)
        {
            return "\u2022" + Convert.ToDouble(number) + Translator.Translate("currency");
        }


        internal DbRecordset GetSums()

        {
            DConsole.WriteLine("COC - GetSums()");

            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue("currentEventId", out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }
            DConsole.WriteLine("In to: " + nameof(GetSums));
            _sums = DBHelper.GetCocSumsByEventId((string) eventId);

            return _sums;
        }

        internal DbRecordset GetServices()
        {
            DConsole.WriteLine("COC - GetServices()");
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue("currentEventId", out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }

            return DBHelper.GetServicesByEventId((string) eventId);
        }

        internal string Concat(float amountFact, float price)
        {
            return Convert.ToSingle(amountFact) + " x " + Convert.ToSingle(price);
        }

        internal DbRecordset GetMaterials()
        {
            DConsole.WriteLine("COC - GetMaterials()");

            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue("currentEventId", out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }

            return DBHelper.GetMaterialsByEventId((string) eventId);
        }
    }
}