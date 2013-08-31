namespace MetroPass.WP8.UI.Services
{
    public interface ICloudItem    
    {
        string ID { get; }
        string Name { get; }
        string ItemType { get; }
        bool IsFolder { get; }
        bool IsKeePassItem { get; }
    }
}