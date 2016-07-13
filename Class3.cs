using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// пространства имен необходимые для работы с базой данныхы
using System.Data;
using Finisar.SQLite;


namespace InWorkTask
{
    class sqliteclass
    {

        // public SQLiteCommandBuilder comBuild;
        //public SQLiteDataAdapter ad;


        //Конструктор
        public sqliteclass()
        {

        }
        //SQL операторы  можно подразделить на два основных типа:

        //*Не возвращающие значения из базы данных (в лучшем случае возвращающие число записей, которые затронуло их выполнение - "Insert", "Delete", "Update").
        //*Возвращающие запрошенную информацию из таблиц баз данных. 


        //                                 ФУНКЦИИ СОЗДАЮЩИЕ ЗАПРОСЫ К БАЗЕ ДАННЫХ 

        //******************************** операторы не возвращающие значения************************************

        // region служит для того чтобы закрыть или открыть область 
        #region iExecuteNonQuery
        //FileData - полный путь к базе данных, sSql - запрос, where - новая бд или уже существует
        // выполняет функции вставить удалить обновить 
        public int iExecuteNonQuery(string FileData, string sSql, int where)
        {
            // количество задействованных строк во время вставки удаления или обновления
            int n = 0;

            try
            {
                //SQLiteConnection - создать соединение с базой.

                // using используется чтобы не использовать dispose для уничтожения объектов

                using (SQLiteConnection con = new SQLiteConnection())
                {
                    if (where == 0)
                    {
                        // аналогичный пример SQLiteConnection con = new SQLiteConnection(string.Format("Data Sourse={0};", databaseName));
                        // new = true  означает новая бд
                        con.ConnectionString = @"Data Source=" + FileData + ";New=True;Version=3";
                    }
                    else
                    {
                        // new - false - не новая бд.
                        con.ConnectionString = @"Data Source=" + FileData + ";New=False;Version=3";
                    }

                    //Открывает подключение к базе данных со значениями свойств, определяемыми объектом ConnectionString.
                    //Это подключение автоматически закрывается при выходе из блока using.
                    con.Open();

                    //Создает и возвращает объект SqlCommand, связанный с SqlConnection.
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        // sSql - текст команды
                        sqlCommand.CommandText = sSql;


                        //Метод ExecuteNonQuery позволяет выполнять операции с каталогом (например, запросы структуры базы данных 
                        //или создание таких объектов базы данных, как таблицы) или вносить изменения в базу данных, не используя DataSet,
                        //с помощью операторов UPDATE, INSERT или DELETE.
                        //Хотя метод ExecuteNonQuery не возвращает ни одной строки, любые выходные параметры или возвращаемые значения,
                        //сопоставляемые с параметрами, заполняются данными.
                        //Операторы UPDATE, INSERT и DELETE возвращают количество строк, которые были обработаны с их помощью. 
                        //Когда у таблицы, для которой выполняется вставка или обновление строк, имеются триггеры, возвращаемое значение
                        //содержит количество задействованных в операции строк, а также количество строк, обработанных триггерами. 
                        //Для всех прочих типов операторов возвращаемым значением является -1. В случае отката также возвращается значение -1. 
                        //ExecuteNonQuery() -выполняет инструкцию Transact-SQL для установленного соединения и возвращает количество задействованных в инструкции строк.
                        n = sqlCommand.ExecuteNonQuery(); // 
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                n = 0;
            }
            return n;
        }
        #endregion


        //**********************Создает соединение с БД и возвращает запрошенную информацию из таблиц баз данных.*****************************
        // чтение из базы данных
        //  Директива #region позволяет указать блок кода, который можно разворачивать и сворачивать с помощью функции структурирования в редакторе кода Visual Studio.                                  

        #region drExecute

        public DataRow[] drExecute(string FileData, string sSql)
        {
            // массив типа DataRow
            //Объекты DataRow и DataColumn являются первичными компонентами DataTable. Используйте объект
            //DataRow и его свойства и методы для извлечения и оценки, вставки, удаления и обновления значений в DataTable.
            DataRow[] datarows = null;

            // DataAdapter выполняет функцию моста между DataSet и источником данных для получения и сохранения данных. 
            //DataAdapter выполняет функции моста посредством сопоставления метода Fill, изменяющего данные в DataSet 
            //для обеспечения соответствия данным в источнике данных, и метода Update, изменяющего данные в источнике данных для 
            //обеспечения соответствия данным в DataSet. 
            SQLiteDataAdapter dataadapter = null;

            // DataSet является находящимся в оперативной памяти представлением данных
            // Существует несколько способов работы с DataSet, которые могут применяться отдельно или в сочетании. Можно сделать следующее. 
            //Заполнить DataSet таблицами данных из существующего реляционного источника данных с помощью DataAdapter.
            // Объект DataSet необходимо заполнить, прежде чем направлять к нему запросы LINQ to DataSet. Существует несколько способов заполнения объекта DataSet. 
            // распространенный способ загрузки данных в объект DataSet — использование класса DataAdapter
            //для получения данных из базы данных. Это показано в следующем примере. 
            DataSet dataset = new DataSet();

            // Представляет одну таблицу с данными в памяти. 
            DataTable datatable = new DataTable();
            try
            {
                using (SQLiteConnection con = new SQLiteConnection())
                {
                    con.ConnectionString = @"Data Source=" + FileData + ";New=False;Version=3";
                    con.Open();
                    using (SQLiteCommand sqlCommand = con.CreateCommand())
                    {
                        dataadapter = new SQLiteDataAdapter(sSql, con);
                        dataset.Reset();
                        dataadapter.Fill(dataset);
                        datatable = dataset.Tables[0]; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                        datarows = datatable.Select();

                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                datarows = null;
            }
            return datarows;

        }
        #endregion
    }
}

