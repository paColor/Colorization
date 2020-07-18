using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;

namespace ColorLibTest.Dierese
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class PoesieTest
    {
        

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TheText.Init();
        }

        const string Pernelle = 
@"C'est que je ne puis voir tout ce ménage-ci,
Et que de me complaire, on ne prend nul souci.
Oui, je sors de chez vous fort mal édifiée;
Dans toutes mes leçons, j'y suis contrariée ;
On n'y respecte rien; chacun y parle haut,
Et c'est, tout justement, la cour du roi Pétaut.";

        const string PernellePoesie =
@"C'est que je ne puis voir tout ce mé-na-ge ci,
Et que de me com-plaire, on ne prend nul sou-ci.
Oui, je sors de chez vous fort mal é-di-fi-ée;
Dans tou-tes mes le-çons, j'y suis con-tra-ri-ée ;
On n'y res-pec-te rien; cha-cun y par-le haut,
Et c'est, tout jus-te-ment, la cour du roi Pé-taut.";


        [TestMethod]
        public void TestPernelle()
        {
            TestTheText ttt = new TestTheText(Pernelle);
            Config conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            ttt.AssertSyls(conf, PernellePoesie);
        }

        const string Tartuffe =
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

        const string TartuffePoesie =
@"Ah! pour ê-tre dé-vot, je n'en suis pas moins homme;
Et lors-qu'on vient à voir vos cé-les-tes ap-pas,
Un coeur se lais-se prendre, et ne rai-son-ne pas.
Je sais qu'un tel dis-cours de moi pa-raît ét-range;
Mais, Ma-dame, après tout, je ne suis pas un ange;
Et si vous con-dam-nez l'a-veu que je vous fais,
Vous de-vez vous en prendre à vos char-mants at-traits.
Dès que j'en vis bril-ler la splen-deur plus qu'hu-maine,
De mon in-té-ri-eur vous fû-tes sou-ve-raine.
De vos re-gards di-vins, l'i-nef-fable dou-ceur,
For-ça la ré-sis-tance où s'ob-sti-nait mon coeur;
El-le sur-mon-ta tout, jeû-nes, pri-è-res, larmes,
Et tour-na tous mes voeux du cô-té de vos charmes.
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
        public void TestTartuffe()
        {
            TestTheText ttt = new TestTheText(Tartuffe);
            Config conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            ttt.AssertSyls(conf, TartuffePoesie);
        }


    }
}
