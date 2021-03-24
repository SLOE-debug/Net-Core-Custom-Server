using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom_Server
{
    public class PipeEntity
    {
        public Func<HttpContext, PipeEntity, Task> Action { get; set; }
        public PipeEntity Next { get; set; }
    }
}
