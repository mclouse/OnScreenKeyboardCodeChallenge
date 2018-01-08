using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OnScreenKeyboardCodeChallenge
{
    public partial class Form1 : Form
    {
        public string previousPosition = "";
        public string lastPosition = "";
        public char previousCharacter = ' ';

        public Form1()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string outStr = "";
            string fileToOpen = "";

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileToOpen = openFileDialog1.FileName;
                using (StreamReader sr = File.OpenText(fileToOpen))
                {
                    string s = String.Empty;
                    while ((s = sr.ReadLine()) != null)
                    {
                         foreach (char c in s.ToString().Trim())
                        {
                            //Account for first char A start position
                            if (previousPosition == "")
                            {
                                previousPosition = "1:0";
                                outStr += writeStepToFirstCharacterInInputString(previousPosition, getCharPositionInString(c)) + "#,";
                            }

                            if (c.ToString().Trim() == "")
                            {
                                 lastPosition = getCharPositionInString(previousCharacter);
                            }
                            else
                            {
                                 previousPosition = getCharPositionInString(c);
                            }

                            string appendStr = "";
                            appendStr = "#,";

                            if (positionValid(previousPosition) && positionValid(lastPosition) && writeStepsBetweenCharacters(c.ToString(), previousPosition, lastPosition).Trim() != "")
                            {
                                string strToAppend = writeStepsBetweenCharacters(c.ToString(), previousPosition, lastPosition) + appendStr;
                                outStr += strToAppend;
                            }
                            else
                            {
                                if (c.ToString() == " ")
                                {
                                    string strToAppend = "S," + writeStepsBetweenCharacters(c.ToString(), previousPosition, lastPosition) + appendStr;
                                    if (strToAppend.Substring(strToAppend.Length - 2, 2) == "#,")
                                    {
                                        strToAppend = strToAppend.Substring(0, strToAppend.Length - 2);
                                    }
                                    outStr += strToAppend;
                                }
                            }

                            lastPosition = previousPosition;
                            previousCharacter = c;
                        }

                        if (outStr.Substring(outStr.Length - 1, 1) == ",")
                        {
                            outStr = outStr.Substring(0, outStr.Length - 1);
                        }

                        txtFinalOutput.Text += outStr + Environment.NewLine;
                        outStr = "";
                    }
                }
            }
            else { MessageBox.Show("Error opening file, cannot continue!"); }
        }

        private string writeStepsBetweenCharacters(string curChar, string firstPos, string lastPos)
        {
            //Incoming string firstPos and lastPos as "row number of char : position of char - zero based"
            string varOut = "";
            string[] tokensFirst = firstPos.Split(':');
            string[] tokensLast = lastPos.Split(':');
            int rowNumFirst = 0;
            int colNumFirst = -1;
            int rowNumLast = 0;
            int colNumLast = -1;
            bool rowNumFirstValid = false;
            bool colNumFirstValid = false;
            bool rowNumLastValid = false;
            bool colNumLastValid = false;

            //Error if hit either of these lines
            //Check for existence of both previous position and current position before processing!
            rowNumFirstValid = int.TryParse(tokensFirst[0], out rowNumFirst);
            colNumFirstValid = int.TryParse(tokensFirst[1], out colNumFirst);
            rowNumLastValid = int.TryParse(tokensLast[0], out rowNumLast);
            colNumLastValid = int.TryParse(tokensLast[1], out colNumLast);

            if (!rowNumFirstValid && !colNumFirstValid && !rowNumLastValid && !colNumLastValid)
            {
                //Error: cannot compare properly
                MessageBox.Show("Error: Missing Row and or Number values" + Environment.NewLine
                    + "rowNumFirstValid" + rowNumFirstValid + Environment.NewLine
                    + "colNumFirstValid" + colNumFirstValid + Environment.NewLine
                    + "rowNumLastValid" + rowNumLastValid + Environment.NewLine
                    + "colNumLastValid" + colNumLastValid + Environment.NewLine);
            }

                if (rowNumFirst > rowNumLast)
                {
                    for (int i = rowNumLast; i < rowNumFirst; ++i)
                    {
                        varOut += "D,";
                    }
                }
                if (rowNumLast > rowNumFirst)
                {
                    for (int i = rowNumFirst; i < rowNumLast; ++i)
                    {
                        varOut += "U,";
                    }
                }

                //Compare column positions
                if (colNumFirst > colNumLast)
                {
                    for (int i = colNumLast; i < colNumFirst; ++i)
                    {
                        varOut += "R,";
                    }
                }
                if (colNumFirst < colNumLast)
                {
                    for (int i = colNumFirst; i < colNumLast; ++i)
                    {
                        varOut += "L,";
                    }
                }

            return varOut;
        }

        private string writeStepToFirstCharacterInInputString(string firstPos, string lastPos)
        {
            //Incoming string firstPos and lastPos as "row number of char : position of char - zero based"
            string varOut = "";
            firstPos = "1:0";
            string[] tokensFirst = firstPos.Split(':');
            string[] tokensLast = lastPos.Split(':');
            int rowNumFirst = 0;
            int colNumFirst = -1;
            int rowNumLast = 0;
            int colNumLast = -1;
            bool rowNumFirstValid = false;
            bool colNumFirstValid = false;
            bool rowNumLastValid = false;
            bool colNumLastValid = false;

            //Error if hit either of these lines
            //Check for existence of both previous position and current position before processing!
            rowNumFirstValid = int.TryParse(tokensFirst[0], out rowNumFirst);
            colNumFirstValid = int.TryParse(tokensFirst[1], out colNumFirst);
            rowNumLastValid = int.TryParse(tokensLast[0], out rowNumLast);
            colNumLastValid = int.TryParse(tokensLast[1], out colNumLast);

            if (!rowNumFirstValid && !colNumFirstValid && !rowNumLastValid && !colNumLastValid)
            {//Error: cannot compare properly
                MessageBox.Show("Error in writeStepsBetweenCharacters(): Missing Row and or Number values" + Environment.NewLine
                    + "rowNumFirstValid" + rowNumFirstValid + Environment.NewLine
                    + "colNumFirstValid" + colNumFirstValid + Environment.NewLine
                    + "rowNumLastValid" + rowNumLastValid + Environment.NewLine
                    + "colNumLastValid" + colNumLastValid + Environment.NewLine);
            }

            if (rowNumFirst > rowNumLast)
            {
                for (int i = rowNumLast; i < rowNumFirst; ++i)
                {
                    varOut += "U,";
                }

            }
            if (rowNumLast > rowNumFirst)
            {
                for (int i = rowNumFirst; i < rowNumLast; ++i)
                {
                    varOut += "D,";
                }
            }

            //Compare column positions
            if (colNumFirst > colNumLast)
            {
                for (int i = colNumLast; i < colNumFirst; ++i)
                {
                    varOut += "L,";
                }

            }
            if (colNumFirst < colNumLast)
            {
                for (int i = colNumFirst; i < colNumLast; ++i)
                {
                    varOut += "R,";
                }
            }

            previousPosition = lastPos; //set the most recent position to the previous position for next comparison
            return varOut;
        }

        private bool positionValid(string position)
        { 
            //Invalid - value(s) missing
            if (position.Length < 3) { return false; }

            string[] tokensFirst = position.Split(':');
            int rowNumFirst = -1;
            int colNumFirst = -1;           
            string TokenFirst = tokensFirst[0];
            string TokenLast = tokensFirst[1];                       
            bool rowNumFirstValid = false;
            bool colNumFirstValid = false;
            rowNumFirstValid = int.TryParse(tokensFirst[0], out rowNumFirst);
            colNumFirstValid = int.TryParse(tokensFirst[1], out colNumFirst);
            if (!rowNumFirstValid && !colNumFirstValid)
            {
                return false;
            }
            else { return true; }
        }

        private string getCharPositionInString(char charIn)
        {
            string R0 = " ";
            string R1 = "ABCDEF";
            string R2 = "GHIJKL";
            string R3 = "MNOPQR";
            string R4 = "STUVWX";
            string R5 = "YZ1234";
            string R6 = "567890";

            charIn = Char.ToUpper(charIn);

            if (R0.IndexOf(charIn) > -1)
            {
                return "0:-1";
            }
            if (R1.IndexOf(charIn) > -1)
            {
                return "1:" + R1.IndexOf(charIn).ToString();
            }
            else if (R2.IndexOf(charIn) > -1)
            {
                return "2:" + R2.IndexOf(charIn).ToString();
            } 
            else if(R3.IndexOf(charIn) > -1)
            {
                return "3:" + R3.IndexOf(charIn).ToString();
            } 
            else if(R4.IndexOf(charIn) > -1)
            {
                return "4:" +  R4.IndexOf(charIn).ToString();
            } 
            else if(R5.IndexOf(charIn) > -1)
            {
                return "5:" + R5.IndexOf(charIn);
            }
            else if (R6.IndexOf(charIn) > -1)
            {
                return "6:" + R6.IndexOf(charIn);
            }
            else
            {
                return "";
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
