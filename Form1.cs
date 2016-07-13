using System;
//using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;


// пространства имен необходимые для работы с базой данныхы
using System.Data;
using Finisar.SQLite; 



namespace InWorkTask
{
    public partial class Form1 : Form
    {
        #region filds

        //conection with db
        SQLiteConnection con;

        /*The DataAdapter serves as a bridge between a DataSet and a data  source for retrieving and saving data. The DataAdapter provides 
         this bridge by mapping Fill, which changes the data in the DataSet to match the data in the data source, and Update, which changes
         the data in the data source to match the data in the DataSet. 
        */
        SQLiteDataAdapter dataadapter;

        //The DataSet, which is an in-memory cache of data retrieved from a data source, is a major component of the ADO.NET architecture.
        DataSet dataset;

        // can be used to generate the Insert, Update, and Delete statements for a DataAdapter.
        SQLiteCommandBuilder builder;
          
        // путь, где находится "exe" файл приложения и полный путь к файлу базы данных
        private string sPath = string.Empty;

        // requeist
        private string sSql = string.Empty;
       

        //DataGridView and Calendar column in datagridview
        
        DataGridViewComboBoxColumn cbc;
        CalendarColumn cal;
        DataGridViewTextBoxColumn sl; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
        
        // object of class which connect and update, add etc..
        sqliteclass mydb = null;

        // datagridview data error indicator 
        int dialogres = 0;

        #endregion
        

        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            button1.BackColor = Color.Gainsboro;
            button2.BackColor = Color.Gainsboro;
            button3.BackColor = Color.Gainsboro;
            button5.BackColor = Color.Gainsboro;
            button6.BackColor = Color.Gainsboro;

            
            label1.Text = "1.For Øst,   2.For Vest,   3.For Pederveien 19,   4.For Grenseveien 31,   5.For Midtveien 12,   6.Dusashvis,   7.Oslo Gjestebolig";
            

            this.Text = "Task manager";
            openFileDialog1.Filter = "csv files (*.csv) | *.csv|All files (*.*)|*.*";
            saveFileDialog1.Filter = "csv files (*.csv) | *.csv|All files (*.*)|*.*";
            button4.Enabled = false; //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Combine method summarize adress of application and name of database
            sPath = Path.Combine(Application.StartupPath, "mybd.db");  
            con = new SQLiteConnection();
            con.ConnectionString = @"Data Source=" + sPath + ";New=False;Version=3";
            sSql = "select * from  tasks";
            

            try
            {
                con.Open();
                
                // using - automaticly  delect object after close curly brase.
                using (SQLiteCommand sqlCommand = con.CreateCommand())
                {
                    dataadapter = new SQLiteDataAdapter(sSql, con);
                    dataset = new DataSet();
                    dataset.Reset();
                    dataadapter.Fill(dataset);

                    try
                    {
                        dataGridView1.DataSource = dataset.Tables[0];
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.Show("Error nr. 1!" + ex.Message);//                     ERROR 1
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error nr. 2!" + ex.Message);//                     ERROR 2
                    }
                }
                con.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error nr.3!" + ex.Message); //                             ERROR 3
            }

            
          

            //***********************change column type to Combobox and CalendarColumn**********************************
            try
            {
                cal = new CalendarColumn();
                dataGridView1.Columns.Remove("StartDato");
                cal.DataPropertyName = "StartDato";
                cal.Name = "StartDato";
                dataGridView1.Columns.Add(cal);

                dataGridView1.Columns.Remove("Status");
                cbc = new DataGridViewComboBoxColumn();
                String[] s = { "Ferdig", "Ikke ferdig", "Ikke aktuelt" };
                cbc.Items.AddRange(s);
                cbc.Name = "Status";
                cbc.DataPropertyName = "Status";
                cbc.HeaderText = "Status";
                dataGridView1.Columns.Add(cbc);

                
                dataGridView1.Columns[9].DefaultCellStyle.BackColor = Color.Bisque;
                dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.Gray;
                dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightBlue;
                dataGridView1.Columns[2].DefaultCellStyle.BackColor = Color.LightBlue;

                dataGridView1.AutoResizeColumn(5);

                // change color of columns headers
                dataGridView1.EnableHeadersVisualStyles = false;
                for (int i = 0; i < dataGridView1.Columns.Count; i++)
                { 
                    dataGridView1.Columns[i].HeaderCell.Style.BackColor = Color.Bisque;
                }
                
                
             }
            catch (Exception ex)
            {
                MessageBox.Show("Error nr.4! " + ex.Message); //                           ERROR 4
                return;
            }

