using ramos_kyoto_hr.Domain.Base;
using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Domain.Utils;

namespace ramos_kyoto_hr.Domain.Entities;

public sealed class Company : Entity
{
    public Cnpj Cnpj { get; private set; }
    public RazaoSocial RazaoSocial { get; private set; }
    
    public Company(DateOnly effectiveStartDate, RazaoSocial razaoSocial, Cnpj cnpj):base(effectiveStartDate)
    {
        RazaoSocial = razaoSocial ?? throw new ArgumentNullException(nameof(razaoSocial));
        EffectiveStartDate = effectiveStartDate != DateOnly.MinValue ? effectiveStartDate: throw new ArgumentNullException(nameof(effectiveStartDate));
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        SetId();
    }
    
    public void UpdateRazaoSocial(DateOnly effectiveStartDate,RazaoSocial novaRazaoSocial)
    {
        if (novaRazaoSocial == null)
            throw new ArgumentNullException(nameof(novaRazaoSocial));

        Update(() =>
        {
            EffectiveStartDate = effectiveStartDate;
            RazaoSocial = novaRazaoSocial;
        });
    }
    
    public void Enable(DateOnly effectiveStartDate)
    {
        Update(() =>
        {
            EffectiveStartDate = effectiveStartDate;
            IsActive = true;
        });
    }
    
    public void Disable(DateOnly effectiveStartDate)
    {
        Update(() =>
        {
            EffectiveStartDate = effectiveStartDate;
            IsActive = false;
        });
    }

    private void SetId()
    {
        Update(() =>
        {
            Id = GuidGenerator.GuidOrganizationalStructure(Cnpj.Valor, EffectiveStartDate);
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