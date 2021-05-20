using Bogus;
using ExampleApi.Entities;
using ExampleApi.Validations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddTransient<AddProductValidation>();
            services.AddTransient<UpdateProductValidation>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var categoryIds = 1;
            var categoryFaker = new Faker<Category>()
                .RuleFor(x => x.Id, y => categoryIds++)
                .RuleFor(x => x.Name, y => y.Lorem.Word());
            var categories = categoryFaker.Generate(20);

            var productIds = 1;
            var productFaker = new Faker<Product>()
                .RuleFor(x => x.Id, y => productIds++)
                .RuleFor(x => x.Name, y => y.Commerce.ProductName())
                .RuleFor(x => x.Price, y => y.Finance.Amount())
                .RuleFor(x => x.Category, y => y.PickRandom(categories));
            var products = productFaker.Generate(1000);

            InMemmoryDatabase.Categories = categories;
            InMemmoryDatabase.Products = products;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(x =>
            {
                x.AllowAnyOrigin();
                x.AllowAnyMethod();
                x.AllowAnyHeader();
            });            

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
