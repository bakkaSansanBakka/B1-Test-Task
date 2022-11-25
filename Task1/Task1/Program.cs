namespace Task1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // change number format of gloating point numbers
            Configuration.ConfigureCulture();
            while(true)
            {
                CallMenu();
            }
        }

        public static void CallMenu()
        {
            Console.WriteLine("*****************************************");
            Console.WriteLine("Menu");
            Console.WriteLine("1. Generate 100 files with 100 000 lines each");
            Console.WriteLine("2. Merge existing files in one with delete of lines " +
                "that contain given combination");
            Console.WriteLine("*****************************************\n");

            Console.WriteLine("Enter menu option:");

            switch (Console.ReadLine())
            {
                case "1": 
                    TextFilesManager.CreateFiles();
                    PressKeyAndClearConsole();
                    break;
                case "2":
                    TextFilesManager.MergeFilesAndDeleteStringWithCharacterCombination();
                    PressKeyAndClearConsole();
                    break;
                default:
                    Console.WriteLine("Entered value is not a menu option! Please try again.\n");
                    PressKeyAndClearConsole();
                    break;

            }
        }

        public static void PressKeyAndClearConsole()
        {
            Console.WriteLine("(Press any key)");
            Console.ReadLine();
            Console.Clear();
        }
    }
}