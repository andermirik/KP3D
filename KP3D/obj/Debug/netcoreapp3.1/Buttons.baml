   M S B A M L   `   `   `  ����?  ;KP3D, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:Lab1Lab1  5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089   FocusVisual  �� Property �� Value J� SnapsToDevicePixels �� StrokeDashArray  Button.Static.Background �� Color  Button.Static.Border  Button.MouseOver.Background  Button.MouseOver.Border  Button.Pressed.Background  Button.Pressed.Border  Button.Disabled.Background  Button.Disabled.Border 	 Button.Disabled.Foreground 
 MyButtonStyle �� 
TargetType �� 
TargetType ��Name J� Opacity	 ��Name
 �� Triggers P� Property P� Value �� 
TargetName  ComboBoxNormalBorderBrush ! ComboBoxNormalBackgroundBrush # ComboBoxDisabledForegroundBrush # ComboBoxDisabledBackgroundBrush  ComboBoxDisabledBorderBrush   ComboBoxToggleButtonTemplate � ColumnDefinitions �� CornerRadius  ComboBoxFlatStyle �� 	ClickMode d� 	IsChecked �� Path �� RelativeSource �� Mode J� IsHitTestVisible �� SelectionBoxItem �� SelectionBoxItemTemplate �� 
IsReadOnly �� 
IsReadOnly � AllowsTransparency �� IsDropDownOpen � ActualWidth �� MaxDropDownHeight-   'System.Windows.Input.KeyboardNavigation    DirectionalNavigation�� 5      F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     :x,http://schemas.microsoft.com/winfx/2006/xaml     5      localclr-namespace:Lab1  5      %/  &	        &	 �     &	 �     &	 �     &	     &	 ?    &	 b    &	 �    &	 �    &		 �    &	
 �    7   7  7 	 &	 �    &	 �    &	     &	 :    &	 X    &	 v    7  7  7  7  7  7  7  &	 �    7  7  7  �� 5      .�5   
   ��   � �� 5      �� 5      .R�5      �� $��2q�6    $
 true��6+   #��� ��6F   $��1��6�   $	 1 2R�6�   5	      5      5      �� 5      $ 	#FFDDDDDD��67   �� 5      $ 	#FF707070��63   �� 5      $ 	#FFBEE6FD��6:   �� 5      $ 	#FF3C7FB1��66   �� 5      $ 	#FF737B7F��68   �� 5      $ 	#FF2C628B��64   �� 5      $ 	#FFF4F4F4��69   �� 5      $ 	#FFADB2B5��65   �� 5      $ 	#FF838383��69   �� 5      " ��6"   .�5   
   ��   �@�FocusVisualStyle8   6-   �� 5   
     � ��8  6'   �� 5   
     � ��
 ���� 6(   �� 5   
     � ��# � ��6'   �� 5   
     � ��$ Center=�67   �� 5   
     � ��$ Center=�65   �� 5   
     � ��$ 4q�6$   �� 5   
     � �� 5      �� 5       " ��6"   .R�5!      � $
 true��6   .+�5"      �� $ BackgroundVisual��6$   #��z��6>   �� 5#      $ PressedVisual��6$   #���  6;   $ 0Q�6n   �� 5$      $ DisabledVisual��6$   #���  6<   $ 0Q�6p   �� 5%      $	 contentPresenter��6+   $|�False��6E   #��z��6W   #��z��6�   $
��True��6�   # z 6�   #��z��6  5&      
 5'      P� 5(       �@��IsDefaulted$
 true��69   P� 5)       �@J�IsMouseOver$
 true��69   . �5*      �� $ BackgroundVisual��68     �@J�Opacity$	 0.6Q�6V   5+      P� 5,       � ��$
 true��67   . �5-      �� $ PressedVisual��68     �@J�Opacity$ 1Q�6S   5.      P� 5/       � {�$ false��67   . �50      �� $ contentPresenter��6G   8    � ��6e   �� 51      $ DisabledVisual��68     �@J�Opacity$ 1Q�6T   52      54      56      �� 57   
     � ��$ 0q�6,   58      �� 5:      $ #e3e9ef��68   �� 5;      $
 #fff��6<   �� 5<      $
 #888��6>   �� 5=      $
 #eee��6>   �� 5>      $
 #888��6:   �� 5@      " d�6   .R�5A   
   �  5B      �� 5C      �� 5D      $��20 �6#   .+�5F      �� $��Border��6)   $��2��6   8��  $ 0��5H      $��
1, 1, 1, 1q�6    8�� 5I      �� 5J      $��ButtonBorder��6L   $��1��6   $��
1, 1, 1, 1q�6%   ���#444$ 
0, 0, 0, 0��5K      $��
0, 0, 0, 0q�6)   8�� 5L      F� 5N      $��Arrow��6   $��1��6    =����        �    �� � 	= ��[ � z �� � z     � 	=  	= 	$��Center=�5P      ���#444$��Center=�5Q      5R      
 5S   
   P� 5T       �@J�IsMouseOver$
 True��67   . �5U      �� $ ButtonBorder��65     � ��
 �����6O   5V      P� 5W       �@d�	IsChecked$
 TrueY�68   . �5X      �� $ ButtonBorder��65     � ��
 �����6O   �� 5Y      $ Arrow��6/     � �� �	#FF8D979E6B   5Z      P� 5[       � {�$ False��65   . �5\      �� $ Border��65   8    � ��6I   �� 5]      $ ButtonBorder��65   8    � ��6O   �� 5^      $ ButtonBorder��67   8    � ��6Q   �� 5_      8    � ��6;   �� 5`      $ Arrow��6/     � �� �#9996B   5a      5c      �� 5e      " ��6'   .�5f   
   ��   �@J�SnapsToDevicePixels$
 True��6:   �� 5g   
     �@�OverridesDefaultStyle$
 True��6C   �� 5h   
     � ��$
 Auto=�6G   �� 5i   
     � ��$
 Auto=�6E   �� 5j   
     � ��$
 True��6:   �� 5k   
     � ��
 �   �63   �� 5l   
     �@�FocusVisualStyle 6>   Z� �� 5m   
     � �� 5n      �� 5o      " ��6"   .R�5p      � .+�5q      d� $��ToggleButton��6'   $��2��6;   $ Press=�5r      $|�False��6#   8��   5s      �� $ IsDropDownOpen� �� *TemplatedParent+$ TwoWay=�5t      �� 5v      $��ContentSite��6+   $��5, 3, 23, 3q�6>   $ False��6S   $
