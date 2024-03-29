﻿// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;

namespace IdentityServer
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment environment)
        {
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you wan to add an MVC-based UI
            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);
            services.AddAuthentication()
     .AddGoogle("Google","谷歌", options =>
     {
         options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

         options.ClientId = "<insert here>";
         options.ClientSecret = "<insert here>";
     }).AddOpenIdConnect("oidc", "OpenID Connect 我的", options =>
     {
         options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
         options.SignOutScheme = IdentityServerConstants.SignoutScheme;
         options.SaveTokens = true;

         options.Authority = "https://demo.identityserver.io/";
         options.ClientId = "implicit";

         options.TokenValidationParameters = new TokenValidationParameters
         {
             NameClaimType = "name",
             RoleClaimType = "role"
         };
     });
            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddTestUsers(Config.GetUsers());

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // uncomment if you want to support static files
            app.UseStaticFiles();

            app.UseIdentityServer();
            // uncomment, if you wan to add an MVC-based UI
            app.UseMvcWithDefaultRoute();
        }
    }
}