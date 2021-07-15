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
    public abstract class Tool : INotifyPropertyChanged
    {
        private bool _isDown;
        private Point _position;
        private Point _downPosition;
        private Cursor _cursor = Cursors.No;

        protected Cursor _activeCursor = Cursors.Hand;
        protected Cursor _inactiveCursor = Cursors.Arrow;
        private ToolState _toolState = ToolState.Inactive;

        public event PropertyChangedEventHandler? PropertyChanged;


        protected Tool(MapController controller, string name, string iconName)
        {
            Name = name;
            IconName = iconName;
            Controller = controller;
        }

        public string Name { get; }
        public string IconName { get; }

        public Point Position
        {
            get => _position;
            private set
            {
                if (value.Equals(_position)) return;
                _position = value;
                OnPropertyChanged();
            }
        }

        public Point DownPosition
        {
            get => _downPosition;
            set
            {
                if (value.Equals(_downPosition)) return;
                _downPosition = value;
                OnPropertyChanged();
            }
        }

        public bool IsDown
        {
            get => _isDown;
            private set
            {
                if (value == _isDown) return;
                _isDown = value;
                OnPropertyChanged();
            }
        }

        public ToolState ToolState
        {
            get => _toolState;
            private set
            {
                if (value == _toolState) return;
                _toolState = value;
                OnPropertyChanged();
                Cursor = value switch
                {
                    ToolState.Invalid => Cursors.No,
                    ToolState.Active => _activeCursor,
                    ToolState.Inactive => _inactiveCursor
                };
            }
        }

        public Cursor Cursor
        {
            get => _cursor;
            private set
            {
                if (Equals(value, _cursor)) return;
                _cursor = value;
                OnPropertyChanged();
            }
        }

        protected MapController Controller { get; }
        

        public void Up(Point position)
        {
            Position = position;
            IsDown = false;
            OnUp();
        }
        
        public void Down(Point position)
        {
            DownPosition = position;
            Position = position;
            IsDown = true;
            OnDown();
        }
        
        public void Move(Point position)
        {
            Position = position;
            OnMove();
        }

        protected void CheckAndUseTool()
        {
            ToolState = GetToolState();
            if (ToolState == ToolState.Active)
            {
                OnUseTool();
            }
        }

        protected virtual void OnUp()
        {
            CheckAndUseTool();
        }
        
        protected virtual void OnDown()
        {
            CheckAndUseTool();
        }

        protected virtual void OnMove()
        {
            CheckAndUseTool();
        }

        protected abstract ToolState GetToolState();
        
        protected abstract void OnUseTool();

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}