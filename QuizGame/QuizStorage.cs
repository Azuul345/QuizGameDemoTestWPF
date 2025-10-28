using QuizGame.DataModels;
using System.IO;
using System.Text.Json;

namespace QuizGame
{
    public static class QuizStorage
    {
        public static string GetFolder()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string folder = Path.Combine(root, "QuizGame", "Quizzs");
            Directory.CreateDirectory(folder);
            return folder;
        }

        // Save one quiz as <Title>.json
        public static async Task SaveAsync(QuizDto quiz)
        {
            if (string.IsNullOrWhiteSpace(quiz.Title))
                throw new ArgumentException("QuizDto.Title required");

            string file = Path.Combine(GetFolder(), SanitizeFileName(quiz.Title) + ".json");
            var opts = new JsonSerializerOptions { WriteIndented = true };
            using var fs = File.Create(file);
            await JsonSerializer.SerializeAsync(fs, quiz, opts);
        }

        // Load all quizzes found in folder
        public static async Task<List<QuizDto>> LoadAllAsync()
        {
            var result = new List<QuizDto>();
            foreach (var file in Directory.EnumerateFiles(GetFolder(), "*.json"))
            {
                using var fs = File.OpenRead(file);
                var dto = await JsonSerializer.DeserializeAsync<QuizDto>(fs);
                if (dto != null) result.Add(dto);
            }
            return result;
        }

        // Load by file path (optional helper)
        public static async Task<QuizDto?> LoadAsync(string filePath)
        {
            if (!File.Exists(filePath)) return null;
            using var fs = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<QuizDto>(fs);
        }

        private static string SanitizeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return name.Trim();
        }
        //handle the files from GitHub repo and add to AppData folder
        public static string AppDataRoot
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "QuizGame");

            }
        }

        // => does the same as the get above
        public static string QuizzFolder => Path.Combine(AppDataRoot, "Quizzs");

        public static async Task EnsureAppDataAsync()
        {
            Directory.CreateDirectory(QuizzFolder);           // create if missing
            await CopyInitialIfEmptyAsync();                  // seed on first run
        }

        private static async Task CopyInitialIfEmptyAsync()
        {
            if (Directory.EnumerateFileSystemEntries(QuizzFolder).Any())
                return;                                       // already has files

            string src = Path.Combine(AppContext.BaseDirectory, "InitialQuizzes");
            if (!Directory.Exists(src)) return;               // nothing to seed

            await Task.Run(() => CopyAll(src, QuizzFolder));  // include images
        }

        private static void CopyAll(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (var dir in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dir.Replace(source, destination));

            foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                var target = file.Replace(source, destination);
                if (!File.Exists(target)) File.Copy(file, target, overwrite: false);
            }
        }



    }
}
