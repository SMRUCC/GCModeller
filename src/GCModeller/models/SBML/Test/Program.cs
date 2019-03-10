using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LANS.SystemsBiology.Assembly.SBML.ExportServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.Language;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            var ddd = new Biopax.MetaCyc.Biopax.Level3.File { Owl = new Biopax.MetaCyc.Biopax.Level3.Elements.owlOntology()};

            ddd.SaveAsXml(@"X:\fffff.xml");

            var owl = Biopax.MetaCyc.Biopax.Level3.File.LoadDoc(@"F:\SBML\data\Escherichia_coli.owl");

            List<string> m = 
                LinqAPI.MakeList<string>() <= from x
                                              in owl.SmallMolecules
                                              where x.displayName.value != "3"
                                              select x.entityReference.value;

            m = m + new string [] { "23","55","gg" };
            m += "asda";
            m -= "56";

            var file = LANS.SystemsBiology.Assembly.SBML.Level2.XmlFile.Load(@"F:\1.13.RegPrecise_network\FBA\xcam314565\19.0\data\metabolic-reactions.xml");
            var dd = file.Model.listOfReactions.First();
            double l = dd.LowerBound;
            double u = dd.UpperBound;
            var rxns = LANS.SystemsBiology.Assembly.SBML.ExportServices.KEGG.GetReactions(file,true);
            Console.Read();
        }
    }
}
