   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  )    #VPDLFramework.Views.Window_CommCard   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f45xceed)http://schemas.xceed.com/wpf/xaml/toolkit   %  !Lan.Window_CommCard.TitleCommTest  � Title#  �   $	��900��$	��600��  Color_DarkBackground#���  
������� � WindowStartupLocation$ CenterScreen=� � DataContext ��   Locator �� Source# [  �� Path$ CommCard� � 	Resources   IOState �� 
TargetType �� Property �� Value	 �� 
TargetType
 �� CornerRadius �� Triggers �� Binding �� RelativeSource �� AncestorType �� Value��%k  &	       �� " ��.���  � ��$ 0q���  � ��$ 0q���  � �� �� "	 ��.R��� $��12��$��12��$��3q�
���   �#��z��$
 0������ " �� ��  ��  �� $ ListViewItemL�$ DataContext� True.a���  � ��
 � � ���  ��  �� $ ListViewItemL�$ DataContext�	 False.a���  � ��
 �����.���� 
�������$��1q�._��  � RowDefinitions �� $	��200 ��� .+��� $	��Top=� �� Orientation$ Vertical=�.+��� $��5q�
�������$	��0.5q�$
��Bold"� $  Lan.Window_CommCard.TextWriteFFP#���    GroupBoxStyle#��[ .���� $ 
Horizontal=�.+��� $
��10,0q�$��Center=�$��Normal"� " Lan.Window_CommCard.TextOffset#���  %  Xceed.Wpf.Toolkit.IntegerUpDown  $��80��$��Center=��  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   DefaultValue$ 0X�$��Normal"�   Maximum$
 1900X�   Minimum$ 0X�   Value �� *InspectionResultOffset+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� $
��10,0q�$��Center=�$��Normal"�   Lan.Window_CommCard.TextData#���  �� $	��200��$��Center=�$��Normal"����� *InspectionResultData+$ TwoWay=�$ PropertyChanged=� � ToolTip �� *InspectionResultData+�� $	��5,0q�$��Center=� ( $Lan.Window_CommCard.TextEnableScript#���  $��Normal"�
������� 0	 ,Lan.Window_CommCard.BtnWriteInpsectResultTip# � 	  d� 	IsChecked �� *IsOutputScriptEnable+�� $
��10,5q�$��3q�$��Center=� /
 +Lan.Window_CommCard.BtnWriteInpsectedResult#��� 
 $��Normal"����� *CmdWriteInspectionResult+�� -    ��Name$ cbStatus��$	��100��$
��10,0q�$��Center=�$��Normal"����� *FfpNdmStatusBitBindableList+�� $
��10,5q�$��3q�$��Center=� & "Lan.Window_CommCard.BtnWriteStatus#���  $��Normal"����� *CmdWriteStatusBit+����  �� ElementName$ cbStatus��$ SelectedItem��� $��5q�
�������$	��0.5q�$
��Bold"� # Lan.Window_CommCard.TextReadFFP#���  #��[ .���� $	��Top=�$ 
Horizontal=�.+��� $
��10,0q�$��Center=�$��Normal"�#���    $��80��$��Center=�$ 0X�$��Normal"�$
 1600X�$ 0X� �� *UserDataOffset+$ TwoWay=�$ PropertyChanged=��� $
��10,0q�$��Center=�$��Normal"� $  Lan.Window_CommCard.TextByteSize#���    $��80��$��Center=�$ 0X�$��Normal"�$
 1600X�$ 0X� �� *UserDataSize+$ TwoWay=�$ PropertyChanged=��� $
��10,0q�$��Center=�$��Normal"�#���  �� $	��200��$��Center=�$��Normal"� �� 
IsReadOnly . ���� *UserDataStr+$ TwoWay=�$ PropertyChanged=��� $	��5,0q�$��Center=�#���  $��Normal"�
������� * &Lan.Window_CommCard.BtnReadUserDataTip# �   �� *IsInputScriptEnable+�� $
��10,5q�$��3q�$��Center=� ' #Lan.Window_CommCard.BtnReadUserData#���  $��Normal"����� *CmdReadUserData+�� $��5q�
�������$	��0.5q�$
��Bold"�$	��I/O��#��[ .���  � ColumnDefinitions �� �� $
��1.8* �.+��� $ 
Horizontal=�.+��� $
��10,0q�$��Center=�$��Normal"� ! Lan.Window_CommCard.TextInput#���  �� $
��Left=�#��[ $��Disabled=�$��Disabled=����� *IOInputsState+���� .R��� $ 
Horizontal=��� $��1��$ 
Horizontal=�.+��� $	��5,0q�$��Center=�$��Normal"� " Lan.Window_CommCard.TextOutput#���  �� -    ��Name$ 	lstOutput��$
��Left=�$��Disabled=�$��Disabled=� �� SelectionMode$ Single=����� *IOOutputsState+����   �� BasedOn#  [ " �� P� ! P� Property! �@��
IsSelected" P� Value" . . ���  � ��$ 2q�P� ! �@��
IsSelected" .  . ���  � ��$ 0q����� .R��� $ 
Horizontal=��� $	��5,0q�$��3q�$��Center=� & "Lan.Window_CommCard.BtnForceOutput#���  $��Normal"� ) %Lan.Window_CommCard.BtnForceOutputTip# �  ���� *CmdForceOutput+���� $ 	lstOutput��$ SelectedIndex�� $��1�� �� $��20 ��� .+��� 
�������$	��0.5q�._�� .+��� $��Center=�$��Center=� ' #Lan.Window_CommCard.TextMessageList#���  �� $��20��$��Right=�# K� ZIndex$# 1��$	����  Font_Ico#��[ ���� *CmdClearMessages+�� $��1��$��0q�
������ 
���   �$��0q�
����������� *
Messages+���� " ��.���  � �� �� .R��� ._��� $��Center=�#��z 