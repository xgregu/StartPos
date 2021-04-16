namespace StartPos.Setup
{
    partial class SetupForm
    {
        /// <summary>
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod generowany przez Projektanta formularzy systemu Windows

        /// <summary>
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetupForm));
            this.MainTab = new System.Windows.Forms.TabControl();
            this.Setup = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.convertButton = new System.Windows.Forms.PictureBox();
            this.saveButton = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.informacje_SerializacjaOpis = new System.Windows.Forms.Label();
            this.informacje_wersjaJava = new System.Windows.Forms.TextBox();
            this.label910 = new System.Windows.Forms.Label();
            this.infoSerial = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.automatyczneAktualizacje_insoft = new System.Windows.Forms.CheckBox();
            this.inne_serwerPcMarket = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.inne_sklepCalodobowy = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.inne_monitorKlienta = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.inne_stanowisko = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.sql_GroupBox = new System.Windows.Forms.GroupBox();
            this.sql_instancja = new System.Windows.Forms.ComboBox();
            this.sql_sprawdzPolaczenieWynik = new System.Windows.Forms.Label();
            this.sql_serwer = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.sql_pokazHaslo = new System.Windows.Forms.RadioButton();
            this.sql_port = new System.Windows.Forms.NumericUpDown();
            this.sql_haslo = new System.Windows.Forms.TextBox();
            this.sql_baza = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.backup_groupBox = new System.Windows.Forms.GroupBox();
            this.backup_dodatkowy_groupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.IsAlternativeBackup = new System.Windows.Forms.CheckBox();
            this.backup_dodatkowy_Sciezka = new System.Windows.Forms.TextBox();
            this.backupGlowny_groupBox = new System.Windows.Forms.GroupBox();
            this.backup_glowny_sciezka_label = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.backup_glowny_Sciezka = new System.Windows.Forms.TextBox();
            this.daneKlienta_groupBox = new System.Windows.Forms.GroupBox();
            this.usText = new System.Windows.Forms.Label();
            this.daneKlienta_lokalizacja_textbox = new System.Windows.Forms.TextBox();
            this.daneKlienta_nazwaFirmy = new System.Windows.Forms.TextBox();
            this.lokalizacja_label = new System.Windows.Forms.Label();
            this.nazwaFirmy_label = new System.Windows.Forms.Label();
            this.Log = new System.Windows.Forms.TabPage();
            this.rbLogSetup = new System.Windows.Forms.RichTextBox();
            this.MainTab.SuspendLayout();
            this.Setup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.convertButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.saveButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.sql_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sql_port)).BeginInit();
            this.backup_groupBox.SuspendLayout();
            this.backup_dodatkowy_groupBox.SuspendLayout();
            this.backupGlowny_groupBox.SuspendLayout();
            this.daneKlienta_groupBox.SuspendLayout();
            this.Log.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainTab
            // 
            this.MainTab.Controls.Add(this.Setup);
            this.MainTab.Controls.Add(this.Log);
            resources.ApplyResources(this.MainTab, "MainTab");
            this.MainTab.Name = "MainTab";
            this.MainTab.SelectedIndex = 0;
            // 
            // Setup
            // 
            this.Setup.Controls.Add(this.label9);
            this.Setup.Controls.Add(this.label4);
            this.Setup.Controls.Add(this.convertButton);
            this.Setup.Controls.Add(this.saveButton);
            this.Setup.Controls.Add(this.pictureBox8);
            this.Setup.Controls.Add(this.pictureBox7);
            this.Setup.Controls.Add(this.pictureBox6);
            this.Setup.Controls.Add(this.pictureBox1);
            this.Setup.Controls.Add(this.groupBox1);
            this.Setup.Controls.Add(this.groupBox2);
            this.Setup.Controls.Add(this.sql_GroupBox);
            this.Setup.Controls.Add(this.backup_groupBox);
            this.Setup.Controls.Add(this.daneKlienta_groupBox);
            resources.ApplyResources(this.Setup, "Setup");
            this.Setup.Name = "Setup";
            this.Setup.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(159)))), ((int)(((byte)(60)))));
            this.label9.CausesValidation = false;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Name = "label9";
            this.label9.Click += new System.EventHandler(this.OnConvertButtonClick);
            this.label9.MouseLeave += new System.EventHandler(this.OnConvertButtonMouseLeave);
            this.label9.MouseHover += new System.EventHandler(this.OnConvertButtonMouseHover);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(159)))), ((int)(((byte)(60)))));
            this.label4.CausesValidation = false;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Name = "label4";
            this.label4.Click += new System.EventHandler(this.OnConvertButtonClick);
            this.label4.MouseLeave += new System.EventHandler(this.OnConvertButtonMouseLeave);
            this.label4.MouseHover += new System.EventHandler(this.OnConvertButtonMouseHover);
            // 
            // convertButton
            // 
            this.convertButton.Image = global::StartPos.Properties.Resources.convert;
            resources.ApplyResources(this.convertButton, "convertButton");
            this.convertButton.Name = "convertButton";
            this.convertButton.TabStop = false;
            this.convertButton.Click += new System.EventHandler(this.OnConvertButtonClick);
            this.convertButton.MouseLeave += new System.EventHandler(this.OnConvertButtonMouseLeave);
            this.convertButton.MouseHover += new System.EventHandler(this.OnConvertButtonMouseHover);
            // 
            // saveButton
            // 
            this.saveButton.Image = global::StartPos.Properties.Resources.Save;
            resources.ApplyResources(this.saveButton, "saveButton");
            this.saveButton.Name = "saveButton";
            this.saveButton.TabStop = false;
            this.saveButton.Click += new System.EventHandler(this.OnSaveButtonClick);
            this.saveButton.MouseLeave += new System.EventHandler(this.OnSaveButtonMouseLeave);
            this.saveButton.MouseHover += new System.EventHandler(this.OnSaveButtonMouseHover);
            // 
            // pictureBox8
            // 
            this.pictureBox8.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.pictureBox8.Image = global::StartPos.Properties.Resources.AnimatedCircles;
            resources.ApplyResources(this.pictureBox8, "pictureBox8");
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.pictureBox7.Image = global::StartPos.Properties.Resources.AnimatedCircles;
            resources.ApplyResources(this.pictureBox7, "pictureBox7");
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.AccessibleRole = System.Windows.Forms.AccessibleRole.ProgressBar;
            this.pictureBox6.Image = global::StartPos.Properties.Resources.AnimatedCircles;
            resources.ApplyResources(this.pictureBox6, "pictureBox6");
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.pictureBox1.Image = global::StartPos.Properties.Resources.StartPos_COMA_Bialy;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.informacje_SerializacjaOpis);
            this.groupBox1.Controls.Add(this.informacje_wersjaJava);
            this.groupBox1.Controls.Add(this.label910);
            this.groupBox1.Controls.Add(this.infoSerial);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // informacje_SerializacjaOpis
            // 
            resources.ApplyResources(this.informacje_SerializacjaOpis, "informacje_SerializacjaOpis");
            this.informacje_SerializacjaOpis.Name = "informacje_SerializacjaOpis";
            // 
            // informacje_wersjaJava
            // 
            resources.ApplyResources(this.informacje_wersjaJava, "informacje_wersjaJava");
            this.informacje_wersjaJava.Name = "informacje_wersjaJava";
            this.informacje_wersjaJava.ReadOnly = true;
            this.informacje_wersjaJava.TabStop = false;
            // 
            // label910
            // 
            resources.ApplyResources(this.label910, "label910");
            this.label910.Name = "label910";
            // 
            // infoSerial
            // 
            resources.ApplyResources(this.infoSerial, "infoSerial");
            this.infoSerial.Name = "infoSerial";
            this.infoSerial.ReadOnly = true;
            this.infoSerial.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.automatyczneAktualizacje_insoft);
            this.groupBox2.Controls.Add(this.inne_serwerPcMarket);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.inne_sklepCalodobowy);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.inne_monitorKlienta);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.inne_stanowisko);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // automatyczneAktualizacje_insoft
            // 
            resources.ApplyResources(this.automatyczneAktualizacje_insoft, "automatyczneAktualizacje_insoft");
            this.automatyczneAktualizacje_insoft.Name = "automatyczneAktualizacje_insoft";
            this.automatyczneAktualizacje_insoft.UseVisualStyleBackColor = true;
            // 
            // inne_serwerPcMarket
            // 
            resources.ApplyResources(this.inne_serwerPcMarket, "inne_serwerPcMarket");
            this.inne_serwerPcMarket.ForeColor = System.Drawing.SystemColors.WindowText;
            this.inne_serwerPcMarket.Name = "inne_serwerPcMarket";
            this.inne_serwerPcMarket.TabStop = false;
            this.inne_serwerPcMarket.TextChanged += new System.EventHandler(this.OnServerPcMarketHostTextChanged);
            // 
            // label15
            // 
            this.label15.AutoEllipsis = true;
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // inne_sklepCalodobowy
            // 
            resources.ApplyResources(this.inne_sklepCalodobowy, "inne_sklepCalodobowy");
            this.inne_sklepCalodobowy.Name = "inne_sklepCalodobowy";
            this.inne_sklepCalodobowy.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // inne_monitorKlienta
            // 
            resources.ApplyResources(this.inne_monitorKlienta, "inne_monitorKlienta");
            this.inne_monitorKlienta.Name = "inne_monitorKlienta";
            this.inne_monitorKlienta.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // inne_stanowisko
            // 
            resources.ApplyResources(this.inne_stanowisko, "inne_stanowisko");
            this.inne_stanowisko.Name = "inne_stanowisko";
            this.inne_stanowisko.ReadOnly = true;
            this.inne_stanowisko.TabStop = false;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // sql_GroupBox
            // 
            this.sql_GroupBox.Controls.Add(this.sql_instancja);
            this.sql_GroupBox.Controls.Add(this.sql_sprawdzPolaczenieWynik);
            this.sql_GroupBox.Controls.Add(this.sql_serwer);
            this.sql_GroupBox.Controls.Add(this.label3);
            this.sql_GroupBox.Controls.Add(this.sql_pokazHaslo);
            this.sql_GroupBox.Controls.Add(this.sql_port);
            this.sql_GroupBox.Controls.Add(this.sql_haslo);
            this.sql_GroupBox.Controls.Add(this.sql_baza);
            this.sql_GroupBox.Controls.Add(this.label12);
            this.sql_GroupBox.Controls.Add(this.label10);
            this.sql_GroupBox.Controls.Add(this.label8);
            this.sql_GroupBox.Controls.Add(this.label2);
            this.sql_GroupBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.sql_GroupBox, "sql_GroupBox");
            this.sql_GroupBox.Name = "sql_GroupBox";
            this.sql_GroupBox.TabStop = false;
            // 
            // sql_instancja
            // 
            resources.ApplyResources(this.sql_instancja, "sql_instancja");
            this.sql_instancja.FormattingEnabled = true;
            this.sql_instancja.Name = "sql_instancja";
            this.sql_instancja.DropDown += new System.EventHandler(this.OnSqlInstanceDropDown);
            this.sql_instancja.TextChanged += new System.EventHandler(this.OnSqlInstanceNameTextChanged);
            this.sql_instancja.Leave += new System.EventHandler(this.OnSqlInstanceNameTextLeave);
            // 
            // sql_sprawdzPolaczenieWynik
            // 
            resources.ApplyResources(this.sql_sprawdzPolaczenieWynik, "sql_sprawdzPolaczenieWynik");
            this.sql_sprawdzPolaczenieWynik.Name = "sql_sprawdzPolaczenieWynik";
            // 
            // sql_serwer
            // 
            resources.ApplyResources(this.sql_serwer, "sql_serwer");
            this.sql_serwer.Name = "sql_serwer";
            this.sql_serwer.Leave += new System.EventHandler(this.OnSqlHostTextChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // sql_pokazHaslo
            // 
            resources.ApplyResources(this.sql_pokazHaslo, "sql_pokazHaslo");
            this.sql_pokazHaslo.BackColor = System.Drawing.Color.Transparent;
            this.sql_pokazHaslo.Name = "sql_pokazHaslo";
            this.sql_pokazHaslo.TabStop = true;
            this.sql_pokazHaslo.UseVisualStyleBackColor = false;
            this.sql_pokazHaslo.CheckedChanged += new System.EventHandler(this.OnSqlShowPasswordCheckedChanged);
            this.sql_pokazHaslo.Click += new System.EventHandler(this.OnSqlShowPasswordClick);
            // 
            // sql_port
            // 
            resources.ApplyResources(this.sql_port, "sql_port");
            this.sql_port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.sql_port.Name = "sql_port";
            this.sql_port.Leave += new System.EventHandler(this.OnSqlPortValueChanged);
            // 
            // sql_haslo
            // 
            resources.ApplyResources(this.sql_haslo, "sql_haslo");
            this.sql_haslo.Name = "sql_haslo";
            this.sql_haslo.Leave += new System.EventHandler(this.OnSqlPasswordTextChanged);
            // 
            // sql_baza
            // 
            resources.ApplyResources(this.sql_baza, "sql_baza");
            this.sql_baza.Name = "sql_baza";
            this.sql_baza.ReadOnly = true;
            this.sql_baza.TabStop = false;
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // backup_groupBox
            // 
            this.backup_groupBox.Controls.Add(this.backup_dodatkowy_groupBox);
            this.backup_groupBox.Controls.Add(this.backupGlowny_groupBox);
            this.backup_groupBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            resources.ApplyResources(this.backup_groupBox, "backup_groupBox");
            this.backup_groupBox.Name = "backup_groupBox";
            this.backup_groupBox.TabStop = false;
            // 
            // backup_dodatkowy_groupBox
            // 
            this.backup_dodatkowy_groupBox.Controls.Add(this.label1);
            this.backup_dodatkowy_groupBox.Controls.Add(this.IsAlternativeBackup);
            this.backup_dodatkowy_groupBox.Controls.Add(this.backup_dodatkowy_Sciezka);
            resources.ApplyResources(this.backup_dodatkowy_groupBox, "backup_dodatkowy_groupBox");
            this.backup_dodatkowy_groupBox.Name = "backup_dodatkowy_groupBox";
            this.backup_dodatkowy_groupBox.TabStop = false;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // IsAlternativeBackup
            // 
            this.IsAlternativeBackup.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.IsAlternativeBackup, "IsAlternativeBackup");
            this.IsAlternativeBackup.Name = "IsAlternativeBackup";
            this.IsAlternativeBackup.UseVisualStyleBackColor = false;
            this.IsAlternativeBackup.CheckedChanged += new System.EventHandler(this.OnIsAlternativeBackupCheckedChanged);
            // 
            // backup_dodatkowy_Sciezka
            // 
            resources.ApplyResources(this.backup_dodatkowy_Sciezka, "backup_dodatkowy_Sciezka");
            this.backup_dodatkowy_Sciezka.Name = "backup_dodatkowy_Sciezka";
            this.backup_dodatkowy_Sciezka.Click += new System.EventHandler(this.OnAlternativeBackupPathClick);
            this.backup_dodatkowy_Sciezka.TextChanged += new System.EventHandler(this.OnAlternativeBackupTextChanged);
            this.backup_dodatkowy_Sciezka.Leave += new System.EventHandler(this.OnAlternativeBackupLeave);
            // 
            // backupGlowny_groupBox
            // 
            this.backupGlowny_groupBox.Controls.Add(this.backup_glowny_sciezka_label);
            this.backupGlowny_groupBox.Controls.Add(this.checkBox1);
            this.backupGlowny_groupBox.Controls.Add(this.backup_glowny_Sciezka);
            resources.ApplyResources(this.backupGlowny_groupBox, "backupGlowny_groupBox");
            this.backupGlowny_groupBox.Name = "backupGlowny_groupBox";
            this.backupGlowny_groupBox.TabStop = false;
            // 
            // backup_glowny_sciezka_label
            // 
            resources.ApplyResources(this.backup_glowny_sciezka_label, "backup_glowny_sciezka_label");
            this.backup_glowny_sciezka_label.Name = "backup_glowny_sciezka_label";
            // 
            // checkBox1
            // 
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.TabStop = false;
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // backup_glowny_Sciezka
            // 
            resources.ApplyResources(this.backup_glowny_Sciezka, "backup_glowny_Sciezka");
            this.backup_glowny_Sciezka.Name = "backup_glowny_Sciezka";
            this.backup_glowny_Sciezka.Click += new System.EventHandler(this.OnObligatoryBackupPathClick);
            this.backup_glowny_Sciezka.TextChanged += new System.EventHandler(this.OnObligatoryBackupTextChanged);
            this.backup_glowny_Sciezka.Leave += new System.EventHandler(this.OnObligatoryBackupLeave);
            // 
            // daneKlienta_groupBox
            // 
            this.daneKlienta_groupBox.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.daneKlienta_groupBox.Controls.Add(this.usText);
            this.daneKlienta_groupBox.Controls.Add(this.daneKlienta_lokalizacja_textbox);
            this.daneKlienta_groupBox.Controls.Add(this.daneKlienta_nazwaFirmy);
            this.daneKlienta_groupBox.Controls.Add(this.lokalizacja_label);
            this.daneKlienta_groupBox.Controls.Add(this.nazwaFirmy_label);
            resources.ApplyResources(this.daneKlienta_groupBox, "daneKlienta_groupBox");
            this.daneKlienta_groupBox.Name = "daneKlienta_groupBox";
            this.daneKlienta_groupBox.TabStop = false;
            // 
            // usText
            // 
            resources.ApplyResources(this.usText, "usText");
            this.usText.Name = "usText";
            // 
            // daneKlienta_lokalizacja_textbox
            // 
            resources.ApplyResources(this.daneKlienta_lokalizacja_textbox, "daneKlienta_lokalizacja_textbox");
            this.daneKlienta_lokalizacja_textbox.Name = "daneKlienta_lokalizacja_textbox";
            this.daneKlienta_lokalizacja_textbox.Leave += new System.EventHandler(this.OndaneKlienta_lokalizacja_textbox_Leave);
            // 
            // daneKlienta_nazwaFirmy
            // 
            resources.ApplyResources(this.daneKlienta_nazwaFirmy, "daneKlienta_nazwaFirmy");
            this.daneKlienta_nazwaFirmy.Name = "daneKlienta_nazwaFirmy";
            this.daneKlienta_nazwaFirmy.TextChanged += new System.EventHandler(this.OnContractorNameTextChanged);
            this.daneKlienta_nazwaFirmy.Leave += new System.EventHandler(this.OnContractorNameLeave);
            // 
            // lokalizacja_label
            // 
            resources.ApplyResources(this.lokalizacja_label, "lokalizacja_label");
            this.lokalizacja_label.Name = "lokalizacja_label";
            // 
            // nazwaFirmy_label
            // 
            resources.ApplyResources(this.nazwaFirmy_label, "nazwaFirmy_label");
            this.nazwaFirmy_label.Name = "nazwaFirmy_label";
            // 
            // Log
            // 
            this.Log.Controls.Add(this.rbLogSetup);
            resources.ApplyResources(this.Log, "Log");
            this.Log.Name = "Log";
            this.Log.UseVisualStyleBackColor = true;
            // 
            // rbLogSetup
            // 
            this.rbLogSetup.Cursor = System.Windows.Forms.Cursors.IBeam;
            resources.ApplyResources(this.rbLogSetup, "rbLogSetup");
            this.rbLogSetup.Name = "rbLogSetup";
            this.rbLogSetup.ReadOnly = true;
            // 
            // SetupForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CausesValidation = false;
            this.Controls.Add(this.MainTab);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.KeyPreview = true;
            this.Name = "SetupForm";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SetupForm_KeyDown);
            this.MainTab.ResumeLayout(false);
            this.Setup.ResumeLayout(false);
            this.Setup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.convertButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.saveButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.sql_GroupBox.ResumeLayout(false);
            this.sql_GroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sql_port)).EndInit();
            this.backup_groupBox.ResumeLayout(false);
            this.backup_dodatkowy_groupBox.ResumeLayout(false);
            this.backup_dodatkowy_groupBox.PerformLayout();
            this.backupGlowny_groupBox.ResumeLayout(false);
            this.backupGlowny_groupBox.PerformLayout();
            this.daneKlienta_groupBox.ResumeLayout(false);
            this.daneKlienta_groupBox.PerformLayout();
            this.Log.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl MainTab;
        private System.Windows.Forms.TabPage Setup;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label informacje_SerializacjaOpis;
        private System.Windows.Forms.TextBox informacje_wersjaJava;
        private System.Windows.Forms.Label label910;
        private System.Windows.Forms.TextBox infoSerial;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox automatyczneAktualizacje_insoft;
        private System.Windows.Forms.TextBox inne_serwerPcMarket;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.CheckBox inne_sklepCalodobowy;
        private System.Windows.Forms.CheckBox inne_monitorKlienta;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox inne_stanowisko;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.GroupBox sql_GroupBox;
        private System.Windows.Forms.Label sql_sprawdzPolaczenieWynik;
        private System.Windows.Forms.TextBox sql_serwer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton sql_pokazHaslo;
        private System.Windows.Forms.NumericUpDown sql_port;
        private System.Windows.Forms.TextBox sql_haslo;
        private System.Windows.Forms.TextBox sql_baza;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox backup_groupBox;
        private System.Windows.Forms.GroupBox backup_dodatkowy_groupBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox IsAlternativeBackup;
        private System.Windows.Forms.TextBox backup_dodatkowy_Sciezka;
        private System.Windows.Forms.GroupBox backupGlowny_groupBox;
        private System.Windows.Forms.Label backup_glowny_sciezka_label;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.TextBox backup_glowny_Sciezka;
        private System.Windows.Forms.GroupBox daneKlienta_groupBox;
        private System.Windows.Forms.Label usText;
        private System.Windows.Forms.TextBox daneKlienta_lokalizacja_textbox;
        private System.Windows.Forms.TextBox daneKlienta_nazwaFirmy;
        private System.Windows.Forms.Label lokalizacja_label;
        private System.Windows.Forms.Label nazwaFirmy_label;
        private System.Windows.Forms.TabPage Log;
        protected internal System.Windows.Forms.RichTextBox rbLogSetup;
        private System.Windows.Forms.PictureBox saveButton;
        private System.Windows.Forms.ComboBox sql_instancja;
        private System.Windows.Forms.PictureBox convertButton;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label4;
    }
}

