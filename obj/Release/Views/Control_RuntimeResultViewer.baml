   M S B A M L   `   `   `  ���� k  gCognex.VisionPro3D.Display.Controls, Version=85.0.0.0, Culture=neutral, PublicKeyToken=ef0f902af9dee505dMclr-namespace:Cognex.VisionPro3D;assembly=Cognex.VisionPro3D.Display.ControlsCognex.VisionPro3D  H DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration 5   /VPDLFramework.Views.Control_RuntimeResultViewer   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     `cognexMclr-namespace:Cognex.VisionPro3D;assembly=Cognex.VisionPro3D.Display.Controls     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  -local!clr-namespace:VPDLFramework.Views  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  YwfiOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration  -      Color_DarkBackground#���   
�������.��� $��2q�.+��� 
�������$	��0.5q�� $��1q�  � RowDefinitions  �� $��30 ��� �� $��32 �.+��  � ColumnDefinitions �� �� $	��100 �.+���  �� Orientation$ 
Horizontal=�.+��� $��0q�$
��Left=� & "Color_EditWorkModuleTitleBackgroud#��[  �� CornerRadius$ 0��._��� $��3q�$
��Left=�$��Center=�$
��Bold"�
���   ����� *StreamOrGroupName+�� $
��10,0q�$ 
Horizontal=�.+��� $��Center=�  Font_Ico#��[ $��16(�$	���� 3 /Lan.Control_RuntimeResultViewer.TextBufferQueue � ToolTip# �  �� $	��2,0q�$��Center=�$��:���� $��Center=����� *BufferedImagesCount+�� $��1��$��Right=�$ 
Horizontal=�.+�d� -    d�Name$ togBtnLiveMode��$��30��$	��2,0q�$	����#��[ $��16(� / +Lan.Control_RuntimeResultViewer.BtnLiveMode# �  ����  �� RelativeSource ��  �� AncestorType$ local:Control_WorkRuntimeL� �� Path$ DataContext.CmdLiveMode�����  �� $' !local:Control_RuntimeResultViewerL�$# DataContext.StreamOrGroupName�	 d� 	IsChecked	 �� *
IsLiveMode+
 �� Mode$
 OneWay=�{���  �� $ local:Control_WorkRuntimeL�$  DataContext.IsSystemOnline�  BoolInverseCvt �� 	Converter# [ �� -   $��30��$	����#��[ $��16(�  Style_Button_Ico#��[  4 0Lan.Control_RuntimeResultViewer.BtnManualTrigger# �  ����  �� $ local:Control_WorkRuntimeL�$" DataContext.CmdRunWorkStream�����  �� $' !local:Control_RuntimeResultViewerL�$# DataContext.StreamOrGroupName�{���  �� $ local:Control_WorkRuntimeL�$  DataContext.IsSystemOnline�# [ y��� *IsWrokStreamType+ $  BoolToVisiblityDefaultVisibleCvt# [ � $��1��.+��� 
�������$��0,0.5q�7  1System.Windows.Forms.Integration.WindowsFormsHost  -     Name$
 host��$	��0,2q�y��� *IsDisplay3D+ #	 BoolToVisiblityDefaultHiddenCvt# [	 '   !Cognex.VisionPro3D.Cog3DDisplayV2  -     Name$ 	display3D��$	��0,2q�y��� *IsDisplay3D+# [ � $��2��.+��� $ 
Horizontal=�.+���  �� Padding$ 3q�$��Center=� .
 *Lan.Control_RuntimeResultViewer.TextResult#��� 
  �� *TriggerTime+�� -    ��Name$ 
textResult��$ 3q�$
��Left=�$��Center=�$��13(����� *ResultForDisplay+�� $��2q�$��Right=�$ Vertical=�.+��� $��40��$��15��$��0,1,0,1q�$��Right=�
���   �$��1q�$ 0������  �� 
TargetType" �� �� Triggers ��  �� Binding �� *ResultStatus+ �� Value True.a���  �� Property � �� �� Value
 � � ���  �� *ResultStatus+	 False.a���  � ��  	Color_Red# [ �� $	��5,0q�$��Right=�$ 
Horizontal=�.+��� -   $ tbTime��$��Right=�$��Bottom=�$��10(����� *ElapsedTime+�� $��Bottom=�$��10(����� " �� ��  ��  �� ElementName$ tbTime��$
 Text�  StringIsNullCvt# [ 	 False.a���  � ��$	  ms��