namespace PersonalTrainerCore.Api
{
    public static class ApiUrls
    {
        private const string apiUrl = "https://localhost:44323/";

        public static string RegisterUrl = apiUrl + "api/user/register";
        public static string LoginUrl = apiUrl + "api/user/login";
        public static string LoginAdminUrl = apiUrl + "api/user/loginAdminSample";
        public static string UserGoalsUrl = apiUrl + "api/user/userGoal/#ID#";
        public static string GetUser = apiUrl + "api/user/#ID#";
        public static string GetUsers = apiUrl + "api/user/users";
        public static string PromoteUser = apiUrl + "api/user/PromoteToAdmin/#ID#";
        public static string DegradateUser = apiUrl + "api/user/DegradateUser/#ID#";
        public static string DeleteUser = apiUrl + "api/user/#ID#";


        public static string GetProductUrl = apiUrl + "api/Product/#ID#";
        public static string GetProductsUrl = apiUrl + "api/Product/Products";
        public static string GetUserProductsUrl = apiUrl + "api/Product/UserProducts/#USERID#";
    
        public static string AddProductUrl = apiUrl + "api/Product/";
        public static string EditProductUrl = apiUrl + "api/Product/#ID#";
        public static string DeleteProduct = apiUrl + "api/Product/#ID#";

        public static string SubscribeProduct = apiUrl + "api/Product/Subscribe/#ID#";
        public static string AcceptSubscriptionProduct = apiUrl + "api/Product/AcceptSubscription/#ID#";
        public static string DeclineSubscriptionProduct = apiUrl + "api/Product/DeclineSubscription/#ID#";

        
        public static string GetDayMeals = apiUrl + "api/Meal/GetDayMeals/#DATE#";
        public static string GetDayMeal = apiUrl + "api/Meal/#USERID#/#ID#";
        public static string SubmitDayMeal = apiUrl + "api/Meal/SubmitDay/#USERID#/#DATE#";
    }
}
