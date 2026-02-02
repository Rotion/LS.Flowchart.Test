using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LS.Flowchart.ModuleActions;

namespace LS.Flowchart.ModuleParamView
{
    /// <summary>
    /// 模块配置界面对应的实现类的特性
    /// </summary>
    public class ModuleParamViewAttribute : Attribute
    {
        public Type ParamViewType { get; private set; }
        public ModuleParamViewAttribute(Type viewType)
        {
            if (!typeof(IModuleParamView).IsAssignableFrom(viewType))
            {
                ParamViewType = null;
            }
            else
            {
                ParamViewType = viewType;
            }
        }
    }
}
