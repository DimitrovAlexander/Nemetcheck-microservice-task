using OperationalMicroservice.Data.DTOs;

namespace OperationalMicroservice.Services.DiceService
{

    public interface IDiceService
    {
        Task<RollResponseDTO> RollAsync(Guid userId);
        Task<PagedResult<RollResponseDTO>> GetHistoryAsync(Guid userId, HistoryQueryParams query);
    }

}
