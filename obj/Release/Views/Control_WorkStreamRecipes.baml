   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  3    -VPDLFramework.Views.Control_WorkStreamRecipes   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006        Color_DarkBackground#���   
�������.��� $��10q�.+��� 
�������$	��0.5q�� $��1q�  � RowDefinitions  �� $��30 ��� $��40 ��� .+��� $��Center=�$��Center=�$
��Bold"����� *&$WorkStream.WorkStreamInfo.StreamName+� $��1��  Color_Selected#��[ .+���  �� Orientation$ 
Horizontal=�.+��� $��30��$��30��$��2q�$
��Left=�$	����  Font_Ico#��[ $��16(�$��Black"�  Style_Button_Ico#��[  / +Lan.Control_WorkStreamRecipes.BtnSaveRecipe � ToolTip# �  ���� *CmdSaveWorkStreamRecipe+�� $��Right=�$ 
Horizontal=�.+��� $��30��$��30��$��2q�$	����#��[ $��14(�#��[  . *Lan.Control_WorkStreamRecipes.BtnAddRecipe# �  ���� *CmdAddWorkStreamRecipe+�� $��30��$��30��$��2q�$	����#��[ $��18(�$
��Bold"�#��[  1 -Lan.Control_WorkStreamRecipes.BtnRemoveRecipe# �  ���� *CmdRemoveWorkStreamRecipe+����  �� ElementName$ listRecipes�� �� Path$ SelectedItem��� -    ��Name$ listRecipes��$��2��
������ $��	0,0.5,0,0q�$
��Auto=����� *WorkStream.Recipes+����  �� 
TargetType" ��.���  �� Property � �� �� Value$	 0,2q���  � �� �� 	 �� 
TargetType"	 ��.R��� #��z��
�������$��0,0,0,2q�._�� 
 � ColumnDefinitions
 �� $��50 ��� .+��� $��30��$��30��$��2q�$��Center=�$	����#��[ $��12(�$
��Bold"�#��[  1 -Lan.Control_WorkStreamRecipes.BtnImportRecipe# �  ����  �� RelativeSource ��  �� AncestorType$% local:Control_WorkStreamRecipesL�$) #DataContext.CmdLoadWorkStreamRecipe�����  �� $ ListViewItemL�$ DataContext�9� $��1��$��0q�  Style_Expander#��[ ���� *
RecipeName+.��� .+��� $��2q�
�������$	��0.5q� �� CornerRadius$ 0��._��   �� $��30 ��� .+��� ._���  �� Padding$ 3q�$��Center=�$��Center=� .	 *Lan.Control_WorkStreamRecipes.TextParaList#��� 	 &   System.Windows.Controls.DataGrid  $��1��$��0q�$	��0.5q� 
 Style_DataGrid#��[
 ���� *Values+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 200  ! Lan.Control_WorkEdit.TextName   Header# �     CellTemplate �� .R��� $ 3q�$��Center=�$
��Bold"����� *Key+  $	 200  ! Lan.Control_WorkEdit.TextType# �   �� .R��� $ 3q�$��Center=����� *Type+ �� *Value+   + 'Lan.Control_WorkStreamRecipes.TextValue# �   �� .R��� $	��100��$��2q�$��3q�$��Center=����� *Value+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=� �� *Value+   6 2Lan.Control_WorkStreamRecipes.TextDescriptionLabel# �   �� .R��� $��3q�$��3q�$��Center=�
�������$
��Bold"� �� 
IsReadOnly ��  �� $ ExpanderL�$$ DataContext.DisableLabelModify����� *Label+$ TwoWay=�$ PropertyChanged=� �� *Label+ �� Triggers P�  P� Property �@��
IsSelected P� Value . . ���  � ��# [ 