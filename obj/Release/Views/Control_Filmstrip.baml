   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem +    %VPDLFramework.Views.Control_Filmstrip   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  -local!clr-namespace:VPDLFramework.Views  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  3system&clr-namespace:System;assembly=mscorlib  
�������.��� .+��� 
�������$��2q�� $��2q�  � RowDefinitions  �� $��30 ��� .+�� .+���    Color_DarkBackground#��[  
�������$��	0,0,0,0.5q��� $��25��$��25��$��2q�$
��Left=�$	����  Font_Ico#��[ $��18(� 0 ,Lan.Control_Filmstrip.MenuBar.BtnImportImage � ToolTip# �  ���� *CmdLoadImages+�� $��Center=�$��Center=� �� Orientation$ 
Horizontal=�.+��� $��25��$��25��$	����#��[ $��15(� / +Lan.Control_Filmstrip.MenuBar.BtnFirstImage# �  ���� *
CmdFirst+�� $��25��$��25��$	����#��[ $��12(� 2 .Lan.Control_Filmstrip.MenuBar.BtnPreviousImage# �  ���� *CmdPrevious+�� $��25��$��25��$	����#��[ $��12(� . *Lan.Control_Filmstrip.MenuBar.BtnNextImage# �  ���� *	CmdNext+�� $��25��$��25��$	����#��[ $��15(� . *Lan.Control_Filmstrip.MenuBar.BtnLastImage# �  ���� *	CmdLast+�� $��Right=�$ 
Horizontal=�.+��� $��3q�$��Center=�$	��Top=�$��16(�$
��Bold"����� *CurrentImageIndex+�� $��Center=�$��12(�$��/���� $��3q�$��Bottom=�$��12(����� *
ImageCount+� $��1��.+��� 
������ $��0q���.  $
��Auto=�$��Hidden=� �� SelectionMode$ Single=� %� IsContainerVirtualizable .  %� IsVirtualizing .  %� VirtualizationMode$ 	Recycling=����� *	ImageList+ �� SelectedIndex �� *CurrentIndex+ �� Mode$ OneWay=����� .R�$� 	 $� CanVerticallyScroll	 .  
 $� Orientation$
 
Horizontal=�����  �� 
TargetType" ��.���  �� Property � �� �� Value ��  �� 
TargetType" ��.R��� $��2q�#��z��$��5q�._��� $��1q�$��3q����� " �� �� Triggers ��  �� Binding �� *IsProcessedImage+ �� Value True.a���  � ��
 � � ���  �� *IsProcessedImage+	 False.a���  � ��
 ���� ._��� $	��100��$��2q�$��Uniform=�����  �� Property � ��  ICogImageToBitmapImageCvt �� 	Converter# [ ���� " �� P�  P� Property �@J�IsMouseOver P� Value . . ���  �@J�Opacity$	 0.7Q� P�  �@��
IsSelected . . ���  � ��
 ��  �P�  �@��
IsSelected .  . ���  � ��
 �����.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ MouseDoubleClick��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command �� *CmdRunSelectedImage+   CommandParameter ��  �� RelativeSource ��  �� AncestorType$ ListBoxL� �� Path$ SelectedIndex