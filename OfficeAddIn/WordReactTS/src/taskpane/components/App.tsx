import * as React from "react";
import { DefaultButton, DefaultPalette, IButtonStyles, IStackItemStyles, IStackStyles, IStackTokens, Stack, Text } from "office-ui-fabric-react";
import CommandButton from "./CommandButton";
import PhonControl from "./PhonControl";

export interface AppProps {
  title: string;
  isOfficeInitialized: boolean;
}

const stackTokens: IStackTokens = { 
  childrenGap: 3,
  padding: 2,
 };

 const phonTokens: IStackTokens = { 
  childrenGap: 3,
  padding: 0,
 };

const stackStyles: IStackStyles = {
  root: {
    // background: DefaultPalette.themeTertiary,
    overflow: 'auto',
    marginBottom: 8,
  },
};

const phonLineStackStyles: IStackStyles = {
  root: {
    overflow: 'auto',
    marginBottom: 2,
  },
};

// first line
const flStackItemStyles: IStackItemStyles = {
  root: {
    alignItems: 'center',
    display: 'flex',
    height: 50,
    justifyContent: 'center',
    overflow: 'hidden',
  },
};

// second line
const slStackItemStyles: IStackItemStyles = {
  root: {
    alignItems: 'center',
    display: 'flex',
    justifyContent: 'center',
    overflow: 'hidden',
  },
};

const customButStyles: IButtonStyles = { 
  root: {
    height: 16, 
    padding: 7,
    margin: 0,
    background: DefaultPalette.themeLighter,
  },
  label: {
    fontSize: 11,
  },
};

const narrowButStyles: IButtonStyles = { 
  root: {
    // width: 27,
    height: 16, 
    padding: 2,
    margin: 0,
    minWidth: 10,
    background: DefaultPalette.themeLighter,
  },
  label: {
    fontSize: 11,
  },
};

/* **********************************************************
let phons = new Map<string, PhonControlProps> ([
  ["a", {phonTxt: "[a]", butTxt: "ta, plat"}],
])

{"a",   {phonTxt: "[a]", butTxt: "ta, plat"  } },
{"q",   {phonTxt: "[e]", butTxt: "le"        } },
{"i",   {phonTxt: "[i]", butTxt: "il, lit"   } },
{"y",   {phonTxt: "[y]", butTxt: "tu, lu"    } },
{"1",   {phonTxt: "[1]", butTxt: "parfum"    } },
{"u",   {phonTxt: "[u]", butTxt: "cou, roue" } },
{"é",   {phonTxt: "[é]", butTxt: "né, été"   } },
{"o",   {phonTxt: "[o]", butTxt: "mot, eau"  } },
{"è",   {phonTxt: "[è]", butTxt: "sel"       } },
{"an",  {phonTxt: "[@]", butTxt: "grand"     } },
{"on",  {phonTxt: "[§]", butTxt: "son"       } },
{"2",   {phonTxt: "[2]", butTxt: "feu, oeuf" } },
{"oi",  {phonTxt: "[oi]", butTxt: "noix"      } },
{"5",   {phonTxt: "[5]", butTxt: "fin"       } },
{"w",   {phonTxt: "[w]", butTxt: "kiwi"      } },
{"j",   {phonTxt: "[j]", butTxt: "payer"     } },
{"ill", {phonTxt: "[ill]", butTxt: "feuille"   } },
{"ng",  {phonTxt: "[ng]", butTxt: "parking"   } },
{"gn",  {phonTxt: "[gn]", butTxt: "ligne"     } },
{"l",   {phonTxt: "[l]", butTxt: "aller"     } },
{"v",   {phonTxt: "[v]", butTxt: "veau"      } },
{"f",   {phonTxt: "[f]", butTxt: "effacer"   } },
{"p",   {phonTxt: "[p]", butTxt: "papa"      } },
{"b",   {phonTxt: "[b]", butTxt: "bébé"      } },
{"m",   {phonTxt: "[m]", butTxt: "pomme"     } },
{"z",   {phonTxt: "[z]", butTxt: "zoo"       } },
{"s",   {phonTxt: "[s]", butTxt: "scie"      } },
{"t",   {phonTxt: "[t]", butTxt: "tortue"    } },
{"d",   {phonTxt: "[d]", butTxt: "dindon"    } },
{"ks",  {phonTxt: "[ks]", butTxt: "rixe"      } },
{"gz",  {phonTxt: "[gz]", butTxt: "examen"    } },
{"r",   {phonTxt: "[r]", butTxt: "rare"      } },
{"n",   {phonTxt: "[n]", butTxt: "Nicole"    } },
{"ge",  {phonTxt: "[ge]", butTxt: "jupe"      } },
{"k",   {phonTxt: "[k]", butTxt: "coq"       } },
{"g",   {phonTxt: "[g]", butTxt: "gare"      } },
{"ch",  {phonTxt: "[ch]", butTxt: "chat"      } },
{"ij",  {phonTxt: "[ij]", butTxt: "pria"      } },
{"47",  {phonTxt: "[47]", butTxt: "0..9"      } },
{"oin", {phonTxt: "[oin]", butTxt: "soin"      } },
{"uni", {phonTxt: "[uni]", butTxt: "0001"      } },
{"diz", {phonTxt: "[diz]", butTxt: "0010"      } },
{"cen", {phonTxt: "[cen]", butTxt: "0100"      } },
{"mil", {phonTxt: "[mil]", butTxt: "1000"      } },
{"_muet", {phonTxt: "[#]", butTxt: "\'muet\'"  } },
{"q_caduc", {phonTxt: "[-]", butTxt: "e caduc" } },  

*******************************************************************/

