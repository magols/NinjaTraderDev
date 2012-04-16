namespace EcoNewsControl
{
    public partial class EcoNewsControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle27 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle21 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle22 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle23 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle24 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle25 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle26 = new System.Windows.Forms.DataGridViewCellStyle();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.newsTab = new System.Windows.Forms.TabPage();
            this.newsList = new System.Windows.Forms.DataGridView();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Impact = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Country = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Previous = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Forecast = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Alert = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TimeLeft = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonUpdateNow = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelLastUpdate = new System.Windows.Forms.Label();
            this.settingsTab = new System.Windows.Forms.TabPage();
            this.buttonLoadDefaults = new System.Windows.Forms.Button();
            this.buttonSaveDefaults = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonLookupAlertFile = new System.Windows.Forms.Button();
            this.textBoxAlertFile = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.alertUpDown = new System.Windows.Forms.NumericUpDown();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonApply = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.refreshUpDown = new System.Windows.Forms.NumericUpDown();
            this.checkTodaysNews = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkedListBoxCountries = new System.Windows.Forms.CheckedListBox();
            this.impactGroup = new System.Windows.Forms.GroupBox();
            this.impactCheckLow = new System.Windows.Forms.CheckBox();
            this.impactCheckMedium = new System.Windows.Forms.CheckBox();
            this.impactCheckHigh = new System.Windows.Forms.CheckBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.mainTabControl.SuspendLayout();
            this.newsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newsList)).BeginInit();
            this.settingsTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.refreshUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.impactGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.newsTab);
            this.mainTabControl.Controls.Add(this.settingsTab);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Multiline = true;
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.Padding = new System.Drawing.Point(3, 3);
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(900, 150);
            this.mainTabControl.TabIndex = 0;
            // 
            // newsTab
            // 
            this.newsTab.Controls.Add(this.newsList);
            this.newsTab.Controls.Add(this.buttonUpdateNow);
            this.newsTab.Controls.Add(this.label4);
            this.newsTab.Controls.Add(this.label3);
            this.newsTab.Controls.Add(this.label2);
            this.newsTab.Controls.Add(this.labelLastUpdate);
            this.newsTab.Location = new System.Drawing.Point(4, 22);
            this.newsTab.Name = "newsTab";
            this.newsTab.Padding = new System.Windows.Forms.Padding(3);
            this.newsTab.Size = new System.Drawing.Size(892, 124);
            this.newsTab.TabIndex = 0;
            this.newsTab.Text = "News";
            this.newsTab.UseVisualStyleBackColor = true;
            // 
            // newsList
            // 
            this.newsList.AllowUserToAddRows = false;
            this.newsList.AllowUserToDeleteRows = false;
            this.newsList.AllowUserToResizeRows = false;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.newsList.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle19;
            this.newsList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newsList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.newsList.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.newsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.newsList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.Impact,
            this.Country,
            this.Previous,
            this.Forecast,
            this.Description,
            this.Alert,
            this.TimeLeft});
            dataGridViewCellStyle27.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle27.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle27.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle27.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle27.SelectionBackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle27.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle27.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.newsList.DefaultCellStyle = dataGridViewCellStyle27;
            this.newsList.GridColor = System.Drawing.Color.White;
            this.newsList.Location = new System.Drawing.Point(85, 3);
            this.newsList.Name = "newsList";
            this.newsList.ReadOnly = true;
            this.newsList.RowHeadersVisible = false;
            this.newsList.RowTemplate.Height = 18;
            this.newsList.Size = new System.Drawing.Size(802, 111);
            this.newsList.TabIndex = 6;
            this.newsList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.newsList_CellContentClick);
            // 
            // Time
            // 
            this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle20.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle20.NullValue = null;
            this.Time.DefaultCellStyle = dataGridViewCellStyle20;
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Time.Width = 36;
            // 
            // Impact
            // 
            this.Impact.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle21.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Impact.DefaultCellStyle = dataGridViewCellStyle21;
            this.Impact.HeaderText = "Impact";
            this.Impact.Name = "Impact";
            this.Impact.ReadOnly = true;
            this.Impact.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Impact.Width = 45;
            // 
            // Country
            // 
            this.Country.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle22.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Country.DefaultCellStyle = dataGridViewCellStyle22;
            this.Country.HeaderText = "Country";
            this.Country.Name = "Country";
            this.Country.ReadOnly = true;
            this.Country.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Country.Width = 49;
            // 
            // Previous
            // 
            this.Previous.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle23.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Previous.DefaultCellStyle = dataGridViewCellStyle23;
            this.Previous.HeaderText = "Previous";
            this.Previous.Name = "Previous";
            this.Previous.ReadOnly = true;
            this.Previous.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Previous.Width = 54;
            // 
            // Forecast
            // 
            this.Forecast.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle24.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Forecast.DefaultCellStyle = dataGridViewCellStyle24;
            this.Forecast.HeaderText = "Forecast";
            this.Forecast.Name = "Forecast";
            this.Forecast.ReadOnly = true;
            this.Forecast.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Forecast.Width = 54;
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle25.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Description.DefaultCellStyle = dataGridViewCellStyle25;
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            this.Description.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Description.Width = 66;
            // 
            // Alert
            // 
            this.Alert.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Alert.FalseValue = "Falses";
            this.Alert.HeaderText = "Alert";
            this.Alert.Name = "Alert";
            this.Alert.ReadOnly = true;
            this.Alert.TrueValue = "Trues";
            this.Alert.Width = 34;
            // 
            // TimeLeft
            // 
            this.TimeLeft.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle26.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.TimeLeft.DefaultCellStyle = dataGridViewCellStyle26;
            this.TimeLeft.HeaderText = "Time Left";
            this.TimeLeft.Name = "TimeLeft";
            this.TimeLeft.ReadOnly = true;
            this.TimeLeft.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TimeLeft.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // buttonUpdateNow
            // 
            this.buttonUpdateNow.Location = new System.Drawing.Point(6, 84);
            this.buttonUpdateNow.Name = "buttonUpdateNow";
            this.buttonUpdateNow.Size = new System.Drawing.Size(75, 23);
            this.buttonUpdateNow.TabIndex = 5;
            this.buttonUpdateNow.Text = "Update Now";
            this.buttonUpdateNow.UseVisualStyleBackColor = true;
            this.buttonUpdateNow.Click += new System.EventHandler(this.buttonUpdateNow_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(7, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "Next Update in:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(7, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "Next Update:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Green;
            this.label2.Location = new System.Drawing.Point(6, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "Time of update";
            // 
            // labelLastUpdate
            // 
            this.labelLastUpdate.AutoSize = true;
            this.labelLastUpdate.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLastUpdate.ForeColor = System.Drawing.Color.Green;
            this.labelLastUpdate.Location = new System.Drawing.Point(7, 7);
            this.labelLastUpdate.Name = "labelLastUpdate";
            this.labelLastUpdate.Size = new System.Drawing.Size(70, 12);
            this.labelLastUpdate.TabIndex = 1;
            this.labelLastUpdate.Text = "Last Update:";
            // 
            // settingsTab
            // 
            this.settingsTab.Controls.Add(this.buttonLoadDefaults);
            this.settingsTab.Controls.Add(this.buttonSaveDefaults);
            this.settingsTab.Controls.Add(this.label6);
            this.settingsTab.Controls.Add(this.buttonLookupAlertFile);
            this.settingsTab.Controls.Add(this.textBoxAlertFile);
            this.settingsTab.Controls.Add(this.label5);
            this.settingsTab.Controls.Add(this.alertUpDown);
            this.settingsTab.Controls.Add(this.buttonCancel);
            this.settingsTab.Controls.Add(this.buttonApply);
            this.settingsTab.Controls.Add(this.label1);
            this.settingsTab.Controls.Add(this.refreshUpDown);
            this.settingsTab.Controls.Add(this.checkTodaysNews);
            this.settingsTab.Controls.Add(this.groupBox1);
            this.settingsTab.Controls.Add(this.impactGroup);
            this.settingsTab.Location = new System.Drawing.Point(4, 22);
            this.settingsTab.Name = "settingsTab";
            this.settingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.settingsTab.Size = new System.Drawing.Size(892, 124);
            this.settingsTab.TabIndex = 1;
            this.settingsTab.Text = "Settings";
            this.settingsTab.UseVisualStyleBackColor = true;
            // 
            // buttonLoadDefaults
            // 
            this.buttonLoadDefaults.Location = new System.Drawing.Point(412, 39);
            this.buttonLoadDefaults.Name = "buttonLoadDefaults";
            this.buttonLoadDefaults.Size = new System.Drawing.Size(96, 23);
            this.buttonLoadDefaults.TabIndex = 13;
            this.buttonLoadDefaults.Text = "Load Defaults";
            this.buttonLoadDefaults.UseVisualStyleBackColor = true;
            this.buttonLoadDefaults.Click += new System.EventHandler(this.buttonLoadDefaults_Click);
            // 
            // buttonSaveDefaults
            // 
            this.buttonSaveDefaults.Location = new System.Drawing.Point(412, 10);
            this.buttonSaveDefaults.Name = "buttonSaveDefaults";
            this.buttonSaveDefaults.Size = new System.Drawing.Size(96, 23);
            this.buttonSaveDefaults.TabIndex = 12;
            this.buttonSaveDefaults.Text = "Save Defaults";
            this.buttonSaveDefaults.UseVisualStyleBackColor = true;
            this.buttonSaveDefaults.Click += new System.EventHandler(this.buttonSaveDefaults_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(409, 80);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Alert File (wav):";
            // 
            // buttonLookupAlertFile
            // 
            this.buttonLookupAlertFile.Location = new System.Drawing.Point(583, 98);
            this.buttonLookupAlertFile.Name = "buttonLookupAlertFile";
            this.buttonLookupAlertFile.Size = new System.Drawing.Size(29, 20);
            this.buttonLookupAlertFile.TabIndex = 10;
            this.buttonLookupAlertFile.Text = "...";
            this.buttonLookupAlertFile.UseVisualStyleBackColor = true;
            this.buttonLookupAlertFile.Click += new System.EventHandler(this.buttonLookupAlertFile_Click);
            // 
            // textBoxAlertFile
            // 
            this.textBoxAlertFile.AcceptsReturn = true;
            this.textBoxAlertFile.Location = new System.Drawing.Point(412, 98);
            this.textBoxAlertFile.Name = "textBoxAlertFile";
            this.textBoxAlertFile.Size = new System.Drawing.Size(165, 20);
            this.textBoxAlertFile.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(249, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Alert Trigger (minutes)";
            // 
            // alertUpDown
            // 
            this.alertUpDown.Location = new System.Drawing.Point(199, 97);
            this.alertUpDown.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.alertUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.alertUpDown.Name = "alertUpDown";
            this.alertUpDown.Size = new System.Drawing.Size(44, 20);
            this.alertUpDown.TabIndex = 7;
            this.alertUpDown.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(537, 39);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonApply
            // 
            this.buttonApply.Location = new System.Drawing.Point(537, 10);
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.Size = new System.Drawing.Size(75, 23);
            this.buttonApply.TabIndex = 5;
            this.buttonApply.Text = "Apply";
            this.buttonApply.UseVisualStyleBackColor = true;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(249, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Refresh Interval (minutes)";
            // 
            // refreshUpDown
            // 
            this.refreshUpDown.Location = new System.Drawing.Point(199, 54);
            this.refreshUpDown.Maximum = new decimal(new int[] {
            1440,
            0,
            0,
            0});
            this.refreshUpDown.Minimum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.refreshUpDown.Name = "refreshUpDown";
            this.refreshUpDown.Size = new System.Drawing.Size(44, 20);
            this.refreshUpDown.TabIndex = 3;
            this.refreshUpDown.Value = new decimal(new int[] {
            720,
            0,
            0,
            0});
            // 
            // checkTodaysNews
            // 
            this.checkTodaysNews.AutoSize = true;
            this.checkTodaysNews.Location = new System.Drawing.Point(199, 14);
            this.checkTodaysNews.Name = "checkTodaysNews";
            this.checkTodaysNews.Size = new System.Drawing.Size(117, 17);
            this.checkTodaysNews.TabIndex = 2;
            this.checkTodaysNews.Text = "Today\'s News Only";
            this.checkTodaysNews.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkedListBoxCountries);
            this.groupBox1.Location = new System.Drawing.Point(86, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(98, 111);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Countries";
            // 
            // checkedListBoxCountries
            // 
            this.checkedListBoxCountries.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBoxCountries.CheckOnClick = true;
            this.checkedListBoxCountries.FormattingEnabled = true;
            this.checkedListBoxCountries.Items.AddRange(new object[] {
            "USD",
            "EUR",
            "JPY",
            "GBP",
            "CHF",
            "AUD",
            "CAD",
            "CNY",
            "NZD"});
            this.checkedListBoxCountries.Location = new System.Drawing.Point(6, 17);
            this.checkedListBoxCountries.Margin = new System.Windows.Forms.Padding(3, 9, 3, 0);
            this.checkedListBoxCountries.Name = "checkedListBoxCountries";
            this.checkedListBoxCountries.Size = new System.Drawing.Size(84, 90);
            this.checkedListBoxCountries.TabIndex = 0;
            // 
            // impactGroup
            // 
            this.impactGroup.Controls.Add(this.impactCheckLow);
            this.impactGroup.Controls.Add(this.impactCheckMedium);
            this.impactGroup.Controls.Add(this.impactCheckHigh);
            this.impactGroup.Location = new System.Drawing.Point(6, 7);
            this.impactGroup.Name = "impactGroup";
            this.impactGroup.Size = new System.Drawing.Size(74, 111);
            this.impactGroup.TabIndex = 0;
            this.impactGroup.TabStop = false;
            this.impactGroup.Text = "Impact";
            // 
            // impactCheckLow
            // 
            this.impactCheckLow.AutoSize = true;
            this.impactCheckLow.Checked = true;
            this.impactCheckLow.CheckState = System.Windows.Forms.CheckState.Checked;
            this.impactCheckLow.Location = new System.Drawing.Point(6, 88);
            this.impactCheckLow.Name = "impactCheckLow";
            this.impactCheckLow.Size = new System.Drawing.Size(46, 17);
            this.impactCheckLow.TabIndex = 2;
            this.impactCheckLow.Text = "Low";
            this.impactCheckLow.UseVisualStyleBackColor = true;
            // 
            // impactCheckMedium
            // 
            this.impactCheckMedium.AutoSize = true;
            this.impactCheckMedium.Checked = true;
            this.impactCheckMedium.CheckState = System.Windows.Forms.CheckState.Checked;
            this.impactCheckMedium.Location = new System.Drawing.Point(7, 54);
            this.impactCheckMedium.Name = "impactCheckMedium";
            this.impactCheckMedium.Size = new System.Drawing.Size(63, 17);
            this.impactCheckMedium.TabIndex = 1;
            this.impactCheckMedium.Text = "Medium";
            this.impactCheckMedium.UseVisualStyleBackColor = true;
            // 
            // impactCheckHigh
            // 
            this.impactCheckHigh.AutoSize = true;
            this.impactCheckHigh.Checked = true;
            this.impactCheckHigh.CheckState = System.Windows.Forms.CheckState.Checked;
            this.impactCheckHigh.Location = new System.Drawing.Point(7, 20);
            this.impactCheckHigh.Name = "impactCheckHigh";
            this.impactCheckHigh.Size = new System.Drawing.Size(48, 17);
            this.impactCheckHigh.TabIndex = 0;
            this.impactCheckHigh.Text = "High";
            this.impactCheckHigh.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "wav files|*.wav";
            this.openFileDialog1.Title = "Select Alert File";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.BackColor = System.Drawing.Color.Transparent;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Indigo;
            this.label7.Location = new System.Drawing.Point(123, 1);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(160, 15);
            this.label7.TabIndex = 1;
            this.label7.Text = "Created by TradingStudies.com";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Times New Roman", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Indigo;
            this.label8.Location = new System.Drawing.Point(333, 1);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(151, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "News from ForexFactory.com";
            // 
            // EcoNewsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.mainTabControl);
            this.MinimumSize = new System.Drawing.Size(4, 150);
            this.Name = "EcoNewsControl";
            this.Size = new System.Drawing.Size(900, 150);
            this.mainTabControl.ResumeLayout(false);
            this.newsTab.ResumeLayout(false);
            this.newsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.newsList)).EndInit();
            this.settingsTab.ResumeLayout(false);
            this.settingsTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alertUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.refreshUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.impactGroup.ResumeLayout(false);
            this.impactGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage newsTab;
        private System.Windows.Forms.TabPage settingsTab;
        private System.Windows.Forms.GroupBox impactGroup;
        private System.Windows.Forms.CheckBox impactCheckLow;
        private System.Windows.Forms.CheckBox impactCheckMedium;
        private System.Windows.Forms.CheckBox impactCheckHigh;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckedListBox checkedListBoxCountries;
        private System.Windows.Forms.CheckBox checkTodaysNews;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonApply;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown refreshUpDown;
        private System.Windows.Forms.Label labelLastUpdate;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonUpdateNow;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown alertUpDown;
        private System.Windows.Forms.TextBox textBoxAlertFile;
        private System.Windows.Forms.Button buttonLookupAlertFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonLoadDefaults;
        private System.Windows.Forms.Button buttonSaveDefaults;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView newsList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Impact;
        private System.Windows.Forms.DataGridViewTextBoxColumn Country;
        private System.Windows.Forms.DataGridViewTextBoxColumn Previous;
        private System.Windows.Forms.DataGridViewTextBoxColumn Forecast;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Alert;
        private System.Windows.Forms.DataGridViewTextBoxColumn TimeLeft;
    }
}
