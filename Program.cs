using System.Text.Json;

Console.WriteLine("Enter file name:");

string saveFileName = Console.ReadLine();

FileManager.CreateSaveFile(saveFileName);

FileManager.AddGameSaveToFile(new GameSave
{
    PlayerId = "zeMontreuil",
    DateCreated = DateTime.Now,
    CurrentLevel = 1,
}, saveFileName);

FileManager.AddGameSaveToFile(new GameSave
{
    PlayerId = "Tuna the Cat",
    DateCreated = DateTime.Now,
    CurrentLevel = 9001,
    CostumesAvailable = new string[] { "Tortoiseshell", "Fishbreath" }
}, saveFileName);

FileManager.DeleteGameSave(1, saveFileName);

public class GameSave
{
    public string PlayerId { get; set; }
    public int? SaveId { get; set; }
    public DateTime DateCreated { get; set; }
    public int CurrentLevel { get; set; }
    public string[] CostumesAvailable { get; set; }
    public GameSave() { }
}

public class SaveFile
{
    public string FilePath { get; set; }
    public List<GameSave> GameSaves { get; set; }
    public SaveFile() 
    { 
        GameSaves = new List<GameSave>();
    }
}

public static class FileManager
{
    private static JsonSerializerOptions Options { get; set; }
    static FileManager()
    {
        Options = new JsonSerializerOptions { WriteIndented = true };
    }

    public static void CreateSaveFile(string fileSaveName)
    {
        string fullPath = $@"C:\Users\zacharie.montreuil\source\repos\GameSave\{fileSaveName}.json";

        SaveFile newFile = new SaveFile
        {
            FilePath = fullPath
        };

        if (!File.Exists(fullPath))
        {
            try
            {
                string serialFile = JsonSerializer.Serialize(newFile, Options);

                File.WriteAllText(fullPath, serialFile);
                Console.WriteLine($"Succesfully wrote to new file at location {fullPath}");
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        } else
        {
            Console.WriteLine($"Did not write new file to path {fullPath}: Save File already exists.");
        }
    }
    public static void AddGameSaveToFile(GameSave gameSave, string saveFileName)
    {
        string path =  $@"C:\Users\zacharie.montreuil\source\repos\GameSave\{saveFileName}.json";

        if (!File.Exists(path))
        {
            CreateSaveFile(path);
        }

        try
        {
            string fileJson = File.ReadAllText(path);
            SaveFile retrievedFile = JsonSerializer.Deserialize<SaveFile>(fileJson);

            gameSave.SaveId = retrievedFile.GameSaves.Count + 1;

            retrievedFile.GameSaves.Add(gameSave);

            fileJson = JsonSerializer.Serialize(retrievedFile, Options);
            File.WriteAllText(path, fileJson);

            Console.WriteLine("Game saved!");

        } catch(IOException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    public static void DeleteGameSave(int saveId, string saveFileName)
    {
        string path = $@"C:\Users\zacharie.montreuil\source\repos\GameSave\{saveFileName}.json";

        if (File.Exists(path))
        {
            try
            {
                string fileJson = File.ReadAllText(path);
                SaveFile retrievedFile = JsonSerializer.Deserialize<SaveFile>(fileJson);

                GameSave saveToDelete = retrievedFile.GameSaves.First(s => s.SaveId == saveId);

                if(saveToDelete != null)
                {
                    retrievedFile.GameSaves.Remove(saveToDelete);
                    fileJson = JsonSerializer.Serialize<SaveFile>(retrievedFile, Options);
                    File.WriteAllText(path, fileJson);

                    Console.WriteLine($"Deleted save file for {saveToDelete.PlayerId}");
                } else
                {
                    Console.WriteLine("No save found with Save Id");
                    return;
                }

            } catch(IOException ex)
            {
                Console.WriteLine(ex.Message);
            }
        } else
        {
            Console.WriteLine("Error: no file with name given found.");
        }
    }
}
