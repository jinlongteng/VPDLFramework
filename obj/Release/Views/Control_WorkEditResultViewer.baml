   M S B A M L   `   `   `  ���� k  gCognex.VisionPro3D.Display.Controls, Version=85.0.0.0, Culture=neutral, PublicKeyToken=ef0f902af9dee505dMclr-namespace:Cognex.VisionPro3D;assembly=Cognex.VisionPro3D.Display.ControlsCognex.VisionPro3D  H DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration 6   0VPDLFramework.Views.Control_WorkEditResultViewer   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     `cognexMclr-namespace:Cognex.VisionPro3D;assembly=Cognex.VisionPro3D.Display.Controls     @d2http://schemas.microsoft.com/expression/blend/2008     [ WNSystem.Data.DwayneNeed.Wpf, Version=2023.1.1.733, Culture=neutral, PublicKeyToken=null8interop*http://erikzhouxin.com/wpf/xaml/dwayneneed  -local!clr-namespace:VPDLFramework.Views  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  YwfiOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration  -   
�������.��� $��2q�.+��� 
�������$	��0.5q�� $��1q�  � RowDefinitions  �� $��30 ��� �� $��32 �.+�� .+���  �� Padding$ 2q�$��Center=�$
��Bold"����� *.,WorkStream.ResultViewModel.StreamOrGroupName+� $��1��.+��� 
�������$��0,0.5q�7  1System.Windows.Forms.Integration.WindowsFormsHost  -     Name$
 host��$	��0,2q�y��� *'%WorkStream.WorkStreamInfo.IsDisplay3D+ #  BoolToVisiblityDefaultHiddenCvt �� 	Converter# [  '   !Cognex.VisionPro3D.Cog3DDisplayV2  -     Name$ 	display3D��$	��0,2q�y��� *'%WorkStream.WorkStreamInfo.IsDisplay3D+ $  BoolToVisiblityDefaultVisibleCvt# [ � $��2��.+���  �� Orientation$ 
Horizontal=�.+��� $ 3q�$��Center=� . *Lan.Control_RuntimeResultViewer.TextResult#���  �� -    ��Name$ 
textResult��$ 3q�$
��Left=�$��Center=�$��13(����� *-+WorkStream.ResultViewModel.ResultForDisplay+�� $��2q�$��Right=�$ Vertical=�.+��� $��40��$��15��$��0,1,0,1q�$��Right=�
���   �$��1q� �� CornerRadius$ 0������  �� 
TargetType" ��	 �� Triggers	 �� 
 �� Binding
 �� *)'WorkStream.ResultViewModel.ResultStatus+ �� Value True.a���  �� Property � �� �� Value
 � � ��� 
 �� *)'WorkStream.ResultViewModel.ResultStatus+	 False.a���  � ��  	Color_Red# [ �� $	��5,0q�$��Right=�$ 
Horizontal=�.+��� -   $ tbTime��$��Right=�$��Bottom=�$��10(����� *(&WorkStream.ResultViewModel.ElapsedTime+�� $��Bottom=�$��10(����� " ��	 �� 
 ��  �� ElementName$ tbTime�� �� Path$
 Text�  StringIsNullCvt# [ 	 False.a���  � ��$	  ms��