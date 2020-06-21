using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ColorLib;
using System.Diagnostics;

namespace ColorLibTest
{
    /// <summary>
    /// Essayons de comprendre ce que peut amener la parallélisation de certaines tâches.
    /// </summary>
    [TestClass]
    public class PerfParallelTest
    {
        private Stopwatch stopWatch;

        public PerfParallelTest()
        {
            stopWatch = new Stopwatch();
        }

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

        private class MyText : TheText
        {
            public MyText(string txt)
                :base(txt)
            { }

            protected override void SetChars(FormattedTextEl fte, Config conf)
            {
                // Do nothing :-)
            }
        }

        private MyText GetLongText()
        {
            // ************************************************************************************
            //                             MODIFIER POUR TESTER LES PERFS
            // ************************************************************************************
            // const int nrTxt = 100;
            const int nrTxt = 1;
            // ************************************************************************************

            StringBuilder sb = new StringBuilder(nrTxt * rougeEtNoir23.Length);
            for (int i = 0; i < nrTxt; i++)
            {
                sb.Append(rougeEtNoir23);
            }
            return new MyText(sb.ToString());
        }

        private void ReportStopWatchDuration(string caller)
        {
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:000}",
            ts.Minutes, ts.Seconds, ts.Milliseconds);
            Debug.WriteLine(caller + " Elapsed (m:s:ms) " + elapsedTime);
        }

        /// <summary>
        /// Mesurons le temps d'exécution de la fonction <c>MarkDuo</c> pour le long texte
        /// construit à partir de l'extrait de "Le Rouge et le Noir".
        /// </summary>
        /// <remarks>
        /// résultats:
        /// tout séquentiel: 10.8 secondes
        /// Avec le parallélismde dans GetPhonWordList: 7.3 secondes
        /// Avec le parallélisme dans GetPhonWords: 5.4 - 5.6 secondes
        /// </remarks>
        [TestMethod]
        public void MeasureDuo()
        {
            stopWatch.Restart();
            GetLongText().MarkDuo(new Config());
            stopWatch.Stop();
            ReportStopWatchDuration("MeasureDuo");
        }

        /// <summary>
        /// Mesurons le temps d'exécution de la fonction <c>ColorizePhons</c> pour le long texte
        /// construit à partir de l'extrait de "Le Rouge et le Noir".
        /// </summary>
        /// <remarks>
        /// résultats:
        /// tout séquentiel: 10.3 secondes
        /// Parallélisme dans GetPhonWords: 4.1 - 4.8 secondes
        /// 
        /// </remarks>
        [TestMethod]
        public void MeasurePhons()
        {
            stopWatch.Restart();
            GetLongText().ColorizePhons(new Config(), PhonConfType.phonemes);
            stopWatch.Stop();
            ReportStopWatchDuration("MeasurePhons");
        }

        [TestMethod]
        public void MeasureMultiplePhons()
        {
            int NrLoops = 1;
            MeasurePhons(Victor48, NrLoops, "Victor48");
            MeasurePhons(Victor52, NrLoops, "Victor52");
        }

        private void MeasurePhons(string s, int nrLoops, string reportText)
        {
            Config conf = new Config();
            stopWatch.Restart();
            for (int i = 0; i < nrLoops; i++)
            {
                MyText mt = new MyText(s);
                mt.ColorizePhons(conf, PhonConfType.phonemes);
            }
            stopWatch.Stop();
            ReportStopWatchDuration("MeasurePhons " + reportText);
        }

        private string Victor48 = @"Waterloo ! Waterloo ! Waterloo ! morne plaine ! 
            Comme une onde qui bout dans une urne trop pleine, Dans ton cirque de bois, 
            de coteaux, de vallons, La pâle mort mêlait les sombres bataillons. D'un côté 
            c'est l'Europe et de l'autre la France. Choc sanglant ! des héros";

        private string Victor52 = @"Waterloo ! Waterloo ! Waterloo ! morne plaine ! 
            Comme une onde qui bout dans une urne trop pleine, Dans ton cirque de bois, 
            de coteaux, de vallons, La pâle mort mêlait les sombres bataillons. D'un côté 
            c'est l'Europe et de l'autre la France. Choc sanglant ! des héros Dieu trompait
            l'espérance;";

        private string rougeEtNoir23 = @"
            Heureusement pour la réputation de M. de Rênal comme
            administrateur, un immense mur de soutènement était nécessaire à la
            promenade publique qui longe la colline à une centaine de pieds audessus
            du cours du Doubs. Elle doit à cette admirable position une
            des vues les plus pittoresques de France. Mais, à chaque printemps,
            les eaux de pluie sillonnaient la promenade, y creusaient des ravins et
            la rendaient impraticable. Cet inconvénient, senti par tous, mit M. de
            Rênal dans l’heureuse nécessité d’immortaliser son administration
            par un mur de vingt pieds de hauteur et de trente ou quarante toises
            de long.
            Le parapet de ce mur pour lequel M. de Rênal a dû faire trois
            voyages à Paris, car l’avant-dernier ministre de l’Intérieur s’était
            déclaré l’ennemi mortel de la promenade de Verrières, le parapet de
            ce mur s’élève maintenant de quatre pieds au-dessus du sol. Et,
            comme pour braver tous les ministres présents et passés, on le garnit
            en ce moment avec des dalles de pierre de taille.
            Combien de fois, songeant aux bals de Paris abandonnés la veille,
            et la poitrine appuyée contre ces grands blocs de pierre d’un beau
            gris tirant sur le bleu, mes regards ont plongé dans la vallée du
            Doubs !
            Au-delà, sur la rive gauche, serpentent cinq ou six vallées au fond
            desquelles l’oeil distingue fort bien de petits ruisseaux. Après avoir
            couru de cascade en cascade on les voit tomber dans le Doubs. Le
            soleil est fort chaud dans ces montagnes ; lorsqu’il brille d’aplomb,
            la rêverie du voyageur est abritée sur cette terrasse par de
            magnifiques platanes. Leur croissance rapide et leur belle verdure
            tirant sur le bleu, ils la doivent à la terre rapportée, que M. le maire a
            fait placer derrière son immense mur de soutènement, car, malgré
            l’opposition du conseil municipal, il a élargi la promenade de plus de
            six pieds (quoiqu’il soit ultra et moi libéral, je l’en loue), c’est
            pourquoi dans son opinion et dans celle de M. Valenod, l’heureux
            directeur du dépôt de mendicité de Verrières, cette terrasse peut
            soutenir la comparaison avec celle de Saint-Germain-en-Laye.
            Je ne trouve, quant à moi, qu’une chose à reprendre au COURS
            DE LÀ FIDÉLITÉ ; on lit ce nom officiel en quinze ou vingt
            endroits, sur des plaques de marbre qui ont valu une croix de plus à
            M. de Rênal ; ce que je reprocherais au Cours de la Fidélité, c’est la
            manière barbare dont l’autorité fait tailler et tondre jusqu’au vif ces
            vigoureux platanes. Au lieu de ressembler par leurs têtes basses,
            rondes et aplaties, à la plus vulgaire des plantes potagères, ils ne
            demanderaient pas mieux que d’avoir ces formes magnifiques qu’on
            leur voit en Angleterre.
            Mais la volonté de M. le maire est despotique, et deux fois par an
            tous les arbres appartenant à la commune sont impitoyablement
            amputés. Les libéraux de l’endroit prétendent, mais ils exagèrent, que
            la main du jardinier officiel est devenue bien plus sévère depuis que
            M. le vicaire Maslon a pris l’habitude de s’emparer des produits de la
            tonte.
            Ce jeune ecclésiastique fut envoyé de Besançon, il y a quelques
            années, pour surveiller l’abbé Chélan et quelques curés des environs.
            Un vieux chirurgien-major de l’armée d’Italie retiré à Verrières, et
            qui de son vivant était à la fois, suivant M. le maire, jacobin et
            bonapartiste, osa bien un jour se plaindre à lui de la mutilation
            périodique de ces beaux arbres.
            — J’aime l’ombre, répondit M. de Rênal avec la nuance de
            hauteur convenable quand on parle à un chirurgien, membre de la
            Légion d’honneur ; j’aime l’ombre, je fais tailler mes arbres pour
            donner de l’ombre, et je ne conçois pas qu’un arbre soit fait pour
            autre chose, quand toutefois, comme l’utile noyer, il ne rapporte pas
            de revenu.
            Voilà le grand mot qui décide de tout à Verrières : RAPPORTER
            DU REVENU. À lui seul il représente la pensée habituelle de plus
            des trois quarts des habitants.
            Rapporter du revenu est la raison qui décide de tout dans cette
            petite ville qui vous semblait si jolie.
            L’étranger qui arrive, séduit par la beauté des fraîches et
            profondes vallées qui l’entourent, s’imagine d’abord que ses
            habitants sont sensibles au beau , ils ne parlent que trop souvent de la
            beauté de leur pays : on ne peut pas nier qu’ils n’en fassent grand
            cas, mais c’est parce qu’elle attire quelques étrangers dont l’argent
            enrichit les aubergistes, ce qui, par le mécanisme de l’octroi, rapporte
            du revenu à la ville.
            C’était par un beau jour d’automne que M. de Rênal se promenait
            sur le Cours de la Fidélité, donnant le bras à sa femme. Tout en
            écoutant son mari qui parlait d’un air grave, l’oeil de Mme de Rênal
            suivait avec inquiétude les mouvements de trois petits garçons.
            L’aîné, qui pouvait avoir onze ans, s’approchait trop souvent du
            parapet et faisait mine d’y monter. Une voix douce prononçait alors
            le nom d’Adolphe, et l’enfant renonçait à son projet ambitieux. Mme
            de Rênal paraissait une femme de trente ans, mais encore assez jolie.
            — Il pourrait bien s’en repentir, ce beau monsieur de Paris, disait
            M. de Rênal d’un air offensé, et la joue plus pâle encore qu’à
            l’ordinaire. Je ne suis pas sans avoir quelques amis au Château…
            Mais, quoique je veuille vous parler de la province pendant deux
            cents pages, je n’aurai pas la barbarie de vous faire subir la longueur
            et les ménagements savants d’un dialogue de province.
            Ce beau monsieur de Paris, si odieux au maire de Verrières, n’était
            autre que M. Appert, qui, deux jours auparavant, avait trouvé le
            moyen de s’introduire non seulement dans la prison et le dépôt de
            mendicité de Verrières, mais aussi dans l’hôpital administré
            gratuitement par le maire et les principaux propriétaires de l’endroit.
            — Mais, disait timidement Mme de Rênal, quel tort peut vous
            faire ce monsieur de Paris, puisque vous administrez le bien des
            pauvres avec la plus scrupuleuse probité ?
            — Il ne vient que pour déverser le blâme, et ensuite il fera insérer
            des articles dans les journaux du libéralisme.
            — Vous ne les lisez jamais, mon ami.
            — Mais on nous parle de ces articles jacobins ; tout cela nous
            distrait et nous empêche de faire le bien. Quant à moi, je ne
            pardonnerai jamais au curé. [Historique.]

            Il faut savoir que le curé de Verrières, vieillard de quatre-vingts
            ans, mais qui devait à l’air vif de ces montagnes une santé et un
            caractère de fer, avait le droit de visiter à toute heure la prison,
            l’hôpital et même le dépôt de mendicité. C’était précisément à six
            heures du matin que M. Appert, qui de Paris était recommandé au
            curé, avait eu la sagesse d’arriver dans une petite ville curieuse.
            Aussitôt il était allé au presbytère.
            En lisant la lettre que lui écrivait M. le marquis de La Mole, pair
            de France, et le plus riche propriétaire de la province, le curé Chélan
            resta pensif.
            Je suis vieux et aimé ici, se dit-il enfin à mi-voix, ils n’oseraient !
            Se tournant tout de suite vers le monsieur de Paris, avec des yeux où,
            malgré le grand âge, brillait ce feu sacré qui annonce le plaisir de
            faire une belle action un peu dangereuse :
            — Venez avec moi, monsieur, et en présence du geôlier et surtout
            des surveillants du dépôt de mendicité, veuillez n’émettre aucune
            opinion sur les choses que nous verrons. M. Appert comprit qu’il
            avait affaire à un homme de coeur : il suivit le vénérable curé, visita
            la prison, l’hospice, le dépôt, fit beaucoup de questions, et, malgré
            d’étranges réponses, ne se permit pas la moindre marque de blâme.
            Cette visite dura plusieurs heures.
            Le curé invita à dîner M. Appert, qui prétendit avoir des lettres à
            écrire : il ne voulait pas compromettre davantage son généreux
            compagnon. Vers les trois heures, ces messieurs allèrent achever
            l’inspection du dépôt de mendicité, et revinrent ensuite à la prison.
            Là, ils trouvèrent sur la porte le geôlier, espèce de géant de six pieds
            de haut et à jambes arquées ; sa figure ignoble était devenue hideuse
            par l’effet de la terreur.
            — Ah ! monsieur, dit-il au curé, dès qu’il l’aperçut, ce monsieur,
            que je vois là avec vous, n’est-il pas M. Appert ?
            — Qu’importe ? dit le curé.
            — C’est que depuis hier j’ai l’ordre le plus précis, et que M. le
            préfet a envoyé par un gendarme, qui a dû galoper toute la nuit, de ne
            pas admettre M. Appert dans la prison.
            — Je vous déclare, M. Noiroud, dit le curé, que ce voyageur, qui
            est avec moi, est M. Appert. Reconnaissez-vous que j’ai le droit
            d’entrer dans la prison à toute heure du jour et de la nuit, et en me
            faisant accompagner par qui je veux ?
            — Oui, M. le curé, dit le geôlier à voix basse, et baissant la tête
            comme un bouledogue que fait obéir à regret la crainte du bâton.
            Seulement, M. le curé, j’ai femme et enfants, si je suis dénoncé on
            me destituera ; je n’ai pour vivre que ma place.
            — Je serais aussi bien fâché de perdre la mienne, reprit le bon
            curé, d’une voix de plus en plus émue.
            — Quelle différence ! reprit vivement le geôlier ; vous, M. le curé,
            on sait que vous avez 800 livres de rente, du bon bien au soleil…
            Tels sont les faits qui, commentés, exagérés de vingt façons
            différentes, agitaient depuis deux jours toutes les passions haineuses
            de la petite ville de Verrières. Dans ce moment, ils servaient de texte
            à la petite discussion que M. de Rênal avait avec sa femme. Le matin,
            suivi de M. Valenod, directeur du dépôt de mendicité, il était allé
            chez le curé pour lui témoigner le plus vif mécontentement. M.
            Chélan n’était protégé par personne ; il sentit toute la portée de leurs
            paroles.
            — Eh bien, messieurs ! je serai le troisième curé, de quatre-vingts
            ans d’âge, que l’on destituera dans ce voisinage. Il y a cinquante-six
            ans que je suis ici ; j’ai baptisé presque tous les habitants de la ville,
            qui n’était qu’un bourg quand j’y arrivai. Je marie tous les jours des
            jeunes gens, dont jadis j’ai marié les grands-pères. Verrières est ma
            famille ; mais je me suis dit, en voyant l’étranger : Cet homme venu
            de Paris peut être à la vérité un libéral, il n’y en a que trop ; mais
            quel mal peut-il faire à nos pauvres et à nos prisonniers ?
            Les reproches de M. de Rênal, et surtout ceux de M. Valenod, le
            directeur du dépôt de mendicité, devenant de plus en plus vifs :
            — Eh bien, messieurs ! faites-moi destituer, s’était écrié le vieux
            curé, d’une voix tremblante. Je n’en habiterai pas moins le pays.
            On sait qu’il y a quarante-huit ans, j’ai hérité d’un champ qui
            rapporte 800 livres. Je vivrai avec ce revenu. Je ne fais point
            d’économies dans ma place, moi, messieurs, et c’est peut-être
            pourquoi je ne suis pas si effrayé quand on parle de me la faire
            perdre.
            M. de Rênal vivait fort bien avec sa femme ; mais ne sachant que
            répondre à cette idée, qu’elle lui répétait timidement : « Quel mal ce
            monsieur de Paris peut-il faire aux prisonniers ? » il était sur le point
            de se fâcher tout à fait quand elle jeta un cri. Le second de ses fils
            venait de monter sur le parapet du mur de la terrasse, et y courait,
            quoique ce mur fût élevé de plus de vingt pieds sur la vigne qui est
            de l’autre côté. La crainte d’effrayer son fils et de le faire tomber
            empêchait Mme de Rênal de lui adresser la parole. Enfin l’enfant, qui
            riait de sa prouesse, ayant regardé sa mère, vit sa pâleur, sauta sur la
            promenade et accourut à elle. Il fut bien grondé.
            Ce petit événement changea le cours de la conversation.
            — Je veux absolument prendre chez moi Sorel, le fils du scieur de
            planches, dit M. de Rênal ; il surveillera les enfants qui commencent
            à devenir trop diables pour nous. C’est un jeune prêtre, ou autant
            vaut, bon latiniste, et qui fera faire des progrès aux enfants ; car il a
            un caractère ferme, dit le curé. Je lui donnerai 300 francs et la
            nourriture.
            J’avais quelques doutes sur sa moralité ; car il était le benjamin de
            ce vieux chirurgien, membre de la Légion d’honneur, qui, sous
            prétexte qu’il était leur cousin, était venu se mettre en pension chez
            les Sorel.
            Cet homme pouvait fort bien n’être au fond qu’un agent secret des
            libéraux ; il disait que l’air de nos montagnes faisait du bien à son
            asthme ; mais c’est ce qui n’est pas prouvé. Il avait fait toutes les
            campagnes de Buonaparté en Italie, et même avait, dit-on, signé non
            pour l’Empire dans le temps. Ce libéral montrait le latin au fils Sorel,
            et lui a laissé cette quantité de livres qu’il avait apportés avec lui.
            Aussi n’aurais-je jamais songé à mettre le fils du charpentier auprès
            de nos enfants ; mais le curé, justement la veille de la scène qui vient
            de nous brouiller à jamais, m’a dit que ce Sorel étudie la théologie
            depuis trois ans, avec le projet d’entrer au séminaire ; il n’est donc
            pas libéral, et il est latiniste.
            Cet arrangement convient de plus d’une façon, continua M. de
            Rênal, en regardant sa femme d’un air diplomatique ; le Valenod est
            tout fier des deux beaux normands qu’il vient d’acheter pour sa
            calèche. Mais il n’a pas de précepteur pour ses enfants.
            — Il pourrait bien nous enlever celui-ci.
            — Tu approuves donc mon projet ? dit M. de Rênal, remerciant sa
            femme, par un sourire, de l’excellente idée qu’elle venait d’avoir.
            Allons, voilà qui est décidé.
            — Ah, bon Dieu ! mon cher ami, comme tu prends vite un parti !
            — C’est que j’ai du caractère, moi, et le curé l’a bien vu. Ne
            dissimulons rien, nous sommes environnés de libéraux ici. Tous ces
            marchands de toile me portent envie, j’en ai la certitude ; deux ou
            trois deviennent des richards ; eh bien ! j’aime assez qu’ils voient
            passer les enfants de M. de Rênal allant à la promenade sous la
            conduite de leur précepteur. Cela imposera. Mon grand-père nous
            racontait souvent que, dans sa jeunesse, il avait eu un précepteur.
            C’est cent écus qu’il m’en pourra coûter, mais ceci doit être classé
            comme une dépense nécessaire pour soutenir notre rang.
            Cette résolution subite laissa Mme de Rênal toute pensive. C’était
            une femme grande, bien faite, qui avait été la beauté du pays, comme
            on dit dans ces montagnes. Elle avait un certain air de simplicité, et
            de la jeunesse dans la démarche ; aux yeux d’un Parisien, cette grâce
            naïve, pleine d’innocence et de vivacité, serait même allée jusqu’à
            rappeler des idées de douce volupté. Si elle eût appris ce genre de
            succès, Mme de Rênal en eût été bien honteuse. Ni la coquetterie, ni
            l’affectation n’avaient jamais approché de ce coeur. M. Valenod, le
            riche directeur du dépôt, passait pour lui avoir fait la cour, mais sans
            succès, ce qui avait jeté un éclat singulier sur sa vertu ; car ce M.
            Valenod, grand jeune homme, taillé en force, avec un visage coloré et
            de gros favoris noirs, était un de ces êtres grossiers, effrontés et
            bruyants, qu’en province on appelle de beaux hommes.
            Mme de Rênal, fort timide, et d’un caractère en apparence fort
            inégal, était surtout choquée du mouvement continuel et des éclats de
            voix de M. Valenod. L’éloignement qu’elle avait pour ce qu’à
            Verrières on appelle de la joie, lui avait valu la réputation d’être très
            fière de sa naissance. Elle n’y songeait pas, mais avait été fort
            contente de voir les habitants de la ville venir moins chez elle. Nous
            ne dissimulerons pas qu’elle passait pour sotte aux yeux de leurs
            dames, parce que, sans nulle politique à l’égard de son mari, elle
            laissait échapper les plus belles occasions de se faire acheter de
            beaux chapeaux de Paris ou de Besançon. Pourvu qu’on la laissât
            seule errer dans son beau jardin, elle ne se plaignait jamais.
            C’était une âme naïve, qui jamais ne s’était élevée même jusqu’à
            juger son mari, et à s’avouer qu’il l’ennuyait. Elle supposait, sans se
            le dire, qu’entre mari et femme il n’y avait pas de plus douces
            relations. Elle aimait surtout M. de Rênal quand il lui parlait de ses
            projets sur leurs enfants, dont il destinait l’un à l’épée, le second à la
            magistrature, et le troisième à l’Église. En somme, elle trouvait M. de
            Rênal beaucoup moins ennuyeux que tous les hommes de sa
            connaissance.
            Ce jugement conjugal était raisonnable. Le maire de Verrières
            devait une réputation d’esprit et surtout de bon ton à une demidouzaine
            de plaisanteries dont il avait hérité d’un oncle.
            Le vieux capitaine de Rênal servait avant la Révolution dans le
            régiment d’infanterie de M. le duc d’Orléans, et, quand il allait à
            Paris, était admis dans les salons du prince. Il y avait vu Mme de
            Montesson, la fameuse Mme de Genlis, M. Ducrest, l’inventeur du
            Palais-Royal. Ces personnages ne reparaissaient que trop souvent
            dans les anecdotes de M. de Rênal. Mais peu à peu ce souvenir de
            choses aussi délicates à raconter était devenu un travail pour lui, et,
            depuis quelque temps, il ne répétait que dans les grandes occasions
            ses anecdotes relatives à la maison d’Orléans. Comme il était
            d’ailleurs fort poli, excepté lorsqu’on parlait d’argent, il passait, avec
            raison, pour le personnage le plus aristocratique de Verrières.
        ";
    }
}
