﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using BitMobile.ClientModel3;
using BitMobile.DbEngine;
using Database = BitMobile.ClientModel3.Database;

namespace Test
{
    /// <summary>
    ///     Обеспечивает работу с базой данных приложения
    /// </summary>
    /// <remarks>
    /// </remarks>
    public static partial class DBHelper
    {
        private const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        private const string EventStatusDoneName = "Done";
        private const string EventStatusCancelName = "Cancel";

        private static Database _db;

        public static void Init()
        {
            _db = new Database();
            if (_db.Exists) return;

            DConsole.WriteLine("Creating DB");
            _db.CreateFromModel();
            DConsole.WriteLine("Filling DB with demo data");

            var sql = Application.GetResourceStream("Model.main.sql");
            var reader = new StreamReader(sql);
            var queryText = reader.ReadToEnd();
            DConsole.WriteLine(queryText.Substring(0, 15));
            var query = new Query(queryText);
            query.Execute();
            _db.Commit();
        }

        public static void SaveEntity(DbEntity entity)
        {
            DConsole.WriteLine($"Saving ref@[{entity.GetTableName()}:[{entity.EntityId}]");
            entity.Save();
            _db.Commit();
        }

        public static void SaveEntities(IEnumerable entities)
        {
            foreach (DbEntity entity in entities)
            {
                entity.Save();
                DConsole.WriteLine($"Saving ref@[{entity.GetTableName()}:[{entity.EntityId}]");
            }
            _db.Commit();
        }
    }
}