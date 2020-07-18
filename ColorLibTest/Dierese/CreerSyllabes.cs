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
@"Ah! pour être dévot, je n'en suis pas moins homme;
Et lorsqu'on vient à voir vos célestes appas,
Un coeur se laisse prendre, et ne raisonne pas.
Je sais qu'un tel discours de moi paraît étrange;
Mais, Madame, après tout, je ne suis pas un ange;
Et si vous condamnez l'aveu que je vous fais,
Vous devez vous en prendre à vos charmants attraits.
Dès que j'en vis briller la splendeur plus qu'humaine,
De mon intérieur vous fûtes souveraine.
De vos regards divins, l'ineffable douceur,
Força la résistance où s'obstinait mon coeur;
Elle surmonta tout, jeûnes, prières, larmes,
Et tourna tous mes voeux du côté de vos charmes.
Mes yeux, et mes soupirs, vous l'ont dit mille fois ;
Et pour mieux m'expliquer, j'emploie ici la voix.
Que si vous contemplez, d'une âme un peu bénigne,
Les tribulations de votre esclave indigne;
S'il faut que vos bontés veuillent me consoler,
Et jusqu'à mon néant daignent se ravaler,
J'aurai toujours pour vous, ô suave merveille,
Une dévotion à nulle autre pareille.
Votre honneur, avec moi, ne court point de hasard;
Et n'a nulle disgrâce à craindre de ma part.
Tous ces galants de cour, dont les femmes sont folles,
Sont bruyants dans leurs faits, et vains dans leurs paroles.
De leurs progrès sans cesse on les voit se targuer;
Ils n'ont point de faveurs, qu'ils n'aillent divulguer;
Et leur langue indiscrète, en qui l'on se confie,
Déshonore l'autel où leur coeur sacrifie:
Mais les gens comme nous, brûlent d'un feu discret,
Avec qui pour toujours on est sûr du secret.
Le soin que nous prenons de notre renommée,
Répond de toute chose à la personne aimée;
Et c'est en nous qu'on trouve, acceptant notre coeur,
De l'amour sans scandale, et du plaisir sans peur.";

        [TestMethod]
        public void WriteSyllabes()
        {
            TheText.Init();

            TestTheText ttt = new TestTheText(Texte);
            Config conf = new Config();

            // ----------- c'est ici qu'on configure ce qu'on veut ------------------------
            conf.sylConf.mode = SylConfig.Mode.ecrit;
            
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
