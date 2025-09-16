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

namespace GlobalBrandAssessment.DAL.UnitofWork
{
    public class UnitOfWork : IUnitofWork
    {
        private readonly GlobalbrandDbContext globalbrandDbContext;
        private Lazy<IEmployeeRepository> _employeeRepository;
        private Lazy<ITaskRepository> _taskRepository;
        private Lazy<ICommentRepository> _commentRepository;
        private Lazy<IAttachmentRepository> _attachmentRepository;
        private Lazy<IManagerRepository> _managerRepository;
        private Lazy<IDepartmentRepository> _departmentRepository;
      

        public UnitOfWork(GlobalbrandDbContext globalbrandDbContext)
        {
            _commentRepository = new Lazy<ICommentRepository>(()=>new CommentRepository(globalbrandDbContext));
            _employeeRepository = new Lazy<IEmployeeRepository>(()=>new EmployeeRepository(globalbrandDbContext));
            _taskRepository = new Lazy<ITaskRepository>(()=>new TaskRepository(globalbrandDbContext));
            _attachmentRepository = new Lazy<IAttachmentRepository>(() => new AttachmentRepository(globalbrandDbContext)); ;
            _managerRepository = new Lazy<IManagerRepository>(()=>new ManagerRepository(globalbrandDbContext));
            _departmentRepository = new Lazy<IDepartmentRepository>(()=>new DepartmentRepository(globalbrandDbContext));
            this.globalbrandDbContext = globalbrandDbContext;
           
        }
        public IAttachmentRepository attachmentRepository => _attachmentRepository.Value;

        public ICommentRepository commentRepository => _commentRepository.Value;

        public IEmployeeRepository employeeRepository => _employeeRepository.Value;


        public ITaskRepository taskRepository => _taskRepository.Value;

        public IManagerRepository ManagerRepository => _managerRepository.Value;

        public IDepartmentRepository departmentRepository => _departmentRepository.Value;

        public Task<int> CompleteAsync()
        {
            return globalbrandDbContext.SaveChangesAsync();
        }


        public IGenericRepository<T> Repository<T>() where T : class
        {
            return new GenericRepository<T>(globalbrandDbContext);
        }

    }
}
