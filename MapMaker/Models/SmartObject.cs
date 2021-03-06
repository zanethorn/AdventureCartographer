using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Threading;
using System.Xml.Serialization;
using MapMaker.Properties;

namespace MapMaker.Models
{
    [Serializable]
    [DataContract]
    public abstract class SmartObject : ISmartObject
    {
        [XmlIgnore]
        [IgnoreDataMember]
        [NotMapped]
        [Browsable(false)]
        private List<string> _propertyBacklog = new();

        
        public event PropertyChangedEventHandler? PropertyChanged;

        [XmlIgnore]
        [IgnoreDataMember]
        [NotMapped]
        [Browsable(false)]
        public Dispatcher UIDispatcher => Application.Current.Dispatcher;

        public object Clone()
        {
            var clone = (SmartObject) MemberwiseClone();
            clone._propertyBacklog = new List<string>();
            OnClone(clone);
            return clone;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void DispatchNotifications()
        {
            void DispatchBacklog()
            {
                OnDispatchNotifications();

                if (_propertyBacklog.Count > 0)
                {
                    foreach (var propertyName in _propertyBacklog)
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                    _propertyBacklog.Clear();
                }

                OnNotificationsDispatched();
            }


            if (UIDispatcher.CheckAccess())
                DispatchBacklog();
            else
                UIDispatcher.BeginInvoke((Action) DispatchBacklog);
        }

        public void Touch(string propertyName)
        {
            OnPropertyChanged(propertyName);
            DispatchNotifications();
        }

        protected virtual void OnClone(object clone)
        {
            // Should be implemented by other classes
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            if (UIDispatcher.CheckAccess())
            {
                DispatchNotifications();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else if (!_propertyBacklog.Contains(propertyName!))
            {
                _propertyBacklog.Add(propertyName!);
            }
        }

        protected virtual void OnDispatchNotifications()
        {
        }

        protected virtual void OnNotificationsDispatched()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) PropertyChanged = null;
        }
    }
}