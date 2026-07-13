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
    
    public bool UpdateRazaoSocial(DateOnly effectiveStartDate,RazaoSocial novaRazaoSocial)
    {
        if (novaRazaoSocial == null)
            throw new ArgumentNullException(nameof(novaRazaoSocial));

        if (novaRazaoSocial == this.RazaoSocial)
        {
            return false;
        }

        Update(() =>
        {
            EffectiveStartDate = effectiveStartDate;
            RazaoSocial = novaRazaoSocial;
        });

        return true;
    }
    
    public bool Enable(DateOnly effectiveStartDate)
    {
        if (IsStatusActive())
        {
            return false;
        }

        Update(() =>
        {
            EffectiveStartDate = effectiveStartDate;
            IsActive = true;
        });
        
        return true;
    }
    
    public bool Disable(DateOnly effectiveStartDate)
    {
        if (IsStatusDeactive())
        {
            return false;
        }

        Update(() =>
        {
            EffectiveStartDate = effectiveStartDate;
            IsActive = false;
        });
        
        return true;
    }

    private void SetId()
    {
        Update(() =>
        {
            Id = GuidGenerator.GuidOrganizationalStructure(Cnpj.Valor, EffectiveStartDate);
        });
    }
    
    protected override void OnBeforeUpdate()
    {
        Console.WriteLine($"[BEFORE] Empresa {Id} está sendo atualizada...");
        Console.WriteLine($"[BEFORE] Razão Social atual: {RazaoSocial.Valor}");
        Console.WriteLine($"[BEFORE] UpdatedAt atual: {UpdatedAt?.ToString() ?? "null"}");
    }

    protected override void OnAfterUpdate()
    {
        Console.WriteLine($"[AFTER] UpdatedAt agora é: {UpdatedAt}");
        
        var registro = $"{UpdatedAt:yyyy-MM-dd HH:mm:ss} - Razão Social alterada para: {RazaoSocial.Valor}";
        Console.WriteLine(registro);
        
        Console.WriteLine(new string('-', 60));
    }
    
}