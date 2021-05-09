import * as React from "react";
import { useBoolean } from '@fluentui/react-hooks';
import { DefaultButton, DefaultPalette, IButtonStyles, IStackItemStyles, IStackStyles, IStackTokens, Stack, Text } from "@fluentui/react";
import CommandButton from "./CommandButton";
import PhonControl from "./PhonControl";
import { useState } from "react";
import CharFormatForm from "./CharFormatForm";

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


const initChkPhons: Map<string, boolean> = new Map([
  ["a", false], ["q", false], ["i", false], ["y", false], ["1", false], ["u", false], ["é", false], ["o", false], ["è", false], ["an", false], ["on", false], ["2", false], ["oi", false], ["5", false], ["w", false], ["j", false], ["ill", false], ["ng", false], 
  ["gn", false], ["l", false], ["v", false], ["f", false], ["p", false], ["b", false], ["m", false], ["z", false], ["s", false], ["t", false], ["d", false], ["ks", false], ["gz", false], ["r", false], ["n", false], ["ge", false], ["ch", false], ["k", false],   
  ["g", false], ["ij", false], ["oin", false], ["47", false], ["uni", false], ["diz", false], ["cen", false], ["mil", false], ["_muet", false], ["q_caduc", false], 
]) 

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

  // Etat de la config
  const [chkPhons, setChkPhons] = useState(initChkPhons);

  // Pour forcer le rendu --> Il  a une variante officielle, ä adapter à l'occase
  const [dummy, setDummy] = useState(false)

  // Pour CharFormatForm
  const [isCFFOpen, { setTrue: showCFF, setFalse: hideCFF }] = useBoolean(false);
  const [phonToEdit, setPTE] = useState("");

  function SetChk(phon: string, chkBoxVal: boolean) {
    let chkMap = chkPhons;
    chkMap.set(phon, chkBoxVal);
    setChkPhons(chkMap);
    setDummy(!dummy);
  }
  
  function OpenCFF(phon: string) {
    setPTE(phon);
    showCFF();
  }

  let phonLines: Array<any> = new Array<any>();
  for (let i = 0; i < phonList.length; i++) {
    if (phonList[i].length === 3) {
      phonLines.push(
        <Stack key= {i} horizontal styles={phonLineStackStyles} tokens={phonTokens}>
          <Stack.Item key= {i*100} align="start" styles={slStackItemStyles}> 
            <PhonControl 
              key= {phonList[i][0][0]} 
              phon= {phonList[i][0][0]} 
              phonTxt ={phonList[i][0][1]} 
              butTxt={phonList[i][0][2]}
              chk = {chkPhons.get(phonList[i][0][0])} 
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF}/>
          </Stack.Item>
          <Stack.Item key= {i*101} align="auto" grow styles={slStackItemStyles}> 
          <PhonControl 
              key= {phonList[i][1][0]} 
              phon= {phonList[i][1][0]} 
              phonTxt ={phonList[i][1][1]} 
              butTxt={phonList[i][1][2]} 
              chk = {chkPhons.get(phonList[i][1][0])} 
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF}/>
          </Stack.Item>
          <Stack.Item key= {i*10000} align="end" styles={slStackItemStyles}> 
          <PhonControl 
              key= {phonList[i][2][0]} 
              phon= {phonList[i][2][0]} 
              phonTxt ={phonList[i][2][1]} 
              butTxt={phonList[i][2][2]} 
              chk = {chkPhons.get(phonList[i][2][0])} 
              chkOnChange = {SetChk}  
              clickBut = {OpenCFF}/>
          </Stack.Item>
        </Stack>
      )
    } else if (phonList[i].length === 2) {
      phonLines.push(
        <Stack horizontal styles={phonLineStackStyles} tokens={phonTokens}>
          <Stack.Item align="start" styles={slStackItemStyles}> 
            <PhonControl 
              key= {phonList[i][0][0]} 
              phon= {phonList[i][0][0]} 
              phonTxt ={phonList[i][0][1]} 
              butTxt={phonList[i][0][2]} 
              chk = {chkPhons.get(phonList[i][0][0])} 
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF} />
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
              chk = {chkPhons.get(phonList[i][1][0])} 
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF} />
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
              chk = {chkPhons.get("diz")} 
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF} />
        </Stack.Item>
        <Stack.Item align="end" styles={slStackItemStyles}> 
          <PhonControl 
              key= "mil" 
              phon= "mil" 
              phonTxt ="[mil]" 
              butTxt="1000" 
              chk = {chkPhons.get("mil")}  
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF} />
        </Stack.Item>
      </Stack>

      <Stack horizontal styles={phonLineStackStyles} tokens={phonTokens}>
        <Stack.Item align="start" styles={slStackItemStyles}> 
          <PhonControl 
                key= "uni" 
                phon= "uni" 
                phonTxt ="[uni]" 
                butTxt="0001" 
                chk = {chkPhons.get("uni")}  
                chkOnChange = {SetChk} 
                clickBut = {OpenCFF} />
        </Stack.Item>
        <Stack.Item align="auto" grow styles={slStackItemStyles}> 
          <PhonControl 
              key= "cen" 
              phon= "cen" 
              phonTxt ="[cen]" 
              butTxt="0100" 
              chk = {chkPhons.get("cen")}  
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF} />
        </Stack.Item>
        <Stack.Item align="end" styles={slStackItemStyles}> 
          <PhonControl 
              key= "47" 
              phon= "47" 
              phonTxt ="[47]" 
              butTxt="0..9" 
              chk = {chkPhons.get("47")}  
              chkOnChange = {SetChk} 
              clickBut = {OpenCFF} />
        </Stack.Item>
      </Stack>

      <CharFormatForm
        visible={isCFFOpen}
        phon= {phonToEdit}
        valid= {hideCFF}
        cancel= {hideCFF}
      />

    </div>
  )
}