��Left=�5w      $��Center=�6:   #��z 5x      #��z 5y      #��z��5z      �� 5{      $��PART_EditableTextBox��6"   $��3, 3, 23, 3q�6>   # z 5|      $y�Hidden=�5}      
������ $
��Left=�5~      $��Center=�61   $
|�True��5      .��6(   ��5�      �� 5�   "   " ��62   .R�5�   &   �� $��PART_ContentHost��6-   $|�False��6E   5�   #   .��5�   0   5�      � 5�      $��Popup��6    $��Bottom=�6-   $|�False��5�      $
 True��6&   #��z 5�      $��Slide=�5�      .��5�      � $��DropDown��6#   $
 True��63   #��z 5�      #��z 5�      .+�5�   "   �� $��DropDownBorder��6)   
�������$��
0, 1, 0, 0q�6R   $ 0��5�      $��1,1,1,1q�6*   8�� 5�      �� 5�   "   $��4q�6/   $
 True��6:   .��5�   &   �� $ 	Contained=�65   5�   #   5�      5�      5�      
 5�      P� 5�       �@��HasItems$ False��6C   . �5�      �� $ DropDownBorder��6K     � ��$ 95��6g   5�      P� 5�       � {�$ False��6A   . �5�      �� 8    � ��6G   5�      P� 5�       �@��
IsGrouping$
 True��6E   . �5�      ��   � ��$ False��6N   5�      P� 5�       �@��
IsEditable$
 True��6A   . �5�      ��   �@  	IsTabStop$ False��6M   �� 5�      $ PART_EditableTextBox��6E     � y�$ Visible=�6g   �� 5�      $ ContentSite��6E     � y�$ Hidden=�6^   5�      5�      5�      5�      5�      