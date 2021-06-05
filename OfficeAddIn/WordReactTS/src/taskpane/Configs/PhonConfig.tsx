import { IRGB } from "@fluentui/react";
import { Dispatch, SetStateAction, useState } from "react";
import CharFormatting from "./CharFormatting";


const phonValides = [
  "a", "q", "i", "y", "1", "u", "é", "o", "è", "an", "on", "2", "oi", "5", "w", "j", "ill", "ng", 
  "gn", "l", "v", "f", "p", "b", "m", "z", "s", "t", "d", "ks", "gz", "r", "n", "ge", "ch", "k",   
  "g", "ij", "oin", "47", "uni", "diz", "cen", "mil", "_muet", "q_caduc", 
]

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
    ["ill",   new CharFormatting (false, true, false, true,  { r: 127, g: 241, b: 0   })], 
  ])

export default class PhonConfig { // équivalent de ColConfWin
    private readonly cfPhons: Map<string, CharFormatting>;
    private readonly setCFPhons: (newCFP : Map<string, CharFormatting>) => void;
    private readonly chkPhons: Map<string, boolean>;
    private readonly setChkPhons: (newCHKP: Map<string, boolean>) => void;
    private readonly dummy: boolean;
    private readonly setDummy: Dispatch<SetStateAction<boolean>>;

    constructor () {
        [this.cfPhons, this.setCFPhons] = useState(roseCFPhons);
        [this.chkPhons, this.setChkPhons] = useState(roseChkPhons);
        [this.dummy, this.setDummy] = useState(false);
    }

    public SetChk(phon: string, chkBoxVal: boolean) {
        let chkMap = this.chkPhons;
        chkMap.set(phon, chkBoxVal);
        this.setChkPhons(chkMap);
        this.setDummy(!this.dummy); // to force rendering
    }

    public GetChk(phon: string) : boolean {
        return this.chkPhons.get(phon);
    }
    
    public SetCF(phon: string, cf: CharFormatting) {
        let cfMap = this.cfPhons;
        cfMap.set(phon, cf);
        this.setCFPhons(cfMap);
    }

    public GetCF(phon: string) : CharFormatting {
        return this.cfPhons.get(phon);
    }

    public SetCERAS() {
        this.setChkPhons(cerasChkPhons);
        this.setCFPhons(cerasCFPhons);
        this.setDummy(!this.dummy);
    }

    public SetRose() {
        this.setChkPhons(roseChkPhons);
        this.setCFPhons(roseCFPhons);
        this.setDummy(!this.dummy);
    }

    public ChkTout() {
        this.SetAllChk(true);
    }

    public ChkRien() {
        this.SetAllChk(false);
    }

    private SetAllChk(val: boolean) {
        let chkMap = new Map<string, boolean> ();
        phonValides.forEach((phon:string) => {chkMap.set(phon, val)});
        this.setChkPhons(chkMap);
        this.setDummy(!this.dummy); // to force rendering
    }

}