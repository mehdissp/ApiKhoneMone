using System;

public static class StringToGuidConverter
{
    public static Guid ConvertToGuid(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Guid.Empty;

        // سعی می‌کنیم مستقیماً پارس کنیم
        if (Guid.TryParse(input, out Guid result))
            return result;

        // اگر پارس مستقیم نشد، از هش استفاده می‌کنیم
        return CreateGuidFromString(input);
    }

    private static Guid CreateGuidFromString(string input)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            return new Guid(hashBytes);
        }
    }

    public static Guid? ConvertToNullableGuid(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        return ConvertToGuid(input);
    }
}