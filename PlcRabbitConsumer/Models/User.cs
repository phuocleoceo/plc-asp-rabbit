namespace PlcRabbitConsumer.Models;

public class User
{
    public string Name { get; set; }

    public bool Gender { get; set; }

    public override string ToString()
    {
        return $"{Name}: {Gender}";
    }
}
