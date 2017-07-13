namespace EasyConsole
{
    using System;
    using System.Linq;

    public static class MenuPageExtensions
    {
        public static void AddBackOption(this MenuPage page)
        {
            if (page.Program.NavigationEnabled && !page.Menu.Contains("Go back"))
            {
                page.Menu.Add("Go back", () => { page.Program.NavigateBack(); });
            }
        }

        public static void AddBreadCrumb(this MenuPage page)
        {
            if (page.Program.History.Count > 1 && page.Program.BreadcrumbHeader)
            {
                string breadcrumb = string.Empty;
                foreach (string title in page.Program.History.Select(p => p.Title).Reverse())
                {
                    breadcrumb += title + " > ";
                }

                string output = breadcrumb.Remove(breadcrumb.Length - 3);

                Console.WriteLine(output);
            }
            else
            {
                Console.WriteLine(page.Title);
            }

            Console.WriteLine("---");
        }
    }
}