namespace CSRFProtectionDemo.Services;

// Estado partilhado apenas para a demonstração. Não utilizar como armazenamento real.
public sealed class DemoStore
{
    private readonly object _gate = new();
    private string _name = "Perfil de demonstração";
    public string Read() { lock (_gate) return _name; }
    public void Update(string name) { lock (_gate) _name = name; }
}
