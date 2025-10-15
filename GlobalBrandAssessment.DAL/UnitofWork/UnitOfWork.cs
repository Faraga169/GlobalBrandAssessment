using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Attachment;
using GlobalBrandAssessment.DAL.Repositories.Generic;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GlobalBrandAssessment.DAL.UnitofWork
{
    public class UnitOfWork : IUnitofWork
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<Type, object> _repositories = new();

        // Constructor accepting IServiceProvider for dependency injection
        public UnitOfWork(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        // Generic method to get repository instance
        public TRepository Repository<TRepository, TEntity>()
            where TRepository : IGenericRepository<TEntity>
            where TEntity : class
        {
            // Get the type of the repository
            var type = typeof(TRepository);

            // Check if repository instance already exists in the dictionary
            if (_repositories.ContainsKey(type))
                return (TRepository)_repositories[type];


            // Create new instance of repository through dependency Injection of IService provider
            var repo = _serviceProvider.GetRequiredService<TRepository>();

            // Add the new repository instance to the dictionary
            _repositories.Add(type, repo);

            // Return the repository instance
            return repo;
        }


        // Save changes to the database
        public async Task<int> CompleteAsync() =>
            await _serviceProvider.GetRequiredService<GlobalbrandDbContext>().SaveChangesAsync();

      

    }
}
