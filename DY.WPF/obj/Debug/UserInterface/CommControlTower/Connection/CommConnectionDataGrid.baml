   M S B A M L   `   `   `  ����
  DY.WPF#    DY.WPF.CommConnectionDataGrid   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      -   5          >�Name$
  this��5	      .��5
      &   System.Windows.Controls.DataGrid  -   5          Name$ 	NDataGrid��5
         AutoGenerateColumns .     SelectionUnit$ FullRow=�5         GridLinesVisibility$	 All=�5         CanUserAddRows .     HeadersVisibility$	 All=�5      $��14(�5   A   ��5      ��  �� Path$ Items� �� ElementName$
 this��	 � ContextMenu	 5   
   �� 5      .@�5      m� -   5        
 m�Name$
 NMI_Add��5      $��Add Connection��6,   6D   m� 5      -   5        $
 NMI_Connect��5      $��Connect��60   6A   m� 5      -   5        $
 NMI_Disconnect��5      $��
Disconnect��63   6G   m� 5      -   5        $
 
NMI_Remove��5      $��Remove��6/   6?   5       � 	Resources 5   
      TextAlignmentCenter �� 
TargetType �� Property �� Value��%V   &	        �� 5      " ��60   .�5      ��  � ��$ Center=�68   5       5!         Columns 5#   
   4  .System.Windows.Controls.DataGridTemplateColumn  5$      ,  &System.Windows.Controls.DataGridColumn   
IsReadOnly .    Width5  /System.Windows.Controls.DataGridLengthConverter$ 34 67      CellTemplate 5%      �� 5&      .R�5'      � $��Center=�6   .+�5(      F� $��20��5+   !   $��20��5,   !    �� Stretch$
 Fill=�5-   !   ��5)   !   �� $ 
ImageColor� �� Mode$ TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=���5*   !   �� $ 	ImageData�$ TwoWay=�$ PropertyChanged=�5.      5/      50      52        54       . $ 50 67      Header$	 Use��6B    55      �� 56      .R�57      ��  ��Name$ NCB_CheckBox��6#   -   5        $��Center=�57   T   $��5,0,0,0q�6q    d� 	IsChecked 6�   �� $ Usable�$ TwoWay=�$ PropertyChanged=�58      5:      0  *System.Windows.Controls.DataGridTextColumn  5<      $ Target��6!   1  +System.Windows.Controls.DataGridBoundColumn   ElementStyle# [  61    . $	 25* 6�      Binding 6w   �� $ Target�$ TwoWay=�$ PropertyChanged=�6�     5=      $
 Type��6!   # [  6/    . $	 15* 6�    6v   �� $ CommType�$ TwoWay=�$ PropertyChanged=�6�     5>      $ Summary��6!   # [  62    . $	 30* 6�    6y   �� $ Summary�$ TwoWay=�$ PropertyChanged=�6�     5@      $ Comment��6!   # [  5A   !   $	 30* 5C   !    5B   !   �� $ Comment�$ TwoWay=�$ PropertyChanged=�5C   !   5E      5F      