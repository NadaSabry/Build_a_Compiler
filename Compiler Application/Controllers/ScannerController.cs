using Microsoft.AspNetCore.Mvc;
using Microsoft.Office.Interop.Excel;
using _Excel = Microsoft.Office.Interop.Excel;

namespace Compiler_Application.Controllers
{
    public class ScannerController : Controller
    {
        public Dictionary<string, int> Ncolumn = new Dictionary<string, int>(); 
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
                    Ncolumn.Add(s , j);
                }
                else break;
            }
        }

        public int State(int current_state=0,string ch="H")
        {
            int i = current_state + 2;
            int j = Ncolumn[ch];
            int ans = -1 ;
            if(ws.Cells[i, j].Value2 != null)
                ans = (int) ws.Cells[i, j].Value2 ;
            return ans;
        }


        public bool isValidToken(String code= "Rational n")
        {
            int i = 0;
            int current_state = 0;
            string ch = code[i]+ "";
            int IsAcceptedstate = State(current_state, "Status");
            Console.WriteLine("length = " + code.Length);
            while (IsAcceptedstate == 0 && State(current_state, ch) !=-1 && i< code.Length)
            {
                Console.WriteLine(current_state + " " + i);
                int newstate = State(current_state, ch);
                i++;
                if (i < code.Length && code[i]!=' ')
                {
                    ch = code[i] + "";
                }
                current_state = newstate;
                IsAcceptedstate = State(current_state, "Status");
            }
            Console.WriteLine(i + " " + current_state + " " + IsAcceptedstate +  " " + State(current_state, ch));
            if (State(current_state, "Status")== 1)
                 return true;
            return false;
        }
    }
}
