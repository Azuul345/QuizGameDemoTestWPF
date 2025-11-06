using QuizGame.DataModels;
using System.IO;
using System.Text.Json;
//using static System.Net.WebRequestMethods;

namespace QuizGame
{
    public static class QuizStorage
    {


        public static string AppDataRoot
        {
            get
            {    // Gets the system's local app data path. 
                string root = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                // Adds a subfolder named "QuizGame" to that path.
                return Path.Combine(root, "QuizGame");
            }
        }

        // %LOCALAPPDATA%\QuizGame\Quizzs
        // Builds a full path for where quiz files will be stored.
        // Uses AppDataRoot as a base and adds a folder named "Quizzs".
        public static string QuizzFolder
        {
            get
            {   //build upon previous logic and add a folder for the quizzes
                return Path.Combine(AppDataRoot, "Quizzs");
            }
        }



        public static async Task EnsureAppDataAsync()
        {
            // Makes sure the folder exists. If it already exists, this does nothing.
            Directory.CreateDirectory(QuizzFolder);
            // If the folder is empty we copy in some starter quizzes.
            await CopyInitialIfEmptyAsync();
        }




        // SAVE one quiz as <Title>.json
        public static async Task SaveAsync(Quiz quiz)
        {
            // Defensive checks. We do not want to save "nothing".
            if (quiz == null) throw new ArgumentNullException(nameof(quiz));
            // We need a title to build the file name.
            if (string.IsNullOrWhiteSpace(quiz.Title)) throw new ArgumentException("QuizDto.Title required");

            // Make sure the folder exists before writing.
            Directory.CreateDirectory(QuizzFolder);

            // Turn the quiz title into a safe file name.
            // Example: "C# Basics" -> "C#_Basics" or similar, depending on your SanitizeFileName.
            string safe = SanitizeFileName(quiz.Title); //method 4

            // Build the full path:  ...\Quizzs\<title>.json
            string file = Path.Combine(QuizzFolder, safe + ".json");


            // JSON options: indent = pretty format so humans can read the file.
            var opts = new JsonSerializerOptions();
            opts.WriteIndented = true;

            // Create (or overwrite) the file and get a FileStream for it.
            //using disposes the stream even if something goes wrong
            //The compiler rewrites this into a try/finally block.
            //Inside the try, your code runs.
            //In the finally, fs.Dispose() is called automatically.
            //Dispose() closes the file handle.
            using (var fs = File.Create(file))
            {
                // Serialize the quiz object to JSON directly into the file stream.
                await JsonSerializer.SerializeAsync(fs, quiz, opts);
            }
            //SerializeAsync(
            // where to write,
            // what to write,
            //how to format it
            //)
        }



        // LOAD all quizzes in folder
        public static async Task<List<Quiz>> LoadAllAsync()
        {
            var list = new List<Quiz>();

            Directory.CreateDirectory(QuizzFolder);

            // Loop through every file that ends with ".json" in the QuizzFolder.
            foreach (var file in Directory.EnumerateFiles(QuizzFolder, "*.json"))
            {
                // Open the file for reading. FileStream is created.
                using (var fs = File.OpenRead(file))
                {
                    // Deserialize JSON in the file into a QuizDto object.
                    var dto = await JsonSerializer.DeserializeAsync<Quiz>(fs);
                    // Only add to the list if deserialization succeeded.
                    if (dto != null) list.Add(dto);
                }
            }
            return list;
        }

        // ---- helpers ----

        private static string SanitizeFileName(string name)
        {
            if (name == null) return "";
            // Replace every invalid file character with '_'
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(c, '_');
            }
            // Remove spaces or tabs at start and end
            return name.Trim();
        }



        private static async Task CopyInitialIfEmptyAsync()
        {
            // if already has files or subfolders → do nothing
            if (Directory.Exists(QuizzFolder))
            {
                //If there’s anything in the folder(files or subfolders), skip copying.
                //This ensures default data is only copied once—on first launch.
                if (Directory.EnumerateFileSystemEntries(QuizzFolder).Any())
                {
                    return;
                }
            }
            // Build the path to a folder inside your app's base directory
            string src = Path.Combine(AppContext.BaseDirectory, "InitialQuizzes");

            // If that folder doesn’t exist → stop.
            if (!Directory.Exists(src)) return;

            // Run the file copy operation on a background thread
            // so the UI doesn’t freeze.
            await Task.Run(() => CopyAll(src, QuizzFolder));
        }






        private static void CopyAll(string source, string destination)
        {
            Directory.CreateDirectory(destination);


            // Create all subfolders from source to destination
            foreach (var dir in Directory.EnumerateDirectories(source, "*", SearchOption.AllDirectories))
            {
                string targetDir = dir.Replace(source, destination);
                Directory.CreateDirectory(targetDir);
            }

            // Copy every file, keeping the same structure
            foreach (var file in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
            {
                string targetFile = file.Replace(source, destination);
                // Only copy if file doesn’t already exist
                if (!File.Exists(targetFile))
                {
                    // Ensure the folder for this file exists
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                    // Copy the file, do not overwrite existing ones (false = no overwrite)
                    File.Copy(file, targetFile, false);
                }
            }
        }

    }
}
