using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GksParser
{
    public class Organization
    {
        private string inn;
        private string name;
        public Report[] Reports { get; set; }



        [Title("ИНН")]
        public string Inn 
        { 
            get => inn;
            set 
            {
                if (!Int64.TryParse(value, out Int64 num))
                    throw new Exception("ИНН может содержать только цифры");
                if(!(value.Length==10 || value.Length==12))
                    throw new Exception("Неверный формат ИНН");
                inn = value;
            } 
        }

        [Title("Название")]
        public string Name 
        { 
            get =>name;
            set 
            {
                if(String.IsNullOrWhiteSpace(value))
                    throw new Exception("Неверный формат наименования организации");
                name = value;
            } 
        }


        public class Report
        {
            [Title("Индекс формы")]
            public string IndexForm { get; set; }
            [Title("Наименование формы")]
            public string NameForm { get; set; }
            [Title("Периодичность формы")]
            public string PeriodicityForm { get; set; }
            [Title("Срок сдачи формы")]
            public string DeadlineForm { get; set; }
            [Title("Комментарий")]
            public string Comments { get; set; }
            [Title("ОКУД")]
            public string Okud { get; set; }
        }




    }
}
