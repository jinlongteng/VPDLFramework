   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  4    .VPDLFramework.Views.Window_EditAdvancedDLModel   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006      9  5Lan.Window_EditAdvancedDLMode.TitleEditAdvancedDLMode  � Title#  �   $
��1200��$	��800��$	��900��$	��600��  Color_DarkBackground#���   � WindowStartupLocation$ CenterScreen=� � WindowStyle$ SingleBorderWindow=�.��� .+��� 
�������$��1q�._��  � RowDefinitions �� $��40 ��� .+���  �� Orientation$ 
Horizontal=�.+��� $��5q�$��5q�  Style_Button_Ico#��[ ���� *CmdAddAdvancedDLModelStep+.���  � ColumnDefinitions �� �� .+��� $��2q�$
��Left=�$��Center=�  Font_Ico#��[ $��16(�$	������ $��1��$��Center=�$��Center=� , (Lan.Window_EditAdvancedDLMode.BtnAddStep#���  �� $��5q�$��5q�#��[ ���� *" CmdAddAdvancedDLToolDLTypeToStep+����  �� ElementName$ listStep�� �� Path$ SelectedItem�.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� , (Lan.Window_EditAdvancedDLMode.BtnAddTool#���  �� $��5q�$��5q�#��[ ���� *%#CmdAddAdvancedDLToolNotDLTypeToStep+���� $ listStep��$ SelectedItem�.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=�#���  �� $��5q�$��5q�#��[ ���� * CmdUpdateAdvancedModelParaList+.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� 0 ,Lan.Window_EditAdvancedDLMode.BtnRefreshPara#���  �� -    ��Name$ listStep��$��1��#��[ 
�������$��0,1,0,0q�
�������$��Disabled=����� *WorkStream.AdvancedDLSteps+���� 	 �� 
TargetType"	 ��.��� 
 �� Property
 � �� �� Value$	 0,2q��� 
 � �� ��  �� 
TargetType" ��.R��� #��z��
�������$��0,0,0,2q�._��  �� $��50 ��� .+��� $ Vertical=�.+��� $	��Top=�$ 
Horizontal=�.+���  �� Padding$ 2q�$��Center=�$��Black"����� *
StepName+�� $��30��$��30��$
��Left=�$	����#��[ $��16(�  	Color_Red#��[ #��[ ����  �� RelativeSource ��  �� AncestorType$ WindowL�$. (DataContext.CmdRemoveAdvancedDLModelStep�����  �� $ ListViewItemL�$ DataContext�9� $��1��$��0q�  Style_Expander#��[ .���  �� �� $��50 ��� �� .+��� $��2q�
�������$	��0.5q� �� CornerRadius$ 0��._��  �� $��30 ��� .+��� 
�������$��	0,0,0,0.5q�._��� $ 3q�$��Center=�$��Center=� 3	 /Lan.Window_EditAdvancedDLMode.TextInputParaList#��� 	 &   System.Windows.Controls.DataGrid  $��1��$	��200��$��0q�$��0q� 
 Style_DataGrid#��[
 ���� *InputParaList+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ *  ! Lan.Control_WorkEdit.TextName   Header# �     CellTemplate �� .R��� $ 3q�$��Center=�$
��Bold"����� *Key+  $	 200  ! Lan.Control_WorkEdit.TextType# �   �� .R��� $ 3q�$��Center=����� *Value+ � ToolTip �� *Value+�� $��1��$��Center=�$��Center=�._��� $��5q�$	����#��[ $��16(�#��[  1 -Lan.Window_EditAdvancedDLMode.BtnTransferPara# �  ����  �� $ WindowL�$1 +DataContext.CmdTransferInputParaListToTools�����  �� $ ListViewItemL�$ DataContext��� $��2��$��2q�
�������$	��0.5q�$ 0��._��  �� $��30 ��� .+��� 
�������$��	0,0,0,0.5q�._��� $ 3q�$��Center=�$��Center=� . *Lan.Window_EditAdvancedDLMode.TextToolList#���  �� $��1��$	��200��$��2q�
������ $��0q����� *Tools+���� "	 ��.��� 
 � ��
 ������� 
 � ��$ 0q��� 
 � �� �� " ��.R��� $��2q�$ 
