   M S B A M L   `   `   `  ����H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  5      -    'VPDLFramework.Views.Control_WorkRuntime   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  5      -local!clr-namespace:VPDLFramework.Views  5      Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  5      X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f45xceed)http://schemas.xceed.com/wpf/xaml/toolkit  5	         Color_DarkBackground#���   5        � DataContext  5      ��   Locator �� Source# [  �� Path$ WorkRuntime�.��5      �  � RowDefinitions 5   
   �� 5      �� 5      $��50 �6   .+�5   
   �� -   5         ��Name$	 tab��5      
������ $��0q�5       �� TabStripPlacement$ Bottom=�5      ��5      �� 5       �� 
TargetType" ��6    �� Triggers 5      �� 5       �� Binding 6&   �� 	 �� ElementName$	 	btn_Image��$ 	IsChecked�
 �� Value
 True6`   .a�5      ��  �� Property �@��SelectedIndex �� Value$ 0��6>   5      �� 5       6&   �� $	 btn_Data��$ 	IsChecked�
 True6_   .a�5       ��  �@��SelectedIndex$ 1��6>   5!      �� 5"       6&   �� $	 btn_Log��$ 	IsChecked�
 True6^   .a�5#      ��  �@��SelectedIndex$ 2��6>   5$      �� 5%       6&   �� $	 btn_Recipes��$ 	IsChecked�
 True6b   .a�5&      ��  �@��SelectedIndex$ 3��6>   5'      �� 5(       6&   �� $	 btn_ToolBlocks��$ 	IsChecked�
 True6e   .a�5)      ��  �@��SelectedIndex$ 4��6>   5*      �� 5+       6&   �� $	 btn_Cameras��$ 	IsChecked�
 True6b   .a�5,      ��  �@��SelectedIndex$ 5��6>   5-      �� 5.       6&   �� $	 btn_Parameters��$ 	IsChecked�
 True6e   .a�5/      ��  �@��SelectedIndex$ 6��6>   50      52       � 	Resources 54      ��%Y   '��       �� 55      " ��6   .�56      ��  � y�$ 	Collapsed=�63   57      58      .@�59      �� .��5:      �  � ColumnDefinitions 5;      �� 5<      �� 5=      ��5>      �� 5?   "   " ��6(    5@   &   �� 5A   *    66   �� $	 tgBtnShowChart��$ 	IsChecked�	
 False6u   .a�5B   .   ��  � ��$ 0 �6F   5C   +   5E   #   5G      .+�5I      �� 
