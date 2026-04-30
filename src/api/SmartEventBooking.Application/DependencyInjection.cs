using Microsoft.Extensions.DependencyInjection;
using SmartEventBooking.Application.Abstractions.Services;
using SmartEventBooking.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartEventBooking.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register business Services later here
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<ICategoryService, CategoryService>();

            return services;
        }
    }
}
