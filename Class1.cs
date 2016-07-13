using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

// пространства имен необходимые для работы с базой данныхы
using System.Data;
using Finisar.SQLite; 

namespace InWorkTask
{

    public class Class1
    {
        //SQLiteConnection - создать соединение с базой.
        SQLiteConnection con;

        /*DataAdapter выполняет функцию моста между DataSet и источником данных для получения и сохранения данных. 
        DataAdapter выполняет функции моста посредством сопоставления метода Fill, изменяющего данные в DataSet 
        для обеспечения соответствия данным в источнике данных, и метода Update, изменяющего данные в источнике данных для 
        обеспечения соответствия данным в DataSet
        */
        SQLiteDataAdapter dataadapter;

        /*DataSet является находящимся в оперативной памяти представлением данных
        Существует несколько способов работы с DataSet, которые могут применяться отдельно или в сочетании. Можно сделать следующее. 
        Заполнить DataSet таблицами данных из существующего реляционного источника данных с помощью DataAdapter.
        Объект DataSet необходимо заполнить, прежде чем направлять к нему запросы LINQ to DataSet. 
        Существует несколько способов заполнения объекта DataSet. 
        распространенный способ загрузки данных в объект DataSet — использование класса DataAdapter
        для получения данных из базы данных. Это показано в следующем примере.
        */
        DataSet dataset;


        SQLiteCommandBuilder builder;

        // путь, где находится "exe" файл приложения и полный путь к файлу базы данных
        private string sPath = Path.Combine(Application.StartupPath, "mybd.db");


        public DataSet CreatDataSet(string str, SQLiteConnection con)
        {

            // show all tasks
            string sSql = str;

            try
            {
                con = new SQLiteConnection();

                con.ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
                con.Open();
                

                using (SQLiteCommand sqlCommand = con.CreateCommand())
                {
                    dataadapter = new SQLiteDataAdapter(sSql, con);


                    // DataSet является находящимся в оперативной памяти представлением данных
                    // Существует несколько способов работы с DataSet, которые могут применяться отдельно или в сочетании. Можно сделать следующее. 
                    //Заполнить DataSet таблицами данных из существующего реляционного источника данных с помощью DataAdapter.
                    // Объект DataSet необходимо заполнить, прежде чем направлять к нему запросы LINQ to DataSet. Существует несколько способов заполнения объекта DataSet. 
                    // распространенный способ загрузки данных в объект DataSet — использование класса DataAdapter
                    //для получения данных из базы данных. Это показано в следующем примере. 
                    dataset = new DataSet();
                    dataset.Reset();

                    dataadapter.Fill(dataset);
                    //dataGridView1.DataSource = dataset.Tables[0];
                }
                
                con.Close();
                
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error!" + ex.Message);
            }



            return dataset;
        }


        public void ChangeData(string str)
        {

            // show all tasks
            string sSql = str;

            try
            {
                con = new SQLiteConnection();
                con.ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
                con.Open();

                using (SQLiteCommand sqlCommand = con.CreateCommand())
                {
                    dataadapter = new SQLiteDataAdapter(sSql, con);
                    dataset = new DataSet();
                    dataset.Reset();

                    dataadapter.Fill(dataset);
                    //dataGridView1.DataSource = dataset.Tables[0];//!!!!!!!!!!!!!!!!
                }

                builder = new SQLiteCommandBuilder(dataadapter);
                dataadapter.Update(dataset);
                MessageBox.Show("Information updates!");
                con.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error!" + ex.Message);
            }


        }
    }
}
