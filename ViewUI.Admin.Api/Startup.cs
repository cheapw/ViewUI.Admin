using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ViewUI.Admin.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ViewUI.Admin.Api
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

            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=ViewUI.Admin.Api;trusted_connection=yes;";

            services.AddDbContext<AdminApiContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";
                    options.ApiName = "api1";
                    options.RequireHttpsMetadata = false;
                    options.JwtValidationClockSkew = TimeSpan.FromSeconds(5);
                });
            services.AddAuthorization();
            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins("http://localhost:5003")
                       .AllowAnyHeader()
                       .AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("default");
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AdminApiContext>();
                context.Database.Migrate();
                if (!context.UserMessages.Any())
                {
                    List<UserMessage> messages = new List<UserMessage>
                    {
                        new UserMessage
                        {
                            Id = Guid.NewGuid(),
                            Title = "test1",
                            Content = "Just for test",
                            CreateTime = DateTime.Now,
                            FromId = Guid.Parse("138C913C-727C-4F86-A68A-E629535BFD1D"),
                            ToId = Guid.Parse("696FE264-B54E-4A57-A5CD-A5A286497E70")
                        },
                        new UserMessage
                        {
                            Id = Guid.NewGuid(),
                            Title = "test2",
                            Content = "Just for test, unread message",
                            CreateTime = DateTime.Now,
                            FromId = Guid.Parse("138C913C-727C-4F86-A68A-E629535BFD1D"),
                            ToId = Guid.Parse("696FE264-B54E-4A57-A5CD-A5A286497E70")
                        },
                        new UserMessage
                        {
                            Id = Guid.NewGuid(),
                            Title = "test2",
                            Content = "read message",
                            IsRead = true,
                            CreateTime = DateTime.Now,
                            FromId = Guid.Parse("138C913C-727C-4F86-A68A-E629535BFD1D"),
                            ToId = Guid.Parse("696FE264-B54E-4A57-A5CD-A5A286497E70")
                        },
                        new UserMessage
                        {
                            Id = Guid.NewGuid(),
                            Title = "test2",
                            Content = "deleted message",
                            IsRead = true,
                            IsDelete = true,
                            CreateTime = DateTime.Now,
                            FromId = Guid.Parse("138C913C-727C-4F86-A68A-E629535BFD1D"),
                            ToId = Guid.Parse("696FE264-B54E-4A57-A5CD-A5A286497E70")
                        }
                    };
                    context.UserMessages.AddRange(messages);
                    context.SaveChanges();
                }
            }
        }
    }
}
