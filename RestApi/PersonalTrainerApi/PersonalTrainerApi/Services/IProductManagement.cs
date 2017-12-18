using PersonalTrainerApi.Model.Dto.Product;
using System;
using System.Collections.Generic;

namespace PersonalTrainerApi.Services
{
    public interface IProductManagement
    {
        /// <summary>
        /// Rejestruje dzień żywieniowy dla podanego dnia.
        /// </summary>
        /// <param name="date">Data dnia rejestrującego produkty.</param>
        /// <param name="food">Lista produktów.</param>
        void SubmitDailyFood(DateTime date, IEnumerable<DailyFoodProductDto> food);

        DailyFoodDto GetDailyFood(Guid userId, DateTime date);

        DailyFoodDto GetDailyFoodFromDailyFoodProductDto(DateTime date, IEnumerable<DailyFoodProductDto> dto);

        /// <summary>
        /// Dodaje produkt do bazy.
        /// </summary>
        /// <param name="dto">Dto produktu. <see cref="ProductDto"/></param>
        void AddProduct(ProductDto dto);

        /// <summary>
        /// Aktualizuje wybrany produkt w bazie danych.
        /// </summary>
        /// <param name="productId">Id produktu.</param>
        /// <param name="dto">Dto produktu. <see cref="ProductDto"/></param>
        void UpdateProduct(ProductDto dto);

        /// <summary>
        /// Usuwa produkt z bazy.
        /// </summary>
        /// <param name="productId">Id produktu.</param>
        void RemoveProduct(Guid productId);

        /// <summary>
        /// Pobiera produkt po jego indetyfikatorze
        /// </summary>
        /// <param name="productId">Identyfikator produktu</param>
        /// <returns></returns>
        ProductDto GetProduct(Guid productId);

        /// <summary>
        /// Zwraca listę wszystkich produktów w systemie
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductDto> GetProducts();

        /// <summary>
        /// Pozyskuje listę wszystkich produktów dodanych przez użytkownika
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductDto> GetUserProducts(Guid userId);

        /// <summary>
        /// Pozyskuje listę wszystkich produktów oczekujących na zatwierdzenie przez administratora systemu.
        /// </summary>
        /// <returns></returns>
        IEnumerable<ProductDto> GetPendingSubscribeProducts();

        /// <summary>
        /// Zasubskrypuj produkt
        /// </summary>
        /// <param name="productId"></param>
        void SubscribeProduct(Guid productId);

        /// <summary>
        /// Potwierdź subskrypcje produktu [Admin]
        /// </summary>
        void AcceptSubscription(Guid productId);

        /// <summary>
        /// Odrzuca subskrypcję [Admin]
        /// </summary>
        /// <param name="productId"></param>
        void DeclineSubscription(Guid productId);

        /// <summary>
        /// Anuluje subskrypcję
        /// </summary>
        /// <param name="productId"></param>
        void CancelSubscription(Guid productId);
    }
}
