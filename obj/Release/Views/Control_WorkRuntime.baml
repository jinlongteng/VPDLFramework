   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  -    'VPDLFramework.Views.Control_WorkRuntime   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  -local!clr-namespace:VPDLFramework.Views  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f45xceed)http://schemas.xceed.com/wpf/xaml/toolkit     Color_DarkBackground#���     � DataContext  ��   Locator �� Source# [  �� Path$ WorkRuntime�.���  � RowDefinitions �� �� $��50 �.+��� -    ��Name$	 tab��
������ $��0q� �� TabStripPlacement$ Bottom=�����  �� 
TargetType" �� �� Triggers ��  �� Binding �� 	 �� ElementName$	 	btn_Image��$ 	IsChecked�
 �� Value
 True.a���  �� Property �@��SelectedIndex �� Value$ 0����  �� $	 btn_Data��$ 	IsChecked�
 True.a���  �@��SelectedIndex$ 1����  �� $	 btn_Log��$ 	IsChecked�
 True.a���  �@��SelectedIndex$ 2����  �� $	 btn_Recipes��$ 	IsChecked�
 True.a���  �@��SelectedIndex$ 3����  �� $	 btn_ToolBlocks��$ 	IsChecked�
 True.a���  �@��SelectedIndex$ 4����  �� $	 btn_Cameras��$ 	IsChecked�
 True.a���  �@��SelectedIndex$ 5����  �� $	 btn_Parameters��$ 	IsChecked�
 True.a���  �@��SelectedIndex$ 6�� � 	Resources ��%4   '��       �� " ��.���  � y�$ 	Collapsed=�.@��� .���  � ColumnDefinitions �� �� ���� " �� ��  �� $	 tgBtnShowChart��$ 	IsChecked�	
 False.a���  � ��$ 0 �.+��� 
�������$��2q�._�� .+��� $��0,0,0,23q�#��[  $��0q�$��Disabled=�$��Disabled=����� " �� ��  �� *	IsZooming+
 True.a���  � �� �� *ZoomResults+��  �� *	IsZooming+	
 False.a���  � �� �� *	Results+.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ MouseDoubleClick��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command ��  �� RelativeSource ��  �� AncestorType$ local:Control_WorkRuntimeL�$# DataContext.CmdZoomResultItem�   CommandParameter ��  �� $ ListViewL�$ SelectedItem����� .R�A�  A� Columns ��  �� $ local:Control_WorkRuntimeL�$+ %DataContext.LayoutInfo.DisplayColumns� �� Mode$ OneWay=� �� UpdateSourceTrigger$ PropertyChanged=� A� Rows ��  �� $ local:Control_WorkRuntimeL�$( "DataContext.LayoutInfo.DisplayRows�$ OneWay=�$ PropertyChanged=����� " ��.���  � ��$ Stretch=���  � �� �� .R�5   /VPDLFramework.Views.Control_RuntimeResultViewer    ��    DisplayRecord �� *ResultRecord+   
RangeImage �� *
RangeImage+�� $��2q�$
��Left=�$��Bottom=� �� Orientation$ 
Horizontal=�{��� *	IsZooming+  BoolInverseCvt �� 	Converter# [ .+��� -    ��Name$ tgBtnShowChart��$
��10,0q�$��Center=� ) %Lan.Control_WorkRuntime.TextShowChart#���  
������� d� 	IsChecked �� *LayoutInfo.IsChartVisible+$ TwoWay=��� $	��2,0q�$��Center=�
������� # Lan.Control_WorkRuntime.TextRow#���  %  Xceed.Wpf.Toolkit.IntegerUpDown  �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]    Maximum$  10X�!   Minimum$! 1X�"   Value" �� *LayoutInfo.DisplayRows+$ TwoWay=�$ PropertyChanged=�   $ ValueChanged��.    �� *CmdChangeRows+ ��  �� $ xceed:IntegerUpDownL�$ Value��� $	��2,0q�$��Center=�
������� # Lan.Control_WorkRuntime.TextCol#���    $  10X�$! 1X�" �� *LayoutInfo.DisplayColumns+$ TwoWay=�$ PropertyChanged=�   $ ValueChanged��.    �� *CmdChangeColumns+ ��  �� $ xceed:IntegerUpDownL�$ Value��� $��20��$	��5,0q�$	����  Font_Ico#��[ $��16(� ) %Lan.Control_WorkRuntime.BtnSaveConfig# � ToolTip## �  ���� *CmdSaveLayoutConfig+�� $��1��$��2,0,0,0q�#���   
�������$��2q�$��Disabled=�$��Disabled=����� *ResultsChart+���� .R�A� ���� " ��.���  � �� �� .R�4   .VPDLFramework.Views.Control_RuntimeResultChart  �� .��0	   *VPDLFramework.Views.Control_ProductionData	    �� *ProductDataViewModel+��   �� *WorkLogViewModel+.��)
   #VPDLFramework.Views.Control_WorkLog
  �� .���� 
