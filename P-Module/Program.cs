using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;

//Варіант 5. Автопарк компанії
namespace Vehicle
{
    // Таблиця “Vehicles”: модель, тип транспортного засобу, пробіг, рік випуску.
    public class Vehicle
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public string Type { get; set; }
        public int Mileage { get; set; }
        public int Year { get; set; }
    }

    public class VehicleContext : DbContext
    {
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=vehicles.db");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new VehicleContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                if (!context.Vehicles.Any())
                {
                    context.Vehicles.AddRange(
                        new Vehicle { Model = "Toyota Camry", Type = "Sedan", Mileage = 120000, Year = 2017 },
                        new Vehicle { Model = "Ford F-150", Type = "Truck", Mileage = 90000, Year = 2018 },
                        new Vehicle { Model = "Honda Accord", Type = "Sedan", Mileage = 85000, Year = 2016 },
                        new Vehicle { Model = "Chevrolet Silverado", Type = "Truck", Mileage = 110000, Year = 2019 }
                    );
                    context.SaveChanges();
                }
            }

            // Завдання 1: Знайти середній пробіг для кожного типу транспортного засобу
            using (var context = new VehicleContext())
            {
                var averageMileageByType = context.Vehicles
                    .GroupBy(v => v.Type)
                    .Select(g => new
                    {
                        Type = g.Key,
                        AverageMileage = g.Average(v => v.Mileage)
                    });

                Console.WriteLine("Середній пробіг для кожного типу транспортного засобу:");
                foreach (var item in averageMileageByType)
                {
                    Console.WriteLine($"Тип: {item.Type}, Середній пробіг: {item.AverageMileage}");
                }

                // Завдання 2: Зберегти дані таблиці у XML-файл
                SaveToXml(context.Vehicles.ToList());
            }
        }

        static void SaveToXml(List<Vehicle> vehicles)
        {
            var xDocument = new XDocument(
                new XElement("Vehicles",
                    vehicles.Select
                    (v =>
                        new XElement("Vehicle",
                            new XElement("Model", v.Model),
                            new XElement("Type", v.Type),
                            new XElement("Mileage", v.Mileage),
                            new XElement("Year", v.Year)
                        )
                    )
                )
            );

            xDocument.Save("vehicles.xml");
            Console.WriteLine("Дані збережено у файл vehicles.xml");
        }
    }
}