   M S B A M L   `   `   `  ���� 
  DY.WPF    DY.WPF.CommDataGrid   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     -     >�Name$
  this��.��&   System.Windows.Controls.DataGrid  -     Name$ 	NDataGrid��   AutoGenerateColumns .     SelectionUnit$ FullRow=�   GridLinesVisibility$	 All=�   CanUserAddRows .     HeadersVisibility$	 All=�$��14(�����  �� Path$ Items� �� ElementName$
 this��	 � ContextMenu	 �� .@�m� -   
 m�Name$
 NMI_Add��$��Add Connection��m� -   $
 NMI_Connect��$��Connect��m� -   $
 NMI_Disconnect��$��
Disconnect��m� -   $
 
NMI_Remove��$��Remove�� � 	Resources    TextAlignmentCenter �� 
TargetType �� Property �� Value��%1   &	        �� " ��.���  � ��$ Center=�   Columns 4  .System.Windows.Controls.DataGridTemplateColumn  ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ 34    CellTemplate �� .R�� $��Center=�.+�F� $��20��$��20�� �� Stretch$
 Fill=����� $ 
ImageColor� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=����� $ 	ImageData�$ TwoWay=�$ PropertyChanged=�   . $ 50    Header$	 Use�� �� .R���  ��Name$ NCB_CheckBox��-   $��Center=�$��5,0,0,0q� d� 	IsChecked �� $ Usable�$ TwoWay=�$ PropertyChanged=�0  *System.Windows.Controls.DataGridTextColumn  $ Target��1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [   . $	 25*    Binding �� $ Target�$ TwoWay=�$ PropertyChanged=�  $
 Type��# [   . $	 15*  �� $ CommType�$ TwoWay=�$ PropertyChanged=�  $ Summary��# [   . $	 30*  �� $ Summary�$ TwoWay=�$ PropertyChanged=�  $ Comment��# [  $	 30*  �� $ Comment�$ TwoWay=�$ PropertyChanged=