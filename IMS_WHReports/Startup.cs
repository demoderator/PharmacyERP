﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IMS_WHReports.Startup))]
namespace IMS_WHReports
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
