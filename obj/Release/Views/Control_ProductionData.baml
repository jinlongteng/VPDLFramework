   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  0    *VPDLFramework.Views.Control_ProductionData   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006        Color_DarkBackground#���   .��� .+��� -     ��Name$	  tb2��
������ 
�������$��2q� �� SelectedIndex$ 0������ *ProductionData+ � 	Resources  �� 
TargetType �� Property �� Value��%2   '��       �� " ��.���  � y�$ Visible=����� .R��� $
��Bold"����� *StreamOrGroupName+ �� ContentTemplate �� .R��� $��2q�
�������$��0q�._��  � RowDefinitions �� $��30 ��� .+��� ._��� $��2q� �� Padding$ 2q�$��Center=�$��Center=�$��12(�$
��Bold"����� *StreamOrGroupName+� $��1�� �� $��50 ��� .+�� .+�� 	 � ColumnDefinitions	 �� �� .+��� 
 �� Orientation$
 Vertical=�.+��� $
 
Horizontal=�.+��� $ 3q�$��Center=� , (Lan.Control_ProductionData.TextStartDate#���  �� $ 3q�$
��Left=�$��Center=����� *	StartDate+ �� StringFormat$ {0:yyyy-MM-dd}���� $ 3q�$��Center=� * &Lan.Control_ProductionData.TextEndDate#���  �� $��Right=�$��Center=����� *	EndDate+$ {0:yyyy-MM-dd}���� $��3q�$
 
Horizontal=�.+�(  "System.Windows.Controls.DatePicker  $
��Left=�$��Center=�   SelectedDate �� *SelectedDate+ �� Mode$ TwoWay=��� $	��4,0q�$��2q�$��Center=� ' #Lan.Control_ProductionData.BtnQuery#���    Style_Button_Ico#��[ ���� *CmdQueryByDate+�� $	��2,0q�$��2q�$��Center=� , (Lan.Control_ProductionData.BtnQueryToday#���  #��[ ���� *CmdQueryAll+�� $	��2,0q�$��2q�$��Center=� + 'Lan.Control_ProductionData.BtnExportCSV#���  #��[ ���� *CmdExportDatabaseToCSV+�� $��1��$��Right=�$��Center=�$
 
Horizontal=�.+��� $ 3q�$��Center=�$
��Bold"� ' #Lan.Control_ProductionData.TextRate#���  �� $
 
Horizontal=�.+��� $
��10,0q�$��Center=�$	��OK/���� $
��10,0q�$��Center=� ( $Lan.Control_ProductionData.TextTotal#���  �� $
 
Horizontal=�.+��� $	��3,0q�$��Center=����� *	GoodNum+�� $	��3,0q�$��Center=�$��/���� $	��3,0q�$��Center=����� *
TotalNum+�� $
��10,0q�$��Center=�$
��Bold"����� *	OKRateStr+&   System.Windows.Controls.DataGrid  $��1��$	��0.5q�$	��0.5q�$
��Auto=� 	 Style_DataGrid#��[	 ���� *Data+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 160  .
 *Lan.Control_ProductionData.TextTriggerTime   Header# � 
    CellTemplate �� .R��� $ 2q�$��Center=����� *Time+ � ToolTip ��  �� RelativeSource ��  �� Mode$
 Self=� �� Path$
 Text�  $ 80  / +Lan.Control_ProductionData.TextTriggerIndex# �   �� .R��� $��Center=����� *TriggerIndex+ ��  �� $
 Self=�$
 Text�  $ *  0 ,Lan.Control_ProductionData.TextResultDisplay# �   �� .R��� $ 2q�$��Center=����� *ResultForDisplay+ ��  �� $
 Self=�$
 Text�  $ *  - )Lan.Control_ProductionData.TextResultSend# �   �� .R��� $ 2q�$��Center=����� *ResultForSend+ ��  �� $
 Self=�$
 Text�  $ 80  / +Lan.Control_ProductionData.TextResultStatus# �   �� .R��� $��2q� J� Opacity$	 0.5Q����� *ResultStatus+  NumToColorGreenRed �� 	Converter# [ 