namespace MyWishlist.Utils
{
    internal interface IImagesStorage
    {
        string Upload(string path);
        void Delete(string path);
    }
}