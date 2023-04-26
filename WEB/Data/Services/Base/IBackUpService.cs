namespace WEB.Data.Services.Base
{
    public interface IBackUpService
    {
        public Task BackUpDatabase();
        public Task RestoreDatabase(byte[] data);
    }
}
