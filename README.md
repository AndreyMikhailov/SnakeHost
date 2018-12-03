Snake Host  

Swagger: ./swagger  
Player API: ./api/player  
Admin API: ./api/game  


ПРАВИЛА  
  
Общие правила игры:  
По двумерному полю одновременно передвигаются несколько змеек, каждой змейкой управляет игрок.  
Змейка может двигаться влево, вправо, вверх, вниз.  
Игрок ходит змейкой отправляя команды на поворот. Время хода ограничено и, если послать несколько команд во время одного хода, то применится последняя.  
Раунд длится определенное время, за которое происходит множество ходов. После этого запускается следующий раунд с новым полем.  
Змейка увеличивает свою длину поедая яблоки, которые разбросаны по полю. Для этого она должна проползти по месту, где яблоко расположено.  
Игровое поле ограничено и змейка, столкнувшись с его краем, погибает.  
Также на поле расставлены стенки в виде прямоугольников разной ширины и высоты при столкновении с которыми змейка также погибает. 
  
Правила PvP:  
В случае, если змейка столкнулась с другой змейкой, погибает та, что короче.   
Если же змейки одной длины, погибает та, что врезалась сбоку. 
  
Правила повяления на поле:  
После старта рануда, чтобы начать игру, игроку нужно сделать хотя бы один ход в любом направлении, иначе змейка не появится на поле.  
После смерти змейка воскреснет в случайном месте карты и некоторое время не сможет двигаться. При этом она будет защищена от столкновений с другими. Все, кто в нее врежется, погибнут сами.  
  
Цель игры:  
Съесть как можно больше яблок и вырасти больше соперников. 
