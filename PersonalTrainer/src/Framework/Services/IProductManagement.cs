using Framework.Models.Dto;
using System;
using System.Collections.Generic;

namespace Framework.Services
{
    public interface IProductManagement
    {
        /// <summary>
        /// Rejestruje dzień żywieniowy dla podanego dnia.
        /// </summary>
        /// <param name="date">Data dnia rejestrującego produkty.</param>
        /// <param name="food">Lista produktów.</param>
        void SubmitDailyFood(DateTime date, IEnumerable<DailyFoodProductDto> food);

        DailyFoodDto GetDailyFood(DateTime date);

        DailyFoodDto GetDailyFoodFromDailyFoodProductDto(DateTime date, IEnumerable<DailyFoodProductDto> dto);

        ProductDto GetProduct(Guid productId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductDto> GetProducts();

        /// <summary>
        /// Pozyskuje listę wszystkich produktów dodanych przez użytkownika
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductDto> GetUserProducts();

        /// <summary>
        /// Pozyskuje listę wszystkich produktów oczekujących na zatwierdzenie przez administratora systemu.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductDto> GetPendingSubscribeProducts();
    }
}
