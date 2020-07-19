using System;
using System.Collections.Generic;
using System.Text;
using ColorLib;
using ColorLib.Dierese;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.Dierese
{
    [TestClass]
    public class CreerSyllabes
    {
        const string Texte =
@"Compère le renard se mit un jour en frais,
Et retint à dîner commère la cigogne.
Le régal fut petit et sans beaucoup d'apprêts :
Le galand, pour toute besogne,
Avait un brouet clair ; il vivait chichement.
Ce brouet fut par lui servi sur une assiette :
La cigogne au long bec n'en put attraper miette,
Et le drôle eut lapé le tout en un moment.
Pour se venger de cette tromperie,
A quelque temps de là, la cigogne le prie.
'Volontiers, lui dit-il ; car avec mes amis
Je ne fais point cérémonie.'
A l'heure dite, il courut au logis
De la cigogne son hôtesse ;
Loua très fort la politesse ;
Trouva le dîner cuit à point :
Bon appétit surtout ; renards n'en manquent point.
Il se réjouissait à l'odeur de la viande
Mise en menus morceaux, et qu'il croyait friande.
On servit, pour l'embarrasser,
En un vase à long col et d'étroite embouchure.
Le bec de la cigogne y pouvait bien passer ;
Mais le museau du sire était d'autre mesure.
Il lui fallut à jeun retourner au logis,
Honteux comme un renard qu'une poule aurait pris,
Serrant la queue, et portant bas l'oreille.

Trompeurs, c'est pour vous que j'écris :
Attendez-vous à la pareille.";

        [TestMethod]
        public void WriteSyllabes()
        {
            TheText.Init();

            TestTheText ttt = new TestTheText(Texte);
            Config conf = new Config();

            // ----------- c'est ici qu'on configure ce qu'on veut ------------------------
            conf.sylConf.mode = SylConfig.Mode.poesie;
            
            List<PhonWord> pws = ttt.GetPhonWordList(conf, true);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
            }

            List<ZonePoeme> zpL = AnalyseDierese.ChercheDierese(ttt, pws, 0);
            StringBuilder sb = new StringBuilder();
            foreach (ZonePoeme zp in zpL)
            {
                foreach (Vers v in zp.vList)
                {
                    sb.AppendLine(v.Syllabes());
                }
            }

            Console.Write(sb.ToString());
        }
    }
}
