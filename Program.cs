using System;
using System.ComponentModel.Design;

namespace Homework_07
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            Цель домашнего задания:
                Научиться создавать собственные типы данных.
                Улучшить навыки работы с файлами.
            
            Разработайте ежедневник, который может хранить множество записей. 
            Каждая запись состоит из нескольких полей, количество которых должно быть не менее пяти. 
            Поля могут быть произвольными, однако одним из них должна быть дата добавления записи.
            
            Для записей реализуйте следующие функции:
                создание,
                удаление,
                редактирование.

            Вы можете сделать создание записи двумя способами:
                Пользователь вводит информацию вручную с клавиатуры.
                Информация генерируется программой.

            Удалять запись можно как по номеру, так и по любому полю. Если несколько записей имеют одинаковые значения полей, следует удалить их все.

            Помимо этого, реализуйте следующие возможности:
                загрузка всех записей из файла,
                сохранение всех записей в файл,
                загрузка записей из файла по диапазону дат,
                упорядочивание записей по выбранному полю.

            Что оценивается
                Создан ежедневник, в котором могут храниться записи.
                Записи имеют как минимум пять полей.
                Одно из полей записи ― дата создания.
                Все записи сохраняются на диске.
                Все записи загружаются с диска.
                С диска загружаются записи в выбранном диапазоне дат.
                Записи можно создавать, редактировать и удалять.
                Записи сортируются по выбранному полю.
            */
            Console.WriteLine("Программа-ежедневник");
            var planner = new DailyPlanner(2, "data.csv");

            LoadData(ref planner);
            PlanningProcess(ref planner);
            SaveData(ref planner);

        }

        /// <summary>
        /// Загрузка данных с диска
        /// </summary>
        /// <param name="planner">ссылка на ежедневник</param>
        private static void LoadData(ref DailyPlanner planner)
        {
            var userAnswer = string.Empty;
            var result = false;
            do
            {
                Console.Write("Желаете загрузить записи с диска? (y/n): ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                if(userAnswer == "y")
                {
                    if (!planner.CanLoad)
                    {
                        Console.WriteLine($"Невозможно загрузить данные. Файл {planner.FilePath} не существует!");
                        return;
                    }
                    break;
                }
                else if (userAnswer == "n")
                {
                    return;
                }
                Console.WriteLine("Невозможно распознать ответ. Пожалуйста введите \"y\" или \"n\"");
                
            } while (true);

            do
            {
                Console.Write("Желаете загрузить все записи? (y/n): ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                if(userAnswer == "y")
                {
                    if(planner.Load())
                    {
                        Console.WriteLine($"Загружено {planner.Count} записей");
                        if (planner.Count > 0)
                        {
                            planner.Print();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Невозможно загрузить записи с диска");
                    }
                    return;
                }
                else if (userAnswer == "n")
                {
                    DateTime startDate, endDate;
                    do
                    {
                        Console.Write("Введите начальную дату: ");    
                        userAnswer = Console.ReadLine().Trim().ToLower();
                        if (DateTime.TryParse(userAnswer, out startDate))
                        {
                            break;
                        }
                        Console.WriteLine($"Невозможно распознать ответ. Пожалуйста введите дату в формате \"dd.mm.yyyy\". Например {DateTime.Now.ToShortDateString()}");
                    } while (true);
                    do
                    {
                        Console.Write("Введите конечную дату: ");    
                        userAnswer = Console.ReadLine().Trim().ToLower();
                        if (DateTime.TryParse(userAnswer, out endDate))
                        {
                            break;
                        }
                        Console.WriteLine($"Невозможно распознать ответ. Пожалуйста введите дату в формате \"dd.mm.yyyy\". Например {DateTime.Now.ToShortDateString()}");
                    } while (true);
            
                    if(planner.Load(startDate, endDate))
                    {
                        Console.WriteLine($"Загружено {planner.Count} записей");
                        if (planner.Count > 0)
                        {
                            planner.Print();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Невозможно загрузить записи с диска");
                    }
                    break;
                }
                Console.WriteLine("Невозможно распознать ответ. Пожалуйста введите \"y\" или \"n\"");
            } while (true);
        }
        
        /// <summary>
        /// Процесс работы с ежедневником
        /// </summary>
        /// <param name="planner">ссылка на ежедненвик</param>
        private static void PlanningProcess(ref DailyPlanner planner)
        {
            var userAnswer = string.Empty;
            do
            {
                Console.Write("Выберите желаемое действие (add - добавить; del - удалить; sort - сортировать; print - распечатать; exit - выйти): ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                switch (userAnswer)
                {
                    case "add":
                        AddingEntry(ref planner);
                        break;
                    case "del":
                        DeletingEntry(ref planner);
                        break;
                    case "sort":
                        SortingEntries(ref planner);
                        break;
                    case "print":
                        planner.Print();
                        break;
                    case "exit":
                        return;
                        break;
                    default:
                        Console.WriteLine("Невозможно распознать ответ");
                        break;
                }
            } while (true);
        }
        /// <summary>
        /// Добавление новой записи в ежедневник
        /// </summary>
        /// <param name="planner"></param>
        private static void AddingEntry(ref DailyPlanner planner)
        {
            Entry entry = new Entry();
            var userAnswer = string.Empty;
            do
            {
                Console.Write("Введите дату: ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                if (DateTime.TryParse(userAnswer, out var startDate))
                {
                    entry.StartDate = startDate;
                    break;
                }
                Console.WriteLine("Невозможно распознать ответ.");
            } while (true);
            do
            {
                Console.Write("Введите заметку: ");
                userAnswer = Console.ReadLine();
                if (!string.IsNullOrEmpty(userAnswer) && !string.IsNullOrWhiteSpace(userAnswer))
                {
                    entry.Note = userAnswer;
                    break;
                }
                Console.WriteLine("Невозможно распознать ответ.");
            } while (true);
            do
            {
                Console.Write("Введите продолжительность: ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                if (TimeSpan.TryParse(userAnswer, out var duration))
                {
                    entry.Duration = duration;
                    break;
                }
                Console.WriteLine("Невозможно распознать ответ.");
            } while (true);
            do
            {
                Console.Write("Введите приоритет: ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                if (int.TryParse(userAnswer, out var priority))
                {
                    entry.Priority = priority;
                    break;
                }
                Console.WriteLine("Невозможно распознать ответ.");
            } while (true);
            planner.Add(entry);
        }
        /// <summary>
        /// Удаление заметки из ежедневника
        /// </summary>
        /// <param name="planner"></param>
        private static void DeletingEntry(ref DailyPlanner planner)
        {
            var userAnswer = string.Empty;
            do
            {
                Console.Write("Введите номер, дату или текст заметки: ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                if (int.TryParse(userAnswer, out var num))
                {
                    planner.Delete(num);
                    break;
                }
                else if (DateTime.TryParse(userAnswer, out var date))
                {
                    planner.Delete(date);
                    break;
                }
                else if (string.IsNullOrEmpty(userAnswer) || string.IsNullOrWhiteSpace(userAnswer))
                {
                    planner.Delete(userAnswer);
                    break;
                }
                Console.WriteLine("Невозможно распознать ответ.");
            } while (true);
        }
        /// <summary>
        /// Сортировка записей в ежедневнике
        /// </summary>
        /// <param name="planner"></param>
        private static void SortingEntries(ref DailyPlanner planner)
        {
            var userAnswer = string.Empty;
            var exit = false;
            do
            {
                Console.Write("Выберите параметр для сортировки (prior - по приоритету; date - по дате; text - по тексту): ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                exit = true;
                switch (userAnswer)
                {
                    case "prior":
                        planner.Sort(DailyPlannerSort.Num);
                        break;
                    case "date":
                        planner.Sort(DailyPlannerSort.Date);
                        break;
                    case "text":
                        planner.Sort(DailyPlannerSort.Text);
                        break;
                    default:
                        Console.WriteLine("Невозможно распознать ответ.");
                        exit = false;
                        break;
                }
                planner.Print();
            } while (!exit);
        }
        /// <summary>
        /// Сохранение данных ежедневника на диск
        /// </summary>
        /// <param name="planner"></param>
        private static void SaveData(ref DailyPlanner planner)
        {
            var userAnswer = string.Empty;
            var result = false;
            do
            {
                Console.Write("Желаете сохранить записи на диск? (y/n): ");
                userAnswer = Console.ReadLine().Trim().ToLower();
                if (userAnswer == "y")
                {
                    planner.Save();
                    Console.WriteLine("Данные сохранены на диск");
                    break;
                }
                else if (userAnswer == "n")
                {
                    break;
                }
                Console.WriteLine("Невозможно распознать ответ.");
            } while (true);
            
        }

        
        
        
        
        
    }
}