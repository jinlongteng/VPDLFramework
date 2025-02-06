using Microsoft.CodeAnalysis;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Reflection;
using VPDLFramework.Models;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Collections.Generic;

namespace VPDLFramework.Views
{
    /// <summary>
    /// Interaction logic for Window_EditScript.xaml
    /// </summary>
    public partial class Window_EditScript : Window,INotifyPropertyChanged
    {
        public Window_EditScript(string workname,bool isThirdCard,string scriptPath,string configPath)
        {
            WorkName = workname;
            IsThirdCard = isThirdCard;
            ScriptPath = scriptPath;
            ConfigPath = configPath;
            InitializeComponent();
            SetDefaultValue();
            ReadAssemblyConfig();
            SetScript();
            
            this.DataContext = this;
        }

        #region 方法
        /// <summary>
        /// 设置Default值
        /// </summary>
        private void SetDefaultValue()
        {
            IsScriptCompiledSucceed = true;
            ScriptAssembly = new BindingList<string> { "System.dll", "System.Runtime.dll", "VPDLFrameworkLib.dll" };
            ScriptCompiledResult = new BindingList<string>();
        }

        /// <summary>
        /// 设置脚本内容
        /// </summary>
        public void SetScript()
        {
            _scriptCompletion = new CSharpCompletion();

            foreach (string dll in ScriptAssembly)
            {
                try
                {
                    if(File.Exists(Directory.GetCurrentDirectory()+@"\"+ dll))
                        _scriptCompletion.AddAssembly(dll);
                }
                catch { }
            }
            _scriptEditor = new CodeTextEditor();

            SetEditorStyle(_scriptEditor);

            _scriptEditor.Completion=_scriptCompletion;

            _scriptEditor.OpenFile(ScriptPath);

            inputScriptGrid.Children.Add(_scriptEditor);
        }

        /// <summary>
        /// 设置编辑器风格
        /// </summary>
        /// <param name="editor"></param>
        private void SetEditorStyle(CodeTextEditor editor)
        {
            editor.FontFamily = new FontFamily("Consolas");
            editor.FontSize = 12;
            editor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("C#");
        }

        /// <summary>
        /// 编译输入脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCompileScript_Click(object sender, RoutedEventArgs e)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters objCompilerParameters = new CompilerParameters();
            foreach (string dll in ScriptAssembly)
            {
                try
                {
                    objCompilerParameters.ReferencedAssemblies.Add(dll);
                }
                catch { }
            }
            objCompilerParameters.GenerateExecutable = false;
            objCompilerParameters.GenerateInMemory = false;
            if(IsThirdCard)
                objCompilerParameters.OutputAssembly = ECFileConstantsManager.RootFolder + @"\" + WorkName + @"\"
                                                  + ECFileConstantsManager.CommCardConfigFolderName + @"\" + ECFileConstantsManager.ThirdCardScriptDllName;
            else
                objCompilerParameters.OutputAssembly =ECFileConstantsManager.RootFolder+@"\"+WorkName+@"\"
                                                    +ECFileConstantsManager.CommCardConfigFolderName + @"\"+ECFileConstantsManager.FfpScriptDllName;

            CompilerResults cr = provider.CompileAssemblyFromSource(objCompilerParameters,_scriptEditor.Text);

            ScriptCompiledResult.Clear();
            if (cr.Errors.HasErrors)
            {
                IsScriptCompiledSucceed = false;
                int errorCount = 0, warnningCount = 0;
                foreach (CompilerError err in cr.Errors)
                {
                    if (!err.IsWarning)
                    {
                        ScriptCompiledResult.Add( ">Error, " + "Row " + err.Line +
                                                    ", ErrorCode: " + err.ErrorNumber +
                                                    ", '" + err.ErrorText + "'");
                        errorCount++;
                    }
                }
                foreach (CompilerError err in cr.Errors)
                {
                    if (err.IsWarning)
                    {
                        ScriptCompiledResult.Add(">Warnning, " + "Row " + err.Line +
                                                    ", ErrorCode: " + err.ErrorNumber +
                                                    ", '" + err.ErrorText + "'");
                        warnningCount++;
                    }
                }
                ScriptCompiledResult.Add($"<=========={DateTime.Now.ToString()} Compile Failed! Error {errorCount.ToString()}, Warnnig {warnningCount.ToString()} ==========>");
            }
            else
            {
                IsScriptCompiledSucceed = true;
                ScriptCompiledResult.Add($"<=========={DateTime.Now.ToString()} Compile Succeed! Error 0, Warnning 0 ==========>");
     
                WriteScriptToFile(ScriptPath,_scriptEditor.Text);
                WriteAssemblyConfig();
            }
        }

        /// <summary>
        /// 写入编译成功的脚本到文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="script"></param>
        private void WriteScriptToFile(string path, string script)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.Write(script);
            sw.Close();
        }

