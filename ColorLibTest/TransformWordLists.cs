using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ColorLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest
{
    [TestClass]
    public class TransformWordLists
    {
        private static int lPos = 12;

        private static void WriteBlock (string s)
        {
            Console.Write('"');
            Console.Write(s);
            Console.Write("\", ");
            lPos += s.Length + 4;
            if (lPos > 93)
            {
                Console.WriteLine();
                Console.Write("            ");
                lPos = 12;
            }
        }

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            lPos = 12;
        }

        //[TestMethod]
        public void CheckIsSubset()
        {
            //foreach (string m in AutomRuleFilter.mots_t_final)
            //{
            //    if (!AutomRuleFilter.mots_t_finalMorph.Contains(m))
            //    {
            //        WriteBlock(m);
            //    }
            //}

            //Assert.IsTrue(AutomRuleFilter.mots_t_final.IsSubsetOf(AutomRuleFilter.mots_t_finalMorph));
        }


        //[TestMethod]
        public void MotsSansAccents()
        {
            //foreach (string v in AutomRuleFilter.mots_t_finalMorph)
            //{
            //    string sansAcc = AutomRuleFilter.ChaineSansAccents(v);
            //    if (sansAcc != v && !AutomRuleFilter.mots_t_finalMorph.Contains(sansAcc))
            //    {
            //        WriteBlock(sansAcc);
            //    }
            //}
        }

        //[TestMethod]
        public void FinalPasS()
        {
            //foreach (string v in AutomRuleFilter.mots_s_final)
            //{
            //    if (v[v.Length - 1] != 's')
            //    {
            //        WriteBlock(v);
            //    }
            //}
        }


        [TestMethod]
        public void PrintConstruct()
        {
            string txt = "vingt";
            TheText tt = new TheText(txt);
            Config conf = new Config();
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                Console.WriteLine(pw.AllStringInfo());
            }
        }

        [TestMethod]
        public void PrintExceptFormat()
        {
            string txt = @"

verumontanum
veto
vetos
vilayet
vilayets
vilebrequin
vilebrequins
voceratrice
voceratrices
vocero
voceros







";
            TheText tt = new TheText(txt);
            Config conf = new Config();
            List<PhonWord> pws = tt.GetPhonWordList(conf);
            foreach (PhonWord pw in pws)
            {
                Console.WriteLine(pw.PourExceptDictionary());
                Console.Write("            ");
            }
        }

        [TestMethod]
        public void WriteRe5()
        {
            HashSet<string> re5Set = new HashSet<string>();
            foreach (string w in motsReE)
            {
                string w5;
                if (w.Length <= 5)
                {
                    w5 = w;
                }
                else
                {
                    w5 = w.Substring(0, 5);
                }
                re5Set.Add(w5);
            }
            List<string> wordList = new List<string>(re5Set);
            wordList.Sort();
            foreach (string s in wordList)
            {
                WriteBlock(s);
            }
        }

        [TestMethod]
        public void WriteRe6()
        {
            HashSet<string> re6Set = new HashSet<string>();
            foreach (string w in motsReE)
            {
                string w6;
                if (w.Length <= 6)
                {
                    w6 = w;
                } 
                else
                {
                    w6 = w.Substring(0, 6);
                }
                re6Set.Add(w6);
            }
            List<string> wordList = new List<string>(re6Set);
            wordList.Sort();
            foreach (string s in wordList)
            {
                WriteBlock(s);
            }
        }

        [TestMethod]
        public void WriteWords()
        {
            HashSet<string> wordSet = new HashSet<string>();
            Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
            MatchCollection matches = rx.Matches(words);
            foreach (Match m in matches)
            {
                wordSet.Add(m.Value); // un doublon n'est ajouté qu'une fois
            }
            List<string> wordList = new List<string>(wordSet);
            wordList.Sort();
            foreach (string s in wordList)
            {
                WriteBlock(s);
            }
        }

        private static HashSet<string> motsReE = new HashSet<string>
        {
            "realia", "recta", "rectal", "rectale", "rectales", "rectangle", "rectangles", "rectangulaire",
            "rectangulaires", "rectas", "rectaux", "rectembryé", "rectembryées", "recteur", "recteurs",
            "recticorne", "rectident", "rectifia", "rectifiable", "rectifiables", "rectifiai",
            "rectifiaient", "rectifiais", "rectifiait", "rectifiâmes", "rectifiant", "rectifias",
            "rectifiasse", "rectifiassent", "rectifiasses", "rectifiassiez", "rectifiassions",
            "rectifiât", "rectifiâtes", "rectificateur", "rectificateurs", "rectificatif", "rectificatifs",
            "rectification", "rectifications", "rectificative", "rectificatives", "rectificatrice",
            "rectificatrices", "rectifie", "rectifié", "rectifiée", "rectifiées", "rectifient",
            "rectifier", "rectifiera", "rectifierai", "rectifieraient", "rectifierais", "rectifierait",
            "rectifieras", "rectifièrent", "rectifierez", "rectifieriez", "rectifierions", "rectifierons",
            "rectifieront", "rectifies", "rectifiés", "rectifieur", "rectifieurs", "rectifieuse",
            "rectifieuses", "rectifiez", "rectifiiez", "rectifiions", "rectifions", "rectiflore",
            "rectiforme", "rectigradation", "rectigrade", "rectilatère", "rectiligne", "rectilignes",
            "rectilinéaire", "rectilinéaires", "rectimètre", "rectinerve", "rection", "rections",
            "rectirostre", "rectisérié", "rectite", "rectites", "rectitude", "rectitudes", "rectiuscule",
            "recto", "rectocurviligne", "rectoral", "rectorale", "rectorales", "rectorat", "rectorats",
            "rectoraux", "rectos", "rectrice", "rectrices", "rectum", "rectums", "reddition",
            "redditions", "redowa", "redowas", "reflex", "reg", "regency", "reggae", "regs", "reichsmark",
            "reichsmarks", "reichstag", "reichstags", "rein", "reine", "reines", "reinette", "reinettes",
            "reins", "reinté", "reintée", "reintées", "reintés", "reis", "reïs", "reître", "reîtres",
            "relaxerse", "remake", "remakes", "rembailler", "remballa", "remballai", "remballaient",
            "remballais", "remballait", "remballâmes", "remballant", "remballas", "remballasse",
            "remballassent", "remballasses", "remballassiez", "remballassions", "remballât", "remballâtes",
            "remballe", "remballé", "remballée", "remballées", "remballent", "remballer", "remballera",
            "remballerai", "remballeraient", "remballerais", "remballerait", "remballeras", "remballèrent",
            "remballerez", "remballeriez", "remballerions", "remballerons", "remballeront", "remballes",
            "remballés", "remballez", "remballiez", "remballions", "remballons", "rembarqua",
            "rembarquai", "rembarquaient", "rembarquais", "rembarquait", "rembarquâmes", "rembarquant",
            "rembarquas", "rembarquasse", "rembarquassent", "rembarquasses", "rembarquassiez",
            "rembarquassions", "rembarquât", "rembarquâtes", "rembarque", "rembarqué", "rembarquée",
            "rembarquées", "rembarquent", "rembarquer", "rembarquera", "rembarquerai", "rembarqueraient",
            "rembarquerais", "rembarquerait", "rembarqueras", "rembarquèrent", "rembarquerez",
            "rembarqueriez", "rembarquerions", "rembarquerons", "rembarqueront", "rembarques",
            "rembarqués", "rembarquez", "rembarquiez", "rembarquions", "rembarquons", "rembarra",
            "rembarrai", "rembarraient", "rembarrais", "rembarrait", "rembarrâmes", "rembarrant",
            "rembarras", "rembarrasse", "rembarrassent", "rembarrasses", "rembarrassiez", "rembarrassions",
            "rembarrât", "rembarrâtes", "rembarre", "rembarré", "rembarrée", "rembarrées", "rembarrent",
            "rembarrer", "rembarrera", "rembarrerai", "rembarreraient", "rembarrerais", "rembarrerait",
            "rembarreras", "rembarrèrent", "rembarrerez", "rembarreriez", "rembarrerions", "rembarrerons",
            "rembarreront", "rembarres", "rembarrés", "rembarrez", "rembarriez", "rembarrions",
            "rembarrons", "remblai", "remblaie", "remblaiement", "remblaiements", "remblaient",
            "remblaiera", "remblaierai", "remblaieraient", "remblaierais", "remblaierait", "remblaieras",
            "remblaierez", "remblaieriez", "remblaierions", "remblaierons", "remblaieront", "remblaies",
            "remblais", "remblaya", "remblayage", "remblayages", "remblayai", "remblayaient",
            "remblayais", "remblayait", "remblayâmes", "remblayant", "remblayas", "remblayasse",
            "remblayassent", "remblayasses", "remblayassiez", "remblayassions", "remblayât", "remblayâtes",
            "remblaye", "remblayé", "remblayée", "remblayées", "remblayent", "remblayer", "remblayera",
            "remblayerai", "remblayeraient", "remblayerais", "remblayerait", "remblayeras", "remblayèrent",
            "remblayerez", "remblayeriez", "remblayerions", "remblayerons", "remblayeront", "remblayes",
            "remblayés", "remblayeur", "remblayeurs", "remblayez", "remblayiez", "remblayions",
            "remblayons", "rembobina", "rembobinai", "rembobinaient", "rembobinais", "rembobinait",
            "rembobinâmes", "rembobinant", "rembobinas", "rembobinasse", "rembobinassent", "rembobinasses",
            "rembobinassiez", "rembobinassions", "rembobinât", "rembobinâtes", "rembobine", "rembobiné",
            "rembobinée", "rembobinées", "rembobinent", "rembobiner", "rembobinera", "rembobinerai",
            "rembobineraient", "rembobinerais", "rembobinerait", "rembobineras", "rembobinèrent",
            "rembobinerez", "rembobineriez", "rembobinerions", "rembobinerons", "rembobineront",
            "rembobines", "rembobinés", "rembobinez", "rembobiniez", "rembobinions", "rembobinons",
            "remboîta", "remboîtage", "remboîtages", "remboîtai", "remboîtaient", "remboîtais",
            "remboîtait", "remboîtâmes", "remboîtant", "remboîtas", "remboîtasse", "remboîtassent",
            "remboîtasses", "remboîtassiez", "remboîtassions", "remboîtât", "remboîtâtes", "remboîte",
            "remboîté", "remboîtée", "remboîtées", "remboîtement", "remboîtements", "remboîtent",
            "remboîter", "remboîtera", "remboîterai", "remboîteraient", "remboîterais", "remboîterait",
            "remboîteras", "remboîtèrent", "remboîterez", "remboîteriez", "remboîterions", "remboîterons",
            "remboîteront", "remboîtes", "remboîtés", "remboîtez", "remboîtiez", "remboîtions",
            "remboîtons", "rembourra", "rembourrage", "rembourrages", "rembourrai", "rembourraient",
            "rembourrais", "rembourrait", "rembourrâmes", "rembourrant", "rembourras", "rembourrasse",
            "rembourrassent", "rembourrasses", "rembourrassiez", "rembourrassions", "rembourrât",
            "rembourrâtes", "rembourre", "rembourré", "rembourrée", "rembourrées", "rembourrent",
            "rembourrer", "rembourrera", "rembourrerai", "rembourreraient", "rembourrerais", "rembourrerait",
            "rembourreras", "rembourrèrent", "rembourrerez", "rembourreriez", "rembourrerions",
            "rembourrerons", "rembourreront", "rembourres", "rembourrés", "rembourrez", "rembourriez",
            "rembourrions", "rembourrons", "rembourrure", "rembourrures", "rembours", "remboursa",
            "remboursable", "remboursables", "remboursai", "remboursaient", "remboursais", "remboursait",
            "remboursâmes", "remboursant", "remboursas", "remboursasse", "remboursassent", "remboursasses",
            "remboursassiez", "remboursassions", "remboursât", "remboursâtes", "rembourse", "remboursé",
            "remboursée", "remboursées", "remboursement", "remboursements", "remboursent", "rembourser",
            "remboursera", "rembourserai", "rembourseraient", "rembourserais", "rembourserait",
            "rembourseras", "remboursèrent", "rembourserez", "rembourseriez", "rembourserions",
            "rembourserons", "rembourseront", "rembourses", "remboursés", "remboursez", "remboursiez",
            "remboursions", "remboursons", "rembraie", "rembraient", "rembraiera", "rembraierai",
            "rembraieraient", "rembraierais", "rembraierait", "rembraieras", "rembraierez", "rembraieriez",
            "rembraierions", "rembraierons", "rembraieront", "rembraies", "rembrandtesque", "rembrandtesques",
            "rembranesque", "rembranesquement", "rembranesques", "rembraya", "rembrayage", "rembrayai",
            "rembrayaient", "rembrayais", "rembrayait", "rembrayâmes", "rembrayant", "rembrayas",
            "rembrayasse", "rembrayassent", "rembrayasses", "rembrayassiez", "rembrayassions",
            "rembrayât", "rembrayâtes", "rembraye", "rembrayé", "rembrayée", "rembrayées", "rembrayent",
            "rembrayer", "rembrayera", "rembrayerai", "rembrayeraient", "rembrayerais", "rembrayerait",
            "rembrayeras", "rembrayèrent", "rembrayerez", "rembrayeriez", "rembrayerions", "rembrayerons",
            "rembrayeront", "rembrayes", "rembrayés", "rembrayez", "rembrayiez", "rembrayions",
            "rembrayons", "rembruni", "rembrunie", "rembrunies", "rembrunîmes", "rembrunir", "rembrunira",
            "rembrunirai", "rembruniraient", "rembrunirais", "rembrunirait", "rembruniras", "rembrunirent",
            "rembrunirez", "rembruniriez", "rembrunirions", "rembrunirons", "rembruniront", "rembrunis",
            "rembrunissaient", "rembrunissais", "rembrunissait", "rembrunissant", "rembrunisse",
            "rembrunissement", "rembrunissements", "rembrunissent", "rembrunisses", "rembrunissez",
            "rembrunissiez", "rembrunissions", "rembrunissons", "rembrunit", "rembrunît", "rembrunîtes",
            "rembucha", "rembûcha", "rembuchai", "rembûchai", "rembuchaient", "rembûchaient",
            "rembuchais", "rembûchais", "rembuchait", "rembûchait", "rembuchâmes", "rembûchâmes",
            "rembuchant", "rembûchant", "rembuchas", "rembûchas", "rembuchasse", "rembûchasse",
            "rembuchassent", "rembûchassent", "rembuchasses", "rembûchasses", "rembuchassiez",
            "rembûchassiez", "rembuchassions", "rembûchassions", "rembuchât", "rembûchât", "rembuchâtes",
            "rembûchâtes", "rembuche", "rembûche", "rembuché", "rembûché", "rembuchée", "rembûchée",
            "rembuchées", "rembûchées", "rembuchement", "rembuchements", "rembuchent", "rembûchent",
            "rembucher", "rembûcher", "rembuchera", "rembûchera", "rembucherai", "rembûcherai",
            "rembucheraient", "rembûcheraient", "rembucherais", "rembûcherais", "rembucherait",
            "rembûcherait", "rembucheras", "rembûcheras", "rembuchèrent", "rembûchèrent", "rembucherez",
            "rembûcherez", "rembucheriez", "rembûcheriez", "rembucherions", "rembûcherions", "rembucherons",
            "rembûcherons", "rembucheront", "rembûcheront", "rembuches", "rembûches", "rembuchés",
            "rembûchés", "rembuchez", "rembûchez", "rembuchiez", "rembûchiez", "rembuchions",
            "rembûchions", "rembuchons", "rembûchons", "remmailla", "remmaillage", "remmaillages",
            "remmaillai", "remmaillaient", "remmaillais", "remmaillait", "remmaillâmes", "remmaillant",
            "remmaillas", "remmaillasse", "remmaillassent", "remmaillasses", "remmaillassiez",
            "remmaillassions", "remmaillât", "remmaillâtes", "remmaille", "remmaillé", "remmaillée",
            "remmaillées", "remmaillent", "remmailler", "remmaillera", "remmaillerai", "remmailleraient",
            "remmaillerais", "remmaillerait", "remmailleras", "remmaillèrent", "remmaillerez",
            "remmailleriez", "remmaillerions", "remmaillerons", "remmailleront", "remmailles",
            "remmaillés", "remmailleur", "remmailleurs", "remmailleuse", "remmailleuses", "remmaillez",
            "remmailliez", "remmaillions", "remmaillons", "remmaillota", "remmaillotai", "remmaillotaient",
            "remmaillotais", "remmaillotait", "remmaillotâmes", "remmaillotant", "remmaillotas",
            "remmaillotasse", "remmaillotassent", "remmaillotasses", "remmaillotassiez", "remmaillotassions",
            "remmaillotât", "remmaillotâtes", "remmaillote", "remmailloté", "remmaillotée", "remmaillotées",
            "remmaillotent", "remmailloter", "remmaillotera", "remmailloterai", "remmailloteraient",
            "remmailloterais", "remmailloterait", "remmailloteras", "remmaillotèrent", "remmailloterez",
            "remmailloteriez", "remmailloterions", "remmailloterons", "remmailloteront", "remmaillotes",
            "remmaillotés", "remmaillotez", "remmaillotiez", "remmaillotions", "remmaillotons",
            "remmancha", "remmanchai", "remmanchaient", "remmanchais", "remmanchait", "remmanchâmes",
            "remmanchant", "remmanchas", "remmanchasse", "remmanchassent", "remmanchasses", "remmanchassiez",
            "remmanchassions", "remmanchât", "remmanchâtes", "remmanche", "remmanché", "remmanchée",
            "remmanchées", "remmanchent", "remmancher", "remmanchera", "remmancherai", "remmancheraient",
            "remmancherais", "remmancherait", "remmancheras", "remmanchèrent", "remmancherez",
            "remmancheriez", "remmancherions", "remmancherons", "remmancheront", "remmanches",
            "remmanchés", "remmancheur", "remmanchez", "remmanchiez", "remmanchions", "remmanchons",
            "remmena", "remmenai", "remmenaient", "remmenais", "remmenait", "remmenâmes", "remmenant",
            "remmenas", "remmenasse", "remmenassent", "remmenasses", "remmenassiez", "remmenassions",
            "remmenât", "remmenâtes", "remmène", "remmené", "remmenée", "remmenées", "remmènent",
            "remmener", "remmènera", "remmènerai", "remmèneraient", "remmènerais", "remmènerait",
            "remmèneras", "remmenèrent", "remmènerez", "remmèneriez", "remmènerions", "remmènerons",
            "remmèneront", "remmènes", "remmenés", "remmenez", "remmeniez", "remmenions", "remmenons",
            "rempailla", "rempaillage", "rempaillages", "rempaillai", "rempaillaient", "rempaillais",
            "rempaillait", "rempaillâmes", "rempaillant", "rempaillas", "rempaillasse", "rempaillassent",
            "rempaillasses", "rempaillassiez", "rempaillassions", "rempaillât", "rempaillâtes",
            "rempaille", "rempaillé", "rempaillée", "rempaillées", "rempaillent", "rempailler",
            "rempaillera", "rempaillerai", "rempailleraient", "rempaillerais", "rempaillerait",
            "rempailleras", "rempaillèrent", "rempaillerez", "rempailleriez", "rempaillerions",
            "rempaillerons", "rempailleront", "rempailles", "rempaillés", "rempailleur", "rempailleurs",
            "rempailleuse", "rempailleuses", "rempaillez", "rempailliez", "rempaillions", "rempaillons",
            "rempara", "remparai", "remparaient", "remparais", "remparait", "remparâmes", "remparant",
            "remparas", "remparasse", "remparassent", "remparasses", "remparassiez", "remparassions",
            "remparât", "remparâtes", "rempare", "remparé", "remparée", "remparées", "remparent",
            "remparer", "remparera", "remparerai", "rempareraient", "remparerais", "remparerait",
            "rempareras", "remparèrent", "remparerez", "rempareriez", "remparerions", "remparerons",
            "rempareront", "rempares", "remparés", "remparez", "rempariez", "remparions", "remparons",
            "rempart", "remparts", "rempiéta", "rempiétai", "rempiétaient", "rempiétais", "rempiétait",
            "rempiétâmes", "rempiétant", "rempiétas", "rempiétasse", "rempiétassent", "rempiétasses",
            "rempiétassiez", "rempiétassions", "rempiétât", "rempiétâtes", "rempiète", "rempiété",
            "rempiétée", "rempiétées", "rempiétement", "rempiétements", "rempiètent", "rempiéter",
            "rempiétera", "rempiéterai", "rempiéteraient", "rempiéterais", "rempiéterait", "rempiéteras",
            "rempiétèrent", "rempiéterez", "rempiéteriez", "rempiéterions", "rempiéterons", "rempiéteront",
            "rempiètes", "rempiétés", "rempiétez", "rempiétiez", "rempiétions", "rempiétons",
            "rempila", "rempilai", "rempilaient", "rempilais", "rempilait", "rempilâmes", "rempilant",
            "rempilas", "rempilasse", "rempilassent", "rempilasses", "rempilassiez", "rempilassions",
            "rempilât", "rempilâtes", "rempile", "rempilé", "rempilée", "rempilées", "rempilent",
            "rempiler", "rempilera", "rempilerai", "rempileraient", "rempilerais", "rempilerait",
            "rempileras", "rempilèrent", "rempilerez", "rempileriez", "rempilerions", "rempilerons",
            "rempileront", "rempiles", "rempilés", "rempilez", "rempiliez", "rempilions", "rempilons",
            "remplaça", "remplaçable", "remplaçables", "remplaçai", "remplaçaient", "remplaçais",
            "remplaçait", "remplaçâmes", "remplaçant", "remplaçante", "remplaçantes", "remplaçants",
            "remplaças", "remplaçasse", "remplaçassent", "remplaçasses", "remplaçassiez", "remplaçassions",
            "remplaçât", "remplaçâtes", "remplace", "remplacé", "remplacée", "remplacées", "remplacement",
            "remplacements", "remplacent", "remplacer", "remplacera", "remplacerai", "remplaceraient",
            "remplacerais", "remplacerait", "remplaceras", "remplacèrent", "remplacerez", "remplaceriez",
            "remplacerions", "remplacerons", "remplaceront", "remplaces", "remplacés", "remplacez",
            "remplaciez", "remplacions", "remplaçons", "remplage", "remplages", "rempli", "remplia",
            "rempliai", "rempliaient", "rempliais", "rempliait", "rempliâmes", "rempliant", "remplias",
            "rempliasse", "rempliassent", "rempliasses", "rempliassiez", "rempliassions", "rempliât",
            "rempliâtes", "remplie", "remplié", "rempliée", "rempliées", "remplient", "remplier",
            "rempliera", "remplierai", "remplieraient", "remplierais", "remplierait", "remplieras",
            "remplièrent", "remplierez", "remplieriez", "remplierions", "remplierons", "remplieront",
            "remplies", "rempliés", "rempliez", "rempliiez", "rempliions", "remplîmes", "remplions",
            "remplir", "remplira", "remplirai", "rempliraient", "remplirais", "remplirait", "rempliras",
            "remplirent", "remplirez", "rempliriez", "remplirions", "remplirons", "rempliront",
            "remplis", "remplissage", "remplissages", "remplissaient", "remplissais", "remplissait",
            "remplissant", "remplisse", "remplissement", "remplissements", "remplissent", "remplisses",
            "remplisseur", "remplisseurs", "remplisseuse", "remplisseuses", "remplissez", "remplissiez",
            "remplissions", "remplissons", "remplit", "remplît", "remplîtes", "remploi", "remploie",
            "remploient", "remploiera", "remploierai", "remploieraient", "remploierais", "remploierait",
            "remploieras", "remploierez", "remploieriez", "remploierions", "remploierons", "remploieront",
            "remploies", "remplois", "remploya", "remployai", "remployaient", "remployais", "remployait",
            "remployâmes", "remployant", "remployas", "remployasse", "remployassent", "remployasses",
            "remployassiez", "remployassions", "remployât", "remployâtes", "remployé", "remployée",
            "remployées", "remployer", "remployèrent", "remployés", "remployez", "remployiez",
            "remployions", "remployons", "rempluma", "remplumai", "remplumaient", "remplumais",
            "remplumait", "remplumâmes", "remplumant", "remplumas", "remplumasse", "remplumassent",
            "remplumasses", "remplumassiez", "remplumassions", "remplumât", "remplumâtes", "remplume",
            "remplumé", "remplumée", "remplumées", "remplument", "remplumer", "remplumera", "remplumerai",
            "remplumeraient", "remplumerais", "remplumerait", "remplumeras", "remplumèrent", "remplumerez",
            "remplumeriez", "remplumerions", "remplumerons", "remplumeront", "remplumes", "remplumés",
            "remplumez", "remplumiez", "remplumions", "remplumons", "rempocha", "rempochai", "rempochaient",
            "rempochais", "rempochait", "rempochâmes", "rempochant", "rempochas", "rempochasse",
            "rempochassent", "rempochasses", "rempochassiez", "rempochassions", "rempochât", "rempochâtes",
            "rempoche", "rempoché", "rempochée", "rempochées", "rempochent", "rempocher", "rempochera",
            "rempocherai", "rempocheraient", "rempocherais", "rempocherait", "rempocheras", "rempochèrent",
            "rempocherez", "rempocheriez", "rempocherions", "rempocherons", "rempocheront", "rempoches",
            "rempochés", "rempochez", "rempochiez", "rempochions", "rempochons", "rempoigner",
            "rempoissonna", "rempoissonnai", "rempoissonnaient", "rempoissonnais", "rempoissonnait",
            "rempoissonnâmes", "rempoissonnant", "rempoissonnas", "rempoissonnasse", "rempoissonnassent",
            "rempoissonnasses", "rempoissonnassiez", "rempoissonnassions", "rempoissonnât", "rempoissonnâtes",
            "rempoissonne", "rempoissonné", "rempoissonnée", "rempoissonnées", "rempoissonnement",
            "rempoissonnements", "rempoissonnent", "rempoissonner", "rempoissonnera", "rempoissonnerai",
            "rempoissonneraient", "rempoissonnerais", "rempoissonnerait", "rempoissonneras", "rempoissonnèrent",
            "rempoissonnerez", "rempoissonneriez", "rempoissonnerions", "rempoissonnerons", "rempoissonneront",
            "rempoissonnes", "rempoissonnés", "rempoissonnez", "rempoissonniez", "rempoissonnions",
            "rempoissonnons", "remporta", "remportai", "remportaient", "remportais", "remportait",
            "remportâmes", "remportant", "remportas", "remportasse", "remportassent", "remportasses",
            "remportassiez", "remportassions", "remportât", "remportâtes", "remporte", "remporté",
            "remportée", "remportées", "remportent", "remporter", "remportera", "remporterai",
            "remporteraient", "remporterais", "remporterait", "remporteras", "remportèrent", "remporterez",
            "remporteriez", "remporterions", "remporterons", "remporteront", "remportes", "remportés",
            "remportez", "remportiez", "remportions", "remportons", "rempota", "rempotage", "rempotages",
            "rempotai", "rempotaient", "rempotais", "rempotait", "rempotâmes", "rempotant", "rempotas",
            "rempotasse", "rempotassent", "rempotasses", "rempotassiez", "rempotassions", "rempotât",
            "rempotâtes", "rempote", "rempoté", "rempotée", "rempotées", "rempotent", "rempoter",
            "rempotera", "rempoterai", "rempoteraient", "rempoterais", "rempoterait", "rempoteras",
            "rempotèrent", "rempoterez", "rempoteriez", "rempoterions", "rempoterons", "rempoteront",
            "rempotes", "rempotés", "rempotez", "rempotiez", "rempotions", "rempotons", "rencaissa",
            "rencaissage", "rencaissages", "rencaissai", "rencaissaient", "rencaissais", "rencaissait",
            "rencaissâmes", "rencaissant", "rencaissas", "rencaissasse", "rencaissassent", "rencaissasses",
            "rencaissassiez", "rencaissassions", "rencaissât", "rencaissâtes", "rencaisse", "rencaissé",
            "rencaissée", "rencaissées", "rencaissement", "rencaissements", "rencaissent", "rencaisser",
            "rencaissera", "rencaisserai", "rencaisseraient", "rencaisserais", "rencaisserait",
            "rencaisseras", "rencaissèrent", "rencaisserez", "rencaisseriez", "rencaisserions",
            "rencaisserons", "rencaisseront", "rencaisses", "rencaissés", "rencaissez", "rencaissiez",
            "rencaissions", "rencaissons", "rencard", "rencarda", "rencardai", "rencardaient",
            "rencardais", "rencardait", "rencardâmes", "rencardant", "rencardas", "rencardasse",
            "rencardassent", "rencardasses", "rencardassiez", "rencardassions", "rencardât", "rencardâtes",
            "rencarde", "rencardé", "rencardée", "rencardées", "rencardent", "rencarder", "rencardera",
            "rencarderai", "rencarderaient", "rencarderais", "rencarderait", "rencarderas", "rencardèrent",
            "rencarderez", "rencarderiez", "rencarderions", "rencarderons", "rencarderont", "rencardes",
            "rencardés", "rencardez", "rencardiez", "rencardions", "rencardons", "rencards", "rencart",
            "rencarts", "renchaussage", "renchéri", "renchérie", "renchéries", "renchérîmes",
            "renchérir", "renchérira", "renchérirai", "renchériraient", "renchérirais", "renchérirait",
            "renchériras", "renchérirent", "renchérirez", "renchéririez", "renchéririons", "renchérirons",
            "renchériront", "renchéris", "renchérissaient", "renchérissais", "renchérissait",
            "renchérissant", "renchérisse", "renchérissement", "renchérissements", "renchérissent",
            "renchérisses", "renchérisseur", "renchérisseurs", "renchérisseuse", "renchérisseuses",
            "renchérissez", "renchérissiez", "renchérissions", "renchérissons", "renchérit", "renchérît",
            "renchérîtes", "renclore", "renclôture", "rencogna", "rencognai", "rencognaient",
            "rencognais", "rencognait", "rencognâmes", "rencognant", "rencognas", "rencognasse",
            "rencognassent", "rencognasses", "rencognassiez", "rencognassions", "rencognât", "rencognâtes",
            "rencogne", "rencogné", "rencognée", "rencognées", "rencognent", "rencogner", "rencognera",
            "rencognerai", "rencogneraient", "rencognerais", "rencognerait", "rencogneras", "rencognèrent",
            "rencognerez", "rencogneriez", "rencognerions", "rencognerons", "rencogneront", "rencognes",
            "rencognés", "rencognez", "rencogniez", "rencognions", "rencognons", "rencoigna",
            "rencoignai", "rencoignaient", "rencoignais", "rencoignait", "rencoignâmes", "rencoignant",
            "rencoignas", "rencoignasse", "rencoignassent", "rencoignasses", "rencoignassiez",
            "rencoignassions", "rencoignât", "rencoignâtes", "rencoigne", "rencoigné", "rencoignée",
            "rencoignées", "rencoignent", "rencoigner", "rencoignera", "rencoignerai", "rencoigneraient",
            "rencoignerais", "rencoignerait", "rencoigneras", "rencoignèrent", "rencoignerez",
            "rencoigneriez", "rencoignerions", "rencoignerons", "rencoigneront", "rencoignes",
            "rencoignés", "rencoignez", "rencoigniez", "rencoignions", "rencoignons", "rencontra",
            "rencontrable", "rencontrai", "rencontraient", "rencontrais", "rencontrait", "rencontrâmes",
            "rencontrant", "rencontras", "rencontrasse", "rencontrassent", "rencontrasses", "rencontrassiez",
            "rencontrassions", "rencontrât", "rencontrâtes", "rencontre", "rencontré", "rencontrée",
            "rencontrées", "rencontrent", "rencontrer", "rencontrera", "rencontrerai", "rencontreraient",
            "rencontrerais", "rencontrerait", "rencontreras", "rencontrèrent", "rencontrerez",
            "rencontreriez", "rencontrerions", "rencontrerons", "rencontreront", "rencontres",
            "rencontrés", "rencontrez", "rencontriez", "rencontrions", "rencontrons", "rencourager",
            "rend", "rendaient", "rendais", "rendait", "rendant", "rende", "rendement", "rendements",
            "rendent", "rendes", "rendeur", "rendeurs", "rendeuse", "rendeuses", "rendez", "rendiez",
            "rendîmes", "rendions", "rendirent", "rendis", "rendisse", "rendissent", "rendisses",
            "rendissiez", "rendissions", "rendit", "rendît", "rendîtes", "rendons", "rendormaient",
            "rendormais", "rendormait", "rendormant", "rendorme", "rendorment", "rendormes", "rendormez",
            "rendormi", "rendormie", "rendormies", "rendormiez", "rendormîmes", "rendormions",
            "rendormir", "rendormira", "rendormirai", "rendormiraient", "rendormirais", "rendormirait",
            "rendormiras", "rendormirent", "rendormirez", "rendormiriez", "rendormirions", "rendormirons",
            "rendormiront", "rendormis", "rendormisse", "rendormissent", "rendormisses", "rendormissiez",
            "rendormissions", "rendormit", "rendormît", "rendormîtes", "rendormons", "rendors",
            "rendort", "rendra", "rendrai", "rendraient", "rendrais", "rendrait", "rendras", "rendre",
            "rendrez", "rendriez", "rendrions", "rendrons", "rendront", "rends", "rendu", "rendue",
            "rendues", "rendus", "renfaîta", "renfaîtage", "renfaîtages", "renfaîtai", "renfaîtaient",
            "renfaîtais", "renfaîtait", "renfaîtâmes", "renfaîtant", "renfaîtas", "renfaîtasse",
            "renfaîtassent", "renfaîtasses", "renfaîtassiez", "renfaîtassions", "renfaîtât", "renfaîtâtes",
            "renfaîte", "renfaîté", "renfaîtée", "renfaîtées", "renfaîtent", "renfaîter", "renfaîtera",
            "renfaîterai", "renfaîteraient", "renfaîterais", "renfaîterait", "renfaîteras", "renfaîtèrent",
            "renfaîterez", "renfaîteriez", "renfaîterions", "renfaîterons", "renfaîteront", "renfaîtes",
            "renfaîtés", "renfaîtez", "renfaîtiez", "renfaîtions", "renfaîtons", "renferma", "renfermai",
            "renfermaient", "renfermais", "renfermait", "renfermâmes", "renfermant", "renfermas",
            "renfermasse", "renfermassent", "renfermasses", "renfermassiez", "renfermassions",
            "renfermât", "renfermâtes", "renferme", "renfermé", "renfermée", "renfermées", "renferment",
            "renfermer", "renfermera", "renfermerai", "renfermeraient", "renfermerais", "renfermerait",
            "renfermeras", "renfermèrent", "renfermerez", "renfermeriez", "renfermerions", "renfermerons",
            "renfermeront", "renfermes", "renfermés", "renfermez", "renfermiez", "renfermions",
            "renfermons", "renfiévrer", "renfila", "renfilai", "renfilaient", "renfilais", "renfilait",
            "renfilâmes", "renfilant", "renfilas", "renfilasse", "renfilassent", "renfilasses",
            "renfilassiez", "renfilassions", "renfilât", "renfilâtes", "renfile", "renfilé", "renfilée",
            "renfilées", "renfilent", "renfiler", "renfilera", "renfilerai", "renfileraient",
            "renfilerais", "renfilerait", "renfileras", "renfilèrent", "renfilerez", "renfileriez",
            "renfilerions", "renfilerons", "renfileront", "renfiles", "renfilés", "renfilez",
            "renfiliez", "renfilions", "renfilons", "renfla", "renflai", "renflaient", "renflais",
            "renflait", "renflâmes", "renflamma", "renflammai", "renflammaient", "renflammais",
            "renflammait", "renflammâmes", "renflammant", "renflammas", "renflammasse", "renflammassent",
            "renflammasses", "renflammassiez", "renflammassions", "renflammât", "renflammâtes",
            "renflamme", "renflammé", "renflammée", "renflammées", "renflamment", "renflammer",
            "renflammera", "renflammerai", "renflammeraient", "renflammerais", "renflammerait",
            "renflammeras", "renflammèrent", "renflammerez", "renflammeriez", "renflammerions",
            "renflammerons", "renflammeront", "renflammes", "renflammés", "renflammez", "renflammiez",
            "renflammions", "renflammons", "renflant", "renflas", "renflasse", "renflassent",
            "renflasses", "renflassiez", "renflassions", "renflât", "renflâtes", "renfle", "renflé",
            "renflée", "renflées", "renflement", "renflements", "renflent", "renfler", "renflera",
            "renflerai", "renfleraient", "renflerais", "renflerait", "renfleras", "renflèrent",
            "renflerez", "renfleriez", "renflerions", "renflerons", "renfleront", "renfles", "renflés",
            "renflez", "renfliez", "renflions", "renflons", "renfloua", "renflouable", "renflouables",
            "renflouage", "renflouages", "renflouai", "renflouaient", "renflouais", "renflouait",
            "renflouâmes", "renflouant", "renflouas", "renflouasse", "renflouassent", "renflouasses",
            "renflouassiez", "renflouassions", "renflouât", "renflouâtes", "renfloue", "renfloué",
            "renflouée", "renflouées", "renflouement", "renflouements", "renflouent", "renflouer",
            "renflouera", "renflouerai", "renfloueraient", "renflouerais", "renflouerait", "renfloueras",
            "renflouèrent", "renflouerez", "renfloueriez", "renflouerions", "renflouerons", "renfloueront",
            "renfloues", "renfloués", "renflouez", "renflouiez", "renflouions", "renflouons",
            "renflure", "renfonça", "renfonçai", "renfonçaient", "renfonçais", "renfonçait", "renfonçâmes",
            "renfonçant", "renfonças", "renfonçasse", "renfonçassent", "renfonçasses", "renfonçassiez",
            "renfonçassions", "renfonçât", "renfonçâtes", "renfonce", "renfoncé", "renfoncée",
            "renfoncées", "renfoncement", "renfoncements", "renfoncent", "renfoncer", "renfoncera",
            "renfoncerai", "renfonceraient", "renfoncerais", "renfoncerait", "renfonceras", "renfoncèrent",
            "renfoncerez", "renfonceriez", "renfoncerions", "renfoncerons", "renfonceront", "renfonces",
            "renfoncés", "renfoncez", "renfonciez", "renfoncions", "renfonçons", "renforça", "renforçai",
            "renforçaient", "renforçais", "renforçait", "renforçâmes", "renforçant", "renforças",
            "renforçasse", "renforçassent", "renforçasses", "renforçassiez", "renforçassions",
            "renforçât", "renforçâtes", "renforçateur", "renforçateurs", "renforçatrice", "renforçatrices",
            "renforce", "renforcé", "renforcée", "renforcées", "renforcement", "renforcements",
            "renforcent", "renforcer", "renforcera", "renforcerai", "renforceraient", "renforcerais",
            "renforcerait", "renforceras", "renforcèrent", "renforcerez", "renforceriez", "renforcerions",
            "renforcerons", "renforceront", "renforces", "renforcés", "renforcez", "renforciez",
            "renforcions", "renforçons", "renformi", "renformie", "renformies", "renformîmes",
            "renformir", "renformira", "renformirai", "renformiraient", "renformirais", "renformirait",
            "renformiras", "renformirent", "renformirez", "renformiriez", "renformirions", "renformirons",
            "renformiront", "renformis", "renformissaient", "renformissais", "renformissait",
            "renformissant", "renformisse", "renformissent", "renformisses", "renformissez", "renformissiez",
            "renformissions", "renformissons", "renformit", "renformît", "renformîtes", "renfort",
            "renforts", "renfrogna", "renfrognai", "renfrognaient", "renfrognais", "renfrognait",
            "renfrognâmes", "renfrognant", "renfrognas", "renfrognasse", "renfrognassent", "renfrognasses",
            "renfrognassiez", "renfrognassions", "renfrognât", "renfrognâtes", "renfrogne", "renfrogné",
            "renfrognée", "renfrognées", "renfrognement", "renfrognements", "renfrognent", "renfrogner",
            "renfrognera", "renfrognerai", "renfrogneraient", "renfrognerais", "renfrognerait",
            "renfrogneras", "renfrognèrent", "renfrognerez", "renfrogneriez", "renfrognerions",
            "renfrognerons", "renfrogneront", "renfrognes", "renfrognés", "renfrognez", "renfrogniez",
            "renfrognions", "renfrognons", "rengage", "rengagé", "rengagea", "rengageai", "rengageaient",
            "rengageais", "rengageait", "rengageâmes", "rengageant", "rengageas", "rengageasse",
            "rengageassent", "rengageasses", "rengageassiez", "rengageassions", "rengageât", "rengageâtes",
            "rengagée", "rengagées", "rengagement", "rengagements", "rengagent", "rengageons",
            "rengager", "rengagera", "rengagerai", "rengageraient", "rengagerais", "rengagerait",
            "rengageras", "rengagèrent", "rengagerez", "rengageriez", "rengagerions", "rengagerons",
            "rengageront", "rengages", "rengagés", "rengagez", "rengagiez", "rengagions", "rengaina",
            "rengainai", "rengainaient", "rengainais", "rengainait", "rengainâmes", "rengainant",
            "rengainard", "rengainarde", "rengainas", "rengainasse", "rengainassent", "rengainasses",
            "rengainassiez", "rengainassions", "rengainât", "rengainâtes", "rengaine", "rengainé",
            "rengainée", "rengainées", "rengainent", "rengainer", "rengainera", "rengainerai",
            "rengaineraient", "rengainerais", "rengainerait", "rengaineras", "rengainèrent", "rengainerez",
            "rengaineriez", "rengainerions", "rengainerons", "rengaineront", "rengaines", "rengainés",
            "rengainez", "rengainiez", "rengainions", "rengainons", "rengorge", "rengorgé", "rengorgea",
            "rengorgeai", "rengorgeaient", "rengorgeais", "rengorgeait", "rengorgeâmes", "rengorgeant",
            "rengorgeas", "rengorgeasse", "rengorgeassent", "rengorgeasses", "rengorgeassiez",
            "rengorgeassions", "rengorgeât", "rengorgeâtes", "rengorgée", "rengorgées", "rengorgement",
            "rengorgements", "rengorgent", "rengorgeons", "rengorger", "rengorgera", "rengorgerai",
            "rengorgeraient", "rengorgerais", "rengorgerait", "rengorgeras", "rengorgèrent", "rengorgerez",
            "rengorgeriez", "rengorgerions", "rengorgerons", "rengorgeront", "rengorges", "rengorgés",
            "rengorgez", "rengorgiez", "rengorgions", "rengraissa", "rengraissai", "rengraissaient",
            "rengraissais", "rengraissait", "rengraissâmes", "rengraissant", "rengraissas", "rengraissasse",
            "rengraissassent", "rengraissasses", "rengraissassiez", "rengraissassions", "rengraissât",
            "rengraissâtes", "rengraisse", "rengraissé", "rengraissée", "rengraissées", "rengraissent",
            "rengraisser", "rengraissera", "rengraisserai", "rengraisseraient", "rengraisserais",
            "rengraisserait", "rengraisseras", "rengraissèrent", "rengraisserez", "rengraisseriez",
            "rengraisserions", "rengraisserons", "rengraisseront", "rengraisses", "rengraissés",
            "rengraissez", "rengraissiez", "rengraissions", "rengraissons", "rengrège", "rengrégé",
            "rengrégea", "rengrégeai", "rengrégeaient", "rengrégeais", "rengrégeait", "rengrégeâmes",
            "rengrégeant", "rengrégeas", "rengrégeasse", "rengrégeassent", "rengrégeasses", "rengrégeassiez",
            "rengrégeassions", "rengrégeât", "rengrégeâtes", "rengrégée", "rengrégées", "rengrègent",
            "rengrégeons", "rengréger", "rengrégera", "rengrégerai", "rengrégeraient", "rengrégerais",
            "rengrégerait", "rengrégeras", "rengrégèrent", "rengrégerez", "rengrégeriez", "rengrégerions",
            "rengrégerons", "rengrégeront", "rengrèges", "rengrégés", "rengrégez", "rengrégiez",
            "rengrégions", "rengréna", "rengrénai", "rengrénaient", "rengrénais", "rengrénait",
            "rengrénâmes", "rengrénant", "rengrénas", "rengrénasse", "rengrénassent", "rengrénasses",
            "rengrénassiez", "rengrénassions", "rengrénât", "rengrénâtes", "rengrène", "rengréné",
            "rengrénée", "rengrénées", "rengrènement", "rengrènements", "rengrènent", "rengréner",
            "rengrénera", "rengrénerai", "rengréneraient", "rengrénerais", "rengrénerait", "rengréneras",
            "rengrénèrent", "rengrénerez", "rengréneriez", "rengrénerions", "rengrénerons", "rengréneront",
            "rengrènes", "rengrénés", "rengrénez", "rengréniez", "rengrénions", "rengrénons",
            "renne", "rennes", "renquilla", "renquillai", "renquillaient", "renquillais", "renquillait",
            "renquillâmes", "renquillant", "renquillas", "renquillasse", "renquillassent", "renquillasses",
            "renquillassiez", "renquillassions", "renquillât", "renquillâtes", "renquille", "renquillé",
            "renquillée", "renquillées", "renquillent", "renquiller", "renquillera", "renquillerai",
            "renquilleraient", "renquillerais", "renquillerait", "renquilleras", "renquillèrent",
            "renquillerez", "renquilleriez", "renquillerions", "renquillerons", "renquilleront",
            "renquilles", "renquillés", "renquillez", "renquilliez", "renquillions", "renquillons",
            "renseigna", "renseignai", "renseignaient", "renseignais", "renseignait", "renseignâmes",
            "renseignant", "renseignas", "renseignasse", "renseignassent", "renseignasses", "renseignassiez",
            "renseignassions", "renseignât", "renseignâtes", "renseigne", "renseigné", "renseignée",
            "renseignées", "renseignement", "renseignements", "renseignent", "renseigner", "renseignera",
            "renseignerai", "renseigneraient", "renseignerais", "renseignerait", "renseigneras",
            "renseignèrent", "renseignerez", "renseigneriez", "renseignerions", "renseignerons",
            "renseigneront", "renseignes", "renseignés", "renseigneur", "renseigneuse", "renseignez",
            "renseigniez", "renseignions", "renseignons", "renta", "rentabilisa", "rentabilisai",
            "rentabilisaient", "rentabilisais", "rentabilisait", "rentabilisâmes", "rentabilisant",
            "rentabilisas", "rentabilisasse", "rentabilisassent", "rentabilisasses", "rentabilisassiez",
            "rentabilisassions", "rentabilisât", "rentabilisâtes", "rentabilise", "rentabilisé",
            "rentabilisée", "rentabilisées", "rentabilisent", "rentabiliser", "rentabilisera",
            "rentabiliserai", "rentabiliseraient", "rentabiliserais", "rentabiliserait", "rentabiliseras",
            "rentabilisèrent", "rentabiliserez", "rentabiliseriez", "rentabiliserions", "rentabiliserons",
            "rentabiliseront", "rentabilises", "rentabilisés", "rentabilisez", "rentabilisiez",
            "rentabilisions", "rentabilisons", "rentabilité", "rentabilités", "rentable", "rentables",
            "rentai", "rentaient", "rentais", "rentait", "rentâmes", "rentant", "rentas", "rentasse",
            "rentassent", "rentasses", "rentassiez", "rentassions", "rentât", "rentâtes", "rente",
            "renté", "rentée", "rentées", "rentent", "renter", "rentera", "renterai", "renteraient",
            "renterais", "renterait", "renteras", "rentèrent", "renterez", "renteriez", "renterions",
            "renterons", "renteront", "rentes", "rentés", "rentez", "rentier", "rentière", "rentières",
            "rentiers", "rentiez", "rentions", "rentoila", "rentoilage", "rentoilages", "rentoilai",
            "rentoilaient", "rentoilais", "rentoilait", "rentoilâmes", "rentoilant", "rentoilas",
            "rentoilasse", "rentoilassent", "rentoilasses", "rentoilassiez", "rentoilassions",
            "rentoilât", "rentoilâtes", "rentoile", "rentoilé", "rentoilée", "rentoilées", "rentoilent",
            "rentoiler", "rentoilera", "rentoilerai", "rentoileraient", "rentoilerais", "rentoilerait",
            "rentoileras", "rentoilèrent", "rentoilerez", "rentoileriez", "rentoilerions", "rentoilerons",
            "rentoileront", "rentoiles", "rentoilés", "rentoileur", "rentoileurs", "rentoilez",
            "rentoiliez", "rentoilions", "rentoilons", "rentons", "rentra", "rentrage", "rentrages",
            "rentrai", "rentraient", "rentraire", "rentrais", "rentrait", "rentraiture", "rentraitures",
            "rentrâmes", "rentrant", "rentrante", "rentrantes", "rentrants", "rentras", "rentrasse",
            "rentrassent", "rentrasses", "rentrassiez", "rentrassions", "rentrât", "rentrâtes",
            "rentrayage", "rentrayages", "rentrayeur", "rentrayeurs", "rentrayeuse", "rentrayeuses",
            "rentre", "rentré", "rentrée", "rentrées", "rentrent", "rentrer", "rentrera", "rentrerai",
            "rentreraient", "rentrerais", "rentrerait", "rentreras", "rentrèrent", "rentrerez",
            "rentreriez", "rentrerions", "rentrerons", "rentreront", "rentres", "rentrés", "rentreur",
            "rentrez", "rentriez", "rentrions", "rentrons", "rentrure", "rentrures", "renverra",
            "renverrai", "renverraient", "renverrais", "renverrait", "renverras", "renverrez",
            "renverriez", "renverrions", "renverrons", "renverront", "renversa", "renversable",
            "renversables", "renversai", "renversaient", "renversais", "renversait", "renversâmes",
            "renversant", "renversante", "renversantes", "renversants", "renversas", "renversasse",
            "renversassent", "renversasses", "renversassiez", "renversassions", "renversât", "renversâtes",
            "renverse", "renversé", "renversée", "renversées", "renversement", "renversements",
            "renversent", "renverser", "renversera", "renverserai", "renverseraient", "renverserais",
            "renverserait", "renverseras", "renversèrent", "renverserez", "renverseriez", "renverserions",
            "renverserons", "renverseront", "renverses", "renversés", "renverseur", "renverseurs",
            "renverseuse", "renverseuses", "renversez", "renversiez", "renversions", "renversons",
            "renvi", "renvia", "renviai", "renviaient", "renviais", "renviait", "renviâmes", "renviant",
            "renvias", "renviasse", "renviassent", "renviasses", "renviassiez", "renviassions",
            "renviât", "renviâtes", "renvida", "renvidai", "renvidaient", "renvidais", "renvidait",
            "renvidâmes", "renvidant", "renvidas", "renvidasse", "renvidassent", "renvidasses",
            "renvidassiez", "renvidassions", "renvidât", "renvidâtes", "renvide", "renvidé", "renvidée",
            "renvidées", "renvident", "renvider", "renvidera", "renviderai", "renvideraient",
            "renviderais", "renviderait", "renvideras", "renvidèrent", "renviderez", "renvideriez",
            "renviderions", "renviderons", "renvideront", "renvides", "renvidés", "renvidez",
            "renvidiez", "renvidions", "renvidons", "renvie", "renvié", "renviée", "renviées",
            "renvient", "renvier", "renviera", "renvierai", "renvieraient", "renvierais", "renvierait",
            "renvieras", "renvièrent", "renvierez", "renvieriez", "renvierions", "renvierons",
            "renvieront", "renvies", "renviés", "renviez", "renviiez", "renviions", "renvions",
            "renvis", "renvoi", "renvoie", "renvoient", "renvoies", "renvois", "renvoya", "renvoyai",
            "renvoyaient", "renvoyais", "renvoyait", "renvoyâmes", "renvoyant", "renvoyas", "renvoyasse",
            "renvoyassent", "renvoyasses", "renvoyassiez", "renvoyassions", "renvoyât", "renvoyâtes",
            "renvoyé", "renvoyée", "renvoyées", "renvoyer", "renvoyèrent", "renvoyés", "renvoyette",
            "renvoyez", "renvoyiez", "renvoyions", "renvoyons", "reps", "reptation", "reptations",
            "reptiforme", "reptile", "reptiles", "reptilien", "reptilienne", "reptiliennes", "reptiliens",
            "reptilité", "reptilités", "reptilivore", "requiem", "requiems", "rescapé", "rescapée",
            "rescapées", "rescapés", "rescinda", "rescindai", "rescindaient", "rescindais", "rescindait",
            "rescindâmes", "rescindant", "rescindas", "rescindasse", "rescindassent", "rescindasses",
            "rescindassiez", "rescindassions", "rescindât", "rescindâtes", "rescinde", "rescindé",
            "rescindée", "rescindées", "rescindent", "rescinder", "rescindera", "rescinderai",
            "rescinderaient", "rescinderais", "rescinderait", "rescinderas", "rescindèrent", "rescinderez",
            "rescinderiez", "rescinderions", "rescinderons", "rescinderont", "rescindes", "rescindés",
            "rescindez", "rescindiez", "rescindions", "rescindons", "rescision", "rescisions",
            "rescisoire", "rescisoires", "rescousse", "rescousses", "rescription", "rescriptions",
            "rescrit", "rescrits", "respect", "respecta", "respectabilité", "respectabilités",
            "respectable", "respectables", "respectai", "respectaient", "respectais", "respectait",
            "respectâmes", "respectant", "respectas", "respectasse", "respectassent", "respectasses",
            "respectassiez", "respectassions", "respectât", "respectâtes", "respecte", "respecté",
            "respectée", "respectées", "respectent", "respecter", "respectera", "respecterai",
            "respecteraient", "respecterais", "respecterait", "respecteras", "respectèrent", "respecterez",
            "respecteriez", "respecterions", "respecterons", "respecteront", "respectes", "respectés",
            "respectez", "respectiez", "respectif", "respectifs", "respections", "respective",
            "respectivement", "respectives", "respectons", "respects", "respectueuse", "respectueusement",
            "respectueuses", "respectueux", "respir", "respira", "respirable", "respirables",
            "respirai", "respiraient", "respirais", "respirait", "respirâmes", "respirant", "respiras",
            "respirasse", "respirassent", "respirasses", "respirassiez", "respirassions", "respirât",
            "respirâtes", "respirateur", "respirateurs", "respiration", "respirations", "respiratoire",
            "respiratoires", "respiratrice", "respiratrices", "respire", "respiré", "respirée",
            "respirées", "respirent", "respirer", "respirera", "respirerai", "respireraient",
            "respirerais", "respirerait", "respireras", "respirèrent", "respirerez", "respireriez",
            "respirerions", "respirerons", "respireront", "respires", "respirés", "respirez",
            "respiriez", "respirions", "respirons", "respirs", "resplendi", "resplendie", "resplendies",
            "resplendîmes", "resplendir", "resplendira", "resplendirai", "resplendiraient", "resplendirais",
            "resplendirait", "resplendiras", "resplendirent", "resplendirez", "resplendiriez",
            "resplendirions", "resplendirons", "resplendiront", "resplendis", "resplendissaient",
            "resplendissais", "resplendissait", "resplendissant", "resplendissante", "resplendissantes",
            "resplendissants", "resplendisse", "resplendissement", "resplendissements", "resplendissent",
            "resplendisses", "resplendissez", "resplendissiez", "resplendissions", "resplendissons",
            "resplendit", "resplendît", "resplendîtes", "responsabilité", "responsabilités", "responsable",
            "responsables", "responsif", "resquilla", "resquillage", "resquillages", "resquillai",
            "resquillaient", "resquillais", "resquillait", "resquillâmes", "resquillant", "resquillas",
            "resquillasse", "resquillassent", "resquillasses", "resquillassiez", "resquillassions",
            "resquillât", "resquillâtes", "resquille", "resquillé", "resquillée", "resquillées",
            "resquillent", "resquiller", "resquillera", "resquillerai", "resquilleraient", "resquillerais",
            "resquillerait", "resquilleras", "resquillèrent", "resquillerez", "resquilleriez",
            "resquillerions", "resquillerons", "resquilleront", "resquilles", "resquillés", "resquilleur",
            "resquilleurs", "resquilleuse", "resquilleuses", "resquillez", "resquilliez", "resquillions",
            "resquillons", "ressaisi", "ressaisie", "ressaisies", "ressaisis", "ressaisissement",
            "ressaisissements", "ressaisît", "ressaisîtes", "ressemblâmes", "ressemblât", "ressemblions",
            "ressemblons", "ressouvenirse", "ressui", "ressuie", "ressuient", "ressuiera", "ressuierai",
            "ressuieraient", "ressuierais", "ressuierait", "ressuieras", "ressuierez", "ressuieriez",
            "ressuierions", "ressuierons", "ressuieront", "ressuies", "ressuiez", "ressuions",
            "ressuis", "ressuscita", "ressuscitable", "ressuscitai", "ressuscitaient", "ressuscitais",
            "ressuscitait", "ressuscitâmes", "ressuscitant", "ressuscitas", "ressuscitasse", "ressuscitassent",
            "ressuscitasses", "ressuscitassiez", "ressuscitassions", "ressuscitât", "ressuscitâtes",
            "ressuscitatif", "ressuscitation", "ressuscitations", "ressuscite", "ressuscité",
            "ressuscitée", "ressuscitées", "ressuscitent", "ressusciter", "ressuscitera", "ressusciterai",
            "ressusciteraient", "ressusciterais", "ressusciterait", "ressusciteras", "ressuscitèrent",
            "ressusciterez", "ressusciteriez", "ressusciterions", "ressusciterons", "ressusciteront",
            "ressuscites", "ressuscités", "ressuscitez", "ressuscitiez", "ressuscitions", "ressuscitons",
            "ressuya", "ressuyage", "ressuyages", "ressuyai", "ressuyâmes", "ressuyant", "ressuyas",
            "ressuyasse", "ressuyassent", "ressuyasses", "ressuyassions", "ressuyât", "ressuyâtes",
            "ressuyé", "ressuyée", "ressuyées", "ressuyer", "ressuyèrent", "ressuyés", "ressuyez",
            "ressuyiez", "ressuyions", "ressuyons", "resta", "restage", "restai", "restaient",
            "restais", "restait", "restâmes", "restant", "restante", "restantes", "restants",
            "restas", "restasse", "restassent", "restasses", "restassiez", "restassions", "restât",
            "restâtes", "restaura", "restaurai", "restauraient", "restaurais", "restaurait", "restaurâmes",
            "restaurant", "restaurants", "restauras", "restaurasse", "restaurassent", "restaurasses",
            "restaurassiez", "restaurassions", "restaurât", "restaurâtes", "restaurateur", "restaurateurs",
            "restauration", "restaurations", "restauratrice", "restauratrices", "restaure", "restauré",
            "restaurée", "restaurées", "restaurent", "restaurer", "restaurera", "restaurerai",
            "restaureraient", "restaurerais", "restaurerait", "restaureras", "restaurèrent", "restaurerez",
            "restaureriez", "restaurerions", "restaurerons", "restaureront", "restaures", "restaurés",
            "restaurez", "restauriez", "restaurions", "restaurons", "restauroute", "reste", "resté",
            "restée", "restées", "restent", "rester", "restera", "resterai", "resteraient", "resterais",
            "resterait", "resteras", "restèrent", "resterez", "resteriez", "resterions", "resterons",
            "resteront", "restes", "restés", "restez", "restiez", "restions", "restitua", "restituable",
            "restituables", "restituai", "restituaient", "restituais", "restituait", "restituâmes",
            "restituant", "restituas", "restituasse", "restituassent", "restituasses", "restituassiez",
            "restituassions", "restituât", "restituâtes", "restitue", "restitué", "restituée",
            "restituées", "restituent", "restituer", "restituera", "restituerai", "restitueraient",
            "restituerais", "restituerait", "restitueras", "restituèrent", "restituerez", "restitueriez",
            "restituerions", "restituerons", "restitueront", "restitues", "restitués", "restituez",
            "restituiez", "restituions", "restituons", "restituteur", "restituteurs", "restitutif",
            "restitutifs", "restitution", "restitutions", "restitutive", "restitutives", "restitutoire",
            "restitutoires", "restons", "restreignaient", "restreignais", "restreignait", "restreignant",
            "restreigne", "restreignent", "restreignes", "restreignez", "restreigniez", "restreignîmes",
            "restreignions", "restreignirent", "restreignis", "restreignisse", "restreignissent",
            "restreignisses", "restreignissiez", "restreignissions", "restreignit", "restreignît",
            "restreignîtes", "restreignons", "restreindra", "restreindrai", "restreindraient",
            "restreindrais", "restreindrait", "restreindras", "restreindre", "restreindrez", "restreindriez",
            "restreindrions", "restreindrons", "restreindront", "restreins", "restreint", "restreinte",
            "restreintes", "restreints", "restrictif", "restrictifs", "restriction", "restrictions",
            "restrictive", "restrictives", "restringent", "restringente", "restringentes", "restringents",
            "rets", "revolver", "revolverien", "revolverienne", "revolvers", "rewrita", "rewritai",
            "rewritaient", "rewritais", "rewritait", "rewritâmes", "rewritant", "rewritas", "rewritasse",
            "rewritassent", "rewritasses", "rewritassiez", "rewritassions", "rewritât", "rewritâtes",
            "rewrite", "rewrité", "rewritée", "rewritées", "rewritent", "rewriter", "rewritera",
            "rewriterai", "rewriteraient", "rewriterais", "rewriterait", "rewriteras", "rewritèrent",
            "rewriterez", "rewriteriez", "rewriterions", "rewriterons", "rewriteront", "rewriters",
            "rewrites", "rewrités", "rewritez", "rewritiez", "rewriting", "rewritings", "rewritions",
            "rewritons", "rexisme", "rexismes", "rez", "rezzou", "rezzous",
        };

        string words =
        @"


