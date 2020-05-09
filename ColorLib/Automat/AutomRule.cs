﻿/********************************************************************************
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

namespace ColorLib
{
    /**************************************************************************************************
     *                                                REMINDER                                        
     * Syntax of a rule:
     * 
     * regexRule =         charWithoutSlash, {charWithoutSlash}; 
	 *						 (* on se simplifie la vie en définissant une règle regex comme une suite 
     *						    de caractères sans "/" *)
     * regex =             "/", regexRule, "/"
	 * ruleFilterComp =    ((quoteMark, "+", quoteMark) | (quoteMark, "-", quoteMark)), ":", regex, "i";
	 * ruleFilter =        ("{", ruleFilterComp, [",", ruleFilterComp], "}") | identifier;
	 * rule =              name, ":", "[", ruleFilter, ",", name, ",", digit, [",", name], "]" ;
     * 
     * '+' means after the current letter in the word.
     * '-' means before the current letter in the word
     * the three names on the rule line mean
     *   1) name of the rule
     *   2) name of the phonème that is applied by the rule if the rulefilter is evaluated to true
     *   3) name of the flag that must be checked before the rule is fired.
     *   
     *  digit on the rule line is the number of letters for the considered phonème.
     *
     * ************************************************************************************************/
    public class AutomRule : AutomElement
    {
        public string RuleName { get; }
        private AutomRuleFilter rf;
        private Phonemes p;
        private int incr;
        private AutomFlagsConfig.RuleFlag flag;

        /************************       CONSTRUCTOR          ******************************************
         * Create an AutomRule Object
         * s:               the string containing the specification of the automata
         * pos:             on entry, the position of the first character of the Rule (should be ')
         *                  on exit the position of the last character of the Rule (should be ']')
         * SortedRuleNames: The valid RuleNames in a sorted array where BinarySearch can be executed.
         * 
         * if s is not a valid Rule specification, an ArgumentException is thrown.
         * The RuleName can be read in the correpsonding Property.
         ***********************************************************************************************/
        public AutomRule(string s, ref int pos, List<string> SortedRuleNames)
            : base(s, pos)
        {
            /* A Rule has the Syntax
             * 'ruleName' : [{list of DirectionRegex separated by ','}, 'phoneme', pas]
             * where 
             *    regexRule is '+|-':/regEx/i
             *    phoneme is a string
             *    pas is an integer
             *  
             *    ruleName has to be part of ruleNames
             */

            // Let's parse the ruleName
            Debug.Assert(s[pos] == '\'', String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, il ne commence pas par \'.",
                pos - start, s.Substring(start, (pos + 1) - start)));
            pos = GetNextChar(pos + 1);
            var endOfRuleNameApostrophyPos = s.IndexOf('\'', pos);
            Debug.Assert(endOfRuleNameApostrophyPos > pos, String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, le nom de la règle ne se termine pas par \'.",
                pos - start, s.Substring(start, (pos + 1) - start)));
            RuleName = s.Substring(pos, endOfRuleNameApostrophyPos - pos).Trim();
            // check that ruleName is in ruleNames
            Debug.Assert(SortedRuleNames.BinarySearch(RuleName) >= 0, String.Format(BaseConfig.cultF, "AutomRule: {0} ne se trouve pas dans la liste des noms de règles.", RuleName));
            pos = endOfRuleNameApostrophyPos;

            // Let's find the colon
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == ':', String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, on attend un \':\'",
                pos - start, s.Substring(start, (pos + 1) - start)));

            // let's find the [
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == '[', String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, on attend un \'[\'",
                pos - start, s.Substring(start, (pos + 1) - start)));

            // let's find the RuleFilter
            pos = GetNextChar(pos + 1);
            rf = new AutomRuleFilter(s, ref pos);

            // let's find the comma
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == ',', String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, on attend une \',\' avant le phonème.",
                pos - start, s.Substring(start, (pos + 1) - start)));

            // let's find the phoneme
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == '\'', String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, on attend \' au début du phonème.",
                pos - start, s.Substring(start, (pos + 1) - start)));

            // let's find the phoneme name
            pos = GetNextChar(pos + 1);
            var endOfPhonemeApostrophyPos = s.IndexOf('\'', pos);
            Debug.Assert(endOfPhonemeApostrophyPos > pos, String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, le nom du phonème ne se termine pas par \'.",
                pos - start, s.Substring(start, (pos + 1) - start)));
            var phonemeName = s.Substring(pos, endOfPhonemeApostrophyPos - pos).Trim();
            p = (Phonemes)Enum.Parse(typeof(Phonemes), phonemeName);
            pos = endOfPhonemeApostrophyPos;

            // let's find the comma
            pos = GetNextChar(pos + 1);
            Debug.Assert(s[pos] == ',', String.Format(BaseConfig.cultF, "La pos {0} de {1} n'est pas un AutomRule, on attend une \',\' avant le pas.",
                pos - start, s.Substring(start, (pos + 1) - start)));

            // let's find the increment i.e. le pas
            pos = GetNextChar(pos + 1);
            var endOfNumberPos = s.IndexOfAny("],".ToCharArray(), pos);
            Debug.Assert(endOfNumberPos > pos, String.Format(BaseConfig.cultF,
                "La pos {0} de {1} n'est pas un AutomRule, on attend \']\' ou \',\' après le pas.",
                pos - start, s.Substring(start, (pos + 1) - start)));
            var theIntString = s.Substring(pos, endOfNumberPos - pos).Trim();
            incr = int.Parse(theIntString, BaseConfig.cultF);
            pos = endOfNumberPos;

            // if we are on a coma, there is an identifier to load.
            if (s[pos] == ',')
            {
                pos = GetNextChar(pos + 1);
                var endOfFlagName = s.IndexOf(']', pos);
                Debug.Assert(endOfFlagName > pos, String.Format(BaseConfig.cultF,
                    "La pos {0} de {1} n'est pas un AutomRule, on attend \']\' après le nom de flag.",
                    pos - start, s.Substring(start, (pos + 1) - start)));
                var flagName = s.Substring(pos, endOfFlagName - pos).Trim();
                flag = (AutomFlagsConfig.RuleFlag)Enum.Parse(typeof(AutomFlagsConfig.RuleFlag), flagName);
                pos = endOfFlagName;
            }
            else
            {
                flag = AutomFlagsConfig.RuleFlag.dummy;
            }

            end = pos;

        } // constructor AutomRule

        public static void InitAutomat()
        {
            AutomRuleFilter.InitAutomat();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Original Text: " + base.ToString());
            sb.AppendLine(String.Format(BaseConfig.cultF, "RuleName: {0}", RuleName));
            sb.AppendLine(String.Format(BaseConfig.cultF, "rf: {0}", rf));

            sb.AppendLine(String.Format(BaseConfig.cultF, "p: {0}", p));
            sb.AppendLine(String.Format(BaseConfig.cultF, "incr: {0}", incr));
            return sb.ToString();
        }

        public bool TryApplyRule (PhonWord pw, ref int pos, string firstPart, string secondPart)
        {
            bool found = pw.T.GetConfig().flagConf.GetFlag(flag) && rf.Check(pw, pos, firstPart, secondPart);
            if (found) 
            {
                PhonInW piw = new PhonInW(pw, pos, pos + incr - 1, p, RuleName);
                pw.AddPhon(piw);
                pos = pos + incr;
            }
            return found;
        }

        public void CountPhons(ref int[] usedPhons)
        {
            usedPhons[(int)p]++;
        }

    }  // public class AutomRule
} // namespace
