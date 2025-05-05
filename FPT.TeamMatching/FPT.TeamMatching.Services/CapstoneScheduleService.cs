using AutoMapper;
using FPT.TeamMatching.Domain.Contracts.Services;
using FPT.TeamMatching.Domain.Contracts.UnitOfWorks;
using FPT.TeamMatching.Domain.Entities;
using FPT.TeamMatching.Services.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ExcelDataReader;
using FPT.TeamMatching.Domain.Models.Responses;
using FPT.TeamMatching.Domain.Utilities;
using Microsoft.AspNetCore.Http;
using FPT.TeamMatching.Domain.Models.Requests.Commands.CapstoneSchedules;
using FPT.TeamMatching.Domain.Contracts.Repositories;
using FPT.TeamMatching.Domain.Models;
using FPT.TeamMatching.Domain.Models.Requests.Commands.Notifications;
using FPT.TeamMatching.Domain.Models.Results;

namespace FPT.TeamMatching.Services
{
    class CapStoneReader
    {
        public int STT { get; set; }
        public string IdeaCode { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string HallName { get; set; }
        public string UniqueKey { get; set; }
    }
    public class CapstoneScheduleService : BaseService<CapstoneSchedule>, ICapstoneScheduleService
    {
        private readonly ICapstoneScheduleRepository _capstoneScheduleRepository;
        private  readonly INotificationService _notificationService;
        public CapstoneScheduleService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService) : base(mapper, unitOfWork)
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

                // Unique key := date.slot.room
                
                var listCapstoneSchedule = await _capstoneScheduleRepository.GetAll();
                var existingKeys = listCapstoneSchedule
                    .Select(x => $"{x.Date.Value.AddHours(7):yyyy-MM-dd}.{x.Time.Trim().ToLower()}.{x.HallName.Trim().ToLower()}")
                    .ToHashSet();
                List<CapstoneScheduleExcelModel> failList = new List<CapstoneScheduleExcelModel>();
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
                                var stt = int.Parse(reader.GetValue(0).ToString());
                                var ideaCode = reader.GetValue(1).ToString();
                                var date = reader.GetValue(3).ToString();
                                if (string.IsNullOrWhiteSpace(date))
                                {
                                    failList.Add(new CapstoneScheduleExcelModel
                                    {
                                        STT = stt,
                                        TopicCode = ideaCode,
                                        Reason = "Ngày không hợp lệ"
                                    });
                                    continue;
                                }
                                var time = reader.GetValue(4).ToString();
                                if (string.IsNullOrWhiteSpace(time.Trim()))
                                {
                                    failList.Add(new CapstoneScheduleExcelModel
                                    {
                                        STT = stt,
                                        TopicCode = ideaCode,
                                        Reason = "Thời gian không thể là rỗng"
                                    });
                                    continue;
                                }
                                
                                var regex = new Regex(@"^\s*(\d{2}:\d{2})\s*-\s*(\d{2}:\d{2})\s*$");
                                var match = regex.Match(time);

                                if (!match.Success)
                                {
                                    failList.Add(new CapstoneScheduleExcelModel
                                    {
                                        STT = stt,
                                        TopicCode = ideaCode,
                                        Reason = "Thời gian không hợp lệ. Theo format HH:mm - HH:mm"
                                    });
                                    continue;
                                }
                                