        /// <summary>
        /// 添加dll,返回添加的文件名
        /// </summary>
        /// <returns></returns>
        private List<string> AddReference()
        {
            List<string> references = new List<string>();
            // 创建一个OpenFileDialog实例
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                // 设置一些基本属性，如过滤器
                Filter = "dll files (*.dll)|*.dll",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            // 可以选择多个
            openFileDialog.Multiselect = true;

            // 显示对话框

            // 注意：ShowDialog方法将返回一个可空的bool值，当用户选择文件并点击“打开”时为true

            // 主程序exe目录
            string directory =Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (openFileDialog.ShowDialog() == true)
            {
                // 获取用户选择的文件路径
                string[] files = openFileDialog.FileNames;

                // 将添加的dll保存到程序目录
                foreach (string file in files)
                {
                    try
                    {
                        string dstPath = directory + @"\" + Path.GetFileName(file);
                        if (!File.Exists(dstPath))
                        {
                            File.Copy(file, dstPath);
                        }
                        references.Add(Path.GetFileName(dstPath));
                    }
                    catch (Exception ex)
                    {
                        ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                    }
                }
            }
            return references;
        }

        /// <summary>
        /// 添加引用到输入脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddReference_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> dlls = AddReference();
                foreach (string dll in dlls)
                    if (!ScriptAssembly.Contains(dll))
                    {
                        ScriptAssembly.Add(dll);
                        _scriptCompletion.AddAssembly(dll);
                    }
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 写入输入脚本引用配置
        /// </summary>
        private void WriteAssemblyConfig()
        {
            try
            {
                ECScriptConfigInfo configInfo = new ECScriptConfigInfo();
                configInfo.ScriptAssembly = ScriptAssembly;
                configInfo.IsDebugMode = IsDebugMode;
                ECSerializer.SaveObjectToJson(ConfigPath, configInfo);
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message, NLog.LogLevel.Error);
            }
        }

        /// <summary>
        /// 读取输入脚本引用配置
        /// </summary>
        private void ReadAssemblyConfig()
        {
            if (File.Exists(ConfigPath))
            {
                ECScriptConfigInfo configInfo = ECSerializer.LoadObjectFromJson<ECScriptConfigInfo>(ConfigPath);
                if(configInfo != null)
                {
                    ScriptAssembly=configInfo.ScriptAssembly;
                    IsDebugMode = configInfo.IsDebugMode;
                }
            }
        }

        #endregion

        #region 字段
        /// <summary>
        /// 输入脚本代码环境
        /// </summary>
        private CSharpCompletion _scriptCompletion;

        /// <summary>
        /// 输入脚本编辑器
        /// </summary>
        private CodeTextEditor _scriptEditor;

        /// <summary>
        /// 工作名称
        /// </summary>
        public string WorkName;

        #endregion

        #region PropertyChanged事件

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisedPropertyChanged(string name)
        {
            if(this.PropertyChanged!=null) 
                this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region 属性

        /// <summary>
        /// 输入脚本编译成功
        /// </summary>
        private bool _isScriptCompiledSucceed;  

        public bool IsScriptCompiledSucceed
        {
            get { return _isScriptCompiledSucceed; }
            set {
                _isScriptCompiledSucceed = value;
                RaisedPropertyChanged(nameof(IsScriptCompiledSucceed));
            }
        }

        /// <summary>
        /// 输入脚本引用的dlls
        /// </summary>
        private BindingList<string> _scriptAssembly;

        public BindingList<string> ScriptAssembly
        {
            get { return _scriptAssembly; }
            set {
                _scriptAssembly = value;
                RaisedPropertyChanged(nameof(ScriptAssembly));
            }
        }

        /// <summary>
        /// 输入脚本编译结果
        /// </summary>
        private BindingList<string> _scriptCompiledResult;

        public BindingList<string> ScriptCompiledResult
        {
            get { return _scriptCompiledResult; }
            set { _scriptCompiledResult = value;
                RaisedPropertyChanged(nameof(ScriptCompiledResult));
            }
        }

        /// <summary>
        /// 使用第三方板卡
        /// </summary>
        private bool _isThirdCard;

        public bool IsThirdCard
        {
            get { return _isThirdCard; }
            set { _isThirdCard = value;
                RaisedPropertyChanged(nameof(IsThirdCard));
            }
        }

        /// <summary>
        /// 脚本路径
        /// </summary>
        private string _scriptPath;

        public string ScriptPath
        {
            get { return _scriptPath; }
            set { _scriptPath = value; }
        }

        /// <summary>
        /// 引用配置文件路径
        /// </summary>
        private string _configPath;

        public string ConfigPath
        {
            get { return _configPath; }
            set { _configPath = value; }
        }

        private bool _isDebugMode;

        public bool IsDebugMode
        {
            get { return _isDebugMode; }
            set { _isDebugMode = value;
                RaisedPropertyChanged(nameof(IsDebugMode));
            }
        }

        #endregion
    }
}
