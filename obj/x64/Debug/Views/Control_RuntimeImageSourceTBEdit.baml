   M S B A M L   `   `   `  ����H  DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views  5      ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35tOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration System.Windows.Forms.Integration 5      :    4VPDLFramework.Views.Control_RuntimeImageSourceTBEdit   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      3local!clr-namespace:VPDLFramework.Views     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      _wfiOclr-namespace:System.Windows.Forms.Integration;assembly=WindowsFormsIntegration     5         Color_DarkBackground#���   5      .��5      �   � RowDefinitions  5   
   �� 5      �� 5      $��50 �6   �� 5      $��30 �6   .+�5   
   7  1System.Windows.Forms.Integration.WindowsFormsHost  -   5          Name$ tbHost��5         
Background
 �����6/   �� 5   
   $��1��5      $	��1,5q�5      
�������$��1q�5      ._�5      �  � ColumnDefinitions 5      �� 5      $	��100 �6'   �� 5      .+�5      �� $��Center=�6    �� Orientation$ 
Horizontal=�68   $	��100��6Q   .+�5      �� $
��10,0q�5       
������� ' #Lan.Control_WorkEdit.TextCameraMode#��[ 5"      �� 5#      $��Center=�5$      
������� d� 	IsChecked 5&      �� *ImageSourceInfo.IsUseCam+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�5'      �� 5(      $��1��5)      $��Center=�5*      $ 
Horizontal=�5+      .+�5,      �� $
��10,0q�5-      $��Center=�5.      
������� ' #Lan.Control_WorkEdit.TextFolderPath#��[ 50      �� 51      
�������$��0,0,0,1q�65   ._�52      �� $	��200��53      $��Center=�54      
���������56      �� *ImageSourceInfo.ImageFilePath+57      �� 58      $��30��59      $��20��5:      $	��5,0q�5;      $	��...��5>      ��5<      ��  �� RelativeSource �� 	 �� AncestorType$	 local:Control_WorkRuntimeL�
 �� Path$$
 DataContext.CmdSelectImageFile���5=      �� *!ImageSourceInfo.ImageSourceName+5>      5?      5@      5A      �� 5B   
   $��2��5C       " Lan.Window_SystemSetup.BtnSave#���  5F      $
��Bold"�5G      ��5D      ��  �� $	 local:Control_WorkRuntimeL�$*
 $DataContext.CmdSaveImageSourceConfig���5E      �� *!ImageSourceInfo.ImageSourceName+5G      5H      5I      