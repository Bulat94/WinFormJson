using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Reflection;
using System.Threading;

namespace WinJson
{
    public partial class Form1 : Form
    {
        List<Currency> List;
        public Form1()
        {
            InitializeComponent();
            
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            List = await GetCurrencyListAsync();
            comboBox1.DataSource   = new BindingSource(List, null);
            comboBox1.DisplayMember = "Name";
        }
        
        public async Task<List<Currency>> GetCurrencyListAsync()
        {
            var client = new HttpClient(); 
            var response = await client.GetAsync(new Uri("https://www.cbr-xml-daily.ru/daily_json.js"));
            var result = await response.Content.ReadAsStringAsync();

            Rootobject root = JsonConvert.DeserializeObject<Rootobject>(result);
            return root.Valute.Select(x=> x.Value).ToList();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Currency cur = List.Where(x => x.Name == comboBox1.Text).FirstOrDefault();
            if (cur == null)
                cur = List.FirstOrDefault();
            string rubles = cur.Nominal == 1 ? " рубль" : " рублей";

            label1.Text = cur.Nominal.ToString()  + rubles + " = " + cur.Value + " " + cur.Name;
            
        }
    }
}
