   M S B A M L   `   `   `  ���� H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  (    "VPDLFramework.Views.Window_Waiting   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     3local!clr-namespace:VPDLFramework.Views     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     $	��250��$	��250��  � AllowsTransparency  . 
������  � WindowStartupLocation$ CenterScreen=� � WindowStyle$
 None=� � 	Resources    borderStyle �� 
TargetType �� Property �� Value��%�   &	        �� " ��.���  � ��$ 25����  � ��$ 50����  �@��CornerRadius$ 16����  � ��
 �������  �@J�RenderTransformOrigin$ 0.5,0.5&� J� Effect 3  -System.Windows.Media.Effects.DropShadowEffect     
BlurRadius$ 15Q�	   	Direction$		 -90Q�
   Opacity$	
 0.7Q�   ShadowDepth$ 0Q�.��� $��10q�.+��� $��2q�  Color_DarkBackground#��[ 
�������$��2q� �� CornerRadius$ 0��� $��10q�.+��  � RowDefinitions �� $	��170 ��� .+�� -    �Name$ animationCanvas��$	��150��$	��150��$
��0,10q� J� RenderTransformOrigin$ 0.5 0.5&�z��� -   rt�� � Triggers :�  :� RoutedEvent$ Loaded��.[��� .���� -    ��Name$ animate��.��U�  g� RepeatBehavior$ Forever�� �� 
TargetName$ animationCanvas�� �� TargetProperty$9 3(UIElement.RenderTransform).(RotateTransform.Angle)�.\�n�  O� KeyTime$ 0:0:0.1�� O� Value$ 45Q�n� $ 0:0:0.2��$ 90Q�n� $ 0:0:0.3��$	 135Q�n� $ 0:0:0.4��$	 180Q�n� $ 0:0:0.5��$	 225Q�n� $ 0:0:0.6��$	 270Q�n� $ 0:0:0.7��$	 315Q�.+���  J� Opacity$	 0.5Q�#��[  z�V� .~�T�  T� Y$	 -50Q���  �� Angle$ 0Q��� $	 0.6Q�#��[  z�V� .~�T� $	 -50Q��� $ 45Q��� $	 0.7Q�#��[  z�V� .~�T� $	 -50Q��� $ 90Q��� $	 0.8Q�#��[  z�V� .~�T� $	 -50Q��� $	 135Q��� $	 0.9Q�#��[  z�V� .~�T� $	 -50Q��� $	 180Q��� $	 0.3Q�#��[  z�V� .~�T� $	 -50Q��� $	 225Q��� $	 0.3Q�#��[  z�V� .~�T� $	 -50Q��� $	 270Q��� $	 0.3Q�#��[  z�V� .~�T� $	 -50Q��� $	 315Q�� $��1��$��10q� � ColumnDefinitions �� �� .+��� -    ��Name$ tb��$��Right=�$��Center=�$��12(�$
��Bold"�
��������� -   $	 tb2��$��1��$
��Left=�$��Center=�$��12(�$
��Bold"�
������� :� $ Loaded��.[��� .���� -   $ animate1��.���� $ Forever��$	 tb2��$
 Text�.�a�  �� KeyTime$ 0:0:0.1�� �� Value$  ��a� $ 0:0:0.2��$ ·��a� $ 0:0:0.3��$
 ··��a� $ 0:0:0.4��$ ···��a� $ 0:0:0.5��$ ····��a� $ 0:0:0.6��$ 
·····��a� $ 0:0:0.7��$ ·······��