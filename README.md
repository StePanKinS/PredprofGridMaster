# Запуск
1. Установите [.Net Desctop Runtime 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
1. Скачайте и разархивируйте [zip архив](https://github.com/StePanKinS/PredprofGridMaster/releases/download/v1.1.0/PredprofInterpreter.zip)
1. Запустите PredprofInterpreter.exe


## Структура проекта
1. MainWindow.xaml - Вся разметка главного окна, кроме поля игрока
1. MainWindow.xaml.cs - Разметка поля игрока, все обработчики кнопок и меню "Файл" главного окна, анимация игрока, вывод ошибок, (де)сериализация положений игрока, сохранение и загрузка из файла
1. DB.cs - Общение с базой данных
1. DBWindow.xaml и DBWindow.xaml.cs - Интерфейс для использования DB.cs
1. PlayerBorder - Зеленый квадратик со своими координатами
1. Image.png - Просто зеленый квадратик
1. Interpreter.cs - Интерпретатор. Работает в отдельном потоке. Имеет 2 режима: пошаговый и без остановки. Для начала работы надо вызвать метод Start. При пошаговом воспроизведении нужно вызывать метод Step до окончания програмы. После выполнения програмы можно получить траекторию игрока методом GetTrajectory. Метод IsRunning возвращает, запущена ли програма
1. BlockType.cs - Хранит тип блока
1. CodeException.cs - Базовый тип для всех ошибок при интерпретации програмы
1. Остальное в папке Interpreter - Различные типы ошибок
1. Папка mysql - все файлы базы данных

## Видео
https://youtu.be/T-1YLc1zWe0
 
https://drive.google.com/file/d/12d_P_WQESnDfmho1kFXVCB0a1XX2PKrm/view?usp=drive_link