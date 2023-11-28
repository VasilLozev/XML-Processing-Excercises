using System.Xml.Serialization;

namespace CarDealer.DTOs.Export
{
    [XmlType("customer")]
    public class ExportSalesPerCustomerDTO
    {
        [XmlElement("full-name")]
        public string FullName { get; set; }
        [XmlElement("bought-cars")]
        public int BoughtCars { get; set; }
        [XmlElement("spent-money")]
        public decimal SpentMoney { get; set; }

    }
}