�������$��2q�60   ._�5J      � .+�5K      �� $��0,0,0,23q�5L   !   #��[  5M   !   $��0q�5N   !   $��Disabled=�5O   !   $��Disabled=�5P   !   ��5Q   "   �� 5R   &   " ��6,    5S   *   �� 5T   .    6:   �� *	IsZooming+
 True6X   .a�5U   2   ��  � �� 6P   �� *ZoomResults+5V   /   �� 5W   .    6:   �� *	IsZooming+	
 False6X   .a�5X   2   ��  � �� 6P   �� *	Results+5Y   /   5[   '   .  (System.Windows.Interactivity.Interaction   Triggers 5]   "   /  )System.Windows.Interactivity.EventTrigger  5^   &      	EventName$ MouseDoubleClick��65   .  (System.Windows.Interactivity.TriggerBase   Actions. 5_   *   6  0System.Windows.Interactivity.InvokeCommandAction     Command 6@   ��  �� RelativeSource ��  �� AncestorType$ local:Control_WorkRuntimeL�$# DataContext.CmdZoomResultItem�   CommandParameter 6�   ��  �� $ ListViewL�$ SelectedItem�5`   '   ��5b   "   �� 5c   &   .R�5d   *   A�  A� Columns 66   ��  �� $ local:Control_WorkRuntimeL�$+ %DataContext.LayoutInfo.DisplayColumns� �� Mode$ OneWay=� �� UpdateSourceTrigger$ PropertyChanged=� A� Rows 6�   ��  �� $ local:Control_WorkRuntimeL�$( "DataContext.LayoutInfo.DisplayRows�$ OneWay=�$ PropertyChanged=�5e   '   ��5g   "   �� 5h   &   " ��6,   .�5i   *   ��  � ��$ Stretch=�6N   �� 5j   *    � �� 5k   .   �� 5l   2   .R�5m   6   5   /VPDLFramework.Views.Control_RuntimeResultViewer    5n   9   ��    DisplayRecord 5o   9   �� *ResultRecord+   
RangeImage 5p   9   �� *
RangeImage+5q   3   5s   +   5t   '   5v      �� 5w      $��2q�5x   !   $
��Left=�5y   !   $��Bottom=�5z   !    �� Orientation$ 
Horizontal=�5|   !   {�5{   !   �� *	IsZooming+  BoolInverseCvt �� 	Converter# [ .+�5}   "   �� -   5         ��Name$ tgBtnShowChart��5~   %   $
��10,0q�5   %   $��Center=�5�   %    ) %Lan.Control_WorkRuntime.TextShowChart#���  5�   %   
������� d� 	IsChecked 5�   %   �� *LayoutInfo.IsChartVisible+$ TwoWay=��� 5�   "   $	��2,0q�5�   %   $��Center=�5�   %   
������� # Lan.Control_WorkRuntime.TextRow#���  5�   %   %  Xceed.Wpf.Toolkit.IntegerUpDown  5�   "   �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]    Maximum$  10X�5�   %   !   Minimum$! 1X�5�   %   "   Value" 5�   %   �� *LayoutInfo.DisplayRows+$ TwoWay=�$ PropertyChanged=� 5�   &     5�   *   $ ValueChanged��69   . 5�   .      6D   �� *CmdChangeRows+ 6f   ��  �� $ xceed:IntegerUpDownL�$ Value�5�   +   5�   #   �� 5�   "   $	��2,0q�5�   %   $��Center=�5�   %   
������� # Lan.Control_WorkRuntime.TextCol#���  5�   %     5�   "   $  10X�5�   %   $! 1X�5�   %   " 5�   %   �� *LayoutInfo.DisplayColumns+$ TwoWay=�$ PropertyChanged=� 5�   &     5�   *   $ ValueChanged��69   . 5�   .      6D   �� *CmdChangeColumns+ 6i   ��  �� $ xceed:IntegerUpDownL�$ Value�5�   +   5�   #   �� 5�   "   $��20��5�   %   $	��5,0q�5�   %   $	����5�   %     Font_Ico#��[ 5�   %   $��16(�5�   %    ) %Lan.Control_WorkRuntime.BtnSaveConfig# � ToolTip## �  5�   %   ��5�   %   �� *CmdSaveLayoutConfig+5�   %   5�      5�      5�      �� 5�      $��1��5�      $��2,0,0,0q�5�      #���   5�      
�������$��2q�5�      $��Disabled=�5�      $��Disabled=�5�      ��5�      �� *ResultsChart+��5�      �� 5�      .R�5�   "   A� 5�      ��5�      �� 5�      " ��6$   .�5�   "   ��  � �� 5�   &   �� 5�   *   .R�5�   .   4   .VPDLFramework.Views.Control_RuntimeResultChart  5�   +   5�   #   5�      5�      5�      5�      �� 5�      .��5�      0	   *VPDLFramework.Views.Control_ProductionData	    6/   �� *ProductDataViewModel+5�      �� 5�        6   �� *WorkLogViewModel+.��5�      )
   #VPDLFramework.Views.Control_WorkLog
  5�      �� 5�      .��5�      �� 
������ $��2q�5�      $ �� SelectedIndex$$ 0��5�      ��5�      �� *WorkStreamsRecipe+ 5�      ��%W   '��       �� 5�      " ��6    .�5�      ��  � y�$ Visible=�6;   5�      5�      ��5�      �� 5�      .R�5�      �� $
��Bold"�6(   ��6:   �� *&$WorkStream.WorkStreamInfo.StreamName+5�      % �� ContentTemplate% 5�      �� 5�      .R�5�      3   -VPDLFramework.Views.Control_WorkStreamRecipes  5�      5�      5�      �� 5�      .��5�      ��  5�      ��%W   '��       �� 5�      " ��6    .�5�      ��  � y�$ Visible=�6;   5�      5�      .@�5�      ��  ) %Lan.Control_WorkEdit.HeaderWorkStream#���  6   .��5�      �� 
������ $��2q�5�      $$ 0��5�      ��5�      �� *WorkStreams+ 5�      ��%W   '��       �� 5�   "   " ��6(   .�5�   &   ��  � y�$ Visible=�6C   5�   #   5�      ��5�      �� 5�   "   .R�5�   &   �� $
��Bold"�60   ��6B   �� *WorkStreamInfo.StreamName+5�   #   % 5�      �� 5�   "   .R�5�   &   9   3VPDLFramework.Views.Control_RuntimeWorkStreamTBEdit  &   	ToolBlock& 6L   �� *
DLOutputTB+5   #   5     5     �� 5      $	  Lan.Control_WorkEdit.HeaderGroup#��� 	 6   .��5     �� 
������ $��2q�5     $$ 0��5	     ��5     �� *
WorkGroups+ 5
     ��%W   '��       �� 5  "   " ��6(   .�5  &   ��  � y�$ Visible=�6C   5  #   5     ��5     �� 5  "   .R�5  &   �� $
��Bold"�60   ��6B   �� *GroupInfo.GroupName+5  #   % 5     �� 5  "   .R�5  &   8   2VPDLFramework.Views.Control_RuntimeWorkGroupTBEdit  '   GroupToolBlock' 6K   �� *	ToolBlock+5  #   5     5     5     5     �� 5     .��5     �� 
������ $��2q�5      $$ 0��5"     ��5!     �� *WorkImageSources+ 5#     ��%W   '��       �� 5$     " ��6    .�5%     ��  � y�$ Visible=�6;   5&     5'     ��5(     �� 5)     .R�5*     �� $
��Bold"�6(   ��6:   �� *!ImageSourceInfo.ImageSourceName+5+     % 5-     �� 5.     .R�5/     :   4VPDLFramework.Views.Control_RuntimeImageSourceTBEdit  (   	ToolBlock( 6E   �� *	ToolBlock+50     52     53     �� 54     .��55     �� 
������ $��2q�57     $$ 0��59     ��58     �� *WorkStreams+ 5:     ��%W   '��       �� 5;     " ��6    .�5<     ��  � y�$ Visible=�6;   5=     5>     ��5?     �� 5@     .R�5A     �� $
��Bold"�6(   ��6:   �� *WorkStreamInfo.StreamName+5B     % 5D     �� 5E     .R�5F     3   -VPDLFramework.Views.Control_RuntimeParameters  5G     5I     5J     5K     �� 5L  
   $��1��6   $ 
Horizontal=�6"   .+�5M     	� -   5        ) 	�Name$) 	btn_Image��5N     $��5q�5O     * 	� 	GroupName$* 
tabControl��5P     ��5Q     �� 5R       
 Style_RadioButton_TabControl+ �� BasedOn#+ [
 6   " 	�6T    5S     �� 5T      6*   �� $		 tab��$ SelectedIndex�
 06b   .a�5U  "   ��  �@d�	IsChecked$
 TrueY�6>   5V     5X     .��5Z     �  5[     �� 5\     �� 5]     .+�5_     �� $��2q�5`     $
��Left=�5a     $��Center=�5b     #��[ 5c     $��16(�5d     $	����5e     �� 5f     $��1��5g     $��Center=�5h     $��Center=�5i      + 'Lan.Control_WorkRuntime.BtnRuntimeImage#���  5j     5k     5l     	� 5m     -   5        $) btn_Data��5n     $��5q�5o     $* 
tabControl��5q     ��5p     �� *CmdShowProductData+��5r     �� 5s     #+ [
 6   " 	�6T    5t     �� 5u      6*   �� $		 tab��$ SelectedIndex�
 16b   .a�5v  "   ��  �@d�	IsChecked$
 TrueY�6>   5w     5y     .��5{     �  5|     �� 5}     �� 5~     .+�5�     �� $��2q�5�     $
��Left=�5�     $��Center=�5�     #��[ 5�     $��16(�5�     $	����5�     �� 5�     $��1��5�     $��Center=�5�     $��Center=�5�      * &Lan.Control_WorkRuntime.BtnRuntimeData#���  5�     5�     5�     	� 5�     -   5        $) btn_Log��5�     $��5q�5�     $* 
tabControl��5�     ��5�     �� *CmdShowWorkLog+��5�     �� 5�     #+ [
 6   " 	�6T    5�     �� 5�      6*   �� $		 tab��$ SelectedIndex�
 26b   .a�5�  "   ��  �@d�	IsChecked$
 TrueY�6>   5�     5�     .��5�     �  5�     �� 5�     �� 5�     .+�5�     �� $��2q�5�     $
��Left=�5�     $��Center=�5�     #��[ 5�     $��16(�5�     $	����5�     �� 5�     $��1��5�     $��Center=�5�     $��Center=�5�      ) %Lan.Control_WorkRuntime.BtnRuntimeLog#���  5�     5�     5�     	� 5�     -   5        $) btn_Recipes��5�     $��5q�5�     $* 
tabControl��5�     {�5�     �� *IsSystemOnline+# [ ��5�     �� 5�     #+ [
 6   " 	�6T    5�     �� 5�      6*   �� $		 tab��$ SelectedIndex�
 36b   .a�5�  "   ��  �@d�	IsChecked$
 TrueY�6>   5�     P� 5�     , P� Property, � {�- P� Value- .  . �5�  "   ��  � ��
 �����6?   5�     5�     .��5�     �  5�     �� 5�     �� 5�     .+�5�     �� $��2q�5�     $
��Left=�5�     $��Center=�5�     #��[ 5�     $��16(�5�     $	����5�     �� 5�     $��1��5�     $��Center=�5�     $��Center=�5�      , (Lan.Control_WorkRuntime.BtnRecipesManage#���  5�     5�     5�     	� 5�     -   5        $) btn_ToolBlocks��5�     $��5q�5�     $* 
tabControl��5�     {�5�     �� *IsSystemOnline+# [ y�5�     ��  �� $ local:MainWindowL�$ DataContext.IsLogin� $  BoolToVisiblityDefaultVisibleCvt# [ ��5�     �� 5�     #+ [
 6   " 	�6T    5�     �� 5�      6*   �� $		 tab��$ SelectedIndex�
 46b   .a�5�  "   ��  �@d�	IsChecked$
 TrueY�6>   5�     P� 5�     , � {�- .  . �5�  "   ��  � ��
 �����6?   5�     5�     .��5�     �  5�     �� 5�     �� 5�     .+�5�     �� $��2q�5�     $
��Left=�5�     $��Center=�5�     #��[ 5�     $��16(�5�     $	����5�     �� 5�     $��1��5�     $��Center=�5�     $��Center=�5�      . *Lan.Control_WorkRuntime.BtnAlgorithmManage#���  5�     5�     5�     	� 5�     -   5        $) btn_Cameras��5�     $��5q�5�     $* 
tabControl��5�     {�5�     �� *IsSystemOnline+# [ y�5�     ��  �� $ local:MainWindowL�$ DataContext.IsLogin�# [ ��5�     �� 5�     #+ [
 6   " 	�6T    5      �� 5      6*   �� $		 tab��$ SelectedIndex�
 56b   .a�5  "   ��  �@d�	IsChecked$
 TrueY�6>   5     P� 5     , � {�- .  . �5  "   ��  � ��
 �����6?   5     5     .��5
     �  5     �� 5     �� 5     .+�5     �� $��2q�5     $
��Left=�5     $��Center=�5     #��[ 5     $��16(�5     $	����5     �� 5     $��1��5     $��Center=�5     $��Center=�5      + 'Lan.Control_WorkRuntime.BtnCameraManage#���  5     5     5     	� 5     -	   5        $) btn_Parameters��5     $��5q�5     $* 
tabControl��5      {�5!     �� *IsSystemOnline+# [ y�5"     ��  �� $ local:MainWindowL�$ DataContext.IsLogin�# [ ��5#     �� 5$     #+ [
 6   " 	�6T    5%     �� 5&      6*   �� $		 tab��$ SelectedIndex�
 66b   .a�5'  "   ��  �@d�	IsChecked$
 TrueY�6>   5(     P� 5)     , � {�- .  . �5*  "   ��  � ��
 �����6?   5+     5-     .��5/     �  50     �� 51     �� 52     .+�54     �� $��2q�55     $
��Left=�56     $��Center=�57     #��[ 58     $��16(�59     $	����5:     �� 5;     $��1��5<     $��Center=�5=     $��Center=�5>      ) %Lan.Control_WorkRuntime.BtnParaManage#���  5?     5@     5A     5B     5C     5D     