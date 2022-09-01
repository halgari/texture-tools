using TextureTools.Operations;

namespace TextureTools;

public class Plan
{
    private List<IOperation> _operations = new();
    public static async Task<Plan> Load(Stream source)
    {
        var plan = new Plan();
        plan._operations.Add(await LoadDDS.Create(source));
        return plan;
    }
}