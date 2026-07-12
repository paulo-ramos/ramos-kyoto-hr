using ramos_kyoto_hr.Domain.Base;
using ramos_kyoto_hr.Domain.ObjectValue;

namespace ramos_kyoto_hr.Domain.Entities;

public sealed class Company : Entity
{
    public Cnpj Cnpj { get; private set; }
    public RazaoSocial RazaoSocial { get; private set; }
    
    public Company(RazaoSocial razaoSocial, Cnpj cnpj)
    {
        RazaoSocial = razaoSocial ?? throw new ArgumentNullException(nameof(razaoSocial));
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
    }
    
    public void UpdateRazaoSocial(RazaoSocial novaRazaoSocial)
    {
        if (novaRazaoSocial == null)
            throw new ArgumentNullException(nameof(novaRazaoSocial));

        Update(() =>
        {
            RazaoSocial = novaRazaoSocial;
        });
    }
    
    public void Enable()
    {
        Update(() =>
        {
            IsActive = true;
        });
    }
    
    public void Disable()
    {
        Update(() =>
        {
            IsActive = false;
        });
    }
    
    // Hook 1: Executado ANTES da alteração (antes de modificar RazaoSocial)
    protected override void OnBeforeUpdate()
    {
        Console.WriteLine($"[BEFORE] Empresa {Id} está sendo atualizada...");
        Console.WriteLine($"[BEFORE] Razão Social atual: {RazaoSocial.Valor}");
        Console.WriteLine($"[BEFORE] UpdatedAt atual: {UpdatedAt?.ToString() ?? "null"}");
    }

    // Hook 2: Executado APÓS a alteração e após UpdatedAt ser atualizado
    protected override void OnAfterUpdate()
    {
        Console.WriteLine($"[AFTER] UpdatedAt agora é: {UpdatedAt}");
        
        // Adiciona ao histórico de auditoria
        var registro = $"{UpdatedAt:yyyy-MM-dd HH:mm:ss} - Razão Social alterada para: {RazaoSocial.Valor}";
        Console.WriteLine(registro);
        
        Console.WriteLine(new string('-', 60));
    }
    
}