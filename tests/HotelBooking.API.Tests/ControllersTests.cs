using HotelBooking.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HotelBooking.API.Tests;

public class ControllersTests
{
    [Test]
    // test that all controller dependencies are registered in the DI container
    public void AllControllers_DependenciesAreRegisteredInDIContainer()
    {
        // Arrange - Use WebApplicationFactory to get the real app configuration
        using var factory = new WebApplicationFactory<Program>();

        // Act & Assert
        using var scope = factory.Services.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var controllerTypes = typeof(Program).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"));

        foreach (var controllerType in controllerTypes)
        {
            Assert.DoesNotThrow(() =>
            {
                ActivatorUtilities.CreateInstance(serviceProvider, controllerType);
            }, $"Failed to resolve dependencies for {controllerType.Name}");
        }
    }
    
}
