   M S B A M L   `   `   `  ���� K  GValueConverters, Version=3.0.26.0, Culture=neutral, PublicKeyToken=nullJ6clr-namespace:ValueConverters;assembly=ValueConvertersValueConverters  H DVPDLFramework, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views ;"clr-namespace:VPDLFramework.ModelsVPDLFramework.Models O,clr-namespace:VPDLFramework.ValidationModelsVPDLFramework.ValidationModels 9!clr-namespace:VPDLFramework.ViewsVPDLFramework.Views *   $VPDLFramework.Views.Control_WorkEdit   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     M
converters6clr-namespace:ValueConverters;assembly=ValueConverters     @d2http://schemas.microsoft.com/expression/blend/2008     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  -local!clr-namespace:VPDLFramework.Views  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006  /models"clr-namespace:VPDLFramework.Models  8valid,clr-namespace:VPDLFramework.ValidationModels  -views!clr-namespace:VPDLFramework.Views  X TXceed.Wpf.Toolkit, Version=4.5.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f45xceed)http://schemas.xceed.com/wpf/xaml/toolkit     Color_DarkBackground#��[    � DataContext  ��   Locator �� Source# [  �� Path$
 Work� � 	Resources   Grid_HiddenInVproMode �� 
TargetType �� Triggers �� Binding �� RelativeSource �� AncestorType	 �� Value
 �� Property �� Value��%*  &	       �� " � ��  ��  �� $ local:Control_WorkEditL�$ DataContext.IsDLEnable�		 false.a��� 
 � ��$ 0���� 
 � ��$ 0q���  ��  �� $ local:Control_WorkEditL�$ DataContext.IsDLEnable�	 true.a��� 
 � ��$ 30���� 
 � ��$	 0,2q�.���  � ColumnDefinitions �� $��* �$	��200���� $��2 ��� $	��600��.+��� $��1��$	��0,2q�$��Stretch=�$��Stretch=�
������ �� $��2q�#��[  
�������$��2q�$��Disabled=�$��Disabled=����� *20EditStreamsViewModel.WorkStreamItemViewModelList+���� .R�A� ���� " ��.��� 
 � �� ��  �� 
TargetType" ��.R�6  0VPDLFramework.Views.Control_WorkEditResultViewer    ��    DisplayRecord �� *)'WorkStream.ResultViewModel.ResultRecord+   
RangeImage �� *'%WorkStream.ResultViewModel.RangeImage+�� $��2��$��2q�$
��Auto=�.���� 
�������$��2q�._��� $	��0.5q� �� Orientation$ Vertical=�.+�9�  * &Lan.Control_WorkEdit.HeaderImageSource#���    Style_Expander#���    �� *EditWorkImageSourceViewModel+.���  � RowDefinitions �� $��25 ��� .+��� �� $��Right=�$ 
Horizontal=�.+��� $��20��$��20��$��2q�$	����  Font_Ico#��[ $��12(�  Style_Button_Ico#��[   Lan.Control_WorkEdit.BtnAdd � ToolTip# �  ���� *CmdAddImageSource+�� $��20��$��20��$	��5,2q�$	����#��[ $��16(�$
��Bold"�#��[  " Lan.Control_WorkEdit.BtnRemove# �  ���� *CmdRemoveImageSource+����  �� ElementName$ dgImageSources��$ SelectedValue�&   System.Windows.Controls.DataGrid  -     Name$ dgImageSources��$��1��$	��0.5q� 	 Style_DataGrid#��� 	 ���� *ImageSourceViewModelList+   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   Width5  /System.Windows.Controls.DataGridLengthConverter$	 100  !
 Lan.Control_WorkEdit.TextName   Header# � 
    CellTemplate �� .R��� $��2q� �� Padding$ 2q�$��Center=����� *1/WorkImageSource.ImageSourceInfo.ImageSourceName+ �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=�  $ 60  ' #Lan.Control_WorkEdit.TextCameraMode# �   �� .R��� $��Center=�$��Center=� d� 	IsChecked �� **(WorkImageSource.ImageSourceInfo.IsUseCam+$ TwoWay=�$ PropertyChanged=�  $ *  ' #Lan.Control_WorkEdit.TextFolderPath# �     
IsReadOnly .  �� .R��� $ 2q�$��Center=�$
��Wrap=����� */-WorkImageSource.ImageSourceInfo.ImageFilePath+$ TwoWay=�$ PropertyChanged=� �� */-WorkImageSource.ImageSourceInfo.ImageFilePath+  $	 120  % !Lan.Control_WorkEdit.TextOperator# �   �� .R�� .+��� $	��2,0q�$��Center=�$ Vertical=�.+��� $��2q�$��2q� # Lan.Control_WorkEdit.BtnEditCam# �  ���� *CmdEditTBAcq+.���  �� �� .+��� $	��2,0q�$��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�#���  �� $��2q�$��2q� & "Lan.Control_WorkEdit.BtnSelectPath# �  ���� *CmdOpenImageFile+.���  �� �� .+��� $	��2,0q�$��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�#���  �� $��2q�$��2q� . *Lan.Control_WorkEdit.BtnResetImageSequence# �  ���� *CmdResetImageSeqConfig+.���  �� �� .+��� $	��2,0q�$��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�#���  �� $��2q�$��2q� # Lan.Control_WorkEdit.BtnRunOnce# �  ���� *
CmdRunOnce+.���  �� �� .+��� $	��2,0q�$��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�#���  d�  d�Name$ btnShowImage��$��2q�$��2q� % !Lan.Control_WorkEdit.BtnViewImage# �  ���� *
CmdRunOnce+.���  �� �� .+��� $	��2,0q�$��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�#���  � $	��800��$	��600�� � AllowsTransparency .  $��Bottom=�$��Slide=�  � 	StaysOpen  .  ���� $ btnShowImage��$ 	IsChecked�! � PlacementTarget! �� $ btnShowImage��.��� .+��� 
�������
�������$��2q��� $��5q�$��Uniform=����� *OutputImage+  $ *  ) %Lan.Control_WorkEdit.TextLightControl# �   .  �� .R��� $��2q�$ Vertical=�.+�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� # Lan.Control_WorkEdit.TextEnable#���  �� $��1��$��Center=�$��Center=�
������� �� *64WorkImageSource.ImageSourceInfo.IsLightControlEnbale+$ TwoWay=�$ PropertyChanged=�� $��25��$��2q�$ IP:Port�� �� $
��0.5* ��� .+��� $��Center=� ' #Lan.Control_WorkEdit.TextController#���  �� $��1��$��Center=����� *20WorkImageSource.ImageSourceInfo.ControllerIPPort+$ TwoWay=�$ PropertyChanged=�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� ' #Lan.Control_WorkEdit.TextLightOnCmd#���  �� $��1��$��Center=����� *0.WorkImageSource.ImageSourceInfo.LightOnCommand+$ TwoWay=�$ PropertyChanged=�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� ( $Lan.Control_WorkEdit.TextLightOffCmd#���  �� $��1��$��Center=����� *1/WorkImageSource.ImageSourceInfo.LightOffCommand+$ TwoWay=�$ PropertyChanged=�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� * &Lan.Control_WorkEdit.TextLightProtocol#���  �� $��1��$��Center=�" �� SelectedIndex" �� *31WorkImageSource.ImageSourceInfo.LightEthernetType+$ TwoWay=�$ PropertyChanged=�@��� $	��UDP���� $	��TCP��� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=�   Lan.Control_WorkEdit.TextHex#���  �� $��1��$��Center=�$��Center=�
������� �� *'%WorkImageSource.ImageSourceInfo.IsHex+$ TwoWay=�$ PropertyChanged=�9�  & "Lan.Control_WorkEdit.HeaderDLModel#���  {��� *
IsDLEnable+���� # �� BasedOn## [ " 9� ��  �� *
IsDLEnable+		 false.a��� 
 � ��$ 0���� 
 � ��$ 0q�.��� $��1��  �� *EditDLWorkspaceViewModel+ �� $��25 ��� .+��� �� $��Right=�$ 
Horizontal=�.+��� $��20��$��20��$��2q�$	����#��[ $��12(�#��[ # �  ���� *CmdAddWorkspace+�� $��20��$��20��$	��5,2q�$	����#��[ $��16(�$
��Bold"�#��[ # �  ���� *CmdRemoveWorkspace+���� $ dgDLWorksapces��$ SelectedValue�  -   $ dgDLWorksapces��$��1��$	��0.5q�#��[	 ���� *DLWorkSpaceViewModelList+   $ *  & "Lan.Control_WorkEdit.TextWorkspace# �   �� .R��� $ 2q�$��Center=����� *WorkspaceName+  $ *  ' #Lan.Control_WorkEdit.TextStreamList# �   �� .R��� $��3q�$��0q�$" 0������ *	Streams+9�  $  Lan.Control_WorkEdit.HeaderTCPIP#���  #���  .���   �� *EditTCPViewModel+ �� $��25 ��� .+��� �� $��Right=�$ 
Horizontal=�.+��� $��20��$��20��$��2q�$	����#��[ $��12(�#��[ # �  ���� *CmdAddTCPDevice+�� $��20��$��20��$	��5,2q�$	����#��[ $��16(�$
��Bold"�#��[ # �  ���� *CmdRemoveTCPDevice+���� $ dgTCPDevice��$ SelectedValue�  -   $ dgTCPDevice��$��1��$	��0.5q�#��[	 ���� *WorkTCPItemList+   $	 100 # � 
  �� .R�� .+��� $ 2q�$��Center=����� *'%TCPDevice.TCPDeviceInfo.TCPDeviceName+  $	 100  ! Lan.Control_WorkEdit.TextMode# �   �� .R�� .+��� $ ��Name$$ 	cbTCPType��$��3q�$��Center=�$��0q����� *TCPTypeConstantsBindableList+% �� SelectedValue% �� *TCPDevice.TCPDeviceInfo.Type+$ TwoWay=�$ PropertyChanged=�  $ * &   MinWidth$	& 150Q� ( $Lan.Control_WorkEdit.TextAddressPort# �   �� .R�� .+��� $ 
Horizontal=����� " �� ��  �� *TCPDevice.TCPDeviceInfo.Type+	 	TCPServer.a��� 
 � y�$ Visible=���  �� *TCPDevice.TCPDeviceInfo.Type+	 	TCPClient.a��� 
 � y�$ Hidden=���  �� *TCPDevice.TCPDeviceInfo.Type+   StringIsNullCvt' �� 	Converter#' [  	 True.a��� 
 � y�$ Hidden=�.+��� $��2q�$ 2q�$��Center=�$	��IP:���� $	��100��$��Center=�$��0q����� *LocalIPBindableList+% �� *#!TCPDevice.TCPDeviceInfo.IPAddress+$ TwoWay=�$ PropertyChanged=��� $��2q�$ 2q�$��Center=�$��Port:��%  Xceed.Wpf.Toolkit.IntegerUpDown  $��50��$��2q�$��Center=��  �Xceed.Wpf.Toolkit.Primitives.UpDownBase`1[[System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]](   Maximum$( 100000X�)   Minimum$) 10X�$ 	10-100000��*   Value* �� *TCPDevice.TCPDeviceInfo.Port+$ TwoWay=�$ PropertyChanged=��� $ 
