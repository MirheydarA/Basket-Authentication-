namespace Fiorello.Utilities
{
    public interface IFIleService
    {
        string Upload(IFormFile file);
        void Delete(string photoName);
        bool IsImage(IFormFile file ); 
        bool IsBiggerThanSize(IFormFile file , int size = 100); 
    }
}
