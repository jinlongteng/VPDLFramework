   M S B A M L   `   `   `  ���� D  @AdonisUI, Version=1.17.1.0, Culture=neutral, PublicKeyToken=null5(clr-namespace:AdonisUI;assembly=AdonisUIAdonisUI  H DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views $   VPDLFramework.Views.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     8aui(clr-namespace:AdonisUI;assembly=AdonisUI     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     3views!clr-namespace:VPDLFramework.Views     -     �Name$  mainwin��$
��1200��$	��800�� � AllowsTransparency .     Color_DarkBackground#��[  $��12(�
������� � 
ResizeMode$ CanResizeWithGrip=� � WindowStartupLocation$ CenterScreen=� � WindowState$ 	Maximized=� � Title �� *Title+ � DataContext ��   Locator �� Source# [  �� Path$
 Main����� 	 �� 
TargetType"	 �
 �� Triggers
 ��  �� Binding �� *IsOnlineMode+ �� Value True.a���  �� Property �@�WindowStyle �� Value$
 None=�.��� .+��  � RowDefinitions �� $��55 ��� �� $��25 �.+�� -    �Name$ logoGrid��.+��� 
���   �  Color_MainColor#��[ $��0,0,0,2q����� "	 ��
 ��  �� *	IsAlarm+ True.a���  � �� �#DD2C00 N� EnterActions ��  ��Name$ alarmStoryboard��.���� .����  g� RepeatBehavior$ Forever�� �� TargetProperty$1 +(Border.Background).(SolidColorBrush.Color)�.g���  �� KeyTime$ 0:0:0�� �� Value #DD2C00�� $ 0:0:1�� #53261f��  �� *	IsAlarm+	 False.a���  �@J�Opacity$ 1Q� ��  �� BeginStoryboardName$ alarmStoryboard���  � ColumnDefinitions �� $	��240 ��� �� $	��320 �.+��� #��[ ._���  �� Orientation$ 
Horizontal=�.+�d� -    d�Name$ btnProjectList��$��40��$��40��$��2q�
������ 
������ $	����  Font_Ico#��[ $��30(�
���   � # Lan.MainWin.MenuBar.BtnWorkList � ToolTip# �   d� 	IsChecked �� *IsShowWorkList+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $	��180��$��5q�$��Center=�$��Center=�$/��)../Resources/Images/Cognex_Logo_Black.png��$��Uniform=��� $��1��$
��Left=�$��Center=�._��� $��5,0,5,8q�$��Center=�
������ |�.  $+��%../Resources/Font/#创客贴金刚体)�$��45(�$
��Bold"� �� IsInputMethodEnabled .  ���� *	WorkTitle+���� "	 ��
 ��  �� *IsWorkLoaded+	 False.a���  � ��
 �������  �� *IsWorkLoaded+ True.a���  � ��
 ������ $��2��$��Right=�.+��� $��Right=�$ 
Horizontal=�.+��� $��40��$��40��
������ 
������ $	����#��[ $��30(�#��[ $ 清除警报������ *CmdClearAlarm+y��� *	IsAlarm+ $  BoolToVisiblityDefaultVisibleCvt  �� 	Converter#  [ �� $��40��$��40��
������ 
������ #��[ $��30(� ! Lan.MainWin.MenuBar.BtnOnLine# �  ���� *CmdSetSystemOnline+���� *IsOnlineMode+   BoolToColorSpringGreenRedCvt#  [ {��� *IsWorkLoaded+���� "	 ��
 ��  �� *IsOnlineMode+ true.a���  � ��$	 ����  �� *IsOnlineMode+	 false.a���  � ��$	 ��P� ! P� Property! � {�" P� Value" . . ���  � ��
 ���� P� ! � {�" .  . ���  � ��
 ���� �� -   # ��Name$# btnLog��$��40��$��40��$��2q�$��Center=�
������ 
������ $	����#��[ $��30(� $  Lan.MainWin.MenuBar.BtnSystemLog# �  ����  	 Style_Button_Ico$ �� BasedOn#$ [	 "	 ��
 P� ! �@J�IsMouseOver" . . ���  � ��
 �   �P� ! �@J�IsMouseOver" .  . ���  � ��
 ������� $��40��$��40��$��2q�
������ 
������ $	����#��[ $��30(� &
 "Lan.MainWin.MenuBar.BtnSystemSetup# � 
 ���� *CmdOpenSystemSetup+{��� *IsOnlineMode+  BoolInverseCvt#  [ ���� #$ [	 "	 ��
 P� ! �@J�IsMouseOver" . . ���  � ��
 �   �P� ! �@J�IsMouseOver" .  . ���  � ��
 ������� $��40��$��40��$��2q�
������ 
������ $	����#��[ $��30(� & "Lan.MainWin.MenuBar.BtnFileManager# �  ���� *CmdOpenFileManager+{��� *IsWorkLoaded+#  [ ���� #$ [	 "	 ��
 P� ! �@J�IsMouseOver" . . ���  � ��
 �   �P� ! �@J�IsMouseOver" .  . ���  � ��
 ������� $��40��$��40��$��2q�
������ 
������ $	����#��[ $��30(� & "Lan.MainWin.MenuBar.BtnVersionInfo# �  ���� *CmdOpenVersionInfo+���� #$ [	 "	 ��
 P� ! �@J�IsMouseOver" . . ���  � ��
 �   �P� ! �@J�IsMouseOver" .  . ���  � ��
 �����d� -   $ btnLogin��$��40��$��40��$��2q�
������ 
������ $��UserView��$	����#��[ $��30(� $  Lan.MainWin.MenuBar.BtnUserLogin# �  ���� *CmdLoadContentView+���� "	 d�
 ��  �� *	IsLogin+ True.a���  � �� �#426ab3P� ! �@J�IsMouseOver" .  . ���  � ��
 ������� $��40��$��40��$��2q�
������ 
������ $	����#��[ $��30(�  Lan.MainWin.MenuBar.BtnHelp# �  ���� *CmdOpenHelp+���� #$ [	 "	 ��
 P� ! �@J�IsMouseOver" . . ���  � ��
 �   �P� ! �@J�IsMouseOver" .  . ���  � ��
 ������ -   % �Name$	% pop��$	��350��$	��280��& � AllowsTransparency& .  $��Bottom=�$��Slide=�' � 	StaysOpen' .  ���� ( �� ElementName$( btnLogin��$ 	IsChecked�) � PlacementTarget) �� $( btnLogin��.���� #���   
�������$��2q�._�+  %VPDLFramework.Views.Control_UserLogin  *   InputPwd* �� *
Password+$ OneWayToSource=��� $��1��
�������$	��0,2q�._�� -   $ midGrid��$��1��.+�]� .+�*  $VPDLFramework.Views.Control_WorkList  $
��Left=����� "	  
 ��  �� *IsShowWorkList+ True.a���  � ��$	 240����  �� *IsShowWorkList+	 False.a���  � ��$ 0����  �� *IsOnlineMode+ True.a���  � ��$ 0��� $��Right=� �� �� $��2 ��� -   + ��Name$+ logRow��$	��200 �.+��� -	   , ��Name$, logSplitter��$��1��$��Stretch=�
������ �� $��2q�
�������$��2q�._��� $	��0,2q����� *ContentView++  %VPDLFramework.Views.Control_SystemLog  $��2��� $��2��.+��� #��[  
�������.@��� .���� $��Center=� + 'Lan.MainWin.StatusBar.TextStatusMonitor#���  �� ��  �� # [ $ WorkRuntime����� "	 ��.���  � �� �� .R�� .+��� $ 
Horizontal=�.+�� $	��200��$��1q� �� $��60 ��� .+��� $��Center=�$��Center=� $  Lan.MainWin.StatusBar.TextCamera#���  �� $��1��
���   ����� *WorkImageSources+���� .R�� - � Orientation$- 
Horizontal=����� "	 ��.���  � �� �� . �� 
TargetType". ��.R��� $��16��$��16��$��1q�/ �� CornerRadius$/ 2������ "	 ��
 ��  �� *ImageSourceInfo.IsOnline+ true.a���  � ��
 �2�2���  �� *ImageSourceInfo.IsOnline+	 false.a���  � ��  	Color_Red# [ ��  �� *ImageSourceInfo.IsUseCam+	 false.a���  � ��
 �����._��� $��Center=�$��Center=�#��[ 
���   �$	�������� "	 ��
 ��  �� *ImageSourceInfo.IsUseCam+	 false.a���  �@�ToolTip �� *!ImageSourceInfo.ImageSourceName+��  �� *ImageSourceInfo.IsUseCam+ true.a���  �@�ToolTip �� *CameraSerialNumName+� $	��200��$��1q� �� $��40 ��� .+��� $��Center=�$��Center=�$
��TCP:���� 0 ��Name$0 clientsPanel��$��1��
���   ����� *WorkTCPDevices+���� .R�� $- 
Horizontal=����� "	 ��.���  � �� �� ". ��.R��� $��16��$��16��$��1q�$/ 2������ "	 ��
 ��  �� *TCPDeviceInfo.Status+	 false.a���  � ��# [ ��  �� *TCPDeviceInfo.Status+ true.a���  � ��
 �2�2�._��� $��Center=�$��Center=�#��[ 
���   �$	���� `�   ConcatStringConverterCvt1 `� 	Converter#1 [ .1��� $! TCPDeviceInfo.TCPDeviceName��� $ TCPDeviceInfo.IPAddress��� $ TCPDeviceInfo.Port���  �� # [ $ CommCard�.���� $ 
Horizontal=�.+��� $��Center=�$��CC24:���� $��16��$��16��$	��5,0q�$/ 2������ "	 ��
 ��  �� *IsCardExist+	 false.a���  � ��
 �������  �� *IsCardExist+ true.a���  � ��
 �2�2�._��� $��Center=�$��Center=�#��[ 
���   �$	������ ��  �� *WindowsSystemInfo+���� "	 ��.���  � �� �� .R�� .+��� $ 
Horizontal=�.+�� $	��150��$��1q� �� $��30 ��� �� $��30 �.+��� $��Center=�$��Center=�$
��CPU:��� $��1��$��3q����#45b97c2 � Value2 �� *
CPUUsage+�� $��2��$ 
Horizontal=�.+��� $��Center=����� *
CPUUsage+�� $��Center=�$��%��� $	��150��$��1q� �� $��30 ��� �� $��30 �.+��� $��Center=�$��Center=�$
��RAM:��� $��1��$��3q����#45b97c2 �� *
RAMUsage+�� $��2��$ 
Horizontal=�.+��� $��Center=����� *
RAMUsage+�� $��Center=�$��%��� $	��200��$��1q� �� $��30 ��� �� $��80 �.+��� $��Center=�$��Center=� ! Lan.MainWin.StatusBar.TextHDD#���   �� *
DiskName+� $��1��$��3q����#45b97c2 �� *	DiskUsage+�� $��2��$ 
Horizontal=�.+��� $��Center=����� *DiskUsedSize+�� $��Center=�$��G/���� $��Center=����� *DiskTotalSize+�� $��Center=�$��G���� $��Right=�.���� $
��10,0q�$ 
Horizontal=�.+��� $��Center=�#��[ $��14(�$	������ $��10,0,5,0q����� *Date+�� -
   3 ��Name$3 border_systemTCPStatus��$��15��$��15��$/ 10������ "	 ��
 ��  �� *IsSystemTCPOpened+ true.a���  � �� �#45b97c��  �� *IsSystemTCPOpened+	 false.a���  � ��
 �������  �� *IsSystemTCPOpened+ true �� .���� .���� $ Forever��$1 +(Border.Background).(SolidColorBrush.Color)�.g��� $ 0:0:0�� #45b97c�� $ 0:0:0.5�� #122e29