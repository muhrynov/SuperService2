using System;
using BitMobile.DbEngine;

namespace Test.Document
{
    public class Tender : DbEntity
    {
        public DbRef Id { get; set; }
        public bool DeletionMark { get; set; }
        public string Number { get; set; }
        public DbRef Client { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public DbRef ActivityType { get; set; }
        public string Description { get; set; }
        public DateTime DeliveryTime { get; set; }
        public string Marketplace { get; set; }
        public decimal Sum { get; set; }
}


}
    