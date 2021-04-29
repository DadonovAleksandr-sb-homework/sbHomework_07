using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Homework_07
{
    /// <summary>
    /// Перечисление вариантов сортировки записей ежедневника
    /// </summary>
    public enum DailyPlannerSort { Num, Date, Text}
    
    /// <summary>
    /// Ежедневник
    /// </summary>
    public struct DailyPlanner
    {
        /// <summary>
        /// Кол-во записей в ежедневнике при его создании или очистке
        /// </summary>
        private int initSize;
        /// <summary>
        /// массив записей ежедневника
        /// </summary>
        private Entry[] entries;
        /// <summary>
        /// путь к файлу с записями (БД ежедневника)
        /// </summary>
        private string filePath;
        /// <summary>
        /// кол-во записей в ежедневнике
        /// </summary>
        private int count;

        /// <summary>
        /// Кол-во записей в ежедневнике
        /// </summary>
        public int Count
        {
            get { return count; }
        }
        /// <summary>
        /// Записи возможно загрузить с диска
        /// </summary>
        public bool CanLoad
        {
            get { return File.Exists(filePath); }
        }
        /// <summary>
        /// Файл с записями ежедневника
        /// </summary>
        public string FilePath
        {
            get { return filePath; }
        }
        /// <summary>
        /// Ежедневник
        /// </summary>
        /// <param name="entriesSize">Кол-во записей в ежедневнике при его создании или очистке</param>
        /// <param name="filePath">Файл с записями ежедневника</param>
        public DailyPlanner(int entriesSize, string filePath)
        {
            this.initSize = entriesSize;
            this.filePath = filePath;
            this.entries = new Entry[initSize];
            this.count = 0;
        }
        
        /// <summary>
        /// Добавление записи в ежедневник
        /// </summary>
        /// <param name="entry">запись</param>
        public void Add(Entry entry)
        {
            if (count >= this.entries.Length)
            {
                Resize();
            }
            entry.AddingDate = DateTime.Now;
            this.entries[count++] = entry;
        }
       /// <summary>
       /// Удаление записи по ее номеру
       /// </summary>
       /// <param name="num">Порядковый номер записи</param>
       /// <returns></returns>
        public int Delete(int num)
        {
            if (num < 1 || num > count)
                return 0;
            this.entries[num - 1].State = EntryState.Deleted;
            RemoveEntries();
            return 1;
        }
        
       /// <summary>
       /// Удаление записи по дате
       /// </summary>
       /// <param name="date">Дата записи</param>
       /// <returns></returns>
        public int Delete(DateTime date)
        {
            var deletedCount = 0;
            for (int i = 0; i < this.count; i++)
            {
                if (entries[i].StartDate.Date == date.Date)
                {
                    entries[i].State = EntryState.Deleted;
                    deletedCount++;
                }
            }
            RemoveEntries();
            this.count -= deletedCount;
            return deletedCount;
        }
        /// <summary>
        /// Удаление записи по тексту заметки
        /// </summary>
        /// <param name="note">заметка</param>
        /// <returns></returns>
        public int Delete(string note)
        {
            if (string.IsNullOrEmpty(note) || string.IsNullOrWhiteSpace(note))
                return 0;
            var count = 0;
            for (int i = 0; i < this.count; i++)
            {
                if (entries[i].Note.Trim().ToLower() == note.Trim().ToLower())
                {
                    entries[i].State = EntryState.Deleted;
                    count++;
                }
            }
            RemoveEntries();
            return count;
        }
        /// <summary>
        /// Загрузка данных с диска
        /// </summary>
        /// <returns></returns>
        public bool Load()
        {
            if (!File.Exists(filePath))
                return false;
            Clear();
            using (StreamReader sr = new StreamReader(filePath, Encoding.Unicode))
            {
                string line;
                Entry entry;
                while ((line = sr.ReadLine()) != null)
                {
                    entry = LineToEntry(line);
                    this.Add(entry);
                }
            }
            return true;
        }
        /// <summary>
        /// Загрузка записей в заданном диапазоне дат
        /// </summary>
        /// <param name="startDate">дата начала выборки записей</param>
        /// <param name="endDate">дата окончания выборки записей</param>
        /// <returns></returns>
        public bool Load(DateTime startDate, DateTime endDate)
        {
            if (!File.Exists(filePath))
                return false;
            Clear();
            using (StreamReader sr = new StreamReader(filePath, Encoding.Unicode))
            {
                string line;
                Entry entry;
                while ((line = sr.ReadLine()) != null)
                {
                    entry = LineToEntry(line);
                    if(entry.StartDate >= startDate && entry.StartDate <= endDate)
                    {
                        this.Add(entry);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// Сортировка записей
        /// </summary>
        /// <param name="sort">Вариант сортировки записей</param>
        public void Sort(DailyPlannerSort sort)
        {
            switch (sort)
            {
                case DailyPlannerSort.Num:
                    // сортировка пузырьком
                    bool f;
                    for (int j=0; j < this.count-1; j++)
                    {
                        f = false;
                        for (int i = 0; i < this.count-1-j; i++)
                        {
                            if (entries[i].Priority > entries[i + 1].Priority)
                            {
                                Swap(entries, i, i+1);
                                f = true;
                            }
                        }
                        if(!f) break;
                    }
                    break;
                case DailyPlannerSort.Date:
                    // шейкерная сортировка
                    var left = 0;
                    var right = this.count - 1;
                    var count = 0;
                    while (left < right)
                    {
                        for (int i = left; i < right; i++)
                        {
                            count++;
                            if (entries[i].StartDate > entries[i + 1].StartDate)
                            {
                                Swap(entries, i, i+1);
                            }
                        }
                        right--;
                        for (int i = right; i > left; i--)
                        {
                            count++;
                            if (entries[i - 1].StartDate > entries[i].StartDate)
                            {
                                Swap(entries, i, i-1);
                            }
                        }

                        left++;
                    }
                    break;
                case DailyPlannerSort.Text:
                    entries = entries.OrderBy(x => x.Note).ToArray();
                    break;
            }
        }
        /// <summary>
        /// Перестановка элементов массива местами
        /// </summary>
        /// <param name="arr">ссылка на массив</param>
        /// <param name="index1">индекс первого элемента</param>
        /// <param name="index2">индекс второго элемента</param>
        private void Swap(Entry[] arr, int index1, int index2)
        {
            var tmp = arr[index1];
            arr[index1] = arr[index2];
            arr[index2] = tmp;
        }
        /// <summary>
        /// Вывод записей ежедневника на консоль
        /// </summary>
        public void Print()
        {
            Console.WriteLine($"{"Номер", 15} {"Дата", 25} {"Заметка", 100} {"Приоритет", 30}");
            for (int i=0; i<count; i++)
            {
                Console.WriteLine($"{i+1, 15} {entries[i].StartDate.ToString(), 25} {entries[i].Note,100} {entries[i].Priority, 50}");
            }
        }
        /// <summary>
        /// Сохранение записей ежедневника на диск
        /// </summary>
        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.Unicode))
            {
                string line;
                for (int i=0; i<count; i++)
                {
                    line = EntryToLine(entries[i]);
                    sw.WriteLine(line);
                }
            }

        }
        /// <summary>
        /// Расширение массива записей
        /// </summary>
        private void Resize()
        {
            Array.Resize(ref this.entries, this.entries.Length*2);
        }
        /// <summary>
        /// Удаление помеченных элементов (записей)
        /// </summary>
        private void RemoveEntries()
        {
            this.entries = this.entries.Where(x => x.State != EntryState.Deleted).ToArray();
        }
        /// <summary>
        /// Полнвая очитска записей
        /// </summary>
        private void Clear()
        {
            this.entries = new Entry[initSize];
            count = 0;
        }
        /// <summary>
        /// Преобразование строкового представления записи в структуру
        /// </summary>
        /// <param name="line">строковое представление</param>
        /// <returns>Запись</returns>
        private Entry LineToEntry(string line)
        {
            string[] data = line.Split('\t');
            Entry entry = new Entry();
            entry.State = EntryState.Empty;

            entry.AddingDate = DateTime.Parse(data[0]);
            entry.StartDate = DateTime.Parse(data[1]);
            entry.Duration = TimeSpan.Parse(data[2]);
            entry.Note = data[3];
            entry.Priority = int.Parse(data[4]);

            return entry;
        }
        /// <summary>
        /// Преобразование записи в строковое представление
        /// </summary>
        /// <param name="entry">Запись</param>
        /// <returns>Строковое представление записи</returns>
        private string EntryToLine(Entry entry)
        {
            return
                $"{entry.AddingDate.ToString()}\t{entry.StartDate.ToString()}\t{entry.Duration.ToString()}\t{entry.Note}\t{entry.Priority}";
        }
    }
}