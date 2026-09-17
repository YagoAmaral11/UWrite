using System;
using System.IO;
using System.Collections.Generic;
using UWrite.Models;

namespace UWrite.Code;

/// <summary>
/// Classe auxiliar para serialização e desserialização de objetos em formato binário.
/// </summary>
internal static class BinarySerializationHelper
{
    /// <summary>
    /// Serializa um ProjectMetadata para um array de bytes.
    /// </summary>
    public static byte[] SerializeProjectMetadata(ProjectMetadata metadata)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms))
        {
            writer.Write(metadata.ProjectName);
            writer.Write(metadata.ApplicationVersion);
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Desserializa um ProjectMetadata de um array de bytes.
    /// </summary>
    public static ProjectMetadata DeserializeProjectMetadata(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        using (var reader = new BinaryReader(ms))
        {
            return new ProjectMetadata
            {
                ProjectName = reader.ReadString(),
                ApplicationVersion = reader.ReadInt32()
            };
        }
    }


    /// <summary>
    /// Serializa um TextDocument para um array de bytes.
    /// </summary>
    public static byte[] SerializeTextDocument(TextDocument document)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms))
        {
            writer.Write(document.Name ?? "");
            writer.Write(document.NextID);
            writer.Write(document.RemovalCount);

            // Escrever FirstTextContainer (ulong? - nullable)
            if (document.FirstTextContainer.HasValue)
            {
                writer.Write(true);
                writer.Write(document.FirstTextContainer.Value);
            }
            else
            {
                writer.Write(false);
            }

            // Escrever LastTextContainer (ulong? - nullable)
            if (document.LastTextContainer.HasValue)
            {
                writer.Write(true);
                writer.Write(document.LastTextContainer.Value);
            }
            else
            {
                writer.Write(false);
            }

            return ms.ToArray();
        }
    }

    /// <summary>
    /// Desserializa um TextDocument de um array de bytes.
    /// </summary>
    public static TextDocument DeserializeTextDocument(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        using (var reader = new BinaryReader(ms))
        {
            var document = new TextDocument
            {
                Name = reader.ReadString(),
                NextID = reader.ReadUInt64(),
                RemovalCount = reader.ReadUInt32()
            };

            // Ler FirstTextContainer
            if (reader.ReadBoolean())
            {
                document.FirstTextContainer = reader.ReadUInt64();
            }

            // Ler LastTextContainer
            if (reader.ReadBoolean())
            {
                document.LastTextContainer = reader.ReadUInt64();
            }

            return document;
        }
    }


    /// <summary>
    /// Serializa um TextSegment para um array de bytes.
    /// </summary>
    public static byte[] SerializeTextSection(TextSegment section)
    {
        using (var ms = new MemoryStream())
        using (var writer = new BinaryWriter(ms))
        {
            // Escrever Previous (ulong? - nullable)
            if (section.Previous.HasValue)
            {
                writer.Write(true);
                writer.Write(section.Previous.Value);
            }
            else
            {
                writer.Write(false);
            }

            // Escrever Next (ulong? - nullable)
            if (section.Next.HasValue)
            {
                writer.Write(true);
                writer.Write(section.Next.Value);
            }
            else
            {
                writer.Write(false);
            }

            writer.Write(section.Text ?? "");
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Desserializa um TextSegment de um array de bytes.
    /// </summary>
    public static TextSegment DeserializeTextSection(byte[] data)
    {
        using (var ms = new MemoryStream(data))
        using (var reader = new BinaryReader(ms))
        {
            var section = new TextSegment();

            // Ler Previous
            if (reader.ReadBoolean())
            {
                section.Previous = reader.ReadUInt64();
            }

            // Ler Next
            if (reader.ReadBoolean())
            {
                section.Next = reader.ReadUInt64();
            }

            section.Text = reader.ReadString();
            return section;
        }
    }

}
