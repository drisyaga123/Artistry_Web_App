using Microsoft.AspNetCore.Mvc;

namespace eCommerce_API.Common
{
    public static class JwtDetailsFetch
    {

        public static int GetCurrentUserId(IHttpContextAccessor _httpContextAccessor)
        {

            var userIdClaim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Contains("nameidentifier"));

            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                return Convert.ToInt32(userId);
            }
            else
            {
                return 0;
            }
        }
    }
    public static class CommonMethods
    {

        public static string ConvertImgToBase64(string filePath)
        {
            if (!string.IsNullOrWhiteSpace(filePath) && System.IO.File.Exists(filePath))
            {

                byte[] imageBytes = System.IO.File.ReadAllBytes(filePath);
                string base64String = Convert.ToBase64String(imageBytes);
                // Get the file extension from the original image path
                string fileExtension = Path.GetExtension(filePath);

                // Update the ProductImage property with the Base64 string and original file extension
                var ImgBase64 = $"data:image/{fileExtension};base64,{base64String}"; // Assuming images are JPEG format
                return ImgBase64;
            }
            return "";
        }
    }
}
