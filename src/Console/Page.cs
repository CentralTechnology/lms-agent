namespace EasyConsole
{
    using System;
    using System.Linq;

    public abstract class Page
    {
        public Page(string title, Program program)
        {
            Title = title;
            Program = program;
        }

        public Program Program { get; set; }
        public string Title { get; private set; }

        public virtual void Display()
        {
            if (Program.History.Count > 1 && Program.BreadcrumbHeader)
            {
                string breadcrumb = null;
                foreach (string title in Program.History.Select(page => page.Title).Reverse())
                    breadcrumb += title + " > ";
                breadcrumb = breadcrumb.Remove(breadcrumb.Length - 3);
                Console.WriteLine(breadcrumb);
            }
            else
            {
                Console.WriteLine(Title);
            }
            Console.WriteLine("---");
        }
    }
}