using LS.Standard.Data;
using Newtonsoft.Json;
using System.IO;
using System;
using LS.Flowchart.Models.Configs;
using LS.Flowchart;
using LS.WPF.Core.MVVM;
using LS.Flowchart.Models.Configs;

public class ConfigParamOperation
{
    #region 配置文件
    /// <summary>
    /// 读取配置文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static BaseResult ReadConfigParam(out P_Environment param)
    {
        BaseResult res = false;
        try
        {
            if (!File.Exists(GlobalData.Path_Config_Environment))
            {
                if (!File.Exists(GlobalData.Path_Config_Environment_Backup))
                {
                    res = false;
                    param = GetDefaultConfig();
                    return res;
                }
                else
                {
                    res = ReadConfigParamBackup(out param);
                    return res;
                }
            }
            param = null;
            string content = System.IO.File.ReadAllText(GlobalData.Path_Config_Environment);
            if (!string.IsNullOrEmpty(content))
            {
                param = JsonConvert.DeserializeObject<P_Environment>(content);
            }
            if (param == null)
            {
                res = ReadConfigParamBackup(out param);
                //param = GetDefaultConfig();
                SaveConfigParam(param);
            }
            res = true;
        }
        catch (Exception ex)
        {
            LogOperate.Error("ReadConfigParam Exception", ex);
            param = GetDefaultConfig();
            res.Message = "读取配置文件失败";
        }
        return res;
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static BaseResult SaveConfigParam(P_Environment param)
    {
        BaseResult res = false;
        try
        {
            if (!Directory.Exists(GlobalData.Path_Config))
            {
                Directory.CreateDirectory(GlobalData.Path_Config);
            }
            string content = JsonConvert.SerializeObject(param);

            if (!string.IsNullOrEmpty(content))
            {
                System.IO.File.WriteAllText(GlobalData.Path_Config_Environment, content);
                res = true;
            }
            else
            {
                res = false;
            }

            //同时保存一份到备份文件中
            SaveConfigParamBackup(param);
        }
        catch (Exception ex)
        {
            LogOperate.Error("SaveConfigParam Exception", ex);
            res.Message = "保存配置文件失败";
        }
        return res;
    }


    /// <summary>
    /// 获取一个默认配置的对象
    /// </summary>
    /// <returns></returns>
    public static P_Environment GetDefaultConfig()
    {
        var cfg = new P_Environment();
        return cfg;
    }



    /// <summary>
    /// 从备份文件中读取
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    private static BaseResult ReadConfigParamBackup(out P_Environment param)
    {

        if (!File.Exists(GlobalData.Path_Config_Environment_Backup))
        {
            param = GetDefaultConfig();
        }
        else
        {
            string content = System.IO.File.ReadAllText(GlobalData.Path_Config_Environment_Backup);
            if (!string.IsNullOrEmpty(content))
            {
                param = JsonConvert.DeserializeObject<P_Environment>(content);
            }
            else
            {
                param = GetDefaultConfig();
            }
        }
        return BaseResult.Successed;
    }

    /// <summary>
    /// 保存备份配置文件
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    private static BaseResult SaveConfigParamBackup(P_Environment param)
    {
        BaseResult res = false;
        try
        {
            if (!Directory.Exists(GlobalData.Path_Config))
            {
                Directory.CreateDirectory(GlobalData.Path_Config);
            }
            string content = JsonConvert.SerializeObject(param);

            if (!string.IsNullOrEmpty(content))
            {
                System.IO.File.WriteAllText(GlobalData.Path_Config_Environment_Backup, content);
                res = true;
            }
            else
            {
                res = false;
            }
        }
        catch (Exception ex)
        {
            LogOperate.Error("SaveConfigParamBackup Exception", ex);
            res.Message = "备份配置文件失败";
        }
        return res;
    }

    #endregion

    #region 历史打开文件

    /// <summary>
    /// 读取历史打开文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    public static BaseResult ReadHistoryFiles(out P_HistoryFiles param)
    {
        BaseResult res = false;
        try
        {
            if (!File.Exists(GlobalData.Path_P_HistoryFiles))
            {
                param = new P_HistoryFiles();
                return BaseResult.Successed;
            }
            param = null;
            string content = System.IO.File.ReadAllText(GlobalData.Path_P_HistoryFiles);
            if (!string.IsNullOrEmpty(content))
            {
                param = JsonConvert.DeserializeObject<P_HistoryFiles>(content);
            }
            if (param == null)
            {
                param = new P_HistoryFiles();
            }
            return BaseResult.Successed;
        }
        catch (Exception ex)
        {
            param = new P_HistoryFiles();
            LogOperate.Error("ReadHistoryFiles Exception", ex);
            res.Message = "读取配置文件失败";
        }
        return BaseResult.Successed;
    }

    /// <summary>
    /// 保存历史打开文件
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    public static BaseResult SaveHistoryFiles(P_HistoryFiles param)
    {
        BaseResult res = false;
        try
        {
            if (!Directory.Exists(GlobalData.Path_Config))
            {
                Directory.CreateDirectory(GlobalData.Path_Config);
            }
            string content = JsonConvert.SerializeObject(param);

            if (!string.IsNullOrEmpty(content))
            {
                System.IO.File.WriteAllText(GlobalData.Path_P_HistoryFiles, content);
                res = true;
            }
            else
            {
                res = false;
            }
        }
        catch (Exception ex)
        {
            LogOperate.Error("SaveHistoryFiles Exception", ex);
            res.Message = "保存历史打开文件失败";
        }
        return res;
    }


    #endregion

}