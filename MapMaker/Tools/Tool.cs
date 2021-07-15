using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Forms.VisualStyles;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MapMaker.Annotations;
using MapMaker.File;

namespace MapMaker
{
    public abstract class Tool:ICommand, INotifyPropertyChanged
    {
        private bool _isDown;
        private bool _canExecute;
        
        protected Cursor _activeCursor = Cursors.Hand;
        protected Cursor _baseCursor = Cursors.Arrow;
        private MapObject _targetObject;
        private Cursor _cursor;

        public event EventHandler? CanExecuteChanged;
        public event PropertyChangedEventHandler? PropertyChanged;
        
        
        protected Tool(MapController controller, string name, string iconName)
        {
            Name = name;
            IconName = iconName;
            Controller=controller;
            Cursor = Cursors.No;
        }
        
        public string Name { get; }
        public string IconName { get; }


        public Cursor Cursor
        {
            get => _cursor;
            protected set
            {
                if (Equals(value, _cursor)) return;
                _cursor = value;
                OnPropertyChanged();
            }
        }

        protected MapController Controller { get; }

        public bool IsDown
        {
            get => _isDown;
            set
            {
                if (value == _isDown) return;
                _isDown = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Cursor));
            }
        }

        public MapObject TargetObject
        {
            get => _targetObject;
            set
            {
                if (Equals(value, _targetObject)) return;
                _targetObject = value;
                OnPropertyChanged();
                CanExecute(value);
            }
        }

        public bool CanExecute(object? parameter)
        {
            var r = OnCanExecute(parameter);
            if (r != _canExecute)
            {
                _canExecute = r;
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                Cursor = _canExecute ? _baseCursor : Cursors.No;
            }

            return r;
        }

        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
            {
                throw new InvalidOperationException();
            }
            OnExecute(parameter);
        }

        protected virtual bool OnCanExecute(object? parameter)
        {
            return true;
        }

        protected abstract void OnExecute(object? parameter);
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}