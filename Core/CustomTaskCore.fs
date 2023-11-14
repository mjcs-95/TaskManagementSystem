module CustomTaskCore
    open System
    open Microsoft.EntityFrameworkCore

    type public CustomTaskStatus =
        | NotStarted
        | InProgress
        | Completed

    [<CLIMutable>]
    type public CustomTask = {
        Id : int
        Title : string
        Description : string option
        DueDate : DateTime
        Status : CustomTaskStatus
    }

    let validateTask(task: CustomTask) =
        let validTitle =  not (String.IsNullOrWhiteSpace task.Title)
        let validDueDate = task.DueDate > DateTime.Now

        if not validTitle then
            Some("Title is required.")
        elif not validDueDate then 
            Some("Due date must be in the future.") 
        elif task.Title.Length > 100 then
            Some("Title Length must not exceed 100 characters")
        elif Option.isSome task.Description && String.length (Option.get task.Description) > 500 then
            Some("Description must not exceed 500 characters")
        elif not (Set.ofList [NotStarted; InProgress; Completed] |> Set.contains task.Status) then
            Some("Invalid task status");
        else
            None

    type CustomTaskDbContext() =
        inherit DbContext()

        [<DefaultValue>]
        val mutable tasks : DbSet<CustomTask>

        member public this.Tasks 
            with get() = 
                this.tasks
            and set p = 
                this.tasks <- p

        override _.OnConfiguring(optionsBuilder : DbContextOptionsBuilder) =
            optionsBuilder.UseSqlServer("YOUR CONNECTION STRING")
            |> ignore


    type public ICustomTaskRepository = 
        abstract GetAllTasks : unit -> CustomTask list
        abstract GetTaskById : int -> CustomTask
        abstract AddTask : CustomTask -> CustomTask
        abstract UpdateTask : CustomTask -> CustomTask
        abstract DeleteTask : int -> unit

    
    type TaskService(taskRepository: ICustomTaskRepository) =
        member this.GetTasks() = taskRepository.GetAllTasks
        member this.GetTaskById(id: int) = taskRepository.GetTaskById id
        member this.CreateTask(task: CustomTask) = taskRepository.AddTask task
        member this.UpdateTask(task: CustomTask) = taskRepository.UpdateTask task
        member this.DeleteTask(id: int) = taskRepository.DeleteTask id
