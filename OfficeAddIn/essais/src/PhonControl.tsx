import { Stack, Checkbox, DefaultButton, IStackStyles, IStackTokens, IButtonStyles, ICheckboxStyles } from "@fluentui/react";
import * as React from "react";

export interface PhonControlProps {
    phon: string;
    phonTxt: string; // le texte après la checkbox
    butTxt: string; // le texte dans le bouton
    chk:boolean | undefined; //la valeur de la checkbox
    chkOnChange: any;
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
                    <DefaultButton text={props.butTxt} styles={phonButStyles}/>
                </Stack.Item>
            </Stack>
        </div>
    )
}