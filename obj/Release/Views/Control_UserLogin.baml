   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  +    %VPDLFramework.Views.Control_UserLogin   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     .��� .+�� $��Center=�$��Center=�  � RowDefinitions  �� �� $
��1.5* ��� .+��� $��20q�$��Center=�$��30(�
������� '  #Lan.Control_UserLogin.TextUserLogin#���   � $��1��  �� ��  � ColumnDefinitions �� $
��0.8* ��� .+��� $��Center=�$��16(� & "Lan.Control_UserLogin.TextUserName#���  �� -   $��cb��$��1��$��16(�$
��Bold"� �� SelectedValuePath$ Content�� �� SelectedValue �� *
UserName+ �� Mode$ TwoWay=�����  �� 
TargetType" �� �� Triggers ��  �� Binding �� *	IsLogin+ �� Value True.a��� 	 �� Property	 �@��SelectedIndex
 �� Value$
 1����  �� *	IsLogin+	 False.a��� 	 �@��SelectedIndex$
 0��.@��� $��operator���� $��admin���� $��1��$��Center=�$��16(� & "Lan.Control_UserLogin.TextPassword#���  �� $��1��$��1��$	��0,5q�$
��Left=�$��Center=� �� Orientation$ 
Horizontal=�.+�G� -    G�Name$	 pwd��$	��150��$��Center=�$��16(��� $��5,0,0,0q�$��Center=�$��Center=�$	����  Font_Ico#��[ $��16(�  Style_Button_Ico#��[  + 'Lan.Control_UserLogin.BtnChangePassword � ToolTip# �  ���� *CmdEditAdminPassword+{��� *	IsLogin+�� $��2��$	��300��$��40��$
��0,10q�$��Center=�$��16(�
���   � �� 	IsDefault . ���� *
CmdLogin+���� " �� ��  ��  �� ElementName$ cb�� �� Path$ SelectedIndex� -1.a��� 	 � {�
 .  ��  �� $ cb��$ SelectedIndex� 0.a��� 	 � {�
 .  ��  �� *	IsLogin+ True.a��� 	 � �� # Lan.Control_UserLogin.BtnLogout#
 �  ��  �� *	IsLogin+	 False.a��� 	 � �� " Lan.Control_UserLogin.BtnLogin#
 �  