﻿using HelpSystem.DAL.Implementantions;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Service.Implementantions;
using HelpSystem.Service.Interfaces;

namespace HelpSystem
{
    public static class Initializer
    {
        public static void InitializeRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<User>, AccountRepository>();
            services.AddScoped<IBaseRepository<Profile>, ProfileRepository>();
            services.AddScoped<IBaseRepository<Role>, RoleRepository>();
            services.AddScoped<IBaseRepository<Statement>, StatementRepository>();
            services.AddScoped<IBaseRepository<Provider>, ProviderRepository>();
            services.AddScoped<IBaseRepository<Warehouse>, WarehouseRepository>();
            services.AddScoped<IBaseRepository<Buyer>, BuyerRepository>();
            services.AddScoped<IBaseRepository<Products>, ProductRepository>();

            //TODO Незабудь добавить сервисы
        }

        public static void InitializeServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStatmentIService, StatmentService>();


        }
    }
}
