using Application.MiddleWare;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using MediatR;
using Domain.Core.Bus;
using Infra.CrossCutting.Bus;
using Domain.Core.Notifications;
using Domain.Events;
using Domain.EventHandlers;
using Domain.Core.Events;
using Infra.CrossCutting.Data.EventSourcing;

namespace IoC
{
    public class NativeInjectorBootStrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddSingleton<DiagnosticListenerObserver>();
            //services.AddSingleton<IDiagnosticProcessor, HttpClientDiagnosticProcessor>();
            services.AddSingleton<IDiagnosticProcessor, TestDiagnosticProcessor>();


            //services.AddScoped<ServiceFactory>(p => p.GetService); 

            // Domain Bus (Mediator)
            services.AddScoped<IMediatorHandler, InMemoryBus>();

            // Domain - Events
            services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
            services.AddScoped<INotificationHandler<CustomerRegisteredEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerUpdatedEvent>, CustomerEventHandler>();
            services.AddScoped<INotificationHandler<CustomerRemovedEvent>, CustomerEventHandler>();

            // Domain - Commands
            //services.AddScoped<IRequestHandler<RegisterNewCustomerCommand, bool>, CustomerCommandHandler>();
            //services.AddScoped<IRequestHandler<UpdateCustomerCommand, bool>, CustomerCommandHandler>();
            //services.AddScoped<IRequestHandler<RemoveCustomerCommand, bool>, CustomerCommandHandler>();

            // Infra - Data EventSourcing
            services.AddScoped<IEventStore, SqlEventStore>();
        }
    }
}
