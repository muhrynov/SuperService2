﻿using BitMobile.ClientModel3;
using BitMobile.ClientModel3.UI;
using BitMobile.Common.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using Test.Components;

namespace Test
{
    // TODO: Сделать задвигающие SwipeHorizontalLayout
    public class MaterialsRequestScreen : Screen
    {
        private static ArrayList _data;
        private static bool _isAdd = Convert.ToBoolean("False");
        private static bool _isEdit = Convert.ToBoolean("False");
        private bool _isEmptyList;
        private TopInfoComponent _topInfoComponent;

        public override void OnLoading()
        {
            _topInfoComponent = new TopInfoComponent(this)
            {
                ExtraLayoutVisible = false,
                HeadingTextView = { Text = Translator.Translate("request") },
                RightButtonImage = { Visible = false },
                LeftButtonImage = { Source = ResourceManager.GetImage("close") }
            };
        }

        public override void OnShow()
        {
        }

        private void GetValueFromOtherScreen()
        {
#if DEBUG
            DConsole.WriteLine($"{nameof(_isAdd)} = {_isAdd} {nameof(_isEdit)} {_isEdit}");
#endif
            if (_isAdd)
            {
                var newItem = BusinessProcess.GlobalVariables.GetValueOrDefault("newItem");

                if (newItem != null)
                {
                    var item = (EditServiceOrMaterialsScreenResult)newItem;

                    if (!CombineIfExist(item))
                    {
                        var itemInfo = DBHelper.GetServiceMaterialPriceByRIMID(item.RimId);
                        var dictionary = new Dictionary<string, object>
                        {
                            //SKU aka rimId
                            {"SKU", item.RimId},
                            {"Count", item.Count},
                            {"Unit", (string) itemInfo["Unit"]},
                            {"Description", (string) itemInfo["Description"]}
                        };
                        _data.Add(dictionary);
#if DEBUG
                        DConsole.WriteLine($"Element is added _data.Count = {_data.Count}");
#endif
                    }
                }
                Variables.Remove("newItem");
                _isAdd = Convert.ToBoolean("False");
            }
            else if (_isEdit)
            {
                var editItem = BusinessProcess.GlobalVariables.GetValueOrDefault("editItem");

                if (editItem != null)
                {
                    var item = (EditServiceOrMaterialsScreenResult)editItem;

                    foreach (var element in _data)
                    {
                        var dictionary = (Dictionary<string, object>)element;
                        var elementRimId = (string)dictionary["SKU"];

                        if (string.Compare(elementRimId, item.RimId, false) == 0)
                        {
                            dictionary["Count"] = item.Count;
                            break;
                        }
                    }
#if DEBUG
                    DConsole.WriteLine("element is changed!");
#endif
                }
                BusinessProcess.GlobalVariables.Remove("editItem");
                _isEdit = Convert.ToBoolean("False");
            }

#if DEBUG
            DConsole.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
            PrintMaterialsData();
#endif
        }

        private void PrintMaterialsData()
        {
            DConsole.WriteLine($"{nameof(_data)}.{nameof(_data.Count)} = {_data.Count}");

            foreach (var item in _data)
            {
                var element = (Dictionary<string, object>)item;

                foreach (var i in element)
                {
                    DConsole.WriteLine($"{i.Key} = {i.Value}");
                }

                DConsole.WriteLine($"{Environment.NewLine}{Environment.NewLine}");
            }
        }

        internal void TopInfo_LeftButton_OnClick(object sender, EventArgs e)
        {
            DConsole.WriteLine("Back to screen .....");
            Navigation.Back();
        }

        internal void TopInfo_RightButton_OnClick(object sender, EventArgs e)
        {
        }

        internal void TopInfo_Arrow_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Метод осуществляет проверку, существует ли
        ///     элемент в коллекции _data. Если существует
        ///     тогда объединяет элементы с одинаковым rimId
        ///     и суммирует их колличество.
        /// </summary>
        /// <param name="item"> Элемент, который нужно проверить на существование в коллекции</param>
        /// <returns> true, если элемент существует в коллекции и объединился. false если не существует </returns>
        private bool CombineIfExist(EditServiceOrMaterialsScreenResult item)
        {
            foreach (var element in _data)
            {
                var dictionary = (Dictionary<string, object>)element;
                var elementRimId = (string)dictionary["SKU"];

                if (string.Compare(elementRimId, item.RimId, false) == 0)
                {
                    DConsole.WriteLine($"Count before {(int)dictionary["Count"]} SKU = {(string)dictionary["SKU"]}");
                    dictionary["Count"] = (int)dictionary["Count"] + item.Count;
                    DConsole.WriteLine($"Element is Exist and changed count = {(int)dictionary["Count"]}");
                    return true;
                }
            }
            return false;
        }

