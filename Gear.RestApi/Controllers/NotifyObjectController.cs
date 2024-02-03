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
        public IEnumerable<NotifyObject> Get()
        {
            //List<NotifyObject> result = new List<NotifyObject>();
            //foreach (var item in App.Notifier.TextQueue.ToList())
            //{
            //    result.Add(new NotifyObject() { Mode = Windows.DisplayMode.Text, Content = item });
            //}
            //return result;
            return new List<NotifyObject>();
        }
    }
}
