using System;

namespace LS.Flowchart.Tools
{
    public static class IDHelper
    {

        /// <summary>
        /// 获取GUID的名称，下划线开头
        /// </summary>
        /// <returns></returns>
        public static string GetGuidName()
        {
            return "_" + Guid.NewGuid().ToString().Replace("-", "");
        }


        /// <summary>
        /// 获取GUID的ID标识符
        /// </summary>
        /// <returns></returns>
        public static string GetGuidId()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }
    }
}
