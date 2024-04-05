using System.Text.Json;
using BlazorBudget.Core;

namespace BlazorBudget.Web.Data;

public class ClassificationService
{
    private const string _rulesFilename = "autoimport/allRules.json";
    private static readonly JsonSerializerOptions serializerOptions = new() { WriteIndented = true };
    
    public async Task<ClassificationRule[]?> GetAllRulesAsync()
    {
        using FileStream openStream = File.OpenRead(_rulesFilename);
        var allRules = await JsonSerializer.DeserializeAsync<ClassificationRule[]>(openStream);
        return allRules;
    }

    public async Task SaveAllRulesAsync(IEnumerable<ClassificationRule> rules)
    {
        using FileStream createStream = File.Create(_rulesFilename);
        await JsonSerializer.SerializeAsync(createStream, rules, serializerOptions);
        await createStream.DisposeAsync();
    }
}
