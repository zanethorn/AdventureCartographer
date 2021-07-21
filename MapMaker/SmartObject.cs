using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Threading;
using System.Xml.Serialization;
using MapMaker.Annotations;

namespace MapMaker
{
    public abstract class SmartObject : INotifyPropertyChanged, ICloneable, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        [XmlIgnore]
        [IgnoreDataMember]
        [NotMapped]
        private List<string> _propertyBacklog = new();


        protected Dispatcher CurrentDispatcher => Dispatcher.CurrentDispatcher;

        public void Touch(string propertyName )
        {
            OnPropertyChanged(propertyName);
            DispatchBacklog();
        }
        
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

        protected virtual void OnClone(object clone)
        {
            // Should be implemented by other classes
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (CurrentDispatcher.CheckAccess())
            {
                DispatchBacklog();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            else if (!_propertyBacklog.Contains(propertyName))
            {
                _propertyBacklog.Add(propertyName);
            }
        }

        protected void DispatchBacklog()
        {
            void ClearBacklog()
            {
                foreach (var propertyName in _propertyBacklog)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }

                _propertyBacklog.Clear();
            }

            if (_propertyBacklog.Count > 0)
            {
                if (CurrentDispatcher.CheckAccess())
                {
                    ClearBacklog();
                }
                else
                {
                    CurrentDispatcher.BeginInvoke((Action) ClearBacklog);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                PropertyChanged = null;
            }
        }
    }
}