                                var hallname = reader.GetValue(5).ToString();
                                if (string.IsNullOrWhiteSpace(hallname.Trim()))
                                {
                                    failList.Add(new CapstoneScheduleExcelModel
                                    {
                                        STT = stt,
                                        TopicCode = ideaCode,
                                        Reason = "Hội trường không thể là rỗng"
                                    });
                                    continue;
                                }
                                capStoneReaders.Add(new CapStoneReader
                                {
                                    STT = stt,
                                    IdeaCode = ideaCode,
                                    Date = date,
                                    Time = time,
                                    HallName = hallname,
                                    UniqueKey = $"{DateTime.Parse(date):yyyy-MM-dd}.{time.Trim().ToLower()}.{hallname.Trim().ToLower()}"
                                });
                            }
                        } while (reader.NextResult());
                    }
                }

                
                var topicCode = capStoneReaders.Select(x => x.IdeaCode).Distinct().ToArray();
                
                var topics = await _unitOfWork.TopicRepository.GetAllTopicsByTopicCode(topicCode);
                List<CapstoneSchedule> capstones = new List<CapstoneSchedule>();

                //Quy hoach dong
                Dictionary<string, Topic> topicsDic = topics.ToDictionary(x => x.TopicCode, x => x);
                foreach (var capStoneReader in capStoneReaders)
                {
                    // Unique key := date.slot.room
                    if (existingKeys.Contains(capStoneReader.UniqueKey))
                    {
                        failList.Add(new CapstoneScheduleExcelModel
                        {
                            STT = capStoneReader.STT,
                            TopicCode = capStoneReader.IdeaCode,
                            Reason = "Đã tồn tại lịch trong hệ thống"
                        });
                        continue;
                    }
                    if (capStoneReaders.Count(x => x.UniqueKey == capStoneReader.UniqueKey) > 1)
                    {
                        failList.Add(new CapstoneScheduleExcelModel
                        {
                            STT = capStoneReader.STT,
                            TopicCode = capStoneReader.IdeaCode,
                            Reason = "Bị trùng lịch trong file"
                        });    
                        continue;
                    }
                    if (listCapstoneSchedule.Any(x => x.Project.Topic.TopicCode == capStoneReader.IdeaCode && x.Stage == stage))
                    {
                        failList.Add(new CapstoneScheduleExcelModel
                        {
                            STT = capStoneReader.STT,
                            TopicCode = capStoneReader.IdeaCode,
                            Reason = "Lịch của nhóm này đã tồn tại"
                        }); 
                        continue;
                    }
                    if (topicsDic.TryGetValue(capStoneReader.IdeaCode, out Topic topic))
                    {
                        capstones.Add(new CapstoneSchedule
                        {
                            ProjectId = topic.Project.Id,
                            Date = DateTime.Parse(capStoneReader.Date).ToUniversalTime(),
                            Time = capStoneReader.Time,
                            HallName = capStoneReader.HallName,
                            Stage = stage,
                        });
                    }
                    else
                    {
                        failList.Add(new CapstoneScheduleExcelModel
                        {
                            STT = capStoneReader.STT,
                            TopicCode = capStoneReader.IdeaCode,
                            Reason = "Không tìm thấy Topic code"
                        });
                        continue;
                    }
                }
                _unitOfWork.CapstoneScheduleRepository.AddRange(capstones);
                var saveChange = await _unitOfWork.SaveChanges();
                if (!saveChange)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage(Const.FAIL_SAVE_MSG);
                }
                
                List<NotificationCreateForTeam> notifications = new List<NotificationCreateForTeam>();
                foreach (var capstoneSchedule in capstones)
                {
                    notifications.Add(new NotificationCreateForTeam
                    {
                        ProjectId = capstoneSchedule.ProjectId,
                        Description = $"Lịch bảo vệ lần ${stage} của nhóm đã có"
                    });
                }
                
                await _notificationService.CreateMultiNotificationForTeam(notifications);
                
                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG)
                    .WithData(failList);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(e.Message);
            }
        }
        
        public async Task<BusinessResult> AddCapstoneSchedule(CapstoneScheduleCreateCommand command)
        {
            try
            {
                // Unique key := date.slot.room
                
                var uniqueKey =
                    $"{command.Date:yyyy-MM-dd}.{command.Time.Trim().ToLower()}.{command.HallName.Trim().ToLower()}";
                var listCapstoneSchedule = await _capstoneScheduleRepository.GetAll();
                var existingKeys = listCapstoneSchedule
                    .Select(x => $"{x.Date.Value.AddHours(7):yyyy-MM-dd}.{x.Time.Trim().ToLower()}.{x.HallName.Trim().ToLower()}")
                    .ToHashSet();
                if (existingKeys.Contains(uniqueKey))
                {
                    return new ResponseBuilder().WithStatus(Const.FAIL_CODE).WithMessage("Phòng đã nhóm");
                }

                if (listCapstoneSchedule.Any(x => x.ProjectId == command.ProjectId && x.Stage == command.Stage))
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Lịch bảo vệ của nhóm đã tồn tại");
                }

                var entities = _mapper.Map<CapstoneSchedule>(command);
                _capstoneScheduleRepository.Add(entities);
                var saveChanges = await _unitOfWork.SaveChanges();
                if (!saveChanges)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Save change fail");
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }
            catch (Exception e)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(e.Message);
            }
        }

        public async Task<BusinessResult> UpdateCapstoneSchedule(CapstoneScheduleUpdateCommand command)
        {
            try
            {
                var uniqueKey =
                    $"{command.Date:yyyy-MM-dd}.{command.Time.Trim().ToLower()}.{command.HallName.Trim().ToLower()}";
                var listCapstoneSchedule = await _capstoneScheduleRepository.GetAll();
                var currentCapstoneSchedule = listCapstoneSchedule.FirstOrDefault(x => x.ProjectId == command.ProjectId);
                if (currentCapstoneSchedule == null)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Project not found");
                }
                var existingKeys = listCapstoneSchedule
                    .Select(x => $"{x.Date.Value.AddHours(7):yyyy-MM-dd}.{x.Time.Trim().ToLower()}.{x.HallName.Trim().ToLower()}")
                    .ToHashSet();
                if (existingKeys.Contains(uniqueKey))
                {
                    return new ResponseBuilder().WithStatus(Const.FAIL_CODE).WithMessage("Phòng đã nhóm");
                }

                currentCapstoneSchedule.Time = command.Time;
                currentCapstoneSchedule.HallName = command.HallName;
                currentCapstoneSchedule.Date = command.Date;
                _capstoneScheduleRepository.Update(currentCapstoneSchedule);
                var saveChanges = await _unitOfWork.SaveChanges();
                if (!saveChanges)
                {
                    return new ResponseBuilder()
                        .WithStatus(Const.FAIL_CODE)
                        .WithMessage("Save change fail");
                }

                return new ResponseBuilder()
                    .WithStatus(Const.SUCCESS_CODE)
                    .WithMessage(Const.SUCCESS_SAVE_MSG);
            }
            catch (Exception e)
            {
                return new ResponseBuilder()
                    .WithStatus(Const.FAIL_CODE)
                    .WithMessage(e.Message);
            }
        }
    }
}
