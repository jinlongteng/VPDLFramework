   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  -    'VPDLFramework.Views.Window_SetupStartup   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f45xceed)http://schemas.xceed.com/wpf/xaml/toolkit  -    +  'Lan.Window_SystemSetup.TitleSystemSteup  � Title#  �   $	��800��$	��600��  Color_DarkBackground#���  
������� � 
ResizeMode$ NoResize=� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ 
ToolWindow=� � DataContext ��   Locator �� Source# [  �� Path$ SystemSetup�.��� .+��� 
�������$��1q�._��  � RowDefinitions �� �� $��40 �.+��  � ColumnDefinitions �� �� .+��  �� �� .+��� $��3q�$	��0.5q�$
��Bold"� * &Lan.Window_SystemSetup.HeaderWorkSteup#���    GroupBoxStyle#��[ .��&   System.Windows.Controls.DataGrid  $	��0.5q�
�������$	��0.5q�$��Normal"�  Style_DataGrid#��[ ���� *" StartupSettings.WorksStartupInfo+	   Columns	 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn
   Width5  /System.Windows.Controls.DataGridLengthConverter$
 *  ' #Lan.Window_SystemSetup.TextWorkName   Header# �     CellTemplate �� .R�� .+���  �� Orientation$ 
Horizontal=�.+��� $��10��$��10��$	��5,0q�
������� �� CornerRadius$ 5���� $��Center=�$��Center=����� $ WorkName�  $
 *  ( $Lan.Window_SystemSetup.TextWorkTitle# �   �� .R�� .+��� $��3q�$��2q�
����������� $ Title� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�  $
 *  & "Lan.Window_SystemSetup.TextDefault# �   �� .R�� .+�	� $��Center=�$��Center=�$��	ExtraBold"� 	� 	GroupName$ default�� d� 	IsChecked �� $ IsDefaultWork�$ TwoWay=�$ PropertyChanged=��� $��1��$��3q�$	��0.5q�$
��Bold"� ,	 (Lan.Window_SystemSetup.HeaderCameraOrder#��� 	 #��[  �� *CameraOrderViewModel+.���  �� �� $��30 �.+�  -     Name$ 	dgCameras��$	��0.5q�
�������$	��0.5q�$��Normal"�#��[ ���� *CamerasInfo+	    �� .R�� .+��� $	��5,0q�$��Center=� 
 Font_Ico#��[
 $	����  $
 *  ! Lan.Window_SystemSetup.TextSN# �   �� .R�� .+��� $ 
Horizontal=�.+��� $��3q�._��� $	��3,0q� �� Padding$ 3q�$��Center=�$��Center=����� $ serialNo�  $
 2*  & "Lan.Window_SystemSetup.TextCamName# �   �� .R�� .+��� $��3q�$ 2q����� *name+�� $��1��.+��� $��50��$��1q�$	����#��[
 $��16(�$��Normal"�  Style_Button_Ico#��[ ���� *	CmdItemUp+����  �� ElementName$ 	dgCameras��$ SelectedItem��� $��50��$��1q�$	����#��[
 $��16(�$��Normal"�#��[ ���� *CmdItemDown+���� $ 	dgCameras��$ SelectedItem��� $��30��$��1q�$	����#��[
 $��16(�$��Normal"�#��[  ( $Lan.Window_SystemSetup.BtnRefreshCam � ToolTip# �  ���� *CmdUpdateCamerasInfo+�� $��30��$��1q�$	����#��[
 $��17(�$��Normal"�#��[  ) %Lan.Window_SystemSetup.BtnClearConfig# �  ���� *CmdClearIniFileCamerasInfo+� $��1�� �� $��80 ��� $	��100 ��� $	��160 ��� $	��120 �.+��� $��3q�$	��0.5q�$
��Bold"� - )Lan.Window_SystemSetup.HeaderSystemServer#���  #��[ .���� $��1��$
��5,10q�$ 
Horizontal=�.+��� $ 2q�$��Center=�$��Normal"�$	��IP:���� $	��100��$��Center=�$��Normal"����� *StartupSettings.LocalIPs+ �� SelectedValue �� *#!StartupSettings.SystemTCPServerIP+$ TwoWay=�$ PropertyChanged=��� $��2q�$ 2q�$��Center=�$��Normal"�$��Port:��%  Xceed.Wpf.Toolkit.IntegerUpDown  $��50��$��2q�$��Center=��  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   DefaultValue$ 10X�$��Normal"�   Maximum$ 100000X�   Minimum$ 10X�$ 	10-100000��   Value �� *%#StartupSettings.SystemTCPServerPort+$ TwoWay=�$ PropertyChanged=��� $��1��$��3q�$	��0.5q�$
��Bold"� $  Lan.Window_SystemSetup.HeaderFFP#���  #��[  �� # [ $ CommCard�.���  �� �� .+��� $��5q�$ 
Horizontal=�.+��� $��Center=�$��Normal"� ' #Lan.Window_SystemSetup.TextCommCard#���  �� $	��2,0q�$��Center=����� *
SerialNo+�� $��15��$��15��$
��10,0q�$ 10������  �� 
TargetType" �� �� Triggers ��  �� Binding �� *IsCardExist+ �� Value	 false.a���   �� Property  � ��! �� Value
! �������  �� *IsCardExist+ true.a���   � ��
! � � ��� $��Center=�$��Normal"� + 'Lan.Window_SystemSetup.TextProtocalType#���  �� $	��100��$	��2,0q�$��Center=�$��Normal"����� *FfpTypeContstantsBindableList+ �� " �� RelativeSource" �� # �� AncestorType$# WindowL�$) #DataContext.StartupSettings.FfpType�$ TwoWay=�$ PropertyChanged=��� $��1��$��5q�$��Center=�$��Center=�$��Normal"� . *Lan.Window_SystemSetup.TextEnableOtherCard#���  
������� �� # [ $! SystemSetup.StartupSettings� �� *EnableThirdCard+$ TwoWay=�$ PropertyChanged=��� $��2��$��3q�$	��0.5q�$
��Bold"� + 'Lan.Window_SystemSetup.HeaderDiskManage#���  #��[ .���� $
��5,10q�$ Vertical=�.+�� $	��0,2q� �� $
��0.5* ��� .+��� $��Center=�$��Normal"� * &Lan.Window_SystemSetup.TextProjectDisk#���  �� $��1��$��Normal"�$ �� SelectedIndex$$ 0������ *StartupSettings.DiskList+ �� *)'StartupSettings.SelectedProjectDiskName+$ TwoWay=�$ PropertyChanged=�� $	��0,2q� �� $
��0.5* ��� .+��� $��Center=�$��Normal"� ( $Lan.Window_SystemSetup.TextImageDisk#���  �� $��1��$��Normal"�$$ 0������ *StartupSettings.DiskList+ �� *'%StartupSettings.SelectedImageDiskName+$ TwoWay=�$ PropertyChanged=�� $	��0,2q� �� $
��0.5* ��� .+��� $��Center=�$��Normal"� + 'Lan.Window_SystemSetup.TextImageStorage#���  �� $��1��$ 
Horizontal=�.+��� $��0,0,5,0q�$��Center=�$��Normal"�$��OK��  $ 30X�$��Normal"�$ 10000X�$ 1X� �� *(&StartupSettings.ImageRetainedDaysForOK+$ TwoWay=�$ PropertyChanged=��� $	��2,0q�$��Center=�$��Normal"� " Lan.Window_SystemSetup.TextDay#���  �� $	��5,0q�$��Center=�$��Normal"�$��NG��  $ 30X�$��Normal"�$ 10000X�$ 1X� �� *(&StartupSettings.ImageRetainedDaysForNG+$ TwoWay=�$ PropertyChanged=��� $	��2,0q�$��Center=�$��Normal"�#���  � $	��0,2q� �� $
��0.5* ��� .+��� $��Center=�$��Normal"� * &Lan.Window_SystemSetup.TextDataStorage#���  �� $��1��$ 
Horizontal=�.+�  $ 30X�$��Normal"�$ 10000X�$ 1X� �� *" StartupSettings.DataRetainedDays+$ TwoWay=�$ PropertyChanged=��� $	��2,0q�$��Center=�$��Normal"�#���  �� $��3��$��3q�$	��0.5q�$
��Bold"� ) %Lan.Window_SystemSetup.HeaderLanguage#���  #��[ .���� $
��5,10q�$ Vertical=�.+�� $	��0,2q� �� $
��0.5* ��� .+��� $��Center=�$��Normal"� * &Lan.Window_SystemSetup.TextLanSelected#���  �� $��1��$��Normal"�$$ 0������ *StartupSettings.LanguageList+ �� *" StartupSettings.SelectedLanguage+$ TwoWay=�$ PropertyChanged=��� $	��0,2q� 0 ,Lan.Window_SystemSetup.BtnExportLanguageFile#���  $��Normal"����� *CmdExportLanguageFile+�� $	��0,2q� 0 ,Lan.Window_SystemSetup.BtnImportLanguageFile#���  $��Normal"����� *CmdImportLanguageFile+� $��1��.+��� ._�� .+��� $��5q�$
��Left=�$��Center=�$��Center=� , (Lan.Window_SystemSetup.TextStartupOnline#���  
������� �� * StartupSettings.IsStatupOnline+�� $��60��$��25��$��5q�$��Right=� "  Lan.Window_SystemSetup.BtnSave#���   ���� *	CmdSave+