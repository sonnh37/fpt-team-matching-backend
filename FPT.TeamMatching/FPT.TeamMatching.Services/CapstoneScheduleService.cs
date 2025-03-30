using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;

namespace FPT.TeamMatching.Services
{
    class CapStoneReader
    {
        public string IdeaCode {get; set;}
        public string Date {get; set;}
        public string Time {get; set;}
        public string HallName {get; set;}
    }
    public class CapstoneScheduleService : BaseService<CapstoneSchedule>, ICapstoneScheduleService
    {
        public CapstoneScheduleService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
          
        }

        public async Task<BusinessResult> ImportExcelFile(IFormFile file, int stage)
        {
            try
            {
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                if (file == null || file.Length == 0)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("No file uploaded!");
                }
                
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\UploadFiles";

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.Name);

                List<CapStoneReader> capStoneReaders = new List<CapStoneReader>();
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                    
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        // skip 2 dòng tiêu đề
                        reader.Read();
                        reader.Read();
                        do
                        {
                            while (reader.Read())
                            {
                                var ideaCode = reader.GetValue(1).ToString();
                                var date = reader.GetValue(3).ToString();
                                var time = reader.GetValue(4).ToString();
                                var hallname = reader.GetValue(5).ToString();
                                capStoneReaders.Add(new CapStoneReader
                                {
                                    IdeaCode = ideaCode,
                                    Date = date,
                                    Time = time,
                                    HallName = hallname,
                                });
                            }
                        } while (reader.NextResult());
                    }
                }
                
                var ideaCodes = capStoneReaders.Select(x => x.IdeaCode).Distinct().ToArray();

                var ideas = await _unitOfWork.IdeaRepository.GetIdeasByIdeaCodes(ideaCodes);
                List<CapstoneSchedule> capstones = new List<CapstoneSchedule>();
                
                
                // Quy hoạch động 
                Dictionary<string, CapStoneReader> readerDict = capStoneReaders
                    .ToDictionary(x => x.IdeaCode, x => x);
                foreach (var idea in ideas)
                {
                    if (readerDict.TryGetValue(idea.IdeaCode, out CapStoneReader reader))
                    {
                        capstones.Add(new CapstoneSchedule
                        {
                            ProjectId = idea.Project.Id,
                            Date = DateTime.Parse(reader.Date).ToUniversalTime(),
                            Time = reader.Time,
                            HallName = reader.HallName,
                            Stage = stage,
                        });
                    }
                }

                _unitOfWork.CapstoneScheduleRepository.AddRange(capstones);
                await _unitOfWork.SaveChanges();
                
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG); 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(e.Message);
            }
        }
    }
}
