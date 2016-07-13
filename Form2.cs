using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace InWorkTask
{
    public partial class Form2 : Form
    {

        private sqliteclass mydb = null;
        private string sPat = string.Empty;// путь, где находится "exe" файл приложения и полный путь к файлу базы данных
        private string sSql = string.Empty;
        private sqliteclass infodb = null;


        string type = null;


        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

            comboBox2.Text = "Choose column, which you want delete";
            //                  COMBOBOX2 (FILL COMBOBOX WITH COLUMNS FROM TABLE)
            #region combobox1
                       
            mydb = new sqliteclass();
            sSql = "select * from  tasks";
            sPat = Path.Combine(Application.StartupPath, "mybd.db");
            DataRow[] datarows = mydb.drExecute(sPat, sSql);
            if (datarows == null)
            {
                Text = "Fail!";
                mydb = null;
                return;
            }
            // temporary table which store all information from database 
            DataTable table1 = new DataTable();
            try
            {
                table1 = datarows[0].Table;

                //delete id from table1
                table1.Columns.Remove("id");

                foreach (DataColumn col in table1.Columns)
                {
                    comboBox2.Items.Add(col.ColumnName);
                }

            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("Empty table!" + ex.Message);
                return;
            }
            #endregion 







            //                 FILL COMBOBOX2
            comboBox1.Text = "Choose column format";
            String[] Types = { "text", "numeral" };
            comboBox1.Items.AddRange(Types);   

        }

        // create db
        private void button4_Click(object sender, EventArgs e)
        {


            mydb = new sqliteclass();


            // name of db
            string name = textBox2.Text; //mydb
            // name of table
            string nameTable = textBox4.Text; // tasks
            // name of new column
            string nameColumn = textBox1.Text; // id


            string namePath = name + @".db ";
            sPat = Path.Combine(Application.StartupPath, namePath);

            //sSql = @"CREATE TABLE if not exists [birthday]([id] INTEGER PRIMARY KEY AUTOINCREMENT,[FIO] TEXT NOT NULL,[bdate] datetime NOT NULL,[gretinyear] INTEGER DEFAULT 0);";
            sSql = @"CREATE TABLE if not exists [" + nameTable + @"]" + @"([" + nameColumn + @"] INTEGER PRIMARY KEY AUTOINCREMENT);";

            textBox3.Text = sSql;


            //Class1.SqlCreate = @"CREATE TABLE if not exists [tasks]([id] INTEGER PRIMARY KEY AUTOINCREMENT, [idstr] TEXT NOT NULL , [info] TEXT NOT NULL, [date] TEXT, [place] TEXT , [sentrale] TEXT, [status] TEXT, [lastDate] TEXT NOT NULL);";


            try
            {
                if (mydb.iExecuteNonQuery(sPat, sSql, 0) == 0)
                {
                    MessageBox.Show("Error! Table was not creating!");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("Table was created!");
            
            
            mydb = null;
            textBox1.Text = String.Empty;
            textBox2.Text = String.Empty;
            textBox4.Text = String.Empty;
            return;


        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    {
                        type = @" TEXT";
                        break;
                    }
                case 1:
                    {
                        type = @" INTEGER";
                        break;
                    }
                default:
                    type = "0";
                    break;
            }


        }



        // Add columns
        private void button1_Click(object sender, EventArgs e)
        {
            mydb = new sqliteclass();
            sPat = Path.Combine(Application.StartupPath, "mybd.db");

            // new column name
            string columnName = textBox1.Text;

            if (columnName == String.Empty)
            {
                MessageBox.Show("Write name of column!");
                return;
            }

            // chekin if type is choosen
            if (type == null)
            {
                MessageBox.Show("Choose format of new column!");
                return;
            }


            //example: sSql = @" alter table tasks add test3 text; ";
            // sql which adding new column in db
            sSql = @" ALTER TABLE tasks ADD " + columnName +  type + @";";
            textBox3.Text = sSql;


            if (mydb.iExecuteNonQuery(sPat, sSql, 1) == 0)
            {
                Text = "Fail!";
            }
            Text = "Sucsess!";
            sPat = null;




            //   add to bd of columns properties
            infodb = new sqliteclass();
            sPat = Path.Combine(Application.StartupPath, "infoDb.db");
            sSql = @"insert into infoDb (columnName ,type) values('" + columnName + @"' , '" + type + @"');";
            if (mydb.iExecuteNonQuery(sPat, sSql, 1) == 0)
            {
                Text = "infoDb additing fail!";
            }
            sSql = null;
            sPat = null;


        }

        //delete column
        private void button3_Click(object sender, EventArgs e)
        {

            FileStream file1 = new FileStream("test.csv", FileMode.Append); // create file stream
            StreamWriter writer = new StreamWriter(file1); //создаем «потоковый писатель» и связываем его с файловым потоком
            
            string tekst = String.Empty;

            mydb = new sqliteclass();
            sSql = "select * from  tasks";
            DataRow[] datarows = mydb.drExecute(sPat, sSql);
            if (datarows == null)
            {
                Text = "Fail of reading data base!";
                mydb = null;
                return;
            }
            Text = "";
            
            DataTable table2 = datarows[0].Table;

            string tempTekst; // temporary string, wich summarize tekst with sign ;
            foreach (DataColumn col in table2.Columns)
            {
                tempTekst = col.ColumnName + ";";
                tekst += tempTekst;

            }           
            writer.WriteLine(tekst);

            tekst = String.Empty;
            tempTekst = String.Empty;
            
            int k = datarows.Count ();
            int n = table2.Columns.Count;
            string semicolon = ";";
            string teksten = String.Empty;
            
                 
            for (int i = 0; i < k ; i++) // int  ряд
            {
                for (int j = 0 ;j < n ; j++)  // столбец
                {
                    tempTekst = datarows[i].ItemArray[j].ToString();
                    tekst = tempTekst + semicolon;
                    teksten += tekst;
                    
                }
                    textBox3.Text = teksten;
                    writer.WriteLine(teksten);
                    teksten = String.Empty;
            }
            

            /*

                foreach (DataRow dr in datarows)
                {
                    tekst = dr[0].ToString() + ";" + dr[1].ToString() + ";" + dr[2].ToString() + ";" + dr[3].ToString() + ";" + dr[4].ToString() + ";" + dr[5].ToString() + ";" + dr[6].ToString() + ";" + dr[7].ToString() + ";" + dr[8].ToString() + ";" + dr[9].ToString() + ";" + dr[10].ToString() + ";" + dr[11].ToString() + ";" + dr[12].ToString() + ";";
                    writer.WriteLine(tekst);
                    textBox3.Text = tekst;
                    tekst = String.Empty;

                }
           */
            
            writer.Close(); //закрываем поток. Не закрыв поток, в файл ничего не запишется 

        }

        // combobox delete column
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {



        }

        

        
    }
}