        internal string GetResourceImage(string tag)
        {
            return ResourceManager.GetImage(tag);
        }

        /// <summary>
        ///     Проверяет данные, которые получили в поле
        ///     _data.
        /// </summary>
        /// <returns>true - если данных нет у поля _data, иначе false</returns>
        internal bool GetIsEmptyList()
        {
            DConsole.WriteLine("GetIsEmptyList()");
            if (_data == null)
            {
                _isAdd = _isEdit = Convert.ToBoolean("False");
                _data = new ArrayList();
            }
            else
                GetValueFromOtherScreen();

            return _isEmptyList = Convert.ToBoolean(_data.Count > 0 ? "False" : "True");
        }

        internal void OpenDeleteButton_OnClick(object sender, EventArgs e)
        {
            var vl = (VerticalLayout)sender;
            var hl = (IHorizontalLayout3)vl.Parent;
            var shl = (ISwipeHorizontalLayout3)hl.Parent;
            ++shl.Index;
        }

        internal void DeleteButton_OnClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            DConsole.WriteLine($"{nameof(btn.Id)} = {btn.Id}");
            var shl = (ISwipeHorizontalLayout3)btn.Parent;
            DeleteElement(btn.Id);
            PrintMaterialsData();
            shl.CssClass = "NoHeight";
            ((IVerticalLayout3)shl.Parent).Refresh();
        }

        internal ArrayList GetData()
        {
            //Данные, которые были получены у БД передаются в XML разметку.
            return _data;
        }

        private void DeleteElement(string id)
        {
            var index = -1;

            for (var i = 0; i < _data.Count; i++)
            {
                var element = (Dictionary<string, object>)_data[i];
                var skuId = (string)element["SKU"];
                if (string.Compare(skuId, id, false) == 0)
                {
                    index = i;
#if DEBUG
                    DConsole.WriteLine("Index naiden" + Environment.NewLine);
#endif
                    break;
                }
            }

            if (index >= 0)
            {
                _data.RemoveAt(index);
#if DEBUG
                DConsole.WriteLine($"Element {id} with {nameof(index)} = {index} is deleted {Environment.NewLine}");
#endif
            }
#if DEBUG
            else
            {
                DConsole.WriteLine(
                    $"Element is not deleted Error in method {nameof(DeleteElement)} {Environment.NewLine}");
            }
#endif
        }

        internal void AddMaterial_OnClick(object sender, EventArgs e)
        {
            //TODO: Отсюда переходим на экран добавления материалов.
            var dictionary = new Dictionary<string, object>
            {
                {Parameters.IdIsService, false},
                {"isMaterialsRequest", true},
                {"returnKey", "newItem"},
                {Parameters.IdBehaviour, BehaviourEditServicesOrMaterialsScreen.ReturnValue}
            };

            _isAdd = Convert.ToBoolean("True");

            Navigation.Move("RIMListScreen", dictionary);
        }

        internal void SendData_OnClick(object sender, EventArgs e)
        {
            //TODO: сохранения данных в БД.
            DBHelper.CreateNeedMatDocument(_data);
            _data = null;
            _isAdd = _isEdit = Convert.ToBoolean("False");
            DConsole.WriteLine("Data is saved");
            Navigation.Back(true);
        }

        internal void OnSwipe_Swipe(object sender, EventArgs e)
        {
        }

        internal void EditNode_OnClick(object sender, EventArgs e)
        {
            var vl = (VerticalLayout)sender;
            //TODO: Если будет починен экран редактирования то колличесто передавать отсюда.
            var dictionary = new Dictionary<string, object>
            {
                {"returnKey", "editItem"},
                {"rimId", vl.Id},
                {"priceVisible", Convert.ToBoolean("False")},
                {"value", GetNumberOfTheItem(vl.Id)},
                {"minimum", 1},
                {Parameters.IdBehaviour, BehaviourEditServicesOrMaterialsScreen.ReturnValue}
            };

            BusinessProcess.GlobalVariables[Parameters.IdIsService] = false;
            BusinessProcess.GlobalVariables["isMaterialsRequest"] = true;
            _isEdit = Convert.ToBoolean("True");
            Navigation.Move("EditServicesOrMaterialsScreen", dictionary);
        }

        private int GetNumberOfTheItem(string id)
        {
            foreach (var element in _data)
            {
                var dictionary = (Dictionary<string, object>)element;
                var elementRimId = (string)dictionary["SKU"];

                if (string.Compare(elementRimId, id, false) == 0)
                {
                    return (int)dictionary["Count"];
                }
            }
            return 0;
        }

        internal string Concat(string first, string secont)
        {
            return $"{first} {secont}";
        }
    }
}