   M S B A M L   `   `   `  ����H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  5      )    #VPDLFramework.Views.Window_CommCard   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      3local!clr-namespace:VPDLFramework.Views     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f45xceed)http://schemas.xceed.com/wpf/xaml/toolkit  5       %  !Lan.Window_CommCard.TitleCommTest  � Title#  �   5	      $	��900��5
      $	��600��5        Color_DarkBackground#���  5      
������� � WindowStartupLocation$ CenterScreen=�5       � DataContext 5      ��   Locator �� Source# [  �� Path$ CommCard� � 	Resources 5        IOState �� 
TargetType �� Property �� Value	 �� 
TargetType
 �� CornerRadius �� Triggers �� Binding �� RelativeSource �� AncestorType �� Value��%�  &	       �� 5   
   " ��6    .�5      ��  � ��$ 0q�6'   �� 5       � ��$ 0q�60   �� 5       � �� 5      �� 5      "	 ��6&   .R�5      �� $��12��5      $��12��5      $��3q�5      
���   �#��z��5      $
 0��5      ��5      �� 5    "   " ��6(    5!   &   �� 5"   *    66   ��  �� $ ListViewItemL�$ DataContext� True6�   .a�5#   .   ��  � ��
 � � �6K   5$   +   �� 5%   *    66   ��  �� $ ListViewItemL�$ DataContext�	 False6�   .a�5&   .   ��  � ��
 �����6K   5'   +   5)   #   5+      5,      5.      5/      50      .��51      �� 
�������$��1q�6    ._�52   
   �  � RowDefinitions 53      �� 54      $	��200 �6    �� 55      .+�57      �� $	��Top=�6    �� Orientation$ Vertical=�61   .+�58      �� $��5q�59      
�������$	��0.5q�5;      $
��Bold"�5<       $  Lan.Window_CommCard.TextWriteFFP#���  5=        GroupBoxStyle#��[ 5>      .��5?      �� $ 
Horizontal=�6!   .+�5@      �� $
��10,0q�5A      $��Center=�5B      $��Normal"�5C       " Lan.Window_CommCard.TextOffset#���  5D      %  Xceed.Wpf.Toolkit.IntegerUpDown  5E      $��80��5F      $��Center=�5G      �  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]   DefaultValue$ 0X�5H      $��Normal"�5I         Maximum$
 1900X�5J         Minimum$ 0X�5K         Value 5L      �� *InspectionResultOffset+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� 5M      $
��10,0q�5N      $��Center=�5O      $��Normal"�5P         Lan.Window_CommCard.TextData#���  5Q      �� 5R      $	��200��5S      $��Center=�5T      $��Normal"�5U      ��5V      �� *InspectionResultData+$ TwoWay=�$ PropertyChanged=� � ToolTip 5W      �� *InspectionResultData+�� 5X      $	��5,0q�5Y      $��Center=�5Z       ( $Lan.Window_CommCard.TextEnableScript#���  5[      $��Normal"�5\      
������� 0	 ,Lan.Window_CommCard.BtnWriteInpsectResultTip# � 	 5_       d� 	IsChecked 5^      �� *IsOutputScriptEnable+5_      �� 5`      $
��10,5q�5a      $��3q�5b      $��Center=�5c       /
 +Lan.Window_CommCard.BtnWriteInpsectedResult#��� 
 5e      $��Normal"�5f      ��5d      �� *CmdWriteInspectionResult+5f      �� 5g      -   5         ��Name$ cbStatus��5h      $	��100��5i      $
