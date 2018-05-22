using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Serialization;
using CsvHelper;
using Newtonsoft.Json;
using WebAPI.data1.Entities;

namespace WebAPI.data1
{
    public class StockSeeder
    {
        private StockContext _ctx;
        public StockSeeder(StockContext ctx)
        {
            _ctx = ctx;
        }

        public void Seed()
        {
            if (_ctx.SecuritySet.Count() > 0)
            {
                return;
            }
            SeedStockCollection();
        }

        //TODO:create key value pairs in web.config for file path to the config file
        void SeedStockCollection()
        {
            var securitiesFileName = HostingEnvironment.MapPath("~/App_Data/securities.json");
            List<Securities> securityList = JsonConvert.DeserializeObject<List<Securities>>(File.ReadAllText(securitiesFileName));
            Dictionary<string, Dictionary<DateTime, string>> aliceClosePriceMap, bobClosePriceMap, charlieClosePriceMap;
            HashSet<DateTime> allDateTimes = new HashSet<DateTime>();

            //ALICE
            var aliceInWonderland = HostingEnvironment.MapPath("~/App_Data/StocksPrices-ALICE.csv");
            using (var sr = new StreamReader(aliceInWonderland))
            {
                var reader = new CsvReader(sr);
                List<Alice> alice = reader.GetRecords<Alice>().ToList();
                foreach (var alice1 in alice)
                {
                    if (allDateTimes.Contains(alice1.date))
                        continue;
                    else
                        allDateTimes.Add(alice1.date);
                }
                aliceClosePriceMap = secCloseDatePriceMap(alice);
            }

            //BOB
            var bobTheBuilder = HostingEnvironment.MapPath("~/App_Data/StocksPrices-BOB.xls");
            var rawData = OpenExcel(bobTheBuilder, "StocksPrices-BOB");
            IEnumerable<DataRow> rows = rawData.Rows.Cast<DataRow>();

            foreach (var row in rows)
            {
                if (allDateTimes.Contains(DateTime.Parse(row["date"].ToString())))
                    continue;
                else
                    allDateTimes.Add(DateTime.Parse(row["date"].ToString()));
            }

            bobClosePriceMap = secCloseDatePriceMap(rows);

            //Charlies Angel

            XmlRootAttribute xRoot = new XmlRootAttribute();
            xRoot.ElementName = "root";
            xRoot.IsNullable = true;
            XmlSerializer serializer = new XmlSerializer(typeof(List<row>), xRoot);
            var charliesAngel = HostingEnvironment.MapPath("~/App_Data/StocksPrices-CHARLIE.xml");
            using (var sr = new StreamReader(charliesAngel))
            {
                List<row> charlie = (List<row>)serializer.Deserialize(sr);
                foreach (var alice1 in charlie)
                {
                    if (allDateTimes.Contains(alice1.date))
                        continue;
                    else
                        allDateTimes.Add(alice1.date);
                }
                charlieClosePriceMap = secCloseDatePriceMap(charlie);
            }

            foreach (var security in securityList)
            {
                Security s = new Security();
                s.symbol = security.symbol;
                s.Sector = security.Sector;
                s.SecurityName = security.Security;
                s.SubIndustry = security.SubIndustry;
                ICollection<Prices> pricesCollection = new List<Prices>();
                foreach (var dateTime in allDateTimes)
                {
                    Prices p = new Prices();
                    p.symbol = security.symbol;
                    p.CloseDate = dateTime;
                    if (aliceClosePriceMap.ContainsKey(security.symbol))
                    {
                        if (aliceClosePriceMap[security.symbol].ContainsKey(dateTime))
                        {
                            string close = aliceClosePriceMap[security.symbol][dateTime];
                            if (String.IsNullOrEmpty(close))
                                p.Alice = "NA";
                            else
                                p.Alice = close;
                        }
                    }
                    if (bobClosePriceMap.ContainsKey(security.symbol))
                    {
                        if (bobClosePriceMap[security.symbol].ContainsKey(dateTime))
                        {
                            string close = bobClosePriceMap[security.symbol][dateTime];
                            if (String.IsNullOrEmpty(close))
                                p.Bob = "NA";
                            else
                                p.Bob = close;
                        }
                    }
                    if (charlieClosePriceMap.ContainsKey(security.symbol))
                    {
                        if (charlieClosePriceMap[security.symbol].ContainsKey(dateTime))
                        {
                            string close = charlieClosePriceMap[security.symbol][dateTime];
                            if (String.IsNullOrEmpty(close))
                                p.Charlie = "NA";
                            else
                                p.Charlie = close;
                        }
                    }
                    pricesCollection.Add(p);
                }
                s.PriceCollection = pricesCollection;
                _ctx.SecuritySet.Add(s);
            }
            _ctx.SaveChanges();
        }

