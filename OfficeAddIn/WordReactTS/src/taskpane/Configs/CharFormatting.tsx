import { IRGB } from "@fluentui/react";

export default class CharFormatting {
    bold: boolean;
    italic: boolean;
    underline: boolean;
    changeColor: boolean;
    color: IRGB;

    constructor(inBold: boolean, inItalic: boolean, inUnderline: boolean, 
        inChangeColor : boolean, inColor : IRGB) {
            this.bold = inBold;
            this.italic = inItalic;
            this.underline = inUnderline;
            this.changeColor = inChangeColor;
            this.color = inColor;
    }
}