   M S B A M L   `   `   `  ����H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  5      0    *VPDLFramework.Views.Control_ProductionData   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      3local!clr-namespace:VPDLFramework.Views     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5         Color_DarkBackground#���   5
      .��5      � .+�5   
   �� -   5          ��Name$	  tb2��5      
������ 
�������$��2q�5       �� SelectedIndex$ 0��5      ��5      �� *ProductionData+ � 	Resources 5       �� 
TargetType �� Property �� Value��%W   '��       �� 5      " ��6   .�5      ��  � y�$ Visible=�63   5      5      ��5      �� 5      .R�5      �� $
��Bold"�6    ��62   �� *StreamOrGroupName+5       �� ContentTemplate 5      �� 5      .R�5       �� $��2q�5!      
�������$��0q�5#      ._�5$      �  � RowDefinitions 5%      �� 5&   "   $��30 �60   �� 5'   "   .+�5)      �� ._�5*   "   �� $��2q�5+   %    �� Padding$ 2q�5,   %   $��Center=�5-   %   $��Center=�5.   %   $��12(�5/   %   $
��Bold"�50   %   ��51   %   �� *StreamOrGroupName+52      � 53      $��1��6#    54   "   �� 55   &   $��50 �64   �� 56   &   .+�58   "   � .+�59   &   � 	 � ColumnDefinitions	 5:   *   �� 5;   .   �� 5<   .   .+�5>   *   �� 
 �� Orientation$
 Vertical=�65   .+�5?   .   �� $
 
Horizontal=�69   .+�5@   2   �� $ 3q�5A   5   $��Center=�5B   5    , (Lan.Control_ProductionData.TextStartDate#���  5C   5   �� 5D   2   $ 3q�5E   5   $
��Left=�5F   5   $��Center=�5G   5   ��5H   5   �� *	StartDate+ �� StringFormat$ {0:yyyy-MM-dd}���� 5I   2   $ 3q�5J   5   $��Center=�5K   5    * &Lan.Control_ProductionData.TextEndDate#���  5L   5   �� 5M   2   $��Right=�5N   5   $��Center=�5O   5   ��5P   5   �� *	EndDate+$ {0:yyyy-MM-dd}��5Q   /   �� 5R   .   $��3q�69   $
 
Horizontal=�6D   .+�5S   2   (  "System.Windows.Controls.DatePicker  $
��Left=�5T   5   $��Center=�5U   5      SelectedDate 5V   5   �� *SelectedDate+ �� Mode$ TwoWay=��� 5W   2   $	��4,0q�5X   5   $��2q�5Y   5   $��Center=�5Z   5    ' #Lan.Control_ProductionData.BtnQuery#���  5\   5     Style_Button_Ico#��[ 5]   5   ��5[   5   �� *CmdQueryByDate+5]   5   �� 5e   2   $	��2,0q�5f   5   $��2q�5g   5   $��Center=�5h   5    , (Lan.Control_ProductionData.BtnQueryToday#���  5j   5   #��[ 5k   5   ��5i   5   �� *CmdQueryAll+5k   5   �� 5l   2   $	��2,0q�5m   5   $��2q�5n   5   $��Center=�5o   5    + 'Lan.Control_ProductionData.BtnExportCSV#���  5q   5   #��[ 5r   5   ��5p   5   �� *CmdExportDatabaseToCSV+5r   5   5s   /   5t   +   �� 5u   *   $��1��5v   -   $��Right=�5w   -   $��Center=�5x   -   $
 
Horizontal=�5y   -   .+�5z   .   �� $ 3q�5{   1   $��Center=�5|   1   $
��Bold"�5}   1    ' #Lan.Control_ProductionData.TextRate#���  5~   1   �� 5   .   $
 
Horizontal=�69   .+�5�   2   �� $
��10,0q�5�   5   $��Center=�5�   5   $	��OK/��5�   5   �� 5�   2   $
��10,0q�5�   5   $��Center=�5�   5    ( $Lan.Control_ProductionData.TextTotal#���  5�   5   5�   /   �� 5�   .   $
 
Horizontal=�69   .+�5�   2   �� $	��3,0q�5�   5   $��Center=�5�   5   ��5�   5   �� *	GoodNum+�� 5�   2   $	��3,0q�5�   5   $��Center=�5�   5   $��/��5�   5   �� 5�   2   $	��3,0q�5�   5   $��Center=�5�   5   ��5�   5   �� *
TotalNum+5�   /   �� 5�   .   $
��10,0q�5�   1   $��Center=�5�   1   $
��Bold"�5�   1   ��5�   1   �� *	OKRateStr+5�   +   5�   '   5�   #   &   System.Windows.Controls.DataGrid  5�   "   $��1��5�   %   $	��0.5q�5�   %   $	��0.5q�5�   %   $
��Auto=�5�   %    	 Style_DataGrid#��[	 5�   %   ��5�   %   �� *Data+   Columns 5�   &   4  .System.Windows.Controls.DataGridTemplateColumn  5�   *   ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 160 6A    .
 *Lan.Control_ProductionData.TextTriggerTime   Header# � 
 6M      CellTemplate 5�   .   �� 5�   2   .R�5�   6   �� $ 2q�5�   9   $��Center=�5�   9   ��5�   9   �� *Time+ � ToolTip 5�   9   ��  �� RelativeSource ��  �� Mode$
 Self=� �� Path$
 Text�5�   3   5�   +     5�   *   $ 80 6A    / +Lan.Control_ProductionData.TextTriggerIndex# �  6L    5�   .   �� 5�   2   .R�5�   6   �� $��Center=�5�   9   ��5�   9   �� *TriggerIndex+ 5�   9   ��  �� $
 Self=�$
 Text�5�   3   5�   +     5�   *   $ * 6A    0 ,Lan.Control_ProductionData.TextResultDisplay# �  6K    5�   .   �� 5�   2   .R�5�   6   �� $ 2q�5�   9   $��Center=�5�   9   ��5�   9   �� *ResultForDisplay+ 5�   9   ��  �� $
 Self=�$
 Text�5�   3   5�   +     5�   *   $ * 6A    - )Lan.Control_ProductionData.TextResultSend# �  6K    5�   .   �� 5�   2   .R�5�   6   �� $ 2q�5�   9   $��Center=�5�   9   ��5�   9   �� *ResultForSend+ 5�   9   ��  �� $
 Self=�$
 Text�5�   3   5�   +     5�   *   $ 80 6A    / +Lan.Control_ProductionData.TextResultStatus# �  6L    5�   .   �� 5�   2   .R�5�   6   �� $��2q�5�   9    J� Opacity$	 0.5Q�5�   9   ��5�   9   �� *ResultStatus+  NumToColorGreenRed �� 	Converter# [ 5�   9   5�   3   5�   +   5�   #   5�      5�      5�      5�      5�      5�      5�      