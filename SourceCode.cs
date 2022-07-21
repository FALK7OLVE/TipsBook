using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace TipsBook_v2
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        int cat_counter = 0, subcat_counter = 0, tip_counter = 0, sw_postsel = 0;
        string MyPath = AppContext.BaseDirectory;
        string[] cat_names_mass = new string[128];
        string[] tip_names_mass = new string[128];
        string[] subcat_names_mass = new string[128];

        //Menu strip -- Кнопка: Настройки шрифт
        private void Menu_strip_settings_font_Click(object sender, EventArgs e)
        {
            Font_dialog.ShowDialog();
        }

        //Menu strip -- Кнопка: Выход
        private void Menu_strip_file_appexit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы действительно хотите выйти?\r\nНесохраненный прогресс будет утерян.", "Выход", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes) Application.Exit();
        }

        //Create panel -- Кнопка: Отмена
        private void Create_panel_button_cancel_Click(object sender, EventArgs e)
        {
            Create_panel.Visible = false;
            sw_postsel = 0;
        }

        //Функция -- Создание кнопки
        void Create_btn(string BtnText, string BtnName, string Page, int counter)
        {
            //Создание основы кнопки
            int dodge = 25 * counter;
            Button CreateBtn = new Button();
            CreateBtn.TextAlign = ContentAlignment.MiddleLeft;
            CreateBtn.Name = BtnName + counter;
            CreateBtn.Size = new Size(175, 25);
            CreateBtn.Location = new Point(5, 5);
            CreateBtn.Text = BtnText;
            if (counter != 0) CreateBtn.Location = new Point(5, 5 + dodge);
            string tmpName = CreateBtn.Name;
            
            //Создание и привязка ContextMenuStrip
            ContextMenuStrip CreateCntx = new ContextMenuStrip();
            CreateCntx.Name = CreateBtn.Name + "_cntx";
            var SaveCntx = new ToolStripMenuItem("Сохранить");
            var DelCntx = new ToolStripMenuItem("Удалить");
            CreateCntx.Items.Add(SaveCntx);
            CreateCntx.Items.Add(DelCntx);
            SaveCntx.Click += SaveCntx_Click;
            DelCntx.Click += DelCntx_Click;
            CreateBtn.ContextMenuStrip = CreateCntx;
            

            //Размещение кнопки в Tab_manager_cat
            if (Page == "Catpage")
            {
                Tab_manager_cat.Controls.Add(CreateBtn);
                CreateBtn.Click += Catbtn_Click;
            }

            //Размещение кнопки в Tab_manager_subcat
            if (Page == "Subcatpage")
            {
                Tab_manager_subcat.Controls.Add(CreateBtn);
                CreateBtn.Click += Subcatbtn_Click;
            }

            //Размещение кнопки в Tab_manager_tips
            if (Page == "Tipspage")
            {
                Tab_manager_tips.Controls.Add(CreateBtn);
                CreateBtn.Click += Tipbtn_Click;
            }

        }
        
        //Функция -- ContextMenuStrip нажатие кнопки "Сохранить"
        void SaveCntx_Click(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;
            if (toolStripMenuItem != null)
            {
                //Определение кнопки на которой вызвано ContextMenuStrip
                string OwnerName = toolStripMenuItem.Owner.Name;
                int OwnerNameLengh = OwnerName.Length - 5;
                string tmpLine = OwnerName.Remove(OwnerNameLengh);

                //Проверка есть ли на странице "Категории" найденная кнопка
                if (Tab_manager_cat.Controls.ContainsKey(tmpLine) == true)
                {
                    var button = Tab_manager_cat.Controls[tmpLine];
                    File.WriteAllText(MyPath + "Categories/" + button.Text + "_cat.ini", String.Empty);
                    StreamWriter sw = File.AppendText(MyPath + "/Categories/" + button.Text + "_cat.ini");
                    for (int i = 0; i < subcat_counter; i++)
                    {
                        sw.WriteLine(subcat_names_mass[i] + "=" + MyPath + "Subcategories/" + subcat_names_mass[i] + "_scat.ini");
                    }
                    sw.Close();
                }

                //Проверка есть ли на странице "Подкатегории" найденная кнопка
                if (Tab_manager_subcat.Controls.ContainsKey(tmpLine) == true)
                {
                    var button = Tab_manager_subcat.Controls[tmpLine];
                    File.WriteAllText(MyPath + "/Subcategories/" + button.Text + "_scat.ini", String.Empty);
                    StreamWriter sw = File.AppendText(MyPath + "/Subcategories/" + button.Text + "_scat.ini");
                    for (int i = 0; i < tip_counter; i++)
                    {
                        sw.WriteLine(tip_names_mass[i] + "=" + MyPath + "Tips/" + tip_names_mass[i] + "_tips.ini");
                    }
                    sw.Close();
                }

                //Проверка есть ли на странице "Заметки" найденная кнопка
                if (Tab_manager_tips.Controls.ContainsKey(tmpLine) == true)
                {
                    var button = Tab_manager_tips.Controls[tmpLine];
                    StreamWriter swn = new StreamWriter(MyPath + "/Tips/" + button.Text + "_tips.ini");
                    swn.Write(Tips_textbox.Text);
                    swn.Close();
                }
            }
        }

        //Функция -- ContextMenuStrip нажатие кнопки "Удалить"
        void DelCntx_Click(object sender, EventArgs e)
        {
            var toolStripMenuItem = (ToolStripMenuItem)sender;

            if (toolStripMenuItem != null)
            {
                string OwnerName = toolStripMenuItem.Owner.Name;
                int OwnerNameLengh = OwnerName.Length - 5;
                string tmpLine = OwnerName.Remove(OwnerNameLengh);

                //Проверка есть ли на странице "Категории" найденная кнопка
                if (Tab_manager_cat.Controls.ContainsKey(tmpLine) == true)
                {
                    var button = Tab_manager_cat.Controls[tmpLine];
                    File.Delete(MyPath + "/Categories/" + button.Text + "_cat.ini");
                    button.Dispose();
                    cat_counter--;
                }

                //Проверка есть ли на странице "Подкатегории" найденная кнопка
                if (Tab_manager_subcat.Controls.ContainsKey(tmpLine) == true)
                {
                    var button = Tab_manager_subcat.Controls[tmpLine];
                    File.Delete(MyPath + "/Subcategories/" + button.Text + "_scat.ini");
                    button.Dispose();
                    subcat_counter--;
                }

                //Проверка есть ли на странице "Заметки" найденная кнопка
                if (Tab_manager_tips.Controls.ContainsKey(tmpLine) == true)
                {
                    var button = Tab_manager_tips.Controls[tmpLine];
                    File.Delete(MyPath + "/Tips/" + button.Text + "_tips.ini");
                    button.Dispose();
                    tip_counter--;
                }
            }
        }

        //Функция -- Нажатие на кнопку Категории
        private void Catbtn_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button != null)
            {
                /*
                if (File.Exists("Categories/" + button.Text + "_cat.ini") != true)
                {
                    //Создание первой подкатегории
                    Menu_strip_create_subcat.Enabled = true;
                    Tab_manager_subcat.Controls.Clear();
                    subcat_counter = 0;
                    Create_perek(2);
                }
                */
                //else 
                if (File.Exists("Categories/" + button.Text + "_cat.ini") == true)
                {
                    //Получение списка подкатегорий для категории
                    Tab_manager_subcat.Controls.Clear();
                    subcat_counter = 0;
                    StreamReader sr = new StreamReader("Categories/" + button.Text + "_cat.ini");
                    int tmp_counter = 0;
                    while (true)
                    {
                        string tmp = sr.ReadLine();
                        if (tmp != null && tmp != "")
                        {
                            subcat_names_mass[tmp_counter] = tmp;
                            subcat_counter++;
                            tmp_counter++;
                        }
                        if (tmp == null) break;
                    }
                    sr.Close();

                    //Создание кнопок подкатегорий
                    for (int i = 0; i < tmp_counter; i++)
                    {
                        if (subcat_names_mass != null)
                        {
                            int pos = subcat_names_mass[i].IndexOf("=");
                            if (pos != -1)
                            {
                                string tmpLine = subcat_names_mass[i].Substring(0, pos);
                                string Path = subcat_names_mass[i].Substring(pos + 1);
                                Status_strip_statuslabel.Text = Path;
                                Create_btn(tmpLine, "Subcatbtn", "Subcatpage", i);
                            }
                        }
                    }
                    Status_strip_statuslabel.Text = "Категория [" + button.Text + "] загружена.";
                }
            }
        }

        //Функция -- Нажатие на кнопку Подкатегории
        private void Subcatbtn_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button != null)
            {
                /*
                if (File.Exists("Subcategories/" + button.Text + "_scat.ini") != true)
                {
                    //Создание первой  заметки
                    Menu_strip_create_tips.Enabled = true;
                    Tab_manager_tips.Controls.Clear();
                    tip_counter = 0;
                    Create_perek(3);
                }
                */
                //else 
                if (File.Exists("Subcategories/" + button.Text + "_scat.ini") == true)
                {
                    //Получени списка заметок для подкатегории
                    Tab_manager_tips.Controls.Clear();
                    tip_counter = 0;
                    StreamReader sr = new StreamReader("Subcategories/" + button.Text + "_scat.ini");
                    int tmp_counter = 0;
                    while (true)
                    {
                        string tmp = sr.ReadLine();
                        if (tmp != null && tmp != "")
                        {
                            tip_names_mass[tmp_counter] = tmp;
                            tip_counter++;
                            tmp_counter++;
                        }
                        if (tmp == null) break;
                    }
                    sr.Close();

                    //Создание кнопок заметок
                    for (int i = 0; i < tmp_counter; i++)
                    {
                        if (tip_names_mass != null)
                        {
                            int pos = tip_names_mass[i].IndexOf("=");
                            if (pos != -1)
                            {
                                string tmpLine = tip_names_mass[i].Substring(0, pos);
                                string Path = tip_names_mass[i].Substring(pos + 1);
                                Status_strip_statuslabel.Text = Path;
                                Create_btn(tmpLine, "Tipsbtn", "Tipspage", i);
                            }
                        }
                    }
                    Status_strip_statuslabel.Text = "Подкатегория [" + button.Text + "] загружена.";
                }
            }
        }

        //Функция -- Нажатие на кнопку Заметки
        private void Tipbtn_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            if (button != null)
            {
                if (File.Exists("Tips/" + button.Text + "_tips.ini") != true) Status_strip_statuslabel.Text = "Заметка [" + button.Text + "] не найдена.";
                if (File.Exists("Tips/" + button.Text + "_tips.ini") == true)
                {
                    Tips_textbox.Text = "";
                    StreamReader sr = new StreamReader("Tips/" + button.Text + "_tips.ini");
                    string tmpText = sr.ReadToEnd();
                    sr.Close();
                    Tips_textbox.Text = tmpText;
                    Status_strip_statuslabel.Text = "Заметка [" + button.Text + "] загружена.";
                }
            }
        }

        //Функция -- Переключатель
        void Create_perek(int sw_sel)
        {
            switch (sw_sel)
            {
                //Отображение панели для создания категории в Create_panel
                case 1:
                    {
                        Create_panel.Visible = true;
                        Create_panel_text.Text = "Введите имя категории:";
                        sw_postsel = 1;
                        break;
                    }

                //Отображение панели для создания подкатегории в Create_panel
                case 2:
                    {
                        Create_panel.Visible = true;
                        Create_panel_text.Text = "Введите имя подкатегории:";
                        sw_postsel = 2;
                        break;
                    }

                //Отображение панели для создания заметки в Create_panel
                case 3:
                    {
                        Create_panel.Visible = true;
                        Create_panel_text.Text = "Введите имя заметки:";
                        sw_postsel = 3;
                        break;
                    }
            }
        }

        //Menu strip -- Кнопка: Создать категорию
        private void Menu_strip_create_cat_Click(object sender, EventArgs e)
        {
            Create_perek(1);
        }

        //Menu strip -- Кнопка: Создать подкатегорию
        private void Menu_strip_create_subcat_Click(object sender, EventArgs e)
        {
            Create_perek(2);
        }

        //Menu strip -- Кнопка: Создать заметку
        private void Menu_strip_create_tips_Click(object sender, EventArgs e)
        {
            Create_perek(3);
        }

        //Menu strip -- Кнопка: Сохранить категории
        private void Menu_strip_file_savecat_Click(object sender, EventArgs e)
        {
            StreamWriter writer = new StreamWriter(MyPath + "Settings.ini");
            writer.Write("[Cat_counter]=" + cat_counter + "\r\n" + "[Tip_counter]=" + tip_counter + "\r\n" + "[Subcat_counter]=" + subcat_counter + "\r\n");
            writer.Close();
            File.WriteAllLines(MyPath + "/Configs/" + "Cat_names.ini", cat_names_mass);
            File.WriteAllLines(MyPath + "/Configs/" + "Subcat_names.ini", subcat_names_mass);
            File.WriteAllLines(MyPath + "/Configs/" + "Tip_names.ini", tip_names_mass);

            Status_strip_statuslabel.Text = "Категории сохранены.";
        }

        //Menu strip -- Кнопка: Справка -> Загрузка
        private void Menu_strip_help_load_Click(object sender, EventArgs e)
        {
            string HelpText = "Загрузка категорий\r\n" + "Для загрузки категорий выполните: Файл -> Обновить категории\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            HelpText += "Загрузка подкатегорий\r\n" + "Для загрузки подкатегорий, достаточно нажать на необходимую категорию\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            HelpText += "Загрузка заметок\r\n" + "Для загрузки заметок, достаточно нажать на необходимую подкатегорию\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            HelpText += "Загрузка содержимого заметок\r\n" + "Для загрузки содержимого заметок, достаточно нажать на необходимую заметку\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            MessageBox.Show(HelpText, "Справка: Загрузка", MessageBoxButtons.OK);
        }

        private void Menu_strip_help_save_Click(object sender, EventArgs e)
        {
            string HelpText = "Сохранение категорий\r\n" + "Для сохранения категории, нажмите на ней ПКМ и выберите Сохранить\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            HelpText += "Сохранение подкатегорий\r\n" + "Для сохранения подкатегории, нажмите на ней ПКМ и выберите Сохранить\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            //HelpText += "Сохранение заметок\r\n" + "Для сохранения заметки, нажмите на ней ПКМ и выберите Сохранить\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            HelpText += "Сохранение содержимого заметок\r\n" + "Для сохранения содержимого заметки, нажмите на ней ПКМ и выберите Сохранить\r\n" + "Внимание! Несохраненные данные могут быть потеряны\r\n\r\n";
            MessageBox.Show(HelpText, "Справка: Сохранение", MessageBoxButtons.OK);
        }

        //Menu strip -- Кнопка: Обновить категории
        private void Menu_strip_file_updcat_Click(object sender, EventArgs e)
        {
            StreamReader sr = new StreamReader(MyPath + "Settings.ini");
            sr.Close();
            string settings = "";
            //int indof_counter = 0;     

            //Получение из файла "Settings.ini" количества: Категорий, Подкатегорий, Заметок
            /*
            while(true)
            {
                settings = sr.ReadLine();
                if (settings != null)
                {
                    int pos = settings.IndexOf("=");
                    if (pos != -1)
                    {
                        if (indof_counter == 0) cat_counter = Convert.ToInt32(settings.Substring(pos + 1));
                        if (indof_counter == 1) tip_counter = Convert.ToInt32(settings.Substring(pos + 1));
                        if (indof_counter == 2) subcat_counter = Convert.ToInt32(settings.Substring(pos + 1));
                        indof_counter++;
                    }
                }
                if (settings == null) break;
            }
            */

            //Получение количества файлов по параметрам (Путь, Название.Расширение, Только в этой папке / Во всех)
            /*
            int allFiles = Directory.GetFiles(MyPath + "Categories/", "*_cat.ini", SearchOption.TopDirectoryOnly).Length;
            cat_counter = allFiles;
            */

            //Определение количества категорий по количеству имен в файле
            sr = new StreamReader(MyPath + "Configs/" + "Cat_names.ini");
            while(true)
            {
                settings = sr.ReadLine();
                if (settings != null && settings != "")
                {
                    cat_counter++;
                }
                if (settings == null) break;
            }
            sr.Close();

            //Проверка на наличие папки "Configs"
            if (Directory.Exists("Configs") == true)
            {
                //Проверка на наличие файла "Cat_names.ini"
                if (File.Exists("Configs/" + "Cat_names.ini") == true)
                {
                    //Получение имен категорий
                    sr = new StreamReader("Configs/" + "Cat_names.ini");
                    for (int i = 0; i < cat_counter; i++)
                    {
                        string tmp_BtnText;
                        int tmpi = i;
                        tmp_BtnText = sr.ReadLine();
                        cat_names_mass[tmpi] = tmp_BtnText;
                        Create_btn(tmp_BtnText, "Catbtn", "Catpage", tmpi);
                    }
                    sr.Close();
                }
            }
            Status_strip_statuslabel.Text = "Категории обновлены.";
        }

        //Create panel -- Кнопка: Принять
        private void Create_panel_button_Click(object sender, EventArgs e)
        {
            //Создание: Категории/Подкатегории/Заметки
            if (sw_postsel == 1)
            {
                Create_btn(Create_panel_textbox.Text, "Catbtn", "Catpage" , cat_counter);
                Create_panel.Visible = false;
                Status_strip_statuslabel.Text = "Категория [" + Create_panel_textbox.Text + "] создана.";
                cat_names_mass[cat_counter] = Create_panel_textbox.Text;
                Create_panel_textbox.Text = "";
                cat_counter++;
                sw_postsel = 0;
            }

            if (sw_postsel == 2)
            {
                Create_btn(Create_panel_textbox.Text, "Subcatbtn", "Subcatpage", subcat_counter);
                Create_panel.Visible = false;
                Status_strip_statuslabel.Text = "Подкатегория [" + Create_panel_textbox.Text + "] создана.";
                subcat_names_mass[subcat_counter] = Create_panel_textbox.Text;
                Create_panel_textbox.Text = "";
                subcat_counter++;
                sw_postsel = 0;
            }

            if (sw_postsel == 3)
            {
                Create_btn(Create_panel_textbox.Text, "Tipsbtn", "Tipspage", tip_counter);
                Create_panel.Visible = false;
                Status_strip_statuslabel.Text = "Заметка [" + Create_panel_textbox.Text + "] создана.";
                tip_names_mass[tip_counter] = Create_panel_textbox.Text;
                Create_panel_textbox.Text = "";
                tip_counter++;
                sw_postsel = 0;
            }
        }

        //Функция -- Проверка и создание необходимых файлов
        private void Main_load(object sender, EventArgs e)
        {
            if (File.Exists(MyPath + "Settings.ini") != true)
            {
                StreamWriter writer = new StreamWriter(MyPath + "Settings.ini");
                writer.Write("[Cat_counter]=" + cat_counter + "\r\n" + "[Tip_counter]=" + tip_counter + "\r\n" + "[Subcat_counter]=" + subcat_counter + "\r\n");
                writer.Close();
                Directory.CreateDirectory(MyPath + "/Categories");
                Directory.CreateDirectory(MyPath + "/Subcategories");
                Directory.CreateDirectory(MyPath + "/Tips");
                Directory.CreateDirectory(MyPath + "/Configs");
                File.Create(MyPath + "/Configs/" + "Cat_names.ini");
                File.Create(MyPath + "/Configs/" + "Subcat_names.ini");
                File.Create(MyPath + "/Configs/" + "Tip_names.ini");
            }
            Status_strip_statuslabel.Text = "Загружено.";
        }
    }
}
