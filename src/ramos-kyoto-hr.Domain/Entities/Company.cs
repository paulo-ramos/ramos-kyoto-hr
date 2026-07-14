using ramos_kyoto_hr.Domain.Base;
using ramos_kyoto_hr.Domain.ObjectValue;
using ramos_kyoto_hr.Domain.Utils;

namespace ramos_kyoto_hr.Domain.Entities;

public sealed class Company : Entity
{
    public Cnpj Cnpj { get; private set; }
    public RazaoSocial RazaoSocial { get; private set; }
    
    public Company(DateOnly effectiveStartDate, RazaoSocial razaoSocial, Cnpj cnpj, bool isActive = true) 
        : base(effectiveStartDate, isActive)
    {
        RazaoSocial = razaoSocial ?? throw new ArgumentNullException(nameof(razaoSocial));
        Cnpj = cnpj ?? throw new ArgumentNullException(nameof(cnpj));
        IsActive = isActive;
        Id = GuidGenerator.GuidOrganizationalStructure(Cnpj.Valor, effectiveStartDate);
    }
    
    public Company? UpdateRazaoSocial(DateOnly newEffectiveStartDate, RazaoSocial novaRazaoSocial)
    {
        if (novaRazaoSocial == null)
            throw new ArgumentNullException(nameof(novaRazaoSocial));

        ValidateNewEffectiveDate(newEffectiveStartDate);

        if (novaRazaoSocial == RazaoSocial)
        {
            return null;
        }
        
        return CreateNewVersion(newEffectiveStartDate, novaRazaoSocial: novaRazaoSocial);
    }
    
    public Company? Enable(DateOnly newEffectiveStartDate)
    {
        ValidateNewEffectiveDate(newEffectiveStartDate);
        
        if (IsStatusActive())
        {
            return null;
        }

        return CreateNewVersion(newEffectiveStartDate, newIsActive: true);
    }
    
    public Company? Disable(DateOnly newEffectiveStartDate)
    {
        ValidateNewEffectiveDate(newEffectiveStartDate);
        
        if (IsStatusDeactive())
        {
            return null;
        }

        return CreateNewVersion(newEffectiveStartDate, newIsActive: false);
    }
    
    #region Private Helpers

    private Company CreateNewVersion(
        DateOnly newEffectiveStartDate, 
        RazaoSocial? novaRazaoSocial = null, 
        bool? newIsActive = null)
    {
        return new Company(
            newEffectiveStartDate,
            novaRazaoSocial ?? this.RazaoSocial,
            this.Cnpj,
            newIsActive ?? this.IsActive
        );
    }

    #endregion
    
}