using Models.Dto.PostPutModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Mapper
{
    public static class SafeMapper
    {
        public static void MapUserFromUserDto(UserDto userDto, User user)
        {
            user.Otch = userDto.Otch;
            user.First_name = userDto.First_name;
            user.Last_name = userDto.Last_name;
            user.BirthDate = userDto.BirthDate;
            user.PhoneNumber = userDto.PhoneNumber;
            user.PassportNumber = userDto.PassportNumber;
            user.PassportSeries = userDto.PassportSeries;
            user.Account!.Roles = userDto.Roles;
            user.Account!.Password = userDto.Password;
            user.Account!.Login = userDto.Login;
            user.Account!.Email = userDto.Email;
        }



        public static void MapUserFromUserDto(UserUpdateDto userDto, User user)
        {
            user.Otch = userDto.Otch;
            user.First_name = userDto.First_name;
            user.Last_name = userDto.Last_name;
            user.BirthDate = userDto.BirthDate;
            user.PhoneNumber = userDto.PhoneNumber;
            user.PassportNumber = userDto.PassportNumber;
            user.PassportSeries = userDto.PassportSeries;
            user.Account!.Roles = userDto.Roles;
            user.Account!.Login = userDto.Login;
            user.Account!.Email = userDto.Email;
        }

        public static void MapUserFromUserDto(RegModel model, User user)
        {
            user.Otch = model.Otch;
            user.First_name = model.First_name;
            user.Last_name = model.Last_name;
            user.BirthDate = model.BirthDate;
            user.PhoneNumber = model.PhoneNumber;
            user.PassportNumber = model.PassportNumber;
            user.PassportSeries = model.PassportSeries;
            user.Account!.Password = model.Password;
            user.Account!.Login = model.Login;
            user.Account!.Email = model.Email;
        }

        public static void MapUserFromUserDto(UpdateModel model, User user)
        {
            user.Otch = model.Otch;
            user.First_name = model.First_name;
            user.Last_name = model.Last_name;
            user.BirthDate = model.BirthDate;
            user.PhoneNumber = model.PhoneNumber;
            user.PassportNumber = model.PassportNumber;
            user.PassportSeries = model.PassportSeries;
            user.Account!.Login = model.Login;
            user.Account!.Email = model.Email;
        }


        public static void MapEquipmentFromEquipmentDto(EquipmentDto dto, Equipment equipment)
        {
            equipment.Status = dto.Status;
            equipment.EquipmentCode = dto.EquipmentCode;
            equipment.Date = dto.Date;
            equipment.Deleted = dto.Deleted;
        }

        public static void MapTechnicalTaskFromTechnicalTaskDto(TechnicalTaskDto dto, TechnicalTask task)
        {
            task.Date = dto.Date;
            task.Content = dto.Content;
            task.NameEquipment = dto.NameEquipment;
        }

        public static void MapServiceFromServiceDto(ServiceDto dto, Service service)
        {
            service.ServiceType = dto.ServiceType;
            service.Status = dto.Status;
            service.WorkContent = dto.WorkContent;
            service.Sum = dto.Sum;
            service.Date = dto.Date;
            service.Deleted = dto.Deleted;
        }

        public static void MapTechnicalTestFromDto(TechnicalTestDto dto, TechnicalTest test)
        {
            test.TestData = dto.TestData;
            test.Description = dto.Description;
            test.Date = dto.Date;
            test.TestPriority = dto.TestPriority;
            test.Comment = dto.Comment;
            test.Deleted = dto.Deleted;
            test.ExpectedConclusion = dto.ExpectedConclusion;
            test.FactCoclusion = dto.FactCoclusion;
        }
    }
}