        //TODO: Implement 'Microsoft.ACE.OLEDB.12.0' provider for xlsx (not registered on the local machine.)
        static DataTable OpenExcel(string filename, string sheet)
        {
            var cs = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=Excel 8.0", filename);
            DataTable dt = new DataTable();
            using (OleDbConnection conn = new OleDbConnection(cs))
            {
                conn.Open();
                using (OleDbCommand cmd = new OleDbCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = string.Format("SELECT * FROM [{0}$]", sheet);
                    using (OleDbDataAdapter adapter = new OleDbDataAdapter())
                    {
                        adapter.SelectCommand = cmd;
                        adapter.Fill(dt);
                    }
                }
                conn.Close();
            }
            return dt;
        }

        //TODO: Generalise following methods into one using genrics T
        Dictionary<string, Dictionary<DateTime, string>> secCloseDatePriceMap(List<Alice> alices)
        {
            Dictionary<string, Dictionary<DateTime, string>> aliceDictionary = new Dictionary<string, Dictionary<DateTime, string>>();
            foreach (var alice in alices)
            {
                if (aliceDictionary.ContainsKey(alice.Name))
                {
                    if (aliceDictionary[alice.Name].ContainsKey(alice.date))
                    {
                        Console.WriteLine("Duplicate: {0}, {1}", alice.Name, alice.date);
                    }
                    else
                    {
                        aliceDictionary[alice.Name].Add(alice.date, alice.close);
                    }
                }
                else
                {
                    aliceDictionary.Add(alice.Name, new Dictionary<DateTime, string>() { { alice.date, alice.close } });
                }
            }
            return aliceDictionary;
        }

         Dictionary<string, Dictionary<DateTime, string>> secCloseDatePriceMap(IEnumerable<DataRow> rows)
        {
            Dictionary<string, Dictionary<DateTime, string>> aliceDictionary = new Dictionary<string, Dictionary<DateTime, string>>();
            foreach (DataRow row in rows)
            {
                if (aliceDictionary.ContainsKey(row["Name"].ToString()))
                {
                    if (aliceDictionary[row["Name"].ToString()].ContainsKey(DateTime.Parse(row["date"].ToString())))
                    {
                        Console.WriteLine("Duplicate: {0}, {1}", row["Name"].ToString(), DateTime.Parse(row["date"].ToString()));
                    }
                    else
                    {
                        aliceDictionary[row["Name"].ToString()].Add(DateTime.Parse(row["date"].ToString()), row["close"].ToString());
                    }
                }
                else
                {
                    aliceDictionary.Add(row["Name"].ToString(), new Dictionary<DateTime, string>() { { DateTime.Parse(row["date"].ToString()), row["close"].ToString() } });
                }
            }
            return aliceDictionary;
        }
         Dictionary<string, Dictionary<DateTime, string>> secCloseDatePriceMap(List<row> row)
        {
            Dictionary<string, Dictionary<DateTime, string>> aliceDictionary = new Dictionary<string, Dictionary<DateTime, string>>();
            foreach (var alice in row)
            {
                if (aliceDictionary.ContainsKey(alice.Name))
                {
                    if (aliceDictionary[alice.Name].ContainsKey(alice.date))
                    {
                        Console.WriteLine("Duplicate: {0}, {1}", alice.Name, alice.date);
                    }
                    else
                    {
                        aliceDictionary[alice.Name].Add(alice.date, alice.close);
                    }
                }
                else
                {
                    aliceDictionary.Add(alice.Name, new Dictionary<DateTime, string>() { { alice.date, alice.close } });
                }
            }
            return aliceDictionary;
        }

    }
}