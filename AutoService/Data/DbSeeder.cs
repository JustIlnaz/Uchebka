using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoService.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Roles.Any() && db.Users.Any())
            return;

        var roles = new List<Role>
        {
            new() { Name = "Администратор" },
            new() { Name = "Механик" },
            new() { Name = "Руководитель" },
            new() { Name = "Клиент" }
        };
        db.Roles.AddRange(roles);
        db.SaveChanges();

        Role adminRole = roles[0], mechanicRole = roles[1], managerRole = roles[2], clientRole = roles[3];

        var users = new List<User>
        {
            new() { Username = "admin",   Password = "admin",   FullName = "Иванов Иван Иванович",      PhoneNumber = "+7 (900) 111-11-11", Email = "admin@autoservice.ru",  RoleId = adminRole.Id,    RoleName = adminRole.Name },
            new() { Username = "manager", Password = "manager", FullName = "Смирнова Анна Петровна",    PhoneNumber = "+7 (900) 222-22-22", Email = "manager@autoservice.ru", RoleId = managerRole.Id,  RoleName = managerRole.Name },
            new() { Username = "mechanic1", Password = "1111",  FullName = "Петров Пётр Сергеевич",     PhoneNumber = "+7 (900) 333-33-33", Email = "petrov@autoservice.ru",  RoleId = mechanicRole.Id, RoleName = mechanicRole.Name },
            new() { Username = "mechanic2", Password = "1111",  FullName = "Сидоров Алексей Николаевич", PhoneNumber = "+7 (900) 444-44-44", Email = "sidorov@autoservice.ru", RoleId = mechanicRole.Id, RoleName = mechanicRole.Name },
            new() { Username = "mechanic3", Password = "1111",  FullName = "Кузнецов Дмитрий Олегович", PhoneNumber = "+7 (900) 555-55-55", Email = "kuznecov@autoservice.ru", RoleId = mechanicRole.Id, RoleName = mechanicRole.Name },
            new() { Username = "client1", Password = "1111",    FullName = "Васильев Олег Андреевич",   PhoneNumber = "+7 (900) 666-66-66", Email = "vasiliev@mail.ru",       RoleId = clientRole.Id,   RoleName = clientRole.Name },
        };
        db.Users.AddRange(users);
        db.SaveChanges();

        User adminUser = users[0], managerUser = users[1];
        User mechUser1 = users[2], mechUser2 = users[3], mechUser3 = users[4];
        User clientUser = users[5];

        var departments = new List<Department>
        {
            new() { Name = "Отдел технического обслуживания" },
            new() { Name = "Кузовной отдел" },
            new() { Name = "Отдел диагностики" }
        };
        db.Departments.AddRange(departments);
        db.SaveChanges();

        var specs = new List<Specialization>
        {
            new() { Name = "Моторист" },
            new() { Name = "Диагност" },
            new() { Name = "Шиномонтажник" },
            new() { Name = "Кузовщик" },
            new() { Name = "Электрик" }
        };
        db.Specializations.AddRange(specs);
        db.SaveChanges();

        var mechanics = new List<Mechanic>
        {
            new() { UserId = mechUser1.Id, DepartmentId = departments[0].Id, SpecializationId = specs[0].Id },
            new() { UserId = mechUser2.Id, DepartmentId = departments[2].Id, SpecializationId = specs[1].Id },
            new() { UserId = mechUser3.Id, DepartmentId = departments[0].Id, SpecializationId = specs[2].Id }
        };
        db.Mechanics.AddRange(mechanics);
        db.SaveChanges();

        Mechanic mech1 = mechanics[0], mech2 = mechanics[1], mech3 = mechanics[2];

        var services = new List<Service>
        {
            new() { Name = "Замена масла",               Description = "Замена моторного масла и масляного фильтра", BasePrice = 2500m,  DurationMinutes = 45 },
            new() { Name = "Замена тормозных колодок",   Description = "Снятие и установка новых тормозных колодок", BasePrice = 3000m,  DurationMinutes = 60 },
            new() { Name = "Компьютерная диагностика",   Description = "Диагностика электронных систем автомобиля",  BasePrice = 1500m,  DurationMinutes = 30 },
            new() { Name = "Ремонт двигателя",           Description = "Капитальный или текущий ремонт ДВС",          BasePrice = 20000m, DurationMinutes = 480 },
            new() { Name = "Балансировка колёс",         Description = "Балансировка 4 колёс на стенде",              BasePrice = 1800m,  DurationMinutes = 40 },
            new() { Name = "Шиномонтаж",                 Description = "Сезонная замена шин",                         BasePrice = 2000m,  DurationMinutes = 50 },
            new() { Name = "Замена свечей зажигания",    Description = "Замена комплекта свечей зажигания",           BasePrice = 1200m,  DurationMinutes = 30 },
            new() { Name = "Ремонт подвески",            Description = "Диагностика и ремонт элементов подвески",     BasePrice = 8000m,  DurationMinutes = 180 },
            new() { Name = "Замена аккумулятора",        Description = "Подбор и установка нового АКБ",               BasePrice = 1000m,  DurationMinutes = 20 },
            new() { Name = "Кузовной ремонт",            Description = "Рихтовка и покраска кузовных элементов",      BasePrice = 15000m, DurationMinutes = 600 }
        };
        db.Services.AddRange(services);
        db.SaveChanges();

        db.MechanicServices.AddRange(new[]
        {
            new MechanicService { MechanicId = mech1.Id, ServiceId = services[0].Id },
            new MechanicService { MechanicId = mech1.Id, ServiceId = services[3].Id },
            new MechanicService { MechanicId = mech1.Id, ServiceId = services[6].Id },
            new MechanicService { MechanicId = mech1.Id, ServiceId = services[7].Id },

            new MechanicService { MechanicId = mech2.Id, ServiceId = services[2].Id },
            new MechanicService { MechanicId = mech2.Id, ServiceId = services[8].Id },

            new MechanicService { MechanicId = mech3.Id, ServiceId = services[1].Id },
            new MechanicService { MechanicId = mech3.Id, ServiceId = services[4].Id },
            new MechanicService { MechanicId = mech3.Id, ServiceId = services[5].Id },
            new MechanicService { MechanicId = mech3.Id, ServiceId = services[9].Id }
        });
        db.SaveChanges();

        var bays = new List<RepairBay>
        {
            new() { Name = "Подъёмник №1",        Description = "Двухстоечный подъёмник" },
            new() { Name = "Подъёмник №2",        Description = "Четырёхстоечный подъёмник" },
            new() { Name = "Диагностический пост", Description = "Пост компьютерной диагностики" },
            new() { Name = "Шиномонтажная зона",  Description = "Стенд шиномонтажа и балансировки" },
            new() { Name = "Кузовной участок",    Description = "Зона кузовного ремонта и покраски" }
        };
        db.RepairBays.AddRange(bays);
        db.SaveChanges();

        var today = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc);
        db.MechanicSchedules.AddRange(new[]
        {
            new MechanicSchedule { MechanicId = mech1.Id, Date = today,                    StartTime = new TimeSpan(9, 0, 0),  EndTime = new TimeSpan(18, 0, 0) },
            new MechanicSchedule { MechanicId = mech1.Id, Date = today.AddDays(1),         StartTime = new TimeSpan(9, 0, 0),  EndTime = new TimeSpan(18, 0, 0) },
            new MechanicSchedule { MechanicId = mech2.Id, Date = today,                    StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(19, 0, 0) },
            new MechanicSchedule { MechanicId = mech2.Id, Date = today.AddDays(2),         StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(19, 0, 0) },
            new MechanicSchedule { MechanicId = mech3.Id, Date = today.AddDays(1),         StartTime = new TimeSpan(8, 0, 0),  EndTime = new TimeSpan(17, 0, 0) },
            new MechanicSchedule { MechanicId = mech3.Id, Date = today.AddDays(3),         StartTime = new TimeSpan(8, 0, 0),  EndTime = new TimeSpan(17, 0, 0) }
        });
        db.SaveChanges();

        var makes = new List<CarMake>
        {
            new() { Name = "Toyota" },
            new() { Name = "BMW" },
            new() { Name = "Lada" },
            new() { Name = "Hyundai" },
            new() { Name = "Volkswagen" }
        };
        db.CarMakes.AddRange(makes);
        db.SaveChanges();

        var categories = new List<CarCategory>
        {
            new() { Name = "Седан" },
            new() { Name = "Хэтчбек" },
            new() { Name = "Внедорожник" },
            new() { Name = "Универсал" }
        };
        db.CarCategories.AddRange(categories);
        db.SaveChanges();

        var clients = new List<Client>
        {
            new() { FullName = "Васильев Олег Андреевич",   Phone = "+7 (911) 100-10-01", Email = "vasiliev@mail.ru" },
            new() { FullName = "Николаева Мария Игоревна",  Phone = "+7 (911) 100-10-02", Email = "nikolaeva@gmail.com" },
            new() { FullName = "Громов Артём Викторович",   Phone = "+7 (911) 100-10-03", Email = "gromov@yandex.ru" },
            new() { FullName = "Фёдорова Елена Сергеевна",  Phone = "+7 (911) 100-10-04", Email = "fedorova@mail.ru" },
            new() { FullName = "Белов Сергей Дмитриевич",   Phone = "+7 (911) 100-10-05", Email = "belov@gmail.com" }
        };
        db.Clients.AddRange(clients);
        db.SaveChanges();

        Client cl1 = clients[0], cl2 = clients[1], cl3 = clients[2], cl4 = clients[3], cl5 = clients[4];

        var cars = new List<Car>
        {
            new() { ClientId = cl1.Id, MakeId = makes[0].Id, CategoryId = categories[0].Id, Model = "Camry",      Year = 2018, Vin = "XW7BF4FK90S012345", RegistrationNumber = "А123БВ78",  Mileage = 85000 },
            new() { ClientId = cl1.Id, MakeId = makes[2].Id, CategoryId = categories[1].Id, Model = "Vesta",      Year = 2021, Vin = "XTAFLR46B1M067890", RegistrationNumber = "В456ГД178", Mileage = 30000 },
            new() { ClientId = cl2.Id, MakeId = makes[1].Id, CategoryId = categories[2].Id, Model = "X5",         Year = 2019, Vin = "WBACR410XKL123456", RegistrationNumber = "Е789ЖЗ78",  Mileage = 60000 },
            new() { ClientId = cl3.Id, MakeId = makes[3].Id, CategoryId = categories[0].Id, Model = "Solaris",    Year = 2020, Vin = "Z94CB41ABKR234567", RegistrationNumber = "К012ИК178", Mileage = 45000 },
            new() { ClientId = cl4.Id, MakeId = makes[4].Id, CategoryId = categories[3].Id, Model = "Passat",     Year = 2017, Vin = "WVWZZZ3CZHE345678", RegistrationNumber = "М345ЛО78",  Mileage = 120000 },
            new() { ClientId = cl5.Id, MakeId = makes[0].Id, CategoryId = categories[2].Id, Model = "RAV4",       Year = 2022, Vin = "JTMRFREV20J456789", RegistrationNumber = "Н678ПР178", Mileage = 15000 }
        };
        db.Cars.AddRange(cars);
        db.SaveChanges();

        Car car1 = cars[0], car2 = cars[1], car3 = cars[2], car4 = cars[3], car5 = cars[4], car6 = cars[5];

        var suppliers = new List<Supplier>
        {
            new() { Name = "ООО «АвтоДеталь»",   ContactInfo = "+7 (812) 200-00-01, avtodetal@mail.ru" },
            new() { Name = "ЗАО «МехЗапчасть»",  ContactInfo = "+7 (812) 200-00-02, mech@zapchast.ru" },
            new() { Name = "ИП Смирнов",          ContactInfo = "+7 (812) 200-00-03, smirnov@parts.ru" }
        };
        db.Suppliers.AddRange(suppliers);
        db.SaveChanges();

        Supplier sup1 = suppliers[0], sup2 = suppliers[1], sup3 = suppliers[2];

        var parts = new List<Part>
        {
            new() { SupplierId = sup1.Id, Sku = "OIL-5W40-4L",   PurchasePrice = 1800m,  SalePrice = 2800m,  QuantityInStock = 20, MinQuantity = 5 },
            new() { SupplierId = sup1.Id, Sku = "OIL-FILTER-01", PurchasePrice = 300m,   SalePrice = 600m,   QuantityInStock = 40, MinQuantity = 10 },
            new() { SupplierId = sup2.Id, Sku = "BRK-PAD-FRONT", PurchasePrice = 1500m,  SalePrice = 2600m,  QuantityInStock = 15, MinQuantity = 4 },
            new() { SupplierId = sup2.Id, Sku = "BRK-PAD-REAR",  PurchasePrice = 1200m,  SalePrice = 2100m,  QuantityInStock = 3,  MinQuantity = 4 },
            new() { SupplierId = sup3.Id, Sku = "SPARK-PLUG-4",  PurchasePrice = 800m,   SalePrice = 1400m,  QuantityInStock = 25, MinQuantity = 8 },
            new() { SupplierId = sup1.Id, Sku = "WHEEL-BAL-WGT", PurchasePrice = 200m,   SalePrice = 400m,   QuantityInStock = 60, MinQuantity = 20 },
            new() { SupplierId = sup2.Id, Sku = "BATTERY-60AH",  PurchasePrice = 5000m,  SalePrice = 7500m,  QuantityInStock = 8,  MinQuantity = 2 },
            new() { SupplierId = sup3.Id, Sku = "SHOCK-FRONT",   PurchasePrice = 3200m,  SalePrice = 5000m,  QuantityInStock = 6,  MinQuantity = 2 },
            new() { SupplierId = sup1.Id, Sku = "TIRE-R16",      PurchasePrice = 6000m,  SalePrice = 8500m,  QuantityInStock = 16, MinQuantity = 4 },
            new() { SupplierId = sup2.Id, Sku = "AIR-FILTER-02", PurchasePrice = 500m,   SalePrice = 900m,   QuantityInStock = 30, MinQuantity = 10 }
        };
        db.Parts.AddRange(parts);
        db.SaveChanges();

        var now = DateTime.UtcNow;
        var appointments = new List<Appointment>
        {
            new() { ClientId = cl1.Id, CarId = car1.Id, ServiceId = services[0].Id, ScheduledAt = now.AddDays(-10), MechanicId = mech1.Id, RepairBayId = bays[0].Id, Status = "Выполнена" },
            new() { ClientId = cl2.Id, CarId = car3.Id, ServiceId = services[2].Id, ScheduledAt = now.AddDays(-7),  MechanicId = mech2.Id, RepairBayId = bays[2].Id, Status = "Выполнена" },
            new() { ClientId = cl3.Id, CarId = car4.Id, ServiceId = services[5].Id, ScheduledAt = now.AddDays(-5),  MechanicId = mech3.Id, RepairBayId = bays[3].Id, Status = "Выполнена" },
            new() { ClientId = cl4.Id, CarId = car5.Id, ServiceId = services[3].Id, ScheduledAt = now.AddDays(-3),  MechanicId = mech1.Id, RepairBayId = bays[1].Id, Status = "Подтверждена" },
            new() { ClientId = cl5.Id, CarId = car6.Id, ServiceId = services[4].Id, ScheduledAt = now.AddDays(-1),  MechanicId = mech3.Id, RepairBayId = bays[3].Id, Status = "Запланирована" },
            new() { ClientId = cl1.Id, CarId = car2.Id, ServiceId = services[6].Id, ScheduledAt = now.AddDays(1),   MechanicId = mech1.Id, RepairBayId = bays[0].Id, Status = "Запланирована" },
            new() { ClientId = cl2.Id, CarId = car3.Id, ServiceId = services[1].Id, ScheduledAt = now.AddDays(2),   MechanicId = mech3.Id, RepairBayId = bays[0].Id, Status = "Подтверждена" },
            new() { ClientId = cl3.Id, CarId = car4.Id, ServiceId = services[8].Id, ScheduledAt = now.AddDays(-2),  MechanicId = mech2.Id, RepairBayId = bays[2].Id, Status = "Отменена" },
            new() { ClientId = cl4.Id, CarId = car5.Id, ServiceId = services[9].Id, ScheduledAt = now.AddDays(4),   MechanicId = mech1.Id, RepairBayId = bays[4].Id, Status = "Запланирована" },
            new() { ClientId = cl5.Id, CarId = car6.Id, ServiceId = services[7].Id, ScheduledAt = now.AddDays(-8),  MechanicId = mech1.Id, RepairBayId = bays[1].Id, Status = "Выполнена" }
        };
        db.Appointments.AddRange(appointments);
        db.SaveChanges();

        var workOrders = new List<WorkOrder>
        {
            new() { AppointmentId = appointments[0].Id, MechanicId = mech1.Id, Status = "Завершён", TotalCost = 0 },
            new() { AppointmentId = appointments[1].Id, MechanicId = mech2.Id, Status = "Завершён", TotalCost = 0 },
            new() { AppointmentId = appointments[2].Id, MechanicId = mech3.Id, Status = "Завершён", TotalCost = 0 },
            new() { AppointmentId = appointments[4].Id, MechanicId = mech3.Id, Status = "В работе", TotalCost = 0 },
            new() { AppointmentId = appointments[9].Id, MechanicId = mech1.Id, Status = "Завершён", TotalCost = 0 }
        };
        db.WorkOrders.AddRange(workOrders);
        db.SaveChanges();

        WorkOrder wo1 = workOrders[0], wo2 = workOrders[1], wo3 = workOrders[2], wo4 = workOrders[3], wo5 = workOrders[4];

        var woServices = new List<WorkOrderService>
        {
            new() { WorkOrderId = wo1.Id, ServiceId = services[0].Id, Quantity = 1, Price = services[0].BasePrice },
            new() { WorkOrderId = wo1.Id, ServiceId = services[6].Id, Quantity = 1, Price = services[6].BasePrice },

            new() { WorkOrderId = wo2.Id, ServiceId = services[2].Id, Quantity = 1, Price = services[2].BasePrice },

            new() { WorkOrderId = wo3.Id, ServiceId = services[5].Id, Quantity = 1, Price = services[5].BasePrice },
            new() { WorkOrderId = wo3.Id, ServiceId = services[4].Id, Quantity = 1, Price = services[4].BasePrice },

            new() { WorkOrderId = wo4.Id, ServiceId = services[4].Id, Quantity = 1, Price = services[4].BasePrice },

            new() { WorkOrderId = wo5.Id, ServiceId = services[7].Id, Quantity = 1, Price = services[7].BasePrice },
            new() { WorkOrderId = wo5.Id, ServiceId = services[2].Id, Quantity = 1, Price = services[2].BasePrice }
        };
        db.WorkOrderServices.AddRange(woServices);
        db.SaveChanges();

        var woParts = new List<WorkOrderPart>
        {
            new() { WorkOrderId = wo1.Id, PartId = parts[0].Id, Quantity = 1, Price = parts[0].SalePrice },
            new() { WorkOrderId = wo1.Id, PartId = parts[1].Id, Quantity = 1, Price = parts[1].SalePrice },
            new() { WorkOrderId = wo1.Id, PartId = parts[4].Id, Quantity = 4, Price = parts[4].SalePrice },

            new() { WorkOrderId = wo2.Id, PartId = parts[9].Id, Quantity = 1, Price = parts[9].SalePrice },

            new() { WorkOrderId = wo3.Id, PartId = parts[8].Id, Quantity = 4, Price = parts[8].SalePrice },
            new() { WorkOrderId = wo3.Id, PartId = parts[5].Id, Quantity = 4, Price = parts[5].SalePrice },

            new() { WorkOrderId = wo4.Id, PartId = parts[5].Id, Quantity = 4, Price = parts[5].SalePrice },

            new() { WorkOrderId = wo5.Id, PartId = parts[7].Id, Quantity = 2, Price = parts[7].SalePrice },
            new() { WorkOrderId = wo5.Id, PartId = parts[3].Id, Quantity = 1, Price = parts[3].SalePrice }
        };
        db.WorkOrderParts.AddRange(woParts);
        db.SaveChanges();

        foreach (var wo in workOrders)
        {
            var sSum = db.WorkOrderServices.Where(ws => ws.WorkOrderId == wo.Id).Sum(ws => ws.Price * ws.Quantity);
            var pSum = db.WorkOrderParts.Where(wp => wp.WorkOrderId == wo.Id).Sum(wp => wp.Price * wp.Quantity);
            wo.TotalCost = sSum + pSum;
        }
        db.SaveChanges();

        var invoices = new List<Invoice>
        {
            new() { WorkOrderId = wo1.Id, CreatedAt = now.AddDays(-9),  AmountBeforeDiscount = wo1.TotalCost, Discount = 500m, Tax = Math.Round(wo1.TotalCost * 0.20m, 2), TotalAmount = wo1.TotalCost - 500m + Math.Round(wo1.TotalCost * 0.20m, 2), Status = "Оплачен" },
            new() { WorkOrderId = wo2.Id, CreatedAt = now.AddDays(-6),  AmountBeforeDiscount = wo2.TotalCost, Discount = 0m,   Tax = Math.Round(wo2.TotalCost * 0.20m, 2), TotalAmount = wo2.TotalCost + Math.Round(wo2.TotalCost * 0.20m, 2), Status = "Частично оплачен" },
            new() { WorkOrderId = wo3.Id, CreatedAt = now.AddDays(-4),  AmountBeforeDiscount = wo3.TotalCost, Discount = 200m, Tax = Math.Round(wo3.TotalCost * 0.20m, 2), TotalAmount = wo3.TotalCost - 200m + Math.Round(wo3.TotalCost * 0.20m, 2), Status = "Оплачен" },
            new() { WorkOrderId = wo4.Id, CreatedAt = now,             AmountBeforeDiscount = wo4.TotalCost, Discount = 0m,   Tax = Math.Round(wo4.TotalCost * 0.20m, 2), TotalAmount = wo4.TotalCost + Math.Round(wo4.TotalCost * 0.20m, 2), Status = "Не оплачен" },
            new() { WorkOrderId = wo5.Id, CreatedAt = now.AddDays(-7), AmountBeforeDiscount = wo5.TotalCost, Discount = 0m,   Tax = Math.Round(wo5.TotalCost * 0.20m, 2), TotalAmount = wo5.TotalCost + Math.Round(wo5.TotalCost * 0.20m, 2), Status = "Оплачен" }
        };
        db.Invoices.AddRange(invoices);
        db.SaveChanges();

        Invoice inv1 = invoices[0], inv2 = invoices[1], inv3 = invoices[2], inv4 = invoices[3], inv5 = invoices[4];

        db.Payments.AddRange(new[]
        {
            new Payment { InvoiceId = inv1.Id, Date = now.AddDays(-9),  Amount = inv1.TotalAmount,               Method = "Карта",   Status = "Успешно", TransactionNumber = "TXN-100001" },
            new Payment { InvoiceId = inv2.Id, Date = now.AddDays(-5),  Amount = inv2.TotalAmount / 2,           Method = "Наличные", Status = "Успешно", TransactionNumber = "TXN-100002" },
            new Payment { InvoiceId = inv2.Id, Date = now.AddDays(-3),  Amount = inv2.TotalAmount / 4,           Method = "Карта",   Status = "Успешно", TransactionNumber = "TXN-100003" },
            new Payment { InvoiceId = inv3.Id, Date = now.AddDays(-4),  Amount = inv3.TotalAmount,               Method = "Карта",   Status = "Успешно", TransactionNumber = "TXN-100004" },
            new Payment { InvoiceId = inv5.Id, Date = now.AddDays(-7),  Amount = inv5.TotalAmount * 0.6m,        Method = "Перевод", Status = "Успешно", TransactionNumber = "TXN-100005" },
            new Payment { InvoiceId = inv5.Id, Date = now.AddDays(-6),  Amount = inv5.TotalAmount * 0.4m,        Method = "Наличные", Status = "Успешно", TransactionNumber = "TXN-100006" }
        });
        db.SaveChanges();

        db.InspectionResults.AddRange(new[]
        {
            new InspectionResult { WorkOrderId = wo1.Id, EngineState = "Хорошее",     BrakesState = "Хорошее",      SuspensionState = "Хорошее",          ElectricalState = "Хорошее",    Recommendations = "Следующая замена масла через 10 000 км." },
            new InspectionResult { WorkOrderId = wo2.Id, EngineState = "Хорошее",     BrakesState = "Удовлетворит.", SuspensionState = "Хорошее",          ElectricalState = "Ошибки ЭБУ",  Recommendations = "Рекомендуется замена воздушного фильтра." },
            new InspectionResult { WorkOrderId = wo3.Id, EngineState = "Хорошее",     BrakesState = "Хорошее",      SuspensionState = "Хорошее",          ElectricalState = "Хорошее",    Recommendations = "Шины заменены на зимние." },
            new InspectionResult { WorkOrderId = wo4.Id, EngineState = "Не проверен", BrakesState = "Не проверено", SuspensionState = "Требует внимания", ElectricalState = "Не проверено", Recommendations = "Требуется диагностика подвески." },
            new InspectionResult { WorkOrderId = wo5.Id, EngineState = "Хорошее",     BrakesState = "Износ колодок", SuspensionState = "Износ амортизаторов", ElectricalState = "Хорошее", Recommendations = "Рекомендуется замена задних тормозных колодок." }
        });
        db.SaveChanges();

        db.Reviews.AddRange(new[]
        {
            new Review { ClientId = cl1.Id, WorkOrderId = wo1.Id, Rating = 5, Comment = "Отличный сервис, всё сделали быстро и качественно!", CreatedAt = now.AddDays(-8) },
            new Review { ClientId = cl2.Id, WorkOrderId = wo2.Id, Rating = 4, Comment = "Хорошая диагностика, но пришлось немного подождать.", CreatedAt = now.AddDays(-5) },
            new Review { ClientId = cl3.Id, WorkOrderId = wo3.Id, Rating = 5, Comment = "Шиномонтаж занял всего 40 минут. Рекомендую!",        CreatedAt = now.AddDays(-3) },
            new Review { ClientId = cl5.Id, WorkOrderId = wo5.Id, Rating = 4, Comment = "Подвеску починили хорошо, но цена выше ожиданий.",   CreatedAt = now.AddDays(-6) }
        });
        db.SaveChanges();

        db.Set<Login>().AddRange(new[]
        {
            new Login { LoginName = "admin",     Password = "admin",   UserId = adminUser.Id },
            new Login { LoginName = "manager",   Password = "manager", UserId = managerUser.Id },
            new Login { LoginName = "mechanic1", Password = "1111",    UserId = mechUser1.Id },
            new Login { LoginName = "mechanic2", Password = "1111",    UserId = mechUser2.Id },
            new Login { LoginName = "mechanic3", Password = "1111",    UserId = mechUser3.Id },
            new Login { LoginName = "client1",   Password = "1111",    UserId = clientUser.Id }
        });
        db.SaveChanges();
    }
}
