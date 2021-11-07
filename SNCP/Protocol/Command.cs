
namespace SNCP.Protocol
{
    class Command
    {
        private string[] _command;
        public Command(string commandString)
        {
            this._command = commandString.Trim().Split(" ");
        }

        public string GetCommand()
        {
            return this._command[0];
        }

        public string GetElementAt(int i)
        {
            if(i < this._command.Length)
            {
                return this._command[i];
            }

            return "";
            
        }

        public string GetFrom(int i)
        {
            string[] ret = new string[1024];
            for (int k = i; k < this._command.Length; k++)
            {
                ret[k - i] = this._command[k];
            }

            return string.Join(" ", ret);
        }
    }
}