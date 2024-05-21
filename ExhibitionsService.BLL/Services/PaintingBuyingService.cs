using ExhibitionsService.BLL.DTO;
using ExhibitionsService.BLL.Infrastructure.Exceptions;
using ExhibitionsService.BLL.Interfaces;
using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace ExhibitionsService.BLL.Services
{
    public class PaintingBuyingService : IPaintingBuyingService
    {
        private readonly IUnitOfWork uow;
        private readonly SessionService sessionService;

        public PaintingBuyingService(IUnitOfWork _uow)
        {
            uow = _uow;
            sessionService = new SessionService();
        }

        public async Task<string> BuyPainting(int paintingId, string requestURL)
        {
            Painting? existingEntity = await uow.Paintings.GetByIdAsync(paintingId);
            if (existingEntity == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, paintingId);

            if (!(existingEntity.IsSold is not null && existingEntity.IsSold == false))
                throw new ArgumentException("Цю картину не можна купити");

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = existingEntity.Name,
                                Description = existingEntity.Description,
                                //Images = new List<string> { $"https://localhost:44350//{existingEntity.ImagePath}" }
                            },
                            UnitAmount = (long)(existingEntity.Price * 100)
                        },
                        Quantity = 1
                    }
                },
                SuccessUrl = $"{requestURL}/payment/success?sessionId={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{requestURL}/paintings/{existingEntity.PaintingId}",
                Metadata = new Dictionary<string, string>
                {
                    { "paintingId", paintingId.ToString() }
                },
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Id;
        }

        public async Task ProcessSuccessfulBuying(string sessionId)
        {
            try
            {
                var session = await sessionService.GetAsync(sessionId);

                if (session.PaymentStatus == "paid")
                {
                    if (session.Metadata.TryGetValue("paintingId", out string paintingIdString) &&
                        int.TryParse(paintingIdString, out int paintingId))
                    {
                        Painting painting = await uow.Paintings.GetByIdAsync(paintingId);
                        if (painting == null) throw new EntityNotFoundException(typeof(PaintingDTO).Name, paintingId);

                        painting.IsSold = true;

                        await uow.Paintings.UpdateAsync(painting);
                        await uow.SaveAsync();
                    }
                    else
                    {
                        throw new ArgumentException($"Відсутні або невалідні значення метадати у сесії");
                    }
                }
                else
                {
                    throw new ArgumentException($"Оплата для сесії ({sessionId}) не була успішною");
                }
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Stripe API помилка: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка під час обробки оплати {sessionId}: {ex.Message}");
                throw;
            }
        }
    }
}
