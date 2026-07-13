using System.Security.Cryptography;
using System.Text;

namespace ramos_kyoto_hr.Domain.Utils;

public static class GuidGenerator
{
    public static Guid GuidOrganizationalStructure(string objectKey, DateOnly validationDate)
    {
        string input = $"{objectKey}_{validationDate:yyyyMMdd}";
        
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = MD5.HashData(inputBytes);

        return new Guid(hashBytes);
    }
}