﻿using Microsoft.Owin;

[assembly: OwinStartup(typeof(LNF.WebApi.Scheduler.Startup))]

namespace LNF.WebApi.Scheduler
{
    /// <summary>
    /// This class must be local to the application or there is an issue with routing when IIS resets.
    /// </summary>
    public class Startup : ApiOwinStartup { }
}