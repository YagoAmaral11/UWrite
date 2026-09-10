using System;
using System.Security.Cryptography;
using System.Text;

namespace UWrite.Code;

/// <summary>
/// Classe auxiliar para criptografia e descriptografia de dados.
/// </summary>
internal static class EncryptionHelper
{
    // Chave de criptografia padrão (256 bits para AES)
    private static readonly byte[] DefaultKey = Encoding.UTF8.GetBytes("UWrite_Default_Key_2024_32Bytes!!");

    // IV padrão (128 bits para AES)
    private static readonly byte[] DefaultIV = Encoding.UTF8.GetBytes("UWrite_IV_16Byt!");

    static EncryptionHelper()
    {
        // Validar que a chave tem exatamente 32 bytes (256 bits)
        if (DefaultKey.Length != 32)
            throw new InvalidOperationException("A chave padrão deve ter 32 bytes (256 bits)");

        // Validar que o IV tem exatamente 16 bytes (128 bits)
        if (DefaultIV.Length != 16)
            throw new InvalidOperationException("O IV padrão deve ter 16 bytes (128 bits)");
    }

    /// <summary>
    /// Criptografa um array de bytes usando AES-256.
    /// </summary>
    public static byte[] Encrypt(byte[] data)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = DefaultKey;
            aes.IV = DefaultIV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            {
                return encryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }
    }

    /// <summary>
    /// Descriptografa um array de bytes usando AES-256.
    /// </summary>
    public static byte[] Decrypt(byte[] data)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = DefaultKey;
            aes.IV = DefaultIV;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            {
                return decryptor.TransformFinalBlock(data, 0, data.Length);
            }
        }
    }
}
