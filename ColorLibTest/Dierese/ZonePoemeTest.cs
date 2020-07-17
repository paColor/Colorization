using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ColorLib;
using ColorLib.Dierese;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.Dierese
{
    [TestClass]
    public class ZonePoemeTest
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

        const string expiation31 =
            @"À la colonne veuve on rendit sa statue.
Quand on levait les yeux, on le voyait debout
Au-dessus de Paris, serein, dominant tout,
Seul, le jour dans l'azur et la nuit dans les astres.
Panthéons, on grava son nom sur vos pilastres !
On ne regarda plus qu'un seul côté des temps,
On ne se souvint plus que des jours éclatants
Cet homme étrange avait comme enivré l'histoire
La justice à l'œil froid disparut sous sa gloire ;
On ne vit plus qu'Eylau, Ulm, Arcole, Austerlitz ;
Comme dans les tombeaux des romains abolis,
On se mit à fouiller dans ces grandes années
Et vous applaudissiez, nations inclinées,
Chaque fois qu'on tirait de ce sol souverain
Ou le consul de marbre ou l'empereur d'airain !

V.

Le nom grandit quand l'homme tombe ;
Jamais rien de tel n'avait lui.
Calme, il écoutait dans sa tombe
La terre qui parlait de lui.
La terre disait : « La victoire
A suivi cet homme en tous lieux.
Jamais tu n'as vu, sombre histoire,
Un passant plus prodigieux !
« Gloire au maître qui dort sous l'herbe !
Gloire à ce grand audacieux !
Nous l'avons vu gravir, superbe,
Les premiers échelons des cieux !
« Il envoyait, âme acharnée,
Prenant Moscou, prenant Madrid,
Lutter contre la destinée
Tous les rêves de son esprit.
« À chaque instant, rentrant en lice,
Cet homme aux gigantesques pas
Proposait quelque grand caprice
À Dieu, qui n'y consentait pas.
« Il n'était presque plus un homme.
Il disait, grave et rayonnant,
En regardant fixement Rome
C'est moi qui règne maintenant !
« Il voulait, héros et symbole,
Pontife et roi, phare et volcan,
Faire du Louvre un Capitole
Et de Saint-Cloud un Vatican.
« César, il eût dit à Pompée :
« Sois fier d'être mon lieutenant ! »
On voyait luire son épée
Au fond d'un nuage tonnant.
« Il voulait, dans les frénésies
De ses vastes ambitions,
Faire devant ses fantaisies
Agenouiller les nations,
« Ainsi qu'en une urne profonde,
Mêler races, langues, esprits,
Répandre Paris sur le monde,
Enfermer le monde en Paris !
« Comme Cyrus dans Babylone,
Il voulait sous sa large main
Ne faire du monde qu'un trône
Et qu'un peuple du genre humain,
« Et bâtir, malgré les huées,
Un tel empire sous son nom,
Que Jéhovah dans les nuées
Fût jaloux de Napoléon ! »

VI.

Enfin, mort triomphant, il vit sa délivrance,
Et l'océan rendit son cercueil à la France.
";

        const string exp31Premiers15 = @"À la colonne veuve on rendit sa statue.
Quand on levait les yeux, on le voyait debout
Au-dessus de Paris, serein, dominant tout,
Seul, le jour dans l'azur et la nuit dans les astres.
Panthéons, on grava son nom sur vos pilastres !
On ne regarda plus qu'un seul côté des temps,
On ne se souvint plus que des jours éclatants
Cet homme étrange avait comme enivré l'histoire
La justice à l'œil froid disparut sous sa gloire ;
On ne vit plus qu'Eylau, Ulm, Arcole, Austerlitz ;
Comme dans les tombeaux des romains abolis,
On se mit à fouiller dans ces grandes années
Et vous applaudissiez, nations inclinées,
Chaque fois qu'on tirait de ce sol souverain
Ou le consul de marbre ou l'empereur d'airain !";

        const string exp31Prem15Syls = @"À la co-lon-ne veuve on ren-dit sa sta-tue
    Quand on le-vait les yeux on le voy-ait de-bout
    Au des-sus de Pa-ris se-rein do-mi-nant tout
    Seul le jour dans l'a-zur et la nuit dans les astres
    Pan-thé-ons on gra-va son nom sur vos pi-lastres
    On ne re-gar-da plus qu'un seul cô-té des temps
    On ne se sou-vint plus que des jours é-cla-tants
    Cet homme é-trange a-vait comme e-ni-vré l'his-toire
    La jus-tice à l'œil froid dis-pa-rut sous sa gloire
    On ne vit plus qu'Ey-lau Ulm Ar-cole Aus-ter-litz
    Com-me dans les tom-beaux des ro-mains a-bo-lis
    On se mit à fou-iller dans ces gran-des an-nées
    Et vous ap-plau-dis-siez na-tions in-cli-nées
    Cha-que fois qu'on ti-rait de ce sol sou-ve-rain
    Ou le con-sul de marbre ou l'em-pe-reur d'ai-rain";

        const string exp31Prem15SylsDierese = @"À la co-lon-ne veuve on ren-dit sa sta-tue
    Quand on le-vait les yeux on le voy-ait de-bout
    Au des-sus de Pa-ris se-rein do-mi-nant tout
    Seul le jour dans l'a-zur et la nuit dans les astres
    Pan-thé-ons on gra-va son nom sur vos pi-lastres
    On ne re-gar-da plus qu'un seul cô-té des temps
    On ne se sou-vint plus que des jours é-cla-tants
    Cet homme é-trange a-vait comme e-ni-vré l'his-toire
    La jus-tice à l'œil froid dis-pa-rut sous sa gloire
    On ne vit plus qu'Ey-lau Ulm Ar-cole Aus-ter-litz
    Com-me dans les tom-beaux des ro-mains a-bo-lis
    On se mit à fou-iller dans ces gran-des an-nées
    Et vous ap-plau-dis-siez na-ti-ons in-cli-nées
    Cha-que fois qu'on ti-rait de ce sol sou-ve-rain
    Ou le con-sul de marbre ou l'em-pe-reur d'ai-rain";


        private TestTheText ttt;
        private Config conf;
        private List<PhonWord> pws;

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() {
            ttt = new TestTheText(expiation31);
            conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            pws = ttt.GetPhonWordList(conf,true);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
            }
        }

        [TestMethod]
        public void Test151()
        {
            ZonePoeme zp = new ZonePoeme(ttt);
            int startLigne = 0;
            Vers v;
            for (int i = 0; i < 15; i++)
            {
                v = new Vers(ttt, startLigne, pws);
                Assert.IsTrue(zp.AddVers(v));
                startLigne = v.Last + 2;
            }

            Assert.AreEqual(exp31Premiers15, zp.ToString());

            startLigne = zp.Last + 2;
            v = new Vers(ttt, startLigne, pws);
            Assert.IsFalse(zp.AddVers(v));
            TestTheText.CompareWordByWord(exp31Prem15Syls, zp.Syllabes());
            zp.ChercheDierese(0);
            TestTheText.CompareWordByWord(exp31Prem15SylsDierese, zp.Syllabes());
        }

        const string v8 = @"Le nom grandit quand l'homme tombe ;
