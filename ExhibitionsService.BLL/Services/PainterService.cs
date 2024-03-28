using AutoMapper;
using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;

namespace ExhibitionsService.BLL.Services
{
    public class PainterService : IPainterService
    {
        private readonly IUnitOfWork uow;
        private readonly IMapper mapper;

        public PainterService(IUnitOfWork _uow, IMapper _mapper)
        {
            uow = _uow;
            mapper = _mapper;
        }

        public async Task CreateAsync(PainterDTO entity)
        {
            await uow.Painters.CreateAsync(mapper.Map<Painter>(entity));
            await uow.SaveAsync();
        }

        public async Task<List<PainterDTO>> GetAllAsync()
        {
            return mapper.Map<List<PainterDTO>>((await uow.Painters.GetAllAsync()).ToList());
        }

        public void Dispose()
        {
            uow.Dispose();
        }
    }
}
