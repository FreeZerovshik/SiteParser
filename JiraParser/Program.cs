

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
        static string header_str;/* = "Тип запроса;" + 
                                "Приоритет;" + 
                                "Код;" + 
                                "Исполнитель;" +
                                "Тема;" +
                                "Обновлен;" + 
                                "Контр.время;" + 
                                "Описание;" + System.Environment.NewLine;*/

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
                Console.WriteLine("Ошибка: HTML файл "+ f_name  + " не найден");
                Console.ReadKey();
                Environment.Exit(-1);
            }

            System.IO.StreamReader sr = new System.IO.StreamReader(f_name, Encoding.GetEncoding("Windows-1251"));

            var parser = new HtmlParser();

            
            var document = parser.Parse(sr.ReadToEnd());


            var heads_tbl = document.QuerySelectorAll("thead td");

            var issue_tbl = document.QuerySelectorAll("[id^='issuerow']");


            foreach (var head in heads_tbl)
            {
                header_str += Regex.Replace(head.Text().Trim(), @"\t|\n|\r|;", "") + ';';
            }


            foreach (var item in issue_tbl) {

                // пробежимся по ячейкам таблицы
                var issue_td = item.QuerySelectorAll("[id^='issuerow'] > td");

                foreach (var td in issue_td)
                {
                    //Console.WriteLine(img.ToHtml());
                    // idx = idx + 1;

                    // получим все теги с картинками
                    var issue_img = td.QuerySelector("img");

                    if (issue_img != null)
                    {
                        issue_str += issue_img.GetAttribute("alt").ToString() + ";";
                    }
                    else
                    {
                        issue_str += get_value(td);
                    }
                }

                issue_str += System.Environment.NewLine;

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
               header_str += System.Environment.NewLine + issue_str;
                System.IO.File.WriteAllText(@".\\"+DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss_") + f_name+".csv", header_str, Encoding.GetEncoding("Windows-1251"));
            } catch ( System.IO.IOException ex)
            {
                Console.WriteLine("Невозможно сохранить в файл: parse.csv. Закройте файл и повторите запуск приложения!");
                Console.ReadKey();
                Environment.Exit(-1);

            } 
            

           
            sr.Close();

            //Console.ReadKey();


        }

        private static string get_value(IElement td)
        {
            string value;

            value = td.Text().Trim();

            value = Regex.Replace(value, @"\t|\n|\r|;", "")+";";
            return value;
        }
    }
}
