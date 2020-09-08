using System;
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

        // Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            lPos = 12;
        }

        // [TestMethod]
        public void CheckIsSubset()
        {
            //foreach (string m in AutomRuleFilter.mots_s_final)
            //{
            //    if (!AutomRuleFilter.mots_s_final_Morphalou.Contains(m))
            //    {
            //        WriteBlock(m);
            //    }
            //}

            //Assert.IsTrue(AutomRuleFilter.mots_s_final.IsSubsetOf(AutomRuleFilter.mots_s_final_Morphalou));
        }


        [TestMethod]
        public void MotsSansAccents()
        {
            foreach (string v in AutomRuleFilter.mots_s_final)
            {
                string sansAcc = AutomRuleFilter.ChaineSansAccents(v);
                if (sansAcc != v && !AutomRuleFilter.mots_s_final.Contains(sansAcc))
                {
                    WriteBlock(sansAcc);
                }
            }
        }

        // [TestMethod]
        public void CheckPasdeDoubleCH()
        {
            //foreach (string v in AutomRuleFilter.motsCHk)
            //{
            //    int pos = v.IndexOf("ch");
            //    Assert.IsTrue(pos >= 0);
            //    if (v.IndexOf("ch", pos + 1) >= 0)
            //        Console.WriteLine(v);
            //    //Assert.IsTrue(v.IndexOf("ch", pos + 1) < 0);
            //}

            //Assert.IsTrue(AutomRuleFilter.verbes_mer.IsSubsetOf(AutomRuleFilter.verbes_mer_Morphalou));
        }

        [TestMethod]
        public void WriteWords()
        {
            Regex rx = new Regex(@"\b\w+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase); // matches words
            MatchCollection matches = rx.Matches(words);
            foreach (Match m in matches)
            {
                WriteBlock(m.Value);
            }
        }
            
            
        string words =
        @"
            abies
abraxas
acarus
acens
acinus
adonis
agasillis
agnus
agnès
agrostis
albatros
albinos
albinos
alcarazas
alios
alkermès
aloès
amadis
amaryllis
ambesas
ambitus
amnios
amorphophallus
amphioxus
anacampséros
anagyris
anchilops
angélus
anis
anschluss
anthyllis
anthémis
antinoüs
antitragus
anus
apios
apis
apus
argus
arsis
artocarpus
as
ascaris
asclépias
asparagus
aspergillus
aspergès
atlas
attacus
autobus
axis
azygos
azygos
bacchus
basileus
bellis
benedictus
benthos
bibliobus
bibus
biceps
bis
bis
bis
bisness
blaps
blockhaus
blocus
bonus
boss
business
byblos
byssus
cacatoès
cactus
calasiris
callaïs
calvados
campus
carolus
carus
catharsis
catoblépas
cavas
cawas
cens
centranthus
cers
chionis
chips
chips
chlass
chorus
christmas
cirrus
citrus
clebs
clitoris
coccus
cochylis
collapsus
coléus
committimus
complexus
conchylis
consensus
contresens
convolvulus
corpus
cortès
corylopsis
coréopsis
cosinus
cosmos
costus
couscous
couscouss
creps
criss
crocus
cross
crésus
csardas
cubitus
cumulus
cupressus
cursus
cycas
cyclas
cynips
cypris
céréus
dinornis
diplodocus
doris
dromos
dross
décubitus
dégobillis
détritus
edelweiss
eucalyptus
eudémis
excursus
express
express
facies
faciès
famulus
favus
ficus
fils
fœtus
fongus
forceps
fucus
galéopsis
garus
gauss
gibus
glass
gneiss
gradus
gratis
gratis
gus
gyrus
habitus
hamadryas
hamamélis
hammerless
heimatlos
heimatlos
hendiadys
hermès
herpès
hiatus
hibiscus
humus
humérus
hypocras
hypothalamus
hystérésis
hélas
ibis
ichtyornis
ichtys
ictus
iléus
infarctus
inlandsis
iris
iritis
isatis
ithos
jacobus
jadis
kermès
klebs
kleps
knickerbockers
koumis
koumys
kouros
kriss
kss
kwas
kwass
labadens
lapis
laps
lapsus
lathyrus
laïus
leggings
leggins
lexis
lias
libripens
links
liparis
lituus
loculus
locus
lœss
logos
londrès
lotus
lupus
lychnis
lys
machairodus
madras
mallus
maous
maouss
maravédis
mars
mas
mathesis
maïs
mess
minus
mirabilis
miss
mistress
modius
moos
mordicus
moss
motocross
motus
mucus
myosis
myosotis
médius
méphistophélès
mérinos
métis
naevus
naos
nauplius
nexus
nimbus
ninas
nodus
nonius
nostras
notos
notus
nucléus
nystagmus
négus
némésis
népenthès
oaristys
oasis
oculus
oestrus
olibrius
omnibus
omnibus
ononis
onyxis
ophrys
opus
orchis
oribus
orémus
os
ours
oxalis
pagus
paliurus
palmarès
pancréas
pandanus
pannus
papas
papyrus
paros
pastis
pataquès
pathos
pelvis
pemphigus
phallus
phimosis
phtiriasis
physalis
phébus
pityriasis
plexiglas
plexus
poncirus
praxis
princeps
princeps
processus
prolapsus
promérops
pronaos
propolis
prospectus
proteus
protéus
prunus
psoas
psoriasis
ptosis
pubis
pyrosis
pécoptéris
pénis
péplos
quadriceps
quibus
quitus
rachis
radius
rams
raptus
rasibus
raïs
reis
relaps
relaps
reps
reïs
rhinocéros
rhombus
rhésus
rictus
risorius
risorius
rollmops
rébus
rétrovirus
salpiglossis
sanctus
saros
sas
satyriasis
schlass
schnaps
schuss
schuss
sempervirens
sens
seps
sialis
siemens
sinus
sirventès
sitaris
skungs
skunks
socius
solidus
speiss
splénius
spéos
stamnos
stimulus
stokes
stradivarius
strass
stratus
stress
strophantus
strychnos
sycosis
syllabus
synopsis
syphilis
tabès
tamaris
tarantass
taxus
tennis
terminus
thalamus
thermos
thesaurus
tholos
thrips
thrombus
thymus
thésis
tonus
tophus
tournevis
tractus
tragus
trass
trias
triceps
triceratops
trichiasis
trismus
tumulus
tupinambis
turneps
tylenchus
typhus
téniasis
tétanos
upas
uraeus
urus
us
utérus
valgus
valgus
varus
vasistas
vidimus
virus
vitellus
volubilis
volvulus
vénus
williams
xiphias
xérès
yass
échinocactus
édelweiss
élaeis
éléphantiasis
épistaxis
éros
éthiops
acanthéchinus
ad honores
ad patres
ains
alloposus
amblyopsis
amblyornis
antivirus
bathycrinus
bathyptéroïs
batrachoseps
caryorrhexis
cetorhinus
chéiranthus
chéiromys
chélys
chéniscus
chiroteuthis
chlamydomonas
chronotaraxis
colotyphus
craniotabes
criocarcinus
crypsis
cynanthémis
cynorchis
delirium tremens
de profundis
dyscomyces
électrobus
électrotonus
épicanthus
épispadias
ès
extra muros
florès
glomus
glossochilus
gyrobus
habeas corpus
haliotis
halobenthos
hippotigris
hypertonus
in extremis
inmedias res
in partibus
intemporalibus
iridodonésis
knickers
lophaetus
macrothésaurus
malus
mégacéros
méningotyphus
métanauplius
micrococcus
minibus
minus habens
monocéros
morphochorésis
myriagauss
myxovirus
naturalibus
néphrotyphus
neuroptéris
nimbostratus
nounours
numerus clausus
odontophorus
oligoamnios
ovibos
ovotestis
palatoschizis
pardeuss
pedibus
pentacrinus
périonyxis
phycomyces
phytéléphas
pleurocanthus
poliovirus
prémycosis
préoestrus
pretium doloris
pronéphros
pronucléus
protococcus
protopterus
provirus
pterocarpus
pterygotus
rapidos
rhinochetus
rhinocoris
rhinovirus
rhizopus
s
saccharomyces
schizanthus
semper virens
sensass
s.o.s
sphénoptéris
s.s
stegosaurus
sui generis
syndésis
syneidésis
synizésis
syntomis
thanatos
thésaurus
tricératops
trichorhexis
trolleybus
tss
typhlosolis
ultravirus
uranoschisis
uranostaphyloschisis
vidéobus
vulgum pecus
zoobenthos
zygoptéris


        ";
    }
}