let phonList = [
  [["a", "[a]", "ta, plat"], ["u", "[u]", "cou roue"], ["on", "[§]", "son"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["i", "[i]", "il, lit"],  ["o", "[o]", "mot eau"],  ["5", "[5]", "fin"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
  [["q", "[e]", "le"],       ["é", "[é]", "né, été"],  ["2", "[2]", "feu oeuf"]],
]

export default function App() {
  let phonLines: Array<any> = new Array<any>();
  for (let i = 0; i < phonList.length; i++) {
    phonLines.push(
      <Stack horizontal styles={phonLineStackStyles} tokens={phonTokens}>
        <Stack.Item align="start" styles={slStackItemStyles}> 
          <PhonControl 
            key= {phonList[i][0][0]} 
            phon= {phonList[i][0][0]} 
            phonTxt ={phonList[i][0][1]} 
            butTxt={phonList[i][0][2]} />
        </Stack.Item>
        <Stack.Item align="auto" grow styles={slStackItemStyles}> 
        <PhonControl 
            key= {phonList[i][1][0]} 
            phon= {phonList[i][1][0]} 
            phonTxt ={phonList[i][1][1]} 
            butTxt={phonList[i][1][2]} />
        </Stack.Item>
        <Stack.Item align="end" styles={slStackItemStyles}> 
        <PhonControl 
            key= {phonList[i][2][0]} 
            phon= {phonList[i][2][0]} 
            phonTxt ={phonList[i][2][1]} 
            butTxt={phonList[i][2][2]} />
        </Stack.Item>
      </Stack>
    )
  }
  
  return (

    <div>
      {/* Première ligne */}
      <Stack horizontal tokens={stackTokens}>
        <Stack.Item align="start" styles={flStackItemStyles}>
          <CommandButton
            butTitle="Cololriser les phonèmes"
            iconSrc="../assets/phon-carré 52.png" />
        </Stack.Item>

        <Stack.Item align="center" grow styles={flStackItemStyles}>
          <div>
            <Text block variant='medium'>Cocher les phonèmes</Text>
            <Text block variant='medium'>à mettre en évidence</Text>
          </div>
        </Stack.Item>

        <Stack.Item align="end" styles={flStackItemStyles}>
          <CommandButton
            butTitle="Cololriser en enoir et enlever le formattage"
            iconSrc="../assets/black_carre_64.png" />
        </Stack.Item>
      </Stack>

      {/* deuxième ligne */}
      <Stack horizontal styles={stackStyles} tokens={stackTokens}>
        <Stack.Item align="start" styles={slStackItemStyles}> 
          <DefaultButton text="tout" styles={narrowButStyles}/>
        </Stack.Item>
        <Stack.Item align="auto" grow styles={slStackItemStyles}> 
          <DefaultButton text="API ceras (foncé)" styles={customButStyles} />
        </Stack.Item>
        <Stack.Item align="auto" grow styles={slStackItemStyles}> 
          <DefaultButton text="API ceras (rosé)" styles={customButStyles}/>
        </Stack.Item>
        <Stack.Item align="end" styles={slStackItemStyles}> 
          <DefaultButton text="rien" styles={narrowButStyles}/>
        </Stack.Item>
      </Stack>

      {phonLines}


      

    </div>
  )
}

