using LS.Flowchart.Models.ToolModels;

namespace LS.Flowchart.ModuleActions
{
    /// <summary>
    /// 模块执行的接口定义
    /// </summary>
    public interface IModuleExecution
    {

        bool CanExecute(ModuleItemModel model,object parameter);

        bool Execute(ModuleItemModel model, object parameter);

    }
}
