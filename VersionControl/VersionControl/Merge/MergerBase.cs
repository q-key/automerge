using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionControl.Merge
{
    public abstract class MergerBase<T>
    {
        protected readonly IComparer<T> Comparer;

        public MergerBase(IComparer<T> comparer)
        {
            this.Comparer = comparer;
        }

        internal void Merge(MergeResult<T> context, List<Command<T>> allCommands)
        {
            //allCommands is a list but not IEnumerable not to allow multiple diff processing

            var groupedCommands = (from c in allCommands
                                   let lineCmd = c as LineCommand<T>
                                   where lineCmd != null

                                   group c by lineCmd.LineNumber into g
                                   orderby g.Key
                                   select new LineAggregator<T>
                                   {
                                       LineNumber = g.Key,
                                       InsertCommands = g.Where(gc => gc is InsertCommand<T>).Cast<InsertCommand<T>>().Distinct(new InsertCommandComparer<T>(Comparer)).ToArray(),
                                       RemoveCommands = g.Where(gc => gc is RemoveCommand<T>).Cast<RemoveCommand<T>>().Distinct(new RemoveCommandComparer<T>()).ToArray()
                                   });



            var orderedGroups = from k in groupedCommands
                                orderby k.LineNumber
                                select k;


            foreach (var lineGroup in orderedGroups)
            {
                //execute only one remove cmd
                //if it has more than 1 insert -> it's a conflict 
                MergeLine(context, lineGroup);
            }

            var appendCommands = allCommands.Where(c => c is AppendLineCommand<T>);

            foreach (var c in appendCommands)
                c.Execute(context);

        }

        protected virtual void MergeLine(MergeResult<T> context, LineAggregator<T> lineGroup)
        {
            var removeCmd = lineGroup.RemoveCommands.SingleOrDefault();
            if (removeCmd != null)
                removeCmd.Execute(context);

            var insertCommands = lineGroup.InsertCommands;
            var realChangeCommands = insertCommands.Where(c => !IsEmptyValue(c.Value)).ToArray();
            //if compare an empty line and a change -> select the change

            if (realChangeCommands.Length > 1)
            {
                var values = realChangeCommands.OrderBy(c => c.Version).Select(c => c.Version + " : " + c.Value).ToArray();
                var cmd = new InsertCommand<T>("", lineGroup.LineNumber, default(T))
                {
                    Message = "Conflict line:" + lineGroup.LineNumber + " " + string.Join(" --- OR --- ", values)
                };

                cmd.Execute(context);

                context.Conflicts.Add(new Conflict
                {
                    Versions = values
                });
            }
            else
            {
                //if it has a change command - ignore all the empty-lined commands
                var change = realChangeCommands.FirstOrDefault();
                if (change != null)
                    change.Execute(context);
                else
                {
                    //ignore multiple empty-lined commands
                    var emptyCommand = insertCommands.FirstOrDefault(c => IsEmptyValue(c.Value));
                    if (emptyCommand != null)
                        emptyCommand.Execute(context);
                }
            }
        }

        abstract protected bool IsEmptyValue(T value);

    }
}
