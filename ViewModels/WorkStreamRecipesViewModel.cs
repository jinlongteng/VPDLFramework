using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using VPDLFramework.Models;

namespace VPDLFramework.ViewModels
{
    public class WorkStreamRecipesViewModel:ViewModelBase
    {
        public WorkStreamRecipesViewModel(string workName, ECWorkStream workStream) 
        { 
            _workName = workName;
            _workStream = workStream;
            BindCmd();
        }

        /// <summary>
        /// 工作名
        /// </summary>
        private string _workName;

        /// <summary>
        /// 工作流
        /// </summary>
        private ECWorkStream _workStream;

        public ECWorkStream WorkStream
        {
            get { return _workStream; }
            set { _workStream = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// 添加工作流配方
        /// </summary>
        public RelayCommand CmdAddWorkStreamRecipe { get; set; }

        /// <summary>
        /// 删除工作流配方
        /// </summary>
        public RelayCommand<object> CmdRemoveWorkStreamRecipe { get; set; }

        /// <summary>
        /// 加载工作流配方
        /// </summary>
        public RelayCommand<object> CmdLoadWorkStreamRecipe { get; set; }

        /// <summary>
        /// 保存工作流配方
        /// </summary>
        public RelayCommand<object> CmdSaveWorkStreamRecipe { get; set; }

        /// <summary>
        /// 绑定命令
        /// </summary>
        private void BindCmd()
        {
            CmdAddWorkStreamRecipe = new RelayCommand(AddWorkStreamRecipe);
            CmdRemoveWorkStreamRecipe = new RelayCommand<object>(RemoveWorkStreamRecipe);
            CmdLoadWorkStreamRecipe = new RelayCommand<object>(LoadWorkStreamRecipe);
            CmdSaveWorkStreamRecipe = new RelayCommand<object>(SaveWorkStreamRecipe);
        }

        /// <summary>
        /// 添加工作流配方
        /// </summary>
        /// <param name="obj"></param>
        private void AddWorkStreamRecipe()
        {
            try
            {
                string name = ECDialogManager.GetUserInput(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.PleaseInputName));
                if (name == null || name.Trim() == "") return;
                if ((new Regex(@"^[A-Za-z_]{1}\w{0,100}$").IsMatch(name)))
                {
                    // 检查是否存在重复名称
                    foreach (ECRecipe recipe in WorkStream.Recipes)
                    {
                        if (name == recipe.RecipeName)
                        {
                            ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.DuplicateName));
                            return;
                        }
                    }
                    // 添加新的配方
                    if (WorkStream.Recipes == null)
                        WorkStream.Recipes = new BindingList<ECRecipe>();
                    ECRecipe newRecipe = ECRecipesManager.FilterValidRecipe(WorkStream.WorkStreamInfo.IsOnlyVpro ? WorkStream.DLOutputTB.Inputs : WorkStream.DLInputTB.Inputs, name);
                    if (newRecipe != null && newRecipe.Values.Count > 0)
                        WorkStream.Recipes.Add(newRecipe);
                    else
                        ECDialogManager.ShowMsg($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ValidType)} (int,double,string)");

                }
                else
                    ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.InvalidName));
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace+ex.Message,NLog.LogLevel.Error);
            }

        }

        /// <summary>
        /// 添加工作流配方
        /// </summary>
        /// <param name="obj"></param>
        private void RemoveWorkStreamRecipe(object obj)
        {
            if (obj == null) { return; }
            ECRecipe recipe = obj as ECRecipe;
            if (ECDialogManager.Verify($"{ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ConfirmRemove)}\"{recipe.RecipeName}\""))
                WorkStream.Recipes.Remove(recipe);

        }

        /// <summary>
        /// 加载工作流配方
        /// </summary>
        /// <param name="obj"></param>
        private void LoadWorkStreamRecipe(object obj)
        {
            if (obj == null) { return; }
            ECRecipe recipe = obj as ECRecipe;
            if (WorkStream.LoadRecipe(recipe.RecipeName))
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImportFinished));
            else
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.ImportFailed));
        }

        /// <summary>
        /// 保存工作流配方
        /// </summary>
        /// <param name="obj"></param>
        private void SaveWorkStreamRecipe(object obj)
        {
            try
            {
                string recipesDirectory = ECFileConstantsManager.RootFolder + @"\" + _workName + @"\" + ECFileConstantsManager.StreamsFolderName + @"\" + WorkStream.WorkStreamInfo.StreamName + @"\" + ECFileConstantsManager.RecipesFolderName;
                ECRecipesManager.SaveRecipe(recipesDirectory, WorkStream.Recipes);
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFinished));
            }
            catch(Exception ex)
            {
                ECDialogManager.ShowMsg(ECDescriptionLabel.FindLabel(ECDescriptionLabel.LabelConstants.SaveFailed));
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
        }
    }
}