Horizontal=�.+��� $ 5������ "	 �� �� Triggers ��  �� Binding �� *ToolInfo.IsDLType+ �� Value True.a��� 
 � ��
 ��d���  �� *ToolInfo.IsDLType+	 False.a��� 
 � �� �#84bf96._��  �� $��30 ��� �� .+��� $ 5q�$��Center=�#��[ $��20(����� "	 �� ��  �� *ToolInfo.IsDLType+ True.a��� 
 � ��$	 ����  �� *ToolInfo.IsDLType+	 False.a��� 
 � ��$	 ���� $��1��._��� $ 2q�$��Center=�$
��Bold"����� *ToolInfo.ToolName+ �� *ToolInfo.ToolName+�� $��2��$ 
Horizontal=�.+�� .+��� $��3q�$ 
Horizontal=�y��� *ToolInfo.IsDLType+ $  BoolToVisiblityDefaultVisibleCvt �� 	Converter# [ .+��� $��30��$��1.5,-1q�$	����#��[ #��[ ����  �� $ WindowL�$' !DataContext.CmdEditAdvancedToolTB�����  �� $ ListViewItemL�$ DataContext��� $��ws��$��30��$	��100��$��Center=�$��0q� �� DisplayMemberPath$ WorkspaceName�� �� SelectedValuePath$ WorkspaceName������  �� $ WindowL�$ DataContext.DLWorkspaces� �� SelectedValue �� *ToolInfo.DLWorkspaceName+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=����� "	 �� ��  �� $ btnResetDLWorkspace��$ 	IsPressed� True.a��� 
 �@��SelectedIndex$ -1���� $��30��$	��100��$	��2,0q�$��Center=�$��0q����� $ ws��$ SelectedItem.Streams� �� *ToolInfo.DLStreamName+$ TwoWay=�$ PropertyChanged=����� "	 �� ��  �� $ btnResetDLWorkspace��$ 	IsPressed� True.a��� 
 �@��SelectedIndex$ -1���� $��30��$��Center=�$��0q�����  �� $ WindowL�$ DataContext.GPUList� �� SelectedIndex �� *ToolInfo.GPUIndex+$ TwoWay=�$ PropertyChanged=���   ��Name$  btnResetDLWorkspace��$��30��$	��5,0q�$	����#��[ $��16(�#��[ �� $��3q�$	����#��[ #��[ ����  �� $ WindowL�$' !DataContext.CmdEditAdvancedToolTB�����  �� $ ListViewItemL�$ DataContext�y��� *ToolInfo.IsDLType+ # BoolToVisiblityDefaultHiddenCvt# [ �� $��30��$��3q�$	����#��[ $��16(�#��[ ����  �� $ WindowL�$' !DataContext.CmdRemoveAdvancedTool���`�  ( $MultiBindingConverter_ObjectArrayCvt! `� 	Converter#! [ .1��� $ DataContext� �� $ ListViewItemL�" �� AncestorLevel$" 2���� $ DataContext� �� $ ListViewItemL��� $��3��$��2q�
�������$	��0.5q�._��  �� $��30 ��� .+��� 
�������$��	0,0,0,0.5q�._��� $ 3q�$��Center=�$��Center=� 4 0Lan.Window_EditAdvancedDLMode.TextOutputParaList#���    $��1��$	��200��$��0q�$��0q�#��[
 ���� *OutputParaList+   $ * # �   �� .R��� $ 3q�$��Center=�$
��Bold"����� *Key+  $	 200 # �   �� .R��� $ 3q�$��Center=����� *Value+ �� *Value+ P� # P� Property# �@��
IsSelected$ P� Value$ . . ��� 
 � ��  Color_Selected# [ 