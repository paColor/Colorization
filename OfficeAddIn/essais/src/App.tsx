import * as React from "react";
import { DefaultButton, DefaultPalette, IButtonStyles, IStackItemStyles, IStackStyles, IStackTokens, Stack, Text } from "@fluentui/react";
import CommandButton from "./CommandButton";
import PhonControl from "./PhonControl";
import { useState } from "react";

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
    marginBottom: 3,
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

const chiffresStackItemStyles: IStackItemStyles = {
  root: {
    alignItems: 'center',
    display: 'flex',
    justifyContent: 'center',
    overflow: 'hidden',
    width: 94,
  },
};


// const phonValides = [
//   "a", "q", "i", "y", "1", "u", "é", "o", "è", "an", "on", "2", "oi", "5", "w", "j", "ill", "ng", 
//   "gn", "l", "v", "f", "p", "b", "m", "z", "s", "t", "d", "ks", "gz", "r", "n", "ge", "ch", "k",   
//   "g", "ij", "oin", "47", "uni", "diz", "cen", "mil", "_muet", "q_caduc", 
// ]


// const mapPhonToIndex: Map<string, number> = new Map([
//   ["a", 0], ["q", 1], ["i", 2], ["y", 3], ["1", 4], ["u", 5], ["é", 6], ["o", 7], ["è", 8], ["an", 9], ["on", 10], ["2", 11], ["oi", 12], ["5", 13], ["w", 14], ["j", 15], ["ill", 16], ["ng", 17], 
//   ["gn", 18], ["l", 19], ["v", 20], ["f", 21], ["p", 22], ["b", 23], ["m", 24], ["z", 25], ["s", 26], ["t", 27], ["d", 28], ["ks", 29], ["gz", 30], ["r", 31], ["n", 32], ["ge", 33], ["ch", 34], ["k", 35],   
//   ["g", 36], ["ij", 37], ["oin", 38], ["47", 39], ["uni", 40], ["diz", 41], ["cen", 42], ["mil", 43], ["_muet", 44], ["q_caduc", 45], 
// ]) 

// const initChkPhons = [
//   false, false, false, false, false, false, false, false, false, false, 
//   false, false, false, false, false, false, false, false, false, false, 
//   false, false, false, false, false, false, false, false, false, false, 
//   false, false, false, false, false, false, false, false, false, false, 
//   false, false, false, false, false, false, false, false, false, false, 
// ]

interface ChkPhons {
  a: boolean, q: boolean, i: boolean, y: boolean, 1: boolean, u: boolean, é: boolean, o: boolean, è: boolean, an: boolean, on: boolean, 2: boolean, oi: boolean, 5: boolean, w: boolean, j: boolean, ill: boolean, ng: boolean, 
  gn: boolean, l: boolean, v: boolean, f: boolean, p: boolean, b: boolean, m: boolean, z: boolean, s: boolean, t: boolean, d: boolean, ks: boolean, gz: boolean, r: boolean, n: boolean, ge: boolean, ch: boolean, k: boolean,   
  g: boolean, ij: boolean, oin: boolean, 47: boolean, uni: boolean, diz: boolean, cen: boolean, mil: boolean, _muet: boolean, q_caduc: boolean, 
}

const iniChkPhons: ChkPhons = {
  a: false, q: false, i: false, y: false, 1: false, u: false, é: false, o: false, è: false, an: false, on: false, 2: false, oi: false, 5: false, w: false, j: false, ill: false, ng: false, 
  gn: false, l: false, v: false, f: false, p: false, b: false, m: false, z: false, s: false, t: false, d: false, ks: false, gz: false, r: false, n: false, ge: false, ch: false, k: false,   
  g: false, ij: false, oin: false, 47: false, uni: false, diz: false, cen: false, mil: false, _muet: false, q_caduc: false, 
}



const phonList = [
  [["a",  "[a]",  "ta, plat"],      ["u",   "[u]",   "cou roue"], ["on",  "[§]",   "son"]],
  [["q",  "[e]",  "le"],            ["é",   "[é]",   "né, été"],  ["2",   "[2]",   "feu oeuf"]],
  [["i",  "[i]",  "il, lit"],       ["o",   "[o]",   "mot eau"],  ["5",   "[5]",   "fin"]],
  [["y",  "[y]",  "tu, lu"],        ["è",   "[è]",   "sel"],      ["oi",  "[oi]",  "noix"]],
  [["ij", "[ij]", "pria"],          ["an",  "[@]",   "grand"],    ["1",   "[1]",   "parfum"]],
  [["q_caduc",  "[-]",  "e caduc"], ["_muet", "[#]", "_muet"],    ["oin", "[oin]", "soin"]],
  [["j",  "[j]",  "payer"],         ["ill", "[ill]", "feuille"],  ["r",   "[r]",   "rare"]],
  [["ng", "[ng]", "parking"],       ["m",   "[m]",   "pomme"],    ["n",   "[n]",   "Nicole"]],
  [["gn", "[gn]", "ligne"],         ["z",   "[z]",   "zoo"],      ["ge",  "[ge]",  "jupe"]],
  [["l",  "[l]",  "aller"],         ["s",   "[s]",   "scie"],     ["ch",  "[ch]",  "chat"]],
  [["v",  "[v]",  "veau"],          ["t",   "[t]",   "tortue"],   ["k",   "[k]",   "coq"]],
  [["f",  "[f]",  "effacer"],       ["d",   "[d]",   "dindon"],   ["g",   "[g]",   "gare"]],
  [["p",  "[p]",  "papa"],          ["ks",  "[ks]",  "rixe"],     ["w",   "[w]",   "kiwi"]],
  [["b",  "[b]",  "bébé"],          ["gz",  "[gz]",  "examen"]],
]

