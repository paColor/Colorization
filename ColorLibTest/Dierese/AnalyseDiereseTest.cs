using System;
using System.Collections.Generic;
using System.Text;
using ColorLib;
using ColorLib.Dierese;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ColorLibTest.Dierese
{
    [TestClass]
    public class AnalyseDiereseTest
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

        const string expiation =
@"I.
Il neigeait. On était vaincu par sa conquête.
Pour la première fois l'aigle baissait la tête.
Sombres jours ! l'empereur revenait lentement,
Laissant derrière lui brûler Moscou fumant.
Il neigeait. L'âpre hiver fondait en avalanche.
Après la plaine blanche une autre plaine blanche.
On ne connaissait plus les chefs ni le drapeau.
Hier la grande armée, et maintenant troupeau.
On ne distinguait plus les ailes ni le centre.
Il neigeait. Les blessés s'abritaient dans le ventre
Des chevaux morts ; au seuil des bivouacs désolés
On voyait des clairons à leur poste gelés,
Restés debout, en selle et muets, blancs de givre,
Collant leur bouche en pierre aux trompettes de cuivre.
Boulets, mitraille, obus, mêlés aux flocons blancs,
Pleuvaient ; les grenadiers, surpris d'être tremblants,
Marchaient pensifs, la glace à leur moustache grise.
Il neigeait, il neigeait toujours ! La froide bise
Sifflait ; sur le verglas, dans des lieux inconnus,
On n'avait pas de pain et l'on allait pieds nus.
Ce n'étaient plus des cœurs vivants, des gens de guerre
C'était un rêve errant dans la brume, un mystère,
Une procession d'ombres sous le ciel noir.
La solitude vaste, épouvantable à voir,
Partout apparaissait, muette vengeresse.
Le ciel faisait sans bruit avec la neige épaisse
Pour cette immense armée un immense linceul ;
Et chacun se sentant mourir, on était seul.
— Sortira-t-on jamais de ce funeste empire ?
Deux ennemis ! le czar, le nord. Le nord est pire.
On jetait les canons pour brûler les affûts.
Qui se couchait, mourait. Groupe morne et confus,
Ils fuyaient ; le désert dévorait le cortège.
On pouvait, à des plis qui soulevaient la neige,
Voir que des régiments s'étaient endormis là.
Ô chutes d'Annibal ! lendemains d'Attila !
Fuyards, blessés, mourants, caissons, brancards, civières,
On s'écrasait aux ponts pour passer les rivières,
On s'endormait dix mille, on se réveillait cent.
Ney, que suivait naguère une armée, à présent
S'évadait, disputant sa montre à trois cosaques.
Toutes les nuits, qui vive ! alerte ! assauts ! attaques !
Ces fantômes prenaient leur fusil, et sur eux
Ils voyaient se ruer, effrayants, ténébreux,
Avec des cris pareils aux voix des vautours chauves,
D'horribles escadrons, tourbillons d'hommes fauves.
Toute une armée ainsi dans la nuit se perdait.
L'empereur était là, debout, qui regardait.
Il était comme un arbre en proie à la cognée.
Sur ce géant, grandeur jusqu'alors épargnée,
Le malheur, bûcheron sinistre, était monté ;
Et lui, chêne vivant, par la hache insulté,
Tressaillant sous le spectre aux lugubres revanches,
Il regardait tomber autour de lui ses branches.
Chefs, soldats, tous mouraient. Chacun avait son tour.
Tandis qu'environnant sa tente avec amour,
Voyant son ombre aller et venir sur la toile,
Ceux qui restaient, croyant toujours à son étoile,
Accusaient le destin de lèse-majesté,
Lui se sentit soudain dans l'âme épouvanté.
Stupéfait du désastre et ne sachant que croire,
L'empereur se tourna vers Dieu ; l'homme de gloire
Trembla ; Napoléon comprit qu'il expiait
Quelque chose peut-être, et, livide, inquiet,
Devant ses légions sur la neige semées :
« Est-ce le châtiment, dit-il, Dieu des armées ? »
Alors il s'entendit appeler par son nom
Et quelqu'un qui parlait dans l'ombre lui dit : Non.

II.

Waterloo ! Waterloo ! Waterloo ! morne plaine !
Comme une onde qui bout dans une urne trop pleine,
Dans ton cirque de bois, de coteaux, de vallons,
La pâle mort mêlait les sombres bataillons.
D'un côté c'est l'Europe et de l'autre la France.
Choc sanglant ! des héros Dieu trompait l'espérance
Tu désertais, victoire, et le sort était las.
Ô Waterloo ! je pleure et je m'arrête, hélas !
Car ces derniers soldats de la dernière guerre
Furent grands ; ils avaient vaincu toute la terre,
Chassé vingt rois, passé les Alpes et le Rhin,
Et leur âme chantait dans les clairons d'airain !

Le soir tombait ; la lutte était ardente et noire.
Il avait l'offensive et presque la victoire ;
Il tenait Wellington acculé sur un bois.
Sa lunette à la main, il observait parfois
Le centre du combat, point obscur où tressaille
La mêlée, effroyable et vivante broussaille,
Et parfois l'horizon, sombre comme la mer.
Soudain, joyeux, il dit : Grouchy ! — C'était Blücher.
L'espoir changea de camp, le combat changea d'âme,
La mêlée en hurlant grandit comme une flamme.
La batterie anglaise écrasa nos carrés.
La plaine, où frissonnaient les drapeaux déchirés,
Ne fut plus, dans les cris des mourants qu'on égorge,
Qu'un gouffre flamboyant, rouge comme une forge ;
Gouffre où les régiments comme des pans de murs
Tombaient, où se couchaient comme des épis mûrs
Les hauts tambours-majors aux panaches énormes,
Où l'on entrevoyait des blessures difformes !
Carnage affreux ! moment fatal ! L'homme inquiet
Sentit que la bataille entre ses mains pliait.
Derrière un mamelon la garde était massée.
La garde, espoir suprême et suprême pensée !
« Allons ! faites donner la garde ! » cria-t-il.
Et, lanciers, grenadiers aux guêtres de coutil,
Dragons que Rome eût pris pour des légionnaires,
Cuirassiers, canonniers qui traînaient des tonnerres,
Portant le noir colback ou le casque poli,
Tous, ceux de Friedland et ceux de Rivoli,
Comprenant qu'ils allaient mourir dans cette fête,
Saluèrent leur dieu, debout dans la tempête.
Leur bouche, d'un seul cri, dit : vive l'empereur !
Puis, à pas lents, musique en tête, sans fureur,
Tranquille, souriant à la mitraille anglaise,
La garde impériale entra dans la fournaise.
Hélas ! Napoléon, sur sa garde penché,
Regardait, et, sitôt qu'ils avaient débouché
Sous les sombres canons crachant des jets de soufre,
Voyait, l'un après l'autre, en cet horrible gouffre,
Fondre ces régiments de granit et d'acier
Comme fond une cire au souffle d'un brasier.
Ils allaient, l'arme au bras, front haut, graves, stoïques.
Pas un ne recula. Dormez, morts héroïques !
Le reste de l'armée hésitait sur leurs corps
Et regardait mourir la garde. — C'est alors
Qu'élevant tout à coup sa voix désespérée,
La Déroute, géante à la face effarée
Qui, pâle, épouvantant les plus fiers bataillons,
Changeant subitement les drapeaux en haillons,
À de certains moments, spectre fait de fumées,
Se lève grandissante au milieu des armées,
La Déroute apparut au soldat qui s'émeut,
Et, se tordant les bras, cria : Sauve qui peut !
Sauve qui peut ! — affront ! horreur ! — toutes les bouches
Criaient ; à travers champs, fous, éperdus, farouches,
Comme si quelque souffle avait passé sur eux,
Parmi les lourds caissons et les fourgons poudreux,
Roulant dans les fossés, se cachant dans les seigles,
Jetant shakos, manteaux, fusils, jetant les aigles,
Sous les sabres prussiens, ces vétérans, ô deuil !
Tremblaient, hurlaient, pleuraient, couraient ! — En un clin d'œil,
Comme s'envole au vent une paille enflammée,
S'évanouit ce bruit qui fut la grande armée,
Et cette plaine, hélas, où l'on rêve aujourd'hui,
Vit fuir ceux devant qui l'univers avait fui !
Quarante ans sont passés, et ce coin de la terre,
Waterloo, ce plateau funèbre et solitaire,
Ce champ sinistre où Dieu mêla tant de néants,
Tremble encor d'avoir vu la fuite des géants !

Napoléon les vit s'écouler comme un fleuve ;
Hommes, chevaux, tambours, drapeaux ; — et dans l'épreuve
Sentant confusément revenir son remords,
Levant les mains au ciel, il dit : « Mes soldats morts,
Moi vaincu ! mon empire est brisé comme verre.
Est-ce le châtiment cette fois, Dieu sévère ? »
Alors parmi les cris, les rumeurs, le canon,
Il entendit la voix qui lui répondait : Non !

III.

Il croula. Dieu changea la chaîne de l'Europe.

Il est, au fond des mers que la brume enveloppe,
Un roc hideux, débris des antiques volcans.
Le Destin prit des clous, un marteau, des carcans,
Saisit, pâle et vivant, ce voleur du tonnerre,
Et, joyeux, s'en alla sur le pic centenaire
Le clouer, excitant par son rire moqueur
Le vautour Angleterre à lui ronger le cœur.

Évanouissement d'une splendeur immense !
Du soleil qui se lève à la nuit qui commence,
Toujours l'isolement, l'abandon, la prison,
Un soldat rouge au seuil, la mer à l'horizon,
Des rochers nus, des bois affreux, l'ennui, l'espace,
Des voiles s'enfuyant comme l'espoir qui passe,
Toujours le bruit des flots, toujours le bruit des vents !
Adieu, tente de pourpre aux panaches mouvants,
Adieu, le cheval blanc que César éperonne !
Plus de tambours battant aux champs, plus de couronne,
Plus de rois prosternés dans l'ombre avec terreur,
Plus de manteau traînant sur eux, plus d'empereur !
Napoléon était retombé Bonaparte.
Comme un romain blessé par la flèche du Parthe,
Saignant, morne, il songeait à Moscou qui brûla.
Un caporal anglais lui disait : halte-là !
Son fils aux mains des rois ! sa femme aux bras d'un autre !
Plus vil que le pourceau qui dans l'égout se vautre,
Son sénat qui l'avait adoré l'insultait.
Au bord des mers, à l'heure où la bise se tait,
Sur les escarpements croulant en noirs décombres,
Il marchait, seul, rêveur, captif des vagues sombres.
Sur les monts, sur les flots, sur les cieux, triste et fier,
L'œil encore ébloui des batailles d'hier,
Il laissait sa pensée errer à l'aventure.
Grandeur, gloire, ô néant ! calme de la nature !
Les aigles qui passaient ne le connaissaient pas.
Les rois, ses guichetiers, avaient pris un compas
Et l'avaient enfermé dans un cercle inflexible.
Il expirait. La mort de plus en plus visible
Se levait dans sa nuit et croissait à ses yeux
Comme le froid matin d'un jour mystérieux.
Son âme palpitait, déjà presque échappée.
Un jour enfin il mit sur son lit son épée,
Et se coucha près d'elle, et dit : « C'est aujourd'hui »
On jeta le manteau de Marengo sur lui.
Ses batailles du Nil, du Danube, du Tibre,
Se penchaient sur son front, il dit : « Me voici libre !
Je suis vainqueur ! je vois mes aigles accourir ! »
Et, comme il retournait sa tête pour mourir,
Il aperçut, un pied dans la maison déserte,
Hudson Lowe guettant par la porte entrouverte.
Alors, géant broyé sous le talon des rois,
Il cria : « La mesure est comble cette fois !
Seigneur ! c'est maintenant fini ! Dieu que j'implore,
Vous m'avez châtié ! » La voix dit : Pas encore !

IV.

Ô noirs événements, vous fuyez dans la nuit !
L'empereur mort tomba sur l'empire détruit.
Napoléon alla s'endormir sous le saule.
Et les peuples alors, de l'un à l'autre pôle,
Oubliant le tyran, s'éprirent du héros.
Les poètes, marquant au front les rois bourreaux,
Consolèrent, pensifs, cette gloire abattue.
À la colonne veuve on rendit sa statue.
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

L'homme, depuis douze ans, sous le dôme doré
Reposait, par l'exil et par la mort sacré.
En paix ! — Quand on passait près du monument sombre,
On se le figurait, couronne au front, dans l'ombre,
Dans son manteau semé d'abeilles d'or, muet,
Couché sous cette voûte où rien ne remuait,
Lui, l'homme qui trouvait la terre trop étroite,
Le sceptre en sa main gauche et l'épée en sa droite,
À ses pieds son grand aigle ouvrant l'œil à demi,
Et l'on disait : C'est là qu'est César endormi !

Laissant dans la clarté marcher l'immense ville,
Il dormait ; il dormait confiant et tranquille.

VII.

Une nuit, — c'est toujours la nuit dans le tombeau, —
Il s'éveilla. Luisant comme un hideux flambeau,
D'étranges visions emplissaient sa paupière ;
Des rires éclataient sous son plafond de pierre ;
Livide, il se dressa ; la vision grandit ;
Ô terreur ! une voix qu'il reconnut, lui dit :

— Réveille-toi. Moscou, Waterloo, Sainte-Hélène,
L'exil, les rois geôliers, l'Angleterre hautaine
Sur ton lit accoudée à ton dernier moment,
Sire, cela n'est rien. Voici le châtiment :

La voix alors devint âpre, amère, stridente,
Comme le noir sarcasme et l'ironie ardente ;
C'était le rire amer mordant un demi-dieu.
— Sire ! on t'a retiré de ton Panthéon bleu !
Sire ! on t'a descendu de ta haute colonne !
Regarde. Des brigands, dont l'essaim tourbillonne,
D'affreux bohémiens, des vainqueurs de charnier
Te tiennent dans leurs mains et t'ont fait prisonnier.
À ton orteil d'airain leur patte infâme touche.
Ils t'ont pris. Tu mourus, comme un astre se couche,
Napoléon le Grand, empereur ; tu renais
Bonaparte, écuyer du cirque Beauharnais.
Te voilà dans leurs rangs, on t'a, l'on te harnache.
Ils t'appellent tout haut grand homme, entre eux, ganache.
Ils traînent, sur Paris qui les voit s'étaler,
Des sabres qu'au besoin ils sauraient avaler.
Aux passants attroupés devant leur habitacle,
Ils disent, entends-les : — Empire à grand spectacle !
Le pape est engagé dans la troupe ; c'est bien,
Nous avons mieux ; le czar en est mais ce n'est rien,
Le czar n'est qu'un sergent, le pape n'est qu'un bonze
Nous avons avec nous le bonhomme de bronze !
Nous sommes les neveux du grand Napoléon ! —
Et Fould, Magnan, Rouher, Parieu caméléon,
Font rage. Ils vont montrant un sénat d'automates.
Ils ont pris de la paille au fond des casemates
Pour empailler ton aigle, ô vainqueur d'Iéna !
Il est là, mort, gisant, lui qui si haut plana,
Et du champ de bataille il tombe au champ de foire.
Sire, de ton vieux trône ils recousent la moire.
Ayant dévalisé la France au coin d'un bois,
Ils ont à leurs haillons du sang, comme tu vois,
Et dans son bénitier Sibour lave leur linge.
Toi, lion, tu les suis ; leur maître, c'est le singe.
Ton nom leur sert de lit, Napoléon premier.
On voit sur Austerlitz un peu de leur fumier.
Ta gloire est un gros vin dont leur honte se grise.
Cartouche essaie et met ta redingote grise
On quête des liards dans le petit chapeau
Pour tapis sur la table ils ont mis ton drapeau.
À cette table immonde où le grec devient riche,
Avec le paysan on boit, on joue, on triche ;
Tu te mêles, compère, à ce tripot hardi,
Et ta main qui tenait l'étendard de Lodi,
Cette main qui portait la foudre, ô Bonaparte,
Aide à piper les dés et fait sauter la carte.
Ils te forcent à boire avec eux, et Carlier
Pousse amicalement d'un coude familier
Votre majesté, sire, et Piétri dans son antre
Vous tutoie, et Maupas vous tape sur le ventre.
Faussaires, meurtriers, escrocs, forbans, voleurs,
Ils savent qu'ils auront, comme toi, des malheurs
Leur soif en attendant vide la coupe pleine
À ta santé ; Poissy trinque avec Sainte-Hélène.

Regarde ! bals, sabbats, fêtes matin et soir.
La foule au bruit qu'ils font se culbute pour voir ;
Debout sur le tréteau qu'assiège une cohue
Qui rit, bâille, applaudit, tempête, siffle, hue,
Entouré de pasquins agitant leur grelot,
— Commencer par Homère et finir par Callot !
Épopée ! épopée ! oh ! quel dernier chapitre ! —
Entre Troplong paillasse et Chaix-d'Est-Ange pitre,
Devant cette baraque, abject et vil bazar
Où Mandrin mal lavé se déguise en César,
Riant, l'affreux bandit, dans sa moustache épaisse,
Toi, spectre impérial, tu bats la grosse caisse ! —

L'horrible vision s'éteignit. L'empereur,
Désespéré, poussa dans l'ombre un cri d'horreur,
Baissant les yeux, dressant ses mains épouvantées.
Les Victoires de marbre à la porte sculptées,
Fantômes blancs debout hors du sépulcre obscur,
Se faisaient du doigt signe, et, s'appuyant au mur,
Écoutaient le titan pleurer dans les ténèbres.
Et lui, cria : « Démon aux visions funèbres,
Toi qui me suis partout, que jamais je ne vois,
Qui donc es-tu ? — Je suis ton crime », dit la voix.
La tombe alors s'emplit d'une lumière étrange
Semblable à la clarté de Dieu quand il se venge
Pareils aux mots que vit resplendir Balthazar,
Deux mots dans l'ombre écrits flamboyaient sur César ;
Bonaparte, tremblant comme un enfant sans mère,
Leva sa face pâle et lut : — DIX-HUIT BRUMAIRE !
";

        const string expiationSyls =
@"I
    Il nei-geait On é-tait vain-cu par sa con-quête
    Pour la pre-miè-re fois l'ai-gle bais-sait la tête
    Som-bres jours l'em-pe-reur re-ve-nait len-te-ment
    Lais-sant der-riè-re lui brû-ler Mos-cou fu-mant
    Il nei-geait L'âpre hi-ver fon-dait en a-va-lanche
    A-près la plai-ne blanche une au-tre plai-ne blanche
    On ne con-nais-sait plus les chefs ni le dra-peau
    Hi-er la grande ar-mée et main-te-nant trou-peau
    On ne dis-tin-guait plus les ai-les ni le centre
    Il nei-geait Les bles-sés s'a-bri-taient dans le ventre
    Des che-vaux morts au seuil des bi-vouacs dé-so-lés
    On voy-ait des clai-rons à leur pos-te ge-lés
    Res-tés de-bout en selle et mu-ets blancs de givre
    Col-lant leur bouche en pierre aux trom-pet-tes de cuivre
    Bou-lets mi-traille o-bus mê-lés aux flo-cons blancs
    Pleu-vaient les gre-na-diers sur-pris d'ê-tre trem-blants
    Mar-chaient pen-sifs la glace à leur mous-ta-che grise
    Il nei-geait il nei-geait tou-jours La froi-de bise
    Sif-flait sur le ver-glas dans des lieux in-con-nus
    On n'a-vait pas de pain et l'on al-lait pieds nus
    Ce n'é-taient plus des cœurs vi-vants des gens de guerre
    C'é-tait un rêve er-rant dans la brume un mys-tère
    U-ne pro-ces-si-on d'om-bres sous le ciel noir
    La so-li-tu-de vaste é-pou-van-table à voir
    Par-tout ap-pa-rais-sait mu-et-te ven-ge-resse
    Le ciel fai-sait sans bruit a-vec la neige é-paisse
    Pour cette im-mense ar-mée un im-men-se lin-ceul
    Et cha-cun se sen-tant mou-rir on é-tait seul
    Sor-ti-ra t-on ja-mais de ce fu-neste em-pire
    Deux en-ne-mis le czar le nord Le nord est pire
    On je-tait les ca-nons pour brû-ler les af-fûts
    Qui se cou-chait mou-rait Grou-pe morne et con-fus
    Ils fu-yaient le dé-sert dé-vo-rait le cor-tège
    On pou-vait à des plis qui sou-le-vaient la neige
    Voir que des ré-gi-ments s'é-taient en-dor-mis là
    Ô chu-tes d'An-ni-bal len-de-mains d'At-ti-la
    Fu-yards bles-sés mou-rants cais-sons bran-cards ci-vières
    On s'é-cra-sait aux ponts pour pas-ser les ri-vières
    On s'en-dor-mait dix mille on se ré-ve-illait cent
    Ney que sui-vait na-guère une ar-mée à pré-sent
    S'é-va-dait dis-pu-tant sa montre à trois co-saques
    Tou-tes les nuits qui vive a-lerte as-sauts at-taques
    Ces fan-tô-mes pre-naient leur fu-sil et sur eux
    Ils voy-aient se ru-er ef-fra-yants té-né-breux
    A-vec des cris pa-reils aux voix des vau-tours chauves
    D'hor-ri-bles es-ca-drons tour-bill-ons d'hom-mes fauves
    Toute une ar-mée ain-si dans la nuit se per-dait
    L'em-pe-reur é-tait là de-bout qui re-gar-dait
    Il é-tait comme un arbre en proie à la co-gnée
    Sur ce gé-ant gran-deur jusqu a-lors é-par-gnée
    Le mal-heur bû-che-ron si-nistre é-tait mon-té
    Et lui chê-ne vi-vant par la hache in-sul-té
    Tres-sa-illant sous le spectre aux lu-gu-bres re-vanches
    Il re-gar-dait tom-ber au-tour de lui ses branches
    Chefs sol-dats tous mou-raient Cha-cun a-vait son tour
    Tan-dis qu'en-vi-ron-nant sa tente a-vec a-mour
    Voy-ant son ombre al-ler et ve-nir sur la toile
    Ceux qui res-taient croy-ant tou-jours à son é-toile
    Ac-cu-saient le des-tin de lè-se ma-jes-té
    Lui se sen-tit sou-dain dans l'âme é-pou-van-té
    Stu-pé-fait du dé-sastre et ne sa-chant que croire
    L'em-pe-reur se tour-na vers Dieu l'hom-me de gloire
    Trem-bla Na-po-lé-on com-prit qu'il ex-pi-ait
    Quel-que cho-se peut être et li-vide in-qui-et
    De-vant ses lé-gi-ons sur la nei-ge se-mées
    Est ce le châ-ti-ment dit il Dieu des ar-mées
    A-lors il s'en-ten-dit ap-pe-ler par son nom
    Et quelqu un qui par-lait dans l'om-bre lui dit Non
    
    I-I
    
    Wa-ter-loo Wa-ter-loo Wa-ter-loo mor-ne plaine
    Comme une on-de qui bout dans une ur-ne trop pleine
    Dans ton cir-que de bois de co-teaux de val-lons
    La pâ-le mort mê-lait les som-bres ba-ta-illons
    D'un cô-té c'est l'Eu-rope et de l'au-tre la France
    Choc san-glant des hé-ros Dieu trom-pait l'es-pé-rance
    Tu dé-ser-tais vic-toire et le sort é-tait las
    Ô Wa-ter-loo je pleure et je m'ar-rête hé-las
    Car ces der-niers sol-dats de la der-niè-re guerre
    Fu-rent grands ils a-vaient vain-cu tou-te la terre
    Chas-sé vingt rois pas-sé les Al-pes et le Rhin
    Et leur â-me chan-tait dans les clai-rons d'ai-rain
    
    Le soir tom-bait la lutte é-tait ar-dente et noire
    Il a-vait l'of-fen-sive et pres-que la vic-toire
    Il te-nait Wel-ling-ton ac-cu-lé sur un bois
    Sa lu-nette à la main il ob-ser-vait par-fois
    Le cen-tre du com-bat point ob-scur où tres-saille
    La mê-lée ef-froy-able et vi-van-te brous-saille
    Et par-fois l'ho-ri-zon som-bre com-me la mer
    Sou-dain joy-eux il dit Grou-chy C'é-tait Blü-cher
    L'es-poir chan-gea de camp le com-bat chan-gea d'âme
    La mê-lée en hur-lant gran-dit comme u-ne flamme
    La bat-te-rie an-glaise é-cra-sa nos car-rés
    La plaine où fris-son-naient les dra-peaux dé-chi-rés
    Ne fut plus dans les cris des mou-rants qu'on é-gorge
    Qu'un gouf-fre flam-boy-ant rou-ge comme u-ne forge
    Gouffre où les ré-gi-ments com-me des pans de murs
    Tom-baient où se cou-chaient com-me des é-pis mûrs
    Les hauts tam-bours ma-jors aux pa-na-ches é-normes
    Où l'on en-tre-voy-ait des bles-su-res dif-formes
    Car-nage af-freux mo-ment fa-tal L'homme in-qui-et
    Sen-tit que la ba-taille en-tre ses mains pli-ait
    Der-rière un ma-me-lon la garde é-tait mas-sée
    La garde es-poir su-prême et su-prê-me pen-sée
    Al-lons fai-tes don-ner la gar-de cri-a t-il
    Et lan-ciers gre-na-diers aux guê-tres de cou-til
    Dra-gons que Rome eût pris pour des lé-gi-on-naires
    Cui-ras-siers ca-non-niers qui traî-naient des ton-nerres
    Por-tant le noir col-back ou le cas-que po-li
    Tous ceux de Fri-e-dland et ceux de Ri-vo-li
    Com-pre-nant qu'ils al-laient mou-rir dans cet-te fête
    Sa-lu-è-rent leur dieu de-bout dans la tem-pête
    Leur bou-che d'un seul cri dit vi-ve l'em-pe-reur
    Puis à pas lents mu-sique en tê-te sans fu-reur
    Tran-quil-le sou-ri-ant à la mi-traille an-glaise
    La garde im-pé-ri-ale en-tra dans la four-naise
    Hé-las Na-po-lé-on sur sa gar-de pen-ché
    Re-gar-dait et si-tôt qu'ils a-vaient dé-bou-ché
    Sous les som-bres ca-nons cra-chant des jets de soufre
    Voy-ait l'un a-près l'autre en cet hor-ri-ble gouffre
    Fon-dre ces ré-gi-ments de gra-nit et d'a-cier
    Com-me fond u-ne cire au souf-fle d'un bra-sier
    Ils al-laient l'arme au bras front haut gra-ves sto-ïques
    Pas un ne re-cu-la Dor-mez morts hé-ro-ïques
    Le res-te de l'ar-mée hé-si-tait sur leurs corps
    Et re-gar-dait mou-rir la gar-de C'est a-lors
    Qu'é-le-vant tout à coup sa voix dé-ses-pé-rée
    La Dé-rou-te gé-ante à la face ef-fa-rée
    Qui pâle é-pou-van-tant les plus fiers ba-ta-illons
    Chan-geant su-bi-te-ment les dra-peaux en ha-illons
    À de cer-tains mo-ments spec-tre fait de fu-mées
    Se lè-ve gran-dis-sante au mi-lieu des ar-mées
    La Dé-route ap-pa-rut au sol-dat qui s'é-meut
    Et se tor-dant les bras cri-a Sau-ve qui peut
    Sau-ve qui peut af-front hor-reur tou-tes les bouches
    Cri-aient à tra-vers champs fous é-per-dus fa-rouches
    Com-me si quel-que souffle a-vait pas-sé sur eux
    Par-mi les lourds cais-sons et les four-gons pou-dreux
    Rou-lant dans les fos-sés se ca-chant dans les seigles
    Je-tant sha-kos man-teaux fu-sils je-tant les aigles
    Sous les sa-bres prus-siens ces vé-té-rans ô deuil
    Trem-blaient hur-laient pleu-raient cou-raient En un clin d'œil
    Com-me s'en-vole au vent u-ne paille en-flam-mée
    S'é-va-nou-it ce bruit qui fut la grande ar-mée
    Et cet-te plaine hé-las où l'on rêve au-jourd hui
    Vit fuir ceux de-vant qui l'u-ni-vers a-vait fui
    Qua-rante ans sont pas-sés et ce coin de la terre
    Wa-ter-loo ce pla-teau fu-nèbre et so-li-taire
    Ce champ si-nistre où Dieu mê-la tant de né-ants
    Tremble en-cor d'a-voir vu la fui-te des gé-ants
    
    Na-po-lé-on les vit s'é-cou-ler comme un fleuve
    Hom-mes che-vaux tam-bours dra-peaux et dans l'é-preuve
    Sen-tant con-fu-sé-ment re-ve-nir son re-mords
    Le-vant les mains au ciel il dit Mes sol-dats morts
    Moi vain-cu mon em-pire est bri-sé com-me verre
    Est ce le châ-ti-ment cet-te fois Dieu sé-vère
    A-lors par-mi les cris les ru-meurs le ca-non
    Il en-ten-dit la voix qui lui ré-pon-dait Non
    
    I-I-I
    
    Il crou-la Dieu chan-gea la chaî-ne de l'Eu-rope
    
    Il est au fond des mers que la brume en-ve-loppe
    Un roc hi-deux dé-bris des an-ti-ques vol-cans
    Le Des-tin prit des clous un mar-teau des car-cans
    Sai-sit pâle et vi-vant ce vo-leur du ton-nerre
    Et joy-eux s'en al-la sur le pic cen-te-naire
    Le clou-er ex-ci-tant par son ri-re mo-queur
    Le vau-tour An-gle-terre à lui ron-ger le cœur
    
    É-va-nou-is-se-ment d'u-ne splen-deur im-mense
    Du so-leil qui se lève à la nuit qui com-mence
    Tou-jours l'i-so-le-ment l'a-ban-don la pri-son
    Un sol-dat rouge au seuil la mer à l'ho-ri-zon
    Des ro-chers nus des bois af-freux l'en-nui l'es-pace
    Des voi-les s'en-fu-yant com-me l'es-poir qui passe
    Tou-jours le bruit des flots tou-jours le bruit des vents
    A-dieu ten-te de pourpre aux pa-na-ches mou-vants
    A-dieu le che-val blanc que Cé-sar é-pe-ronne
    Plus de tam-bours bat-tant aux champs plus de cou-ronne
    Plus de rois pros-ter-nés dans l'ombre a-vec ter-reur
    Plus de man-teau traî-nant sur eux plus d'em-pe-reur
    Na-po-lé-on é-tait re-tom-bé Bo-na-parte
    Comme un ro-main bles-sé par la flè-che du Parthe
    Sai-gnant morne il son-geait à Mos-cou qui brû-la
    Un ca-po-ral an-glais lui di-sait hal-te là
    Son fils aux mains des rois sa femme aux bras d'un autre
    Plus vil que le pour-ceau qui dans l'é-gout se vautre
    Son sé-nat qui l'a-vait a-do-ré l'in-sul-tait
    Au bord des mers à l'heure où la bi-se se tait
    Sur les es-car-pe-ments crou-lant en noirs dé-combres
    Il mar-chait seul rê-veur cap-tif des va-gues sombres
    Sur les monts sur les flots sur les cieux triste et fier
    L'œil en-core é-blou-i des ba-ta-illes d'hi-er
    Il lais-sait sa pen-sée er-rer à l'a-ven-ture
    Gran-deur gloire ô né-ant cal-me de la na-ture
    Les ai-gles qui pas-saient ne le con-nais-saient pas
    Les rois ses gui-che-tiers a-vaient pris un com-pas
    Et l'a-vaient en-fer-mé dans un cercle in-fle-xible
    Il ex-pi-rait La mort de plus en plus vi-sible
    Se le-vait dans sa nuit et crois-sait à ses yeux
    Com-me le froid ma-tin d'un jour mys-té-ri-eux
    Son â-me pal-pi-tait dé-jà presque é-chap-pée
    Un jour en-fin il mit sur son lit son é-pée
    Et se cou-cha près d'elle et dit C'est au-jourd hui
    On je-ta le man-teau de Ma-ren-go sur lui
    Ses ba-ta-illes du Nil du Da-nu-be du Tibre
    Se pen-chaient sur son front il dit Me voi-ci libre
    Je suis vain-queur je vois mes ai-gles ac-cou-rir
    Et comme il re-tour-nait sa tê-te pour mou-rir
    Il a-per-çut un pied dans la mai-son dé-serte
    Hud-son Low-e guet-tant par la porte en-trou-verte
    A-lors gé-ant broy-é sous le ta-lon des rois
    Il cri-a La me-sure est com-ble cet-te fois
    Sei-gneur c'est main-te-nant fi-ni Dieu que j'im-plore
    Vous m'a-vez châ-ti-é La voix dit Pas en-core
    
    IV
    
    Ô noirs é-vé-ne-ments vous fu-yez dans la nuit
    L'em-pe-reur mort tom-ba sur l'em-pi-re dé-truit
    Na-po-lé-on al-la s'en-dor-mir sous le saule
    Et les peu-ples a-lors de l'un à l'au-tre pôle
    Ou-bli-ant le ty-ran s'é-pri-rent du hé-ros
    Les po-è-tes mar-quant au front les rois bour-reaux
    Con-so-lè-rent pen-sifs cet-te gloire a-bat-tue
    À la co-lon-ne veuve on ren-dit sa sta-tue
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
    Ou le con-sul de marbre ou l'em-pe-reur d'ai-rain
    
    V
    
    Le nom gran-dit quand l'hom-me tombe
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
    Fût ja-loux de Na-po-lé-on
    
    VI
    
    En-fin mort tri-om-phant il vit sa dé-li-vrance
    Et l'o-cé-an ren-dit son cer-cueil à la France
    
    L'hom-me de-puis douze ans sous le dô-me do-ré
    Re-po-sait par l'e-xil et par la mort sa-cré
    En paix Quand on pas-sait près du mo-nu-ment sombre
    On se le fi-gu-rait cou-ronne au front dans l'ombre
    Dans son man-teau se-mé d'a-be-illes d'or mu-et
    Cou-ché sous cet-te voûte où rien ne re-mu-ait
    Lui l'hom-me qui trou-vait la ter-re trop é-troite
    Le sceptre en sa main gauche et l'é-pée en sa droite
    À ses pieds son grand aigle ou-vrant l'œil à de-mi
    Et l'on di-sait C'est là qu'est Cé-sar en-dor-mi
    
    Lais-sant dans la clar-té mar-cher l'im-men-se ville
    Il dor-mait il dor-mait con-fi-ant et tran-quille
    
    VI-I
    
    U-ne nuit c'est tou-jours la nuit dans le tom-beau
    Il s'é-ve-illa Lui-sant comme un hi-deux flam-beau
    D'é-tran-ges vi-si-ons em-plis-saient sa pau-pière
    Des ri-res é-cla-taient sous son pla-fond de pierre
    Li-vide il se dres-sa la vi-si-on gran-dit
    Ô ter-reur u-ne voix qu'il re-con-nut lui dit
    
    Ré-ve-ille toi Mos-cou Wa-ter-loo Sainte Hé-lène
    L'e-xil les rois geô-liers l'An-gle-ter-re hau-taine
    Sur ton lit ac-cou-dée à ton der-nier mo-ment
    Si-re ce-la n'est rien Voi-ci le châ-ti-ment
    
    La voix a-lors de-vint âpre a-mè-re stri-dente
    Com-me le noir sar-casme et l'i-ro-nie ar-dente
    C'é-tait le rire a-mer mor-dant un de-mi dieu
    Sire on t'a re-ti-ré de ton Pan-thé-on bleu
    Sire on t'a des-cen-du de ta hau-te co-lonne
    Re-gar-de Des bri-gands dont l'es-saim tour-bill-onne
    D'af-freux boh-é-mi-ens des vain-queurs de char-nier
    Te tien-nent dans leurs mains et t'ont fait pri-son-nier
    À ton or-teil d'ai-rain leur patte in-fâ-me touche
    Ils t'ont pris Tu mou-rus comme un as-tre se couche
    Na-po-lé-on le Grand em-pe-reur tu re-nais
    Bo-na-parte é-cu-yer du cir-que Beauh-ar-nais
    Te voi-là dans leurs rangs on t'a l'on te har-nache
    Ils t'ap-pel-lent tout haut grand homme entre eux ga-nache
    Ils traî-nent sur Pa-ris qui les voit s'é-ta-ler
    Des sa-bres qu'au be-soin ils sau-raient a-va-ler
    Aux pas-sants at-trou-pés de-vant leur ha-bi-tacle
    Ils di-sent en-tends les Em-pire à grand spec-tacle
    Le pape est en-ga-gé dans la trou-pe c'est bien
    Nous a-vons mieux le czar en est mais ce n'est rien
    Le czar n'est qu'un ser-gent le pa-pe n'est qu'un bonze
    Nous a-vons a-vec nous le bon-hom-me de bronze
    Nous som-mes les ne-veux du grand Na-po-lé-on
    Et Fould Ma-gnan Rouh-er Pa-rieu ca-mé-lé-on
    Font rage Ils vont mon-trant un sé-nat d'au-to-mates
    Ils ont pris de la paille au fond des ca-se-mates
    Pour em-pa-iller ton aigle ô vain-queur d'I-é-na
    Il est là mort gi-sant lui qui si haut pla-na
    Et du champ de ba-taille il tombe au champ de foire
    Si-re de ton vieux trône ils re-cou-sent la moire
    A-yant dé-va-li-sé la France au coin d'un bois
    Ils ont à leurs ha-illons du sang com-me tu vois
    Et dans son bé-ni-tier Si-bour la-ve leur linge
    Toi li-on tu les suis leur maî-tre c'est le singe
    Ton nom leur sert de lit Na-po-lé-on pre-mier
    On voit sur Aus-ter-litz un peu de leur fu-mier
    Ta gloire est un gros vin dont leur hon-te se grise
    Car-touche es-saie et met ta re-din-go-te grise
    On quê-te des li-ards dans le pe-tit cha-peau
    Pour ta-pis sur la table ils ont mis ton dra-peau
    À cet-te table im-monde où le grec de-vient riche
    A-vec le pa-y-san on boit on joue on triche
    Tu te mê-les com-père à ce tri-pot har-di
    Et ta main qui te-nait l'é-ten-dard de Lo-di
    Cet-te main qui por-tait la foudre ô Bo-na-parte
    Aide à pi-per les dés et fait sau-ter la carte
    Ils te for-cent à boire a-vec eux et Car-lier
    Pousse a-mi-ca-le-ment d'un cou-de fa-mi-lier
    Vo-tre ma-jes-té sire et Pié-tri dans son antre
    Vous tu-toie et Mau-pas vous ta-pe sur le ventre
    Faus-sai-res meur-tri-ers es-crocs for-bans vo-leurs
    Ils sa-vent qu'ils au-ront com-me toi des mal-heurs
    Leur soif en at-ten-dant vi-de la cou-pe pleine
    À ta san-té Pois-sy trinque a-vec Sainte Hé-lène
    
    Re-gar-de bals sab-bats fê-tes ma-tin et soir
    La foule au bruit qu'ils font se cul-bu-te pour voir
    De-bout sur le tré-teau qu'as-siège u-ne coh-ue
    Qui rit bâille ap-plau-dit tem-pê-te sif-fle hue
    En-tou-ré de pas-quins a-gi-tant leur gre-lot
    Com-men-cer par Ho-mère et fi-nir par Cal-lot
    É-po-pée é-po-pée oh quel der-nier cha-pitre
    En-tre Tro-plong pa-illasse et Chaix d'Est An-ge pitre
    De-vant cet-te ba-raque ab-ject et vil ba-zar
    Où Man-drin mal la-vé se dé-guise en Cé-sar
    Ri-ant l'af-freux ban-dit dans sa mous-tache é-paisse
    Toi spectre im-pé-ri-al tu bats la gros-se caisse
    
    L'hor-ri-ble vi-si-on s'é-tei-gnit L'em-pe-reur
    Dé-ses-pé-ré pous-sa dans l'ombre un cri d'hor-reur
    Bais-sant les yeux dres-sant ses mains é-pou-van-tées
    Les Vic-toi-res de marbre à la por-te scu-lptées
    Fan-tô-mes blancs de-bout hors du sé-pulcre ob-scur
    Se fai-saient du doigt signe et s'ap-pu-yant au mur
    É-cou-taient le ti-tan pleu-rer dans les té-nèbres
    Et lui cri-a Dé-mon aux vi-si-ons fu-nèbres
    Toi qui me suis par-tout que ja-mais je ne vois
    Qui donc es tu Je suis ton cri-me dit la voix
    La tombe a-lors s'em-plit d'u-ne lu-mière é-trange
    Sem-blable à la clar-té de Dieu quand il se venge
    Pa-reils aux mots que vit res-plen-dir Bal-tha-zar
    Deux mots dans l'ombre é-crits flam-boy-aient sur Cé-sar
    Bo-na-par-te trem-blant comme un en-fant sans mère
    Le-va sa fa-ce pâle et lut DIX HUIT BRU-MAIRE";

        private TestTheText ttt;
        private Config conf;
        private List<PhonWord> pws;

        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {
            ttt = new TestTheText(expiation);
            conf = new Config();
            conf.sylConf.mode = SylConfig.Mode.poesie;
            pws = ttt.GetPhonWordList(conf, true);
            foreach (PhonWord pw in pws)
            {
                pw.ComputeSyls();
            }
        }

        [TestMethod]
        public void TestChercheDierese()
        {
            List<ZonePoeme> zpL = AnalyseDierese.ChercheDierese(ttt, pws, 0);
            StringBuilder sb = new StringBuilder();
            //foreach (ZonePoeme zp in zpL)
            //{
            //    foreach (Vers v in zp.vList)
            //    {
            //        sb.AppendLine(v.Syllabes());
            //    }
            //}
            //Assert.AreEqual(expiationSyls, sb.ToString());

            TestTheText.CompareWordByWord(expiationSyls, TestTheText.ToSyllabes(pws));

            foreach (ZonePoeme zp in zpL)
            {
                foreach (Vers v in zp.vList)
                {
                    if (v.nrPieds > 4)
                    {
                        Assert.AreEqual(zp.nrPiedsVoulu, v.nrPieds);
                    }
                }
            }
        }
    }
}
