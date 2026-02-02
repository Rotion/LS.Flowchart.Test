using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LS.Flowchart.ModuleActions
{
    /// <summary>
    /// 模块执行对应的实现类的特性
    /// </summary>
    public class ModuleExecutionAttribute : Attribute
    {
        public Type ExecutionType { get; private set; }
        public ModuleExecutionAttribute(Type executionType)
        {
            if (!typeof(IModuleExecution).IsAssignableFrom(executionType))
            {
                ExecutionType = null;
            }
            else
            {
                ExecutionType = executionType;
            }
        }
    }
}
