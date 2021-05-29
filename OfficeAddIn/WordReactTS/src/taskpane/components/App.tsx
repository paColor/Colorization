import * as React from "react";
import { useBoolean } from '@fluentui/react-hooks';
import { DefaultButton, DefaultPalette, getColorFromRGBA, getColorFromString, IButtonStyles, IRGB, IStackItemStyles, IStackStyles, IStackTokens, Stack, Text } from "@fluentui/react";
import CommandButton from "./CommandButton";
import PhonControl from "./PhonControl";
import { useState } from "react";
import CharFormatForm from "./CharFormatForm";
import CharFormatting from "./CharFormatting";

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

/* 
const phonValides = [
  "a", "q", "i", "y", "1", "u", "é", "o", "è", "an", "on", "2", "oi", "5", "w", "j", "ill", "ng", 
  "gn", "l", "v", "f", "p", "b", "m", "z", "s", "t", "d", "ks", "gz", "r", "n", "ge", "ch", "k",   
  "g", "ij", "oin", "47", "uni", "diz", "cen", "mil", "_muet", "q_caduc", 
]
 */
/* 
const initChkPhons: Map<string, boolean> = new Map([
  ["a", false], ["q", false], ["i", false], ["y", false], ["1", false], ["u", false], ["é", false], ["o", false], ["è", false], ["an", false], ["on", false], ["2", false], ["oi", false], ["5", false], ["w", false], ["j", false], ["ill", false], ["ng", false], 
  ["gn", false], ["l", false], ["v", false], ["f", false], ["p", false], ["b", false], ["m", false], ["z", false], ["s", false], ["t", false], ["d", false], ["ks", false], ["gz", false], ["r", false], ["n", false], ["ge", false], ["ch", false], ["k", false],   
  ["g", false], ["ij", false], ["oin", false], ["47", false], ["uni", false], ["diz", false], ["cen", false], ["mil", false], ["_muet", false], ["q_caduc", false], 
]) 
 */

const cerasChkPhons: Map<string, boolean> = new Map([
  ["a", false], ["q", false], ["i", false], ["y", false], ["1", true], ["u", true], ["é", true], ["o", true], ["è", true], ["an", true], ["on", true], ["2", true], ["oi", true], ["5", true], ["w", false], ["j", false], ["ill", false], ["ng", false], 
  ["gn", false], ["l", false], ["v", false], ["f", false], ["p", false], ["b", false], ["m", false], ["z", false], ["s", false], ["t", false], ["d", false], ["ks", false], ["gz", false], ["r", false], ["n", false], ["ge", false], ["ch", false], ["k", false],   
  ["g", false], ["ij", false], ["oin", true], ["47", false], ["uni", false], ["diz", false], ["cen", false], ["mil", false], ["_muet", true], ["q_caduc", false], 
]) 

const roseChkPhons: Map<string, boolean> = new Map([
  ["a", false], ["q", false], ["i", false], ["y", false], ["1", true], ["u", true], ["é", true], ["o", true], ["è", true], ["an", true], ["on", true], ["2", true], ["oi", true], ["5", true], ["w", false], ["j", false], ["ill", true], ["ng", false], 
  ["gn", false], ["l", false], ["v", false], ["f", false], ["p", false], ["b", false], ["m", false], ["z", false], ["s", false], ["t", false], ["d", false], ["ks", false], ["gz", false], ["r", false], ["n", false], ["ge", false], ["ch", false], ["k", false],   
  ["g", false], ["ij", false], ["oin", true], ["47", false], ["uni", false], ["diz", false], ["cen", false], ["mil", false], ["_muet", true], ["q_caduc", false], 
]) 

const black: IRGB = { r: 0, g: 0, b: 0 }; 
const defCF: CharFormatting = new CharFormatting(false, false, false, false, black);

const cerasCFPhons: Map<string, CharFormatting> = new Map([
  ["a", defCF], ["q", defCF], ["i", defCF], ["y", defCF], ["w", defCF], ["j", defCF], ["ill", defCF], ["ng", defCF], 
  ["gn", defCF], ["l", defCF], ["v", defCF], ["f", defCF], ["p", defCF], ["b", defCF], ["m", defCF], ["z", defCF], ["s", defCF], 
  ["t", defCF], ["d", defCF], ["ks", defCF], ["gz", defCF], ["r", defCF], ["n", defCF], ["ge", defCF], ["ch", defCF], ["k", defCF],   
  ["g", defCF], ["ij", defCF], ["47", defCF], ["uni", defCF], ["diz", defCF], ["cen", defCF], ["mil", defCF], ["q_caduc", defCF], 
  ["1",     new CharFormatting (false, false, true, false, {r: 0, g: 0, b: 0})], 
  ["u",     new CharFormatting (false, false, false, true, {r: 255, g: 0, b: 0})], 
  ["é",     new CharFormatting (false, false, false, true, {r: 0, g: 20, b: 208})], 
  ["o",     new CharFormatting (false, false, false, true, {r: 240, g: 222, b: 0})], 
  ["è",     new CharFormatting (false, false, false, true, {r: 164, g: 20, b: 210})], 
  ["an",    new CharFormatting (false, false, false, true, {r: 237, g: 125, b: 49})], 
  ["on",    new CharFormatting (false, false, false, true, {r: 172, g: 121, b: 66})], 
  ["2",     new CharFormatting (false, false, false, true, {r: 71, g: 115, b: 255})], 
  ["oi",    new CharFormatting (true, false, false, true,  {r: 0, g: 0, b: 0})], 
  ["5",     new CharFormatting (false, false, false, true, {r: 51, g: 153, b: 102})],
  ["oin",   new CharFormatting (false, false, false, true, {r: 15, g: 201, b: 221})],
  ["_muet", new CharFormatting (false, false, false, true, {r: 166, g: 166, b: 166})],
])

