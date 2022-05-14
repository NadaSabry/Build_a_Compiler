using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace Compiler_Application.Controllers
{
    public class ScannerController : Controller
    {
        public Dictionary<string, int> Ncolumn = new Dictionary<string, int>();
        string TokenType = "invalid token";
        string path = "";
        int indx = 0;
        int line = 1;
        string[] ans=new string[1000];
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
            //Console.WriteLine("token = " + token + ";");
            int i = 0, error =-1,current_state = 0;
            string ch = token[i] + "";
            while (i < token.Length && current_state != error)
            {
                int newstate = State(current_state, ch);
                //Console.WriteLine(ch + " " + current_state + " " + newstate);
                current_state = newstate;
                i++;
                if (i < token.Length)
                    ch = token[i] + "";
            }
            //Console.WriteLine("current_state = " + current_state);
            if (current_state != error && State(current_state, "Status")== 1) return true;
            if (IsIdentifier(token)) return true;
            return false;
        }
      
        
        public bool IsDelimiter(char c)
        {
            if (c == ' ' || c == ';'|| c=='\n') return true;
            return false;
        }
        public bool IsIdentifier(string token)
        {
            int j = 0, newState=0,current_state = 0,error=-1; 
            while(j < token.Length && current_state != error)
            {
                if ((token[j] >= 'a' && token[j] <= 'z') || (token[j] >= 'A' && token[j] <= 'Z')||token [j]=='_') //letter
                    newState = State(current_state, "letter");
                else if (token[j] >= '0' && token[j] <= '9') //digit
                    newState = State(current_state, "digit");
                else return false;
                current_state = newState; 
                j++;
            }
            if (current_state == -1) return false;
            newState = State(current_state, "other");
            if (State(newState, "Status")==1) return true;
            return false;
        }

        public void Token(String code = "@ Type Person { \n Rational G ( ) { \n")
        {
            int i = 0;
            while (i < code.Length)
            {
                string token = ""; TokenType= "invalid token";
                while (i < code.Length && !IsDelimiter(code[i]))
                {
                    if (i + 2 < code.Length && code[i] == '*' && code[i + 1] == '*' && code[i + 2] == '*')
                    {
                        while (i < code.Length && code[i] != '\n' && code[i] != ';' ){token += code[i]; if (code[i] == '\n' || code[i] == ';') line++; i++;}
                        TokenType = "single line comment";
                    }
                    else if (i + 1 < code.Length && code[i] == '<' && code[i+1] == '/' )
                    {
                        while (i + 1 < code.Length && (code[i] != '/' || code[i + 1] != '>' )) {token += code[i]; if (code[i] == '\n' || code[i] == ';') line++; i++; }
                        if (i + 1 < code.Length)TokenType = "multiline comment";
                    }
                    else
                    {
                        token += code[i];
                        //if (code[i] == '\n' || code[i] == ';') line++;
                        i++;
                    }
                }
                if (code[i] == '\n' || code[i] == ';') line++;
                // Console.WriteLine("token = " + token); //+" valid ? " + isValidToken(token));
                if (token != "")
                {
                    string output;
                    if (TokenType != "invalid token")
                        output = "Line : " + line + " Token Text: " + token + "\tToken Type: " + TokenType;
                    else
                    {
                        isValidToken(token);
                        output = "Line : " + line + " Token Text: " + token + "  \tToken Type: " + TokenType;
                        //Console.WriteLine(token + " : invalid token ");
                    }
                    Console.WriteLine(output);
                    ans[indx] = output; indx++;
                }
                i++;
            }

        }


    }
}
