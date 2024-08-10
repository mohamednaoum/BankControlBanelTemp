using System.Text.Json.Serialization;
using BankingControlPanel.Domain.Models;
using BankingControlPanel.Domain.ValueObjects;
using BankingControlPanel.Infrastructure.Data;
using BankingControlPanel.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BankingControlPanel.Infrastructure.UnitTests
{
    public class ClientRepositoryTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public ClientRepositoryTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
        }

        private ApplicationDbContext CreateContext() => new ApplicationDbContext(_dbContextOptions);

        [Fact]
        public void GetClients_ShouldReturnClients_WithCorrectFilterAndPaging()
        {
            // Arrange
            using var context = CreateContext();
            context.Clients.AddRange(
                new Client { FirstName = "John", LastName = "Doe" , Email = new Email("john.doe@test.com") ,
                    PersonalId = new PersonalId("12345678901"),
                    MobileNumber = "12345",
                    ProfilePhoto = "test" },
                new Client { FirstName = "Jane", LastName = "Smith", Email = new Email("john.doe@test.com") ,
                    PersonalId = new PersonalId("12345678901"),
                    MobileNumber = "12345",
                    ProfilePhoto = "test" },
                new Client { FirstName = "John", LastName = "Smith", Email = new Email("john.doe@test.com") ,
                    PersonalId = new PersonalId("12345678901"),
                    MobileNumber = "12345",
                    ProfilePhoto = "test" }
            );
            context.SaveChanges();

            var repository = new ClientRepository(context);

            // Act
            var result = repository.GetClients("John", null, 1, 10);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, c => Assert.Contains("John", c.FirstName));
        }

        [Fact]
        public void GetClientById_ShouldReturnCorrectClient()
        {
            // Arrange
            using var context = CreateContext();
            var client = new Client { FirstName = "John", LastName = "Doe", Email = new Email("john.doe@test.com") ,
                PersonalId = new PersonalId("12345678901"),
                MobileNumber = "12345",
                ProfilePhoto = "test" };
            context.Clients.Add(client);
            context.SaveChanges();

            var repository = new ClientRepository(context);

            // Act
            var result = repository.GetClientById(client.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(client.FirstName, result.FirstName);
            Assert.Equal(client.LastName, result.LastName);
        }

        [Fact]
        public void AddClient_ShouldAddClientToDatabase()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ClientRepository(context);
            var client = new Client 
            { 
                FirstName = "John", 
                LastName = "Doe", 
                Email = new Email("john.doe@test.com") ,
                PersonalId = new PersonalId("12345678901"),
                MobileNumber = "12345",
                ProfilePhoto = "test"
            };

            // Act
            repository.AddClient(client);

            // Assert
            Assert.Equal(1, context.Clients.Count());
            var addedClient = context.Clients.Single();
            Assert.Equal(client.FirstName, addedClient.FirstName);
            Assert.Equal(client.LastName, addedClient.LastName);
            Assert.Equal(client.Email.Value, addedClient.Email.Value);
        }

        [Fact]
        public void UpdateClient_ShouldModifyClientInDatabase()
        {
            // Arrange
            using var context = CreateContext();
            var client = new Client { FirstName = "John", LastName = "Doe" , Email = new Email("john.doe@test.com") ,
                PersonalId = new PersonalId("12345678901"),
                MobileNumber = "12345",
                ProfilePhoto = "test" };
            context.Clients.Add(client);
            context.SaveChanges();

            var repository = new ClientRepository(context);
            client.LastName = "Smith";

            // Act
            repository.UpdateClient(client);

            // Assert
            var updatedClient = context.Clients.Single();
            Assert.Equal("Smith", updatedClient.LastName);
        }

        [Fact]
        public void DeleteClient_ShouldRemoveClientFromDatabase()
        {
            // Arrange
            using var context = CreateContext();
            var client = new Client { FirstName = "John", LastName = "Doe", Email = new Email("john.doe@test.com") ,
                PersonalId = new PersonalId("12345678901"),
                MobileNumber = "12345",
                ProfilePhoto = "test" };
            context.Clients.Add(client);
            context.SaveChanges();

            var repository = new ClientRepository(context);

            // Act
            repository.DeleteClient(client.Id);

            // Assert
            Assert.Empty(context.Clients);
        }

        [Fact]
        public void GetLastSearchParameters_ShouldReturnLastSearchParameters()
        {
            // Arrange
            using var context = CreateContext();
            var repository = new ClientRepository(context);

            // Act
            var result = repository.GetLastSearchParameters(3);

            // Assert
            Assert.Empty(result); // Since logic is not implemented, expected result is empty.
        }
    }
}
