   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  +    %VPDLFramework.Views.Control_SystemLog   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006        Color_DarkBackground#��[    � DataContext  ��   Locator �� Source# [  �� Path$	 Log�.��� .+��� $��25��$��25��$��3q�$��Right=�$	��Top=� K� ZIndex$ 1��$	����  Font_Ico#��[   Style_Button_Ico#��[ ���� *
CmdClear+&   System.Windows.Controls.DataGrid  
�������$��2q�  Style_DataGrid#��[ ���� *
LogItems+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 200  " Lan.Control_SystemLog.TextTime   Header# �     CellTemplate �� .R���  �� Padding$ 3q�$��Center=����� *Time+  $	 100  # Lan.Control_SystemLog.TextLevel# �   �� .R��� $��Center=�
����������� *Level+.������ 	 �� 
TargetType"	 ��.��� 
 �� Property
 � �� �� Value ��  �� 
TargetType" ��.R��� $	��2,0q� �� Orientation$ 
Horizontal=�.+��� $	��2,0q�$��Center=�#��[ $��14(����� "	 �� �� Triggers ��  �� Binding ��  �� ElementName$ 
tbLogLevel��$
 Text� �� Value	 Error.a��� 
 � ��$	 ���� 
 � ��  	Color_Red# [ ��  �� $ 
tbLogLevel��$
 Text�	 Trace.a��� 
 � ��$	 ���� 
 � �� �#009ad6��  �� $ 
tbLogLevel��$
 Text� Warn.a��� 
 � ��$	 ���� 
 � �� �#ffe600��  �� $ 
tbLogLevel��$
 Text�	 Debug.a��� 
 � ��$	 ���� 
 � �� �#1d953f��  ��Name$ 
tbLogLevel��$��Center=�#��z��.��  $ *  % !Lan.Control_SystemLog.TextMessage# �   �� .R��� $ 3q�$��Center=�$
��Wrap=����� *	Message+