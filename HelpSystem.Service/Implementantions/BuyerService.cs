using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelpSystem.DAL.Interfasces;
using HelpSystem.Domain.Entity;
using HelpSystem.Domain.Enum;
using HelpSystem.Domain.Response;
using HelpSystem.Domain.ViewModel.Buyer;
using HelpSystem.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpSystem.Service.Implementantions
{
    public class BuyerService : IBuyerService
    {
        private readonly IBaseRepository<Buyer> _buyerRepository;
        public BuyerService(IBaseRepository<Buyer> buyerRepository)
        {
            _buyerRepository = buyerRepository;
        }

        public async Task<BaseResponse<Buyer>> CreateBuyer(BuyerViewModel model)
        {
            try
            {
                var Buyer = await _buyerRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Name == model.BuyerName);
                if (Buyer == null)
                {
                    var NewBuyer = new Buyer()
                    {
                        Name = model.BuyerName
                    };
                    await _buyerRepository.Create(NewBuyer);
                    return new BaseResponse<Buyer>()
                    {
                        Data = NewBuyer,
                        StatusCode = StatusCode.Ok,
                        Description = "Покупатель успешно добавлен"
                    };

                }

                return new BaseResponse<Buyer>()
                {
                    Description = "Покупатель с таким наименованием уже существует",
                    StatusCode = StatusCode.UnCreated
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Buyer>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<Buyer>> UpdateBuyer(BuyerViewModel model)
        {
            try
            {
                var UpdateBuyer = await _buyerRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (UpdateBuyer != null)
                {
                    if (UpdateBuyer.Name != model.BuyerName)
                    {
                        UpdateBuyer.Name = model.BuyerName;
                        await _buyerRepository.Update(UpdateBuyer);
                        return new BaseResponse<Buyer>()
                        {
                            Data = UpdateBuyer,
                            Description = "Данные обновлены",
                            StatusCode = StatusCode.Ok,
                        };
                    }
                    
                }

                return new BaseResponse<Buyer>()
                {
                    Description = "Покупатель не найден",
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Buyer>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
           

        }

        public async Task<BaseResponse<Buyer>> DeleteBuyer(Guid id)
        {
            try
            {
                var Del = await _buyerRepository.GetAll()
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (Del != null)
                {
                    await _buyerRepository.Delete(Del);
                    return new BaseResponse<Buyer>()
                    {
                        Description = "Покупатель удалён",
                        StatusCode = StatusCode.Ok
                    };
                }

                return new BaseResponse<Buyer>()
                {
                    Description = "Покупатель не найдён для удаления",
                    StatusCode = StatusCode.NotFind
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Buyer>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<BuyerViewModel>>> GetAllBuyer()
        {
            try
            {
                var AllBuyer = await _buyerRepository.GetAll()
                    .Select(x => new BuyerViewModel()
                    {
                        BuyerName = x.Name
                    })
                    .ToListAsync();
                if (AllBuyer == null)
                {
                    return new BaseResponse<IEnumerable<BuyerViewModel>>()
                    {
                        Description = "Список покупателей пуст",
                        StatusCode = StatusCode.NotFind
                    };
                }

                return new BaseResponse<IEnumerable<BuyerViewModel>>()
                {
                    Data = AllBuyer,
                    StatusCode = StatusCode.Ok
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<IEnumerable<BuyerViewModel>>()
                {
                    Description = $"{ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
