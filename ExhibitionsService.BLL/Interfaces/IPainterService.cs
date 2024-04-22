﻿using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.DTO.HelperDTO;

namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPainterService : IService<PainterDTO>
    {
        Task CreateAsync(PainterDTO entity);
        Task<PainterDTO> UpdateAsync(PainterDTO entity);
        Task DeleteAsync(int id);
        Task<PainterDTO?> GetByIdAsync(int id);
        Task<List<PainterDTO>> GetAllAsync();
        Task<PainterInfoDTO> GetByIdWithInfoAsync(int id);
        Task<Tuple<List<PainterInfoDTO>, int>> GetPagePainterInfoAsync(PaginationRequestDTO pagination);
    }
}
