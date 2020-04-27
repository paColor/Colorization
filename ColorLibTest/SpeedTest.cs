using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest
{
    [TestClass]
    public class SpeedTest
    {
        static string[] verbes_ier = {"affilier","allier","amnistier","amplifier","anesthesier","apparier",
            "approprier","apprecier","asphyxier","associer","atrophier","authentifier","autographier",
            "autopsier","balbutier","bonifier","beatifier","beneficier","betifier","calligraphier","calomnier",
            "carier","cartographier","certifier","charrier","chier","choregraphier","chosifier","chatier",
            "clarifier","classifier","cocufier","codifier","colorier","communier","conchier","concilier",
            "confier","congedier","contrarier","copier","crier","crucifier","dactylographier",
            "differencier","disgracier","disqualifier","dissocier","distancier","diversifier","domicilier",
            "decrier","dedier","defier","deifier","delier","demarier","demultiplier","demystifier","denazifier",
            "denier","deplier","deprecier","dequalifier","dévier","envier","estropier","excommunier",
            "exemplifier","exfolier","expatrier","expier","exproprier","expedier","extasier","falsifier",
            "fier","fluidifier","fortifier","frigorifier","fructifier","gazeifier","glorifier","gracier",
            "gratifier","horrifier","humidifier","humilier","identifier","incendier","ingenier","initier",
            "injurier","intensifier","inventorier","irradier","justifier","licencier","lier","liquefier",
            "lubrifier","magnifier","maleficier","manier","marier","mendier","modifier","momifier","mortifier",
            "multiplier","mystifier","mythifier","mefier","nier","notifier","negocier","obvier","officier",
            "opacifier","orthographier","oublier","pacifier","palinodier","pallier","parier","parodier",
            "personnifier","photocopier","photographier","plagier","planifier","plastifier","plier","polycopier",
            "pontifier","prier","privilegier","psalmodier","publier","purifier","putrefier","pepier","petrifier",
            "qualifier","quantifier","radier","radiographier","rallier","ramifier","rapatrier","rarefier",
            "rassasier","ratifier","razzier","recopier","rectifier","relier","remanier","remarier",
            "remercier","remedier","renier","renegocier","replier","republier","requalifier","revivifier",
            "reverifier","rigidifier","reconcilier","recrier","reexpedier","refugier","repertorier","repudier",
            "resilier","reunifier","reedifier","reetudier","sacrifier","salarier","sanctifier","scier",
            "signifier","simplifier","skier","solidifier","soucier","spolier","specifier","statufier","strier",
            "stupefier","supplicier","supplier","serier","terrifier","tonifier","trier","tumefier",
            "typographier","telegraphier","unifier","varier","versifier","vicier","vitrifier","vivifier",
            "verifier","echographier","ecrier","edifier","electrifier","emulsifier","epier","etudier"};

        [TestMethod]
        public void TestSortedList()
        {
            List<string> verbes_ier_sorted = new List<string>(verbes_ier);
            verbes_ier_sorted.Sort();
            for (int i = 0; i < 1; i++)
            {
                verbes_ier_sorted.BinarySearch("unifier");
                verbes_ier_sorted.BinarySearch("deprecier");
                verbes_ier_sorted.BinarySearch("identifier");
                verbes_ier_sorted.BinarySearch("salut");
                verbes_ier_sorted.BinarySearch("o temps suspends ton vol");
            }
        }


        [TestMethod]
        public void TestDictionary()
        {
            StringDictionary verbes_ier_hashed = new StringDictionary();
            for (int i=0; i < verbes_ier.Length; i++)
                verbes_ier_hashed.Add(verbes_ier[i], null);
            for (int i = 0; i < 1; i++)
            {
                _ = verbes_ier_hashed.ContainsKey("unifier");
                _ = verbes_ier_hashed.ContainsKey("deprecier");
                _ = verbes_ier_hashed.ContainsKey("identifier");
                _ = verbes_ier_hashed.ContainsKey("salut");
                _ = verbes_ier_hashed.ContainsKey("o temps suspends ton vol");
            }
        }

    }
}
