using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Flowchart.Models.ToolModels;

namespace LS.Flowchart.ModuleActions.Images
{
    public class ImageOutputExecution : IModuleExecution
    {
        public bool CanExecute(ModuleItemModel model,object parameter)
        {
            return true;
        }

        public bool Execute(ModuleItemModel model, object parameter)
        {
            if (CanExecute(model, parameter))
            {
                return true;
            }
            return false;
        }
    }
}
