   M S B A M L   `   `   `  ���� >  :GED, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullclr-namespace:GEDGED      GED.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     #localclr-namespace:GED     -     �Name$  
Window_GED�� � Title$	 GED��$��536.8��$��785.4��.��]� .+��� $��Bottom=� �� Orientation$ 
Horizontal=�}�.  K� ZIndex$ 2�����	#FF99B5F5$��RightToLeft=�.+��� -    ��Name$ data_xy��$��90                    ���� -   $ 
coordinate��$��:Координаты ��$��0,0,1,0q����	#FF020202�� -   $ md��$��70��$��LeftToRight=��� -   $ now_mode��$��:Режим ��$��0,0,1,0q�
���   ��� -   $ 
elem_group��$��0,0,1,0q�
���   ��� $ 
Horizontal=�.+��� -    ��Name$ panel_options��$
��Left=�$ 1�����	#FF33527E$��
0,0,0,-0.4q�$	��100��.+�o� 
�������.@�m� $��Файл��.@�m� -    m�Name$ 	Save_file��$��Сохранить��
�������m� -	   $ 	Open_file��$��Открыть��
��������� -
    ��Name$ std_bt��$��Курсор��
 � Tag$ integer(point)��
��������� -   $ line_bt��$��
Линия��
��������� -   $ bt_morf��$��Морфинг��
��������� -   	 ��Name$	 Slider_morf��
 � Maximum$
 1Q� �� TickFrequency$	 0.1Q� � LargeChange$	 0.1Q� �� TickPlacement$ BottomRight=� J� IsHitTestVisible . ����  �� Color$ 	#FF9CB4F2���� $ 
Horizontal=�.+��� -   $ 	save_morf��$��	Сохр.��$��51��
��������� -   $ cancle_morf��$��Отмен.��$��50��
��������� $ 
Horizontal=�.+��� -   $ bt_backward��$��33������ $ 	#FF707070�� �� Opacity$ 0Q����� $ Black��$ 0Q�.���� -    ��Name$ backward��$��
Arrow1.png��$��20�� J� RenderTransformOrigin$ 0.5,0.5&�z�V� .~���  �� ScaleY$ 1Q� �� ScaleX$ -1Q���  �� AngleY$ 0Q� �� AngleX$ 0Q���  �� Angle$ 0Q�T� �� -    ��Name$ count_list_canvas��$�� ��$��34��
������� �� TextAlignment$ Center=�$��14(��� -   $ 
bt_forward��$��33������ $ 	#FF707070��$ 0Q����� $ 	#FFDDDDDD��$ 0Q�.���� -   $ forward��$��
Arrow1.png��$��20��� -    �Name$ DG�� � RowDefinitions �� �� ��  � ColumnDefinitions �� �� �� �� -   $ go_transform��$��Выполнить��
��������� -   $ bt_anim��$��0q����	#FF99B5F5.���� -   $ 	arrow_img��$��	Arrow.png��$��20���� -    ��Name$ scr_view��$
��Auto=�$
��Auto=�$ 1��.���� -    ��Name$ canvas��$ 0��|�. 
�������$ 0.5,0.5&�z�V� .~��� $ -1Q�$ 1Q��� $ 0Q�$ 0Q��� $ 0Q�T� 