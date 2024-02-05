using Gear.Base.Class;
using Gear.Base.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Gear.RestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FormulaNotification : ControllerBase
    {
        private readonly ILogger<FormulaNotification> _logger;
        public static INotifyQueueService NotifyService { get; set; } = TextNotification.NotifyService;

        public FormulaNotification(ILogger<FormulaNotification> logger)
        {
            _logger = logger;
        }

        private static INotifyQueueService GetQueueService()
        {
            var services = new ServiceCollection();
            services.AddSingleton<INotifyQueueService, NotificationQueueService>();
            var serviceProvider = services.BuildServiceProvider();
#pragma warning disable CS8603 // 可能返回 null 引用。
            return serviceProvider.GetService<INotifyQueueService>();
#pragma warning restore CS8603 // 可能返回 null 引用。
        }

        [HttpGet(Name = "GetFormulas")]
        public List<NotifyObject> Get()
        {
            return NotifyService.GetObjects(ContentForm.Formula);
        }

        [HttpPost(Name = "EnqueueFormula")]
        public void Post(string content)
        {
            NotifyService.EnqueueNotification(new(ContentForm.Formula, content));

        }
    }
}
