using eCommerce_API.Dtos;

namespace eCommerce_API.Services.Pdf
{
    public interface IDocService
    {
        public Task<string> CreateDoc(DocRequest docRequest);
    }
}
