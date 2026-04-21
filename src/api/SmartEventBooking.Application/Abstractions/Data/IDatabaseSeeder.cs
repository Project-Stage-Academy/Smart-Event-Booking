using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEventBooking.Application.Abstractions.Data
{
    public interface IDatabaseSeeder
    {
        Task SeedAsync();
    }
}
