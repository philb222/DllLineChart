   M S B A M L   `   `   `  ����R  NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35L5clr-namespace:System.Diagnostics;assembly=WindowsBaseSystem.Diagnostics  5      G CDllLineChart, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:DllLineChartDllLineChart 5         DllLineChart.LineChart   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      Ddiag5clr-namespace:System.Diagnostics;assembly=WindowsBase     5      *localclr-namespace:DllLineChart     5      Hmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml      5      -   5          >�Name$  ControlArea��5      $	��410��5      $	��590��6    � 	Resources 5
       �� MergedDictionaries �� Source�� 5       5      �� 5      $ Dictionary1.xaml?�6   %    5      .��5      � -   5         �Name$ MainGrid��5         Background1#��[  6    � RowDefinitions 5      �� 5      $��28 �6   �� 5      -   5         ��Name$ GridRow1��5      $��* �6%   �� 5      -   5        $ GridRow2��5      $��28 �6%    � ColumnDefinitions 5      �� 5      $��18 �6   �� 5      $��30 �6   �� 5      $��* �6   �� 5      $��28 �6   z�5#      �� 5$       �� ScaleX 6   �� 	 �� RelativeSource	 �� *FindAncestor+
 �� AncestorType  DllLineChart.LineChart"
   �� Path$ CcScale� �� Mode$ OneWay=� �� ScaleY 5&      �� 	 �� *FindAncestor+"
  $ CcScale�$ OneWay=�.+�5+      �� -   5         ��Name$ labChartName��5+   
     LabelStyle2#��[ 6    $��0��5,      $��2��6     Foreground2#��[ 5-      $��0 8 0 0q�5.      �� 51      -   5         ��Name$ CanvasScroller��51      $��2��6)   $��1��69     Scroller#��[ 6F   $��0q�52      $��0q�6   ! �� IsDeferredScrollingEnabled . $
��Auto=�54      $��Disabled=�6<   $
��Auto��55      ��56      �� 	 �� *FindAncestor+"
  $ ScrollViewerWidth�$ OneWay=�.��59      �� .+�5;      �� -   5         ��Name$ 
MainBorder��5;       �� Padding$ 5q�5<        Background2#��[ 5=      
����  �$��2q�5>      ._�5@      �� -   5         ��Name$ 
MainCanvas��5@      }�. ��5A      �� 	 �� *FindAncestor+"
  $ ChartHeight�$ OneWay=���5C      �� 	 �� *FindAncestor+"
  $ 
ChartWidth�$ OneWay=�.+�5F      �� -	   5         ��Name$ txtError��5F      $
��Wrap=�6$    J� Opacity$ 0Q�5G      $	��200��6   #��[ 5I      #��[ 5J      ��5H      ��  �� ElementName$ CanvasScroller��$ ViewportWidth�$ OneWay=�5J      5K      5L      �� 5O      -
   5        $ CanvasXaxis��5O      }�.  #��[  5P      $��20��5Q      ��5R      �� 	 �� *FindAncestor+"
  $ 
ChartWidth�$ OneWay=�5T      5U      �� 5X      -   5        $ labXaxisDescription��5X   
   #��[ 6'   $��2��5Y      $��2��6   $��Bottom=�5Z      #��[ 5[      $��0 -2 0 0q�5\      �� 5_      -   5        $ labYaxis��5_   
   #��[ 6   $��2��5`      $��0��6   $��4��6%   #��[ 5a      $��Values��5b      $
��Left=�5c      ��5d      �� $ ControlArea��$ Height�$ OneWay=�z�5e      �� 5f       �� Angle$	 -90Q�6    �� CenterX$ 9Q�6"    �� CenterY$ 9Q�6.   5h      �� 5k      -   5        $ 
YaxisPanel��5k      $��1��6   $��1��6/   }�.  5l      5n      5p      