using PLCLib;

namespace PLCProducer;

public interface IProductProducer
{
    public void SendProductMessage(Product product);
}