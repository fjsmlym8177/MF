using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Pipeline
{
    public interface IPipelineContext
    {
        void AddRange<T>(IList<T> list, string pipelineName);
        void AddRange<T>(IList<T> list, Func<T, string> pipelineName);

        void ExecutePipeline<T>(IList<string> pipelineNames, Func<IList<PipelineItem<T>>, string> func);
    }
}
