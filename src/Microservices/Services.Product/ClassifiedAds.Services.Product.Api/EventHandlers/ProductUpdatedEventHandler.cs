﻿using ClassifiedAds.Application;
using ClassifiedAds.CrossCuttingConcerns.ExtensionMethods;
using ClassifiedAds.Domain.Events;
using ClassifiedAds.Infrastructure.Identity;
using ClassifiedAds.Services.AuditLog.Contracts.DTOs;
using ClassifiedAds.Services.Product.Api.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ClassifiedAds.Services.Product.EventHandlers
{
    public class ProductUpdatedEventHandler : IDomainEventHandler<EntityUpdatedEvent<Entities.Product>>
    {
        private readonly IServiceProvider _serviceProvider;

        public ProductUpdatedEventHandler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void Handle(EntityUpdatedEvent<Entities.Product> domainEvent)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var currentUser = serviceProvider.GetService<ICurrentUser>();
                var dispatcher = serviceProvider.GetService<Dispatcher>();

                dispatcher.Dispatch(new AddAuditLogEntryCommand
                {
                    AuditLogEntry = new AuditLogEntryDTO
                    {
                        UserId = currentUser.UserId,
                        CreatedDateTime = domainEvent.EventDateTime,
                        Action = "UPDATED_PRODUCT",
                        ObjectId = domainEvent.Entity.Id.ToString(),
                        Log = domainEvent.Entity.AsJsonString(),
                    },
                });
            }
        }
    }
}
