namespace MyWishlist.Utils
{
    public class StubImagesStorage : IImagesStorage
    {
        public string Upload(string path)
        {
            return "img/sample.png";
        }

        public void Delete(string path)
        {
            //do nothing
        }
    }
}