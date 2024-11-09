using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Walterlv.Win32;

namespace GrassDemoPark.WPF2.Tiny.RegEdit
{
    /*
     * [如何为你的 Windows 应用程序关联一种或多种文件类型 - walterlv]
     * (https://blog.walterlv.com/post/windows-file-type-association.html)
     */

    /// <summary>
    /// 向注册表中注册可执行程序
    /// </summary>
    class RegisterProgram
    {
        /// <summary>
        /// 需要管理的执行程序的产品ID，(厂商名.应用名.版本号)
        /// e.g. Microsoft.PowerShellConsole.1
        /// </summary>
        public string ProgramId { get; }

        /// <summary>
        /// 该执行程序所关联文件的类型描述
        /// e.g. Text Document
        /// </summary>
        public string? TypeName { get; set; }

        /// <summary>
        /// 该执行程序所关联文件的类型描述
        /// e.g. 一个神奇的文本文件
        /// </summary>
        public string? FriendlyTypeName { get; set; }

        /// <summary>
        /// 该执行程序所关联文件对应的 Icon
        /// </summary>
        public string? DefaultIcon { get; set; }

        /// <summary>
        /// 是否总是显示指定文件类型的扩展名
        /// </summary>
        public bool? IsAlwaysShowExt { get; set; }

        /// <summary>
        /// 该执行程序可执行的操作/谓词
        /// </summary>
        public string? Operation { get; set; }

        /// <summary>
        /// 对应谓词下，其执行的具体命令；仅在<see cref="Operation"/>有效时，此值才有效
        /// </summary>
        public string? Command { get; set; }

        /// <summary>
        /// 根据指定 ProgramId，创建 <see cref="RegisterProgram"/> 的实例。
        /// </summary>
        /// <param name="programId"></param>
        public RegisterProgram(string programId)
        {
            if (string.IsNullOrWhiteSpace(programId))
            {
                throw new ArgumentNullException(nameof(programId));
            }

            ProgramId = programId;
        }

        /// <summary>
        /// 将此文件扩展名注册到当前用户的注册表中
        /// </summary>
        public void WriteToCurrentUser()
        {
            WriteToRegistry(RegistryHive.CurrentUser);
        }

        /// <summary>
        /// 将此文件扩展名注册到所有用户的注册表中。（进程需要以管理员身份运行）
        /// </summary>
        public void WriteToAllUser()
        {
            WriteToRegistry(RegistryHive.LocalMachine);
        }

        /// <summary>
        /// 将此文件扩展名写入到注册表中
        /// </summary>
        private void WriteToRegistry(RegistryHive registryHive)
        {
            // 写 默认描述
            registryHive.Write32(BuildRegistryPath(ProgramId), TypeName ?? string.Empty);

            // 写 FriendlyTypeName
            if (FriendlyTypeName != null && !string.IsNullOrWhiteSpace(FriendlyTypeName))
            {
                registryHive.Write32(BuildRegistryPath(ProgramId), "FriendlyTypeName", FriendlyTypeName);
            }

            // 写 IsAlwaysShowExt
            if (IsAlwaysShowExt != null)
            {
                registryHive.Write32(BuildRegistryPath(ProgramId), "IsAlwaysShowExt", IsAlwaysShowExt.Value ? "1" : "0");
            }

            // 写 Icon 
            if (DefaultIcon != null && !string.IsNullOrWhiteSpace(DefaultIcon))
            {
                registryHive.Write32(BuildRegistryPath($"{ProgramId}\\DefaultIcon"), DefaultIcon);
            }

            // 写 Command
            if (Operation != null && !string.IsNullOrWhiteSpace(Operation))
            {
                registryHive.Write32(BuildRegistryPath($@"{ProgramId}\shell\{Operation}\command"), Command ?? string.Empty);
            }
        }

        private string BuildRegistryPath(string relativePath)
        {
            return $@"Software\Classes\{relativePath}";
        }
    }
}