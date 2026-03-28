namespace payment_system.Application.Common
{
    /// <summary>
    /// Generic Result pattern sınıfı - başarı ve hata yönetimi için
    /// Tüm service metodları bu sınıfın instansını döndürür
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public int? StatusCode { get; set; }

        /// <summary>
        /// Başarılı sonuç döndür
        /// </summary>
        public static Result<T> Success(T data, string message = "Başarılı", int statusCode = 200)
            => new() 
            { 
                IsSuccess = true, 
                Data = data, 
                Message = message, 
                StatusCode = statusCode 
            };

        /// <summary>
        /// Hata sonucu döndür
        /// </summary>
        public static Result<T> Failure(string message, int statusCode = 400)
            => new() 
            { 
                IsSuccess = false, 
                Message = message, 
                StatusCode = statusCode 
            };
    }
}