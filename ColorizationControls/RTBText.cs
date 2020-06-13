using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorLib;

namespace ColorizationControls
{
    /// <summary>
    /// Classe descendant de <see cref="TheText"/> pour gérer le formatage du texte dans une
    /// <c>RichTextBox</c>
    /// </summary>
    class RTBText : TheText
    {
        // ****************************************************************************************
        // *                               private static members                                 *
        // ****************************************************************************************

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static List<int> finDeLignesStatic;

        // ****************************************************************************************
        // *                                    private members                                   *
        // ****************************************************************************************

        /// <summary>
        /// La <see cref="RichTextBox"/> sur laquelle est construit le <c>RTBText</c>.
        /// </summary>
        private RichTextBox rtb;

        /// <summary>
        /// Liste des positions de fin de ligne (dans S) à retourner dans <see cref="GetLastLinesPos"/>.
        /// </summary>
        private List<int> finDeLignes;

        // ****************************************************************************************
        // *                                     public methods                                   *
        // ****************************************************************************************

        public RTBText(RichTextBox theRTB)
            :base(theRTB.Text)
        {
            logger.ConditionalDebug("RTBText");
            rtb = theRTB;
            logger.ConditionalTrace(rtb.Text);
            finDeLignes = new List<int>();
            if (theRTB.Multiline)
            {
                int lineStart = 0;
                string[] theLines = theRTB.Lines;
                for (int i = 0; i < theLines.Length; i++)
                {
                    int eol = lineStart + theLines[i].Length - 1;
                    finDeLignes.Add(eol);
                    lineStart = eol + 2;
                }
            }
#if DEBUG
            for (int i = 0; i < finDeLignes.Count; i++)
                logger.ConditionalTrace("finDeLignes[{0}] == {1}", i, finDeLignes[i]);
#endif
        }

        // ****************************************************************************************
        // *                                   protected methods                                  *
        // ****************************************************************************************

        protected override List<int> GetLastLinesPos()
        {
            logger.ConditionalDebug("GetLastLinesPos");
            return finDeLignes;
        }

        protected override void SetChars(FormattedTextEl fte, Config conf)
        {
            logger.ConditionalTrace("SetChars");
            rtb.Select(fte.First, fte.Last - fte.First + 1);
            ApplyCFToSelection(fte.cf, conf);
        }

        // ****************************************************************************************
        // *                                     private methods                                  *
        // ****************************************************************************************

        private void ApplyCFToSelection(CharFormatting cf, Config inConf)
        {
            Font currentFont = rtb.SelectionFont;
            bool bold, italic, underline;
            if (cf.bold)
                bold = true;
            else if (cf.ForceNonBold(inConf))
                bold = false;
            else
                bold = currentFont.Bold;

            if (cf.italic)
                italic = true;
            else if (cf.ForceNonItalic(inConf))
                italic = false;
            else
                italic = currentFont.Italic;

            if (cf.underline)
                underline = true;
            else if (cf.ForceNonUnderline(inConf))
                underline = false;
            else
                underline = currentFont.Underline;

            rtb.SelectionFont = new Font(currentFont.FontFamily, currentFont.Size, 
                StaticColorizControls.GetFontStyle(bold, italic, underline));

            if (cf.changeColor) // set new color
                rtb.SelectionColor = cf.color;
            else if (cf.ForceBlackColor(inConf))
                rtb.SelectionColor = ColConfWin.predefinedColors[(int)PredefCols.black];
        }
    }
}
