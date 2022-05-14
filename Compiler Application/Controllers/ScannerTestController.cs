using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace Compiler_Application.Controllers
{
    public class ScannerTestController : Controller
    {
        // Attribute 
        public Dictionary<string, int> Ncolumn = new Dictionary<string, int>();
        string TokenType = "invalid token";
        string path = "";
        int indx = 0;
        int line = 1;
        string[] ans = new string[1000];
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;

        public ScannerTestController(string path = "E:\\level-3\\level3_T2\\compiler\\Compiler-Project\\Compiler Application\\Data\\scanner2.xlsx", int sheet = 1)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[sheet];
            for (int j = 1; ; j++)
            {
                if (ws.Cells[1, j].Value2 != null)
                {
                    string s = ws.Cells[1, j].Value2 + "";
                    if (s == " =") s = "=";
                    else if (s == " -") s = "-";
                    else if (s == "space") s = " ";
                    Ncolumn.Add(s, j);
                    Console.WriteLine(s +" : " + j);
                }
                else break;
            }
        }
        public bool IsAcceptedState(int current_state = 0)
        {
            int j = Ncolumn["Status"], i = current_state + 2;
            string state = ws.Cells[i, j].Value2 + "";
            if (state != "0")
            {
                TokenType = state;
                return true;
            }
            return false;
        }
        public int getState(int current_state = 0, string ch = "")
        {
            if (ch == "\n") ch="newLine";
            int i = current_state + 2;
            if (!Ncolumn.ContainsKey(ch)) return -1;
            int j = Ncolumn[ch];
            int ans = -1;
            //Console.WriteLine(i +" " + j + " " + ws.Cells[i, j].Value2 + " " + ch);
            if (ws.Cells[i, j].Value2 != null) ans = (int)ws.Cells[i, j].Value2;
            return ans;
        }

        public bool isLetter(char c)
        {
            if((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_')return true;
            return false;
        }
        public bool isDigit(char c)
        {
            if (c >= '0' && c <= '9')return true;
            return false;
        }


        public bool IsIdentifier(string token)
        {
            int j = 0, newState = 0, current_state = 0, error = -1;
            while (j < token.Length && current_state != error)
            {
                if (isLetter(token[j]))
                    newState = getState(current_state, "letter");
                else if (isDigit(token[j]))
                    newState = getState(current_state, "digit");
                else return false;
                current_state = newState;
                j++;
            }
            if (current_state == -1) return false;
            newState = getState(current_state, "other");
            if (IsAcceptedState(newState)) return true;
            return false;
        }
        public bool isLineDelimiter(char c)
        {
            if(c == '\n' || c == ';')
            {
                line++;
                return true;
            }
            return false;
        }
        public bool isWordDelimiter(char c)
        {
            if (c == ' ') return true;
            return false;
        }

        public string ValidToken(string code,ref int i,ref int currentState)
        {
            int newState = 0, other = -1 ;
            string token1 = "";
            while (i < code.Length)
            {
                newState = getState(currentState, code[i] + "");
                if (newState == other) break;
                token1 += code[i];
                currentState = newState;
                //Console.WriteLine(code[i] + " c " + currentState);
                i++;
            }
            return token1;
        }

        public void Token(String code = " @ Type Person{\nRational G() {;int frt = 5; *** sum to number;")
        {
            int i = 0 ;
            while (i < code.Length)
            {
                while (i < code.Length && (isLineDelimiter(code[i]) || isWordDelimiter(code[i]))) i++;
                if (i >= code.Length) break;

                string output = "invalid", token1 = "", token2 = "";
                int j = i, currentState = 0;
                //Console.WriteLine(i + " : " + j);
                token1 = ValidToken(code, ref i,ref currentState);

                while (j < code.Length && (isLetter(code[j]) || isDigit(code[j]))) { token2 += code[j]; j++; }

                //Console.WriteLine(token1 + " " + token2 + ":");
                //Console.WriteLine("i = " + i +"state = " + currentState);
                
                // it is not read the character not in language like this  ? ( ) 
                if (token1 == "" && token2 == "")
                {
                     output = "Line : " + line + " Error in Token Text: " + code[i] ;
                     i++;
                }
                else if (token1.Length >= token2.Length && IsAcceptedState(currentState))
                {
                     output = "Line : " + line + " Token Text: " + token1 + "\tToken Type: " + TokenType;
                }
                else if(token1.Length > token2.Length)
                {
                    output = "Line : " + line + " Error in Token Text: " + token1;
                }
                else
                {
                    //Console.WriteLine(token2 );
                    if (IsIdentifier(token2)) output = "Line : " + line + "\tToken Text: " + token2 + "  Token Type: " + TokenType;
                    else output = "Line : " + line + " Error in Token Text: " + token2 ;
                    if (j >= i) i = j;
                }
                //Console.WriteLine(token1 + " " + token2 + ":");
                Console.WriteLine(output);
            }
        }
    }
}

    