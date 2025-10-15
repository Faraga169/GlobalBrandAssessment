using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.DAL.Repositories;
using GlobalBrandAssessment.DAL.Repositories.Generic;

namespace GlobalBrandAssessment.DAL.UnitofWork
{
    public interface IUnitofWork
    {

        //public IAttachmentRepository attachmentRepository { get; }
        //public ICommentRepository commentRepository { get; }

        //public IEmployeeRepository employeeRepository { get; }



        //public ITaskRepository taskRepository { get; }

        //public IManagerRepository ManagerRepository { get; }

        //public IDepartmentRepository departmentRepository { get; }
        public Task<int> CompleteAsync();

      public  TRepository Repository<TRepository, TEntity>()
      where TRepository : IGenericRepository<TEntity>
      where TEntity : class;
     



    }
}
