   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  *    $VPDLFramework.Views.Control_WorkList   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  -local!clr-namespace:VPDLFramework.Views  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  -     >�Name$  workListControl�����#3D3D4C � DataContext ��    Locator �� Source# [   �� Path$
 Main� � 	Resources   cm  Color_DarkBackground  ContextMenuStyle%  System.Windows.Markup.Reference   Name  BoolInverseCvt �� 	Converter , (Lan.Control_WorkList.ContextMenu.BtnCopy
 � Tag m� Command	 m� CommandParameter . *Lan.Control_WorkList.ContextMenu.BtnExport . *Lan.Control_WorkList.ContextMenu.BtnDelete��%�  &	       7  7  7  �� 8��  
�������8�� {���    $ workListControl��$ DataContext.IsWorkLoaded�8  .@�m� #���  $	 �� ��    $ workListControl��$ DataContext.CmdCopyWork�	 ��    $ workList��$ SelectedItem�{���    $ workListControl��$ DataContext.IsLogin�m� #���  $	 �� ��    $ workListControl��$ DataContext.CmdExportWork�	 ��    $ workList��$ SelectedItem�{���    $ workListControl��$ DataContext.IsLogin�m� #���  $	 �� ��    $ workListControl��$ DataContext.CmdDeleteWork�	 ��    $ workList��$ SelectedItem�{���    $ workListControl��$ DataContext.IsLogin�.��� .+�� -   
 �Name$
 g��$	��0,2q�.+��� 
�������$��2q�� $	��0.5q� � RowDefinitions �� $��40 ��� �� $��40 �.+��� $��1��
�������$��	0,0.5,0,0q��� $	��4,0q�$
��Left=� �� Orientation$ 
Horizontal=�����  �� 
TargetType" ��.���  �� Property � {� �� Value `�  / +MultiBindingConverter_LoginAndWorkLoadedCvt `� 	Converter# [ .1��� $ IsLogin��� $ IsWorkLoaded�.+��� $��30��$��30��$��2q�$	���� 	 Font_Ico#��[	 $��16(� 
 Style_Button_Ico#��[
  + 'Lan.Control_WorkList.MenuBar.BtnRefresh � ToolTip# �  ���� *CmdRefreshWorkList+�� $��30��$��30��$��2q�$	����#��[	 $��16(�#��[
  ' #Lan.Control_WorkList.MenuBar.BtnAdd# �  ���� *CmdCreateNewWork+�� $��30��$��30��$��2q�$	����#��[	 $��16(�#��[
  * &Lan.Control_WorkList.MenuBar.BtnImport# �  ���� *CmdImportWork+����  �� ElementName$ workList��$ SelectedItem��� $	��4,0q�$��Right=�$ 
Horizontal=����� " ��.���  � {� `� # [ .1��� $ IsLogin��� $ IsWorkLoaded�.+��� $��30��$��30��$��2q�$	����#��[	 $��16(�#��[
  ( $Lan.Control_WorkList.MenuBar.BtnEdit# �  ���� *CmdSelectWork+���� $ workList��$ SelectedValue��� -    ��Name$ workList��$��1��$��5q�$��0q�$��Hidden=�$
��Auto=����� *
WorkList+���� " �� �� Triggers ��  �� Binding �� *IsWorkLoaded+ �� Value True.a���  � ��
 ���� ��  �� *IsWorkLoaded+	 False.a���  � ��
 ���� ���� " ��.���  � �� ��  �� RelativeSource ��  �� AncestorType$
 GridL�$ ActualWidth���  � ��$ Center=���  � ��$	 0,2q���  � ��$ 0q���  �@�ContextMenu# �  ��  � �� ��  �� 
TargetType" ��.R�� .+��� #��z��
�������#��z�� �� CornerRadius$ 0�� J� SnapsToDevicePixels . � $��1q�.+��� $ 
Horizontal=�.+��� $��2q� �� Padding$ 3q�$��Center=�#��[	 $��16(�$
��Bold"�$	�������� " �� ��  �� *IsEdit+ True.a���  � ��
 � �����  �� *IsEdit+	 False.a���  � ��
 ������  � ColumnDefinitions �� $��30 ��� .+��� $	��5,0q�$
��Left=�$��Center=�
����������� *WorkInfo.WorkID+��  ��Name$ tbName��$��1��$
��Left=�$��Center=�$
��Bold"�
����������� *WorkInfo.WorkName+ �� *WorkInfo.WorkName+�� $��Right=�$ 
Horizontal=�.+��� $��20��$��20��$	��2,0q�$	����#��[	 $��16(�#��[
    Lan.Control_WorkList.BtnSave# �  ���� *CmdSaveWork+���� $ workList��$ SelectedItem�y��� *IsEdit+ $  BoolToVisiblityDefaultVisibleCvt# [ �� $��20��$��20��$	��2,0q�$	����#��[	 $��16(�#��[
  ! Lan.Control_WorkList.BtnClose# �  ���� *CmdCloseWork+����  �� $ ListBoxItemL�$ DataContext�y��� *IsEdit+# [  P�  P� Property �@��
IsSelected  P� Value  . . ���  � ��  Color_Selected# [ P�  �@J�IsMouseOver  . . ���  � ��$ 1q�.  (System.Windows.Interactivity.Interaction!   Triggers! /  )System.Windows.Interactivity.EventTrigger  "   	EventName$" MouseDoubleClick��.  (System.Windows.Interactivity.TriggerBase#   Actions.# 6  0System.Windows.Interactivity.InvokeCommandAction  $   Command$ ��  �� $ local:MainWindowL�$ DataContext.CmdSelectWork�%   CommandParameter% ��  �� $ ListBoxL�$ SelectedItem�� $��2�� �� �� .+��� $��2q�#��[
 ���� *CmdLoadWork+���� $ workList��$ SelectedValue�{��� *IsWorkLoaded+# [ .���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[	 $��12(�$	������ $��1��$��Center=�$��Center=� , (Lan.Control_WorkList.MenuBar.BtnLoadWork#���  �� $��1��$��2q�#��[
 ���� *CmdUnLoadWork+{��� *IsWorkLoaded+.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[	 $��12(�$	������ $��1��$��Center=�$��Center=� . *Lan.Control_WorkList.MenuBar.BtnUnloadWork#���  