realia
recta
rectal
rectale
rectales
rectangle
rectangles
rectangulaire
rectangulaires
rectas
rectaux
rectembryé
rectembryées
recteur
recteurs
recticorne
rectident
rectifia
rectifiable
rectifiables
rectifiai
rectifiaient
rectifiais
rectifiait
rectifiâmes
rectifiant
rectifias
rectifiasse
rectifiassent
rectifiasses
rectifiassiez
rectifiassions
rectifiât
rectifiâtes
rectificateur
rectificateurs
rectificatif
rectificatifs
rectification
rectifications
rectificative
rectificatives
rectificatrice
rectificatrices
rectifie
rectifié
rectifiée
rectifiées
rectifient
rectifier
rectifiera
rectifierai
rectifieraient
rectifierais
rectifierait
rectifieras
rectifièrent
rectifierez
rectifieriez
rectifierions
rectifierons
rectifieront
rectifies
rectifiés
rectifieur
rectifieurs
rectifieuse
rectifieuses
rectifiez
rectifiiez
rectifiions
rectifions
rectiflore
rectiforme
rectigradation
rectigrade
rectilatère
rectiligne
rectilignes
rectilinéaire
rectilinéaires
rectimètre
rectinerve
rection
rections
rectirostre
rectisérié
rectite
rectites
rectitude
rectitudes
rectiuscule
recto
rectocurviligne
rectoral
rectorale
rectorales
rectorat
rectorats
rectoraux
rectos
rectrice
rectrices
rectum
rectums
reddition
redditions
redowa
redowas
reflex
reg
regency
reggae
regs
reichsmark
reichsmarks
reichstag
reichstags
rein
reine
reines
reinette
reinettes
reins
reinté
reintée
reintées
reintés
reis
reïs
reître
reîtres
relaxerse
remake
remakes
rembailler
remballa
remballai
remballaient
remballais
remballait
remballâmes
remballant
remballas
remballasse
remballassent
remballasses
remballassiez
remballassions
remballât
remballâtes
remballe
remballé
remballée
remballées
remballent
remballer
remballera
remballerai
remballeraient
remballerais
remballerait
remballeras
remballèrent
remballerez
remballeriez
remballerions
remballerons
remballeront
remballes
remballés
remballez
remballiez
remballions
remballons
rembarqua
rembarquai
rembarquaient
rembarquais
rembarquait
rembarquâmes
rembarquant
rembarquas
rembarquasse
rembarquassent
rembarquasses
rembarquassiez
rembarquassions
rembarquât
rembarquâtes
rembarque
rembarqué
rembarquée
rembarquées
rembarquent
rembarquer
rembarquera
rembarquerai
rembarqueraient
rembarquerais
rembarquerait
rembarqueras
rembarquèrent
rembarquerez
rembarqueriez
rembarquerions
rembarquerons
rembarqueront
rembarques
rembarqués
rembarquez
rembarquiez
rembarquions
rembarquons
rembarra
rembarrai
rembarraient
rembarrais
rembarrait
rembarrâmes
rembarrant
rembarras
rembarrasse
rembarrassent
rembarrasses
rembarrassiez
rembarrassions
rembarrât
rembarrâtes
rembarre
rembarré
rembarrée
rembarrées
rembarrent
rembarrer
rembarrera
rembarrerai
rembarreraient
rembarrerais
rembarrerait
rembarreras
rembarrèrent
rembarrerez
rembarreriez
rembarrerions
rembarrerons
rembarreront
rembarres
rembarrés
rembarrez
rembarriez
rembarrions
rembarrons
remblai
remblaie
remblaiement
remblaiements
remblaient
remblaiera
remblaierai
remblaieraient
remblaierais
remblaierait
remblaieras
remblaierez
remblaieriez
remblaierions
remblaierons
remblaieront
remblaies
remblais
remblaya
remblayage
remblayages
remblayai
remblayaient
remblayais
remblayait
remblayâmes
remblayant
remblayas
remblayasse
remblayassent
remblayasses
remblayassiez
remblayassions
remblayât
remblayâtes
remblaye
remblayé
remblayée
remblayées
remblayent
remblayer
remblayera
remblayerai
remblayeraient
remblayerais
remblayerait
remblayeras
remblayèrent
remblayerez
remblayeriez
remblayerions
remblayerons
remblayeront
remblayes
remblayés
remblayeur
remblayeurs
remblayez
remblayiez
remblayions
remblayons
rembobina
rembobinai
rembobinaient
rembobinais
rembobinait
rembobinâmes
rembobinant
rembobinas
rembobinasse
rembobinassent
rembobinasses
rembobinassiez
rembobinassions
rembobinât
rembobinâtes
rembobine
rembobiné
rembobinée
rembobinées
rembobinent
rembobiner
rembobinera
rembobinerai
rembobineraient
rembobinerais
rembobinerait
rembobineras
rembobinèrent
rembobinerez
rembobineriez
rembobinerions
rembobinerons
rembobineront
rembobines
rembobinés
rembobinez
rembobiniez
rembobinions
rembobinons
remboîta
remboîtage
remboîtages
remboîtai
remboîtaient
remboîtais
remboîtait
remboîtâmes
remboîtant
remboîtas
remboîtasse
remboîtassent
remboîtasses
remboîtassiez
remboîtassions
remboîtât
remboîtâtes
remboîte
remboîté
remboîtée
remboîtées
remboîtement
remboîtements
remboîtent
remboîter
remboîtera
remboîterai
remboîteraient
remboîterais
remboîterait
remboîteras
remboîtèrent
remboîterez
remboîteriez
remboîterions
remboîterons
remboîteront
remboîtes
remboîtés
remboîtez
remboîtiez
remboîtions
remboîtons
rembourra
rembourrage
rembourrages
rembourrai
rembourraient
rembourrais
rembourrait
rembourrâmes
rembourrant
rembourras
rembourrasse
rembourrassent
rembourrasses
rembourrassiez
rembourrassions
rembourrât
rembourrâtes
rembourre
rembourré
rembourrée
rembourrées
rembourrent
rembourrer
rembourrera
rembourrerai
rembourreraient
rembourrerais
rembourrerait
rembourreras
rembourrèrent
rembourrerez
rembourreriez
rembourrerions
rembourrerons
rembourreront
rembourres
rembourrés
rembourrez
rembourriez
rembourrions
rembourrons
rembourrure
rembourrures
rembours
remboursa
remboursable
remboursables
remboursai
remboursaient
remboursais
remboursait
remboursâmes
remboursant
remboursas
remboursasse
remboursassent
remboursasses
remboursassiez
remboursassions
remboursât
remboursâtes
rembourse
remboursé
remboursée
remboursées
remboursement
remboursements
remboursent
rembourser
remboursera
rembourserai
rembourseraient
rembourserais
rembourserait
rembourseras
remboursèrent
rembourserez
rembourseriez
rembourserions
rembourserons
rembourseront
rembourses
remboursés
remboursez
remboursiez
remboursions
remboursons
rembraie
rembraient
rembraiera
rembraierai
rembraieraient
rembraierais
rembraierait
rembraieras
rembraierez
rembraieriez
rembraierions
rembraierons
rembraieront
rembraies
rembrandtesque
rembrandtesques
rembranesque
rembranesquement
rembranesques
rembraya
rembrayage
rembrayai
rembrayaient
rembrayais
rembrayait
rembrayâmes
rembrayant
rembrayas
rembrayasse
rembrayassent
rembrayasses
rembrayassiez
rembrayassions
rembrayât
rembrayâtes
rembraye
rembrayé
rembrayée
rembrayées
rembrayent
rembrayer
rembrayera
rembrayerai
rembrayeraient
rembrayerais
rembrayerait
rembrayeras
rembrayèrent
rembrayerez
rembrayeriez
rembrayerions
rembrayerons
rembrayeront
rembrayes
rembrayés
rembrayez
rembrayiez
rembrayions
rembrayons
rembruni
rembrunie
rembrunies
rembrunîmes
rembrunir
rembrunira
rembrunirai
rembruniraient
rembrunirais
rembrunirait
rembruniras
rembrunirent
rembrunirez
rembruniriez
rembrunirions
rembrunirons
rembruniront
rembrunis
rembrunissaient
rembrunissais
rembrunissait
rembrunissant
rembrunisse
rembrunissement
rembrunissements
rembrunissent
rembrunisses
rembrunissez
rembrunissiez
rembrunissions
rembrunissons
rembrunit
rembrunît
rembrunîtes
rembucha
rembûcha
rembuchai
rembûchai
rembuchaient
rembûchaient
rembuchais
rembûchais
rembuchait
rembûchait
rembuchâmes
rembûchâmes
rembuchant
rembûchant
rembuchas
rembûchas
rembuchasse
rembûchasse
rembuchassent
rembûchassent
rembuchasses
rembûchasses
rembuchassiez
rembûchassiez
rembuchassions
rembûchassions
rembuchât
rembûchât
rembuchâtes
rembûchâtes
rembuche
rembûche
rembuché
rembûché
rembuchée
rembûchée
rembuchées
rembûchées
rembuchement
rembuchements
rembuchent
rembûchent
rembucher
rembûcher
rembuchera
rembûchera
rembucherai
rembûcherai
rembucheraient
rembûcheraient
rembucherais
rembûcherais
rembucherait
rembûcherait
rembucheras
rembûcheras
rembuchèrent
rembûchèrent
rembucherez
rembûcherez
rembucheriez
rembûcheriez
rembucherions
rembûcherions
rembucherons
rembûcherons
rembucheront
rembûcheront
rembuches
rembûches
rembuchés
rembûchés
rembuchez
rembûchez
rembuchiez
rembûchiez
rembuchions
rembûchions
rembuchons
rembûchons
remmailla
remmaillage
remmaillages
remmaillai
remmaillaient
remmaillais
remmaillait
remmaillâmes
remmaillant
remmaillas
remmaillasse
remmaillassent
remmaillasses
remmaillassiez
remmaillassions
remmaillât
remmaillâtes
remmaille
remmaillé
remmaillée
remmaillées
remmaillent
remmailler
remmaillera
remmaillerai
remmailleraient
remmaillerais
remmaillerait
remmailleras
remmaillèrent
remmaillerez
remmailleriez
remmaillerions
remmaillerons
remmailleront
remmailles
remmaillés
remmailleur
remmailleurs
remmailleuse
remmailleuses
remmaillez
remmailliez
remmaillions
remmaillons
remmaillota
remmaillotai
remmaillotaient
remmaillotais
remmaillotait
remmaillotâmes
remmaillotant
remmaillotas
remmaillotasse
remmaillotassent
remmaillotasses
remmaillotassiez
remmaillotassions
remmaillotât
remmaillotâtes
remmaillote
remmailloté
remmaillotée
remmaillotées
remmaillotent
remmailloter
remmaillotera
remmailloterai
remmailloteraient
remmailloterais
remmailloterait
remmailloteras
remmaillotèrent
remmailloterez
remmailloteriez
remmailloterions
remmailloterons
remmailloteront
remmaillotes
remmaillotés
remmaillotez
remmaillotiez
remmaillotions
remmaillotons
remmancha
remmanchai
remmanchaient
remmanchais
remmanchait
remmanchâmes
remmanchant
remmanchas
remmanchasse
remmanchassent
remmanchasses
remmanchassiez
remmanchassions
remmanchât
remmanchâtes
remmanche
remmanché
remmanchée
remmanchées
remmanchent
remmancher
remmanchera
remmancherai
remmancheraient
remmancherais
remmancherait
remmancheras
remmanchèrent
remmancherez
remmancheriez
remmancherions
remmancherons
remmancheront
remmanches
remmanchés
remmancheur
remmanchez
remmanchiez
remmanchions
remmanchons
remmena
remmenai
remmenaient
remmenais
remmenait
remmenâmes
remmenant
remmenas
remmenasse
remmenassent
remmenasses
remmenassiez
remmenassions
remmenât
remmenâtes
remmène
remmené
remmenée
remmenées
remmènent
remmener
remmènera
remmènerai
remmèneraient
remmènerais
remmènerait
remmèneras
remmenèrent
remmènerez
remmèneriez
remmènerions
remmènerons
remmèneront
remmènes
remmenés
remmenez
remmeniez
remmenions
remmenons
rempailla
rempaillage
rempaillages
rempaillai
rempaillaient
rempaillais
rempaillait
rempaillâmes
rempaillant
rempaillas
rempaillasse
rempaillassent
rempaillasses
rempaillassiez
rempaillassions
rempaillât
rempaillâtes
rempaille
rempaillé
rempaillée
rempaillées
rempaillent
rempailler
rempaillera
rempaillerai
rempailleraient
rempaillerais
rempaillerait
rempailleras
rempaillèrent
rempaillerez
rempailleriez
rempaillerions
rempaillerons
rempailleront
rempailles
rempaillés
rempailleur
rempailleurs
rempailleuse
rempailleuses
rempaillez
rempailliez
rempaillions
rempaillons
rempara
remparai
remparaient
remparais
remparait
remparâmes
remparant
remparas
remparasse
remparassent
remparasses
remparassiez
remparassions
remparât
remparâtes
rempare
remparé
remparée
remparées
remparent
remparer
remparera
remparerai
rempareraient
remparerais
remparerait
rempareras
remparèrent
remparerez
rempareriez
remparerions
remparerons
rempareront
rempares
remparés
remparez
rempariez
remparions
remparons
rempart
remparts
rempiéta
rempiétai
rempiétaient
rempiétais
rempiétait
rempiétâmes
rempiétant
rempiétas
rempiétasse
rempiétassent
rempiétasses
rempiétassiez
rempiétassions
rempiétât
rempiétâtes
rempiète
rempiété
rempiétée
rempiétées
rempiétement
rempiétements
rempiètent
rempiéter
rempiétera
rempiéterai
rempiéteraient
rempiéterais
rempiéterait
rempiéteras
rempiétèrent
rempiéterez
rempiéteriez
rempiéterions
rempiéterons
rempiéteront
rempiètes
rempiétés
rempiétez
rempiétiez
rempiétions
rempiétons
rempila
rempilai
rempilaient
rempilais
rempilait
rempilâmes
rempilant
rempilas
rempilasse
rempilassent
rempilasses
rempilassiez
rempilassions
rempilât
rempilâtes
rempile
rempilé
rempilée
rempilées
rempilent
rempiler
rempilera
rempilerai
rempileraient
rempilerais
rempilerait
rempileras
rempilèrent
rempilerez
rempileriez
rempilerions
rempilerons
rempileront
rempiles
rempilés
rempilez
rempiliez
rempilions
rempilons
remplaça
remplaçable
remplaçables
remplaçai
remplaçaient
remplaçais
remplaçait
remplaçâmes
remplaçant
remplaçante
remplaçantes
remplaçants
remplaças
remplaçasse
remplaçassent
remplaçasses
remplaçassiez
remplaçassions
remplaçât
remplaçâtes
remplace
remplacé
remplacée
remplacées
remplacement
remplacements
remplacent
remplacer
remplacera
remplacerai
remplaceraient
remplacerais
remplacerait
remplaceras
remplacèrent
remplacerez
remplaceriez
remplacerions
remplacerons
remplaceront
remplaces
remplacés
remplacez
remplaciez
remplacions
remplaçons
remplage
remplages
rempli
remplia
rempliai
rempliaient
rempliais
rempliait
rempliâmes
rempliant
remplias
rempliasse
rempliassent
rempliasses
rempliassiez
rempliassions
rempliât
rempliâtes
remplie
remplié
rempliée
rempliées
remplient
remplier
rempliera
remplierai
remplieraient
remplierais
remplierait
remplieras
remplièrent
remplierez
remplieriez
remplierions
remplierons
remplieront
remplies
rempliés
rempliez
rempliiez
rempliions
remplîmes
remplions
remplir
remplira
remplirai
rempliraient
remplirais
remplirait
rempliras
remplirent
remplirez
rempliriez
remplirions
remplirons
rempliront
remplis
remplissage
remplissages
remplissaient
remplissais
remplissait
remplissant
remplisse
remplissement
remplissements
remplissent
remplisses
remplisseur
remplisseurs
remplisseuse
remplisseuses
remplissez
remplissiez
remplissions
remplissons
remplit
remplît
remplîtes
remploi
remploie
remploient
remploiera
remploierai
remploieraient
remploierais
remploierait
remploieras
remploierez
remploieriez
remploierions
remploierons
remploieront
remploies
remplois
remploya
remployai
remployaient
remployais
remployait
remployâmes
remployant
remployas
remployasse
remployassent
remployasses
remployassiez
remployassions
remployât
remployâtes
remployé
remployée
remployées
remployer
remployèrent
remployés
remployez
remployiez
remployions
remployons
rempluma
remplumai
remplumaient
remplumais
remplumait
remplumâmes
remplumant
remplumas
remplumasse
remplumassent
remplumasses
remplumassiez
remplumassions
remplumât
remplumâtes
remplume
remplumé
remplumée
remplumées
remplument
remplumer
remplumera
remplumerai
remplumeraient
remplumerais
remplumerait
remplumeras
remplumèrent
remplumerez
remplumeriez
remplumerions
remplumerons
remplumeront
remplumes
remplumés
remplumez
remplumiez
remplumions
remplumons
rempocha
rempochai
rempochaient
rempochais
rempochait
rempochâmes
rempochant
rempochas
rempochasse
rempochassent
rempochasses
rempochassiez
rempochassions
rempochât
rempochâtes
rempoche
rempoché
rempochée
rempochées
rempochent
rempocher
rempochera
rempocherai
rempocheraient
rempocherais
rempocherait
rempocheras
rempochèrent
rempocherez
rempocheriez
rempocherions
rempocherons
rempocheront
rempoches
rempochés
rempochez
rempochiez
rempochions
rempochons
rempoigner
rempoissonna
rempoissonnai
rempoissonnaient
rempoissonnais
rempoissonnait
rempoissonnâmes
rempoissonnant
rempoissonnas
rempoissonnasse
rempoissonnassent
rempoissonnasses
rempoissonnassiez
rempoissonnassions
rempoissonnât
rempoissonnâtes
rempoissonne
rempoissonné
rempoissonnée
rempoissonnées
rempoissonnement
rempoissonnements
rempoissonnent
rempoissonner
rempoissonnera
rempoissonnerai
rempoissonneraient
rempoissonnerais
rempoissonnerait
rempoissonneras
rempoissonnèrent
rempoissonnerez
rempoissonneriez
rempoissonnerions
rempoissonnerons
rempoissonneront
rempoissonnes
rempoissonnés
rempoissonnez
rempoissonniez
rempoissonnions
rempoissonnons
remporta
remportai
remportaient
remportais
remportait
remportâmes
remportant
remportas
remportasse
remportassent
remportasses
remportassiez
remportassions
remportât
remportâtes
remporte
remporté
remportée
remportées
remportent
remporter
remportera
remporterai
remporteraient
remporterais
remporterait
remporteras
remportèrent
remporterez
remporteriez
remporterions
remporterons
remporteront
remportes
remportés
remportez
remportiez
remportions
remportons
rempota
rempotage
rempotages
rempotai
rempotaient
rempotais
rempotait
rempotâmes
rempotant
rempotas
rempotasse
rempotassent
rempotasses
rempotassiez
rempotassions
rempotât
rempotâtes
rempote
rempoté
rempotée
rempotées
rempotent
rempoter
rempotera
rempoterai
rempoteraient
rempoterais
rempoterait
rempoteras
rempotèrent
rempoterez
rempoteriez
rempoterions
rempoterons
rempoteront
rempotes
rempotés
rempotez
rempotiez
rempotions
rempotons
rencaissa
rencaissage
rencaissages
rencaissai
rencaissaient
rencaissais
rencaissait
rencaissâmes
rencaissant
rencaissas
rencaissasse
rencaissassent
rencaissasses
rencaissassiez
rencaissassions
rencaissât
rencaissâtes
rencaisse
rencaissé
rencaissée
rencaissées
rencaissement
rencaissements
rencaissent
rencaisser
rencaissera
rencaisserai
rencaisseraient
rencaisserais
rencaisserait
rencaisseras
rencaissèrent
rencaisserez
rencaisseriez
rencaisserions
rencaisserons
rencaisseront
rencaisses
rencaissés
rencaissez
rencaissiez
rencaissions
rencaissons
rencard
rencarda
rencardai
rencardaient
rencardais
rencardait
rencardâmes
rencardant
rencardas
rencardasse
rencardassent
rencardasses
rencardassiez
rencardassions
rencardât
rencardâtes
rencarde
rencardé
rencardée
rencardées
rencardent
rencarder
rencardera
rencarderai
rencarderaient
rencarderais
rencarderait
rencarderas
rencardèrent
rencarderez
rencarderiez
rencarderions
rencarderons
rencarderont
rencardes
rencardés
rencardez
rencardiez
rencardions
rencardons
rencards
rencart
rencarts
renchaussage
renchéri
renchérie
renchéries
renchérîmes
renchérir
renchérira
renchérirai
renchériraient
renchérirais
renchérirait
renchériras
renchérirent
renchérirez
renchéririez
renchéririons
renchérirons
renchériront
renchéris
renchérissaient
renchérissais
renchérissait
renchérissant
renchérisse
renchérissement
renchérissements
renchérissent
renchérisses
renchérisseur
renchérisseurs
renchérisseuse
renchérisseuses
renchérissez
renchérissiez
renchérissions
renchérissons
renchérit
renchérît
renchérîtes
renclore
renclôture
rencogna
rencognai
rencognaient
rencognais
rencognait
rencognâmes
rencognant
rencognas
rencognasse
rencognassent
rencognasses
rencognassiez
rencognassions
rencognât
rencognâtes
rencogne
rencogné
rencognée
rencognées
rencognent
rencogner
rencognera
rencognerai
rencogneraient
rencognerais
rencognerait
rencogneras
rencognèrent
rencognerez
rencogneriez
rencognerions
rencognerons
rencogneront
rencognes
rencognés
rencognez
rencogniez
rencognions
rencognons
rencoigna
rencoignai
rencoignaient
rencoignais
rencoignait
rencoignâmes
rencoignant
rencoignas
rencoignasse
rencoignassent
rencoignasses
rencoignassiez
rencoignassions
rencoignât
rencoignâtes
rencoigne
rencoigné
rencoignée
rencoignées
rencoignent
rencoigner
rencoignera
rencoignerai
rencoigneraient
rencoignerais
rencoignerait
rencoigneras
rencoignèrent
rencoignerez
rencoigneriez
rencoignerions
rencoignerons
rencoigneront
rencoignes
rencoignés
rencoignez
rencoigniez
rencoignions
rencoignons
rencontra
rencontrable
rencontrai
rencontraient
rencontrais
rencontrait
rencontrâmes
rencontrant
rencontras
rencontrasse
rencontrassent
rencontrasses
rencontrassiez
rencontrassions
rencontrât
rencontrâtes
rencontre
rencontré
rencontrée
rencontrées
rencontrent
rencontrer
rencontrera
rencontrerai
rencontreraient
rencontrerais
rencontrerait
rencontreras
rencontrèrent
rencontrerez
rencontreriez
rencontrerions
rencontrerons
rencontreront
rencontres
rencontrés
rencontrez
rencontriez
rencontrions
rencontrons
rencourager
rend
rendaient
rendais
rendait
rendant
rende
rendement
rendements
rendent
rendes
rendeur
rendeurs
rendeuse
rendeuses
rendez
rendiez
rendîmes
rendions
rendirent
rendis
rendisse
rendissent
rendisses
rendissiez
rendissions
rendit
rendît
rendîtes
rendons
rendormaient
rendormais
rendormait
rendormant
rendorme
rendorment
rendormes
rendormez
rendormi
rendormie
rendormies
rendormiez
rendormîmes
rendormions
rendormir
rendormira
rendormirai
rendormiraient
rendormirais
rendormirait
rendormiras
rendormirent
rendormirez
rendormiriez
rendormirions
rendormirons
rendormiront
rendormis
rendormisse
rendormissent
rendormisses
rendormissiez
rendormissions
rendormit
rendormît
rendormîtes
rendormons
rendors
rendort
rendra
rendrai
rendraient
rendrais
rendrait
rendras
rendre
rendrez
rendriez
rendrions
rendrons
rendront
rends
rendu
rendue
rendues
rendus
renfaîta
renfaîtage
renfaîtages
renfaîtai
renfaîtaient
renfaîtais
renfaîtait
renfaîtâmes
renfaîtant
renfaîtas
renfaîtasse
renfaîtassent
renfaîtasses
renfaîtassiez
renfaîtassions
renfaîtât
renfaîtâtes
renfaîte
renfaîté
renfaîtée
renfaîtées
renfaîtent
renfaîter
renfaîtera
renfaîterai
renfaîteraient
renfaîterais
renfaîterait
renfaîteras
renfaîtèrent
renfaîterez
renfaîteriez
renfaîterions
renfaîterons
renfaîteront
renfaîtes
renfaîtés
renfaîtez
renfaîtiez
renfaîtions
renfaîtons
renferma
renfermai
renfermaient
renfermais
renfermait
renfermâmes
renfermant
renfermas
renfermasse
renfermassent
renfermasses
renfermassiez
renfermassions
renfermât
renfermâtes
renferme
renfermé
renfermée
renfermées
renferment
renfermer
renfermera
renfermerai
renfermeraient
renfermerais
renfermerait
renfermeras
renfermèrent
renfermerez
renfermeriez
renfermerions
renfermerons
renfermeront
renfermes
renfermés
renfermez
renfermiez
renfermions
renfermons
renfiévrer
renfila
renfilai
renfilaient
renfilais
renfilait
renfilâmes
renfilant
renfilas
renfilasse
renfilassent
renfilasses
renfilassiez
renfilassions
renfilât
renfilâtes
renfile
renfilé
renfilée
renfilées
renfilent
renfiler
renfilera
renfilerai
renfileraient
renfilerais
renfilerait
renfileras
renfilèrent
renfilerez
renfileriez
renfilerions
renfilerons
renfileront
renfiles
renfilés
renfilez
renfiliez
renfilions
renfilons
renfla
renflai
renflaient
renflais
renflait
renflâmes
renflamma
renflammai
renflammaient
renflammais
renflammait
renflammâmes
renflammant
renflammas
renflammasse
renflammassent
renflammasses
renflammassiez
renflammassions
renflammât
renflammâtes
renflamme
renflammé
renflammée
renflammées
renflamment
renflammer
renflammera
renflammerai
renflammeraient
renflammerais
renflammerait
renflammeras
renflammèrent
renflammerez
renflammeriez
renflammerions
renflammerons
renflammeront
renflammes
renflammés
renflammez
renflammiez
renflammions
renflammons
renflant
renflas
renflasse
renflassent
renflasses
renflassiez
renflassions
renflât
renflâtes
renfle
renflé
renflée
renflées
renflement
renflements
renflent
renfler
renflera
renflerai
renfleraient
renflerais
renflerait
renfleras
renflèrent
renflerez
renfleriez
renflerions
renflerons
renfleront
renfles
renflés
renflez
renfliez
renflions
renflons
renfloua
renflouable
renflouables
renflouage
renflouages
renflouai
renflouaient
renflouais
renflouait
renflouâmes
renflouant
renflouas
renflouasse
renflouassent
renflouasses
renflouassiez
renflouassions
renflouât
renflouâtes
renfloue
renfloué
renflouée
renflouées
renflouement
renflouements
renflouent
renflouer
renflouera
renflouerai
renfloueraient
renflouerais
renflouerait
renfloueras
renflouèrent
renflouerez
renfloueriez
renflouerions
renflouerons
renfloueront
renfloues
renfloués
renflouez
renflouiez
renflouions
renflouons
renflure
renfonça
renfonçai
renfonçaient
renfonçais
renfonçait
renfonçâmes
renfonçant
renfonças
renfonçasse
renfonçassent
renfonçasses
renfonçassiez
renfonçassions
renfonçât
renfonçâtes
renfonce
renfoncé
renfoncée
renfoncées
renfoncement
renfoncements
renfoncent
renfoncer
renfoncera
renfoncerai
renfonceraient
renfoncerais
renfoncerait
renfonceras
renfoncèrent
renfoncerez
renfonceriez
renfoncerions
renfoncerons
renfonceront
renfonces
renfoncés
renfoncez
renfonciez
renfoncions
renfonçons
renforça
renforçai
renforçaient
renforçais
renforçait
renforçâmes
renforçant
renforças
renforçasse
renforçassent
renforçasses
renforçassiez
renforçassions
renforçât
renforçâtes
renforçateur
renforçateurs
renforçatrice
renforçatrices
renforce
renforcé
renforcée
renforcées
renforcement
renforcements
renforcent
renforcer
renforcera
renforcerai
renforceraient
renforcerais
renforcerait
renforceras
renforcèrent
renforcerez
renforceriez
renforcerions
renforcerons
renforceront
renforces
renforcés
renforcez
renforciez
renforcions
renforçons
renformi
renformie
renformies
renformîmes
renformir
renformira
renformirai
renformiraient
renformirais
renformirait
renformiras
renformirent
renformirez
renformiriez
renformirions
renformirons
renformiront
renformis
renformissaient
renformissais
renformissait
renformissant
renformisse
renformissent
renformisses
renformissez
renformissiez
renformissions
renformissons
renformit
renformît
renformîtes
renfort
renforts
renfrogna
renfrognai
renfrognaient
renfrognais
renfrognait
renfrognâmes
renfrognant
renfrognas
renfrognasse
renfrognassent
renfrognasses
renfrognassiez
renfrognassions
renfrognât
renfrognâtes
renfrogne
renfrogné
renfrognée
renfrognées
renfrognement
renfrognements
renfrognent
renfrogner
renfrognera
renfrognerai
renfrogneraient
renfrognerais
renfrognerait
renfrogneras
renfrognèrent
renfrognerez
renfrogneriez
renfrognerions
renfrognerons
renfrogneront
renfrognes
renfrognés
renfrognez
renfrogniez
renfrognions
renfrognons
rengage
rengagé
rengagea
rengageai
rengageaient
rengageais
rengageait
rengageâmes
rengageant
rengageas
rengageasse
rengageassent
rengageasses
rengageassiez
rengageassions
rengageât
rengageâtes
rengagée
rengagées
rengagement
rengagements
rengagent
rengageons
rengager
rengagera
rengagerai
rengageraient
rengagerais
rengagerait
rengageras
rengagèrent
rengagerez
rengageriez
rengagerions
rengagerons
rengageront
rengages
rengagés
rengagez
rengagiez
rengagions
rengaina
rengainai
rengainaient
rengainais
rengainait
rengainâmes
rengainant
rengainard
rengainarde
rengainas
rengainasse
rengainassent
rengainasses
rengainassiez
rengainassions
rengainât
rengainâtes
rengaine
rengainé
rengainée
rengainées
rengainent
rengainer
rengainera
rengainerai
rengaineraient
rengainerais
rengainerait
rengaineras
rengainèrent
rengainerez
rengaineriez
rengainerions
rengainerons
rengaineront
rengaines
rengainés
rengainez
rengainiez
rengainions
rengainons
rengorge
rengorgé
rengorgea
rengorgeai
rengorgeaient
rengorgeais
rengorgeait
rengorgeâmes
rengorgeant
rengorgeas
rengorgeasse
rengorgeassent
rengorgeasses
rengorgeassiez
rengorgeassions
rengorgeât
rengorgeâtes
rengorgée
rengorgées
rengorgement
rengorgements
rengorgent
rengorgeons
rengorger
rengorgera
rengorgerai
rengorgeraient
rengorgerais
rengorgerait
rengorgeras
rengorgèrent
rengorgerez
rengorgeriez
rengorgerions
rengorgerons
rengorgeront
rengorges
rengorgés
rengorgez
rengorgiez
rengorgions
rengraissa
rengraissai
rengraissaient
rengraissais
rengraissait
rengraissâmes
rengraissant
rengraissas
rengraissasse
rengraissassent
rengraissasses
rengraissassiez
rengraissassions
rengraissât
rengraissâtes
rengraisse
rengraissé
rengraissée
rengraissées
rengraissent
rengraisser
rengraissera
rengraisserai
rengraisseraient
rengraisserais
rengraisserait
rengraisseras
rengraissèrent
rengraisserez
rengraisseriez
rengraisserions
rengraisserons
rengraisseront
rengraisses
rengraissés
rengraissez
rengraissiez
rengraissions
rengraissons
rengrège
rengrégé
rengrégea
rengrégeai
rengrégeaient
rengrégeais
rengrégeait
rengrégeâmes
rengrégeant
rengrégeas
rengrégeasse
rengrégeassent
rengrégeasses
rengrégeassiez
rengrégeassions
rengrégeât
rengrégeâtes
rengrégée
rengrégées
rengrègent
rengrégeons
rengréger
rengrégera
rengrégerai
rengrégeraient
rengrégerais
rengrégerait
rengrégeras
rengrégèrent
rengrégerez
rengrégeriez
rengrégerions
rengrégerons
rengrégeront
rengrèges
rengrégés
rengrégez
rengrégiez
rengrégions
rengréna
rengrénai
rengrénaient
rengrénais
rengrénait
rengrénâmes
rengrénant
rengrénas
rengrénasse
rengrénassent
rengrénasses
rengrénassiez
rengrénassions
rengrénât
rengrénâtes
rengrène
rengréné
rengrénée
rengrénées
rengrènement
rengrènements
rengrènent
rengréner
rengrénera
rengrénerai
rengréneraient
rengrénerais
rengrénerait
rengréneras
rengrénèrent
rengrénerez
rengréneriez
rengrénerions
rengrénerons
rengréneront
rengrènes
rengrénés
rengrénez
rengréniez
rengrénions
rengrénons
renne
rennes
renquilla
renquillai
renquillaient
renquillais
renquillait
renquillâmes
renquillant
renquillas
renquillasse
renquillassent
renquillasses
renquillassiez
renquillassions
renquillât
renquillâtes
renquille
renquillé
renquillée
renquillées
renquillent
renquiller
renquillera
renquillerai
renquilleraient
renquillerais
renquillerait
renquilleras
renquillèrent
renquillerez
renquilleriez
renquillerions
renquillerons
renquilleront
renquilles
renquillés
renquillez
renquilliez
renquillions
renquillons
renseigna
renseignai
renseignaient
renseignais
renseignait
renseignâmes
renseignant
renseignas
renseignasse
renseignassent
renseignasses
renseignassiez
renseignassions
renseignât
renseignâtes
renseigne
renseigné
renseignée
renseignées
renseignement
renseignements
renseignent
renseigner
renseignera
renseignerai
renseigneraient
renseignerais
renseignerait
renseigneras
renseignèrent
renseignerez
renseigneriez
renseignerions
renseignerons
renseigneront
renseignes
renseignés
renseigneur
renseigneuse
renseignez
renseigniez
renseignions
renseignons
renta
rentabilisa
rentabilisai
rentabilisaient
rentabilisais
rentabilisait
rentabilisâmes
rentabilisant
rentabilisas
rentabilisasse
rentabilisassent
rentabilisasses
rentabilisassiez
rentabilisassions
rentabilisât
rentabilisâtes
rentabilise
rentabilisé
rentabilisée
rentabilisées
rentabilisent
rentabiliser
rentabilisera
rentabiliserai
rentabiliseraient
rentabiliserais
rentabiliserait
rentabiliseras
rentabilisèrent
rentabiliserez
rentabiliseriez
rentabiliserions
rentabiliserons
rentabiliseront
rentabilises
rentabilisés
rentabilisez
rentabilisiez
rentabilisions
rentabilisons
rentabilité
rentabilités
rentable
rentables
rentai
rentaient
rentais
rentait
rentâmes
rentant
rentas
rentasse
rentassent
rentasses
rentassiez
rentassions
rentât
rentâtes
rente
renté
rentée
rentées
rentent
renter
rentera
renterai
renteraient
renterais
renterait
renteras
rentèrent
renterez
renteriez
renterions
renterons
renteront
rentes
rentés
rentez
rentier
rentière
rentières
rentiers
rentiez
rentions
rentoila
rentoilage
rentoilages
rentoilai
rentoilaient
rentoilais
rentoilait
rentoilâmes
rentoilant
rentoilas
rentoilasse
rentoilassent
rentoilasses
rentoilassiez
rentoilassions
rentoilât
rentoilâtes
rentoile
rentoilé
rentoilée
rentoilées
rentoilent
rentoiler
rentoilera
rentoilerai
rentoileraient
rentoilerais
rentoilerait
rentoileras
rentoilèrent
rentoilerez
rentoileriez
rentoilerions
rentoilerons
rentoileront
rentoiles
rentoilés
rentoileur
rentoileurs
rentoilez
rentoiliez
rentoilions
rentoilons
rentons
rentra
rentrage
rentrages
rentrai
rentraient
rentraire
rentrais
rentrait
rentraiture
rentraitures
rentrâmes
rentrant
rentrante
rentrantes
rentrants
rentras
rentrasse
rentrassent
rentrasses
rentrassiez
rentrassions
rentrât
rentrâtes
rentrayage
rentrayages
rentrayeur
rentrayeurs
rentrayeuse
rentrayeuses
rentre
rentré
rentrée
rentrées
rentrent
rentrer
rentrera
rentrerai
rentreraient
rentrerais
rentrerait
rentreras
rentrèrent
rentrerez
rentreriez
rentrerions
rentrerons
rentreront
rentres
rentrés
rentreur
rentrez
rentriez
rentrions
rentrons
rentrure
rentrures
renverra
renverrai
renverraient
renverrais
renverrait
renverras
renverrez
renverriez
renverrions
renverrons
renverront
renversa
renversable
renversables
renversai
renversaient
renversais
renversait
renversâmes
renversant
renversante
renversantes
renversants
renversas
renversasse
renversassent
renversasses
renversassiez
renversassions
renversât
renversâtes
renverse
renversé
renversée
renversées
renversement
renversements
renversent
renverser
renversera
renverserai
renverseraient
renverserais
renverserait
renverseras
renversèrent
renverserez
renverseriez
renverserions
renverserons
renverseront
renverses
renversés
renverseur
renverseurs
renverseuse
renverseuses
renversez
renversiez
renversions
renversons
renvi
renvia
renviai
renviaient
renviais
renviait
renviâmes
renviant
renvias
renviasse
renviassent
renviasses
renviassiez
renviassions
renviât
renviâtes
renvida
renvidai
renvidaient
renvidais
renvidait
renvidâmes
renvidant
renvidas
renvidasse
renvidassent
renvidasses
renvidassiez
renvidassions
renvidât
renvidâtes
renvide
renvidé
renvidée
renvidées
renvident
renvider
renvidera
renviderai
renvideraient
renviderais
renviderait
renvideras
renvidèrent
renviderez
renvideriez
renviderions
renviderons
renvideront
renvides
renvidés
renvidez
renvidiez
renvidions
renvidons
renvie
renvié
renviée
renviées
renvient
renvier
renviera
renvierai
renvieraient
renvierais
renvierait
renvieras
renvièrent
renvierez
renvieriez
renvierions
renvierons
renvieront
renvies
renviés
renviez
renviiez
renviions
renvions
renvis
renvoi
renvoie
renvoient
renvoies
renvois
renvoya
renvoyai
renvoyaient
renvoyais
renvoyait
renvoyâmes
renvoyant
renvoyas
renvoyasse
renvoyassent
renvoyasses
renvoyassiez
renvoyassions
renvoyât
renvoyâtes
renvoyé
renvoyée
renvoyées
renvoyer
renvoyèrent
renvoyés
renvoyette
renvoyez
renvoyiez
renvoyions
renvoyons
reps
reptation
reptations
reptiforme
reptile
reptiles
reptilien
reptilienne
reptiliennes
reptiliens
reptilité
reptilités
reptilivore
requiem
requiems
rescapé
rescapée
rescapées
rescapés
rescinda
rescindai
rescindaient
rescindais
rescindait
rescindâmes
rescindant
rescindas
rescindasse
rescindassent
rescindasses
rescindassiez
rescindassions
rescindât
rescindâtes
rescinde
rescindé
rescindée
rescindées
rescindent
rescinder
rescindera
rescinderai
rescinderaient
rescinderais
rescinderait
rescinderas
rescindèrent
rescinderez
rescinderiez
rescinderions
rescinderons
rescinderont
rescindes
rescindés
rescindez
rescindiez
rescindions
rescindons
rescision
rescisions
rescisoire
rescisoires
rescousse
rescousses
rescription
rescriptions
rescrit
rescrits
respect
respecta
respectabilité
respectabilités
respectable
respectables
respectai
respectaient
respectais
respectait
respectâmes
respectant
respectas
respectasse
respectassent
respectasses
respectassiez
respectassions
respectât
respectâtes
respecte
respecté
respectée
respectées
respectent
respecter
respectera
respecterai
respecteraient
respecterais
respecterait
respecteras
respectèrent
respecterez
respecteriez
respecterions
respecterons
respecteront
respectes
respectés
respectez
respectiez
respectif
respectifs
respections
respective
respectivement
respectives
respectons
respects
respectueuse
respectueusement
respectueuses
respectueux
respir
respira
respirable
respirables
respirai
respiraient
respirais
respirait
respirâmes
respirant
respiras
respirasse
respirassent
respirasses
respirassiez
respirassions
respirât
respirâtes
respirateur
respirateurs
respiration
respirations
respiratoire
respiratoires
respiratrice
respiratrices
respire
respiré
respirée
respirées
respirent
respirer
respirera
respirerai
respireraient
respirerais
respirerait
respireras
respirèrent
respirerez
respireriez
respirerions
respirerons
respireront
respires
respirés
respirez
respiriez
respirions
respirons
respirs
resplendi
resplendie
resplendies
resplendîmes
resplendir
resplendira
resplendirai
resplendiraient
resplendirais
resplendirait
resplendiras
resplendirent
resplendirez
resplendiriez
resplendirions
resplendirons
resplendiront
resplendis
resplendissaient
resplendissais
resplendissait
resplendissant
resplendissante
resplendissantes
resplendissants
resplendisse
resplendissement
resplendissements
resplendissent
resplendisses
resplendissez
resplendissiez
resplendissions
resplendissons
resplendit
resplendît
resplendîtes
responsabilité
responsabilités
responsable
responsables
responsif
resquilla
resquillage
resquillages
resquillai
resquillaient
resquillais
resquillait
resquillâmes
resquillant
resquillas
resquillasse
resquillassent
resquillasses
resquillassiez
resquillassions
resquillât
resquillâtes
resquille
resquillé
resquillée
resquillées
resquillent
resquiller
resquillera
resquillerai
resquilleraient
resquillerais
resquillerait
resquilleras
resquillèrent
resquillerez
resquilleriez
resquillerions
resquillerons
resquilleront
resquilles
resquillés
resquilleur
resquilleurs
resquilleuse
resquilleuses
resquillez
resquilliez
resquillions
resquillons
ressaisi
ressaisie
ressaisies
ressaisis
ressaisissement
ressaisissements
ressaisît
ressaisîtes
ressemblâmes
ressemblât
ressemblions
ressemblons
ressouvenirse
ressui
ressuie
ressuient
ressuiera
ressuierai
ressuieraient
ressuierais
ressuierait
ressuieras
ressuierez
ressuieriez
ressuierions
ressuierons
ressuieront
ressuies
ressuiez
ressuions
ressuis
ressuscita
ressuscitable
ressuscitai
ressuscitaient
ressuscitais
ressuscitait
ressuscitâmes
ressuscitant
ressuscitas
ressuscitasse
ressuscitassent
ressuscitasses
ressuscitassiez
ressuscitassions
ressuscitât
ressuscitâtes
ressuscitatif
ressuscitation
ressuscitations
ressuscite
ressuscité
ressuscitée
ressuscitées
ressuscitent
ressusciter
ressuscitera
ressusciterai
ressusciteraient
ressusciterais
ressusciterait
ressusciteras
ressuscitèrent
ressusciterez
ressusciteriez
ressusciterions
ressusciterons
ressusciteront
ressuscites
ressuscités
ressuscitez
ressuscitiez
ressuscitions
ressuscitons
ressuya
ressuyage
ressuyages
ressuyai
ressuyâmes
ressuyant
ressuyas
ressuyasse
ressuyassent
ressuyasses
ressuyassions
ressuyât
ressuyâtes
ressuyé
ressuyée
ressuyées
ressuyer
ressuyèrent
ressuyés
ressuyez
ressuyiez
ressuyions
ressuyons
resta
restage
restai
restaient
restais
restait
restâmes
restant
restante
restantes
restants
restas
restasse
restassent
restasses
restassiez
restassions
restât
restâtes
restaura
restaurai
restauraient
restaurais
restaurait
restaurâmes
restaurant
restaurants
restauras
restaurasse
restaurassent
restaurasses
restaurassiez
restaurassions
restaurât
restaurâtes
restaurateur
restaurateurs
restauration
restaurations
restauratrice
restauratrices
restaure
restauré
restaurée
restaurées
restaurent
restaurer
restaurera
restaurerai
restaureraient
restaurerais
restaurerait
restaureras
restaurèrent
restaurerez
restaureriez
restaurerions
restaurerons
restaureront
restaures
restaurés
restaurez
restauriez
restaurions
restaurons
restauroute
reste
resté
restée
restées
restent
rester
restera
resterai
resteraient
resterais
resterait
resteras
restèrent
resterez
resteriez
resterions
resterons
resteront
restes
restés
restez
restiez
restions
restitua
restituable
restituables
restituai
restituaient
restituais
restituait
restituâmes
restituant
restituas
restituasse
restituassent
restituasses
restituassiez
restituassions
restituât
restituâtes
restitue
restitué
restituée
restituées
restituent
restituer
restituera
restituerai
restitueraient
restituerais
restituerait
restitueras
restituèrent
restituerez
restitueriez
restituerions
restituerons
restitueront
restitues
restitués
restituez
restituiez
restituions
restituons
restituteur
restituteurs
restitutif
restitutifs
restitution
restitutions
restitutive
restitutives
restitutoire
restitutoires
restons
restreignaient
restreignais
restreignait
restreignant
restreigne
restreignent
restreignes
restreignez
restreigniez
restreignîmes
restreignions
restreignirent
restreignis
restreignisse
restreignissent
restreignisses
restreignissiez
restreignissions
restreignit
restreignît
restreignîtes
restreignons
restreindra
restreindrai
restreindraient
restreindrais
restreindrait
restreindras
restreindre
restreindrez
restreindriez
restreindrions
restreindrons
restreindront
restreins
restreint
restreinte
restreintes
restreints
restrictif
restrictifs
restriction
restrictions
restrictive
restrictives
restringent
restringente
restringentes
restringents
rets
revolver
revolverien
revolverienne
revolvers
rewrita
rewritai
rewritaient
rewritais
rewritait
rewritâmes
rewritant
rewritas
rewritasse
rewritassent
rewritasses
rewritassiez
rewritassions
rewritât
rewritâtes
rewrite
rewrité
rewritée
rewritées
rewritent
rewriter
rewriter
rewritera
rewriterai
rewriteraient
rewriterais
rewriterait
rewriteras
rewritèrent
rewriterez
rewriteriez
rewriterions
rewriterons
rewriteront
rewriters
rewrites
rewrités
rewritez
rewritiez
rewriting
rewritings
rewritions
rewritons
rexisme
rexismes
rez
rezzou
rezzous








        ";
    }
}
