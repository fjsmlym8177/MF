using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MF.Core.Pipeline
{
    public class PipelineItem<T>
    {
        public PipelineItem(T data)
        {
            Id = Guid.NewGuid();
            Data = data;
        }

        public T Data { get; set; }

        public Guid Id { get; set; }

    }
}
