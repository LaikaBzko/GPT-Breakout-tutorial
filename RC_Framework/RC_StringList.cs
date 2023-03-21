using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace Tutorial_1.RC_Framework
{
    public class StringList
    {
        public List<string> lst;

        public int Count
        {
            get { return lst.Count; }
        }

        public StringList()
        {
            lst = new List<string>();
        }

        public StringList(string s)
        {
            lst = new List<string>();
            lst.Add(s);
        }

        public StringList(StringList s)
        {
            lst = new List<string>();
            copy(s);
        }

        public StringList(string[] s)
        {
            lst = new List<string>();
            for (int i = 0; i < s.Length; i++)
            {
                lst.Add(s[i]);
            }
        }

        public string this[int index]
        {
            get
            {
                if (index < 0 || index >= lst.Count)
                    throw new IndexOutOfRangeException();
                else
                    return lst[index];
            }
        }

        public void copy(StringList s)
        {
            lst.Clear();
            for (int i = 0; i < s.Count; i++)
            {
                lst.Add(s[i]);
            }
        }

        public void Add(string s)
        {
            lst.Add(s);
        }

        public void RemoveAt(int index)
        {
            lst.RemoveAt(index);
        }

        public bool saveToFile(string fileName)
        {
            try
            {

                //Pass the filepath and filename to the StreamWriter Constructor
                StreamWriter sw = new StreamWriter(fileName);
                for (int i = 0; i < Count; i++)
                {
                    //Write a line of text
                    sw.WriteLine(lst[i]);
                }

                //Close the file
                sw.Close();
                return true;
            }
            catch //(Exception e)
            {
                //Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public bool readFromFile(string fileName)
        {
            string line;
            try
            {
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(fileName);

                //Read the first line of text
                line = sr.ReadLine();

                //Continue to read until you reach end of file
                while (line != null)
                {
                    lst.Add(line);
                    //Read the next line
                    line = sr.ReadLine();
                }

                //close the file
                sr.Close();
                return true;
            }
            catch //(Exception e)
            {
                //Console.WriteLine("Exception: " + e.Message);
                return false;
            }
        }

        public int getKeyIndex(string key)
        {
            int j;
            char[] delimiterChars = { '=' };

            for (int i = 0; i < Count; i++)
            {
                j = lst[i].IndexOf('=');
                if (j == -1) continue;
                string[] words = lst[i].Split(delimiterChars);
                if (words[0].Trim() == key.Trim()) return i;
            }
            return -1;
        }

        public string getValueFromPair(string key)
        {
            char[] delimiterChars = { '=' };
            int i = getKeyIndex(key);
            if (i == -1) return "";
            string[] words = lst[i].Split(delimiterChars);
            return words[1].Trim();
        }

        public bool getValueFromPairBool(string key)
        {
            if (getValueFromPair(key) == "true") return true;
            else return false;
        }

        public int getValueFromPairInt(string key)
        {
            char[] delimiterChars = { '=' };
            int i = getKeyIndex(key);
            if (i == -1) return 0;
            string[] words = lst[i].Split(delimiterChars);
            try
            {
                int numVal = Convert.ToInt32(words[1].Trim());
                return numVal;
            }
            catch
            {
                //Console.WriteLine("Input string is not a sequence of digits.");
                return 0;
            }
        }

        public double getValueFromPairFloat(string key)
        {
            return (float)getValueFromPairDouble(key);
        }

        public double getValueFromPairDouble(string key)
        {
            char[] delimiterChars = { '=' };
            int i = getKeyIndex(key);
            if (i == -1) return 0;
            string[] words = lst[i].Split(delimiterChars);
            try
            {
                double numVal = Convert.ToDouble(words[1].Trim());
                return numVal;
            }
            catch
            {
                //Console.WriteLine("Input string is not a sequence of digits.");
                return 0;
            }
        }

        public void setValuePair(string key, string val)
        {
            int i = getKeyIndex(key);
            if (i == -1)
            {
                lst.Add(key + "=" + val);
                return;
            }
            string s = key + "=" + val;
            lst[i] = s;
        }

        public void setValuePair(string key, int val)
        {
            setValuePair(key, val.ToString());
        }

        public void setValuePair(string key, bool val)
        {
            if (val) setValuePair(key, "true");
            else setValuePair(key, "false");
        }

        public void setValuePair(string key, double val)
        {
            setValuePair(key, val.ToString());
        }

        public void Clear()
        {
            lst.Clear();
        }

        public void limitList(int n)
        {
            if (n < Count) return;
            while (n > Count) lst.RemoveAt(0);
        }

    }
}
