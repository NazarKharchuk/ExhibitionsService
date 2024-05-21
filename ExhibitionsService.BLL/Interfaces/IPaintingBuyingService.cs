namespace ExhibitionsService.BLL.Interfaces
{
    public interface IPaintingBuyingService
    {
        Task<string> BuyPainting(int paintingId, string requestURL);
        Task ProcessSuccessfulBuying(string sessionId);
    }
}
