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

        const string PernellePoesie13 =
@"C'est que je ne puis voir tout ce mé-na-ge ci,
Et que de me com-plaire, on ne prend nul sou-ci.
Oui, je sors de chez vous fort mal é-di-fi-ée;
Dans tou-tes mes le-çons, j'y suis con-tra-ri-ée ;
On n'y res-pec-te ri-en; cha-cun y par-le haut,
Et c'est, tout jus-te-ment, la cour du roi Pé-taut.";

        const string PernellePoesieSansDierese =
@"C'est que je ne puis voir tout ce mé-na-ge ci,
Et que de me com-plaire, on ne prend nul sou-ci.
Oui, je sors de chez vous fort mal é-di-fiée;
Dans tou-tes mes le-çons, j'y suis con-tra-riée ;
On n'y res-pec-te rien; cha-cun y par-le haut,
Et c'est, tout jus-te-ment, la cour du roi Pé-taut.";

        const string PernelleOral =
@"C'est que je ne puis voir tout ce mé-nage ci,
Et que de me com-plaire, on ne prend nul sou-ci.
Oui, je sors de chez vous fort mal é-di-fiée;
Dans toutes mes le-çons, j'y suis con-tra-riée ;
On n'y res-pecte rien; cha-cun y parle haut,
Et c'est, tout jus-te-ment, la cour du roi Pé-taut.";


        [TestMethod]
        public void TestPernelle()
        {
            TestTheText ttt = new TestTheText(Pernelle);
            Config conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            ttt.AssertSyls(conf, PernellePoesie);

            conf.sylConf.chercherDierese = false;
            ttt.AssertSyls(conf, PernellePoesieSansDierese);

            conf.sylConf.chercherDierese = true;
            conf.sylConf.nbrPieds = 13;
            ttt.AssertSyls(conf, PernellePoesie13);

            conf.sylConf.nbrPieds = 10;
            ttt.AssertSyls(conf, PernellePoesieSansDierese);

            conf.sylConf.mode = SylConfig.Mode.oral;
            ttt.AssertSyls(conf, PernelleOral);
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
@"Ah pour ê-tre dé-vot je n'en suis pas moins homme
Et lors-qu on vient à voir vos cé-les-tes ap-pas
Un coeur se lais-se prendre et ne rai-son-ne pas
Je sais qu'un tel dis-cours de moi pa-raît é-trange
Mais Ma-dame a-près tout je ne suis pas un ange
Et si vous con-dam-nez l'a-veu que je vous fais
Vous de-vez vous en prendre à vos char-mants at-traits
Dès que j'en vis brill-er la splen-deur plus qu'hu-maine
De mon in-té-ri-eur vous fû-tes sou-ve-raine
De vos re-gards di-vins l'i-nef-fa-ble dou-ceur
For-ça la ré-sis-tance où s'ob-sti-nait mon coeur
El-le sur-mon-ta tout jeû-nes pri-è-res larmes
Et tour-na tous mes voeux du cô-té de vos charmes
Mes yeux et mes sou-pirs vous l'ont dit mill-e fois
Et pour mieux m'ex-pli-quer j'em-ploie i-ci la voix
Que si vous con-tem-plez d'une âme un peu bé-nigne
Les tri-bu-la-ti-ons de votre es-clave in-digne
S'il faut que vos bon-tés veu-illent me con-so-ler
Et jusqu à mon né-ant dai-gnent se ra-va-ler
J'au-rai tou-jours pour vous ô su-a-ve mer-veille
U-ne dé-vo-ti-on à nulle au-tre pa-reille
Votre hon-neur a-vec moi ne court point de ha-sard
Et n'a nul-le dis-grâce à crain-dre de ma part
Tous ces ga-lants de cour dont les fem-mes sont folles
Sont bru-yants dans leurs faits et vains dans leurs pa-roles
De leurs pro-grès sans cesse on les voit se tar-guer
Ils n'ont point de fa-veurs qu'ils n'a-illent di-vul-guer
Et leur langue in-dis-crète en qui l'on se con-fie
Dé-sho-no-re l'au-tel où leur coeur sa-cri-fie
Mais les gens com-me nous brû-lent d'un feu dis-cret
A-vec qui pour tou-jours on est sûr du se-cret
Le soin que nous pre-nons de no-tre re-nom-mée
Ré-pond de tou-te chose à la per-sonne ai-mée
Et c'est en nous qu'on trouve ac-cep-tant no-tre coeur
De l'a-mour sans scan-dale et du plai-sir sans peur";

        const string TartuffePoesieSansDierese =
@"Ah pour ê-tre dé-vot je n'en suis pas moins homme
Et lors-qu on vient à voir vos cé-les-tes ap-pas
Un coeur se lais-se prendre et ne rai-son-ne pas
Je sais qu'un tel dis-cours de moi pa-raît é-trange
Mais Ma-dame a-près tout je ne suis pas un ange
Et si vous con-dam-nez l'a-veu que je vous fais
Vous de-vez vous en prendre à vos char-mants at-traits
Dès que j'en vis brill-er la splen-deur plus qu'hu-maine
De mon in-té-rieur vous fû-tes sou-ve-raine
De vos re-gards di-vins l'i-nef-fa-ble dou-ceur
For-ça la ré-sis-tance où s'ob-sti-nait mon coeur
El-le sur-mon-ta tout jeû-nes pri-è-res larmes
Et tour-na tous mes voeux du cô-té de vos charmes
Mes yeux et mes sou-pirs vous l'ont dit mill-e fois
Et pour mieux m'ex-pli-quer j'em-ploie i-ci la voix
Que si vous con-tem-plez d'une âme un peu bé-nigne
Les tri-bu-la-tions de votre es-clave in-digne
S'il faut que vos bon-tés veu-illent me con-so-ler
Et jusqu à mon né-ant dai-gnent se ra-va-ler
J'au-rai tou-jours pour vous ô su-a-ve mer-veille
U-ne dé-vo-tion à nulle au-tre pa-reille
Votre hon-neur a-vec moi ne court point de ha-sard
Et n'a nul-le dis-grâce à crain-dre de ma part
Tous ces ga-lants de cour dont les fem-mes sont folles
Sont bru-yants dans leurs faits et vains dans leurs pa-roles
De leurs pro-grès sans cesse on les voit se tar-guer
Ils n'ont point de fa-veurs qu'ils n'a-illent di-vul-guer
Et leur langue in-dis-crète en qui l'on se con-fie
Dé-sho-no-re l'au-tel où leur coeur sa-cri-fie
Mais les gens com-me nous brû-lent d'un feu dis-cret
A-vec qui pour tou-jours on est sûr du se-cret
Le soin que nous pre-nons de no-tre re-nom-mée
Ré-pond de tou-te chose à la per-sonne ai-mée
Et c'est en nous qu'on trouve ac-cep-tant no-tre coeur
De l'a-mour sans scan-dale et du plai-sir sans peur";

        const string TartuffeEcrit =
@"Ah pour ê-tre dé-vot je n'en suis pas moins hom-me
Et lors-qu on vient à voir vos cé-les-tes ap-pas
Un coeur se lais-se pren-dre et ne rai-son-ne pas
Je sais qu'un tel dis-cours de moi pa-raît é-tran-ge
Mais Ma-da-me a-près tout je ne suis pas un an-ge
Et si vous con-dam-nez l'a-veu que je vous fais
Vous de-vez vous en pren-dre à vos char-mants at-traits
Dès que j'en vis brill-er la splen-deur plus qu'hu-mai-ne
De mon in-té-rieur vous fû-tes sou-ve-rai-ne
De vos re-gards di-vins l'i-nef-fa-ble dou-ceur
For-ça la ré-sis-tan-ce où s'ob-sti-nait mon coeur
El-le sur-mon-ta tout jeû-nes pri-è-res lar-mes
Et tour-na tous mes voeux du cô-té de vos char-mes
Mes yeux et mes sou-pirs vous l'ont dit mill-e fois
Et pour mieux m'ex-pli-quer j'em-ploie i-ci la voix
Que si vous con-tem-plez d'u-ne â-me un peu bé-ni-gne
Les tri-bu-la-tions de vo-tre es-cla-ve in-di-gne
S'il faut que vos bon-tés veu-illent me con-so-ler
Et jusqu à mon né-ant dai-gnent se ra-va-ler
J'au-rai tou-jours pour vous ô su-a-ve mer-ve-ille
U-ne dé-vo-tion à nul-le au-tre pa-re-ille
Vo-tre hon-neur a-vec moi ne court point de ha-sard
Et n'a nul-le dis-grâ-ce à crain-dre de ma part
Tous ces ga-lants de cour dont les fem-mes sont fol-les
Sont bru-yants dans leurs faits et vains dans leurs pa-ro-les
De leurs pro-grès sans ces-se on les voit se tar-guer
Ils n'ont point de fa-veurs qu'ils n'a-illent di-vul-guer
Et leur lan-gue in-dis-crè-te en qui l'on se con-fie
Dé-sho-no-re l'au-tel où leur coeur sa-cri-fie
Mais les gens com-me nous brû-lent d'un feu dis-cret
A-vec qui pour tou-jours on est sûr du se-cret
Le soin que nous pre-nons de no-tre re-nom-mée
Ré-pond de tou-te cho-se à la per-son-ne ai-mée
Et c'est en nous qu'on trou-ve ac-cep-tant no-tre coeur
De l'a-mour sans scan-da-le et du plai-sir sans peur";


        [TestMethod]
        public void TestTartuffe()
        {
            TestTheText ttt = new TestTheText(Tartuffe);
            Config conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            ttt.AssertSyls(conf, TartuffePoesie);

            conf.sylConf.chercherDierese = false;
            ttt.AssertSyls(conf, TartuffePoesieSansDierese);

            conf.sylConf.mode = SylConfig.Mode.ecrit;
            ttt.AssertSyls(conf, TartuffeEcrit);
        }
    }
}
