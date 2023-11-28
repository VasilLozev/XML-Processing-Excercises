using CarDealer.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("car")]
    public class ExportCarsParts
    {
       
        [XmlAttribute("make")]
        public string make { get; set; }
        [XmlAttribute("model")]
        public string model { get; set; }
        [XmlAttribute("traveled-distance")]
        public long traveleddistance { get; set; }
        public ExportPartCar[] parts { get; set; }
    }
}
