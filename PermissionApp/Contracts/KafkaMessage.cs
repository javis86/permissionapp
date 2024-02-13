namespace PermissionApp.Contracts;

public class KafkaMessage
{
    public Guid Id { get; set; }
    public string NameOperation { get; set; }
}