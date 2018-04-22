/*
 *  Copyright (C) 2010-2015 Leandro Nini
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driver.libsidplayfp.utils
{
    public class iniParser
    {



        //# include <string>
        //# include <map>
        //# include <utility>

        //private typedef std::map<std::string, std::string> keys_t;
        //private typedef std::map<std::string, keys_t> sections_t;
        private List<Tuple<string, List<Tuple<string, string>>>> sections=new List<Tuple<string, List<Tuple<string, string>>>>();// sections_t sections;
        private Tuple<string, List<Tuple<string, string>>> curSection;// sections_t::const_iterator curSection;
                                                                      //private string parseSection(ref string buffer) { return null; }
                                                                      //private List<Tuple<string, string>> parseKey(ref string buffer) { return null; }

        //public bool open(string fName) { return false; }
        //public void close() { }
        //public bool setSection(string section) { return false; }
        //public string getValue(string key) { return null; }




        /*
 *  Copyright (C) 2010-2015 Leandro Nini
 *
 *  This program is free software; you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation; either version 2 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */

        //#include "iniParser.h"
        //#include "sidcxx11.h"
        //#include <fstream>

        public class parseError : Exception { };

        private string parseSection(ref string buffer)
        {
            int pos = buffer.IndexOf(']');

            if (pos == -1)
            {
                throw new parseError();
            }

            return buffer.Substring(1, pos - 1);
        }

        private Tuple<string, string> parseKey(ref string buffer)
        {
            int pos = buffer.IndexOf('=');

            if (pos == -1)
            {
                throw new parseError();
            }

            string key = buffer.Substring(0, buffer.LastIndexOf(' ', pos - 1) + 1);
            string value = buffer.Substring(pos + 1);
            return new Tuple<string, string>(key, value);
        }

        public bool open(string fName)
        {
            Tuple<string, List<Tuple<string, string>>> mIt = null;

            try
            {
                using (System.IO.StreamReader iniFile = new System.IO.StreamReader(fName))
                {

                    string buffer;
                    while (iniFile.Peek() >= 0)
                    {
                        buffer = iniFile.ReadLine();

                        if (buffer == "")
                            continue;

                        switch (buffer[0])
                        {
                            case ';':
                            case '#':
                                // skip comments
                                break;
                            case '[':
                                try
                                {
                                    string section = parseSection(ref buffer);
                                    List<Tuple<string, string>> keys = null;
                                    sections.Add(
                                        new Tuple<string, List<Tuple<string, string>>>(section, keys)
                                        );
                                    mIt = sections[0];
                                }
                                catch (parseError)
                                {
                                }
                                break;
                            default:
                                try
                                {
                                    mIt.Item2.Add(parseKey(ref buffer));
                                }
                                catch (parseError)
                                {
                                }
                                break;
                        }
                    }

                    return true;

                }
            }
            catch
            {
                return false;

            }

        }

        public void close()
        {
            sections.Clear();
        }

        public bool setSection(string section)
        {
            curSection = null;
            foreach (Tuple<string, List<Tuple<string, string>>> c in sections)
            {
                if (c.Item1 == section)
                {
                    curSection = c;
                    break;
                }
            }
            return (curSection != sections[sections.Count - 1]);
        }

        public string getValue(byte[] key)
        {
            Tuple<string, string> keyIt = null;
            foreach (Tuple<string, string> c in curSection.Item2)
            {
                if (c.Item1 == Encoding.ASCII.GetString(key))
                {
                    keyIt = c;
                    break;
                }
            }
            return (keyIt != curSection.Item2[curSection.Item2.Count - 1]) ? keyIt.Item2 : null;
        }

    }
}