export default function App() {


  const [chkPhons, setChkPhons] = useState(iniChkPhons);

  const [cenState, setCenState] = useState(false);

  function SetChk(phon: string, chkBoxVal: boolean) {
    console.log(phon, chkBoxVal);
    let chkMap = chkPhons;
    chkMap[phon as keyof ChkPhons]=chkBoxVal;
    console.log(chkMap);
    setChkPhons(chkMap);
    setCenState(!cenState);
  }

  let phonLines: Array<any> = new Array<any>();
  for (let i = 0; i < phonList.length; i++) {
    if (phonList[i].length === 3) {
      phonLines.push(
        <Stack key= {i} horizontal styles={phonLineStackStyles} tokens={phonTokens}>
          <Stack.Item key= {i*100 + 1} align="start" styles={slStackItemStyles}> 
            <PhonControl 
              key= {phonList[i][0][0]} 
              phon= {phonList[i][0][0]} 
              phonTxt ={phonList[i][0][1]} 
              butTxt={phonList[i][0][2]}
              chk = {chkPhons[(phonList[i][0][0]) as keyof ChkPhons]} 
              chkOnChange = {SetChk} />
          </Stack.Item>
          <Stack.Item key= {i*100 + 2} align="auto" grow styles={slStackItemStyles}> 
          <PhonControl 
              key= {phonList[i][1][0]} 
              phon= {phonList[i][1][0]} 
              phonTxt ={phonList[i][1][1]} 
              butTxt={phonList[i][1][2]} 
              chk = {chkPhons[(phonList[i][1][0]) as keyof ChkPhons]} 
              chkOnChange = {SetChk} />
          </Stack.Item>
          <Stack.Item key= {i*100 + 3} align="end" styles={slStackItemStyles}> 
          <PhonControl 
              key= {phonList[i][2][0]} 
              phon= {phonList[i][2][0]} 
              phonTxt ={phonList[i][2][1]} 
              butTxt={phonList[i][2][2]} 
              chk = {chkPhons[(phonList[i][2][0]) as keyof ChkPhons]} 
              chkOnChange = {SetChk} />
          </Stack.Item>
        </Stack>
      )
    } else if (phonList[i].length === 2) {
      phonLines.push(
        <Stack key= {i} horizontal styles={phonLineStackStyles} tokens={phonTokens}>
          <Stack.Item align="start" styles={slStackItemStyles}> 
            <PhonControl 
              key= {phonList[i][0][0]} 
              phon= {phonList[i][0][0]} 
              phonTxt ={phonList[i][0][1]} 
              butTxt={phonList[i][0][2]} 
              chk = {chkPhons[(phonList[i][0][0]) as keyof ChkPhons]} 
              chkOnChange = {SetChk} />
          </Stack.Item>
          <Stack.Item align="auto" grow styles={slStackItemStyles}> 
            <Text></Text>
          </Stack.Item>
          <Stack.Item align="end" styles={slStackItemStyles}> 
          <PhonControl 
              key= {phonList[i][1][0]} 
              phon= {phonList[i][1][0]} 
              phonTxt ={phonList[i][1][1]} 
              butTxt={phonList[i][1][2]} 
              chk = {chkPhons[(phonList[i][1][0]) as keyof ChkPhons]} 
              chkOnChange = {SetChk} />
          </Stack.Item>
        </Stack>
      )
    }
    
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
            <Text block nowrap variant='medium'>Cocher les phonèmes</Text>
            <Text block nowrap variant='medium'>à mettre en évidence</Text>
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

      {/* Phonèmes */}
      {phonLines}

      {/* Chiffres */}
      <Stack horizontal styles={phonLineStackStyles} tokens={phonTokens}>
        <Stack.Item align="start" styles={chiffresStackItemStyles}> 
          <Text block nowrap variant='xLarge'>Chiffres</Text>
        </Stack.Item>
        <Stack.Item align="auto" grow styles={slStackItemStyles}> 
          <PhonControl 
              key= "diz" 
              phon= "diz" 
              phonTxt ="[diz]" 
              butTxt="0010" 
              chk = {chkPhons.diz} 
              chkOnChange = {SetChk} />
        </Stack.Item>
        <Stack.Item align="end" styles={slStackItemStyles}> 
          <PhonControl 
              key= "mil" 
              phon= "mil" 
              phonTxt ="[mil]" 
              butTxt="1000" 
              chk = {chkPhons.mil}  
              chkOnChange = {SetChk} />
        </Stack.Item>
      </Stack>

      <Stack horizontal styles={phonLineStackStyles} tokens={phonTokens}>
        <Stack.Item align="start" styles={slStackItemStyles}> 
          <PhonControl 
                key= "uni" 
                phon= "uni" 
                phonTxt ="[uni]" 
                butTxt="0001" 
                chk = {chkPhons.uni}  
                chkOnChange = {SetChk} />
        </Stack.Item>
        <Stack.Item align="auto" grow styles={slStackItemStyles}> 
          <PhonControl 
              key= "cen" 
              phon= "cen" 
              phonTxt ="[cen]" 
              butTxt="0100" 
              chk = {chkPhons.cen}  
              chkOnChange = {SetChk} />
        </Stack.Item>
        <Stack.Item align="end" styles={slStackItemStyles}> 
          <PhonControl 
              key= "47" 
              phon= "47" 
              phonTxt ="[47]" 
              butTxt="0..9" 
              chk = {chkPhons["47"]}  
              chkOnChange = {SetChk} />
        </Stack.Item>
      </Stack>

    </div>
  )
}


