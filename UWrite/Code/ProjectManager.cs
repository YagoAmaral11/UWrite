using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using UWrite.Models;
using System.Threading.Tasks;

namespace UWrite.Code;

/// <summary>
/// Gerencia um projeto específico, contendo um proxy, gerenciador de mudanças, etc. para os documentos e seções do projeto.
/// </summary>
/// <param name="projectFilePath">Caminho do arquivo .uwp do projeto</param>
public class ProjectManager(string projectFilePath) : IDisposable
{
    private readonly string rootFilePath = Path.Combine(ProjectsFolderPath, projectFilePath);
    private ZipArchive rootFileZip;    
    private bool rootFileLoaded = false; // Se o arquivo zip raiz foi carregado (zipfile e sua metadata.uwf)    

    // Constantes de caminhos
    private const string MetadataPath = "metadata.uwf";
    private const string DocumentsFolderPath = "docs";
    public static string ProjectsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "UWrite", "Projects");

    // Dados do projeto carregado
    private ProjectMetadata projectMetadata;

    // Cache de documentos carregados
    private readonly Dictionary<string, TextDocument> loadedDocuments = new();

    // Cache de seções carregadas (documentName -> (sectionId -> section))
    private readonly Dictionary<string, Dictionary<ulong, TextSection>> loadedSections = new();

    


    /// <summary>
    /// Carrega o projeto, lendo ProjectMetadata do ZipContainer.
    /// </summary>
    public void LoadProject()
    {
        if (rootFileLoaded)
            return;

        try
        {
            // Abrir o ZipArchive
            rootFileZip = ZipFile.OpenRead(rootFilePath);

            // Carregar ProjectMetadata
            projectMetadata = LoadProjectMetadataInternal();
            rootFileLoaded = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao carregar o projeto: {rootFilePath}", ex);
        }
    }

    /// <summary>
    /// Obtém os metadados do projeto já carregados.
    /// </summary>
    public ProjectMetadata GetProjectMetadata()
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        return projectMetadata;
    }

    /// <summary>
    /// Carrega um TextDocument pelo nome.
    /// </summary>
    public TextDocument LoadDocument(string documentName)
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        // Verificar cache
        if (loadedDocuments.TryGetValue(documentName, out var cachedDoc))
            return cachedDoc;

        try
        {
            var document = LoadDocumentInternal(documentName);
            loadedDocuments[documentName] = document;
            return document;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao carregar documento: {documentName}", ex);
        }
    }

    /// <summary>
    /// Obtém uma TextSection específica pelo ID dentro de um documento.
    /// </summary>
    public TextSection GetTextSection(string documentName, ulong sectionId)
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        // Inicializar cache do documento se necessário
        if (!loadedSections.ContainsKey(documentName))
            loadedSections[documentName] = new Dictionary<ulong, TextSection>();

        var docCache = loadedSections[documentName];

        // Verificar cache
        if (docCache.TryGetValue(sectionId, out var cachedSection))
            return cachedSection;

        try
        {
            var section = LoadTextSectionInternal(documentName, sectionId);
            docCache[sectionId] = section;
            return section;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Erro ao carregar seção {sectionId} do documento {documentName}", ex);
        }
    }

    /// <summary>
    /// Carrega múltiplas TextSections de um documento.
    /// </summary>
    public List<TextSection> GetTextSections(string documentName, params ulong[] sectionIds)
    {
        return sectionIds.Select(id => GetTextSection(documentName, id)).ToList();
    }

    /// <summary>
    /// Carrega todas as seções de um documento em ordem.
    /// </summary>
    public List<TextSection> LoadAllSectionsInDocument(string documentName)
    {
        var document = LoadDocument(documentName);
        var sections = new List<TextSection>();

        if (!document.FirstTextContainer.HasValue)
            return sections;

        var currentId = document.FirstTextContainer.Value;

        while (currentId != 0)
        {
            var section = GetTextSection(documentName, currentId);
            sections.Add(section);

            if (!section.Next.HasValue)
                break;

            currentId = section.Next.Value;
        }

        return sections;
    }

    /// <summary>
    /// Salva o projeto, escrevendo ProjectMetadata e documentos modificados.
    /// </summary>
    public void SaveProject()
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        try
        {
            rootFileZip?.Dispose();

            // Usar ZipFile.Open com modo Update para modificar o arquivo
            using (var zipArchive = ZipFile.Open(rootFilePath, ZipArchiveMode.Update))
            {
                // Salvar ProjectMetadata
                SaveProjectMetadataInternal(zipArchive, projectMetadata);

                // Salvar todos os documentos em cache
                foreach (var document in loadedDocuments)
                {
                    SaveDocumentInternal(zipArchive, document.Key, document.Value);
                }

                // Salvar todas as seções em cache
                foreach (var docKvp in loadedSections)
                {
                    var documentName = docKvp.Key;
                    foreach (var secKvp in docKvp.Value)
                    {
                        SaveTextSectionInternal(zipArchive, documentName, secKvp.Key, secKvp.Value);
                    }
                }
            }

            // Reabrir para leitura
            rootFileZip = ZipFile.OpenRead(rootFilePath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao salvar o projeto", ex);
        }
    }

    /// <summary>
    /// Cria um novo documento vazio.
    /// </summary>
    public TextDocument CreateNewDocument(string documentName)
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        if (loadedDocuments.ContainsKey(documentName))
            throw new InvalidOperationException($"Documento '{documentName}' já existe.");

        var newDocument = new TextDocument
        {
            Name = documentName,
            NextID = 0,
            RemovalCount = 0,
            FirstTextContainer = null,
            LastTextContainer = null
        };

        loadedDocuments[documentName] = newDocument;
        return newDocument;
    }

    /// <summary>
    /// Cria uma nova TextSection e adiciona ela no final do documento
    /// </summary>
    public ulong AppendTextSection(string documentName, string text)
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        var document = LoadDocument(documentName);

        // Gerar novo ID
        var newId = document.NextID;
        document.NextID++;

        var newSection = new TextSection
        {
            Text = text,
            Previous = document.LastTextContainer,
            Next = null
        };

        // Atualizar último elemento se existir
        if (document.LastTextContainer.HasValue)
        {
            var lastSection = GetTextSection(documentName, document.LastTextContainer.Value);
            lastSection.Next = newId;
            loadedSections[documentName][document.LastTextContainer.Value] = lastSection;
        }

        // Atualizar primeiro container se é o primeiro elemento
        if (!document.FirstTextContainer.HasValue)
        {
            document.FirstTextContainer = newId;
        }

        // Atualizar último container
        document.LastTextContainer = newId;

        // Adicionar seção ao cache
        if (!loadedSections.ContainsKey(documentName))
            loadedSections[documentName] = new Dictionary<ulong, TextSection>();

        loadedSections[documentName][newId] = newSection;

        return newId;
    }

    /// <summary>
    /// Modifica o texto de uma TextSection existente.
    /// </summary>
    public void UpdateTextSection(string documentName, ulong sectionId, string newText)
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        var section = GetTextSection(documentName, sectionId);
        section.Text = newText;

        if (!loadedSections.ContainsKey(documentName))
            loadedSections[documentName] = new Dictionary<ulong, TextSection>();

        loadedSections[documentName][sectionId] = section;
    }

    /// <summary>
    /// Remove uma TextSection do documento, atualizando o encadeamento.
    /// </summary>
    public void RemoveTextSection(string documentName, ulong sectionId)
    {
        if (!rootFileLoaded)
            throw new InvalidOperationException("Projeto não foi carregado. Chame LoadProject() primeiro.");

        var document = LoadDocument(documentName);
        var section = GetTextSection(documentName, sectionId);

        // Atualizar previous
        if (section.Previous.HasValue)
        {
            var prevSection = GetTextSection(documentName, section.Previous.Value);
            prevSection.Next = section.Next;
            loadedSections[documentName][section.Previous.Value] = prevSection;
        }
        else if (document.FirstTextContainer == sectionId)
        {
            document.FirstTextContainer = section.Next;
        }

        // Atualizar next
        if (section.Next.HasValue)
        {
            var nextSection = GetTextSection(documentName, section.Next.Value);
            nextSection.Previous = section.Previous;
            loadedSections[documentName][section.Next.Value] = nextSection;
        }
        else if (document.LastTextContainer == sectionId)
        {
            document.LastTextContainer = section.Previous;
        }

        // Remover do cache e incrementar contador de remoção
        loadedSections[documentName].Remove(sectionId);
        document.RemovalCount++;
    }

    public Task<Project> CreateNewProject(string Name)
    {
        // TODO: Criar um novo projeto com o nome especificado, inicializando ProjectMetadata e salvando no arquivo .uwp, caso não exista outro arquivo com o mesmo nome.
    }

    public Task<List<(string, ProjectMetadata)>> ListProjects()
    {
        // TODO: Listar todos os projetos já criados, junto com seus metadados
    }


    /// <summary>
    /// Carrega ProjectMetadata internamente 
    /// </summary>
    private ProjectMetadata LoadProjectMetadataInternal()
    {
        try
        {
            var entry = rootFileZip.GetEntry(MetadataPath);
            if (entry == null)
                throw new FileNotFoundException($"Arquivo de metadados não encontrado: {MetadataPath}");

            using (var stream = entry.Open())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var encryptedData = ms.ToArray();
                var decryptedData = EncryptionHelper.Decrypt(encryptedData);
                return BinarySerializationHelper.DeserializeProjectMetadata(decryptedData);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao carregar metadados do projeto", ex);
        }
    }

    /// <summary>
    /// Salva ProjectMetadata internamente 
    /// </summary>
    private void SaveProjectMetadataInternal(ZipArchive zipArchive, ProjectMetadata metadata)
    {
        try
        {
            var serialized = BinarySerializationHelper.SerializeProjectMetadata(metadata);
            var encrypted = EncryptionHelper.Encrypt(serialized);

            // Remover entrada existente se houver
            var existingEntry = zipArchive.GetEntry(MetadataPath);
            existingEntry?.Delete();

            // Adicionar nova entrada
            var entry = zipArchive.CreateEntry(MetadataPath);
            using (var stream = entry.Open())
            {
                stream.Write(encrypted, 0, encrypted.Length);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao salvar metadados do projeto", ex);
        }
    }

    /// <summary>
    /// Carrega um TextDocument internamente 
    /// </summary>
    private TextDocument LoadDocumentInternal(string documentName)
    {
        try
        {
            var documentPath = $"{DocumentsFolderPath}/{documentName}/metadata.uwf";
            var entry = rootFileZip.GetEntry(documentPath);
            if (entry == null)
                throw new FileNotFoundException($"Arquivo de documento não encontrado: {documentPath}");

            using (var stream = entry.Open())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var encryptedData = ms.ToArray();
                var decryptedData = EncryptionHelper.Decrypt(encryptedData);
                return BinarySerializationHelper.DeserializeTextDocument(decryptedData);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao carregar documento: {documentName}", ex);
        }
    }

    /// <summary>
    /// Salva um TextDocument internamente 
    /// </summary>
    private void SaveDocumentInternal(ZipArchive zipArchive, string documentName, TextDocument document)
    {
        try
        {
            var documentPath = $"{DocumentsFolderPath}/{documentName}/metadata.uwf";
            var serialized = BinarySerializationHelper.SerializeTextDocument(document);
            var encrypted = EncryptionHelper.Encrypt(serialized);

            // Remover entrada existente se houver
            var existingEntry = zipArchive.GetEntry(documentPath);
            existingEntry?.Delete();

            // Adicionar nova entrada
            var entry = zipArchive.CreateEntry(documentPath);
            using (var stream = entry.Open())
            {
                stream.Write(encrypted, 0, encrypted.Length);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao salvar documento: {documentName}", ex);
        }
    }

    /// <summary>
    /// Carrega uma TextSection internamente
    /// </summary>
    private TextSection LoadTextSectionInternal(string documentName, ulong sectionId)
    {
        try
        {
            var sectionPath = $"{DocumentsFolderPath}/{documentName}/{sectionId}.uwf";
            var entry = rootFileZip.GetEntry(sectionPath);
            if (entry == null)
                throw new FileNotFoundException($"Seção não encontrada: {sectionPath}");

            using (var stream = entry.Open())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var encryptedData = ms.ToArray();
                var decryptedData = EncryptionHelper.Decrypt(encryptedData);
                return BinarySerializationHelper.DeserializeTextSection(decryptedData);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Erro ao carregar seção {sectionId} do documento {documentName}", ex);
        }
    }

    /// <summary>
    /// Salva uma TextSection internamente 
    /// </summary>
    private void SaveTextSectionInternal(ZipArchive zipArchive, string documentName, ulong sectionId, TextSection section)
    {
        try
        {
            var sectionPath = $"{DocumentsFolderPath}/{documentName}/{sectionId}.uwf";
            var serialized = BinarySerializationHelper.SerializeTextSection(section);
            var encrypted = EncryptionHelper.Encrypt(serialized);

            // Remover entrada existente se houver
            var existingEntry = zipArchive.GetEntry(sectionPath);
            existingEntry?.Delete();

            // Adicionar nova entrada
            var entry = zipArchive.CreateEntry(sectionPath);
            using (var stream = entry.Open())
            {
                stream.Write(encrypted, 0, encrypted.Length);
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Erro ao salvar seção {sectionId} do documento {documentName}", ex);
        }
    }

    /// <summary>
    /// Libera os recursos do ProjectManager.
    /// </summary>
    public void Dispose()
    {
        rootFileZip?.Dispose();
        loadedDocuments.Clear();
        loadedSections.Clear();
        rootFileLoaded = false;
    }

}