��10,0q�5j      $��Center=�5k      $��Normal"�5l      ��5m      �� *FfpNdmStatusBitBindableList+�� 5n      $
��10,5q�5o      $��3q�5p      $��Center=�5q       & "Lan.Window_CommCard.BtnWriteStatus#���  5t      $��Normal"�5u      ��5r      �� *CmdWriteStatusBit+��5s      ��  �� ElementName$ cbStatus��$ SelectedItem�5u      5v      5w      �� 5x      $��5q�5y      
�������$	��0.5q�5{      $
��Bold"�5|       # Lan.Window_CommCard.TextReadFFP#���  5}      #��[ 5~      .��5      �� $	��Top=�6!   $ 
Horizontal=�69   .+�5�      �� $
��10,0q�5�      $��Center=�5�      $��Normal"�5�      #���  5�        5�      $��80��5�      $��Center=�5�      $ 0X�5�      $��Normal"�5�      $
 1600X�5�      $ 0X�5�       5�      �� *UserDataOffset+$ TwoWay=�$ PropertyChanged=��� 5�      $
��10,0q�5�      $��Center=�5�      $��Normal"�5�       $  Lan.Window_CommCard.TextByteSize#���  5�        5�      $��80��5�      $��Center=�5�      $ 0X�5�      $��Normal"�5�      $
 1600X�5�      $ 0X�5�       5�      �� *UserDataSize+$ TwoWay=�$ PropertyChanged=��� 5�      $
��10,0q�5�      $��Center=�5�      $��Normal"�5�      #���  5�      �� 5�      $	��200��5�      $��Center=�5�      $��Normal"�5�       �� 
IsReadOnly . ��5�      �� *UserDataStr+$ TwoWay=�$ PropertyChanged=��� 5�      $	��5,0q�5�      $��Center=�5�      #���  5�      $��Normal"�5�      
������� * &Lan.Window_CommCard.BtnReadUserDataTip# �  5�       5�      �� *IsInputScriptEnable+5�      �� 5�      $
��10,5q�5�      $��3q�5�      $��Center=�5�       ' #Lan.Window_CommCard.BtnReadUserData#���  5�      $��Normal"�5�      ��5�      �� *CmdReadUserData+5�      5�      5�      �� 5�      $��5q�5�      
�������$	��0.5q�5�      $
��Bold"�5�      $	��I/O��5�      #��[ 5�      .��5�      �  � ColumnDefinitions 5�      �� 5�      �� 5�      $
��1.8* �6/   .+�5�      �� $ 
Horizontal=�6%   .+�5�      �� $
��10,0q�5�   !   $��Center=�5�   !   $��Normal"�5�   !    ! Lan.Window_CommCard.TextInput#���  5�   !   �� 5�      $
��Left=�5�   !   #��[ 5�   !   $��Disabled=�5�   !   $��Disabled=�5�   !   ��5�   !   �� *IOInputsState+��5�   "   �� 5�   &   .R�5�   *   �� $ 
Horizontal=�65   5�   '   5�      5�      �� 5�      $��1��6%   $ 
Horizontal=�65   .+�5�      �� $	��5,0q�5�   !   $��Center=�5�   !   $��Normal"�5�   !    " Lan.Window_CommCard.TextOutput#���  5�   !   �� 5�      -   5         ��Name$ 	lstOutput��5�   !   $
��Left=�5�   !   $��Disabled=�5�   !   $��Disabled=�5�   !    �� SelectionMode$ Single=�5�   !   ��5�   !   �� *IOOutputsState+��5�   "   �� 5�   &     �� BasedOn#  [ 6,   " ��6[    5�   *   P� 5�   .   ! P� Property! �@��
IsSelected" P� Value" . . �5�   2   ��  � ��$ 2q�6T   5�   /   P� 5�   .   ! �@��
IsSelected" .  . �5�   2   ��  � ��$ 0q�6T   5�   /   5�   '   ��5�   "   �� 5�   &   .R�5�   *   �� $ 
Horizontal=�65   5�   '   5�      �� 5�      $	��5,0q�5�   !   $��3q�5�   !   $��Center=�5�   !    & "Lan.Window_CommCard.BtnForceOutput#���  5�   !   $��Normal"�5�   !    ) %Lan.Window_CommCard.BtnForceOutputTip# �  5�   !   ��5�   !   �� *CmdForceOutput+��5�   !   �� $ 	lstOutput��$ SelectedIndex�5�   !   5�      5�      5�      5      � 5     $��1��6    5     �� 5     $��20 �6$   �� 5     .+�5     �� 
�������$	��0.5q�6,   ._�5     � .+�5     �� $��Center=�5	     $��Center=�5
      ' #Lan.Window_CommCard.TextMessageList#���  5     �� 5     $��20��5     $��Right=�5     # K� ZIndex$# 1��5     $	����5       Font_Ico#��[ 5     ��5     �� *CmdClearMessages+5     5     5     �� 5     $��1��5     $��0q�5     
������ 
���   �$��0q�5     
���������5     �� *
Messages+��5     �� 5     " ��6    .�5     ��  � �� 5   "   �� 5!  &   .R�5"  *   �� ._�5#  .   �� $��Center=�68   #��z 6S   5$  +   5%  '   5'     5(     5*     5+     5,     5-     5.     