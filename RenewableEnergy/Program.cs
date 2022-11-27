/*
* Program:         RenewableEnergy
* Date:            July 20, 2020
* Author:          Ruben Dario  Mejia Cardona
* Description:     Renewable Energy Production in 2020
*/

using System;
using System.Collections.Generic;
using System.Xml;           // XmlDocument class
using System.Xml.XPath;     // XPathNavigator class

namespace RenewableEnergy
{
    class Program
    {
        // Constant - File is in the bin\Debug\netcoreapp3.1 folder
        const string XML_FILE = "renewable-energy.xml";

        static void Main(string[] args)
        {
            // Load XML file into the DOM
            XmlDocument doc = new XmlDocument();
            doc.Load(XML_FILE);

            // Create an XPathNavigator object for performing XPath queries
            XPathNavigator nav = doc.CreateNavigator();
           
            // verify if the command input is a valid option
            printMenu();
            string selection = Console.ReadLine().ToUpper();
            // verify if the command input is a valid option
            while (!(Equals(selection, "C") || Equals(selection, "R") || Equals(selection, "P")))
            {
                Console.Write("The command entered is invalid. Please try again\n");
                Console.Write("\nEnter a command: ");
                selection = Console.ReadLine().ToUpper();
            } // end while

            // Report for a selected Country
            if (selection == "C")
            {
                // Get all Countries names
                XmlNodeList allCountries = doc.SelectNodes("/renewable-energy-production/*");
                List<string> CountryList = new List<string>();
                countries(allCountries, CountryList);

                // Read the user input (enter a specific country from the list)
                Console.Write("\nEnter a country #: ");
                string selCtry = Console.ReadLine();
                int numCtry = Convert.ToInt32(selCtry);

                // verify if the country entered is a valid option
                while (numCtry < 0 || numCtry > CountryList.Count)
                {
                    Console.Write("\nThe command entered is invalid. Please try again\n");
                    Console.Write("\nEnter a valid number: ");
                    selCtry = Console.ReadLine();
                    numCtry = Convert.ToInt32(selCtry);
                } // end while

                // store the country selected
                string selectedCountry = CountryList[numCtry - 1];

                // Report selected Country
                reportByCountry(nav, numCtry, selectedCountry);
            }
            else if (selection == "R") 
            {
                // Report for a selected type of renewable energy
                Console.WriteLine("Select a renewable by number as shown below...");
                Console.WriteLine("  1. hydro\n  2. wind\n  3. biomass\n  4. solar\n  5. geothermal");

                // Get the user's task selection
                Console.Write("\nEnter a renewable #: ");

                // verify if the number input is a valid option (1 to 5)
                string num = Console.ReadLine();
                while (!(Equals(num, "1") || Equals(num, "2") || Equals(num, "3") || Equals(num, "4") || Equals(num, "5")))
                {
                    Console.Write("The command entered is invalid. Please try again\n");
                    Console.Write("\nEnter a command: ");
                    selection = Console.ReadLine();
                } // end while

                string queryCountry = $"//country";

                // Process the task
                switch (num)
                {
                    case "1": 
                        // Hydro Energy Production Report
                        reportByHydroEnergy(nav, queryCountry);                       
                        break;
                    case "2": 
                        // Wind Energy Production Report
                        reportByWindEnergy(nav, queryCountry);                       
                        break;
                    case "3":
                        // Biomass Energy Production Report
                        reportByBiomassEnergy(nav, queryCountry);
                        break;
                    case "4":
                        // Solar Energy Production Report
                        reportBySolarEnergy(nav, queryCountry);                        
                        break;
                    case "5":
                        // Geothermal Energy Production Report
                        reportByGeothermalEnergy(nav, queryCountry);
                        break;
                } // end switch
            }
            else if (selection == "P")
            {
                // Report for a selected % from renewables
                bool valid;
                string input;
                double min;
                double max;

                // Get user input for minimum %
                do
                {
                    Console.Write("\nEnter the minimum % of renewables produced or press enter for no minimum: ");
                    input = Console.ReadLine();
                    if (input == "")
                    {
                        min = 0;
                        valid = true;
                    }
                    else
                        valid = Double.TryParse(input, out min) && min > 0 && min <= 100;

                    if (!valid)
                        Console.WriteLine("\n\tERROR: Invalid input. Please enter a valid number or press enter when prompted.\n");
                } while (!valid);

                valid = false;

                // Get user input for maximum %
                do
                {
                    Console.Write("Enter the maximum % of renewables produced or press enter for no maximum: ");
                    input = Console.ReadLine();
                    if (input == "")
                    {
                        max = 100;
                        valid = true;
                    }
                    else
                        valid = Double.TryParse(input, out max) && max > 0 && max <= 100 && max > min;

                    if (!valid)
                        Console.WriteLine("\n\tERROR: Invalid input. Please enter a valid number or press enter when prompted.\n");
                } while (!valid);

                // Display the correct header based on values
                minMaxHeader(min, max);

                // format console output 
                const string formatTitle = "{0,32} {1,19} {2,18} {3,19}";
                //const string formatPercent = "{0,32} {1,18} {2,18} {3,18}";
                Console.WriteLine(string.Format(formatTitle, "Country", "All Energy (GWh)", "Renewable (GWh)", " % Renewable\n"));

                // Report selected %
                if (min == 0 && max == 100)
                    reportForSelectedPctMinMax(nav, min, max); //min max default value
               else
                    reportForSelectedPct(nav, min, max);
            }
        }

