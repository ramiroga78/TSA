using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TSA.Infrastructure.Data;
using TSA.Infrastructure.DependencyInjection;
using TSA.Infrastructure.Services;
using TSA.Infrastructure.Mappings;
using System.Text.Json.Serialization;

namespace TSA.Web
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
            services.AddRepository();
            services.AddServices();
            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddDbContext<TSADbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDatabaseDeveloperPageExceptionFilter();

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<TSADbContext>();

            //----------------------------------------------------------------
            services.AddAuthentication(
                    CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                         {
                             options.LoginPath = "/";
                             options.LogoutPath = "/";
                             options.AccessDeniedPath = "/Login/AccessDenied/";
                         });

            // authentication 
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Organizaciones.Read", policy => policy.RequireClaim("Organizaciones.Read", "True"));
                options.AddPolicy("Organizaciones.Create", policy => policy.RequireClaim("Organizaciones.Create", "True"));
                options.AddPolicy("Organizaciones.Update", policy => policy.RequireClaim("Organizaciones.Update", "True"));
                options.AddPolicy("Organizaciones.Delete", policy => policy.RequireClaim("Organizaciones.Delete", "True"));

                options.AddPolicy("Profiles.Read", policy => policy.RequireClaim("Profiles.Read", "True"));
                options.AddPolicy("Profiles.Create", policy => policy.RequireClaim("Profiles.Create", "True"));
                options.AddPolicy("Profiles.Update", policy => policy.RequireClaim("Profiles.Update", "True"));
                options.AddPolicy("Profiles.Delete", policy => policy.RequireClaim("Profiles.Delete", "True"));

                options.AddPolicy("Policies.Read", policy => policy.RequireClaim("Policies.Read", "True"));
                options.AddPolicy("Policies.Create", policy => policy.RequireClaim("Policies.Create", "True"));
                options.AddPolicy("Policies.Update", policy => policy.RequireClaim("Policies.Update", "True"));
                options.AddPolicy("Policies.Delete", policy => policy.RequireClaim("Policies.Delete", "True"));

                options.AddPolicy("Certificados.Read", policy => policy.RequireClaim("Certificados.Read", "True"));
                options.AddPolicy("Certificados.Create", policy => policy.RequireClaim("Certificados.Create", "True"));
                options.AddPolicy("Certificados.Update", policy => policy.RequireClaim("Certificados.Update", "True"));
                options.AddPolicy("Certificados.Delete", policy => policy.RequireClaim("Certificados.Delete", "True"));

                options.AddPolicy("Servidores NTP.Read", policy => policy.RequireClaim("Servidores NTP.Read", "True"));
                options.AddPolicy("Servidores NTP.Create", policy => policy.RequireClaim("Servidores NTP.Create", "True"));
                options.AddPolicy("Servidores NTP.Update", policy => policy.RequireClaim("Servidores NTP.Update", "True"));
                options.AddPolicy("Servidores NTP.Delete", policy => policy.RequireClaim("Servidores NTP.Delete", "True"));

                options.AddPolicy("Permisos IP.Read", policy => policy.RequireClaim("Permisos IP.Read", "True"));
                options.AddPolicy("Permisos IP.Create", policy => policy.RequireClaim("Permisos IP.Create", "True"));
                options.AddPolicy("Permisos IP.Update", policy => policy.RequireClaim("Permisos IP.Update", "True"));
                options.AddPolicy("Permisos IP.Delete", policy => policy.RequireClaim("Permisos IP.Delete", "True"));

                options.AddPolicy("Alertas.Read", policy => policy.RequireClaim("Alertas.Read", "True"));
                options.AddPolicy("Alertas.Create", policy => policy.RequireClaim("Alertas.Create", "True"));
                options.AddPolicy("Alertas.Update", policy => policy.RequireClaim("Alertas.Update", "True"));
                options.AddPolicy("Alertas.Delete", policy => policy.RequireClaim("Alertas.Delete", "True"));

                options.AddPolicy("Deltas.Read", policy => policy.RequireClaim("Deltas.Read", "True"));
                options.AddPolicy("Deltas.Create", policy => policy.RequireClaim("Deltas.Create", "True"));
                options.AddPolicy("Deltas.Update", policy => policy.RequireClaim("Deltas.Update", "True"));
                options.AddPolicy("Deltas.Delete", policy => policy.RequireClaim("Deltas.Delete", "True"));

                options.AddPolicy("Usuarios.Read", policy => policy.RequireClaim("Usuarios.Read", "True"));
                options.AddPolicy("Usuarios.Create", policy => policy.RequireClaim("Usuarios.Create", "True"));
                options.AddPolicy("Usuarios.Update", policy => policy.RequireClaim("Usuarios.Update", "True"));
                options.AddPolicy("Usuarios.Delete", policy => policy.RequireClaim("Usuarios.Delete", "True"));

                options.AddPolicy("Roles.Read", policy => policy.RequireClaim("Roles.Read", "True"));
                options.AddPolicy("Roles.Create", policy => policy.RequireClaim("Roles.Create", "True"));
                options.AddPolicy("Roles.Update", policy => policy.RequireClaim("Roles.Update", "True"));
                options.AddPolicy("Roles.Delete", policy => policy.RequireClaim("Roles.Delete", "True"));

                options.AddPolicy("Log de Transacciones.Read", policy => policy.RequireClaim("Log de Transacciones.Read", "True"));
                options.AddPolicy("Log de Transacciones.Create", policy => policy.RequireClaim("Log de Transacciones.Create", "True"));
                options.AddPolicy("Log de Transacciones.Update", policy => policy.RequireClaim("Log de Transacciones.Update", "True"));
                options.AddPolicy("Log de Transacciones.Delete", policy => policy.RequireClaim("Log de Transacciones.Delete", "True"));

                options.AddPolicy("UsuariosExternos.Read", policy => policy.RequireClaim("UsuariosExternos.Read", "True"));
                options.AddPolicy("UsuariosExternos.Create", policy => policy.RequireClaim("UsuariosExternos.Create", "True"));
                options.AddPolicy("UsuariosExternos.Update", policy => policy.RequireClaim("UsuariosExternos.Update", "True"));
                options.AddPolicy("UsuariosExternos.Delete", policy => policy.RequireClaim("UsuariosExternos.Delete", "True"));

            });

            services.AddDistributedMemoryCache();
            //------------------------------------------------------------------------

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();


            //------------------------------------------------------------------------
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();
            //------------------------------------------------------------------------


            //app.UseRouting();

            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Login}/{id?}");
                //endpoints.MapRazorPages();
            });
        }
    }
}
