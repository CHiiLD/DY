   M S B A M L   `   `   `  ����H  DMahApps.Metro, Version=1.1.3.0, Culture=neutral, PublicKeyToken=nullV;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.MetroMahApps.Metro.Controls  5      R NOxyPlot.Wpf, Version=0.0.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0>.clr-namespace:OxyPlot.Wpf;assembly=OxyPlot.WpfOxyPlot.Wpf 5      A =DY.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:DY.WPFDY.WPF 5	      %   DY.WPF.COxyRectangleGraphType01   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Kmah;clr-namespace:MahApps.Metro.Controls;assembly=MahApps.Metro     5      >oxy.clr-namespace:OxyPlot.Wpf;assembly=OxyPlot.Wpf     5      &localclr-namespace:DY.WPF     5	      -   5          >�Name$
  this��5
      .��5      �  � ColumnDefinitions 5   
   �� 5      $��2 �6   �� 5      $��37 �6   �� 5      $
��155* �6   �� 5      $��7 �6   �� 5      $��28 �6    � RowDefinitions 5   
   �� 5      $��28 �6   �� 5      $��8 �6   �� 5      $
��225* �6   �� 5      $��33 �6   �� 5      $��2 �6   .+�5   
   (   "MahApps.Metro.Controls.RangeSlider  -   5          Name$ NRangeX��5      $��2��6+   $��0��6;   $��1��6H      TickPlacement$ BottomRight=�5         TickFrequency$ 2Q�5         MinRangeWidth$ 0Q�5         AutoToolTipPlacement$ TopLeft=�5          AutoToolTipPrecision$ 0��5!      	 � Maximum	 5"      �� 
 �� Path$
 PlotMaximumX� �� ElementName$
 this�� �� Mode$ TwoWay=� � Minimum 5#      �� $
 PlotMinimumX�$
 this��$ TwoWay=�   
UpperValue 5$      �� $
 RectMaximunX�$
 this��$ TwoWay=�   
LowerValue 5%      �� $
 RectMinimunX�$
 this��$ TwoWay=�  5'   
   -   5        $ NRangeY��5'      $��4��6+   $��2��6;      Orientation$ Vertical=�6H   $ TopLeft=�5(      $ 4Q�5)      $ 0Q�5*      $ BottomRight=�5+      $ 0��5,      	 5-      �� $
 PlotMaximumY�$
 this��$ TwoWay=� 5.      �� $
 PlotMinimumY�$
 this��$ TwoWay=� 5/      �� $
 RectMaximunY�$
 this��$ TwoWay=� 50      �� $
 RectMinimunY�$
 this��$ TwoWay=�  OxyPlot.Wpf.Plot  53   
   -   5          Name$ NPlot��53      $��1��6"   $��1��62   $��3��6?   $��3��6S      
BlackColor   	TextColor# �   54        WindowBackgroundBrush#���  55      #���  56         PlotAreaBorderColor# �   57         Axes 59        OxyPlot.Wpf.LinearAxis  5:      -   5          Name$ NLinearAxisY��5:   !     OxyPlot.Wpf.Axis   Position$
 Left=�67      MajorGridlineStyle$ Solid=�5;   !      MinorGridlineStyle$	 Dot=�6<      TicklineColor# �   5>   !    	 Gray7   MajorGridlineColor# �  5?   !      MinorGridlineColor# �  5@   !      Maximum 5<   !   �� $
 PlotMaximumY�$
 this��$ TwoWay=�   Minimum 5=   !   �� $
 PlotMinimumY�$
 this��$ TwoWay=�5@   !      Annotations 5D      %  OxyPlot.Wpf.RectangleAnnotation  5E      -   5          Name$ 
NRectangle��5E   *     AccentColor4!  OxyPlot.Wpf.ShapeAnnotation    Fill#  �  5F   *     OxyPlot.Wpf.Annotation!   Layer$! BelowSeries=�5K   *   "   MaximumX" 5G   *   �� $
 RectMaximunX�$
 this��$ TwoWay=�#   MaximumY# 5H   *   �� $
 RectMaximunY�$
 this��$ TwoWay=�$   MinimumX$ 5I   *   �� $
 RectMinimunX�$
 this��$ TwoWay=�%   MinimumY% 5J   *   �� $
 RectMinimunY�$
 this��$ TwoWay=�5K   *   5M      5N      5O      