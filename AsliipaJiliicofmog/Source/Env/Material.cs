using System.Text.Json;

namespace AsliipaJiliicofmog.Env;

public class Material
{
  public string Element { get; set; }
  public string Name { get; set; }
  public float Density { get; set; }
  public float[] Color { get; set; }
  public float Strength { get; set; }
  public float MeltingPoint { get; set; }

  public static Material Deserialize(string json) => JsonSerializer.Deserialize<Material>(json);
}