        // Display main menu
        private static void printMenu()
        {
            Console.WriteLine("Renewable Energy Production in 2020");
            Console.WriteLine("===================================\n");
            Console.Write("Enter 'C' to select a country, 'R' to select a specific renewable, or 'P' to select \na % range of renewables production: ");
        } // end printMenu

        // Get list of countries
        private static void countries(XmlNodeList countries, List<string> list)
        {
            // report all Countries names
            if (countries.Count > 0)
            {
                int idx = 1;
                string ctry = "";
                for (int i = 0; i < countries.Count; ++i)
                {
                    ctry = ((XmlElement)countries[i]).GetAttribute("name");
                    Console.WriteLine($"\t{idx}. {ctry}");
                    list.Add(ctry);
                    idx++;
                } //  end for
            } // end if	
        } // end countries

        // Report for a selected Country Method
        private static void reportByCountry(XPathNavigator nav, int numCtry, string selCountry)
        {            
            string query1 = $"//country[{numCtry}]/renewable ";
            XPathNodeIterator nodeIt = nav.Select(query1);

            if (nodeIt.Count > 0)
            {
                Console.WriteLine($"\nRenewable Energy Production in {selCountry}");
                Console.WriteLine("---------------------------------------------------------------------------\n");
                const string format = "{0,18} {1,18} {2,18} {3,18}";
                Console.WriteLine(string.Format(format, "Renewable Type", "Amount (GWh)", "% of Total", "% of Renewables"));
                Console.WriteLine();
                int matches = 1;

                while (nodeIt.MoveNext())
                {
                    string type = "";
                    string amount = "";
                    string percentAll = "";
                    string percentRenewables = "";

                    try
                    {
                        XPathExpression tp = XPathExpression.Compile($"string(//country[{numCtry}]/renewable[{matches}]/@type)");
                        XPathExpression at = XPathExpression.Compile($"string(//country[{numCtry}]/renewable[{matches}]/@amount)");
                        XPathExpression pa = XPathExpression.Compile($"string(//country[{numCtry}]/renewable[{matches}]/@percent-of-all)");
                        XPathExpression pr = XPathExpression.Compile($"string(//country[{numCtry}]/renewable[{matches}]/@percent-of-renewables)");
                        type = nav.Evaluate(tp).ToString();
                        amount = nav.Evaluate(at).ToString();
                        percentAll = nav.Evaluate(pa).ToString();
                        percentRenewables = nav.Evaluate(pr).ToString();
                    }
                    catch (XPathException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // numeric format for amount value 
                    int numInt;
                    double numDb;
                    string amntFormat = "";
                    // check if the number is a double or an int
                    if (int.TryParse(amount, out numInt))
                        amntFormat = $"{numInt:n0}";
                    else if (double.TryParse(amount, out numDb))
                        amntFormat = $"{numDb:n1}";

                    Console.WriteLine(string.Format(format, char.ToUpper(type[0]) + type.Substring(1), amntFormat != "" ? amntFormat : "n/a",
                        percentAll != "" ? percentAll : "n/a", percentRenewables != "" ? percentRenewables : "n/a"));

                    matches++;
                } // end while
                Console.WriteLine($"\n{nav.Evaluate($"count(//country[{numCtry}]/renewable)")} match(es) found.");
            } // end if
            else
                Console.WriteLine("\tNo Countries match that criterion.");
        } // end reportByCountry

        // Hydro Energy Production Method
        private static void reportByHydroEnergy(XPathNavigator navEnergy, string queryCtry)
        {
            Console.WriteLine("\nHydro Energy Production");
            Console.WriteLine("-----------------------\n");
            const string format = "{0,32} {1,18} {2,18} {3,18}";
            Console.WriteLine(string.Format(format, "Country", "Amount (GWh)", "% of Total", "% of Renewables\n"));
            XPathNodeIterator nodeItHydro = navEnergy.Select(queryCtry);

            // Custom numeric format for amount value 
            int idx = 1;
            int numInt;
            double numDb;

            while (nodeItHydro.MoveNext())
            {
                string queryH = $"string(//country[{idx}]/renewable[1]/@type)";
                string amntFormat = "";
                if (navEnergy.Evaluate(queryH).ToString() == "hydro")
                {
                    string name = "";
                    string amount = "";
                    string percentAll = "";
                    string percentRenewables = "";

                    try
                    {
                        XPathExpression nm = XPathExpression.Compile($"string(//country[{idx}]/@name)");
                        XPathExpression at = XPathExpression.Compile($"string(//country[{idx}]/renewable[1]/@amount)");
                        XPathExpression pa = XPathExpression.Compile($"string(//country[{idx}]/renewable[1]/@percent-of-all)");
                        XPathExpression pr = XPathExpression.Compile($"string(//country[{idx}]/renewable[1]/@percent-of-renewables)");
                        name = navEnergy.Evaluate(nm).ToString();
                        amount = navEnergy.Evaluate(at).ToString();
                        percentAll = navEnergy.Evaluate(pa).ToString();
                        percentRenewables = navEnergy.Evaluate(pr).ToString();
                    }
                    catch (XPathException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // check if the number is an int or double
                    if (int.TryParse(amount, out numInt))
                        amntFormat = $"{numInt:n0}";
                    else if (double.TryParse(amount, out numDb))
                        amntFormat = $"{numDb:n1}";

                    Console.WriteLine(string.Format(format, name, amntFormat != "" ? amntFormat : "n/a",
                        percentAll != "" ? percentAll : "n/a", percentRenewables != "" ? percentRenewables : "n/a"));

                    idx++;
                } // end if
            } // end while

            Console.WriteLine($"\n{navEnergy.Evaluate("count(//country/renewable[@type=\"hydro\"])")} match(es) found.");
        } // end reportByHydroEnergy

        // Wind Energy Production Method
        private static void reportByWindEnergy(XPathNavigator navEnergy, string queryCtry)
        {
            Console.WriteLine("\nWind Energy Production");
            Console.WriteLine("----------------------\n");
            const string format = "{0,32} {1,18} {2,18} {3,18}";
            Console.WriteLine(string.Format(format, "Country", "Amount (GWh)", "% of Total", "% of Renewables\n"));
            XPathNodeIterator nodeItWind = navEnergy.Select(queryCtry);
            
            // Custom numeric format for amount value 
            int idx = 1;
            int numInt;
            double numDb;

            while (nodeItWind.MoveNext())
            {
                string queryW = $"string(//country[{idx}]/renewable[2]/@type)";
                string amntFormat = "";
                if (navEnergy.Evaluate(queryW).ToString() == "wind")
                {
                    string name = "";
                    string amount = "";
                    string percentAll = "";
                    string percentRenewables = "";

                    try
                    {
                        XPathExpression nm = XPathExpression.Compile($"string(//country[{idx}]/@name)");
                        XPathExpression at = XPathExpression.Compile($"string(//country[{idx}]/renewable[2]/@amount)");
                        XPathExpression pa = XPathExpression.Compile($"string(//country[{idx}]/renewable[2]/@percent-of-all)");
                        XPathExpression pr = XPathExpression.Compile($"string(//country[{idx}]/renewable[2]/@percent-of-renewables)");
                        name = navEnergy.Evaluate(nm).ToString();
                        amount = navEnergy.Evaluate(at).ToString();
                        percentAll = navEnergy.Evaluate(pa).ToString();
                        percentRenewables = navEnergy.Evaluate(pr).ToString();
                    }
                    catch (XPathException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // check if the number is an int or double
                    if (int.TryParse(amount, out numInt))
                        amntFormat = $"{numInt:n0}";
                    else if (double.TryParse(amount, out numDb))
                        amntFormat = $"{numDb:n1}";

                    Console.WriteLine(string.Format(format, name, amntFormat != "" ? amntFormat : "n/a",
                        percentAll != "" ? percentAll : "n/a", percentRenewables != "" ? percentRenewables : "n/a"));

                    idx++;
                } // end if
            } // end while

            Console.WriteLine($"\n{navEnergy.Evaluate("count(//country/renewable[@type=\"wind\"])")} match(es) found.");
        } // end reportByWindEnergy

        // Biomass Energy Production Method
        private static void reportByBiomassEnergy(XPathNavigator navEnergy, string queryCtry)
        {
            Console.WriteLine("\nBiomass Energy Production");
            Console.WriteLine("-------------------------\n");
            const string format = "{0,32} {1,18} {2,18} {3,18}";
            Console.WriteLine(string.Format(format, "Country", "Amount (GWh)", "% of Total", "% of Renewables\n"));
            XPathNodeIterator nodeItBio = navEnergy.Select(queryCtry);
            
            // Custom numeric format for amount value 
            int idx = 1;
            int numInt;
            double numDb;

            while (nodeItBio.MoveNext())
            {
                string queryW = $"string(//country[{idx}]/renewable[3]/@type)";
                string amntFormat = "";
                if (navEnergy.Evaluate(queryW).ToString() == "biomass")
                {
                    string name = "";
                    string amount = "";
                    string percentAll = "";
                    string percentRenewables = "";

                    try
                    {
                        XPathExpression nm = XPathExpression.Compile($"string(//country[{idx}]/@name)");
                        XPathExpression at = XPathExpression.Compile($"string(//country[{idx}]/renewable[3]/@amount)");
                        XPathExpression pa = XPathExpression.Compile($"string(//country[{idx}]/renewable[3]/@percent-of-all)");
                        XPathExpression pr = XPathExpression.Compile($"string(//country[{idx}]/renewable[3]/@percent-of-renewables)");
                        name = navEnergy.Evaluate(nm).ToString();
                        amount = navEnergy.Evaluate(at).ToString();
                        percentAll = navEnergy.Evaluate(pa).ToString();
                        percentRenewables = navEnergy.Evaluate(pr).ToString();
                    }
                    catch (XPathException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // check if the number is an int or double
                    if (int.TryParse(amount, out numInt))
                        amntFormat = $"{numInt:n0}";
                    else if (double.TryParse(amount, out numDb))
                        amntFormat = $"{numDb:n1}";

                    Console.WriteLine(string.Format(format, name, amntFormat != "" ? amntFormat : "n/a",
                        percentAll != "" ? percentAll : "n/a", percentRenewables != "" ? percentRenewables : "n/a"));

                    idx++;
                } // end if
            } // end while

            Console.WriteLine($"\n{navEnergy.Evaluate("count(//country/renewable[@type=\"biomass\"])")} match(es) found.");
        } // end reportByBiomassEnergy

        // Solar Energy Production Method
        private static void reportBySolarEnergy(XPathNavigator navEnergy, string queryCtry)
        {
            Console.WriteLine("\nSolar Energy Production");
            Console.WriteLine("-------------------------\n");
            const string format = "{0,32} {1,18} {2,18} {3,18}";
            Console.WriteLine(string.Format(format, "Country", "Amount (GWh)", "% of Total", "% of Renewables\n"));
            XPathNodeIterator nodeItSolar = navEnergy.Select(queryCtry);

            // Custom numeric format for amount value 
            int idx = 1;
            int numInt;
            double numDb;

            while (nodeItSolar.MoveNext())
            {
                string queryS = $"string(//country[{idx}]/renewable[4]/@type)";
                string amntFormat = "";
                if (navEnergy.Evaluate(queryS).ToString() == "solar")
                {
                    string name = "";
                    string amount = "";
                    string percentAll = "";
                    string percentRenewables = "";

                    try
                    {
                        XPathExpression nm = XPathExpression.Compile($"string(//country[{idx}]/@name)");
                        XPathExpression at = XPathExpression.Compile($"string(//country[{idx}]/renewable[4]/@amount)");
                        XPathExpression pa = XPathExpression.Compile($"string(//country[{idx}]/renewable[4]/@percent-of-all)");
                        XPathExpression pr = XPathExpression.Compile($"string(//country[{idx}]/renewable[4]/@percent-of-renewables)");
                        name = navEnergy.Evaluate(nm).ToString();
                        amount = navEnergy.Evaluate(at).ToString();
                        percentAll = navEnergy.Evaluate(pa).ToString();
                        percentRenewables = navEnergy.Evaluate(pr).ToString();
                    }
                    catch (XPathException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    // check if the number is an int or double
                    if (int.TryParse(amount, out numInt))
                        amntFormat = $"{numInt:n0}";
                    else if (double.TryParse(amount, out numDb))
                        amntFormat = $"{numDb:n1}";

                    Console.WriteLine(string.Format(format, name, amntFormat != "" ? amntFormat : "n/a",
                        percentAll != "" ? percentAll : "n/a", percentRenewables != "" ? percentRenewables : "n/a"));

                    idx++;
                } // end if
            } // end while

            Console.WriteLine($"\n{navEnergy.Evaluate("count(//country/renewable[@type=\"solar\"])")} match(es) found.");
        } // end reportBySolarEnergy

        // Geothermal Energy Production Method
        private static void reportByGeothermalEnergy(XPathNavigator navEnergy, string queryCtry)
        {
            Console.WriteLine("Geothermal Energy Production");
            Console.WriteLine("----------------------------\n");
            const string format = "{0,32} {1,18} {2,18} {3,18}";
            Console.WriteLine(string.Format(format, "Country", "Amount (GWh)", "% of Total", "% of Renewables\n"));
            XPathNodeIterator nodeItGthermal = navEnergy.Select(queryCtry);
            
            // Custom numeric format for amount value 
            int idx = 1;
            int numInt;
            double numDb;

            while (nodeItGthermal.MoveNext())
            {
                string queryS = $"string(//country[{idx}]/renewable[5]/@type)";
                string amntFormat = "";
                if (navEnergy.Evaluate(queryS).ToString() == "geothermal")
                {
                    string name = "";
                    string amount = "";
                    string percentAll = "";
                    string percentRenewables = "";

                    try
                    {
                        XPathExpression nm = XPathExpression.Compile($"string(//country[{idx}]/@name)");
                        XPathExpression at = XPathExpression.Compile($"string(//country[{idx}]/renewable[5]/@amount)");
                        XPathExpression pa = XPathExpression.Compile($"string(//country[{idx}]/renewable[5]/@percent-of-all)");
                        XPathExpression pr = XPathExpression.Compile($"string(//country[{idx}]/renewable[5]/@percent-of-renewables)");
                        name = navEnergy.Evaluate(nm).ToString();
                        amount = navEnergy.Evaluate(at).ToString();
                        percentAll = navEnergy.Evaluate(pa).ToString();
                        percentRenewables = navEnergy.Evaluate(pr).ToString();
                    }
                    catch (XPathException e)
                    {
                        Console.WriteLine(e.Message);
                    }

                    //check if the number is an int or double
                    if (int.TryParse(amount, out numInt))
                        amntFormat = $"{numInt:n0}";
                    else if (double.TryParse(amount, out numDb))
                        amntFormat = $"{numDb:n1}";

                    Console.WriteLine(string.Format(format, name, amntFormat != "" ? amntFormat : "n/a",
                        percentAll != "" ? percentAll : "n/a", percentRenewables != "" ? percentRenewables : "n/a"));

                    idx++;
                } // end if
            } // end while

            Console.WriteLine($"\n{navEnergy.Evaluate("count(//country/renewable[@type=\"geothermal\"])")} match(es) found.");
        } // end reportByGeothermalEnergy

        // Display the correct header based on values
        private static void minMaxHeader(double min, double max)
        {

            if (min == 0 && max < 100)
            {
                Console.WriteLine($"\nCountries Where Renewables Account for Up to {String.Format("{0:0.00}", max)}% of Energy Production");
                Console.WriteLine("-----------------------------------------------------------------------\n");
            }
            else if (min > 0 && max == 100)
            {
                Console.WriteLine(string.Format("{0:0.00}", $"\nCountries Where Renewables Account for At Least {String.Format("{0:0.00}", min)}% of Energy Production"));
                Console.WriteLine("---------------------------------------------------------------------------\n");
            }
            else if (min == 0 && max == 100)
            {
                Console.WriteLine($"\nCombined Renewables for All Countries");
                Console.WriteLine("-------------------------------------\n");
            }
            else
            {
                Console.WriteLine($"\nCountries Where Renewables Account for {String.Format("{0:0.00}", min)}% to { String.Format("{0:0.00}", max)}% of Energy Production");
                Console.WriteLine("----------------------------------------------------------------------------\n");
            } // end else if	
        } // end minMaxHeader

        // Report for a selected % from renewables, min max default value
        private static void reportForSelectedPctMinMax(XPathNavigator navPct, double min, double max)
        {
            const string formatPercent = "{0,32} {1,18} {2,18} {3,18}";
            string query1 = $"//country";
            XPathNodeIterator nodeIt = navPct.Select(query1);

            int idx = 1;

            while (nodeIt.MoveNext())
            {
                string name = "";
                string sources = "";
                string renewables = "";
                string percent = "";
                // numeric format for amount value 
                int numInt;
                double numDb;

                try
                {
                    XPathExpression nm = XPathExpression.Compile($"string(//country[{idx}]/@name)");
                    name = navPct.Evaluate(nm).ToString();

                    XPathExpression aSrc;
                    string srcQuery = $"string(//country[{idx}]/totals/@all-sources)";
                    string srcTxt = navPct.Evaluate(srcQuery).ToString();

                    if (srcTxt != "")
                    {
                        aSrc = XPathExpression.Compile(srcQuery);
                        sources = navPct.Evaluate(aSrc).ToString();
                    }
                    else
                        sources = "n/a";

                    XPathExpression aRenew;
                    string renewQuery = $"string(//country[{idx}]/totals/@all-renewables)";
                    string renewTxt = navPct.Evaluate(renewQuery).ToString();

                    if (renewTxt != "")
                    {
                        aRenew = XPathExpression.Compile(renewQuery);
                        renewables = navPct.Evaluate(aRenew).ToString();
                    }
                    else
                        renewables = "n/a";

                    XPathExpression rPct;
                    string pctQuery = $"string(//country[{idx}]/totals/@renewable-percent)";
                    string pctTxt = navPct.Evaluate(pctQuery).ToString();

                    if (pctTxt != "")
                    {
                        rPct = XPathExpression.Compile(pctQuery);
                        percent = navPct.Evaluate(rPct).ToString();
                    }
                    else
                        percent = "n/a";

                }
                catch (XPathException e)
                {
                    Console.WriteLine(e.Message);
                }

                //check if sources has an int or double
                if (int.TryParse(sources, out numInt))
                    sources = $"{numInt:n0}";
                else if (double.TryParse(sources, out numDb))
                    sources = $"{numDb:n1}";

                //check if sources has an int or double
                if (int.TryParse(renewables, out numInt))
                    renewables = $"{numInt:n0}";
                else if (double.TryParse(sources, out numDb))
                    renewables = $"{numDb:n1}";

                Console.WriteLine(string.Format(formatPercent, char.ToUpper(name[0]) + name.Substring(1), sources, renewables, percent));

                idx++;
            } // end while
            Console.WriteLine($"\n{navPct.Evaluate($"count(//country/@name)")} match(es) found.");
        } // end reportForSelectedPct

        // Report for a selected % from renewables
        private static void reportForSelectedPct(XPathNavigator navPct, double min, double max)
        {
            const string formatPercent = "{0,32} {1,18} {2,18} {3,18}";
            string query = $"//country/totals[@renewable-percent >= {min} and @renewable-percent <= {max}]";
            string queryPct = $"//country/totals[@renewable-percent >= {min} and @renewable-percent <= {max}]/ancestor::country/@name";
            string querySources = $"//country/totals[@renewable-percent >= {min} and @renewable-percent <= {max}]/@all-sources";
            string queryRenewables = $"//country/totals[@renewable-percent >= {min} and @renewable-percent <= {max}]/@all-renewables";
            string queryPercent = $"//country/totals[@renewable-percent >= {min} and @renewable-percent <= {max}]/@renewable-percent";

            XPathNodeIterator nodeItCtry = navPct.Select(query);
            XPathNodeIterator nodeItPct = navPct.Select(queryPct);
            XPathNodeIterator nodeItSources = navPct.Select(querySources);
            XPathNodeIterator nodeItRenewables = navPct.Select(queryRenewables);
            XPathNodeIterator nodeItPercent = navPct.Select(queryPercent);

            int numInt;
            double numDb;

            while (nodeItCtry.MoveNext())
            {
                string name;
                if (nodeItPct.MoveNext())
                    name = nodeItPct.Current.Value;
                else
                    name = "n/a";

                string sources;
                if (nodeItSources.MoveNext())
                    sources = nodeItSources.Current.Value;
                else
                    sources = "n/a";

                string renewables;
                if (nodeItRenewables.MoveNext())
                    renewables = nodeItRenewables.Current.Value;
                else
                    renewables = "n/a";

                string percent;
                if (nodeItPercent.MoveNext())
                    percent = nodeItPercent.Current.Value;
                else
                    percent = "n/a";

                // check if sources has an int or double
                if (int.TryParse(sources, out numInt))
                    sources = $"{numInt:n0}";
                else if (double.TryParse(sources, out numDb))
                    sources = $"{numDb:n1}";

                // check if sources has an int or double
                if (int.TryParse(renewables, out numInt))
                    renewables = $"{numInt:n0}";
                else if (double.TryParse(renewables, out numDb))
                    renewables = $"{numDb:n1}";

                Console.WriteLine(string.Format(formatPercent, name, sources, renewables, percent));
            }

            Console.WriteLine("\n" + navPct.Evaluate($"count(//country/totals[@renewable-percent >= {min} and @renewable-percent <= {max}]/ancestor::country/@name)") + " match(es) found.");
        } // end reportForSelectedPct
    }
}