Horizontal=����� " �� ��  �� *TCPDevice.TCPDeviceInfo.Type+	 	TCPClient.a��� 
 � y�$ Visible=���  �� *TCPDevice.TCPDeviceInfo.Type+	 	TCPServer.a��� 
 � y�$ Hidden=���  �� *TCPDevice.TCPDeviceInfo.Type+#' [  	 True.a��� 
 � y�$ Hidden=�.+��� $��2q�$ 2q�$��Center=�$	��IP:���� $	��100��$��2q�$��Center=�.������ $ TwoWay=�$' !TCPDevice.TCPDeviceInfo.IPAddress�$ PropertyChanged=�+ �� ValidationRules+ 5  /VPDLFramework.ValidationModels.IPTextValidation  .���� $��2q�$ 2q�$��Center=�$��Port:��  $��50��$��2q�$��Center=�$( 100000X�$	) 100X�$ 
100-100000��* �� *TCPDevice.TCPDeviceInfo.Port+$ TwoWay=�$ PropertyChanged=�  $	 150 # �   �� .R�� .+��� $ 
Horizontal=�.+��� $��50��$��2q�$��2q����� *CmdOpenOrCloseTCP+���� ## [ " �� ��  �� *TCPDevice.IsTCPOpened+	 True.a��� 
 � �� "! Lan.Control_WorkEdit.TextClose# � ! �� 
 � �� �#1d953f�� 
 � ��
 �������  �� *TCPDevice.IsTCPOpened+		 False.a��� 
 � �� !" Lan.Control_WorkEdit.TextOpen# � " �� $ 
