using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using ViewUI.Admin.IdentityServer.Data.UserStore;
using IdentityServer4.Models;
using System.Collections.ObjectModel;
using IdentityModel;
//using System.Security.Cryptography;
//using ViewUI.Admin.IdentityServer.Helpers;

namespace ViewUI.Admin.IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;
            const string connectionString = @"Data Source=(LocalDb)\MSSQLLocalDB;database=IdentityServer4.Quickstart.EntityFramework-3.0.0;trusted_connection=yes;";

            services.AddDbContext<UserContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });
            services.AddScoped<UserStore>();
            //var builder = services.AddIdentityServer()
            //    .AddInMemoryIdentityResources(Config.Ids)
            //    .AddInMemoryApiResources(Config.Apis)
            //    .AddInMemoryClient0s(Config.Clients)
            //    .AddTestUsers(TestUsers.Users);
            //builder.AddDeveloperSigningCredential();

            services.AddIdentityServer()
                //.AddTestUsers(TestUsers.Users)
                //.AddSigningCredential(null)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddResourceOwnerValidator<UserResourceOwnerPasswordValidator>()
                .AddProfileService<UserProfileService>()
                .AddDeveloperSigningCredential();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // this will do the initial DB population
            //InitializeDatabase(app);


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
        private void InitializeDatabase(IApplicationBuilder app)
        {
            using(var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                context.Database.Migrate();
                if (!context.Clients.Any())
                {
                    foreach(var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.Ids)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.Apis)
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                var userContext = serviceScope.ServiceProvider.GetRequiredService<UserContext>();
                userContext.Database.Migrate();
                if (!userContext.Users.Any())
                {
                    var guid1 = Guid.NewGuid();
                    var user1 = new User
                    {
                        Id = guid1,
                        IsActive = true,
                        Username = "Alice",
                        Password = "Alice".Sha256(),
                        SubjectId = guid1.ToString(),
                        Claims = new List<Claim>()
                        {
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.Name,
                                Value = "Alice Smith",
                                UserId = guid1
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.GivenName,
                                Value = "Alice",
                                UserId = guid1
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.FamilyName,
                                Value = "Smith",
                                UserId = guid1
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.Picture,
                                Value = "https://avatars.githubusercontent.com/u/20942571?s=460&v=4",
                                UserId = guid1
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.Email,
                                Value = "AliceSmith@email.com",
                                UserId = guid1
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.WebSite,
                                Value = "http://alice.com",
                                UserId = guid1
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = "power",
                                Value = "[\"admin\"]",
                                UserId = guid1
                            }
                        }
                    };
                    var guid2 = Guid.NewGuid();
                    var User2 = new User
                    {
                        Id = guid2,
                        IsActive = true,
                        Username = "bob",
                        Password = "bob".Sha256(),
                        SubjectId = guid2.ToString(),
                        Claims = new List<Claim>()
                        {
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.Name,
                                Value = "Bob Smith",
                                UserId = guid2
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.GivenName,
                                Value = "Bob",
                                UserId = guid2
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.FamilyName,
                                Value = "Smith",
                                UserId = guid2
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.Picture,
                                Value = "https://file.iviewui.com/dist/a0e88e83800f138b94d2414621bd9704.png",
                                UserId = guid2
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.Email,
                                Value = "BobSmith@email.com",
                                UserId = guid2
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = JwtClaimTypes.WebSite,
                                Value = "http://bob.com",
                                UserId = guid2
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = "location",
                                Value = "somewhere",
                                UserId = guid2
                            },
                            new Claim
                            {
                                Id = Guid.NewGuid(),
                                Type = "power",
                                Value = "[\"super_admin\", \"admin\"]",
                                UserId = guid2
                            }
                        }
                    };

                    userContext.Users.AddRange(new User[2]
                    {
                        user1,
                        User2
                    });
                    userContext.SaveChanges();
                }
            }
        }
    }
}
