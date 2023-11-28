using CarDealer.Models;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("part")]
    public class ExportPartCar
    {
        [XmlAttribute("name")]
        public string name { get;set; }
        [XmlAttribute("price")]
        public decimal price { get; set; }
    }
}