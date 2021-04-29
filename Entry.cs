using System;

namespace Homework_07
{
    /// <summary>
    /// Перечисление статуса записи
    /// </summary>
    public enum EntryState
    {
        Empty, 
        Active,
        Deleted
    };

     public struct Entry
    {
        /// <summary>
        /// Дата добавления записи
        /// </summary>
        public DateTime AddingDate { get; set; }
        /// <summary>
        /// Дата начала события
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// Продолжительность события
        /// </summary>
        public TimeSpan Duration { get; set; }
        /// <summary>
        /// Замметка
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// Приоритет
        /// </summary>
        public int Priority {get; set;}
        /// <summary>
        /// Статус записи
        /// </summary>
        public EntryState State { get; set; }
    }
}