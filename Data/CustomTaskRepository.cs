using Microsoft.FSharp.Collections;
using static CustomTaskCore;

namespace Data
{
    public class CustomTaskRepository : ICustomTaskRepository
    {
        private readonly CustomTaskCore.CustomTaskDbContext dbContext;

        public CustomTaskRepository(CustomTaskCore.CustomTaskDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        CustomTask ICustomTaskRepository.AddTask(CustomTask task)
        {
            if (task == null)
                throw new ArgumentNullException();

            dbContext.Tasks.Add(task);
            dbContext.SaveChanges();
            return task;
        }

        void ICustomTaskRepository.DeleteTask(int id)
        {
            var taskToDelete = dbContext.Tasks.FirstOrDefault(t => t.Id == id);
            if (taskToDelete != null)
            {
                dbContext.Tasks.Remove(taskToDelete);
                dbContext.SaveChanges();
            }
        }

        FSharpList<CustomTask> ICustomTaskRepository.GetAllTasks()
        {
            return ListModule.OfSeq(dbContext.tasks);
        }

        CustomTask ICustomTaskRepository.GetTaskById(int id)
        {
            var task = dbContext.Tasks.FirstOrDefault(t => t.Id == id);
            if (task == null)
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            return task;
        }

        CustomTask ICustomTaskRepository.UpdateTask(CustomTask task)
        {
            if (task == null)
                throw new ArgumentNullException();

            var taskToUpdate = dbContext.Tasks.FirstOrDefault(t => t.Id == task.Id);

            if (taskToUpdate == null)
                throw new KeyNotFoundException($"Task with ID {task.Id} not found.");
            
            taskToUpdate.Title = task.Title;
            taskToUpdate.Description = task.Description;
            taskToUpdate.DueDate = task.DueDate;
            taskToUpdate.Status = task.Status;
            dbContext.SaveChanges();

            return taskToUpdate;
        }
    }
}