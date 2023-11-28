using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            // 9. Import Suppliers
            //string inputSuppliersXML = File.ReadAllText("../../../Datasets/suppliers.xml");
            //Console.WriteLine(ImportSuppliers(context,inputSuppliersXML));

            // 10.Import Parts
            //string inputPartsXML = File.ReadAllText("../../../Datasets/parts.xml");
            //Console.WriteLine(ImportParts(context,inputPartsXML));

            // 11. Import Cars
            //string inputCarsXML = File.ReadAllText("../../../Datasets/cars.xml");
            //Console.WriteLine(ImportCars(context, inputCarsXML));

            // 12. Import Customers
            //string inputCustomersXML = File.ReadAllText("../../../Datasets/customers.xml");
            //Console.WriteLine(ImportCustomers(context, inputCustomersXML));

            // 13. Import Sales
            //string inputSalesXML = File.ReadAllText("../../../Datasets/sales.xml");
            //Console.WriteLine(ImportSales(context, inputSalesXML));

            //14. Export Cars With Distance
            //Console.WriteLine(GetCarsWithDistance(context));

            //15.Export Cars from Make BMW
            //Console.WriteLine(GetCarsFromMakeBmw(context));

            //16. Export Local Suppliers
            //Console.WriteLine(GetLocalSuppliers(context));

            //17. Export Cars with Their List of Parts
            //Console.WriteLine(GetCarsWithTheirListOfParts(context));

            // 18. Export Total Sales by Customer
            // Console.WriteLine(GetTotalSalesByCustomer(context));

            //19.Export Sales with Applied Discount
            Console.WriteLine(GetSalesWithAppliedDiscount(context));

        }

        private static Mapper GetMapper()
        {
            var cfg = new MapperConfiguration(c => c.AddProfile<CarDealerProfile>());
            return new Mapper(cfg);
        }

        //9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            // 1. Create xml serializer
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDTO[]),
                new XmlRootAttribute("Suppliers"));

            // 2.
            using var reader = new StringReader(inputXml);
            ImportSupplierDTO[] importSupplierDTOs = (ImportSupplierDTO[])xmlSerializer.Deserialize(reader);

            // 3.
            var mapper = GetMapper();
            Supplier[] suppliers = mapper.Map<Supplier[]>(importSupplierDTOs);

            // 4. Add to Ef context
            context.AddRange(suppliers);

            // 5. Commit changes to DB
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }
        // 10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportPartsDTO[]), new XmlRootAttribute("Parts"));

            using StringReader inputReader = new StringReader(inputXml);
            ImportPartsDTO[] importPartsDTOs = (ImportPartsDTO[])xmlSerializer.Deserialize(inputReader);

            var supplierIds = context.Suppliers
                .Select(s => s.Id)
                .ToArray();

            var mapper = GetMapper();

            Models.Part[] parts = mapper.Map<Models.Part[]>(importPartsDTOs.Where(p => supplierIds.Contains(p.SupplierId)));

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }
        // 11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportCarsDTO[]), new XmlRootAttribute("Cars"));

            using StringReader stringReader = new StringReader(inputXml);

            ImportCarsDTO[] importCarsDTOs = (ImportCarsDTO[])xmlSerializer.Deserialize(stringReader);

            var mapper = GetMapper();
            List<Car> cars = new List<Car>();

            foreach (var carDTO in importCarsDTOs)
            {
                Car car = mapper.Map<Car>(carDTO);

                int[] carPartIds = carDTO.PartsIds
                    .Select(p => p.Id)
                    .Distinct()
                    .ToArray();

                var carParts = new List<PartCar>();

                foreach (var id in carPartIds)
                {
                    carParts.Add(new PartCar
                    {
                        Car = car,
                        PartId = id
                    });
                }

                car.PartsCars = carParts;
                cars.Add(car);
            }

            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }
        // 12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportCustomersDTO[]), new XmlRootAttribute("Customers"));

            using StringReader stringReader = new StringReader(inputXml);

            ImportCustomersDTO[] importCustomersDTOs = (ImportCustomersDTO[])xmlSerializer.Deserialize(stringReader);

            var mapper = GetMapper();

            Customer[] customers = mapper.Map<Customer[]>(importCustomersDTOs);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }
        // 13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ImportSalesDTO[]), new XmlRootAttribute("Sales"));

            using StringReader stringReader = new StringReader(inputXml);

            ImportSalesDTO[] importSalesDTOs = (ImportSalesDTO[])xmlSerializer.Deserialize(stringReader);

            var mapper = GetMapper();

            int[] carIds = context.Cars
                .Select(car => car.Id).ToArray();

            Sale[] sales = mapper.Map<Sale[]>(importSalesDTOs)
                .Where(s => carIds.Contains(s.CarId)).ToArray();

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";
        }
        // 14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var mapper = GetMapper();
            var carsWithDistance = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                    .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarsWithDistance>(mapper.ConfigurationProvider);


            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(ExportCarsWithDistance), new XmlRootAttribute("cars"));

            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, carsWithDistance, xsn);
            }

            return stringBuilder.ToString().TrimEnd();
        }
        //15. Export Cars from Make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var mapper = GetMapper();

            var BMWcars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new ExportBMWCars
                {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ProjectTo<ExportBMWCars>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer xmlSerializer =
             new XmlSerializer(typeof(ExportBMWCars[]), new XmlRootAttribute("cars"));

            StringBuilder stringBuilder = new();
            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, BMWcars, xsn);
                return stringBuilder.ToString().TrimEnd();
            }
        }
        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var mapper = GetMapper();
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter != true)
                .Select(s => new ExportLocalSuppliersDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                }).ProjectTo<ExportLocalSuppliersDTO>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer xmlSerializer =
             new XmlSerializer(typeof(ExportLocalSuppliersDTO[]), new XmlRootAttribute("suppliers"));

            StringBuilder stringBuilder = new();
            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, suppliers, xsn);
                return stringBuilder.ToString().TrimEnd();
            }
        }

        //17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var mapper = GetMapper();

            var cars = context.Cars
                .OrderByDescending(s => s.TraveledDistance)
                .ThenBy(s => s.Model)
                .Take(5)
                .Select(c => new ExportCarsParts
                {
                    make = c.Make,
                    model = c.Model,
                    traveleddistance = c.TraveledDistance,
                    parts = c.PartsCars.Select(pc => new ExportPartCar { name = pc.Part.Name, price = pc.Part.Price }
                    ).OrderByDescending(c => c.price).ToArray()
                }
                )
                .ProjectTo<ExportCarsParts>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer xmlSerializer =
             new XmlSerializer(typeof(ExportCarsParts[]), new XmlRootAttribute("cars"));

            StringBuilder stringBuilder = new();
            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, cars, xsn);
                return stringBuilder.ToString().TrimEnd();
            }
        }

        //// 18. Export Total Sales by Customer
        //private static string GetTotalSalesByCustomer(CarDealerContext context)
        //    {
        //        var totalSales = context.Customers
        //            .Where(c => c.Sales.Any())
        //            .Select(c => new ExportSalesPerCustomerDTO
        //            {
        //                FullName = c.Name,
        //                BoughtCars = c.Sales.Count,
        //                SpentMoney = c.Sales.Sum(s =>
        //                    s.Car.PartsCars.Sum(pc =>
        //                        c.IsYoungDriver ? pc.Part.Price * 0.95m : pc.Part.Price - s.Discount
        //                    )),
        //            })
        //            .OrderByDescending(x => x.SpentMoney)
        //            .ToArray();

        //      //  return SerializeToXml<ExportSalesPerCustomerDTO[]>(totalSales, "customer");
        //}

        /// <summary>
        /// Generic method to serialize DTOs to XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dto"></param>
        /// <param name="xmlRootAttribute"></param>
        /// <returns></returns>
        /*public static string SerializeToXml<T>(T dto, string xmlRootAttribute)
        {
            XmlSerializer xmlSerializer =
                new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttribute));

            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);
                try
                {
                    xmlSerializer.Serialize(stringWriter, dto, xmlSerializerNamespaces);
                }
                catch
                {

                    throw;
                }
            }

            return stringBuilder.ToString();
        }*/
        //19. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var mapper = GetMapper();

            var sales = context.Sales
                .Select(x => new ExportSalesWithDiscount
                {
                    car = new ExportCarsParts { make = x.Car.Make, model = x.Car.Model, traveleddistance = x.Car.TraveledDistance },
                    discount = x.Discount,
                    customername = x.Customer.Name,
                    price = x.Car.PartsCars.Sum(x => x.Part.Price),
                    pricewithdiscount = x.Customer.IsYoungDriver ? 
                    x.Car.PartsCars.Sum(x => x.Part.Price) - (x.Car.PartsCars.Sum(x => x.Part.Price) * ((x.Discount) / 100)) :
                    x.Car.PartsCars.Sum(x => x.Part.Price) - (x.Car.PartsCars.Sum(x => x.Part.Price) * (x.Discount + 5/ 100))
                })
                .ProjectTo<ExportSalesWithDiscount>(mapper.ConfigurationProvider)
                .ToArray();

            XmlSerializer xmlSerializer =
             new XmlSerializer(typeof(ExportSalesWithDiscount[]), new XmlRootAttribute("sales"));

            StringBuilder stringBuilder = new();
            var xsn = new XmlSerializerNamespaces();
            xsn.Add(string.Empty, string.Empty);

            using (StringWriter sw = new StringWriter(stringBuilder))
            {
                xmlSerializer.Serialize(sw, sales, xsn);
                return stringBuilder.ToString().Trim();
            }
        }
    }
}