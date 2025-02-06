using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VPDLFramework.Models
{
    public class ECFileConstantsManager
    {
        /// <summary>
        /// 程序名称
        /// </summary>
        public static string ProgramName = (string)Application.Current.TryFindResource("string_ProgramName");

        /// <summary>
        /// 程序大版本号
        /// </summary>
        public static string ProgramMajorVersion = (string)Application.Current.TryFindResource("string_MajorVersion");

        /// <summary>
        /// 程序大版本号
        /// </summary>
        public static string ProgramMinorVersion = (string)Application.Current.TryFindResource("string_MinorVersion");

        /// <summary>
        /// 本地磁盘集合
        /// </summary>
        public static System.IO.DriveInfo[] Disks = System.IO.DriveInfo.GetDrives();

        /// <summary>
        /// Default选择本地磁盘第一个盘作为根目录
        /// </summary>
        public static string RootDisk = Disks[0].Name;

        /// <summary>
        /// VPCameraOrder.ini文件Default路径
        /// </summary>
        public static string VproCameraOrderIniFilePtah = Disks[0].Name + @"\Users\Public\Cognex\Common\VPCameraOrder.ini";

        /// <summary>
        /// 程序启动配置文件目录
        /// </summary>
        public static string ProgramStartupConifgFolder =Directory.GetParent(Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName) + @"\" + "ProgramSetup";

        /// <summary>
        /// 工作项目根目录名称
        /// </summary>
        public static string RootFolderName = "VisionProFramework";

        /// <summary>
        /// 图片根目录名称
        /// </summary>
        public static string ImageRootFolderName = "VisionProFrameworkImage";

        /// <summary>
        /// 工作项目根目录
        /// </summary>
        public static string RootFolder = RootDisk + $"\\{RootFolderName}\\";

        /// <summary>
        /// 软件程序目录
        /// </summary>
        public static string ProjectFolder = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        /// <summary>
        /// 图片根目录
        /// </summary>
        public static string ImageRootFolder = RootDisk + $"\\{ImageRootFolderName}\\";

        /// <summary>
        /// 授权文件目录
        /// </summary>
        public static string LicenseFolder = ProjectFolder + @"\" + "License";

        /// <summary>
        /// 授权文件名称
        /// </summary>
        public static string LicenseFileName = "license.json";


        /// <summary>
        /// 语言文件目录
        /// </summary>
        public static string LanguagesFolder = ProjectFolder + @"\" + "Languages";

        /// <summary>
        /// 标准文件目录
        /// </summary>
        public static string StdFilesFolder = ProjectFolder + @"\StdFiles\";

        /// <summary>
        /// 标准采集Toolblock
        /// </summary>
        public static string StdTB_AcqPath = StdFilesFolder + "TB_StdAcq.vpp";

        /// <summary>
        /// 标准本地图像Toolblock
        /// </summary>
        public static string StdTB_LocalImagePath = StdFilesFolder + "TB_StdLocalImage.vpp";

        /// <summary>
        /// 标准ViDi输入Toolblock
        /// </summary>
        public static string StdTB_DLInputPath = StdFilesFolder + "TB_StdDLInput.vpp";

        /// <summary>
        /// 标准ViDi输出Toolblock
        /// </summary>
        public static string StdTB_DLOutputPath = StdFilesFolder + "TB_StdDLOutput.vpp";

        /// <summary>
        /// 标准组Toolblock
        /// </summary>
        public static string StdTB_GroupPath = StdFilesFolder + "TB_StdGroup.vpp";

        /// <summary>
        /// 实时显示Toolblock
        /// </summary>
        public static string StdTB_LiveModePath = StdFilesFolder + "TB_LiveMode.vpp";

        /// <summary>
        /// 标准Ffp脚本文件名
        /// </summary>
        public static string Std_FfpScriptName = "Std_FfpScript.cs";

        /// <summary>
        /// 第三方卡脚本文件名
        /// </summary>
        public static string Std_ThirdCardScriptName = "Std_ThirdCardScript.cs";

        /// <summary>
        /// 图像源文件夹名称
        /// </summary>
        public static string ImageSourceFolderName = "ImageSourcesConfig";

        /// <summary>
        /// 工作流文件夹名称
        /// </summary>
        public static string StreamsFolderName = "StreamsConfig";

        /// <summary>
        /// 工作流文件夹名称
        /// </summary>
        public static string RecipesFolderName = "Recipes";

        /// <summary>
        /// TCP通讯配置文件夹名称
        /// </summary>
        public static string TCPConfigFolderName = "TCPConfig";

        /// <summary>
        /// 通讯板卡配置文件夹名称
        /// </summary>
        public static string CommCardConfigFolderName = "CommCardConfig";

        /// <summary>
        /// 工作流高级DL模式配置文件夹名称
        /// </summary>
        public static string AdvancedDLModelFolderName = "AdvancedDLModel";

        /// <summary>
        /// 保存图像的文件夹名称
        /// </summary>
        public static string ImageRecordFolderName = "ImageRecord";

        /// <summary>
        /// 原始图像的文件夹名称
        /// </summary>
        public static string OriginalImageFolderName = "Original";

        /// <summary>
        /// 原始图像的文件夹名称
        /// </summary>
        public static string GraphicImageFolderName = "Graphic";

        /// <summary>
        /// 日志文件夹名称
        /// </summary>
        public static string DatabaseFolderName = "Database";

        /// <summary>
        /// 工作日志文件夹名称
        /// </summary>
        public static string WorkLogFolderName = "WorkLog";

        /// <summary>
        /// 工作流数据文件夹名称
        /// </summary>
        public static string WorkStreamsDataFolderName = "StreamsData";

        /// <summary>
        /// ViDi工作区文件夹名称
        /// </summary>
        public static string WorkspaceFolderName = "Workspaces";

        /// <summary>
        /// 工作流组文件夹名称
        /// </summary>
        public static string GroupsFolderName = "GroupsConfig";

        /// <summary>
        /// 启动配置文件名称
        /// </summary>
        public static string StartupConfigName = "startupConfig.json";

        /// <summary>
        /// 工作信息文件名称
        /// </summary>
        public static string WorkInfoFileName = "workInfo.json";

        /// <summary>
        /// 图像源配置文件名称
        /// </summary>
        public static string ImageSourceConfigName = "imageSourceConfig.json";

        /// <summary>
        /// 工作流配置文件名称
        /// </summary>
        public static string StreamConfigName = "streamConfig.json";

        /// <summary>
        /// TCP配置文件名称
        /// </summary>
        public static string TCPConfigName = "tcpConfig.json";

        /// <summary>
        /// 工作流组配置文件名称
        /// </summary>
        public static string GroupConfigName = "groupConfig.json";

        /// <summary>
        /// IO输入配置文件名称
        /// </summary>
        public static string IOInputConfigName = "ioInputConfig.json";

        /// <summary>
        /// IO输出源配置文件名称
        /// </summary>
        public static string IOOutputConfigName = "ioOutputConfig.json";

        /// <summary>
        /// Ffp脚本文件名称
        /// </summary>
        public static string FfpScriptName = "FfpInputScript.cs";

        /// <summary>
        /// 第三方板卡脚本文件名称
        /// </summary>
        public static string ThirdCardScriptName = "ThirdCardScript.cs";

        /// <summary>
        /// Ffp脚本dll文件名称
        /// </summary>
        public static string FfpScriptDllName = "ECFfpScriptProcessor.dll";

        /// <summary>
        /// 第三方板卡脚本dll文件名称
        /// </summary>
        public static string ThirdCardScriptDllName = "ECThirdCardScriptProcessor.dll";

        /// <summary>
        /// Ffp脚本配置文件名称
        /// </summary>
        public static string FfpScriptConfigName = "FfpScriptConfig.json";

        /// <summary>
        /// 第三方板卡脚本配置文件名称
        /// </summary>
        public static string ThirdCardScriptConfigName = "ThirdCardScriptConfig.json";

        /// <summary>
        /// IO输出源配置文件名称
        /// </summary>
        public static string AdvancedToolConfigName = "advancedToolConfig.json";

        /// <summary>
        /// 配方文件名称
        /// </summary>
        public static string RecipeConfigName = "recipeConfig.json";

        /// <summary>
        /// 图像源Toolblock文件名称
        /// </summary>
        public static string ImageSourceAcqTBName = "TB_Acq.vpp";

        /// <summary>
        /// 组Toolblock文件名称
        /// </summary>
        public static string GroupTBName = "TB_Group.vpp";

        /// <summary>
        /// DL输入Toolblock文件名称
        /// </summary>
        public static string DLInputTBName = "TB_DLInput.vpp";

        /// <summary>
        /// DL输出Toolblock文件名称
        /// </summary>
        public static string DLOutputTBName = "TB_DLOutput.vpp";

        /// <summary>
        /// DL输出Toolblock文件名称
        /// </summary>
        public static string AdvancedToolTBName = "TB_Advanced.vpp";

        /// <summary>
        /// 工作流数据文件名称
        /// </summary>
        public static string StreamDatabaseFileName = "StreamDataDB.db3";

        /// <summary>
        /// 工作流组数据文件名称
        /// </summary>
        public static string GroupDatabaseFileName = "GroupDataDB.db3";

        /// <summary>
        /// 工作流数据文件表名称
        /// </summary>
        public static string StreamDatabaseTableName = "DataTable";

        /// <summary>
        /// 工作流组数据文件表名称
        /// </summary>
        public static string GroupDatabaseTableName = "DataTable";

        /// <summary>
        /// 工作项目日志文件名称
        /// </summary>
        public static string LogDatabaseFileName = "WorkLogDB.db3";

        /// <summary>
        /// 工作项目日志文件表名称
        /// </summary>
        public static string LogDatabaseTableName = "LogTable";

        /// <summary>
        /// 软件帮助文件路径
        /// </summary>
        public static string HelpFilePath = ProjectFolder + @"\" + @"Help\" + "VisionproFrameworkHelp.chm";

        /// <summary>
        /// 图像源文件夹配置文件名称
        /// </summary>
        public static string ImageFolderConfigName = "imgfolder_cfg.json";

        /// <summary>
        /// 图像源文件夹配置文件名称
        /// </summary>
        public static string LayoutConfigName = "layout_cfg.json";
        
    }
}

