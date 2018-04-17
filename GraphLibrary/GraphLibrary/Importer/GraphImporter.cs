using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Importer
{
    abstract class GraphImporter
    {
        protected string FFilePath;

        protected abstract IGraph ParseGraphFile();

        public IGraph ImportGraph(string _FilePath)
        {
            FFilePath = _FilePath;
            return ParseGraphFile();
        }

        
    }
}
