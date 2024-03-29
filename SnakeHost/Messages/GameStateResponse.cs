﻿// ReSharper disable CommentTypo

namespace SnakeHost.Messages
{
    /// <summary>Содержит состояние игрового поля.</summary>
    public class GameStateResponse
    {
        /// <summary>Запущена ли игра администратором.</summary>
        public bool IsStarted { get; set; }

        /// <summary>Поставлена ли игра на паузу администратором.</summary>
        public bool IsPaused { get; set; }

        /// <summary>Текущий номер раунда.</summary>
        public int RoundNumber { get; set; }

        /// <summary>Текущий номер хода.</summary>
        public int TurnNumber { get; set; }

        /// <summary>Время хода в миллисекнудах.</summary>
        public int TurnTimeMilliseconds { get; set; }

        /// <summary>Время до следующего хода в миллисекнудах.</summary>
        public int TimeUntilNextTurnMilliseconds { get; set; }

        /// <summary>Размер игрового поля (ширина, длина).</summary>
        public Size2D GameBoardSize { get; set; }

        /// <summary>Максимальное количество еды генерируемое на поле.</summary>
        public int MaxFood { get; set; }

        /// <summary>Состояния всех игроков и их змеек на поле, включая вашу змейку.</summary>
        public PlayerState[] Players { get; set; }
        
        /// <summary>Ваша змейка. Все точки, из которых она состоит (X, Y), начиная с головы и заканчивая хвостом.</summary>
        public Point2D[] Snake { get; set; }

        /// <summary>Позиции всех точек с едой на поле (X, Y).</summary>
        public Point2D[] Food { get; set; }

        /// <summary>Позиции всех стенок на поле (X левого верхнего края, Y левого верхнего края, ширина, длина).</summary>
        public Rectangle2D[] Walls { get; set; }
    }
}