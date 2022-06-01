using System.Threading.Channels;

namespace Study;

public class EnumerableConvert
{
    private readonly List<string> _lst = new();
    public EnumerableConvert()
    {
        _lst.Add(Guid.NewGuid().ToString("N"));
    }
    
    /// <summary>
    /// 将lst集合转为ILIst之后，对IList做Add，原lst集合是否也增加了。
    /// </summary>
    public void Show()
    {
        Console.WriteLine($"将lst集合转为ILIst之后，对IList做Add，原lst集合是否也增加了：");
        IList<string> lstBase = _lst;
        lstBase.Add("IList");

        Console.WriteLine($"lstBase.Count={lstBase.Count}");
        Console.WriteLine($"List.Count={_lst.Count}");
        Console.WriteLine("说明，这种类型的转换，依然还是操作同一个对象");
    }
}