namespace MetroPass.WP8.UI.Services.Cloud
{
    public interface ICloudItem    
    {
        string ID { get; }
        string UploadPath { get; }
        string Name { get; }
        string ItemType { get; }
        bool IsFolder { get; }
        bool IsKeePassItem { get; }
    }
}