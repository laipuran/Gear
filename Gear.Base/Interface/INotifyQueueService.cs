using Gear.Base.Class;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gear.Base.Interface
{
    public interface INotifyQueueService
    {
        void EnqueueNotification(NotifyObject @object);
        NotifyObject? DequeueNotification(ContentForm mode);
        int Count(ContentForm mode);
        List<NotifyObject> GetObjects(ContentForm form);
    }

    public class NotificationQueueService : INotifyQueueService
    {
        private readonly ConcurrentQueue<NotifyObject> TextQueue = new();
        private readonly ConcurrentQueue<NotifyObject> FormulaQueue = new();

        public void EnqueueNotification(NotifyObject @object)
        {
            if (@object.DisplayMode == ContentForm.Text)
            {
                TextQueue.Enqueue(@object);
            }
            else if (@object.DisplayMode == ContentForm.Formula)
            {
                FormulaQueue.Enqueue(@object);
            }
        }

        public NotifyObject? DequeueNotification(ContentForm form)
        {
            if (form == ContentForm.Text)
            {
                TextQueue.TryDequeue(out var message);
                return message;
            }
            else
            {
                FormulaQueue.TryDequeue(out var message);
                return message;
            }
        }

        public int Count(ContentForm form)
        {
            if (form == ContentForm.Text)
            {
                return TextQueue.Count;
            }
            else
            {
                return FormulaQueue.Count;
            }
        }

        public List<NotifyObject> GetObjects(ContentForm form)
        {
            if (form == ContentForm.Text)
            {
                return TextQueue.ToList();
            }
            else
            {
                return FormulaQueue.ToList();
            }
        }
    }
}
