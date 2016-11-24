using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lynicon.Tools
{
    public class FileModel
    {
        List<string> lines = null;
        string path = null;
        int lineNum = 0;

        public FileModel(string path)
        {
            lines = new List<string>();
            this.path = path;
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                    lines.Add(reader.ReadLine());
            }
        }

        public void Write()
        {
            using (var writer = new StreamWriter(path, false))
            {
                foreach (string line in lines)
                    writer.WriteLine(line);
            }
        }

        public void ToTop()
        {
            lineNum = 0;
        }

        public bool FindLineContains(string contains)
        {
            lineNum = lines.Skip(lineNum).IndexOfPredicate(l => l.Contains(contains)) + lineNum;
            if (lineNum < 0)
                lineNum = lines.Count;
            return lineNum < lines.Count;
        }

        public bool FindLineIs(string lineIs)
        {
            lineNum = lines.Skip(lineNum).IndexOfPredicate(l => l.Trim() == lineIs) + lineNum;
            if (lineNum < 0)
                lineNum = lines.Count;
            return lineNum < lines.Count;
        }

        public void InsertUniqueLineWithIndent(string line, bool useIndentAfter = false)
        {
            if (lines.Any(l => l.Contains(line)))
                return;
            InsertLineWithIndent(line, useIndentAfter: useIndentAfter);
        }

        public void InsertLineWithIndent(string line, bool useIndentAfter = false)
        {
            string indent = "";
            if (!useIndentAfter && lineNum > 0)
                indent = new string(lines[lineNum - 1].TakeWhile(c => char.IsWhiteSpace(c)).ToArray());
            else if (useIndentAfter && lineNum < lines.Count - 1)
                indent = new string(lines[lineNum + 1].TakeWhile(c => char.IsWhiteSpace(c)).ToArray());
            lineNum++;
            lines.Insert(lineNum, indent + line);
        }
    }
}
