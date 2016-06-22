﻿using System;
using System.Collections;
using System.Collections.Generic;
using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using Test.Components;

namespace Test
{
    public class RIMListScreen : Screen
    {
        private bool _isMaterialRequest;
        private bool _isService;
        private string _currentEventID;

        private TopInfoComponent _topInfoComponent;

        private bool _fieldsAreInitialized = false;

        public override void OnLoading()
        {
            DConsole.WriteLine("RIMListScreen init");

            InitClassFields();

            _topInfoComponent = new TopInfoComponent(this)
            {
                HeadingTextView =
                {
                    Text = _isService ? Translator.Translate("services") : Translator.Translate("materials")
                },
                LeftButtonImage = {Source = ResourceManager.GetImage("topheading_back")},
                RightButtonImage = {Visible = false},
                ExtraLayoutVisible = false
            };


        }


        public int InitClassFields()
        {
            DConsole.WriteLine("InitClassFields()");

            if (_fieldsAreInitialized)
            {
                return 0;
            }

            var isMaterialRequest = Variables.GetValueOrDefault("isMaterialsRequest", Convert.ToBoolean("False"));
            _isMaterialRequest = (bool) isMaterialRequest;

            var isService = Variables.GetValueOrDefault("isService", Convert.ToBoolean("False"));
            _isService = (bool) isService;

            var currentEventId = Variables.GetValueOrDefault("currentEventId", Convert.ToString(""));
            _currentEventID = (string) currentEventId;

            _fieldsAreInitialized = true;

            return 0;
        }


        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            Navigation.Back();
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
            _topInfoComponent.Arrow_OnClick(sender, e);
        }

        internal void RIMLayout_OnClick(object sender, EventArgs eventArgs)
        {

            if (_isMaterialRequest == true)
            {
                //пришли из экрана заявки на материалы


            }
            else
            {
                var rimID = ((VerticalLayout)sender).Id;
                var price = decimal.Parse(((TextView)((VerticalLayout)sender).Controls[1]).Text);

                DConsole.WriteLine("Пытаемся найти номенклатуру в документе " + (string)_currentEventID + " по гуиду " + rimID);
                var line = DBHelper.GetEventServicesMaterialsLineByRIMID((string)_currentEventID, rimID);

                if (line == null)
                {
                    DConsole.WriteLine("Позиция не найдена, просто добавлеям новую");

                    var dictionary = new Dictionary<string, object>
                    {
                        {"behaviour", BehaviourEditServicesOrMaterialsScreen.InsertIntoDB},
                        {"rimID"    , rimID}
                    };

                    Navigation.Move("EditServicesOrMaterialsScreen", dictionary);


                    /*line = new EventServicesMaterialsLine
                    {
                        Ref = (string)_currentEventID,
                        SKU = rimID,
                        Price = price,
                        AmountPlan = 0,
                        SumPlan = 0,
                        AmountFact = 1
                    };
                    line.SumFact = line.AmountFact * line.Price;

                    DBHelper.InsertEventServicesMaterialsLine(line);

                    DConsole.WriteLine("Добавили");*/
                }
                else
                {
                    DConsole.WriteLine("Позиция найдена, открываем окно редактирования количества ");
                    var dictionary = new Dictionary<string, object>
                    {
                        {"behaviour", BehaviourEditServicesOrMaterialsScreen.UpdateDB},
                        {"lineId"   , line.ID}
                    };

                    Navigation.Move("EditServicesOrMaterialsScreen", dictionary);


                    /*
                    var dictionary = new Dictionary<string, object>
                    {
                        {"behaviour", BehaviourEditServicesOrMaterialsScreen.UpdateDB},
                        {"lineId", line.ID}
                    };

                    Navigation.Move("EditServicesOrMaterialsScreen", dictionary);
                    */
                }






                /*
                //пришли из экрана АВР    
                if (_isMaterialRequest)
                {
                    var key = Variables.GetValueOrDefault("returnKey", "newItem");
                    var dictionary = new Dictionary<string, object>
                {
                    {"rimId", rimID},
                    {"priceVisible", Convert.ToBoolean("False")},
                    {"behaviour", BehaviourEditServicesOrMaterialsScreen.ReturnValue},
                    {"returnKey", key},
                    {"lineId", null}
                };
                    DConsole.WriteLine("Go to EditServicesOrMaterials is Material Request true");
                    Navigation.Move("EditServicesOrMaterialsScreen", dictionary);
                }
                else
                {
                    var dictionary = new Dictionary<string, object>
                {
                    {"rimId", rimID},
                    {"behaviour", BehaviourEditServicesOrMaterialsScreen.InsertIntoDB}
                };

                    DConsole.WriteLine("Go to EditServicesOrMaterials is Material Request false");
                    Navigation.Move("EditServicesOrMaterialsScreen", dictionary);
                }
                */
                
           }

        }


        internal IEnumerable GetRIM()
        {
            DConsole.WriteLine("получение позиций товаров и услуг");


            DbRecordset result;

            if (_isService)
            {
                result = DBHelper.GetRIMByType(RIMType.Service);
                DConsole.WriteLine("Получили услуги " + RIMType.Material);
            }
            else
            {
                result = DBHelper.GetRIMByType(RIMType.Material);
                DConsole.WriteLine("Получили товары " + RIMType.Material);
            }

            return result;
        }
    }
}