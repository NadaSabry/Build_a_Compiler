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
        int indx = 1;
        int line = 1,NOofErrors=0;
        string[] ans = new string[1000];
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;

        public ScannerTestController(string path = "E:\\level-3\\level3_T2\\compiler\\Compiler-Project\\Compiler Application\\Data\\scanner4.xlsx", int sheet = 1)
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
            if (state != "0"&& state!= "Status")
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

        public string ValidToken(string code, ref int i, ref int State)
        {
            int newState = 0, other = -1;
            string token1 = "";
            while (i < code.Length && !IsAcceptedState(State) && State != other)
            {
                newState = getState(State, code[i] + "");
                State = newState;
                //Console.WriteLine(code[i] + " : " + State);
                if (!IsAcceptedState(State) && State != other)
                {
                    token1 += code[i];
                }
                i++;
            } i--;
            return token1;
        }
        // ; \n --> check
        public void Token(String code = "Type test{\nIpokf x=3.5; Ipok y = x + 5 ?; } ")
        {
            int i = 0 ;
            while (i < code.Length)
            {
                while (i < code.Length && (isLineDelimiter(code[i]) || isWordDelimiter(code[i]))) i++;
                if (i >= code.Length) break;

                string output = "invalid", token = "";
                int currentState = 0;
                
                token = ValidToken(code, ref i,ref currentState);

                
                
                

                if (token == "")
                {
                    while (i < code.Length && code[i] != '\n' && code[i] != ' ' && code[i] != ';') { token += code[i]; i++; }
                    output = "Line : " + line + " Error in Token Text: " + token;
                    NOofErrors++;
                }
                else if (IsAcceptedState(currentState))
                    output = "Line : " + line + " Token Text: " + token + "  Token Type: " + TokenType;
                else
                {
                    output = "Line : " + line + " Error in Token Text: " + token;
                    NOofErrors++;
                }
                ans[indx] = output;
                indx++;

                Console.WriteLine(output);
            }
            ans[0] = NOofErrors+"";
            Console.WriteLine("Total NO of errors: " + ans[0]);
        }

        [HttpPost]
        public String getText(string code)
        {
            Token(code);
            String Display="";
            for (int i = 1; i < indx; i++)
            {
                Display += ans[i]+"<br />";
            }
            Display += "Total NO of errors: " + NOofErrors ;
            return Display;
        }
    }
}

    