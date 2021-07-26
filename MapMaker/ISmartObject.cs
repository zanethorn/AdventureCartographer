using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace MapMaker
{
    public interface ISmartObject:INotifyPropertyChanged, ICloneable, IDisposable
    {

        [Browsable(false)]
        Dispatcher UIDispatcher { get; }

        void DispatchNotifications();
    }
}