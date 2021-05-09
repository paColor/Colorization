import { useId } from '@fluentui/react-hooks';
import { ContextualMenu, FontWeights, getTheme, IconButton, IDragOptions, mergeStyleSets, Modal, IIconProps, } from "@fluentui/react";
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

const dragOptions: IDragOptions = {
    moveMenuItemText: 'Move',
    closeMenuItemText: 'Close',
    menu: ContextualMenu,
};

const cancelIcon: IIconProps = { iconName: 'Cancel' };

export default function CharFormatForm(props:CharFormatFormProps) {

    const titleId = useId('title');
    return(
        <Modal
            isOpen= {props.visible}
            onDismiss= {props.cancel}
            isModeless= {false}
            containerClassName={contentStyles.container}
            dragOptions={dragOptions}
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
      flex: '4 4 auto',
      padding: '0 24px 24px 24px',
      overflowY: 'hidden',
      selectors: {
        p: { margin: '14px 0' },
        'p:first-child': { marginTop: 0 },
        'p:last-child': { marginBottom: 0 },
      },
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
        maxWidth: 300,
        width: 200,
        minHeight: 100,
        height: 100,
    }
}

