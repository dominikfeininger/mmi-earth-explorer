   M S B A M L   `   `   `  ����  SkeletonBasics-WPF.    (Microsoft.mmi.Kinect.Explorer.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          � Title$  Earth Explorer��5   	   $
��1000��6    $
��1800��6.    � 	Resources 5         MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush �� 
TargetType �� Property �� Value  SquareCheckBox �� 
TargetType �� Orientation �Name	 ��Name
 ��Name �� TextAlignment �� Triggers P� Property P� Value �� 
TargetName��%1  &	        &	 #     &	 F     '�� i     &	 �     7  7  �� 5   
   $ 	#ff6e6e6e��62   �� 5   
   $ 	#ff52318f��64   �� 5	   
   $ 	#ff00BCF2��62   �� 5
   
   " ��6   .�5      ��  �@J�SnapsToDevicePixels . 64   5      �� 5   
   " ��6   .�5      ��  � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $ 
Horizontal=�6)   
������ .+�5   "   � $ SquareCheckBoxChecked��6'   .+�5   &   �� $	 CheckedNormal��6,   $��Images\CheckedNormal.png��6C   $
��None=�6e   $��Center=�6t   �� 5   &   $	 CheckedHover��6,   $��Images\CheckedHover.png��6B   $
��None=�6c   $��Center=�6r   $y�	Collapsed=�6�   5   #   � 5   "   $ SquareCheckBoxUnchecked��6'   $y�	Collapsed=�6H   .+�5   &   �� $	 UncheckedNormal��6,   $ ��Images\UncheckedNormal.png��6E   $
��None=�6i   $��Center=�6x   �� 5   &   $	 UncheckedHover��6,   $��Images\UncheckedHover.png��6D   $
��None=�6g   $��Center=�6v   $y�	Collapsed=�6�   5   #   �� 5   "   $
 SquareCheckBoxText��6,   #��z��6H   $
 Left=�6i   $��Center=�6   8��  $��15(�6�   $��9,0,0,0q�6�   5      5       5      P� 5       �@d�	IsChecked$ falseY�6;   . �5    "   �� $ SquareCheckBoxChecked��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5!   "   $ SquareCheckBoxUnchecked��6O    � y�$ Visible=�6?   6O   5"      P� 5#       �@J�IsMouseOver . . �5$   "   �� $ CheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5%   "   $ UncheckedNormal��6Q    � y�$ 	Collapsed=�6?   6Q   �� 5&   "   $ CheckedHover��6O    � y�$ Visible=�6?   6O   �� 5'   "   $ UncheckedHover��6O    � y�$ Visible=�6?   6O   �� 5(   "   8  $ SquareCheckBoxText��6h    � ��5)      5+      5-      5.      5/      .��51      � -   5        $��
layoutGrid��51      $��	10 0 10 0q�6    � RowDefinitions 52   
   �� 53      $
��Auto �6   �� 54      $��* �6   �� 55      $
��Auto �6   �� 56      $
��Auto �6   .+�58   
   ]� $��0��6   $��0 0 0 20q�6!   .+�59      �� $
��Left=�6   $��Images\Logo.png��6*   $
��None=�6C   $��0 10 0 5q�6R   �� 5:      $��Right=�6   $��0 0 -1 0q�6/   $��Bottom=�6A   #��[  6\   $��Segoe UI)�6�   $��18(�6�   Earth Explorer6�   �� 5;      $��1��6   $��Images\Status.png��6$   $
��None=�6?   $��Center=�6N   $��0 0 0 5q�6k   5<      (� 5?   
   $��1��6    (� Stretch$ Uniform=�6   $��Center=�61   .��5@      �� -   5        $��Image��5@      $
��1000��6!   $	��900��6.   5A      �� 5D   
   -   5        $��
MainCanvas��5D      $
��1000��6#   $	��900��61    +� FocusedElement 6=   ��  �� ElementName$ Browser��.+�5E      �� -   5        $��
rectangle1��5E   F    �� Left$ 0��6   
 �� Top$	 492��6(   $	��108��69   $
��1680��6j   ���	#FFBFE0FF��6X   Z� 6w   (  "System.Windows.Controls.WebBrowser  5F      -   5          Name$ Browser��5F      $��922.143��6*   $
��1692��6;   $	 -12��6H   $	 -30��6Z    � UseLayoutRounding .  }�.  $��0,0,0,0q�6�   $��Disabled=�6�   $��Disabled=�6�   ! �� IsDeferredScrollingEnabled .  ��.  5G      �� 5H      -   5        $��image1��5H   R    J� Opacity$	 0.9Q�6   $
 1072��6"   $	 590��65   $��70��6F   $
��Fill=�6`   $��75��6o   �� 5J      -   5        $��label1��5J   b   $
 1056��6   $	 800��6'   $��Current Controller:��68   $��28��6V   $	��108��6p   �� 5K      -	   5        $��controllerText��5K   `   $	 818��6   $	 800��6&   $��Example Controller��67   $��28��6T   $	��140��6v   �� 5L      -
   5        $��
textBlock1��5L   E   $	 447��6   $ 0��6*   $��17��69   $1��+(Use keyboard {1, 2} to change controllers)��6W   $	��249��6�   5M      �� 5P   
   -   5        $��checkBoxSeatedMode��5P   �   $��2��6   #��[ 6    $��Seated Mode��6H   $
��Auto��6^   $
��Left=�6l   $��Center=�6�   $��
0 10 10 10q�6�   6�   �� 5Q   
   -   5        $��	statusBar��5Q   ?   $��3��6   $��Stretch=�6!   $��Bottom=�6P   
�������#��[  6~   .@�5R      �� $��0 0 0 10q�6   .��5S      �� -   5        $��statusBarText��5S      $��-1 0 0 0q�61   DBClick 'Seated' to change skeletal pipeline type and explore casual6�   5T      5U      5V      5W      