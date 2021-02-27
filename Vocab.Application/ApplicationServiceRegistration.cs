﻿using Microsoft.Extensions.DependencyInjection;
using Vocab.Application.Contracts.Application;
using Vocab.Application.Services;

namespace Vocab.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IWordService, WordService>();

            return services;
        }
    }
}
