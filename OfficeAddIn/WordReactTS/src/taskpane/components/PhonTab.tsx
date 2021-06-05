import * as React from "react";
import { useBoolean } from '@fluentui/react-hooks';
import { DefaultButton, DefaultPalette, getColorFromRGBA, getColorFromString, IButtonStyles, IStackItemStyles, IStackStyles, IStackTokens, Stack, Text } from "@fluentui/react";
import CommandButton from "./CommandButton";
import PhonControl from "./PhonControl";
import { useState } from "react";
import CharFormatForm from "./CharFormatForm";
import CharFormatting from "../Configs/CharFormatting";
import PhonConfig from "../Configs/PhonConfig";

export interface PhonTabProps {
    pc: PhonConfig;
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
  
  
export default function PhonTab(props: PhonTabProps) {
  
    const pc = props.pc;
  
    // Pour CharFormatForm
    const [isCFFOpen, { setTrue: showCFF, setFalse: hideCFF }] = useBoolean(false);
    const [phonToEdit, setPTE] = useState("");
    const white = getColorFromString('#ffffff')!;
    const [cffColor, setCffColor] = useState(white);
    const [cffBold, {toggle : clickCffBold, setTrue : setBold, setFalse : clearBold}] = useBoolean(false);
    const [cffItalic, {toggle : clickCffItalic, setTrue : setItalic, setFalse : clearItalic}] = useBoolean(false);
    const [cffUnderline, {toggle : clickCffUnderline, setTrue : setUnderline, setFalse : clearUnderline}] = useBoolean(false);
  
    function LoadCffData() {
      pc.SetCF(phonToEdit, new CharFormatting(cffBold, cffItalic, cffUnderline, true, cffColor));
      hideCFF();
    }
  
    function SetChk(phon: string, valeurChkBox: boolean) {
      pc.SetChk(phon, valeurChkBox);
    }

    function LocSetCERAS() {
        pc.SetCERAS();
    }

    function LocSetRose() {
        pc.SetRose();
    }

    function LocChkTout() {
        pc.ChkTout();
    }

    function LocChkRien() {
        pc.ChkRien();
    }
    
    function OpenCFF(phon: string) {
      setPTE(phon);
      let cf = pc.GetCF(phon);
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
                cf = {pc.GetCF(phonList[i][0][0])}
                chk = {pc.GetChk(phonList[i][0][0])} 
                chkOnChange = {SetChk} 
                clickBut = {OpenCFF}/>
            </Stack.Item>
            <Stack.Item key= {i*101} align="auto" grow styles={slStackItemStyles}> 
            <PhonControl 
                key= {phonList[i][1][0]} 
                phon= {phonList[i][1][0]} 
                phonTxt ={phonList[i][1][1]} 
                butTxt={phonList[i][1][2]} 
                cf = {pc.GetCF(phonList[i][1][0])}
                chk = {pc.GetChk(phonList[i][1][0])} 
                chkOnChange = {SetChk} 
                clickBut = {OpenCFF}/>
            </Stack.Item>
            <Stack.Item key= {i*10000} align="end" styles={slStackItemStyles}> 
            <PhonControl 
                key= {phonList[i][2][0]} 
                phon= {phonList[i][2][0]} 
                phonTxt ={phonList[i][2][1]} 
                butTxt={phonList[i][2][2]} 
                cf = {pc.GetCF(phonList[i][2][0])}
                chk = {pc.GetChk(phonList[i][2][0])} 
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
                cf = {pc.GetCF(phonList[i][0][0])}
                chk = {pc.GetChk(phonList[i][0][0])} 
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
                cf = {pc.GetCF(phonList[i][1][0])}
                chk = {pc.GetChk(phonList[i][1][0])} 
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
            <DefaultButton text="tout" styles={narrowButStyles} onClick={LocChkTout}/>
          </Stack.Item>
          <Stack.Item align="auto" grow styles={slStackItemStyles}> 
            <DefaultButton 
              text="API ceras (foncé)" 
              styles={customButStyles} 
              onClick = {LocSetCERAS}
            />
          </Stack.Item>
          <Stack.Item align="auto" grow styles={slStackItemStyles}> 
            <DefaultButton 
              text="API ceras (rosé)" 
              styles={customButStyles}
              onClick = {LocSetRose}
            />
          </Stack.Item>
          <Stack.Item align="end" styles={slStackItemStyles}> 
            <DefaultButton text="rien" styles={narrowButStyles} onClick={LocChkRien}/>
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
                cf = {pc.GetCF("diz")}
                chk = {pc.GetChk("diz")} 
                chkOnChange = {SetChk} 
                clickBut = {OpenCFF} />
          </Stack.Item>
          <Stack.Item align="end" styles={slStackItemStyles}> 
            <PhonControl 
                key= "mil" 
                phon= "mil" 
                phonTxt ="[mil]" 
                butTxt="1000" 
                cf = {pc.GetCF("mil")}
                chk = {pc.GetChk("mil")}  
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
                  cf = {pc.GetCF("uni")}
                  chk = {pc.GetChk("uni")}  
                  chkOnChange = {SetChk} 
                  clickBut = {OpenCFF} />
          </Stack.Item>
          <Stack.Item align="auto" grow styles={slStackItemStyles}> 
            <PhonControl 
                key= "cen" 
                phon= "cen" 
                phonTxt ="[cen]" 
                butTxt="0100" 
                chk = {pc.GetChk("cen")}  
                cf = {pc.GetCF("cen")}
                chkOnChange = {SetChk} 
                clickBut = {OpenCFF} />
          </Stack.Item>
          <Stack.Item align="end" styles={slStackItemStyles}> 
            <PhonControl 
                key= "47" 
                phon= "47" 
                phonTxt ="[47]" 
                butTxt="0..9" 
                cf = {pc.GetCF("47")}
                chk = {pc.GetChk("47")}  
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
  
  