������ $��2q�$ �� SelectedIndex$$ 0������ *WorkStreamsRecipe+ ��%2   '��       �� " ��.���  � y�$ Visible=����� .R��� $
��Bold"����� *&$WorkStream.WorkStreamInfo.StreamName+% �� ContentTemplate% �� .R�3   -VPDLFramework.Views.Control_WorkStreamRecipes  �� .����  ��%2   '��       �� " ��.���  � y�$ Visible=�.@���  ) %Lan.Control_WorkEdit.HeaderWorkStream#���  .���� 
������ $��2q�$$ 0������ *WorkStreams+ ��%2   '��       �� " ��.���  � y�$ Visible=����� .R��� $
��Bold"����� *WorkStreamInfo.StreamName+% �� .R�9   3VPDLFramework.Views.Control_RuntimeWorkStreamTBEdit  &   	ToolBlock& �� *
DLOutputTB+��  $	  Lan.Control_WorkEdit.HeaderGroup#��� 	 .���� 
������ $��2q�$$ 0������ *
WorkGroups+ ��%2   '��       �� " ��.���  � y�$ Visible=����� .R��� $
��Bold"����� *GroupInfo.GroupName+% �� .R�8   2VPDLFramework.Views.Control_RuntimeWorkGroupTBEdit  '   GroupToolBlock' �� *	ToolBlock+�� .���� 
������ $��2q�$$ 0������ *WorkImageSources+ ��%2   '��       �� " ��.���  � y�$ Visible=����� .R��� $
��Bold"����� *!ImageSourceInfo.ImageSourceName+% �� .R�:   4VPDLFramework.Views.Control_RuntimeImageSourceTBEdit  (   	ToolBlock( �� *	ToolBlock+�� .���� 
������ $��2q�$$ 0������ *WorkStreams+ ��%2   '��       �� " ��.���  � y�$ Visible=����� .R��� $
��Bold"����� *WorkStreamInfo.StreamName+% �� .R�3   -VPDLFramework.Views.Control_RuntimeParameters  �� $��1��$ 
Horizontal=�.+�	� -   ) 	�Name$) 	btn_Image��$��5q�* 	� 	GroupName$* 
tabControl������   
 Style_RadioButton_TabControl+ �� BasedOn#+ [
 " 	� ��  �� $		 tab��$ SelectedIndex�
 0.a���  �@d�	IsChecked$
 TrueY�.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� + 'Lan.Control_WorkRuntime.BtnRuntimeImage#���  	� -   $) btn_Data��$��5q�$* 
tabControl������ *CmdShowProductData+���� #+ [
 " 	� ��  �� $		 tab��$ SelectedIndex�
 1.a���  �@d�	IsChecked$
 TrueY�.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� * &Lan.Control_WorkRuntime.BtnRuntimeData#���  	� -   $) btn_Log��$��5q�$* 
tabControl������ *CmdShowWorkLog+���� #+ [
 " 	� ��  �� $		 tab��$ SelectedIndex�
 2.a���  �@d�	IsChecked$
 TrueY�.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� ) %Lan.Control_WorkRuntime.BtnRuntimeLog#���  	� -   $) btn_Recipes��$��5q�$* 
tabControl��{��� *IsSystemOnline+# [ ���� #+ [
 " 	� ��  �� $		 tab��$ SelectedIndex�
 3.a���  �@d�	IsChecked$
 TrueY�P� , P� Property, � {�- P� Value- .  . ���  � ��
 �����.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� , (Lan.Control_WorkRuntime.BtnRecipesManage#���  	� -   $) btn_ToolBlocks��$��5q�$* 
tabControl��{��� *IsSystemOnline+# [ y���  �� $ local:MainWindowL�$ DataContext.IsLogin� $  BoolToVisiblityDefaultVisibleCvt# [ ���� #+ [
 " 	� ��  �� $		 tab��$ SelectedIndex�
 4.a���  �@d�	IsChecked$
 TrueY�P� , � {�- .  . ���  � ��
 �����.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� . *Lan.Control_WorkRuntime.BtnAlgorithmManage#���  	� -   $) btn_Cameras��$��5q�$* 
tabControl��{��� *IsSystemOnline+# [ y���  �� $ local:MainWindowL�$ DataContext.IsLogin�# [ ���� #+ [
 " 	� ��  �� $		 tab��$ SelectedIndex�
 5.a���  �@d�	IsChecked$
 TrueY�P� , � {�- .  . ���  � ��
 �����.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� + 'Lan.Control_WorkRuntime.BtnCameraManage#���  	� -	   $) btn_Parameters��$��5q�$* 
tabControl��{��� *IsSystemOnline+# [ y���  �� $ local:MainWindowL�$ DataContext.IsLogin�# [ ���� #+ [
 " 	� ��  �� $		 tab��$ SelectedIndex�
 6.a���  �@d�	IsChecked$
 TrueY�P� , � {�- .  . ���  � ��
 �����.���  �� �� .+��� $��2q�$
��Left=�$��Center=�#��[ $��16(�$	������ $��1��$��Center=�$��Center=� ) %Lan.Control_WorkRuntime.BtnParaManage#���  