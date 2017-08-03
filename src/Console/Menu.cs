namespace EasyConsole
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Menu
    {
        public Menu()
        {
            Options = new List<Option>();
        }

        private IList<Option> Options { get; set; }

        public Menu Add(string option, Action callback)
        {
            return Add(new Option(option, callback));
        }

        public Menu Add(Option option)
        {
            Options.Add(option);
            return this;
        }

        public bool Contains(string option)
        {
            return Options.FirstOrDefault(op => op.Name.Equals(option)) != null;
        }

        public void Display()
        {
            for (int i = 0; i < Options.Count; i++)
            {
                Console.WriteLine("{0}. {1}", i + 1, Options[i].Name);
            }
            int choice = Input.ReadInt("Choose an option:", 1, Options.Count);

            Options[choice - 1].Callback();
        }
    }
}