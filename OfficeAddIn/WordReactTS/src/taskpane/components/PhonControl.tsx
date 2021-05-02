import { Stack, Checkbox, DefaultButton, IStackStyles, IStackTokens, IButtonStyles, ICheckboxStyles } from "office-ui-fabric-react";
import * as React from "react";

export interface PhonControlProps {
    phonTxt: string; // le texte après la checkbox
    butTxt: string; // le texte dans le bouton
}

const stackStyles: IStackStyles = {
    root: {
        overflow: 'auto',
    },
};

const stackTokens: IStackTokens = { 
    childrenGap: 5,
    padding: 2,
};

const phonButStyles: IButtonStyles = { 
    root: {
      width: 50,
      height: 20, 
      padding: 0,
      margin: 0,
      minWidth: 10,
    },
    label: {
      fontSize: 11,
    },
  };
  
const phonCBStyles: ICheckboxStyles ={
    // root: {
    //   width: 80,
    //   height: 80,
    //   minWidth: 10,
    //   maxWidth:200,
    //   minHeight:10,
    //   maxHeight:200,
    // },

    checkbox: {
        width: 12,
        height: 12,
        marginTop: 4,
        marginRight:0,
    }
}

export default function PhonControl(props:PhonControlProps) {
    return (
        <div>
            <Stack horizontal styles={stackStyles} tokens={stackTokens}>
                <Stack.Item>
                    <Checkbox label={props.phonTxt} styles={phonCBStyles}/>
                </Stack.Item>
                <Stack.Item>
                    <DefaultButton text={props.butTxt} styles={phonButStyles}/>
                </Stack.Item>
            </Stack>
        </div>
    )
}