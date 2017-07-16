﻿/*
 * Copyright © 2017 EDDiscovery development team
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 * 
 * EDDiscovery is not affiliated with Frontier Developments plc.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace Conditions
{
    public class ConditionFunctionsBase : ConditionFunctionHandlers
    {
        public ConditionFunctionsBase(ConditionFunctions c, ConditionVariables v, ConditionFileHandles h, int recd) : base(c, v, h, recd)
        {
            if (functions == null)        // one time init, done like this cause can't do it in {}
            {
                functions = new Dictionary<string, FuncEntry>();

                functions.Add("abs", new FuncEntry(Abs, 2, 2, 1, 0));  // first is var or literal or string
                functions.Add("alt", new FuncEntry(Alt, 2, 20, 0xfffffff, 0xfffffff));  // first is var or literal or string, etc.
                functions.Add("closefile", new FuncEntry(CloseFile, 1, 1, 1, 0));  // first is a var

                functions.Add("datetimenow", new FuncEntry(DateTimeNow, 1, 1, 0));     // literal type
                functions.Add("datehour", new FuncEntry(DateHour, 1, 1, 1, 1));   // first is a var or string
                functions.Add("date", new FuncEntry(Date, 2, 2, 1, 1));   // first is a var or string, second is literal
                functions.Add("direxists", new FuncEntry(DirExists, 1, 20, 0xfffffff, 0xfffffff));   // check var, can be string

                functions.Add("escapechar", new FuncEntry(EscapeChar, 1, 1, 1, 1));   // check var, can be string
                functions.Add("eval", new FuncEntry(Eval, 1, 2, 1, 1));   // can be string, can be variable, p2 is not a variable, and can't be a string
                functions.Add("exist", new FuncEntry(Exists, 1, 20, 0, 0xfffffff));
                functions.Add("existsdefault", new FuncEntry(ExistsDefault, 2, 2, 2, 3));   // first is a macro but can not exist, second is a string or macro which must exist
                functions.Add("expand", new FuncEntry(Expand, 1, 20, 0xfffffff, 0xfffffff)); // check var, can be string (if so expanded)
                functions.Add("expandarray", new FuncEntry(ExpandArray, 4, 5, 2, 3 + 16));  // var 1 is text root/string, not var, not string, var 2 can be var or string, var 3/4 is integers or variables, checked in function
                functions.Add("expandvars", new FuncEntry(ExpandVars, 4, 5, 2, 3 + 16));   // var 1 is text root/string, not var, not string, var 2 can be var or string, var 3/4 is integers or variables, checked in function

                functions.Add("filelength", new FuncEntry(FileLength, 1, 1, 1, 1));   // check var, can be string
                functions.Add("fileexists", new FuncEntry(FileExists, 1, 20, 0xfffffff, 0xfffffff));   // check var, can be string
                functions.Add("findline", new FuncEntry(FindLine, 2, 2, 3, 2));   //check var1 and var2, second can be a string
                functions.Add("floor", new FuncEntry(Floor, 2, 2, 1));     // check var1, not var 2 no strings

                functions.Add("ifnotempty", new FuncEntry(Ifnotempty, 2, 3, 7, 7));   // check var1-3, allow strings var1-3
                functions.Add("ifempty", new FuncEntry(Ifempty, 2, 3, 7, 7));
                functions.Add("iftrue", new FuncEntry(Iftrue, 2, 3, 7, 7));   // check var1-3, allow strings var1-3
                functions.Add("iffalse", new FuncEntry(Iffalse, 2, 3, 7, 7));
                functions.Add("ifzero", new FuncEntry(Ifzero, 2, 3, 7, 7));   // check var1-3, allow strings var1-3
                functions.Add("ifnonzero", new FuncEntry(Ifnonzero, 2, 3, 7, 7));   // check var1-3, allow strings var1-3

                functions.Add("ifcontains", new FuncEntry(Ifcontains, 3, 5, 31, 31)); // check var1-5, allow strings var1-5
                functions.Add("ifnotcontains", new FuncEntry(Ifnotcontains, 3, 5, 31, 31));
                functions.Add("ifequal", new FuncEntry(Ifequal, 3, 5, 31, 31));
                functions.Add("ifnotequal", new FuncEntry(Ifnotequal, 3, 5, 31, 31));

                functions.Add("ifgt", new FuncEntry(Ifnumgreater, 3, 5, 31, 31)); // check var1-5, allow strings var1-5
                functions.Add("iflt", new FuncEntry(Ifnumless, 3, 5, 31, 31));
                functions.Add("ifge", new FuncEntry(Ifnumgreaterequal, 3, 5, 31, 31));
                functions.Add("ifle", new FuncEntry(Ifnumlessequal, 3, 5, 31, 31));
                functions.Add("ifeq", new FuncEntry(Ifnumequal, 3, 5, 31, 31));
                functions.Add("ifne", new FuncEntry(Ifnumnotequal, 3, 5, 31, 31));

                functions.Add("indexof", new FuncEntry(IndexOf, 2, 2, 3, 3));   // check var1 and 2 if normal, allow string in 1 and 2
                functions.Add("indirect", new FuncEntry(Indirect, 1, 20, 0xfffffff, 0xfffffff));   // check var, no strings

                functions.Add("ispresent", new FuncEntry(Ispresent, 2, 3, 2, 2));   // 1 may not be there, 2 either a macro or can be string. 3 is optional and a var or literal

                functions.Add("join", new FuncEntry(Join, 3, 20, 0xfffffff, 0xfffffff));   // all can be string, check var

                functions.Add("length", new FuncEntry(Length, 1, 1, 1, 1));
                functions.Add("lower", new FuncEntry(Lower, 1, 20, 0xfffffff, 0xfffffff));   // all can be string, check var

                functions.Add("mkdir", new FuncEntry(MkDir, 1, 1, 1, 1));   // check var, can be string

                functions.Add("openfile", new FuncEntry(OpenFile, 3, 3, 2, 2));

                functions.Add("phrase", new FuncEntry(Phrase, 1, 1, 1, 1));

                functions.Add("random", new FuncEntry(Random, 1, 1, 0, 0));   // no change var, not string
                functions.Add("readline", new FuncEntry(ReadLineFile, 2, 2, 1, 0));   // first must be a macro, second is a literal varname only
                functions.Add("replace", new FuncEntry(Replace, 3, 3, 7, 7)); // var/string for all
                functions.Add("replaceescapechar", new FuncEntry(ReplaceEscapeChar, 1, 1, 1, 1));   // check var, can be string
                functions.Add("replacevar", new FuncEntry(ReplaceVar, 2, 2, 1, 3)); // var/string, literal/var/string
                functions.Add("round", new FuncEntry(RoundCommon, 3, 3, 1));
                functions.Add("roundnz", new FuncEntry(RoundCommon, 4, 4, 1));
                functions.Add("roundscale", new FuncEntry(RoundCommon, 5, 5, 1));
                functions.Add("rs", new FuncEntry(ReplaceVarSC, 2, 2, 1, 3)); // var/string, literal/var/string
                functions.Add("rv", new FuncEntry(ReplaceVar, 2, 2, 1, 3)); // var/string, literal/var/string

                functions.Add("seek", new FuncEntry(SeekFile, 2, 2, 1, 0));   //first is macro, second is literal or macro

                functions.Add("safevarname", new FuncEntry(SafeVarName, 1, 1, 1, 1));   //macro/string

                functions.Add("sc", new FuncEntry(SplitCaps, 1, 1, 1, 1));   //shorter alias 
                functions.Add("ship", new FuncEntry(Ship, 1, 1, 1, 1));   //ship translator
                functions.Add("splitcaps", new FuncEntry(SplitCaps, 1, 1, 1, 1));   //check var, allow strings
                functions.Add("substring", new FuncEntry(SubString, 3, 3, 1, 1));   // check var1, var1 can be string, var 2 and 3 can either be macro or ints not strings
                functions.Add("systempath", new FuncEntry(SystemPath, 1, 1, 0, 0));   // literal

                functions.Add("tell", new FuncEntry(TellFile, 1, 1, 1, 0));   //first is macro
                functions.Add("tickcount", new FuncEntry(TickCount, 0, 0, 0, 0));   // no paras
                functions.Add("trim", new FuncEntry(Trim, 1, 2, 1, 1));

                functions.Add("upper", new FuncEntry(Upper, 1, 20, 0xfffffff, 0xfffffff));   // all can be string, check var

                functions.Add("wordlistcount", new FuncEntry(WordListCount, 1, 1, 1));       // first is a var or string
                functions.Add("wordlistentry", new FuncEntry(WordListEntry, 2, 2, 1, 1));       // first is a var or string, second is a var or literal
                functions.Add("wordof", new FuncEntry(WordOf, 2, 3, 1 + 4, 1 + 4));   // first is a var or string, second is a var or literal, third is a macro or string
                functions.Add("write", new FuncEntry(WriteFile, 2, 2, 3, 2));      // first must be a var, second can be macro or string
                functions.Add("writeline", new FuncEntry(WriteLineFile, 2, 2, 3, 2));      // first must be a var, second can be macro or string
            }
        }

        static Dictionary<string, FuncEntry> functions = null;

        protected override FuncEntry FindFunction(string name)
        {
            return functions.ContainsKey(name) ? functions[name] : null;
        }

        #region Macro Functions

        protected bool Exists(out string output)
        {
            foreach (Parameter s in paras)
            {
                if (!vars.Exists(s.value))      // either s.value is an expanded string, or a literal.. does not matter.
                {
                    output = "0";
                    return true;
                }
            }

            output = "1";
            return true;
        }

        protected bool Alt(out string output)
        {
            output = "";

            foreach (Parameter s in paras)
            {
                string sv = s.isstring ? s.value : vars[s.value];

                if (sv.Length > 0)
                {
                    output = sv;
                    break;
                }
            }

            return true;
        }

        protected bool ExistsDefault(out string output)
        {
            if (vars.Exists(paras[0].value))        // either s.value is an expanded string, or a literal.. does not matter.
                output = vars[paras[0].value];
            else
                output = paras[1].isstring ? paras[1].value : vars[paras[1].value];

            return true;
        }

        protected bool Expand(out string output)
        {
            return ExpandCore(out output, false);
        }

        protected bool Indirect(out string output)
        {
            return ExpandCore(out output, true);
        }

        protected bool ExpandCore(out string output, bool indirect)
        {
            if (recdepth > 9)
            {
                output = "Recursion detected - aborting expansion";
                return false;
            }

            output = "";

            foreach (Parameter p in paras)
            {
                string value = (p.isstring) ? p.value : vars[p.value];          // if string, its the value, else look up vars (must exist i've checked)

                if (indirect)
                {
                    if (vars.Exists(value))
                        value = vars[value];
                    else
                    {
                        output = "Indrect Variable " + value + " not found";
                        return false;
                    }
                }

                string res;
                ConditionFunctions.ExpandResult result = caller.ExpandStringFull(value, out res, recdepth + 1);

                if (result == ConditionFunctions.ExpandResult.Failed)
                {
                    output = res;
                    return false;
                }

                output += res;
            }

            return true;
        }

        #endregion

        #region Formatters

        protected bool SplitCaps(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            output = value.SplitCapsWordFull();
            return true;
        }

        static public string PhoneticShipName(string inname)
        {
            return inname.Replace("Mk. IV", "Mark 4").Replace("Mk. III", "Mark 3");
        }

        protected bool Ship(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            output = PhoneticShipName(value);
            output = output.SplitCapsWordFull();
            return true;
        }

        #endregion

        #region Dates

        protected bool DateTimeNow(out string output)
        {
            paras.Add(new Parameter() { isstring = false, value = paras[0].value });            // move P1 to P2
            paras[0].value = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            paras[0].isstring = true;
            return Date(out output);
        }

        protected bool Date(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];

            DateTime res;

            if (DateTime.TryParse(value, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"),
                                    System.Globalization.DateTimeStyles.None, out res))
            {
                string t = paras[1].value.ToLower();

                if (t.Equals("longtime"))
                {
                    output = res.ToLongTimeString();
                }
                else if (t.Equals("shorttime"))
                {
                    output = res.ToShortTimeString();
                }
                else if (t.Equals("longdate"))
                {
                    output = res.ToLongDateString();
                }
                else if (t.Equals("longdatetime"))
                {
                    output = res.ToLongDateString() + " " + res.ToLongTimeString();
                }
                else if (t.Equals("datetime"))
                {
                    output = res.ToShortDateString() + " " + res.ToLongTimeString();
                }
                else if (t.Equals("shortdate"))
                {
                    output = res.ToShortDateString();
                }
                else if (t.Equals("utc"))
                {
                    output = res.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                }
                else if (t.Equals("local"))
                {
                    output = res.ToString("yyyy/MM/dd HH:mm:ss");
                }
                else if (t.Equals(""))
                {
                    output = res.ToUniversalTime().ToString("yyyy/mm/dd HH:mm:ss");
                }
                else
                {
                    output = "Format selector not supported";
                    return false;
                }

                return true;
            }
            else
                output = "Date is not in correct en-US format";

            return false;
        }

        protected bool DateHour(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];

            DateTime res;

            if (DateTime.TryParse(value, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"),
                                    System.Globalization.DateTimeStyles.None, out res))
            {
                output = res.Hour.ToString(ct);
                return true;
            }
            else
                output = "Date is not in correct en-US format";

            return false;
        }

        #endregion

        #region String Manip

        protected bool SubString(out string output)
        {
            int start, length;

            bool okstart = paras[1].value.InvariantParse(out start) || (vars.Exists(paras[1].value) && vars[paras[1].value].InvariantParse(out start));
            bool oklength = paras[2].value.InvariantParse(out length) || (vars.Exists(paras[2].value) && vars[paras[2].value].InvariantParse(out length));

            if (okstart && oklength)
            {
                string v = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];

                if (start >= 0 && start < v.Length)
                {
                    if (start + length > v.Length)
                        length = v.Length - start;

                    output = v.Substring(start, length);
                }
                else
                    output = "";

                return true;
            }
            else
                output = "Start and/or length are not integers or variables do not exist";

            return false;
        }

        protected bool IndexOf(out string output)
        {
            string test = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            string value = (paras[1].isstring) ? paras[1].value : vars[paras[1].value];
            output = test.IndexOf(value).ToString(ct);
            return true;
        }

        protected bool Lower(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            string delim = (paras.Count > 1) ? ((paras[1].isstring) ? paras[1].value : vars[paras[1].value]) : "";

            for (int i = 2; i < paras.Count; i++)
                value += delim + ((paras[i].isstring) ? paras[i].value : vars[paras[i].value]);

            output = value.ToLower();
            return true;
        }

        protected bool Upper(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            string delim = (paras.Count > 1) ? ((paras[1].isstring) ? paras[1].value : vars[paras[1].value]) : "";

            for (int i = 2; i < paras.Count; i++)
                value += delim + ((paras[i].isstring) ? paras[i].value : vars[paras[i].value]);

            output = value.ToUpper();
            return true;
        }

        protected bool Join(out string output)
        {
            string delim = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            string value = (paras[1].isstring) ? paras[1].value : vars[paras[1].value];

            for (int i = 2; i < paras.Count; i++)
                value += delim + ((paras[i].isstring) ? paras[i].value : vars[paras[i].value]);

            output = value;
            return true;
        }

        protected bool Trim(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            output = value.Trim();
            return true;
        }

        protected bool Length(out string output)
        {
            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            output = value.Length.ToString(ct);
            return true;
        }

        protected bool EscapeChar(out string output)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];
            output = s.EscapeControlChars();
            return true;
        }

        protected bool ReplaceEscapeChar(out string output)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];
            output = s.ReplaceEscapeControlChars();
            return true;
        }

        protected bool WordOf(out string output)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];
            string c = vars.Exists(paras[1].value) ? vars[paras[1].value] : paras[1].value;
            string splitter = (paras.Count >= 3) ? (paras[2].isstring ? paras[2].value : vars[paras[2].value]) : ";";
            char splitchar = (splitter.Length > 0) ? splitter[0] : ';';

            int count;
            if (c.InvariantParse(out count))
            {
                string[] split = s.Split(splitchar);
                count = Math.Max(1, Math.Min(count, split.Length));  // between 1 and split length
                output = split[count - 1];
                return true;
            }
            else
            {
                output = "Parameter should be an integer constant or a variable name with an integer in its value";
                return false;
            }
        }

        protected bool WordListCount(out string output)
        {
            BaseUtils.StringParser l = new BaseUtils.StringParser(paras[0].isstring ? paras[0].value : vars[paras[0].value]);
            List<string> ll = l.NextQuotedWordList();
            output = ll.Count.ToStringInvariant();
            return true;
        }

        protected bool WordListEntry(out string output)
        {
            BaseUtils.StringParser l = new BaseUtils.StringParser(paras[0].isstring ? paras[0].value : vars[paras[0].value]);
            string c = vars.Exists(paras[1].value) ? vars[paras[1].value] : paras[1].value;

            output = "";

            int count;
            if (c.InvariantParse(out count))
            {
                List<string> ll = l.NextQuotedWordList();
                if (count >= 0 && count < ll.Count)
                {
                    output = ll[count];
                }
            }
            else
            {
                output = "Parameter should be an integer constant or a variable name with an integer in its value";
                return false;
            }

            return true;
        }


        protected bool Replace(out string output)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];
            string f1 = paras[1].isstring ? paras[1].value : vars[paras[1].value];
            string f2 = paras[2].isstring ? paras[2].value : vars[paras[2].value];
            output = s.Replace(f1, f2, StringComparison.InvariantCultureIgnoreCase);
            return true;
        }

        protected bool ReplaceVar(out string output)
        {
            return ReplaceVarCommon(out output, false);
        }

        protected bool ReplaceVarSC(out string output)
        {
            return ReplaceVarCommon(out output, true);
        }

        protected bool ReplaceVarCommon(out string output, bool sc)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];
            string varroot = paras[1].isstring ? paras[1].value : (vars.Exists(paras[1].value) ? vars[paras[1].value] : paras[1].value);

            foreach (string key in vars.NameEnumuerable)          // all vars.. starting with varroot
            {
                if (key.StartsWith(varroot))
                {
                    string[] subs = vars[key].Split(';');
                    if (subs.Length == 2 && subs[0].Length > 0 && s.IndexOf(subs[0], StringComparison.InvariantCultureIgnoreCase) >= 0)
                        s = s.Replace(subs[0], subs[1], StringComparison.InvariantCultureIgnoreCase);
                }
            }

            if (sc)
                output = s.SplitCapsWordFull();
            else
                output = s;

            return true;
        }

        protected bool Phrase(out string output)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];
            output = s.PickOneOfGroups(rnd);
            return true;
        }

        protected bool SafeVarName(out string output)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];
            output = s.SafeVariableString();
            return true;
        }

        #endregion

        #region Numbers

        protected bool Abs(out string output)
        {
            double para;
            if (vars[paras[0].value].InvariantParse(out para))
            {
                string fmt = vars.Exists(paras[1].value) ? vars[paras[1].value] : paras[1].value;
                if (FormatIt(Math.Abs(para), fmt, out output))
                    return true;
            }
            else
                output = "Parameter number be a number";

            return false;
        }

        protected bool Floor(out string output)
        {
            double para;
            if (vars[paras[0].value].InvariantParse(out para))
            {
                string fmt = vars.Exists(paras[1].value) ? vars[paras[1].value] : paras[1].value;
                if (FormatIt(Math.Floor(para), fmt, out output))
                    return true;
            }
            else
                output = "Parameter number be a number";

            return false;
        }

        protected bool RoundCommon(out string output)
        {
            int extradigits = 0;

            if (paras.Count >= 4)
            {
                if (!paras[3].value.InvariantParse(out extradigits) && !(vars.Exists(paras[3].value) && vars[paras[3].value].InvariantParse(out extradigits)))
                {
                    output = "The variable " + paras[3] + " does not exist or the value is not an integer";
                    return false;
                }
            }

            double scale = 1.0;
            if (paras.Count >= 5)       // round scale.
            {
                if (!paras[4].value.InvariantParse(out scale) && !(vars.Exists(paras[4].value) && vars[paras[4].value].InvariantParse(out scale)))
                {
                    output = "The variable " + paras[4] + " does not exist of the value is not a fractional";
                    return false;
                }
            }

            double value;

            if (vars[paras[0].value].InvariantParse(out value))
            {
                value *= scale;

                int digits = 0;
                if (paras[1].value.InvariantParse(out digits) || (vars.Exists(paras[1].value) && vars[paras[1].value].InvariantParse(out digits)))
                {
                    string fmt = vars.Exists(paras[2].value) ? vars[paras[2].value] : paras[2].value;

                    double res = Math.Round(value, digits);

                    if (extradigits > 0 && Math.Abs(res) < 0.0000001)     // if rounded to zero..
                    {
                        digits += extradigits;
                        fmt += new string('#', extradigits);
                        res = Math.Round(value, digits);
                    }

                    if (FormatIt(res, fmt, out output))
                        return true;
                }
                else
                    output = "Digits must be a variable or an integer number of digits";
            }
            else
                output = "Variable must be a integer or fractional";

            return false;
        }

        protected bool FormatIt(double v, string fmt, out string output)
        {
            output = "";

            if (fmt.StartsWith("M"))
            {
                fmt = fmt.Substring(1);

                if (v < 0)
                {
                    output = "Minus ";
                    v = -v;
                }
            }

            try
            {
                output += v.ToString(fmt, ct);
                return true;
            }
            catch
            {
                output = "Format must be a c# ToString format for doubles";
                return false;
            }
        }

        protected bool Random(out string output)
        {
            int v;
            if (paras[0].value.InvariantParse(out v) || (vars.Exists(paras[0].value) && vars[paras[0].value].InvariantParse(out v)))
            {
                output = rnd.Next(v).ToString(ct);
                return true;
            }
            else
            {
                output = "Parameter should be an integer constant or a variable name with an integer in its value";
                return false;
            }
        }

        protected bool Eval(out string output)
        {
            string s = paras[0].isstring ? paras[0].value : vars[paras[0].value];

            bool tryit = paras.Count > 1 && paras[1].value.Equals("Try", StringComparison.InvariantCultureIgnoreCase);

            bool evalstate = s.Eval(out output);      // true okay, with output, false bad, with error

            if (tryit && !evalstate)                   // if try and failed.. NAN without error
            {
                output = "NAN";
                return true;
            }

            return evalstate;                       // else return error and output
        }

        #endregion

        #region Conditionals

        protected bool Ifnotempty(out string output) { return IfCommon(out output, IfType.Empty, false); }
        protected bool Ifempty(out string output) { return IfCommon(out output, IfType.Empty, true); }
        protected bool Iftrue(out string output) { return IfCommon(out output, IfType.True, true); }
        protected bool Iffalse(out string output) { return IfCommon(out output, IfType.True, false); }
        protected bool Ifzero(out string output) { return IfCommon(out output, IfType.Zero, true); }
        protected bool Ifnonzero(out string output) { return IfCommon(out output, IfType.Zero, false); }
        protected bool Ifcontains(out string output) { return IfCommon(out output, IfType.Contains, true); }
        protected bool Ifnotcontains(out string output) { return IfCommon(out output, IfType.Contains, false); }
        protected bool Ifequal(out string output) { return IfCommon(out output, IfType.StrEquals, true); }
        protected bool Ifnotequal(out string output) { return IfCommon(out output, IfType.StrEquals, false); }
        protected bool Ifnumgreater(out string output) { return IfCommon(out output, IfType.Greater, false); }
        protected bool Ifnumless(out string output) { return IfCommon(out output, IfType.Less, false); }
        protected bool Ifnumgreaterequal(out string output) { return IfCommon(out output, IfType.GreaterEqual, false); }
        protected bool Ifnumlessequal(out string output) { return IfCommon(out output, IfType.LessEqual, false); }
        protected bool Ifnumequal(out string output) { return IfCommon(out output, IfType.NumEqual, true); }
        protected bool Ifnumnotequal(out string output) { return IfCommon(out output, IfType.NumEqual, false); }

        protected enum IfType { True, Contains, StrEquals, Empty, Zero, Greater, Less, GreaterEqual, LessEqual, NumEqual };

        protected bool IfCommon(out string output,
                              IfType iftype, bool test)
        {
            if (iftype == IfType.Empty || iftype == IfType.True || iftype == IfType.Zero)         // these, insert a dummy entry to normalise - we don't have a comparitor
                paras.Insert(1, new Parameter() { value = "", isstring = true });

            // p0 = value, p1 = comparitor, p2 = true expansion, p3 = false expansion, p4 = empty expansion

            string value = (paras[0].isstring) ? paras[0].value : vars[paras[0].value];
            string comparitor = (paras[1].isstring) ? paras[1].value : vars[paras[1].value];

            int pexp = 0;       // 0 = blank, else parameter to expand

            if (paras.Count >= 5 && value.Length == 0)        // if we have an empty, and string is empty.
                pexp = 4;
            else
            {
                bool tres;

                if (iftype == IfType.True)
                {
                    int nres;
                    bool ok = value.InvariantParse(out nres);

                    if (!ok)
                    {
                        output = "Condition value is not an integer";
                        return false;
                    }

                    tres = (nres != 0) == test;
                }
                else if (iftype == IfType.Contains)
                {
                    tres = (value.IndexOf(comparitor, StringComparison.InvariantCultureIgnoreCase) != -1) == test;
                }
                else if (iftype == IfType.StrEquals)
                {
                    tres = value.Equals(comparitor, StringComparison.InvariantCultureIgnoreCase) == test;
                }
                else if (iftype == IfType.Empty)                 // 2 parameters
                {
                    tres = (value.Length == 0) == test;
                }
                else if (iftype == IfType.Zero)
                {
                    double nres;
                    bool ok = value.InvariantParse(out nres);

                    if (!ok)
                    {
                        output = "Condition value is not an fractional or integer";
                        return false;
                    }

                    tres = (Math.Abs(nres) < 0.000001) == test;
                }
                else
                {
                    double nleft, nright = 0;
                    bool ok = value.InvariantParse(out nleft) && comparitor.InvariantParse(out nright);

                    if (!ok)
                    {
                        output = "Condition value is not an fractional or integer on one or both sides";
                        return false;
                    }

                    if (iftype == IfType.Greater)
                        tres = nleft > nright;
                    else if (iftype == IfType.GreaterEqual)
                        tres = nleft >= nright;
                    else if (iftype == IfType.Less)
                        tres = nleft < nright;
                    else if (iftype == IfType.LessEqual)
                        tres = nleft < nright;
                    else
                        tres = (Math.Abs(nleft - nright) < 0.000001) == test;
                }

                if (tres)
                    pexp = 2;
                else if (paras.Count >= 4)          // if we don't have p4, then use 0, which is empty
                    pexp = 3;
            }

            if (pexp == 0)
                output = "";
            else if (paras[pexp].isstring)      // string.. already been expanded, don't do it again
                output = paras[pexp].value;
            else
                return caller.ExpandStringFull(vars[paras[pexp].value], out output, recdepth + 1) != ConditionFunctions.ExpandResult.Failed;

            return true;
        }

        #endregion

        #region Is functions    

        protected bool Ispresent(out string output)
        {
            if (vars.Exists(paras[0].value))        // if paras[0] is a macro which exists
            {
                string mvalue = vars[paras[0].value];
                string cvalue = (paras[1].isstring) ? paras[1].value : vars[paras[1].value];

                output = mvalue.IndexOf(cvalue, StringComparison.InvariantCultureIgnoreCase) >= 0 ? "1" : "0";
            }
            else
            {   // var does not exist..
                if (paras.Count == 3)   // if default is there, see if its a macro, if so return value, else just return it
                    output = vars.Exists(paras[2].value) ? vars[paras[2].value] : paras[2].value;
                else
                    output = "0";
            }

            return true;
        }

        #endregion

        #region Arrays

        protected bool ExpandArray(out string output)
        {
            return ExpandArrayCommon(out output, false);
        }

        protected bool ExpandVars(out string output)
        {
            return ExpandArrayCommon(out output, true);
        }

        protected bool ExpandArrayCommon(out string output, bool join)
        {
            string arrayroot = paras[0].value;
            string separ = (paras[1].isstring) ? paras[1].value : vars[paras[1].value];

            int start, length;
            bool okstart = paras[2].value.InvariantParse(out start) || (vars.Exists(paras[2].value) && vars[paras[2].value].InvariantParse(out start));
            bool oklength = paras[3].value.InvariantParse(out length) || (vars.Exists(paras[3].value) && vars[paras[3].value].InvariantParse(out length));

            if (okstart && oklength)
            {
                bool splitcaps = paras.Count == 5 && paras[4].value.IndexOf("splitcaps", StringComparison.InvariantCultureIgnoreCase) >= 0;

                output = "";

                if (join)
                {
                    bool nameonly = paras.Count == 5 && paras[4].value.IndexOf("nameonly", StringComparison.InvariantCultureIgnoreCase) >= 0;
                    bool valueonly = paras.Count == 5 && paras[4].value.IndexOf("valueonly", StringComparison.InvariantCultureIgnoreCase) >= 0;

                    int index = 0;
                    foreach (string key in vars.NameEnumuerable)
                    {
                        if (key.StartsWith(arrayroot))
                        {
                            index++;
                            if (index >= start && index < start + length)
                            {
                                string value = vars[key];

                                string entry = (valueonly) ? vars[key] : (key.Substring(arrayroot.Length) + (nameonly ? "" : (" = " + vars[key])));

                                if (output.Length > 0)
                                    output += separ;

                                output += (splitcaps) ? entry.SplitCapsWordFull() : entry;
                            }
                        }
                    }
                }
                else
                {
                    for (int i = start; i < start + length; i++)
                    {
                        string aname = arrayroot + "[" + i.ToString(ct) + "]";

                        if (vars.Exists(aname))
                        {
                            if (i != start)
                                output += separ;

                            if (splitcaps)
                                output += vars[aname].SplitCapsWordFull();
                            else
                                output += vars[aname];
                        }
                        else
                            break;
                    }
                }

                return true;
            }
            else
                output = "Start and/or length are not integers or variables do not exist";

            return false;
        }

        #endregion

        #region File Functions

        protected bool OpenFile(out string output)
        {
            if (handles == null)
            {
                output = "File access not supported";
                return false;
            }

            string handle = paras[0].value;
            string file = paras[1].isstring ? paras[1].value : vars[paras[1].value];
            string mode = vars.Exists(paras[2].value) ? vars[paras[2].value] : paras[2].value;

            FileMode fm;
            if (Enum.TryParse<FileMode>(mode, true, out fm))
            {
                if (VerifyFileAccess(file, fm))
                {
                    string errmsg;
                    int id = handles.Open(file, fm, fm == FileMode.Open ? FileAccess.Read : FileAccess.Write, out errmsg);
                    if (id > 0)
                        vars[handle] = id.ToString();
                    else
                        output = errmsg;

                    output = (id > 0) ? "1" : "0";
                    return true;
                }
                else
                    output = "Permission denied access to " + file;
            }
            else
                output = "Unknown File Mode";

            return false;
        }

        protected bool CloseFile(out string output)
        {
            int? hv = vars[paras[0].value].InvariantParseIntNull();

            if (hv != null && handles != null)
            {
                handles.Close(hv.Value);
                output = "1";
                return true;
            }
            else
            {
                output = "File handle not found or invalid";
                return false;
            }
        }

        protected bool ReadLineFile(out string output)
        {
            int? hv = vars[paras[0].value].InvariantParseIntNull();

            if (hv != null && handles != null)
            {
                if (handles.ReadLine(hv.Value, out output))
                {
                    if (output == null)
                        output = "0";
                    else
                    {
                        vars[paras[1].value] = output;
                        output = "1";
                    }
                    return true;
                }
                else
                    return false;
            }
            else
            {
                output = "File handle not found or invalid";
                return false;
            }
        }

        protected bool WriteLineFile(out string output)
        {
            return WriteToFile(out output, true);
        }
        protected bool WriteFile(out string output)
        {
            return WriteToFile(out output, false);
        }
        protected bool WriteToFile(out string output, bool lf)
        {
            int? hv = vars[paras[0].value].InvariantParseIntNull();
            string line = paras[1].isstring ? paras[1].value : vars[paras[1].value];

            if (hv != null && handles != null)
            {
                if (handles.WriteLine(hv.Value, line, lf, out output))
                {
                    output = "1";
                    return true;
                }
                else
                    return false;
            }
            else
            {
                output = "File handle not found or invalid";
                return false;
            }
        }

        protected bool SeekFile(out string output)
        {
            int? hv = vars[paras[0].value].InvariantParseIntNull();
            long? pos = paras[1].value.InvariantParseLongNull();
            if (pos == null && vars.Exists(paras[1].value))
                pos = vars[paras[1].value].InvariantParseLongNull();

            if (hv != null && pos != null && handles != null)
            {
                if (handles.Seek(hv.Value, pos.Value, out output))
                {
                    output = "1";
                    return true;
                }
                else
                    return false;
            }
            else
            {
                output = "File handle not found or invalid, or position invalid";
                return false;
            }
        }
        protected bool TellFile(out string output)
        {
            int? hv = vars[paras[0].value].InvariantParseIntNull();
            if (hv != null && handles != null)
            {
                if (handles.Tell(hv.Value, out output))
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                output = "File handle not found or invalid";
                return false;
            }
        }

        protected bool FileExists(out string output)
        {
            foreach (Parameter p in paras)
            {
                string s = p.isstring ? p.value : vars[p.value];

                if (!System.IO.File.Exists(s))
                {
                    output = "0";
                    return true;
                }
            }

            output = "1";
            return true;
        }

        protected bool DirExists(out string output)
        {
            foreach (Parameter p in paras)
            {
                string s = p.isstring ? p.value : vars[p.value];

                if (!System.IO.Directory.Exists(s))
                {
                    output = "0";
                    return true;
                }
            }

            output = "1";
            return true;
        }


        protected bool FileLength(out string output)
        {
            string line = paras[0].isstring ? paras[0].value : vars[paras[0].value];

            try
            {
                FileInfo fi = new FileInfo(line);
                output = fi.Length.ToString(ct);
                return true;
            }
            catch { }
            {
                output = "-1";
                return true;
            }
        }


        protected bool MkDir(out string output)
        {
            string line = paras[0].isstring ? paras[0].value : vars[paras[0].value];

            try
            {
                DirectoryInfo di = Directory.CreateDirectory(line);
                output = "1";
                return true;
            }
            catch { }

            output = "0";
            return true;
        }

        protected bool SystemPath(out string output)
        {
            string id = paras[0].value;

            Environment.SpecialFolder sf;

            if (Enum.TryParse<Environment.SpecialFolder>(id, true, out sf))
            {
                output = Environment.GetFolderPath(sf);
                return true;
            }
            else
            {
                output = "";
                return false;
            }
        }

        protected bool FindLine(out string output)
        {
            string value = (paras[1].isstring) ? paras[1].value : (vars.Exists(paras[1].value) ? vars[paras[1].value] : null);

            if (value != null)
            {
                using (System.IO.TextReader sr = new System.IO.StringReader(vars[paras[0].value]))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.IndexOf(value, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            output = line;
                            return true;
                        }
                    }
                }

                output = "";
                return true;
            }
            else
                output = "The variable " + paras[1].value + " does not exist";

            return false;
        }

        protected virtual bool VerifyFileAccess(string file, FileMode fm)      // override to provide protection
        {
            return true;
        }

        #endregion

        #region Misc

        protected bool TickCount(out string output)
        {
            output = Environment.TickCount.ToStringInvariant();
            return true;
        }

        #endregion

    }
}