const roseCFPhons: Map<string, CharFormatting> = new Map([
  ["a", defCF], ["q", defCF], ["i", defCF], ["y", defCF], ["w", defCF], ["j", defCF], ["ng", defCF], 
  ["gn", defCF], ["l", defCF], ["v", defCF], ["f", defCF], ["p", defCF], ["b", defCF], ["m", defCF], ["z", defCF], ["s", defCF], 
  ["t", defCF], ["d", defCF], ["ks", defCF], ["gz", defCF], ["r", defCF], ["n", defCF], ["ge", defCF], ["ch", defCF], ["k", defCF],   
  ["g", defCF], ["ij", defCF], ["47", defCF], ["uni", defCF], ["diz", defCF], ["cen", defCF], ["mil", defCF], ["q_caduc", defCF], 
  ["1",     new CharFormatting (false, false, true, false, { r: 0,   g: 0,   b: 0   })], 
  ["u",     new CharFormatting (false, false, false, true, { r: 255, g: 0,   b: 0   })], 
  ["é",     new CharFormatting (false, false, false, true, { r: 255, g: 100, b: 177 })], 
  ["o",     new CharFormatting (false, false, false, true, { r: 240, g: 222, b: 0   })], 
  ["è",     new CharFormatting (false, false, false, true, { r: 164, g: 20,  b: 210 })], 
  ["an",    new CharFormatting (false, false, false, true, { r: 237, g: 125, b: 49  })], 
  ["on",    new CharFormatting (false, false, false, true, { r: 172, g: 121, b: 66  })], 
  ["2",     new CharFormatting (false, false, false, true, { r: 71,  g: 115, b: 255 })], 
  ["oi",    new CharFormatting (true, false, false, true,  { r: 0,   g: 0,   b: 0   })], 
  ["5",     new CharFormatting (false, false, false, true, { r: 51,  g: 153, b: 102 })],
  ["oin",   new CharFormatting (false, false, false, true, { r: 15,  g: 201, b: 221 })],
  ["_muet", new CharFormatting (false, false, false, true, { r: 166, g: 166, b: 166 })],
  ["ill",   new CharFormatting (false, false, false, true, { r: 127, g: 241, b: 0   })], 
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

  const [cfPhons, setCFPhons] = useState(roseCFPhons);
  const [chkPhons, setChkPhons] = useState(roseChkPhons);

  // Pour forcer le rendu
  const [dummy, setDummy] = useState(false)

  // Pour CharFormatForm
  const [isCFFOpen, { setTrue: showCFF, setFalse: hideCFF }] = useBoolean(false);
  const [phonToEdit, setPTE] = useState("");
  const white = getColorFromString('#ffffff')!;
  const [cffColor, setCffColor] = useState(white);
  const [cffBold, {toggle : clickCffBold, setTrue : setBold, setFalse : clearBold}] = useBoolean(false);
  const [cffItalic, {toggle : clickCffItalic, setTrue : setItalic, setFalse : clearItalic}] = useBoolean(false);
  const [cffUnderline, {toggle : clickCffUnderline, setTrue : setUnderline, setFalse : clearUnderline}] = useBoolean(false);

  function SetChk(phon: string, chkBoxVal: boolean) {
    let chkMap = chkPhons;
    chkMap.set(phon, chkBoxVal);
    setChkPhons(chkMap);
    setDummy(!dummy); // to force rendering
  }

  function SetCF(phon: string, cf: CharFormatting) {
    let cfMap = cfPhons;
    cfMap.set(phon, cf);
    setCFPhons(cfMap);
  }

  function LoadCffData() {
    SetCF(phonToEdit, new CharFormatting(cffBold, cffItalic, cffUnderline, true, cffColor));
    hideCFF();
  }

  function SetCERAS() {
    setChkPhons(cerasChkPhons);
    setCFPhons(cerasCFPhons);
    setDummy(!dummy);
  }

  function SetRose() {
    setChkPhons(roseChkPhons);
    setCFPhons(roseCFPhons);
    setDummy(!dummy);
  }
  
  function OpenCFF(phon: string) {
    setPTE(phon);
    let cf = cfPhons.get(phon);
    if (cf.bold) {
      setBold();
    } else {
      clearBold();
    }
    if (cf.italic) {
      setItalic();
    } else {
      clearItalic();
    }
    if (cf.underline) {
      setUnderline();
    } else {
      clearUnderline();
    }
    setCffColor(getColorFromRGBA(cf.color));
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
          <DefaultButton 
            text="API ceras (foncé)" 
            styles={customButStyles} 
            onClick = {SetCERAS}
          />
        </Stack.Item>
        <Stack.Item align="auto" grow styles={slStackItemStyles}> 
          <DefaultButton 
            text="API ceras (rosé)" 
            styles={customButStyles}
            onClick = {SetRose}
          />
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
        bold= {cffBold}
        clickBold = {clickCffBold}
        italic= {cffItalic}
        clickItalic={clickCffItalic}
        underline= {cffUnderline}
        clickUnderline= {clickCffUnderline}
        color= {cffColor}
        setColor= {setCffColor}
        valid= {LoadCffData}
        cancel= {hideCFF}
      />

    </div>
  )
}

