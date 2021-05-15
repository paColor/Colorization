import { useId } from '@fluentui/react-hooks';
import { FontWeights, getTheme, IconButton, mergeStyleSets, Modal, IIconProps, getColorFromString, IColor, ColorPicker, IColorPickerStyles, ImageFit, DefaultButton, } from "@fluentui/react";
import * as React from "react";

export interface CharFormatFormProps {
    
    // indique si le dialogue doit être affiché (true) ou caché (false)
    visible : boolean;

    // le phonème / son pour lequel on ouvre le dialogue
    phon: string;

    // fonction appelée quand OK ou "Valider" est cliqué.
    valid: any;

    // fonction appelée quand le dialogue est annulé   
    cancel: any; 
        
}

const btnSize = 15;

const boldIcon: IIconProps = {
  imageProps: {
      imageFit: ImageFit.centerContain,
      width: btnSize,
      height: btnSize,
      src: "../assets/phon-carré 52.png"
  }
};

const cancelIcon: IIconProps = { iconName: 'Cancel' };

export default function CharFormatForm(props:CharFormatFormProps) {

    const white = getColorFromString('#ffffff')!;
    const [color, setColor] = React.useState(white);
    const updateColor = React.useCallback((_ev: any, colorObj: IColor) => setColor(colorObj), []);

    const titleId = useId('title');
    
    return(
        <Modal
            isOpen= {props.visible}
            onDismiss= {props.cancel}
            isModeless= {false}
            containerClassName={contentStyles.container}
            // dragOptions={dragOptions}
            styles={modalStyles}
        >
            <div className={contentStyles.header}>
                <span id={titleId}>Configurer {props.phon}</span>
                <IconButton
                    styles={iconButtonStyles}
                    iconProps={cancelIcon}
                    ariaLabel="Close popup modal"
                    onClick={props.cancel}
                />
            </div>
            <div className={contentStyles.body}>
              <ColorPicker
                color={color}
                onChange={updateColor}
                alphaType={"none"}
                showPreview={true}
                styles={colorPickerStyles}
                // The ColorPicker provides default English strings for visible text.
                // If your app is localized, you MUST provide the `strings` prop with localized strings.
                strings={{
                  red: "Rouge",
                  green: "Vert",
                  blue: "Bleu",
                }}
              />

              <DefaultButton
                toggle
                checked={false}
                iconProps={muted ? volume0Icon : volume3Icon}
                onClick={setMuted}
                allowDisabledFocus
                disabled={disabled}
              />
            </div>
        </Modal>
    )
}

const theme = getTheme();
const contentStyles = mergeStyleSets({
    container: {
      display: 'flex',
      flexFlow: 'column nowrap',
      alignItems: 'stretch',
    },
    header: [
      theme.fonts.mediumPlus,
      {
        flex: '1 1 auto',
        borderTop: `4px solid ${theme.palette.themePrimary}`,
        color: theme.palette.neutralPrimary,
        display: 'flex',
        alignItems: 'center',
        fontWeight: FontWeights.semibold,
        padding: '0px 4px 4px 12px',
      },
    ],
    body: {
      padding: '0 10px',
      overflowY: 'hidden',
    },
});

const iconButtonStyles = {
    root: {
      color: theme.palette.neutralPrimary,
      marginLeft: 'auto',
      marginTop: '4px',
      marginRight: '2px',
    },
    rootHovered: {
      color: theme.palette.neutralDark,
    },
};

const modalStyles = {
    main: {
        minWidth: 150,
        maxWidth: 350,
        width: 300,
        minHeight: 100,
        height: 500,
    }
}

const colorPickerStyles: Partial<IColorPickerStyles> = {
  panel: { 
    padding: 12,
  },
  root: {
    maxWidth: 280,
    minWidth: 280,
  },
  colorRectangle: { height: 100 },
};

