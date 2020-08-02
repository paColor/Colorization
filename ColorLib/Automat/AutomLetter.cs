/********************************************************************************
 *  Copyright 2020, Pierre-Alain Etique                                         *
 *                                                                              *
 *  This file is part of Coloriƨation.                                          *
 *                                                                              *
 *  Coloriƨation is free software: you can redistribute it and/or modify        *
 *  it under the terms of the GNU General Public License as published by        *
 *  the Free Software Foundation, either version 3 of the License, or           *
 *  (at your option) any later version.                                         *
 *                                                                              *
 *  Coloriƨation is distributed in the hope that it will be useful,             *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of              *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the               *
 *  GNU General Public License for more details.                                *
 *                                                                              *
 *  You should have received a copy of the GNU General Public License           *
 *  along with Coloriƨation.  If not, see <https://www.gnu.org/licenses/>.      *
 *                                                                              *
 ********************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Globalization;

namespace ColorLib
{
    public class AutomLetter : AutomElement
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void InitAutomat()
        {
            logger.ConditionalDebug("InitAutomat");
            AutomRule.InitAutomat();
        }

        public char Letter { get; private set; }
        private List<string> ruleOrder;
        private Dictionary<string, AutomRule> rules;

        protected AutomLetter()
            :base(" ", 0)
        {
            logger.ConditionalTrace("AutomLetter");
            Letter = '$';
            ruleOrder = null;
            rules = null;
        }

        public AutomLetter(string s, ref int pos)
            : base(s, pos)
        {
            logger.ConditionalTrace("AutomLetter(string s, ref int pos)");
            /*
             * An AutomLetter has the Syntax:
             * 'letter' : [[list of rulenames], {List of AutomRules}]
             */

            // Let's skip possible leading spaces
            pos = GetNextChar(pos);
            // The char at pos must be an '
            Debug.Assert(s[pos] == '\'', String.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un \''\' en début de lettre.", 
                pos - start, s.Substring(start, (pos+1) - start)));
            // Let's find the letter
            pos = GetNextChar(pos + 1);
            Letter = s[pos];
            // Let's find the closing '
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == '\'', string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un \''\' en fin de lettre.",
                pos - start, s.Substring(start, (pos + 1) - start)));
            // Let's find the :
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == ':', string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un : après la lettre",
                pos - start, s.Substring(start, (pos + 1) - start)));
            // let's find the first [
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == '[', string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un [ après le :",
                pos - start, s.Substring(start, (pos + 1) - start)));
            // let's find the second [
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == '[', string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un [ après le premier [",
                pos - start, s.Substring(start, (pos + 1) - start)));

            //Let's load the list of rule names
            pos = GetNextChar(pos + 1);
            ruleOrder = new List<string>();
            while (s[pos] != ']')
            {
                // The char at pos must be an '
                Debug.Assert(s[pos] == '\'', String.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un \''\' en début de nom de règle.",
                    pos - start, s.Substring(start, (pos + 1) - start)));
                // the rulename must end with a '
                pos = GetNextChar(pos + 1);
                int endOfRuleNameApostrophyPos = s.IndexOf('\'', pos);
                Debug.Assert(endOfRuleNameApostrophyPos > pos, String.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un \''\' en fin de nom de règle.",
                    pos - start, s.Substring(start, (pos + 1) - start)));
                string theRuleName = s.Substring(pos, endOfRuleNameApostrophyPos - pos);
                ruleOrder.Add(theRuleName);
                pos = GetNextChar(endOfRuleNameApostrophyPos + 1);
                // it is either ',' or ']'
                Debug.Assert(((s[pos] == ',') || (s[pos] == ']')), string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend une \',\' ou un \']\'.",
                    pos - start, s.Substring(start, (pos + 1) - start)));
                if (s[pos] == ',')
                    pos = GetNextChar(pos + 1);
            } // while

            // Let's find the ,
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == ',', string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend une \',\' avant les règles",
                pos - start, s.Substring(start, (pos + 1) - start)));

            // Let's load the rules
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == '{', string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend une \'{{\' avant la liste de règles",
                pos - start, s.Substring(start, (pos + 1) - start)));
            List<string> sortedRuleNames = new List<string>(ruleOrder);
            sortedRuleNames.Sort();
            // Find the first character of the rule
            pos = GetNextChar(pos + 1);
            rules = new Dictionary<string, AutomRule>(ruleOrder.Count);
            while (s[pos] != '}') 
            {
                AutomRule ar = new AutomRule(s, ref pos, sortedRuleNames);
                rules.Add(ar.RuleName, ar);
                pos = GetNextChar(pos + 1);
                // it is either ',' or '}'
                Debug.Assert(((s[pos] == ',') || (s[pos] == '}')), string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend une \',\' ou un \'}}\' après une règle.",
                    pos - start, s.Substring(start, (pos + 1) - start)));
                if (s[pos] == ',')
                    pos = GetNextChar(pos + 1);
            } // while

            // Let's Find the closing ]
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == ']', string.Format(ConfigBase.cultF, "La pos {0} de {1} n'est pas un AutomLetter, on attend un \']\' après la liste des règles",
                pos - start, s.Substring(start, (pos + 1) - start)));

            end = pos;

        } // Constructor

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Original Text: " + base.ToString());
            sb.AppendLine(String.Format(ConfigBase.cultF, "letter: {0}", Letter));
            sb.AppendLine("ruleOrder:");
            foreach (string rS in ruleOrder)
                sb.AppendLine("  " + rS);
            sb.AppendLine("rules:");
            foreach (KeyValuePair<string, AutomRule> kvp in rules) 
                sb.AppendLine(kvp.Value.ToString());
            return sb.ToString();
        }

        /// <summary>
        /// Fires the appropriate rule for the letter in <paramref name="pw"/> at position
        /// <paramref name="pos"/>. The <see cref="PhonWord"/> <c>pw</c> is expanded with the corresponding
        /// 'phonème'.
        /// </summary>
        /// <param name="pw">The <see cref="PhonWord"/> that must be analysed and where phonèmes must be identified.</param>
        /// <param name="pos">The position in <c>pw</c> (zero based) of the letter to analyse.</param>
        /// <param name="conf">The <see cref="Config"/> to use when deciding how the rules should be applied.</param>
        public void FireRule(PhonWord pw, ref int pos, Config conf)
            // Fires the appropriate rule for the letter in pw at position pos
        {
            logger.ConditionalTrace("FireRule");
            Debug.Assert(pw != null);
            bool found = false;
            int i = 0;
            string firstPart, secondPart;
            string w = pw.GetWord();
            Debug.Assert((w[pos] == Letter) || (Letter == '*'));
            firstPart = w.Substring(0, pos); // string before the letter under examination. if pos is the first letter in the string then ""
            if (pos == w.Length - 1)
                secondPart = "";
            else
                secondPart = w.Substring(pos + 1, w.Length - (pos + 1)); // string after the letter under examination. if pos is the last letter in the string then ""

            while ((!found) && (i < ruleOrder.Count))
            {
                AutomRule ar;
                if (rules.TryGetValue(ruleOrder[i], out ar)) 
                {
                    found = ar.TryApplyRule(pw, ref pos, firstPart, secondPart, conf);
                }
                else
                {
                    string message = String.Format(ConfigBase.cultF, "La règle \"{0}\" n'existe pas", ruleOrder[i]);
                    throw new KeyNotFoundException(message);
                }
                i++;
            }
            Debug.Assert (found, String.Format(ConfigBase.cultF, "Pas de règle pour pos {0} dans \"{1}\" ", pos, pw.GetWord()));
        } // public void FireRule

        public void CountPhons(ref int[] usedPhons)
        {
            foreach (KeyValuePair<string, AutomRule> k in rules)
                k.Value.CountPhons(ref usedPhons);
        }
    } // class AutomLetter
} // namespace
