   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration :    4VPDLFramework.Views.Control_RuntimeImageSourceTBEdit   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     _wfiOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration        Color_DarkBackground#���   .���   � RowDefinitions  �� �� $��50 ��� $��30 �.+�7  1System.Windows.Forms.Integration.WindowsFormsHost  -     Name$ tbHost��   
Background
 ������� $��1��$	��1,5q�
�������$��1q�._��  � ColumnDefinitions �� $	��100 ��� .+��� $��Center=� �� Orientation$ 
Horizontal=�$	��100��.+��� $
��10,0q�
������� ' #Lan.Control_WorkEdit.TextCameraMode#��[ �� $��Center=�
������� d� 	IsChecked �� *ImageSourceInfo.IsUseCam+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $��1��$��Center=�$ 
Horizontal=�.+��� $
��10,0q�$��Center=�
������� ' #Lan.Control_WorkEdit.TextFolderPath#��[ �� 
�������$��0,0,0,1q�._��� $	��200��$��Center=�
����������� *ImageSourceInfo.ImageFilePath+�� $��30��$��20��$	��5,0q�$	��...������  �� RelativeSource �� 	 �� AncestorType$	 local:Control_WorkRuntimeL�
 �� Path$$
 DataContext.CmdSelectImageFile����� *!ImageSourceInfo.ImageSourceName+�� $��2�� " Lan.Window_SystemSetup.BtnSave#���  $
��Bold"�����  �� $	 local:Control_WorkRuntimeL�$*
 $DataContext.CmdSaveImageSourceConfig����� *!ImageSourceInfo.ImageSourceName+