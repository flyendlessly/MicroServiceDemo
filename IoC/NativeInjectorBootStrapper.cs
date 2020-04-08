using Application.MiddleWare;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<DiagnosticListenerObserver>();
            //services.AddSingleton<IDiagnosticProcessor, HttpClientDiagnosticProcessor>();
            services.AddSingleton<IDiagnosticProcessor, TestDiagnosticProcessor>();

        }
    }
}
