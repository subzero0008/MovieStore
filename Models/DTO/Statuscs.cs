namespace MovieStoreMvc.Models.DTO
{
    public class Status
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
    }
}
//служи като удобен начин за предаване на резултатите от операции в приложението.
//Той позволява стандартизиране на резултатите и улеснява обработката на различни състояния (успех, грешка и т.н.) в контролерите.