using Gear.Base.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Gear.RestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotifyObjectController : ControllerBase
    {
        private readonly ILogger<NotifyObjectController> _logger;

        public NotifyObjectController(ILogger<NotifyObjectController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetTextQueue")]
        public List<Base.Class.NotifyObject> Get()
        {
            var services = new ServiceCollection();
            services.AddSingleton<INotifyQueueService, NotificationQueueService>();
            var serviceProvider = services.BuildServiceProvider();
#pragma warning disable CS8602 // �����ÿ��ܳ��ֿ����á�
            return serviceProvider.GetService<INotifyQueueService>().GetObjects(Base.Class.ContentForm.Text);
#pragma warning restore CS8602 // �����ÿ��ܳ��ֿ����á�
        }
    }
}
