using Gear.Base.Class;
using Gear.Base.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Gear.RestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TextNotification : ControllerBase
    {
        private readonly ILogger<TextNotification> _logger;
        public static INotifyQueueService NotifyService { get; set; } = GetQueueService();

        public TextNotification(ILogger<TextNotification> logger)
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

        [HttpGet(Name = "GetTexts")]
        public List<NotifyObject> Get()
        {
            return NotifyService.GetObjects(ContentForm.Text);
        }

        [HttpPost(Name = "EnqueueText")]
        public List<NotifyObject> Post(string content)
        {
            NotifyService.EnqueueNotification(new(ContentForm.Text, content));
            return Get();
        }
    }
}
