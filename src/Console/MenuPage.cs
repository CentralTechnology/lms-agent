﻿namespace EasyConsole
{
    public abstract class MenuPage : Page
    {
        protected MenuPage(string title, Program program, params Option[] options)
            : base(title, program)
        {
            Menu = new Menu();

            foreach (Option option in options)
            {
                Menu.Add(option);
            }
        }

        public Menu Menu { get; set; }

        public override void Display()
        {
            base.Display();

            this.AddBackOption();

            Menu.Display();
        }
    }
}