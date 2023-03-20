namespace PLCLib;

public class Product
{
    public string Name { get; set; }

    public double Price { get; set; }

    public Product()
    {
    }

    public override string ToString()
    {
        return $"{this.Name}: {this.Price}";
    }
}