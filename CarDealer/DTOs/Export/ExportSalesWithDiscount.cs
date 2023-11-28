using CarDealer.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("sale")]
    public class ExportSalesWithDiscount
    {
        public ExportCarsParts car { get;set; }

        [XmlElement("discount")]
        public decimal discount { get; set; }

        [XmlElement("customer-name")]
        public string customername { get; set; }
        [XmlElement("price")]
        public decimal price { get; set; }
        
        [XmlElement("price-with-discount")]
        [Precision(0,2)]
        public decimal pricewithdiscount { get; set; }
    }
}
