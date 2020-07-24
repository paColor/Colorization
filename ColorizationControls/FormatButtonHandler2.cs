using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using ColorLib;

namespace ColorizationControls
{
    /// <summary>
    /// Manages format buttons lige bold, italic or underline in the FormatForms. Ensures a behaviour equivalent to the one
    /// found in the office applications for these buttons.
    /// </summary>
    /// <remarks>
    /// A FormatButton is in fact composed of four PictureBoxes that are piled at the same place: <c>SetPict</c>, <c>UnsetPict</c>, 
    /// <c>SetPict</c> and <c>SetOverPict</c>. Depending on mouse events one or the other picture is made visible.
    /// </remarks>
    public class FormatButtonHandler2
    {
        private enum State {unset, unsetOver, unsetPressed, setOver, set, setPressed}
        private enum Trigger { enter, leave, down, up}
        private enum Pict { unsetPict = 0, setPict = 1, pressedPict = 2, setOverPict = 3, nrPict = 4 }
        private enum Intent { set, unset, no}

        private struct Transition
        {
            public Transition(State inTargState, bool inIsErr, Intent inAct)
            {
                targetState = inTargState;
                isError = inIsErr;
                action = inAct;
            }

            public State targetState { get; private set; }
            public bool isError { get; private set; } // indicates that the transition should logically not happen
            public Intent action { get; private set; }
        }

        private static Transition[,] stateMachine = new Transition[,]
        {    /*                      enter                                              leave                                          down                                                  up                       */ 
/*unset       */ { new Transition(State.unsetOver,    false, Intent.no), new Transition(State.unset, true,  Intent.no), new Transition(State.unsetPressed, true,  Intent.no), new Transition(State.setOver,   true,  Intent.set) },
/*unsetOver   */ { new Transition(State.unsetOver,    true,  Intent.no), new Transition(State.unset, false, Intent.no), new Transition(State.unsetPressed, false, Intent.no), new Transition(State.setOver,   true,  Intent.set) },
/*unsetPressed*/ { new Transition(State.unsetPressed, true,  Intent.no), new Transition(State.unset, false, Intent.no), new Transition(State.unsetPressed, true,  Intent.no), new Transition(State.setOver,   false, Intent.set) },
/*setOver     */ { new Transition(State.setOver,      true,  Intent.no), new Transition(State.set,   false, Intent.no), new Transition(State.setPressed,   false, Intent.no), new Transition(State.unsetOver, true,  Intent.unset) },
/*set         */ { new Transition(State.setOver,      false, Intent.no), new Transition(State.set,   true,  Intent.no), new Transition(State.setPressed,   true,  Intent.no), new Transition(State.unsetOver, true,  Intent.unset) },
/*setPressed  */ { new Transition(State.setPressed,   true,  Intent.no), new Transition(State.set,   false, Intent.no), new Transition(State.setPressed,   true,  Intent.no), new Transition(State.unsetOver, false, Intent.unset) }
        };

        private static Pict[] pictForState = new Pict[]
        {
            /*unset       */ Pict.unsetPict,
            /*unsetOver   */ Pict.setPict,
            /*unsetPressed*/ Pict.pressedPict,
            /*setOver     */ Pict.setOverPict,
            /*set         */ Pict.setPict,
            /*setPressed  */ Pict.pressedPict
        };

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private State state;
        private PictureBox theBox;
        private Image[] picts;
        private Pict activePict;
        private Action setFormat;
        private Action unsetFormat;

        /// <summary>
        /// Constructor of a FormatButton. The different <c>PictureBox</c>es must be at exactly the same possition. Their visibility is 
        /// set according to the start status.
        /// </summary>
        /// <param name="inUnsetPict">The <c>PictureBox</c> that shows unset status.</param>
        /// <param name="inSetPict">The <c>PictureBox</c> that shows set status.</param>
        /// <param name="inPressedPict">The <c>PictureBox</c> that shows the status when the button is pressed.</param>
        /// <param name="inSetOverPict">The <c>PictureBox</c> that shows the status when the mouse is over the set button. This is typically a 
        /// supplementary visible frame around the button</param>
        /// <param name="inSetAct">The method that must be called when the button is set</param>
        /// <param name="inUnsetAct">The method that must be called when the button is unset</param>
        /// <param name="startState"><c>true</c> means set, <c>false</c> means unset</param>
        public FormatButtonHandler2(PictureBox inPictBox,
                                    Image inUnsetPict,
                                    Image inSetPict,
                                    Image inPressedPict,
                                    Image inSetOverPict,
                                    Action inSetAct,
                                    Action inUnsetAct,
                                    bool startState )
        {
            logger.ConditionalDebug("Contructor FormatButtonHandler");
            picts = new Image[(int)Pict.nrPict];
            theBox = inPictBox;
            picts[(int)Pict.pressedPict] = inPressedPict;
            picts[(int)Pict.setOverPict] = inSetOverPict;
            picts[(int)Pict.setPict]     = inSetPict;
            picts[(int)Pict.unsetPict]   = inUnsetPict;
            setFormat = inSetAct;
            unsetFormat = inUnsetAct;
            activePict = Pict.nrPict; // no picture displayed yet

            if (startState)
                state = State.set;
            else
                state = State.unset;
            ActivatePict(state);

            theBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbx_MouseDown);
            theBox.MouseEnter += new System.EventHandler(this.pbx_MouseEnter);
            theBox.MouseLeave += new System.EventHandler(this.pbx_MouseLeave);
            theBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbx_MouseUp);
        }

        private void ActivatePict(State s)
        {
            logger.ConditionalDebug("ActivatePict for state {0}", s.ToString());
            Pict p = pictForState[(int)s];
            if (p != activePict)
            {
                theBox.Image = picts[(int)p];
                activePict = p;
            }
            logger.ConditionalDebug("EXIT ActivatePict. Picture {0} is active", activePict.ToString()) ;
        }

        private void HandleEvent(Trigger t)
        {
            logger.ConditionalDebug("HandleEvent {0} in state {1}", t.ToString(), state.ToString());
            Transition transition = stateMachine[(int)state, (int)t];
            state = transition.targetState;
            ActivatePict(state);
            if (transition.isError)
            {
                logger.Warn("State {0} with Trigger {1} combination unexpected", state.ToString(), t.ToString());
            }
            switch (transition.action)
            {
                case Intent.set:
                    setFormat();
                    break;
                case Intent.unset:
                    unsetFormat();
                    break;
                default:
                    break;
            }
            logger.ConditionalDebug("EXIT HandleEvent in state {0}", state.ToString());
        }   

        private void pbx_MouseEnter(object sender, EventArgs e) => HandleEvent(Trigger.enter);

        private void pbx_MouseLeave(object sender, EventArgs e) => HandleEvent(Trigger.leave);

        private void pbx_MouseDown(object sender, MouseEventArgs e) => HandleEvent(Trigger.down);

        private void pbx_MouseUp(object sender, MouseEventArgs e) => HandleEvent(Trigger.up);

    }
}
