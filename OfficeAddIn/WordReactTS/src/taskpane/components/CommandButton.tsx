import * as React from "react";
import { IButtonStyles, IconButton, IIconProps, ImageFit} from "@fluentui/react";

export interface CommandButtonProps {
    butTitle : string;
    iconSrc: string;
    // il faudra ajouter l'action...
}

const iconSize = 40;

const customIconButStyles: IButtonStyles = { 
  root: {height: iconSize + 5, width: iconSize + 5, border: "solid", borderWidth: 1, borderColor: "#A19F9D"},
  icon: {height: iconSize}
};

export default function CommandButton (props: CommandButtonProps) {

    const phonIcon: IIconProps = {
        imageProps: {
            imageFit: ImageFit.centerContain,
            width: iconSize,
            height: iconSize,
            src: props.iconSrc
        }
    };
    return(
        <IconButton
          iconProps={phonIcon}
          title={props.butTitle}
          styles= {customIconButStyles}
        />
    )

}
  