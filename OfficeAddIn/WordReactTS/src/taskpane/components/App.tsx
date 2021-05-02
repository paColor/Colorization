import * as React from "react";
import { DefaultButton, DefaultPalette, IButtonStyles, IStackItemStyles, IStackStyles, IStackTokens, Stack, Text } from "office-ui-fabric-react";
import CommandButton from "./CommandButton";
import PhonControl from "./PhonControl";

export interface AppProps {
  title: string;
  isOfficeInitialized: boolean;
}

const stackTokens: IStackTokens = { 
  childrenGap: 3,
  padding: 2,
 };

const stackStyles: IStackStyles = {
  root: {
    // background: DefaultPalette.themeTertiary,
    overflow: 'auto',
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

export default function App() {
  return (

    <div>
      {/* Première ligne */}
      <Stack horizontal styles={stackStyles} tokens={stackTokens}>
        <Stack.Item align="start" styles={flStackItemStyles}>
          <CommandButton
            butTitle="Cololriser les phonèmes"
            iconSrc="../assets/phon-carré 52.png" />
        </Stack.Item>

        <Stack.Item align="center" grow styles={flStackItemStyles}>
          <div>
            <Text block variant='medium'>Cocher les phonèmes</Text>
            <Text block variant='medium'>à mettre en évidence</Text>
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

      <PhonControl phonTxt="[a]" butTxt="ta, plat"/>
        
    </div>

      

  )
}

