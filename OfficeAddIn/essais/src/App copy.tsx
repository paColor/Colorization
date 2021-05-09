import React, { useState } from 'react';
import PhonControl from './PhonControl';

function App() {
  const [state, setState] = useState(false);

  function SetChk(phon: string, chkBoxVal: boolean) {
    console.log(phon, chkBoxVal)
    setState(chkBoxVal);
  }

  return (
    <div>
      <PhonControl
        phon="é"
        phonTxt="é"
        butTxt="été"
        chk= {state}
        chkOnChange = {SetChk} />
    </div>
  );
}

export default App;
