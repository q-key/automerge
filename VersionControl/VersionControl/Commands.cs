using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl
{
    public abstract class Command<T>
    {
        public string Version;
        abstract public void Execute(MergeResult<T> c);

        public Command(string version)
        {
            this.Version = version;
        }
    }


    public abstract class LineCommand<T> : Command<T>
    {
        public int LineNumber;
        public string Message;

        public LineCommand(string version, int lineNmber)
            : base(version)
        {
            this.LineNumber = lineNmber;
        }
    }


    public class InsertCommand<T> : LineCommand<T>
    {
        public T Value;

        public InsertCommand(string version, int lineNmber, T value)
            : base(version, lineNmber)
        {
            this.Value = value;
        }

        public override void Execute(MergeResult<T> c)
        {
            var n = LineNumber + c.Offset + 1;
            c.Lines.Insert(n, Value);

            c.Offset++;
        }
    }


    public class AppendLineCommand<T> : Command<T>
    {
        public T Value;

        public AppendLineCommand(string version, T value)
            : base(version)
        {
            this.Value = value;
        }

        public override void Execute(MergeResult<T> c)
        {
            c.Lines.Add(Value);
        }
    }

    public class RemoveCommand<T> : LineCommand<T>
    {
        public RemoveCommand(string version, int lineNmber)
            : base(version, lineNmber)
        {
        }

        public override void Execute(MergeResult<T> c)
        {
            c.Lines.RemoveAt(LineNumber + c.Offset);
            c.Offset--;
        }
    }


}
