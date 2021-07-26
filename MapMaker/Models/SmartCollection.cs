using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace MapMaker.Models
{
    public class SmartCollection<T> : ObservableCollection<T>, ISmartObject
        where T : SmartObject
    {
        [XmlIgnore]
        [IgnoreDataMember]
        [NotMapped]
        private List<string> _propertyBacklog = new();

        public SmartCollection()
        {
        }

        public SmartCollection(IEnumerable<T> collection) : base(collection)
        {
        }

        [XmlIgnore]
        [IgnoreDataMember]
        [NotMapped]
        public Dispatcher UIDispatcher => Application.Current.Dispatcher;

        public void DispatchNotifications()
        {
            void DispatchBacklog()
            {
                OnDispatchNotifications();
                foreach (var propertyName in _propertyBacklog)
                    base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

                _propertyBacklog.Clear();
                OnNotificationsDispatched();
            }

            if (_propertyBacklog.Count > 0)
            {
                if (UIDispatcher.CheckAccess())
                    DispatchBacklog();
                else
                    UIDispatcher.BeginInvoke((Action) DispatchBacklog);
            }
        }

        public object Clone()
        {
            var myClone = Activator.CreateInstance(GetType(), this)!;
            OnClone(myClone);
            return myClone;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (UIDispatcher.CheckAccess())
            {
                DispatchNotifications();
                base.OnPropertyChanged(e);
            }
            else if (!_propertyBacklog.Contains(e.PropertyName!))
            {
                _propertyBacklog.Add(e.PropertyName!);
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (UIDispatcher.CheckAccess())
                base.OnCollectionChanged(e);
            else
                UIDispatcher.BeginInvoke(() => { base.OnCollectionChanged(e); });
        }

        protected virtual void OnDispatchNotifications()
        {
        }

        protected virtual void OnNotificationsDispatched()
        {
            foreach (var i in this) i.DispatchNotifications();
        }

        protected virtual void OnClone(object clone)
        {
            // Should be implemented by other classes
            var myClone = (SmartCollection<T>) clone;
            myClone._propertyBacklog = new List<string>();
            foreach (var i in this) myClone.Add(i);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var i in this) i.Dispose();
                ClearItems();
            }
        }
    }
}