import { Stack, Checkbox, DefaultButton, IStackStyles, IStackTokens, IButtonStyles, ICheckboxStyles } from "@fluentui/react";
import * as React from "react";

export interface PhonControlProps {
    // Le phonème / son
    phon: string;

    // le texte affiché à côté de la checkbox
    phonTxt: string;

    // le texte dans le bouton
    butTxt: string; 

    //la valeur de la checkbox
    chk:boolean; 

    // la fonction à appeler quand la checkbox est cliquée
    // signature: (phon: string, valeurChkBox: boolean) : void
    // Il faut que j'apprenne comment déclarer le bon type :-)
    chkOnChange: any;

    // la fonction à appeler quand le bouton est cliqué.
    // signature: (phon: string) : void
    clickBut: any;
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

const phonButStyles: IButtonStyles = { 
    root: {
      width: 53,
      height: 20, 
      padding: 0,
      margin: 0,
      minWidth: 10,
      flexWrap: 'nowrap',
    },
    label: {
      fontSize: 11,
      padding: 0,
      margin: 0,
      flexWrap: 'nowrap',
    },

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



export default function PhonControl(props:PhonControlProps) {

    function onChecked(_ev?: React.FormEvent<HTMLElement | HTMLInputElement>, checked?: boolean) {
        props.chkOnChange(props.phon, checked);
    }

    function onClicked() {
        props.clickBut(props.phon);
    }

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