using MapMaker.Annotations;
using MapMaker.File;

namespace MapMaker
{
    public class Pointer:Tool
    {
        public Pointer(MapController controller) : base(controller, nameof(Pointer), nameof(Pointer))
        {
        }

        protected override bool OnCanExecute(object? parameter)
        {
            return TargetObject!=null;
        }

        protected override void OnExecute(object? parameter)
        {
            
        }
    }
}