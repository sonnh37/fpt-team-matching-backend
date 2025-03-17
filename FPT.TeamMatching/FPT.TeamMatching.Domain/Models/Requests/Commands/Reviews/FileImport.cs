using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.TeamMatching.Domain.Models.Requests.Commands.Reviews
{
    public class FileImport
    {
        public IFormFile? file {  get; set; }
        public int reviewNumber { get; set; }
    }
}
