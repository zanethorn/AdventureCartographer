using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace MapMaker.Models
{
    public interface ISmartObject : INotifyPropertyChanged, ICloneable, IDisposable
    {
        [Browsable(false)]
        Dispatcher UIDispatcher { get; }

        void DispatchNotifications();
    }
}