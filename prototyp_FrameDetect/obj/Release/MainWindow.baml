   M S B A M L   `   `   `  ����   EarthExplorer.    (Microsoft.mmi.Kinect.Explorer.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     � Title$  Earth Explorer��$	��760��$
��1030��$��0q� � WindowStyle$ SingleBorderWindow=� � Topmost .  
������  � WindowStartupLocation$ CenterScreen=� � WindowState$ Normal=� � 	Resources    MediumGreyBrush �� Color  KinectPurpleBrush  KinectBlueBrush  MyButtonStyle �� 
TargetType �� Property	 �� Value
 �� 
TargetType �� Padding �� CornerRadius �� Triggers P� Property P� Value �� 
TargetName��%h  &	        &	      &	 *     &	 ?     �� $ 	#ff6e6e6e���� $ 	#ff52318f���� $ 	#ff00BCF2���� " ��.���  �@�OverridesDefaultStyle	 . ��  � ��	 �� "
 ��.R��� $��border��$��0q�$	 4,2q�
�������$ 3��#��z��._��� $��Center=�$��Center=� P�  �@J�IsMouseOver . . ��� $ border�� � ��
	 �   �.��� -   $��
layoutGrid��$��	10 0 10 0q� � RowDefinitions �� $
��Auto ��� �� $
��Auto ��� $
��Auto �.+�(� $��2�� (� Stretch$
 None=� J� Opacity$ 50Q�$
��Left=�.���� -   $��Image��$	��130��$
��auto��(� $��1�� K� ZIndex$ 1��$
 Fill=�$ 0Q�$��Center=�.��(  "System.Windows.Controls.WebBrowser  -     Name$ Browser��$
��1500��$	��550��$��Center=� � UseLayoutRounding .  }�. $��Disabled=�$��Disabled=�! �� IsDeferredScrollingEnabled .  ��.  $��3�� J� RenderTransformOrigin$ 0.816,0.481&��� -   $��	statusBar��$��2��$ 0��$��Stretch=�$��	130,0,0,0q�$	��130��$��Bottom=�
������ #��[  .@�� -   $��layoutGrid2��$��Stretch=�.+��� -   $��PositionBody��$��15(�
���   �$��100,0,448,99q�$	��200��$��25��''�� -   $��gestureMove��$��15(�
���   �$��100,25,448,74q�$	��200��$��25��''�� -	   $��gestureZoom��$��15(�
���   �$��100,50,448,49q�$	��200��$��25��''�� -
    ��Name$ gesturePerspective��$��15(�
���   �$��100,75,448,24q�$	��200��$��25��''�� -   $��speech��$��15(�
���   �$��100,100,448,0q�$	��200��$��25��''�� -    ��Name$ proportionParam��$	��120��$��25��
���   �$��15(�$��proportionParam�� �� 
IsReadOnly .  $��320,-100,300,0q�.���� -   $ minMovementFrame��$	��120��$��25��
���   �$��15(�$��minMoveFrame�� .  $��320,-50,300,0q�.���� -   $ 	zoomSpeed��$	��120��$��25��
���   �$��15(�$��	zoomSpeed�� .  $��320,0,300,0q�.���� -   $ handMovingZTollerance��$	��120��$��25��
���   �$��15(�$��handZTollerance�� .  $��320,50,300,0q��� -   $ 	distanceX��$��0��$��0��
���   �$��15(�$��	distanceX�� .  $��320,100,300,0q��� -   $ tolleranceHandsSDiff��$	��120��$��25��
���   �$��15(�$��** ungenutzt **�� .  $��320,100,300,0q��� -   $
��left��$	��150��$��442,0,254,30q��� -   $��right��$	��150��$��442,0,254,30q��� -   $��up��$	��150��$��442,0,254,30q��� -   $
��down��$	��150��$��442,0,254,30q��� -   $��superman��$��396,0,404,30q��� -   $��zoomIn��$	��254��$��
592,0,0,30q��� -   $��zoomIn2x��$	��254��$��
592,0,0,30q��� -   $��zoomOut��$	��254��$��
592,0,0,30q��� -   $��	zoomOut2x��$	��254��$��
592,0,0,30q��� -   $
��icon��$��	0,5,746,4q��� -   $ noKinect��$��Center=� �� TextAlignment$ Center=�$��20(�
���  ��$��
494,94,0,0q��� -    ��Name$ kinectUp��#��[ $�� ��$��
1,5,746,65q�
������ �� -   $ 
kinectDown��#��[ $�� ��$��
0,65,746,5q�
������ 