Horizontal=�.+��� $��Center=� %# !Lan.Control_WorkEdit.TextReceived#��� # �� $	��100��$��3q�$��Center=�$��0,0,0,1q�, �� 
IsReadOnly, . ���� *TCPDevice.TCPMsg+9�  '$ #Lan.Control_WorkEdit.HeaderCommCard#��� $ #���    �� *EditCommCardViewModel+.���  �� $��40 ��� .+�� .+��� $ 
Horizontal=�.+��� $ 3q�$��Center=�  % Lan.Control_WorkEdit.TextFFP#��� % �� $ 2q�$��Center=����� *	FfpType+�� $ 3q�$��Center=� #& Lan.Control_WorkEdit.TextScript#��� & �� $	��5,6q�$��2q�#��[ ���� *CmdEditFfpScript+.���  �� �� .+��� $��Right=�$��Center=�#��[ $��16(�$	������ $��1��$	��2,0q�$
��Left=�$��Center=� &' "Lan.Control_WorkEdit.BtnEditScript#��� ' �� $	��5,6q�$��5q�$	����#��[ $��16(� '( #Lan.Control_WorkEdit.BtnResetScript# � ( ���� *CmdResetScriptFile+�� $	��5,6q�$��3q�$��Right=� $)  Lan.Control_WorkEdit.BtnCommTest#��� ) ���� *CmdTestCC24+{��� *IsThirdCard+ * BoolInverseCvt#' [* � $��1��{��� *IsThirdCard+#' [*  �� �� .+�� $��2q� �� $��25 ��� .+��� $��Center=�$��Center=� "+ Lan.Control_WorkEdit.TextInput#��� +   -   $ 	dgIOInput��$��1��$	��0.5q�
�������-   CanUserAddRows- .  #��[	 ���� *
InputItems+   $ 50  $,  Lan.Control_WorkEdit.TextLineNum# � ,  �� .R�� .+��� $ 2q�$��Center=����� *
LineNumber+  $ *  !- Lan.Control_WorkEdit.TextType# � -  �� .R�� .+��� $	��100��$��3q�$
��Left=�$��Center=�$��0q�����  �� $ ExpanderL�$6 0DataContext.InputSignalTypeConstantsBindableList�. �� SelectedItem. �� *
SignalType+$ TwoWay=�$ PropertyChanged=�� $��1��$��2q� �� $��25 ��� .+��� �� $��Center=�$��Center=� #. Lan.Control_WorkEdit.TextOutput#��� .   -   $ 
dgIOOutput��$��1��$	��0.5q�
�������#��[	 ���� *OutputItems+   $ 50 # � ,  �� .R�� .+��� $ 2q�$��Center=����� *
LineNumber+  $	 100 # � -  �� .R��� $��3q�$��Center=�$��0q�����  �� $ ExpanderL�$7 1DataContext.OutputSignalTypeConstantsBindableList�. �� *
SignalType+$ TwoWay=�$ PropertyChanged=�  $ *  '/ #Lan.Control_WorkEdit.TextPulseWidth# � /  �� .R�� .+�  /  Name$/ cbOutputDuration��$��80��$��3,3,25,3q�$��Stretch=�$��Center=�$��1q�* �� *
Duration+$ TwoWay=�$ PropertyChanged=��� $	��3,0q�$��Right=�$��Center=�$��ms��9�  $0  Lan.Control_WorkEdit.HeaderGroup#��� 0 #���  .��� $��1��  �� *EditGroupViewModel+ �� $��25 ��� .+��� �� $��Right=�$ 
Horizontal=�.+��� $��20��$��20��$��2q�$	����#��[ $��12(�#��[ # �  ���� *CmdAddGroup+�� $��20��$��20��$	��5,2q�$	����#��[ $��16(�$
��Bold"�#��[ # �  ���� *CmdRemoveGroup+���� $ dgGroups��$ SelectedValue�  -   $ dgGroups��$��1��$	��0.5q�#��[	 ���� *WorkGroupItemList+   $	 100 # � 
  �� .R��� $ 2q�$��Center=����� *WorkGroup.GroupInfo.GroupName+  $ *  -1 )Lan.Control_WorkEdit.TextStreamCollection# � 1  �� .R��� 
