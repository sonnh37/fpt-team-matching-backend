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
using FPT.TeamMatching.Domain.Models.Requests.Commands.CapstoneSchedules;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Services
{
    class CapStoneReader
    {
        public string IdeaCode { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string HallName { get; set; }
    }
    public class CapstoneScheduleService : BaseService<CapstoneSchedule>, ICapstoneScheduleService
    {
        private readonly ICapstoneScheduleRepository _capstoneScheduleRepository;
        public CapstoneScheduleService(IMapper mapper, IUnitOfWork unitOfWork) : base(mapper, unitOfWork)
        {
            _capstoneScheduleRepository = _unitOfWork.CapstoneScheduleRepository;
        }

        public async Task<BusinessResult> GetByProjectId(Guid projectId)
        {
            try
            {
                var capstoneSchedules = await _capstoneScheduleRepository.GetByProjectId(projectId);
                if (capstoneSchedules.Count == 0)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
                }
                return new ResponseBuilder()
                    .WithData(_mapper.Map<List<CapstoneScheduleResult>>(capstoneSchedules))
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }
            catch (Exception e)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(e.Message);
            }
        }

        public async Task<BusinessResult> GetBySemesterIdAndStage(CapstoneScheduleFilter command)
        {
            try
            {
                var capstoneSchedules = await _capstoneScheduleRepository.GetBySemesterIdAndStage(command.SemesterId, command.Stage);
                if (capstoneSchedules.Count == 0)
                {
                    return new ResponseBuilder()
                    .WithStatus(Const.NOT_FOUND_CODE)
                    .WithMessage(Const.NOT_FOUND_MSG);
                }
                return new ResponseBuilder()
                    .WithData(_mapper.Map<List<CapstoneScheduleResult>>(capstoneSchedules))
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_READ_MSG);
            }
            catch (Exception e)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(e.Message);
            }
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

                var uploadsFolder = $"{Directory.GetCurrentDirectory()}/UploadFiles";

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
                                var ideaCodeRaw = reader.GetValue(1);
                                if (ideaCodeRaw == null || string.IsNullOrWhiteSpace(ideaCodeRaw.ToString()))
                                    break;
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
                    //sua db
                    //if (readerDict.TryGetValue(idea.Topic.TopicCode, out CapStoneReader reader))
                    //    {
                    //    capstones.Add(new CapstoneSchedule
                    //    {
                    //        //ProjectId = idea.Project.Id,
                    //        ProjectId = idea.Topic.Project.Id,
                    //        Date = DateTime.Parse(reader.Date).ToUniversalTime(),
                    //        Time = reader.Time,
                    //        HallName = reader.HallName,
                    //        Stage = stage,
                    //    });
                    //}
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