                label2.Text = "Count of rows:" + (dataGridView1.Rows.Count - 1).ToString ();

        }

        //SHOW ALL TASKS
        private void button1_Click(object sender, EventArgs e)
        {
            // count of rows in the table
            label2.Text = "Count of rows:" + (dataGridView1.Rows.Count - 1).ToString();
            
            button1.BackColor = Color.LightBlue;
            button3.BackColor = Color.Gainsboro;
            sSql = "select * from  tasks";
                       
            try
            {
                con.Open();

                using (SQLiteCommand sqlCommand = con.CreateCommand())
                {
                    dataadapter = new SQLiteDataAdapter(sSql, con);
                    dataset = new DataSet();
                    dataset.Reset();
                    dataadapter.Fill(dataset);
                    dataGridView1.DataSource = dataset.Tables[0];
     
                }
                con.Close();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error nr.5! " + ex.Message);
                return;                                                   //               ERROR 5
            }
                      
        }

       // Change data in dataBase
        private void button2_Click(object sender, EventArgs e)
        {
            //******** loop cheking, if it was entered column "id" *************************************
            for (int i = 0; i < dataset.Tables[0].Rows.Count; i++ )
            {
                if (dataset.Tables[0].Rows[i].RowState == DataRowState.Added ) 
                {
                    try
                    {   
                        //cheking if fist fild was filled and int is not last row.
                        if (dataGridView1[0, i].Value.ToString().Equals(String.Empty) && dataGridView1.Rows[i].IsNewRow == false)
                        {
                            //string b = dataset.Tables[0].Rows[i].RowState.ToString(); 
                            //textBox1.Text = i + " - " + b;
                            MessageBox.Show("MARCO! I told you, fill first fild - id");
                            return;
                        }
                      

                    }
                    // catch if it wasnt new row, user mistakly press in new fild, but didnt write anything.
                    catch (NullReferenceException ex)
                    {
                        MessageBox.Show("Error nr. 6! "+ ex.Message); //                      ERROR 6
                        dataset.Tables[0].Rows[i].RejectChanges ();
                    }
                    
                    catch (IndexOutOfRangeException ex)
                    {
                        MessageBox.Show("Error nr.7! " + ex.Message); //                       ERROR 7    
                    }
                    
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error nr.8! " + ex.Message); //                       ERROR 8
                    }
                      
                }
            }
            //***********************************************************************************************
            try
            {
                con.Open();
                builder = new SQLiteCommandBuilder(dataadapter);
                
                // cheking if it was any changes in the data
                if (dataadapter.Update(dataset) == 0)
                {
                    MessageBox.Show("There was no changes, Ibarra!  ");
                    con.Close();
                    return;
                }
                
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Error nr.9! " + ex.Message ); //                              ERROR 9
                con.Close();
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show ("Error nr.10!  " + ex.Message ); //                          ERROR 10
                con.Close();
                return;
            }

