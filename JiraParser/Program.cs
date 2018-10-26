

using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Extensions;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace JiraParser
{
    class Program
    {
        static string issue_str;
        static string issue_type;
        static string issue_prior;
        
        static string issue_num;
        static string issue_assignee;
        static string issue_name;
        static string issue_desc;
        static string issue_update;
        static string issue_ctrl;

        static string f_name;

        // заполним заголовок
        static string out_str = "Тип запроса;" + 
                                "Приоритет;" + 
                                "Код;" + 
                                "Исполнитель;" +
                                "Тема;" +
                                "Обновлен;" + 
                                "Контр.время;" + 
                                "Описание;" + System.Environment.NewLine;

        static void Main(string[] args)
        {
   
            if (args.Length != 0)
            {
                f_name = args[0];
            } else
            {
                f_name = "j.htm";
            }

            Console.WriteLine("Запуск разбора файла: " + f_name);

            if (File.Exists(f_name) == false)
            {
                Console.WriteLine("Ошибка: HTML файл"+ f_name  + " не найден");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            System.IO.StreamReader sr = new System.IO.StreamReader(f_name, Encoding.GetEncoding("Windows-1251"));

            var parser = new HtmlParser();

            
            var document = parser.Parse(sr.ReadToEnd());

           // Console.WriteLine(sr.CurrentEncoding);
            

            //Do something with document like the following

            //var issue_tbl = document.QuerySelectorAll("issuerow");
            //var issue_tbl = document.QuerySelectorAll("*").Where(m => m.LocalName == "tr" && m.Id == "issuerow448028");

            var issue_tbl = document.QuerySelectorAll("[id^='issuerow']");


            foreach (var item in issue_tbl) {

             
                var idx = new int();
                var issue_img = item.QuerySelectorAll("img");


                // получим все теги с картинками
                foreach (var img in issue_img)
                 {
                    
                    //Console.WriteLine(img.ToHtml());
                    idx = idx + 1;
                   // Console.WriteLine(idx);

                    if (idx == 1) {
                        issue_type = img.GetAttribute("alt").ToString();
                    } else if (idx == 2) {
                        issue_prior = img.GetAttribute("alt").ToString();
                    } 

                    
                 }


                // пробежимся по ячейкам таблицы
                var issue_td = item.QuerySelectorAll("td");
                idx = 0;

                foreach (var td in issue_td)
                {
                     //Console.WriteLine(img.ToHtml());
                    idx = idx + 1;

                    if (idx == 3) // код
                    {
                        issue_num = td.Text().Trim();
                    }
                    else if (idx == 4) // на кого назначено
                    {
                        issue_assignee = td.Text().Trim();
                    }
                    else if (idx == 5) // Наименование
                    {
                        issue_name = td.Text().Trim();
                        issue_name = Regex.Replace(issue_name, @"\t|\n|\r|;", "");
                    }
                    else if (idx == 6) // Обновлен
                    {
                        issue_update = td.Text().Trim();
                    }
                    else if (idx == 7) // Контр. время
                    {
                        issue_ctrl = Regex.Replace(td.Text().Trim(), @"\t|\n|\r|;", "");
                    }
                    else if (idx == 8) // Наименование
                    {
                        issue_desc =  Regex.Replace(td.Text().Trim(), @"\t|\n|\r|;", "");
                    }
                    //Console.WriteLine(idx);

                }



                issue_str += issue_type + ";" + 
                            issue_prior + ";" + 
                            issue_num + ";" + 
                            issue_assignee + ";" +
                            issue_name + ";" +
                            issue_update + ";" + 
                            issue_ctrl + ";" + 
                            issue_desc + ";" + 
                            System.Environment.NewLine;

                //Console.WriteLine(issue_str);
                // вывод текста из HTML
                //Console.WriteLine(item.Text());

                //Console.WriteLine("---------------------------------------------------");
                 //Console.WriteLine(item.Text());
            }

            //Console.WriteLine(issue_str);

            //issue_str.Replace(System.Environment.NewLine, string.Empty);

           // issue_str = Regex.Replace(issue_str, @"\t|\n|\r", "");

            try
            {
                out_str += issue_str;
                System.IO.File.WriteAllText(@".\\"+DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_") + f_name+".csv", out_str, Encoding.GetEncoding("Windows-1251"));
            } catch ( System.IO.IOException ex)
            {
                Console.WriteLine("Невозможно сохранить в файл: parse.csv. Закройте файл и повторите запуск приложения!");
                Console.ReadKey();
                Environment.Exit(-1);

            } 
            

           
            sr.Close();

            //Console.ReadKey();


        }
    }
}
