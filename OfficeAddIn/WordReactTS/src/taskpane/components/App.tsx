import * as React from "react";
import Config from "../Configs/Config";
import PhonTab from "./PhonTab";

export interface AppProps {
  title: string;
  isOfficeInitialized: boolean;
}

export default function App() {
  const conf : Config = new Config();

  return (
    <div>
      <PhonTab 
        pc= {conf.pc}
      />
    </div>
  )
}

