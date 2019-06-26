using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSScriptLibrary;

namespace CopyPaster
{
    public class ScriptLoader
    {
        private string scriptsPath;
        public ScriptLoader()
        {
            scriptsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CopyPaster\\";
            if (!Properties.Settings.Default.UseScripts)
            {
                throw new InvalidOperationException("Scripts are disabled in settings.");
            }
        }

        public bool LoadScript(string name, string[] args)
        {
            string fileName = scriptsPath + name + ".cs";
            try
            {
                dynamic script = CSScript.LoadCode(File.ReadAllText(fileName)).CreateObject("*");
                script.Script_Start(args);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            return true;
        }
    }
}
