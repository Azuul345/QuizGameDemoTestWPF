using QuizGame.DataModels;
using System.IO;
using System.Text.Json;

namespace QuizGame
{
    public static class QuizStorage
    {


        public static string AppDataRoot
        {
            get
            {
                string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(root, "QuizGame");
            }
        }

        // %LOCALAPPDATA%\QuizGame\Quizzs
        public static string QuizzFolder
        {
            get
            {
                return Path.Combine(AppDataRoot, "Quizzs");
            }
        }

        public static async Task EnsureAppDataAsync()
        {
            Directory.CreateDirectory(QuizzFolder);
            await CopyInitialIfEmptyAsync();
        }

        // SAVE one quiz as <Title>.json
        public static async Task SaveAsync(QuizDto quiz)
        {
            if (quiz == null) throw new ArgumentNullException(nameof(quiz));
            if (string.IsNullOrWhiteSpace(quiz.Title)) throw new ArgumentException("QuizDto.Title required");

            Directory.CreateDirectory(QuizzFolder);

            string safe = SanitizeFileName(quiz.Title);
            string file = Path.Combine(QuizzFolder, safe + ".json");

            var opts = new JsonSerializerOptions();
            opts.WriteIndented = true;

            using (var fs = File.Create(file))
            {
                await JsonSerializer.SerializeAsync(fs, quiz, opts);
            }
        }

        // LOAD all quizzes in folder
        public static async Task<List<QuizDto>> LoadAllAsync()
        {
            var list = new List<QuizDto>();

            Directory.CreateDirectory(QuizzFolder);

            foreach (var file in Directory.EnumerateFiles(QuizzFolder, "*.json"))
            {
                using (var fs = File.OpenRead(file))
                {
                    var dto = await JsonSerializer.DeserializeAsync<QuizDto>(fs);
                    if (dto != null) list.Add(dto);
                }
            }
            return list;
        }

        // ---- helpers ----

        private static string SanitizeFileName(string name)
        {
            if (name == null) return "";
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            return name.Trim();
        }

        private static async Task CopyInitialIfEmptyAsync()
        {
            // if already has files or subfolders → do nothing
            if (Directory.Exists(QuizzFolder))
            {
                if (Directory.EnumerateFileSystemEntries(QuizzFolder).Any())
                {
                    return;
                }
            }

            string src = Path.Combine(AppContext.BaseDirectory, "InitialQuizzes");
            if (!Directory.Exists(src)) return;

            await Task.Run(() => CopyAll(src, QuizzFolder));
        }

        private static void CopyAll(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            foreach (var dir in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
            {
                string targetDir = dir.Replace(source, destination);
                Directory.CreateDirectory(targetDir);
            }

            foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                string targetFile = file.Replace(source, destination);
                if (!File.Exists(targetFile))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                    File.Copy(file, targetFile, false);
                }
            }
        }

    }
}
