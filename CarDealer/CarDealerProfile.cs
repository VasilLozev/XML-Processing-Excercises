using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<ImportSupplierDTO, Supplier>();
            CreateMap<ImportPartsDTO, Models.Part>();
            CreateMap<ImportCarsDTO, Car>();
            CreateMap<ImportCustomersDTO, Customer>();
            CreateMap<ImportSalesDTO, Sale>();
            CreateMap<Car, ExportCarsWithDistance>();
            CreateMap<Car, ExportBMWCars>();
            CreateMap<ExportBMWCars, ExportBMWCars>();
            CreateMap<Supplier, ExportLocalSuppliersDTO>();
            CreateMap<ExportLocalSuppliersDTO, ExportLocalSuppliersDTO>();
            CreateMap<ExportCarsParts, ExportCarsParts>();
            CreateMap<Part, Part>();
            CreateMap<ExportCarsParts[], ExportCarsParts[]>();
            CreateMap<ICollection<PartCar>, ExportPartCar>();
            CreateMap<ExportSalesWithDiscount, ExportSalesWithDiscount>();
        }
    }
}