            MessageBox.Show("Information update! Good job, Marco Antonio!");
            con.Close();
            
        }


        // show important tasks
        private void button3_Click(object sender, EventArgs e)
        {
            button3.BackColor = Color.LightBlue; // Honeydew
            button1.BackColor = Color.Gainsboro;
            
            // date today
            DateTime thisDay = DateTime.Today; 
            sSql = @"select * from  tasks where SluttDato = '" + thisDay.ToString("dd.MM.yyyy") + @"'";
       
            try
            {
                con.Open();
                using (SQLiteCommand sqlCommand = con.CreateCommand())
                {
                    dataadapter = new SQLiteDataAdapter(sSql, con);
                    dataset = new DataSet();
                    dataset.Reset();
                    dataadapter.Fill(dataset);
                    dataGridView1.DataSource = dataset.Tables[0];
                }
                con.Close();
            }
            
            catch (Exception ex)
            {
                MessageBox.Show("Error nr.11! " + ex.Message); //                                ERROR 11
            }
 
        }
        
        /* event which happend if it was end of editing datarGrid cell
        1. change 'SlutDato' if it was any changes in the 'StartDato' column
        2. change 'Plassering' if user enter number from 1 to 7
        */
        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // index of 'Plassering' column
            int indexP = dataGridView1.Columns["Plassering"].Index; 

            // index of 'StartDato' column
            int index = dataGridView1.Columns["StartDato"].Index;
            
            // StarDato changed
            if (e.ColumnIndex == index) // index of Start dato !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
            {
                try
                {
                    string d = (String)dataGridView1[e.ColumnIndex, e.RowIndex].Value;
                    DateTime last1 = DateTime.Parse(d); 
                    //time interval
                    System.TimeSpan duration1 = new System.TimeSpan(4, 0, 0, 0);
                    //summ of dates
                    System.DateTime answer1 = last1.Add(duration1);
                    // string which lagre last date of task
                    string lDate = answer1.ToString("dd.MM.yyyy");
                    // put value to the SluttDato cell 
                    dataGridView1["SluttDato", e.RowIndex].Value = lDate;
                }
                catch (FormatException exep)
                {
                    MessageBox.Show("Error nr. 12" + exep.Message);                               //ERROR 12
                    return;
                }

                catch (InvalidCastException ex)
                {
                   MessageBox.Show("Error nr.13. Date did not choose ");                          //ERROR 13
                   return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error nr. 14! " + ex.Message);                                //ERROR 14
                    return;
                }
            }
            // end of edit column 'Plassering'
            else if (e.ColumnIndex == indexP)
            {
                String[] Plass = { "For Øst", "For Vest", "For Pederveien 19", "For Grenseveien 31", "For Midtveien 12,", "Dusashvis", "Oslo Gjestebolig" };
                // value, which was entered in the 'Plassering' column
                string chek = dataGridView1[indexP, e.RowIndex].Value.ToString();
                switch (chek)
                {
                    case "1":
                        dataGridView1[indexP, e.RowIndex].Value = Plass[0];
                        break;
                    case "2":
                        dataGridView1[indexP, e.RowIndex].Value = Plass[1];
                        break;
                    case "3":
                        dataGridView1[indexP, e.RowIndex].Value = Plass[2];
                        break;
                    case "4":
                        dataGridView1[indexP, e.RowIndex].Value = Plass[3];
                        break;
                    case "5":
                        dataGridView1[indexP, e.RowIndex].Value = Plass[4];
                        break;
                    case "6":
                        dataGridView1[indexP, e.RowIndex].Value = Plass[5];
                        break;
                    case "7":
                        dataGridView1[indexP, e.RowIndex].Value = Plass[6];
                        break;
                    default:
                        dataGridView1[indexP, e.RowIndex].Value = " ";
                        break;
                }
            }
        }
                
       
        // Start of editing cell in datagridView: 'SluttDato', 'ArbBokstav'
        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int index = dataGridView1.Columns["SluttDato"].Index;
            int indexM = dataGridView1.Columns["M"].Index;
           
            // SluttDato Changed
            if (e.ColumnIndex == index) 
            {
                MessageBox.Show("Error nr.15! Marco! Forget again?! You can not edit this column"); //                           ERROR 15
                return;
            }
            // ArbBokstav changed
            else if (e.ColumnIndex == indexM)
            {
                dataGridView1[indexM, e.RowIndex].Value = "M";
            }
         }
 

        // save file in csv format which you can open in excel
        private void button5_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {   
                // name of file
                string namen = saveFileDialog1.FileName; 
                // temporary storing
                string tekst = String.Empty; 
                // temporary string, wich summarize tekst with sign ;
                string tempTekst; 
                FileStream file1 = null;
                StreamWriter writer = null;

                try
                {
                    file1 = new FileStream(namen, FileMode.Append);
                    writer = new StreamWriter(file1);       
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error nr.16! " + ex.Message);                      // ERROR 16
                }
  
                mydb = new sqliteclass();
                sSql = "select * from  tasks";
                DataRow[] datarows = mydb.drExecute(sPath, sSql);
                if (datarows == null)
                {
                    MessageBox.Show("Error nr.17 of reading data base!");               //ERROR 17 
                    mydb = null;
                    return;
                }
                
                //write columns names in the file// temporary table to store data from db
                DataTable table2 = datarows[0].Table; 
                
                // write string 
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
                
                // write all rows to the file
                for (int i = 0; i < k; i++) // row
                {
                    for (int j = 0; j < n; j++)  // сolumn
                    {
                        tempTekst = datarows[i].ItemArray[j].ToString();
                        tekst = tempTekst + semicolon;
                        teksten += tekst;

                    }
                    writer.WriteLine(teksten);
                    teksten = String.Empty;
                }

                writer.Close(); 

            }
            // if file wasnt choose
            else
            {
                return;
            }

        }

        // export file from csv
        private void button6_Click(object sender, EventArgs e)
        {
            
            StreamReader reader;
            string read = String.Empty;
            
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName == String.Empty)
            {
                MessageBox.Show("Please, choose csv-file!");
                return;
            }

            try
            {
                reader = new StreamReader(openFileDialog1.FileName);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show("Error nr. 18! " + ex.Message);                         // ERROR 18
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error nr. 19! " + ex.Message);                         //ERROR 19
                return;
            }
            
            #region Get columns name
            // Trying to get columns names to adding into sql expression 
            mydb = new sqliteclass();
            sSql = "select * from  tasks";
            DataRow[] datarows = mydb.drExecute(sPath, sSql);
            if (datarows == null)
            {
                MessageBox.Show("Error nr.20 of reading data base!");                      //ERROR 20 
                mydb = null;
                return;
            }
            mydb = null;
            string ColName = String.Empty;
            // temporary table to store data from db
            DataTable table2 = datarows[0].Table; 
            string str = String.Empty;

            for (int i = 1; i < table2.Columns.Count; i++)
            {
                if (i < table2.Columns.Count - 1)
                {
                    str = table2.Columns[i].ToString() + ",";
                }
                else
                    str = table2.Columns[i].ToString ();
                // ColName contain: ArbBokstav,ArbeidsOrder ,IOnr, Info, Plassering, BlokkEtg ,BranndetektorNr,Brannsentral, StartDato,SluttDato , Status , Timer
                ColName += str;  
            }
            #endregion

            // read first line, with columns names,but not write this in db
            reader.ReadLine (); 
            
            mydb = new sqliteclass();
            // loop wich read line from csv and write this into database.
            while (reader.EndOfStream != true)
            {
                // read one line and save in temporary string variable
                read = reader.ReadLine(); 
            
                // Using regular expression,  changes string, which will matches the tekst after "Values " in sql expression 
                #region regex

                //replase ; to ,
                string resul = Regex.Replace(read, @"([^;]+);{1}", @"'$1',");

                //rest of ; replace with null spase
                string res = Regex.Replace(resul, @";{1}", @"' ',");

                //deleted first fild id looks like 1; or 2;
                string resalt = Regex.Replace(res, @"^('{1})(\d+)('{1})(,{1})", " ");

                // delete last coma ,
                string resalte = Regex.Replace(resalt, @",$", " "); // looks like this:  'M','0',' ','antonio','M',' ',' ',' ',' ',' ',' ',' ' 

                #endregion
            
                sSql = @"INSERT INTO tasks (" + ColName + @") VALUES (" + resalte + @");";    //looks like this : sSql = @"INSERT INTO tasks (ArbBokstav,ArbeidsOrder ,IOnr, Info, Plassering, BlokkEtg ,BranndetektorNr,Brannsentral, StartDato,SluttDato , Status , Timer) VALUES ('M', '2017' ,'5', 'levliukh angel', ' ', '9' ,'301','8', '13.08.2014 ',' ' , 'Ferdig' , ' ');";
                
                if (mydb.iExecuteNonQuery(sPath, sSql, 1) == 0)
                {
                    MessageBox.Show("Fail of importing row: " + sSql);
                }
                else
                {
                    MessageBox.Show("Sucsess!");
                }
            } 
        
       }

        // FAIL of incorrect data in datagridview, example - null in datagridCombox value.
        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            if (dialogres == 1)
            {
                return;
            }
            DialogResult result;
            
            result = MessageBox.Show ("Fail"+anError.Context + ".Please, change data in column: "+ anError.ColumnIndex + "@ in the row:" + anError.RowIndex + @"! Press OK, if you not want to see this message again.", "Fail!", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                dialogres = 1;
                return;
            }
            else
            {
                return;
            }
           
        }


        // edit database table .. Form2
        private void button4_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
        }

        // FORM CLOSING
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            var result = MessageBox.Show("Marco, are you sure you want to leave this wonderful program?", "Did you save changes?",  
                          MessageBoxButtons.YesNo,
                          MessageBoxIcon.Question);
            if (result != DialogResult.Yes) 
                e.Cancel = true; // следует ли отменить событие..в данном случае следует

        }

        

        

       

    }
}