Jamais rien de tel n'avait lui.
Calme, il écoutait dans sa tombe
La terre qui parlait de lui.
La terre disait : « La victoire
A suivi cet homme en tous lieux.
Jamais tu n'as vu, sombre histoire,
Un passant plus prodigieux !
« Gloire au maître qui dort sous l'herbe !
Gloire à ce grand audacieux !
Nous l'avons vu gravir, superbe,
Les premiers échelons des cieux !
« Il envoyait, âme acharnée,
Prenant Moscou, prenant Madrid,
Lutter contre la destinée
Tous les rêves de son esprit.
« À chaque instant, rentrant en lice,
Cet homme aux gigantesques pas
Proposait quelque grand caprice
À Dieu, qui n'y consentait pas.
« Il n'était presque plus un homme.
Il disait, grave et rayonnant,
En regardant fixement Rome
C'est moi qui règne maintenant !
« Il voulait, héros et symbole,
Pontife et roi, phare et volcan,
Faire du Louvre un Capitole
Et de Saint-Cloud un Vatican.
« César, il eût dit à Pompée :
« Sois fier d'être mon lieutenant ! »
On voyait luire son épée
Au fond d'un nuage tonnant.
« Il voulait, dans les frénésies
De ses vastes ambitions,
Faire devant ses fantaisies
Agenouiller les nations,
« Ainsi qu'en une urne profonde,
Mêler races, langues, esprits,
Répandre Paris sur le monde,
Enfermer le monde en Paris !
« Comme Cyrus dans Babylone,
Il voulait sous sa large main
Ne faire du monde qu'un trône
Et qu'un peuple du genre humain,
« Et bâtir, malgré les huées,
Un tel empire sous son nom,
Que Jéhovah dans les nuées
Fût jaloux de Napoléon ! »";

        const string v8Syls = @"Le nom gran-dit quand l'hom-me tombe
    Ja-mais rien de tel n'a-vait lui
    Calme il é-cou-tait dans sa tombe
    La ter-re qui par-lait de lui
    La ter-re di-sait La vic-toire
    A sui-vi cet homme en tous lieux
    Ja-mais tu n'as vu sombre his-toire
    Un pas-sant plus pro-di-gieux
    Gloire au maî-tre qui dort sous l'herbe
    Gloire à ce grand au-da-cieux
    Nous l'a-vons vu gra-vir su-perbe
    Les pre-miers é-che-lons des cieux
    Il en-voy-ait âme a-char-née
    Pre-nant Mos-cou pre-nant Ma-drid
    Lut-ter con-tre la des-ti-née
    Tous les rê-ves de son es-prit
    À chaque ins-tant ren-trant en lice
    Cet homme aux gi-gan-tes-ques pas
    Pro-po-sait quel-que grand ca-price
    À Dieu qui n'y con-sen-tait pas
    Il n'é-tait pres-que plus un homme
    Il di-sait grave et ra-yon-nant
    En re-gar-dant fi-xe-ment Rome
    C'est moi qui rè-gne main-te-nant
    Il vou-lait hé-ros et sym-bole
    Pon-tife et roi phare et vol-can
    Fai-re du Louvre un Ca-pi-tole
    Et de Saint Cloud un Va-ti-can
    Cé-sar il eût dit à Pom-pée
    Sois fier d'ê-tre mon lieu-te-nant
    On voy-ait lui-re son é-pée
    Au fond d'un nu-a-ge ton-nant
    Il vou-lait dans les fré-né-sies
    De ses vas-tes am-bi-tions
    Fai-re de-vant ses fan-tai-sies
    A-ge-nou-iller les na-tions
    Ain-si qu'en une ur-ne pro-fonde
    Mê-ler ra-ces lan-gues es-prits
    Ré-pan-dre Pa-ris sur le monde
    En-fer-mer le monde en Pa-ris
    Com-me Cy-rus dans Ba-by-lone
    Il vou-lait sous sa lar-ge main
    Ne fai-re du mon-de qu'un trône
    Et qu'un peu-ple du genre hu-main
    Et bâ-tir mal-gré les hu-ées
    Un tel em-pi-re sous son nom
    Que Jéh-o-vah dans les nu-ées
    Fût ja-loux de Na-po-lé-on";

        const string v8SylsDierese = @"Le nom gran-dit quand l'hom-me tombe
    Ja-mais rien de tel n'a-vait lui
    Calme il é-cou-tait dans sa tombe
    La ter-re qui par-lait de lui
    La ter-re di-sait La vic-toire
    A sui-vi cet homme en tous lieux
    Ja-mais tu n'as vu sombre his-toire
    Un pas-sant plus pro-di-gi-eux
    Gloire au maî-tre qui dort sous l'herbe
    Gloire à ce grand au-da-ci-eux
    Nous l'a-vons vu gra-vir su-perbe
    Les pre-miers é-che-lons des cieux
    Il en-voy-ait âme a-char-née
    Pre-nant Mos-cou pre-nant Ma-drid
    Lut-ter con-tre la des-ti-née
    Tous les rê-ves de son es-prit
    À chaque ins-tant ren-trant en lice
    Cet homme aux gi-gan-tes-ques pas
    Pro-po-sait quel-que grand ca-price
    À Dieu qui n'y con-sen-tait pas
    Il n'é-tait pres-que plus un homme
    Il di-sait grave et ra-yon-nant
    En re-gar-dant fi-xe-ment Rome
    C'est moi qui rè-gne main-te-nant
    Il vou-lait hé-ros et sym-bole
    Pon-tife et roi phare et vol-can
    Fai-re du Louvre un Ca-pi-tole
    Et de Saint Cloud un Va-ti-can
    Cé-sar il eût dit à Pom-pée
    Sois fier d'ê-tre mon lieu-te-nant
    On voy-ait lui-re son é-pée
    Au fond d'un nu-a-ge ton-nant
    Il vou-lait dans les fré-né-sies
    De ses vas-tes am-bi-ti-ons
    Fai-re de-vant ses fan-tai-sies
    A-ge-nou-iller les na-ti-ons
    Ain-si qu'en une ur-ne pro-fonde
    Mê-ler ra-ces lan-gues es-prits
    Ré-pan-dre Pa-ris sur le monde
    En-fer-mer le monde en Pa-ris
    Com-me Cy-rus dans Ba-by-lone
    Il vou-lait sous sa lar-ge main
    Ne fai-re du mon-de qu'un trône
    Et qu'un peu-ple du genre hu-main
    Et bâ-tir mal-gré les hu-ées
    Un tel em-pi-re sous son nom
    Que Jéh-o-vah dans les nu-ées
    Fût ja-loux de Na-po-lé-on";

        [TestMethod]
        public void Test8()
        {
            ZonePoeme zp = new ZonePoeme(ttt);
            int startLigne = 720;
            Vers v;
            for (int i = 0; i < 48; i++)
            {
                v = new Vers(ttt, startLigne, pws);
                Assert.IsTrue(zp.AddVers(v));
                startLigne = v.Last + 2;
            }

            Assert.AreEqual(v8, zp.ToString());

            startLigne = zp.Last + 2;
            v = new Vers(ttt, startLigne, pws);
            Assert.IsFalse(zp.AddVers(v));
            
            TestTheText.CompareWordByWord(v8Syls, zp.Syllabes());
            zp.ChercheDierese(0);
            TestTheText.CompareWordByWord(v8SylsDierese, zp.Syllabes());
        }
    }
}
