using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace Compiler_Application.Controllers
{
    public class ScannerController : Controller
    {
        public Dictionary<string, int> Ncolumn = new Dictionary<string, int>();
        string TokenType = "";
        string path = "";
        _Application excel = new _Excel.Application();
        Workbook wb;
        Worksheet ws;
        
        public ScannerController(string path= "E:\\level-3\\level3_T2\\compiler\\Compiler-Project\\Compiler Application\\Data\\scanner.xlsx", int sheet=1)
        {
            this.path = path;
            wb = excel.Workbooks.Open(path);
            ws = wb.Worksheets[sheet];
            for (int j = 1; ; j++)
            {
                if (ws.Cells[1, j].Value2 != null)
                {
                    string s = ws.Cells[1, j].Value2 + "";
                    if (s == "'='") s = "=";
                    else if (s == " -") s = "-";
                    Ncolumn.Add(s, j);
                }
                else break;
            }
        }

        public int State(int current_state=0,string ch="H")
        {
            int i = current_state + 2;
            if (!Ncolumn.ContainsKey(ch)) return -1;
            int j = Ncolumn[ch];
            int ans = -1 ;
            if(ch == "Status")
            {
                string state = ws.Cells[i, j].Value2 + "";
                if (state != "0") { TokenType = state; return 1; }
                return 0;
            }
            //Console.WriteLine(i +" " + j + " " + ws.Cells[i, j].Value2 + " " + ch);
            if(ws.Cells[i, j].Value2 != null)
                ans = (int) ws.Cells[i, j].Value2 ;
            return ans;
        }


        public bool isValidToken(String token= "")
        {
            int i = 0, error =-1,current_state = 0;
            string ch = token[i] + "";
            while (i < token.Length && State(current_state, ch) != error)
            {
                int newstate = State(current_state, ch);
                //Console.WriteLine(ch + " " + current_state + " " + newstate);
                current_state = newstate;
                i++; 
                if(i<token.Length) 
                    ch = token[i] + "";
            }
            if (State(current_state, "Status")== 1)return true;
            if (IsIdentifier(token)) return true;
            return false;
        }
      // na</m,mbox,zmx,/>
        
        public bool IsDelimiter(char c)
        {
            if (c == ' ' || c == ';') return true;
            return false;
        }
        public bool IsIdentifier(string token)
        {
            int j = 0, newState=0,current_state = 0; 
            while(j < token.Length)
            {
                if ((token[j] >= 'a' && token[j] <= 'z') || (token[j] >= 'A' && token[j] <= 'Z')||token [j]=='_') //letter
                    newState = State(current_state, "letter");
                else if (token[j] >= '0' && token[j] <= '9') //digit
                    newState = State(current_state, "digit");
                else return false;
                current_state = newState; 
                j++;
            }
            newState = State(current_state, "other");
            if (State(newState, "Status")==1) return true;
            return false;
        }

        public void Token(String code = "Rational If ^ nada Else When")
        {
            int i = 0;
            while (i < code.Length)
            {
                string token = "";
                while (i < code.Length && !IsDelimiter(code[i]))
                {
                    token += code[i];
                    i++;
                }
               // Console.WriteLine("token = " + token); //+" valid ? " + isValidToken(token));
                if (token != "")
                {
                    if (!isValidToken(token)) Console.WriteLine(token + " : invalid token ");
                    else  Console.WriteLine(token + " : Token Type : " + TokenType);
                    /* if (IsIdentifier(token))
                     {
                         Console.WriteLine(token + " : Token Type : " + TokenType);
                     }
                     else
                     {
                         Console.WriteLine(token + " : invalid token ");
                     }
                 }
                 else
                     Console.WriteLine(token + " : Token Type : " + TokenType);
                    */

                }
                i++;
            }

        }


    }
}
