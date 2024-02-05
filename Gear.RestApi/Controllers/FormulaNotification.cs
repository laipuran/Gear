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

        [HttpGet(Name = "GetFormulas")]
        public List<NotifyObject> Get()
        {
            return NotifyService.GetObjects(ContentForm.Formula);
        }

        [HttpPost(Name = "EnqueueFormula")]
        public List<NotifyObject> Post(string content)
        {
            NotifyService.EnqueueNotification(new(ContentForm.Formula, content));
            return Get();
        }
    }
}
