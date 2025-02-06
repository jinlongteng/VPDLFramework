using Cognex.VisionPro.ToolBlock;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace VPDLFramework.Models
{
    public class ECRecipesManager
    {
        /// <summary>
        /// 获取配方
        /// </summary>
        /// <param name="path">配方路径</param>
        /// <returns>返回配方包含的数值列表</returns>
        public static BindingList<ECRecipe> GetRecipes(string recipesPath)
        {
            BindingList<ECRecipe> recipes = new BindingList<ECRecipe> ();
            try
            {
                if (Directory.Exists(recipesPath))
                {
                    string[] folders= Directory.GetDirectories(recipesPath);
                    foreach (string folder in folders)
                    {
                        string jsonPath = folder + @"\" + ECFileConstantsManager.RecipeConfigName;
                        ECRecipe recipe=ECSerializer.LoadObjectFromJson<ECRecipe>(jsonPath);
                        recipes.Add(recipe);
                    }
                }               
            }
            catch(Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
            }
            return recipes;
        }

        /// <summary>
        /// 保存配方
        /// </summary>
        /// <param name="path"></param>
        /// <param name="recipe"></param>
        /// <returns>保存成功返回True,否则范湖False</returns>
        public static bool SaveRecipe(string recipesPath,BindingList<ECRecipe> recipes)
        {
            try
            {
                if(Directory.Exists(recipesPath)) Directory.Delete(recipesPath,true);
                Directory.CreateDirectory(recipesPath);
                // 遍历所有配方
                foreach(ECRecipe recipe in recipes)
                {
                    string recipeFolder = recipesPath + @"\" + recipe.RecipeName;
                    // 创建文件夹
                    Directory.CreateDirectory(recipeFolder);
                    
                    // 配置文件路径
                    string jsonPath= recipeFolder + @"\"+ECFileConstantsManager.RecipeConfigName;
                    ECSerializer.SaveObjectToJson(jsonPath, recipe);
                }
                
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 从集合中过滤可以保存到配方的参数,可保存的数值类型包含Bool、Int、Double、String
        /// </summary>
        /// <param name="terminals"></param>
        /// <returns></returns>
        public static ECRecipe FilterValidRecipe(CogToolBlockTerminalCollection terminals,string recipeName) 
        {
            try
            {
                ECRecipe recipe = new ECRecipe(recipeName);
                foreach (CogToolBlockTerminal terminal in terminals)
                {
                    if (terminal.ValueType == typeof(bool) || terminal.ValueType == typeof(int) || terminal.ValueType == typeof(double) || terminal.ValueType == typeof(string))
                        if(terminal.Name!="DefaultUserData")
                        {
                            recipe.Values.Add(new ECKeyValuePair(terminal.Name, terminal.Value,terminal.ValueType.FullName));
                        }
                }
                return recipe;
            }
            catch (Exception ex)
            {
                ECLog.WriteToLog(ex.StackTrace + ex.Message, NLog.LogLevel.Error);
                return null;
            }
        }
    }
}