������ $��0q����� *!WorkGroup.GroupInfo.StreamsList+���� .R�� $
��Left=�$��Center=�0 � Orientation$0 
Horizontal=����� " ��.��� 
 � �� �� .R�� $	��3,0q�.+��� $ 
Horizontal=�.+��� #��[ 
�������$	����� 
�������.+��� 
���   �$��1q��� $ 3q�$��Center=�$��Center=�$��10(�����  �� $ ListViewItemL�$ Content�  $ *  "2 Lan.Control_WorkEdit.TextSetup# � 2  �� .R�� .+��� $��2q�$ Vertical=�.+�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� (3 $Lan.Control_WorkEdit.TextAlgorithmTB#��� 3 �� $��1��$	����#��[ #��[ ���� *CmdEditGroupToolBlock+���� $ gridEditWorkStream��$ DataContext�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� *4 &Lan.Control_WorkEdit.TextResultGraphic#��� 4 � $��1��$��1q�.+��� $��Center=�$��0q�$" 0������ *$"ResultGraphicConstantsBindableList+% �� *)'WorkGroup.GroupInfo.ResultGraphicOption+$ TwoWay=�$ PropertyChanged=�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� *5 &Lan.Control_WorkEdit.TextDisplayOption#��� 5 � $��1��$��1q�.+��� $��0,0,35,0q�$��Center=�$��0q����� *#!WorkGroup.ToolBlockRecordsKeyList+% �� *(&WorkGroup.GroupInfo.ToolBlockRecordKey+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetGroupTBRecordKey��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� 1 ��Name$1 btnResetGroupTBRecordKey��$��30��$��Right=�$	����#��[ $��16(�#��[ � $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� (6 $Lan.Control_WorkEdit.TextChartSeries#��� 6   $��1��$	��1,0q�2   DefaultValue$2 0X�$( 10X�$) 0X�* �� *,*WorkGroup.GroupInfo.ResultChartSeriesCount+$ TwoWay=�$ PropertyChanged=�� $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� '7 #Lan.Control_WorkEdit.TextResultSend#��� 7 �� $��1��$��1q�$ 
Horizontal=�.+��� $��50��$��Center=�$��0q����� *%#ResultSendTypeConstantsBindableList+% �� *&$WorkGroup.GroupInfo.ResultSenderType+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnReset��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $1 btnReset��$��30��$	��5,0q�$	����#��[ $��16(�#��[ d� $ btn_GroupResultSendDetail��$	��5,0q�$��2q�
������  "8 Lan.Control_WorkEdit.BtnDetail#��� 8 � $��25��$��2q� �� $
��0.5* ��� .+��� $��Center=� -9 )Lan.Control_WorkEdit.TextVisibleInRuntime#��� 9 �� $��1��$��Center=� �� *(&WorkGroup.GroupInfo.IsVisibleInRuntime+$ TwoWay=�$ PropertyChanged=�� $	��200��$��80�� .  $��Bottom=�$��Slide=�  .  ���� $ btn_GroupResultSendDetail��$ 	IsChecked�! �� $ btn_GroupResultSendDetail��.��� .+��� 
�������
���   �$��1q�� $��1q�.+��� $��Center=�$��Center=�$ Vertical=�.+��� $
��0,10q�$ 
Horizontal=�.+��� $��2q�$ 2q�$��Center=�
���   � $:  Lan.Control_WorkEdit.TextTCPSend#��� : �� $	��100��$��0q�3 �� DisplayMemberPath$+3 %TCPDevice.TCPDeviceInfo.TCPDeviceName��4 �� SelectedValuePath$+4 %TCPDevice.TCPDeviceInfo.TCPDeviceName������ $ dgTCPDevice��$ ItemsSource�% �� *#!WorkGroup.GroupInfo.TCPSenderName+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� *&$WorkGroup.GroupInfo.ResultSenderType+	 IO.a��� 
 � {� .  �� $ 
Horizontal=�.+��� $��2q�$ 2q�$��Center=�
���   �#��� , �� $	��100��$��0q�$" 0������ *IOOutputConstantsBindableList+���� " �� ��  �� *&$WorkGroup.GroupInfo.ResultSenderType+	 TCP.a��� 
 � {� .    $ 50  %; !Lan.Control_WorkEdit.TextValidity# � ;  �� .R��� $��Center=�$��Center=�#��[ $��16(� (< $Lan.Control_WorkEdit.TextValidityTip# � < ���� " �� ��  �� *WorkGroup.GroupInfo.IsValid+$ OneWay=�	 true.a��� 
 � ��$	 ���� 
 � �� �#65c294��  �� *WorkGroup.GroupInfo.IsValid+$ OneWay=�		 false.a��� 
 � ��$	 ���� 
 � �� �#ffd4009�  )= %Lan.Control_WorkEdit.HeaderWorkStream#��� = #���  .��� -   5 �Name$5 gridEditWorkStream��  �� *EditStreamsViewModel+ �� $��30 ��� .+��� �� $	��0,2q�$
��Left=�$ 
Horizontal=����� " �� ��  �� $ dgWorkStreams��$ SelectedIndex�	 -1.a��� 
 � {� .  .+��� $	��2,0q�$��2q�#��[ ���� *CmdRunWorkStreamItem+���� $ dgWorkStreams��$ SelectedValue�.���  �� �� .+��� $��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�#���  d� -   $ btn_EditRecipes��$��2q�.���  �� �� .+��� $��2q�$��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=� #> Lan.Control_WorkEdit.BtnRecipes#��� > �� $	��2,0q�$��2q�#��[ ���� *CmdCopyWorkStreamItem+���� $ dgWorkStreams��$ SelectedValue�.���  �� �� .+��� $��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�  ? Lan.Control_WorkEdit.BtnCopy#��� ? �� $��2q�#��[ ���� *CmdWorkStreamItemUp+���� $ dgWorkStreams��$ SelectedValue�.���  �� �� .+��� $��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=� @ Lan.Control_WorkEdit.BtnUp#��� @ �� $	��2,0q�$��2q�#��[ ���� *CmdWorkStreamItemDown+���� $ dgWorkStreams��$ SelectedValue�.���  �� �� .+��� $��Right=�$��Center=�#��[ $��16(�$	������ $��1��$
��Left=�$��Center=�  A Lan.Control_WorkEdit.BtnDown#��� A �� $��Right=�$ 
Horizontal=�.+��� $��20��$��20��$��2q�$	����#��[ $��12(�#��[ # �  ���� *CmdAddWorkStreamItem+�� $��20��$��20��$	��5,2q�$	����#��[ $��16(�$
��Bold"�#��[ # �  ���� *CmdRemoveWorkStreamItem+���� $ dgWorkStreams��$ SelectedValue��� -	   6 ��Name$6 dgWorkStreams��$��1��#��[  
�������$��0q����� *WorkStreamItemViewModelList+" �� *SelectedWorkStreamIndex+$ TwoWay=�$ PropertyChanged=����� " �� P� 7 P� Property7 �@��
IsSelected8 P� Value8 . . ��� 
 � �� B Color_Selected# [B .��� 
 � ��$ 0q��� 
 � �� �� " ��.R�� #��z�� �� $��30 ��� .+��� ._��� $ 
Horizontal=�.+��� $ 3q�$��Center=�$��Center=�#��[ $��16(�
�������$	����y���  �� $ ListViewItemL�$ 
IsSelected� $C  BoolToVisiblityDefaultVisibleCvt#' [C 9� $��1��$	��2,1q�
�������$��2q�$��Microsoft YaHei)�#��[ ���� *.,WorkStream.ResultViewModel.StreamOrGroupName+.��� $��15,0,0,0q�.+��� $ Vertical=�.+�� $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=� ,D (Lan.Control_WorkEdit.TextInternalTrigger#��� D �� $��1��$
��Left=�$ 
Horizontal=�.+��� $��Center=�$��Center=�#���  
������� �� *31WorkStream.WorkStreamInfo.IsEnableInternalTrigger+$ TwoWay=�$ PropertyChanged=��� $
��10,0q�$��Center=�$��Center=� &E "Lan.Control_WorkEdit.TextAutoStart#��� E 
������� �� *20WorkStream.WorkStreamInfo.IsInternalTriggerBegin+$ TwoWay=�$ PropertyChanged=�� $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=� &F "Lan.Control_WorkEdit.TextAsyncMode#��� F �� $��1��$ 
Horizontal=�.+��� 9 ��Name$9 cbAsync��$
��Left=�$��Center=� �� *'%WorkStream.WorkStreamInfo.IsAsyncMode+$ TwoWay=�$ PropertyChanged=��� $
��10,0q�$
��Left=�$��Center=� (G $Lan.Control_WorkEdit.TextMultiThread#��� G 
������� �� */-WorkStream.WorkStreamInfo.IsEnableMultiThread+$ TwoWay=�$ PropertyChanged=�y���  �� $ local:Control_WorkEditL�$ DataContext.IsDLEnable� #H BoolToVisiblityDefaultHiddenCvt#' [H   $��50��$	��5,2q�
���   �$��0q�$2 2X�$( 10X�$) 2X�y���  �� $ local:Control_WorkEditL�$ DataContext.IsDLEnable�#' [H * �� *'%WorkStream.WorkStreamInfo.ThreadCount+$ TwoWay=�$ PropertyChanged=�� $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�#��� 0 �� $��1��$��0,2,5,2q�._�� $	��0.5q�.+��� $��0,0,50,0q�$��Center=�$��0q�$#3 WorkGroup.GroupInfo.GroupName��$#4 WorkGroup.GroupInfo.GroupName������ $ dgGroups��$ ItemsSource�% �� *%#WorkStream.WorkStreamInfo.GroupName+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetGroup��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1��.	  (System.Windows.Interactivity.Interaction: 	  Triggers: /
  )System.Windows.Interactivity.EventTrigger
  ; 
  	EventName$; SelectionChanged��.  (System.Windows.Interactivity.TriggerBase<   Actions.< 6  0System.Windows.Interactivity.InvokeCommandAction  =   Command= �� *CmdSelectGroup+�� $1 btnResetGroup��$��30��$��Right=�$	����#��[ $��16(�#��[ � #��[  #���   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=� %I !Lan.Control_WorkEdit.TextOnlyVPro#��� I �� $9 chb_OnlyVpro��$��1��$��Center=� �� *&$WorkStream.WorkStreamInfo.IsOnlyVpro+$ TwoWay=�$ PropertyChanged=�� $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�#���  �� $��1��$��0,2,5,2q�._�� $	��0.5q�.+��� $$ cbImageSource��$��0,0,50,0q�$��Center=�$��0q�$53 /WorkImageSource.ImageSourceInfo.ImageSourceName��$54 /WorkImageSource.ImageSourceInfo.ImageSourceName������ $ dgImageSources��$ ItemsSource�% �� *+)WorkStream.WorkStreamInfo.ImageSourceName+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetImageSource��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $1 btnResetImageSource��$��30��$��Right=�$	����#��[ $��16(�#��[ � #��[  #���  {��� $ chb_OnlyVpro��$ 	IsChecked�#' [*  �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=� $J  Lan.Control_WorkEdit.TextInputTB#��� J �� $��1��$��0,2,5,2q�._��� $��1��$��Stretch=�$	����#��[ #��[ ���� *CmdEditInputToolBlock+����  �� $ ListViewItemL�$ DataContext�� #��[  #���  {��� $ chb_OnlyVpro��$ 	IsChecked�#' [*  �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�#���  �� $��1��$��0,2,5,2q�._�� $	��0.5q�.+��� $��1��$	��0.5q�$ 
Horizontal=�.+��� $9 chbAdvancedDL��$	��5,0q�$��Center=� )K %Lan.Control_WorkEdit.TextAdvancedMode#��� K 
������� �� *0.WorkStream.WorkStreamInfo.IsUseAdvancedDLModel+$ TwoWay=�$ PropertyChanged=�� .+��� $��0,0,50,0q�$ 
Horizontal=�y��� $ chbAdvancedDL��$ 	IsChecked�#' [H .+��� $��ws��$	��150��$��Center=�$��0q�$3 WorkspaceName��$4 WorkspaceName������ $ dgDLWorksapces��$ ItemsSource�% �� *)'WorkStream.WorkStreamInfo.WorkspaceName+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetDLWorkspace��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $	��100��$	��2,0q�$��Center=�$��0q����� $ ws��$ SelectedItem.Streams�% �� */-WorkStream.WorkStreamInfo.WorkspaceStreamName+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetDLWorkspace��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $��30��$	����#��[ #��[ ���� *#!CmdAddDLResultTerminalsDLOutputTB+����  �� $ ListViewItemL�$ DataContext��� $��0,0,30,0q�$��Stretch=�$	����#��[ #��[ ���� *CmdEditAdvancedDLModel+y��� $ chbAdvancedDL��$ 	IsChecked�#' [C ��`�  (L $MultiBindingConverter_ObjectArrayCvt> `� 	Converter#> [L .1��� $ DataContext� �� $ ListViewItemL��� $ dgDLWorksapces��$ ItemsSource��� $1 btnResetDLWorkspace��$��1��$��30��$��Right=�$	����#��[ $��16(�#��[ � $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=����� " �� ��  ��  �� $ local:Control_WorkEditL�$ DataContext.IsDLEnable�	 true.a��� 
 � �� %M !Lan.Control_WorkEdit.TextOutputTB# � M ��  ��  �� $ local:Control_WorkEditL�$ DataContext.IsDLEnable�		 false.a��� 
 � ��# � 3 �� $��1��$��0,2,5,2q�._��� $��1��$��30��$��Stretch=�$	����#��[ #��[ ���� *CmdEditOutputToolBlock+����  �� $ ListViewItemL�$ DataContext�� #��[  #���   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�  N Lan.Control_WorkEdit.TextGPU#��� N �� $��1��$��0,2,5,2q�._��� $��1��$	��100��$	��0.5q�$��Center=�$��0q����� *	GPUList+" �� *$"WorkStream.WorkStreamInfo.GpuIndex+$ TwoWay=�$ PropertyChanged=�� $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�#��� 4 �� $��1��$��0,2,5,2q�._�� $	��0.5q�.+��� $��0,0,50,0q�$��Center=�$��0q�$" 0������ *$"ResultGraphicConstantsBindableList+% �� */-WorkStream.WorkStreamInfo.ResultGraphicOption+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetResultGraphic��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $1 btnResetResultGraphic��$��30��$��Right=�$	����#��[ $��16(�#��[ � $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�#��� 5 � $��1��$��0.5,2.5,5.5,2.5q�.+��� $	��340��$��0,0,50,0q�$
��Left=�$��Center=�$��0q����� *$"WorkStream.ToolBlockRecordsKeyList+% �� *.,WorkStream.WorkStreamInfo.ToolBlockRecordKey+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetTBRecordKey��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $��Right=�$ 
Horizontal=�.+��� $
��10,0q�$��Center=�$��Center=�$��3D��
������� �� *'%WorkStream.WorkStreamInfo.IsDisplay3D+$ TwoWay=�$ PropertyChanged=��� $1 btnResetTBRecordKey��$��30��$	����#��[ $��16(�#��[ � $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=� (O $Lan.Control_WorkEdit.TextImageRecord#��� O �� $��1��$��0,2,5,2q�._�� $	��0.5q�.+��� $��	0,0,200,0q�$ 
Horizontal=�.+��� $	��100��$��Center=�$��0q����� *" ImageRecordConstantsBindableList+% �� *-+WorkStream.WorkStreamInfo.ImageRecordOption+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetImageRecord��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $��50��$
��10,0q�$��Center=�$��0q����� *)'OriginalImageTypeContstantsBindableList+% �� *53WorkStream.WorkStreamInfo.OriginalImageTypeConstant+$ TwoWay=�$ PropertyChanged=��� $��Right=�$ 
Horizontal=�.+��� $��2q�$ 2q�$��Center=� &P "Lan.Control_WorkEdit.TextCondition#��� P �� $��50��$��Center=�$��0q����� *+)ImageRecordConditionConstantsBindableList+% �� *64WorkStream.WorkStreamInfo.ImageRecordConditionOption+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetImageRecord��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $1 btnResetImageRecord��$��30��$��5,0,0,0q�$	����#��[ $��16(�#��[ � $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=� $Q  Lan.Control_WorkEdit.TextTrigger#��� Q �� $��1��$��0,2,5,2q�._�� $	��0.5q�.+��� $��	0,0,200,0q�$��Center=�$��0q����� *" TriggerTypeConstantsBindableList+% �� *'%WorkStream.WorkStreamInfo.TriggerType+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetTrigger��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $��Right=�$ 
Horizontal=�.+�d� $ btn_TriggerDetail��$	��5,0q�$��2q�
������ #��� 8 �� $1 btnResetTrigger��$��30��$	����#��[ $��16(�#��[ � $	��300��$	��120�� .  $��Bottom=�$��Slide=�  .  ���� $ btn_TriggerDetail��$ 	IsChecked�! �� $ btn_TriggerDetail��.��� .+��� 
�������
���   �$��1q�� $��1q�.+��� $��Center=�$��Center=�$ Vertical=�.+��� $
��0,10q�$ 
Horizontal=�.+��� $��Center=�
���   �$
��TCP:���� $��80��$	��5,0q�$��Center=�$��0q�$+3 %TCPDevice.TCPDeviceInfo.TCPDeviceName��
���   �$+4 %TCPDevice.TCPDeviceInfo.TCPDeviceName������ $ dgTCPDevice��$ ItemsSource�% �� *+)WorkStream.WorkStreamInfo.TCPReceiverName+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetTrigger��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1����  �� *'%WorkStream.WorkStreamInfo.TriggerType+	 IO.a��� 
 � {� .  ��  �� *'%WorkStream.WorkStreamInfo.TriggerType+	 FFP.a��� 
 � {� .  �� $ 
Horizontal=�.+��� $��Center=�
���   �$��IO:  ���� $��80��$	��6,0q�$��Center=�$��0q����� *IOInputConstantsBindableList+% �� *+)WorkStream.WorkStreamInfo.IOInputConstant+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetTrigger��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1����  �� *'%WorkStream.WorkStreamInfo.TriggerType+	 TCP.a��� 
 � {� .  ��  �� *'%WorkStream.WorkStreamInfo.TriggerType+	 FFP.a��� 
 � {� .  �� $
��0,10q�$ 
Horizontal=�.+��� $��Center=�
���   �$
��FFP:���� $��80��$	��6,0q�$��Center=�$��0q����� * SoftEventConstantsBindableList+% �� *-+WorkStream.WorkStreamInfo.SoftEventConstant+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetTrigger��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1����  �� *'%WorkStream.WorkStreamInfo.TriggerType+	 TCP.a��� 
 � {� .  ��  �� *'%WorkStream.WorkStreamInfo.TriggerType+	 IO.a��� 
 � {� .  � $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�#��� 7 �� $��1��$��0,2,5,2q�._�� $	��0.5q�.+��� $��	0,0,200,0q�$��Center=�$��0q����� *%#ResultSendTypeConstantsBindableList+% �� *,*WorkStream.WorkStreamInfo.ResultSenderType+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetResultSend��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1���� $��Right=�$ 
Horizontal=�.+�d� $ btn_ResultSendDetail��$	��5,0q�$��2q�
������ #��� 8 �� $1 btnResetResultSend��$��30��$	����#��[ $��16(�#��[ � $	��200��$	��100�� .  $��Bottom=�$��Slide=�  .  ���� $ btn_ResultSendDetail��$ 	IsChecked�! �� $ btn_ResultSendDetail��.��� .+��� 
�������
���   �$��1q�� $��1q�.+��� $��Center=�$��Center=�$ Vertical=�.+��� $
��0,10q�$ 
Horizontal=�.+��� $��2q�$ 2q�$��Center=�
���   �#��� : �� $��50��$��Center=�$��0q�$+3 %TCPDevice.TCPDeviceInfo.TCPDeviceName��
���   �$+4 %TCPDevice.TCPDeviceInfo.TCPDeviceName������ $ dgTCPDevice��$ ItemsSource�% �� *)'WorkStream.WorkStreamInfo.TCPSenderName+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetResultSend��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1����  �� *,*WorkStream.WorkStreamInfo.ResultSenderType+	 IO.a��� 
 � {� .  �� $ 
Horizontal=�.+��� $��2q�$ 2q�$��Center=�
���   �#��� , �� $��50��$��Center=�$��0q����� *IOOutputConstantsBindableList+% �� *,*WorkStream.WorkStreamInfo.IOOutputConstant+$ TwoWay=�$ PropertyChanged=����� " �� ��  �� $ btnResetResultSend��$ 	IsPressed�	 True.a��� 
 �@��SelectedIndex$ -1����  �� *,*WorkStream.WorkStreamInfo.ResultSenderType+	 TCP.a��� 
 � {� .  � $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=�#��� 6 �� $��1��$��0,2,5,2q�._�  $��1��$	��0.5q�
���   �$��0q�$2 0X�$( 10X�$) 0X�* �� *20WorkStream.WorkStreamInfo.ResultChartSeriesCount+$ TwoWay=�$ PropertyChanged=�� $��30��$	��0,2q�#��[   �� $
��0.3* ��� .+��� $
��10,0q�$
��Left=�$��Center=� $R  Lan.Control_WorkEdit.TextDisable#��� R �� $��1��$
��Left=�$��Center=� �� *+)WorkStream.WorkStreamInfo.IsStreamDisable+$ TwoWay=�$ PropertyChanged=��� $	��0,2q� 'S #Lan.Control_WorkEdit.BtnImageReplay#��� S $
��Bold"����� $ gridEditWorkStream��$) #DataContext.CmdExpandWorkStreamItem���`� #> [L .1��� $ DataContext� �� $ ListViewItemL��� $ cbImageSource��$ SelectedItem�� $	��600��$	��600�� . $��Bottom=�$��Slide=�  .  ���� $ btn_EditRecipes��$ 	IsChecked�! �� $ btn_EditRecipes��? J� Effect? 3  -System.Windows.Media.Effects.DropShadowEffect  @   
BlurRadius$@ 15Q�A   	Direction$	A -90Q�B   Opacity$	B 0.7Q�C   ShadowDepth$C 0Q�.��� $��10q�.+��� #���   
�������$��1q�� $��1q� �� $��30 ��� .+��� $��Right=�$ 
Horizontal=�.+��� $��20��$��20��$��2q�$	����#��[ $��12(�#��[  .T *Lan.Control_WorkStreamRecipes.BtnAddRecipe# � T ���� $ dgWorkStreams��$) #SelectedItem.CmdAddWorkStreamRecipe��� $��20��$��20��$	��5,2q�$	����#��[ $��16(�$
��Bold"�#��[  1U -Lan.Control_WorkStreamRecipes.BtnRemoveRecipe# � U ���� $ dgWorkStreams��$, &SelectedItem.CmdRemoveWorkStreamRecipe����� $ listRecipes��$ SelectedItem��� -
   $6 listRecipes��$��1��#���   $��0,2,0,0q�$��Disabled=����� $ dgWorkStreams��$% SelectedItem.WorkStream.Recipes����� " ��.��� 
 � ��$	 0,2q��� 
 � �� �� " ��.R��� #��z��
�������$��0,0,0,2q�._��  �� $��50 ��� .+��� $ Vertical=�.+�d� $��30��$��30��$��2q�$��Center=�$	����#��[ $��14(�$
��Bold"� EV ALan.Control_WorkStreamRecipes.TextDesableChangingDiscriptionLabel# � V  �� *DisableLabelModify+$ TwoWay=�$ PropertyChanged=����� " d� P� 7 �@d�	IsChecked$
8 TrueY�. ��� 
 � ��
 ������� $��30��$��30��$��2q�$��Center=�$	����#��[ $��12(�$
��Bold"�#��[  1W -Lan.Control_WorkStreamRecipes.BtnImportRecipe# � W ���� $ dgWorkStreams��$* $SelectedItem.CmdLoadWorkStreamRecipe�����  �� $ ListViewItemL�$ DataContext�9� $��1��$��0q�#��[ ���� *
RecipeName+.��� .+��� $��2q�
�������$	��0.5q�D �� CornerRadius$D 0��._��  �� $��30 ��� .+��� ._��� $ 3q�$��Center=�$��Center=� .X *Lan.Control_WorkStreamRecipes.TextParaList#��� X   $��1��$	��200��$��0q�$��0q�#��[	 ���� *Values+   $	 150 # � 
  �� .R��� $ 3q�$��Center=�$
��Bold"����� *Key+  $ * # � -  �� .R��� $ 3q�$��Center=����� *Type+ �� *Type+  $ *  +Y 'Lan.Control_WorkStreamRecipes.TextValue# � Y  �� .R��� $ 3q�$��Center=����� *Value+ �� *Value+  $ *  6Z 2Lan.Control_WorkStreamRecipes.TextDescriptionLabel# � Z  �� .R��� $��3q�$��3q�$��Center=�
�������$
��Bold"�, ��  �� $ ExpanderL�$$ DataContext.DisableLabelModify����� *Label+$ TwoWay=�$ PropertyChanged=� �� *Label+ P� 7 �@J�	IsFocused8 . . ��� 
 �@��
IsSelected . P� 7 �@��
IsSelected8 . . ��� 
 � ��# [B 