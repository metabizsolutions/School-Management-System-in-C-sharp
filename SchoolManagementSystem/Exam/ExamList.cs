﻿using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using MySql.Data.MySqlClient;
using SchoolManagementSystem.Datesheet;
using System;
using System.Data;
using System.Media;
using System.Windows.Forms;

namespace SchoolManagementSystem.Exam
{
    public partial class ExamList : DevExpress.XtraEditors.XtraUserControl
    {
        private static ExamList _instance;

        public static ExamList instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ExamList();
                return _instance;
            }
        }
        CommonFunctions fun = new CommonFunctions();
        public ExamList()
        {
            InitializeComponent();
            loadfunctions();
        }
        public void loadfunctions()
        {
            FillGridExam();
        }
        public void permissions_btns(bool add, bool Edit, bool Delete)
        {
            btnEAdd.Enabled = false;
            if (add)
            {
                btnEAdd.Enabled = true;
            }
            gridView1.OptionsBehavior.Editable = false;
            if (Edit)
            {
                gridView1.OptionsBehavior.Editable = true;
            }
            BtnEDelete.Enabled = false;
            if (Delete)
                BtnEDelete.Enabled = true;
        }
        private void btnEAdd_Click(object sender, EventArgs e)
        {
            if (txtDate.Text == "" || txtName.Text == "")
            {
                MessageBox.Show("Fill all fields....", "Info");
                return;
            }
            MySqlConnection con = new MySqlConnection(Login.constring);
            con.Open();
            try
            {
                DateTime d = Convert.ToDateTime(txtDate.Text);

                MySqlCommand cmd = new MySqlCommand("INSERT into exam(name,date,comment,sync,session_id,allow) VALUES('" + txtName.Text + "','" + (d.Year + "-" + d.Month + "-" + d.Day) + "','" + txtComment.Text + "','0','" + fun.GetSessionID(fun.GetDefaultSessionName()) + "','0');", con);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show(ex.Message, "Error");
                return;
            }
            con.Close();
            FillGridExam();
            empty();
        }
        private void empty()
        {
            txtName.Text = "";
            txtDate.Text = "";
            txtComment.Text = "";
        }

        public void FillGridExam()
        {
            MySqlConnection con = new MySqlConnection(Login.constring);
            con.Open();
            MySqlCommand cmdP = new MySqlCommand("SELECT * FROM exam where 1 = 1 order by exam.exam_id DESC;", con);
            MySqlDataAdapter adp = new MySqlDataAdapter(cmdP);
            DataTable table = new DataTable();
            adp.Fill(table);
            gridExamList.DataSource = CommonFunctions.AutoNumberedTable(table);
            gridView1.BestFitColumns();
            if (table.Rows.Count > 0)
            {
                var col = gridView1.Columns["exam_id"];
                col.OptionsColumn.ReadOnly = true;
                var col1 = gridView1.Columns["session_id"];
                col1.Visible = false;
                var col2 = gridView1.Columns["sync"];
                col2.Visible = false;
                GridColumn Column = gridView1.Columns["exam_id"];
                //Column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
                GridColumn Column3 = gridView1.Columns["Sr#"];
                //Column3.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;

                RepositoryItemDateEdit riCombo = new RepositoryItemDateEdit();
                gridView1.Columns["date"].ColumnEdit = riCombo;

            }
            con.Close();
            /*
            if (Main_FD.SelectedSession != fun.GetDefaultSessionName())
            {
                btnEAdd.Enabled = false;
                BtnEDelete.Enabled = false;
                gridView1.OptionsBehavior.Editable = false;
            }
            else
            {
                btnEAdd.Enabled = true;
                BtnEDelete.Enabled = true;
                gridView1.OptionsBehavior.Editable = true;
            }
            */
        }

        private void gridView1_RowUpdated(object sender, DevExpress.XtraGrid.Views.Base.RowObjectEventArgs e)
        {
            DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
            MySqlConnection con = new MySqlConnection(Login.constring);
            con.Open();
            DateTime d = Convert.ToDateTime(row["date"]);
            var Allow = (Convert.ToBoolean(row["allow"]) == true) ? 1 : 0;
            //string date = DateTime.Now.ToString("yyyy-MM-dd");
            MySqlCommand cmd = new MySqlCommand("UPDATE exam set name='" + row["name"] + "',date='" + d.ToString("yyyy-MM-dd") + "',comment='" + Convert.ToString(row["comment"]) + "',sync='0',allow='" + Allow + "' WHERE exam_id='" + row["exam_id"] + "';", con);
            cmd.ExecuteNonQuery();
            con.Close();
            FillGridExam();
        }

        private void BtnEDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure to delete?", "Confirm Delete!!", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    DataRow row = gridView1.GetDataRow(gridView1.FocusedRowHandle);
                    MySqlConnection con = new MySqlConnection(Login.constring);
                    con.Open();
                    MySqlCommand cmdM = new MySqlCommand("DELETE from exam WHERE exam_id='" + row[1] + "';", con);
                    cmdM.ExecuteNonQuery();
                    con.Close();
                    FillGridExam();
                }
            }
            catch (Exception ex)
            {
                SystemSounds.Hand.Play();
                MessageBox.Show(ex.Message, "Info");
            }
        }


    

            private void button1_Click(object sender, EventArgs e)
            {
                Form1 form1 = new Form1();
                form1.Show(); 
            }
        }
    }


