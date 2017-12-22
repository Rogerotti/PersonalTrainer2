using PersonalTrainerApi.Extensions;
using PersonalTrainerApi.Model.Database.Context;
using PersonalTrainerApi.Model.Database.Entity;
using PersonalTrainerApi.Model.Dto.Product;
using PersonalTrainerApi.Model.Types;
using PersonalTrainerApi.Services.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonalTrainerApi.Services.Products
{
    public class ProductManagement : IProductManagement
    {
        private readonly DefaultContext context;
        private readonly IUserManagement userManagement;

        public ProductManagement(DefaultContext context,
            IUserManagement userManagement)
        {
            this.context = context;
            this.userManagement = userManagement;
        }

        public void SubmitDailyFood(DateTime date, IEnumerable<DailyFoodProductDto> food)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var guid = userManagement.GetCurrentUserId();

                var day = context.DailyFood.Where(x => x.UserId.Equals(guid) &&
                    x.Date.Year == date.Year &&
                    x.Date.Month == date.Month &&
                    x.Date.Day == date.Day)
                    .FirstOrDefault();

                List<DiaryProduct> foodList = new List<DiaryProduct>();

                Guid dailyFoodId;
                if (day != null)
                    dailyFoodId = day.DayId;
                else
                    dailyFoodId = Guid.NewGuid();

                foreach (var item in food)
                {
                    foodList.Add(new DiaryProduct()
                    {
                        DiaryProductId = Guid.NewGuid(),
                        Quantity = item.ProductQuantity,
                        ProductId = item.ProductId,
                        MealType = (Int32)item.MealType,
                        DayId = dailyFoodId,
                        Day = day
                    });
                }
                var productsIds = foodList.Select(x => x.ProductId).ToList();
                var productDetails = context.ProductsDetails.Where(pd => !productsIds.Any(p => p == pd.ProductId));

                var TotalCalories = productDetails.Sum(x => x.Calories);
                var TotalFat = productDetails.Sum(x => x.Fat);
                var TotalProteins = productDetails.Sum(x => x.Protein);
                var TotalCarbohydrates = productDetails.Sum(x => x.Carbohydrates);

                if (day == null)
                {
                    context.DailyFood.Add(new DayFoodDiary()
                    {
                        Date = date,
                        UserId = guid,
                        DayId = dailyFoodId,
                        TotalCalories = TotalCalories,
                        TotalFat = TotalFat,
                        TotalProteins = TotalProteins,
                        TotalCarbohydrates = TotalCarbohydrates
                    });
                }

                var foods = context.DiaryProducts.Where(x => x.DayId.Equals(dailyFoodId));
                foreach (var item in foods)
                    context.DiaryProducts.Remove(item);
                context.SaveChanges();
                context.DiaryProducts.AddRange(foodList);
                context.SaveChanges();
                trans.Commit();
            }
        }

        public DailyFoodDto GetDailyFood(Guid userId, DateTime date)
        {

            var guid = userId;
            var test = context.DailyFood.Where(d => d.Date.Year == date.Year
                             && d.Date.Month == date.Month
                             && d.Date.Day == date.Day
                             && d.UserId.Equals(guid)).FirstOrDefault();



            var result = (from d in context.DailyFood
                          join dp in context.DiaryProducts on d.DayId equals dp.DayId
                          join p in context.Product on dp.ProductId equals p.ProductId
                          join pd in context.ProductsDetails on p.ProductId equals pd.ProductId
                          where d.Date.Year == date.Year
                          && d.Date.Month == date.Month
                          && d.Date.Day == date.Day
                          && d.UserId.Equals(guid)
                          select new
                          {
                              d,
                              dp,
                              p,
                              pd
                          });


            var dailyFood = result.Select(x => x.d).FirstOrDefault();

            if (dailyFood != null)
            {
                List<DailyProductDto> daily = new List<DailyProductDto>();
                var foodTypes = result.Select(x => x.dp).ToList();
                var products = result.Select(x => x.p).ToList();
                foreach (var item in result.Select(x => x.dp).ToList())
                {

                    var prod = products.First(x => x.ProductId.Equals(item.ProductId));
                    var pd = result.Select(x => x.pd);
                    var productDetails = pd.Where(x => x.ProductId.Equals(item.ProductId)).FirstOrDefault();
                    var a = new ProductDto()
                    {
                        Name = item.Product.Name,
                        Manufacturer = item.Product.Manufacturer,
                        ProductId = item.ProductId,
                        UserId = item.Product.UserId,
                        Macro = new Macro()
                        {
                            Quantity = productDetails.Quantity,
                            Calories = productDetails.Calories,
                            Carbohydrates = productDetails.Carbohydrates,
                            Fat = productDetails.Fat,
                            Protein = productDetails.Protein,
                            QuantityType = GetQuantityTypeEnum(productDetails.QuantityType)
                        },
                        Type = GetProductTypeEnum(item.Product.ProductType),
                        TypeDisplayName = GetProductTypeEnum(item.Product.ProductType).GetDisplayName(),
                        State = GetProductStateEnum(item.Product.ProductState)

                    };
                    var food = foodTypes.FirstOrDefault(x => x.ProductId.Equals(item.ProductId));
                    var ration = (decimal)food.Quantity / productDetails.Quantity;
                    var currentQuant = food.Quantity;
                    var currentCalories = Math.Round(productDetails.Calories * ration, 2);
                    var currentFat = Math.Round(productDetails.Fat * ration, 2);
                    var currentProtein = Math.Round(productDetails.Protein * ration, 2);
                    var currentCarbohydrates = Math.Round(productDetails.Carbohydrates * ration, 2);

                    daily.Add(new DailyProductDto()
                    {
                        Product = a,
                        CurrentMacro = new Macro()
                        {
                            Quantity = currentQuant,
                            Calories = currentCalories,
                            Carbohydrates = currentCarbohydrates,
                            Fat = currentFat,
                            Protein = currentProtein,
                            QuantityType = GetQuantityTypeEnum(productDetails.QuantityType)
                        },
                        MealType = (MealType)item.MealType
                    });
                }

                return new DailyFoodDto()
                {
                    Day = date,
                    DayCalories = 0,
                    DayProteins = 0,
                    DayCarbohydrates = 0,
                    DayFat = 0,
                    DailyProduct = daily
                };
            }

            return new DailyFoodDto()
            {
                Day = date,
                DayCalories = 0,
                DayProteins = 0,
                DayCarbohydrates = 0,
                DayFat = 0,
                DailyProduct = new List<DailyProductDto>()
            };
        }

        /// <summary>
        /// Dodaje produkt
        /// </summary>
        /// <param name="dto"><see cref="ProductDto"/></param>
        /// <returns></returns>
        public Guid AddProduct(ProductDto dto)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                if (dto == null) throw new ArgumentNullException(nameof(dto));

                var productId = Guid.NewGuid();
                Int32 productType = GetProductTypeValue(dto.Type);
                Int32 productState = GetProductStateValue(dto.State);

                context.Product.Add(new Product()
                {
                    ProductId = productId,
                    UserId = dto.UserId,
                    Name = dto.Name,
                    Manufacturer = dto.Manufacturer,
                    ProductType = productType,
                    ProductState = productState

                });

                var quantityType = GetQuantityTypeValue(dto.Macro.QuantityType);

                context.ProductsDetails.Add(new ProductDetails()
                {
                    ProductId = productId,
                    Protein = dto.Macro.Protein,
                    Fat = dto.Macro.Fat,
                    Carbohydrates = dto.Macro.Carbohydrates,
                    Calories = dto.Macro.Calories,
                    Quantity = dto.Macro.Quantity,
                    QuantityType = quantityType
                });

                context.SaveChanges();
                trans.Commit();
                return productId;
            }
        }

        public void RemoveProduct(Guid productId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var p = context.Product.FirstOrDefault(x => x.ProductId.Equals(productId));
                var pd = context.ProductsDetails.FirstOrDefault(x => x.ProductId.Equals(productId));

                var diaryProducts = context.DiaryProducts.Where(x => x.ProductId.Equals(productId)).ToList();
                foreach (var item in diaryProducts)
                {
                    context.DiaryProducts.Remove(item);
                }

                context.ProductsDetails.Remove(pd);
                context.Product.Remove(p);

                context.SaveChanges();
                trans.Commit();
            }
        }

        public void UpdateProduct(ProductDto dto)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var p = context.Product.FirstOrDefault(x => x.ProductId.Equals(dto.ProductId));
                var pd = context.ProductsDetails.FirstOrDefault(x => x.ProductId.Equals(dto.ProductId));

                p.Name = dto.Name;
                p.Manufacturer = dto.Manufacturer;
                p.ProductType = GetProductTypeValue(dto.Type);

                pd.Calories = dto.Macro.Calories;
                pd.Fat = dto.Macro.Fat;
                pd.Protein = dto.Macro.Protein;
                pd.Carbohydrates = dto.Macro.Carbohydrates;
                pd.Quantity = dto.Macro.Quantity;
                pd.QuantityType = GetQuantityTypeValue(dto.Macro.QuantityType);

                context.SaveChanges();
                trans.Commit();
            }
        }

        public void AcceptSubscription(Guid productId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var p = context.Product.FirstOrDefault(x => x.ProductId.Equals(productId));
                p.ProductState = 3;
                context.SaveChanges();
                trans.Commit();
            }
        }

        public void DeclineSubscription(Guid productId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var p = context.Product.FirstOrDefault(x => x.ProductId.Equals(productId));
                p.ProductState = 2;
                context.SaveChanges();
                trans.Commit();
            }
        }

        public void SubscribeProduct(Guid productId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var p = context.Product.FirstOrDefault(x => x.ProductId.Equals(productId));
                p.ProductState = 1;
                context.SaveChanges();
                trans.Commit();
            }
        }

        public void CancelSubscription(Guid productId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var p = context.Product.FirstOrDefault(x => x.ProductId.Equals(productId));
                p.ProductState = 0;
                context.SaveChanges();
                trans.Commit();
            }
        }


        /// <summary>
        /// Pozyskuje produkt o podanym id.
        /// </summary>
        /// <param name="productId">Id produktu.</param>
        /// <returns></returns>
        public ProductDto GetProduct(Guid productId)
        {
            using (var trans = context.Database.BeginTransaction())
            {
                var p = context.Product.FirstOrDefault(x => x.ProductId.Equals(productId));
                var pd = context.ProductsDetails.FirstOrDefault(x => x.ProductId.Equals(productId));

                return new ProductDto()
                {
                    Name = p.Name,
                    Manufacturer = p.Manufacturer,
                    ProductId = p.ProductId,
                    UserId = p.UserId,
                    State = GetProductStateEnum(p.ProductState),
                    Type = GetProductTypeEnum(p.ProductType),
                    TypeDisplayName = GetProductTypeEnum(p.ProductType).GetDisplayName(),
                    Macro = new Macro()
                    {
                        Calories = pd.Calories,
                        Fat = pd.Fat,
                        Carbohydrates = pd.Carbohydrates,
                        Protein = pd.Protein,
                        Quantity = pd.Quantity,
                        QuantityType = GetQuantityTypeEnum(pd.QuantityType)
                    }
                };
            }
        }

        /// <summary>
        /// Pozyskuje listę wszystkich produktów w bazie danych.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductDto> GetProducts()
        {
            var result = from p in context.Product
                         join pd in context.ProductsDetails
                         on p.ProductId equals pd.ProductId
                         select new
                         {
                             p,
                             pd
                         };

            var list = result.Select(x => new ProductDto()
            {
                ProductId = x.p.ProductId,
                UserId = x.p.UserId,
                Name = x.p.Name,
                Manufacturer = x.p.Manufacturer,
                Type = GetProductTypeEnum(x.p.ProductType),
                TypeDisplayName = GetProductTypeEnum(x.p.ProductType).GetDisplayName(),
                State = GetProductStateEnum(x.p.ProductState),
                Macro = new Macro()
                {
                    Protein = x.pd.Protein,
                    Fat = x.pd.Fat,
                    Carbohydrates = x.pd.Carbohydrates,
                    Calories = x.pd.Calories,
                    Quantity = x.pd.Quantity,
                    QuantityType = GetQuantityTypeEnum(x.pd.QuantityType)
                }
            });

            return list != null ? list.ToList() : new List<ProductDto>();
        }

        /// <summary>
        /// Pozyskuje listę wszystkich produktów dodanych przez użytkownika
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProductDto> GetUserProducts(Guid userId)
        {
            var guid = userId;

            var result = from p in context.Product
                         join pd in context.ProductsDetails
                         on p.ProductId equals pd.ProductId
                         where p.UserId.Equals(guid)
                         select new
                         {
                             p,
                             pd
                         };

            var list = result.Select(x => new ProductDto()
            {
                ProductId = x.p.ProductId,
                UserId = x.p.UserId,
                Name = x.p.Name,
                Manufacturer = x.p.Manufacturer,
                Type = GetProductTypeEnum(x.p.ProductType),
                TypeDisplayName = GetProductTypeEnum(x.p.ProductType).GetDisplayName(),
                State = GetProductStateEnum(x.p.ProductState),
                Macro = new Macro()
                {
                    Protein = x.pd.Protein,
                    Fat = x.pd.Fat,
                    Carbohydrates = x.pd.Carbohydrates,
                    Calories = x.pd.Calories,
                    Quantity = x.pd.Quantity,
                    QuantityType = GetQuantityTypeEnum(x.pd.QuantityType)
                }
            });

            return list != null ? list.ToList() : new List<ProductDto>();
        }

        /// <summary>
        /// Konwertuje typ produktu na wartość zapisaną do bazy danych.
        /// </summary>
        /// <param name="typeEnum"><see cref="ProductType"/></param>
        /// <returns></returns>
        private Int32 GetProductTypeValue(ProductType typeEnum)
        {
            Int32 value = 0;

            if (typeEnum == ProductType.DairyProducts)
                value = 0;
            else if (typeEnum == ProductType.FastFood)
                value = 1;
            else if (typeEnum == ProductType.Fruits)
                value = 2;
            else if (typeEnum == ProductType.Sweets)
                value = 3;
            else if (typeEnum == ProductType.Vegetables)
                value = 4;
            else
                throw new NotSupportedException(nameof(typeEnum));

            return value;
        }

        /// <summary>
        /// Konwertuje typ pojemnościowy produktu na wartość zapisaną do bazy danych.
        /// </summary>
        /// <param name="typeEnum"><see cref="QuantityType"/></param>
        /// <returns></returns>
        private Int32 GetQuantityTypeValue(QuantityType typeEnum)
        {
            Int32 value = 0;

            if (typeEnum == QuantityType.Grams)
                value = 0;
            else if (typeEnum == QuantityType.Milliliters)
                value = 1;
            else if (typeEnum == QuantityType.Piece)
                value = 2;
            else if (typeEnum == QuantityType.Package)
                value = 3;
            else
                throw new NotSupportedException(nameof(typeEnum));

            return value;
        }

        /// <summary>
        /// Konwertuje status produktu na wartość zapisaną do bazy danych.
        /// </summary>
        /// <param name="stateEnum"><see cref="ProductState"/></param>
        /// <returns></returns>
        private Int32 GetProductStateValue(ProductState stateEnum)
        {
            Int32 value = 0;

            if (stateEnum == ProductState.Private)
                value = 0;
            else if (stateEnum == ProductState.Pending)
                value = 1;
            else if (stateEnum == ProductState.Denied)
                value = 2;
            else if (stateEnum == ProductState.Accepted)
                value = 3;
            else
                throw new NotSupportedException(nameof(stateEnum));

            return value;
        }

        /// <summary>
        /// Konwertuje wartość typu produktu przechowywaną na typ enumeracyjny.
        /// </summary>
        /// <param name="typeValue"></param>
        /// <returns></returns>
        private ProductType GetProductTypeEnum(Int32 typeValue)
        {
            ProductType type;

            if (typeValue == 0)
                type = ProductType.DairyProducts;
            else if (typeValue == 1)
                type = ProductType.FastFood;
            else if (typeValue == 2)
                type = ProductType.Fruits;
            else if (typeValue == 3)
                type = ProductType.Sweets;
            else if (typeValue == 4)
                type = ProductType.Vegetables;
            else throw new NotSupportedException(nameof(typeValue));

            return type;
        }

        /// <summary>
        /// Konwertuje wartość typu pojemności produktu na typ enumeracyjny.
        /// </summary>
        /// <param name="typeValue"></param>
        /// <returns></returns>
        private QuantityType GetQuantityTypeEnum(Int32 typeValue)
        {
            QuantityType type;

            if (typeValue == 0)
                type = QuantityType.Grams;
            else if (typeValue == 1)
                type = QuantityType.Milliliters;
            else if (typeValue == 2)
                type = QuantityType.Piece;
            else if (typeValue == 3)
                type = QuantityType.Package;
            else throw new NotSupportedException(nameof(typeValue));

            return type;
        }

        /// <summary>
        /// Konwertuje wartości stanu produktu na typ enumeracyjny.
        /// </summary>
        /// <param name="stateValue"></param>
        /// <returns></returns>
        private ProductState GetProductStateEnum(Int32 stateValue)
        {
            ProductState type;

            if (stateValue == 0)
                type = ProductState.Private;
            else if (stateValue == 1)
                type = ProductState.Pending;
            else if (stateValue == 2)
                type = ProductState.Denied;
            else if (stateValue == 3)
                type = ProductState.Accepted;
            else throw new NotSupportedException(nameof(stateValue));

            return type;
        }

        public DailyFoodDto GetDailyFoodFromDailyFoodProductDto(DateTime date, IEnumerable<DailyFoodProductDto> dto)
        {
            List<DailyProductDto> products = new List<DailyProductDto>();
            foreach (var item in dto)
            {
                var p = GetProduct(item.ProductId);
                var quantity = item.ProductQuantity;
                var res = (decimal)quantity / p.Macro.Quantity;
                var currentMacro = new Macro()
                {
                    QuantityType = p.Macro.QuantityType,
                    Calories = Math.Round(p.Macro.Calories * res, 2),
                    Fat = Math.Round(p.Macro.Fat * res, 2),
                    Protein = Math.Round(p.Macro.Protein * res, 2),
                    Carbohydrates = Math.Round(p.Macro.Carbohydrates * res, 2),
                    Quantity = quantity
                };

                products.Add(new DailyProductDto()
                {
                    Product = p,
                    MealType = item.MealType,
                    CurrentMacro = currentMacro
                });
            }

            var totalFat = products.Sum(x => x.CurrentMacro.Fat);
            var totalCarbohydrates = products.Sum(x => x.CurrentMacro.Carbohydrates);
            var totalCalories = products.Sum(x => x.CurrentMacro.Calories);
            var totalProtein = products.Sum(x => x.CurrentMacro.Protein);


            return new DailyFoodDto()
            {
                DailyProduct = products,
                Day = date,
                DayCalories = totalCalories,
                DayCarbohydrates = totalCarbohydrates,
                DayFat = totalFat,
                DayProteins = totalProtein
            };

        }

        public IEnumerable<ProductDto> GetPendingSubscribeProducts()
        {
            var result = from p in context.Product
                         join pd in context.ProductsDetails
                         on p.ProductId equals pd.ProductId
                         where GetProductStateEnum(p.ProductState) == ProductState.Pending
                         select new
                         {
                             p,
                             pd
                         };

            var list = result.Select(x => new ProductDto()
            {
                ProductId = x.p.ProductId,
                UserId = x.p.UserId,
                Name = x.p.Name,
                Manufacturer = x.p.Manufacturer,
                Type = GetProductTypeEnum(x.p.ProductType),
                TypeDisplayName = GetProductTypeEnum(x.p.ProductType).GetDisplayName(),
                State = GetProductStateEnum(x.p.ProductState),
                Macro = new Macro()
                {
                    Protein = x.pd.Protein,
                    Fat = x.pd.Fat,
                    Carbohydrates = x.pd.Carbohydrates,
                    Calories = x.pd.Calories,
                    Quantity = x.pd.Quantity,
                    QuantityType = GetQuantityTypeEnum(x.pd.QuantityType)
                }
            });

            return list != null ? list.ToList() : new List<ProductDto>();
        }
    }
}
