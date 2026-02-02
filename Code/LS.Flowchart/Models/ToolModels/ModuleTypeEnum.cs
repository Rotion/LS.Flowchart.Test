using LS.Standard.Data;
using LS.Flowchart.ModuleActions;
using LS.Flowchart.ModuleActions.Images;
using LS.Flowchart.ModuleParamView;
using LS.Flowchart.ModuleParamView.Images;

namespace LS.Flowchart.Models.ToolModels
{
    public enum ModuleTypeEnum
    {

        [ModuleParamView(typeof(ImageSourceWindow))]
        [ModuleExecution(typeof(ImageSourceExecution))]
        [EnumName("图像源")]
        ImageSource = 0,
        [ModuleExecution(typeof(MultiImageExecution))]
        [EnumName("多图采集")]
        MultiImage = 1,
        [ModuleExecution(typeof(ImageOutputExecution))]
        [EnumName("输出图像")]
        ImageOutput = 2,
        [ModuleExecution(typeof(LightSourceExecution))]
        [EnumName("光源")]
        LightSource = 3,



        [EnumName("二维码识别")]
        QR_Recognize = 30,
        [EnumName("条码识别")]
        BarCode_Recognize = 31,
        [EnumName("字符识别")]
        Char_Recognize = 32,
    }
}
