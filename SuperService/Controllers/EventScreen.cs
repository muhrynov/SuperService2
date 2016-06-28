﻿using System;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class EventScreen : Screen
    {
        private DbRecordset _currentEventRecordset;
        private Button _refuseButton;
        private DockLayout _rootLayout;
        private Button _startButton;

        private Button _startFinishButton;

        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this);

            LoadControls();
            FillControls();

            IsEmptyDateTime((string) _currentEventRecordset["ActualStartDate"]);
        }

        private void FillControls()
        {
            _topInfoComponent.HeadingTextView.Text = (string) _currentEventRecordset["clientDescription"];
            _topInfoComponent.CommentTextView.Text = (string) _currentEventRecordset["clientAddress"];
            _topInfoComponent.LeftButtonImage.Source = ResourceManager.GetImage("topheading_back");
            _topInfoComponent.RightButtonImage.Source = ResourceManager.GetImage("topheading_info");

            _topInfoComponent.LeftExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = ResourceManager.GetImage("topinfo_extra_map")
            });
            _topInfoComponent.LeftExtraLayout.AddChild(new TextView
            {
                Text = Translator.Translate("onmap"),
                CssClass = "TopInfoSideText"
            });

            _topInfoComponent.RightExtraLayout.AddChild(new Image
            {
                CssClass = "TopInfoSideImage",
                Source = ResourceManager.GetImage("topinfo_extra_person")
            });

            var visContact = (string)_currentEventRecordset["ContactVisitingDescription"];
            DConsole.WriteLine(visContact);

            _topInfoComponent.RightExtraLayout.AddChild(new TextView
            {
                Text = (string)_currentEventRecordset["ContactVisitingDescription"],
                CssClass = "TopInfoSideText"
            });

            DConsole.WriteLine($"{nameof(GoToMapScreen_OnClick)} before add");
            _topInfoComponent.LeftExtraLayout.OnClick += GoToMapScreen_OnClick;
            DConsole.WriteLine($"{nameof(GoToMapScreen_OnClick)} after");
        }

        public override void OnShow()
        {
            GPS.StartTracking();
        }

        private void LoadControls()
        {
            _rootLayout = (DockLayout) GetControl("RootLayout");
            _startFinishButton = (Button) GetControl("StartFinishButton", true);
            _startButton = (Button) GetControl("StartButton", true);
            _refuseButton = (Button) GetControl("RefuseButton", true);
        }

        internal void ClientInfoButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move("ClientScreen");
        }

        internal void RefuseButton_OnClick(object sender, EventArgs eventArgs)
        {
            DBHelper.UpdateCancelEventById((string) BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
            Navigation.Back(true);
        }

        internal string FormatEventStartDatePlanTime(string eventStartDatePlanTime)
        {
            return eventStartDatePlanTime.Substring(0, 5);
        }

        internal void StartButton_OnClick(object sender, EventArgs eventArgs)
        {
            Dialog.Ask(Translator.Translate("areYouSure"), (o, args) =>
            {
                if (args.Result == Dialog.Result.Yes)
                {
                    ChangeLayoutToStartedEvent();
                }
            });
        }

        private void ChangeLayoutToStartedEvent()
        {
            _startButton.CssClass = "NoHeight";
            _startButton.Visible = false;
            _startButton.Refresh();
            _refuseButton.CssClass = "NoHeight";
            _refuseButton.Visible = false;
            _refuseButton.Refresh();
            _startFinishButton.CssClass = "FinishButton";
            _startFinishButton?.Refresh();
            _startFinishButton.Text = Translator.Translate("finish");
            _rootLayout.Refresh();
            Event_OnStart();
        }

        internal void StartFinishButton_OnClick(object sender, EventArgs eventArgs)
        {
            Dialog.Alert(Translator.Translate("closeeventquestion"), (o, args) =>
            {
                if (CheckEventBeforeClosing() && args.Result == 0)
                {
                    DBHelper.UpdateActualEndDateByEventId(DateTime.Now,
                        (string) BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
                    Navigation.Move("CloseEventScreen");
                }
            }, null,
                Translator.Translate("yes"), Translator.Translate("no"));
        }

        private bool CheckEventBeforeClosing()
        {
            // TODO: Здесь будет проверка наряда перед закрытием
            return true;
        }

        private void Event_OnStart()
        {
            DBHelper.UpdateActualStartDateByEventId(DateTime.Now,
                (string) BusinessProcess.GlobalVariables[Parameters.IdCurrentEventId]);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs eventArgs)
        {
            BusinessProcess.GlobalVariables[Parameters.IdClientId] = _currentEventRecordset[Parameters.IdClientId].ToString();
            Navigation.Move("ClientScreen");
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs eventArgs)
        {
            _topInfoComponent.Arrow_OnClick(sender, eventArgs);
            _rootLayout.Refresh();
        }

        internal void TaskCounterLayout_OnClick(object sender, EventArgs eventArgs)
        {
            if (CheckBigButtonActive(sender))
                Navigation.Move("TaskListScreen");
        }

        private bool CheckBigButtonActive(object sender)
        {
            // TODO: Сделать проверку более аккуратной?
            var layout = (HorizontalLayout) sender;
            return ((TextView) layout.Controls[2]).Text != "0";
        }

        internal void GoToCOCScreen_OnClick(object sender, EventArgs e)
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }

            var dictinory = new Dictionary<string, object>()
            {
                {Parameters.IdCurrentEventId,(string)eventId }
            };
            Navigation.Move("COCScreen", dictinory);
        }

        internal void CheckListCounterLayout_OnClick(object sender, EventArgs eventArgs)
        {
            Navigation.Move("CheckListScreen");
        }

        internal DbRecordset GetCurrentEvent()
        {
            object eventId;
            if (!BusinessProcess.GlobalVariables.TryGetValue(Parameters.IdCurrentEventId, out eventId))
            {
                DConsole.WriteLine("Can't find current event ID, going to crash");
            }
            _currentEventRecordset = DBHelper.GetEventByID((string) eventId);
            return _currentEventRecordset;
        }

        internal string GetStringPartOfTotal(double part, double total)
        {
            if (Convert.ToInt64(part) != 0) return $"{part}/{total}";
//            DConsole.WriteLine($"{part == 0L}, {Convert.ToInt64(total) == 0L}, {part}, {total}");
            return $"{Convert.ToInt64(total)}";
        }

        internal bool IsEmptyDateTime(string dateTime)
        {
            return dateTime == "0001-01-01 00:00:00";
        }

        internal bool IsNotEmptyDateTime(string dateTime)
        {
            return dateTime != "0001-01-01 00:00:00";
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void GoToMapScreen_OnClick(object sender, EventArgs e)
        {
            var clientId = (string) _currentEventRecordset[Parameters.IdClientId];
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdScreenStateId, MapScreenStates.EventScreen},
                {Parameters.IdClientId, clientId}
            };

            BusinessProcess.GlobalVariables.Remove(Parameters.IdScreenStateId);
            BusinessProcess.GlobalVariables.Remove(Parameters.IdClientId);
            BusinessProcess.GlobalVariables[Parameters.IdScreenStateId] = MapScreenStates.EventScreen;
            BusinessProcess.GlobalVariables[Parameters.IdClientId] = clientId;

            Navigation.Move("MapScreen", dictionary);
        }
    }
}