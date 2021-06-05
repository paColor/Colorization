import { Stack, Checkbox, DefaultButton, IStackStyles, IStackTokens, IButtonStyles, ICheckboxStyles, getColorFromRGBA } from "@fluentui/react";
import * as React from "react";
import CharFormatting from "../Configs/CharFormatting";

export interface PhonControlProps {
    // Le phonème / son
    phon: string;

    // le texte affiché à côté de la checkbox
    phonTxt: string;

    // le texte dans le bouton
    butTxt: string; 

    // Le formattage
    cf: CharFormatting;

    //la valeur de la checkbox
    chk:boolean; 

    // la fonction à appeler quand la checkbox est cliquée
    chkOnChange: (phon: string, valeurChkBox: boolean) => void;

    // la fonction à appeler quand le bouton est cliqué.
    clickBut: (phon: string) => void;
}

const stackStyles: IStackStyles = {
    root: {
        overflow: 'hidden',
    },
};

const stackTokens: IStackTokens = { 
    childrenGap: 5,
    padding: 2,
};
  
const phonCBStyles: ICheckboxStyles ={
    root: {
       width: 32,
       minWidth: 10,
       maxWidth:200,
    },

    checkbox: {
        width: 12,
        height: 12,
        marginTop: 4,
        marginRight:0,
    },
    text: {
        fontSize: 10.5,
    },
}


export default function PhonControl(props: PhonControlProps) {

    function onChecked(_ev?: React.FormEvent<HTMLElement | HTMLInputElement>, checked?: boolean) {
        props.chkOnChange(props.phon, checked);
    }

    function onClicked() {
        props.clickBut(props.phon);
    }

    
    let col: string = "#FFFFFF"; // blanc
    let fontCol: string = "#000000"; // noir

    if (props.chk && props.cf.changeColor) {
        let iCol = getColorFromRGBA(props.cf.color);
        col = iCol.str;
        if (((0.9 * iCol.r) + (1.5 * iCol.g) + (0.5 * iCol.b)) < 380) {
            // foncé
            fontCol = "#FFFFFF"; // blanc
        }
    }

    const phonButStyles: IButtonStyles = { 
        root: {
          width: 53,
          height: 20, 
          padding: 0,
          margin: 0,
          minWidth: 10,
          flexWrap: 'nowrap',
          background: col,
        },
        label: {
          fontSize: 11,
          fontWeight: props.chk && props.cf.bold?"800":"400",
          fontStyle: props.chk && props.cf.italic?"italic":"normal",
          textDecoration: props.chk && props.cf.underline?"underline":"",
          padding: 0,
          margin: 0,
          flexWrap: 'nowrap',
          color: fontCol,
        },
      };

    return (
        <div>
            <Stack horizontal styles={stackStyles} tokens={stackTokens}>
                <Stack.Item>
                    <Checkbox 
                        label={props.phonTxt} 
                        styles={phonCBStyles}
                        checked={props.chk}
                        onChange={onChecked}
                        />
                </Stack.Item>
                <Stack.Item>
                    <DefaultButton 
                        text={props.butTxt} 
                        styles={phonButStyles}
                        onClick={onClicked}
                    />
                </Stack.Item>
            </Stack>
        </div>
    )
}