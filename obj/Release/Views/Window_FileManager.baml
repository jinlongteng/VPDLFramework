   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  ,    &VPDLFramework.Views.Window_FileManager   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -    +  'Lan.Window_FileManager.TitleFileManager  � Title#  �   $	��800��$	��600��  Color_DarkBackground#���   � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ 
ToolWindow=� � DataContext ��   Locator �� Source# [  �� Path$ FileManager�.��� .+�� .+��� 
�������$��1q�� $��1q�.+��� $��Disabled=�$
��Auto=�.���� $��Center=�$��Center=�#���  $��0q�$��Disabled=�$
��Auto=����� *WorksFileList+����  �� 
TargetType" ��.���  �� Property � ��	 �� Value$	 Center=���  � ��$	 Center=���  � ��	 �� 
 �� 
TargetType"
 ��.R�� #��z��.+�9�  9�Name$
 work��  Style_Expander#��[ ���� *
WorkName+.���  � RowDefinitions �� $��30 ��� .+��  � ColumnDefinitions �� �� .+���  5 1Lan.Window_FileManager.BtnWorkLogBatchExportToCSV#���  ����  �� RelativeSource ��  �� AncestorType$ local:Window_FileManagerL�$, &DataContext.CmdWorkLogBatchExportToCSV����� *
WorkName+�� $��1�� 9 5Lan.Window_FileManager.BtnProductDataBatchExportToCSV#���  ����  �� $ local:Window_FileManagerL�$0 *DataContext.CmdProductDataBatchExportToCSV����� *
WorkName+&   System.Windows.Controls.DataGrid    Name$ dg��$��1��$	��0.5q�#���  
�������$	��0.5q�  Style_DataGrid#��[ ���� *StreamsFileInfo+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  
stream��,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$ *  - )Lan.Window_FileManager.TextWorkStreamName   Header# �     CellTemplate �� .R��� $��3q� �� Padding$ 3q�$��Center=����� *
StreamName+  $ *  - )Lan.Window_FileManager.TextProductionData# �   �� .R��� $ 3q�$��Center=����� *DatabaseSize+  $ *  %	 !Lan.Control_WorkEdit.TextOperator# � 	  �� .R���  �� Orientation$ 
Horizontal=�.+��� $��30��$��30��$	���� 
 Font_Ico#��[
 $��20(� $  Lan.Window_FileManager.BtnExport � ToolTip# �  ���� *CmdExportDatabase+ ��  �� $ local:Window_FileManagerL�$ DataContext���`�   ConcatStringConverterCvt `� 	Converter# [ .1���  �� ElementName$
 work��$ Header��� $ SelectedIndex� �� $ DataGridL��� $��30��$��30��$	����#��[
 $��20(� # Lan.Window_FileManager.BtnClear# �  ���� *CmdClearDatabase+ ��  �� $ local:Window_FileManagerL�$ DataContext���`� # [ .1��� $
 work��$ Header��� $ SelectedIndex� �� $ DataGridL�  $ *  $  Lan.Window_FileManager.TextImage# �   �� .R��� $ 3q�$��Center=����� *SpaceOccupiedByImages+  $ * # � 	  �� .R��� $ 
Horizontal=�.+��� $��30��$��30��$	����#��[
 $��20(�# �  ���� *CmdExportImages+ ��  �� $ local:Window_FileManagerL�$ DataContext���`� # [ .1��� $
 work��$ Header��� $ SelectedIndex� �� $ DataGridL��� $��30��$��30��$	����#��[
 $��20(�# �  ���� *CmdClearImages+ ��  �� $ local:Window_FileManagerL�$ DataContext���`� # [ .1��� $
 work��$ Header��� $ SelectedIndex� �� $ DataGridL