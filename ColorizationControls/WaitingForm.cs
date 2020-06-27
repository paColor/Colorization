using ColorLib;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ColorizationControls
{
    public partial class WaitingForm : Form
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // Durée maximale de l'attente prévue, en millisecondes, pendant laquelle il n'est pas
        // nécessaire d'afficher la fenêtre. Au delà, on affiche.
        private const long AffordableRemainingTime = 1200;
        private static WaitingForm wf;

        public static void Init()
        {
            
            logger.ConditionalDebug("Init");
            wf = new WaitingForm();
            
            wf.Enabled = false;
            ProgressNotifier.thePN.ProgressEvent += HandleProgressEvent;
            ProgressNotifier.thePN.CompletedEvent += HandleCompleteEvent;
        }

        private static void HandleProgressEvent(object sender, ProgressEventArgs e)
        {
            logger.ConditionalTrace("HandleProgressEvent e: {0}", e.ToString());
            wf.progressBar1.Value = e.progress;
            if (!wf.Visible && e.remainingMilliseconds > AffordableRemainingTime)
            {
                wf.Visible = true;
                wf.Enabled = true;
            }
        }

        private static void HandleCompleteEvent(object sender, CompletedEventArgs e)
        {
            logger.ConditionalTrace("HandleCompleteEvent e: {0}", e.ToString());
            wf.Visible = false;
            wf.Enabled = false;
        }


        public WaitingForm()
        {
            logger.ConditionalDebug("WaitingForm");
            InitializeComponent();

            // il y a un comportement que je ne comprends pas sur un ordi avec un petit écran. Peut-être
            // lié à l'utilisation de la barre de progression... 
            // J'en suis réduit à court-circuiter le scaling automatique... :-(
            // Il y a peut-être un des paramètres de la fenêtre (Form) que j'ai mal défini. (????)

            // Compute ScaleFacor
            const int OrigHeight = 131;
            double dimWidth;
            if (AutoScaleMode == AutoScaleMode.Dpi)
                dimWidth = 96; // value observed on the development machine
            else if (AutoScaleMode == AutoScaleMode.Font)
                dimWidth = 6; // value observed on the development machine
            else
            {
                dimWidth = AutoScaleDimensions.Width;
                logger.Warn("Unexpected AutoScaleMode encountered. Scaling may not work properly.");
            }
            double scaleFactor = CurrentAutoScaleDimensions.Width / dimWidth;
            Height = ((int)(OrigHeight * scaleFactor)) + 2;
        }
    }
}
