   M S B A M L   `   `   `  ����H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  5      +    %VPDLFramework.Views.Control_SystemLog   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      3local!clr-namespace:VPDLFramework.Views     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5         Color_DarkBackground#��[  5
        � DataContext  5      ��   Locator �� Source# [  �� Path$	 Log�.��5      � .+�5   
   �� $��25��5      $��25��5      $��3q�5      $��Right=�5      $	��Top=�5       K� ZIndex$ 1��5      $	����5        Font_Ico#��[ 5        Style_Button_Ico#��[ 5      ��5      �� *
CmdClear+5      &   System.Windows.Controls.DataGrid  5   
   
�������$��2q�5        Style_DataGrid#��[ 5      ��5      �� *
LogItems+   Columns 5      4  .System.Windows.Controls.DataGridTemplateColumn  5      ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 200 6)    " Lan.Control_SystemLog.TextTime   Header# �  65      CellTemplate 5       �� 5!      .R�5"      ��  �� Padding$ 3q�5#   !   $��Center=�5$   !   ��5%   !   �� *Time+5&      5(        5)      $	 100 6)    # Lan.Control_SystemLog.TextLevel# �  65    5*      �� 5+      .R�5,      �� $��Center=�5-   !   
���������5/   !   �� *Level+.��68   ��50   "   �� 51   &   	 �� 
TargetType"	 ��6,   .�52   *   �� 
 �� Property
 � �� �� Value 53   .   �� 54   2    �� 
TargetType" ��6B   .R�55   6   �� $	��2,0q�6A    �� Orientation$ 
Horizontal=�6N   .+�56   :   �� $	��2,0q�57   =   $��Center=�58   =   #��[ 59   =   $��14(�5:   =   ��5;   >   �� 5<   B   "	 ��6H    �� Triggers 5=   F   �� 5>   J    �� Binding 6V   ��  �� ElementName$ 
tbLogLevel��$
 Text� �� Value	 Error6�   .a�5?   N   �� 
 � ��$	 ��6e   �� 5@   N   
 � ��  	Color_Red# [ 6k   5A   K   �� 5B   J    6V   �� $ 
tbLogLevel��$
 Text�	 Trace6�   .a�5C   N   �� 
 � ��$	 ��6e   �� 5D   N   
 � �� �#009ad66k   5E   K   �� 5F   J    6V   �� $ 
tbLogLevel��$
 Text� Warn6�   .a�5G   N   �� 
 � ��$	 ��6e   �� 5H   N   
 � �� �#ffe6006k   5I   K   �� 5J   J    6V   �� $ 
tbLogLevel��$
 Text�	 Debug6�   .a�5K   N   �� 
 � ��$	 ��6e   �� 5L   N   
 � �� �#1d953f6k   5M   K   5O   C   5Q   ;   �� 5R   :    ��Name$ 
tbLogLevel��5S   =   $��Center=�5T   =   #��z��5U   =   5V   7   5W   3   5Y   +   5Z   '   .��5[   1   5\      5]      5_        5`      $ * 6)    % !Lan.Control_SystemLog.TextMessage# �  63    5a      �� 5b      .R�5c      �� $ 3q�5d   !   $��Center=�5e   !   $
��Wrap=�5g   !   ��5f   !   �� *	Message+5g   !   5h      5j      5l      5m      5n      