using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TimerScrapper.Models;

namespace TimerScrapper
{
    public static class Jail
    {
        [FunctionName("Jail")]
        public static void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Jail Timer trigger function executed at: {DateTime.Now}");
            // https://web.minnehahacounty.org/dept/so/jailInmateInfo/jailInmateInfoSearchResults.php?txtLastName=&btnSearch=Search
            // https://web.minnehahacounty.org/dept/so/jailInmateInfo/jailInmateInfo.php 
            var html = string.Format("https://web.minnehahacounty.org/dept/so/jailInmateInfo/jailInmateInfoSearchResults.php?txtLastName=&btnSearch=Search");
            string[] stringArray = { "span", "script", "head", "title", "br" };
            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);
            var inmates = new List<Inmate>();

            foreach (HtmlNode table in htmlDoc.DocumentNode.SelectNodes("//table[@class='tablesorter']"))
            {
                var rows = table.Descendants("tr");
                foreach (HtmlNode row in rows)
                {
                    var cells = row.SelectNodes("th|td");

                    // Full row of details
                    if (cells?.Count() == 9)
                    {
                        // header row?
                        inmates.Add(ParseInmate(cells));
                    }
                }
            }

            foreach (Inmate inmate in inmates)
            {
                log.LogInformation(string.Format("{0} {1} - {2} {3}", inmate.FirstName,inmate.LastName,inmate.IntakeDate,inmate.IntakeTime));
            }

        }

        private static Inmate ParseInmate(HtmlNodeCollection cells)
        {
            var inmate = new Inmate();
            var counter = 0;
            foreach (HtmlNode cell in cells)
            {

                switch (counter)
                {
                    case 0:
                        inmate.InmateNumber = cell.InnerText;
                        break;
                    case 1:
                        inmate.FirstName = cell.InnerText;
                        break;
                    case 2:
                        inmate.MiddleName = cell.InnerText;
                        break;
                    case 3:
                        inmate.LastName = cell.InnerText;
                        break;
                    case 4:
                        inmate.IntakeDate = cell.InnerText;
                        break;
                    case 5:
                        inmate.IntakeTime = cell.InnerText;
                        break;
                    case 6:
                        inmate.Facility = cell.InnerText;
                        break;
                    case 7:
                        inmate.BondAmount = cell.InnerText;
                        break;
                    case 8:
                        inmate.Charges = cell.InnerText;
                        break;
                    default:
                        break;
                }

                counter += 1;
            }
            return inmate;
        }
    }
}
