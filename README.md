# LS.Flowchart
流程图架构

# 项目背景

1.项目背景： 设计一款软件，以流程图的方式做界面设计，按流程执行作业
2.使用开发语言 C#
3.UI使用WPF 作为开发设计框架
4.运行框架使用 .NET 8.0


# 设计要求：

1.软件界面设计成模块拖放的方式进行流程设计
2.以连线的方式进行设计作业流程
3.每个模块有对应的参数配置，和输入输出参数，以及触发条件等
4.当前模块能获取前面节点所有模块的输出参数

# 方案结构说明
* Asset   图片等资源存放目录
* Cameras 工业相机对接SDK
* Components 设备组件，如TCP，UDP，串口等通讯基础实现
* Models 数据模型 定义
* ModuleActions 模块方法的定义
* ModuleControls 模块的控件定义，已经一些公共控件或窗口的定义
* ModuleParamView 模块的参数界面定义，双击模块会弹出参数配置，每个模块按参数定义来动态生成控件显示，也会按模块进行扩展，需要实现接口IModuleParamView
* Operation 全局操作类
* Styles 样式
* Tools 帮助类方法
* UCControls 自定义控件，如模块列表，连接线等定义
* ViewModels UI界面对应的VM定义
* Views UI界面定义