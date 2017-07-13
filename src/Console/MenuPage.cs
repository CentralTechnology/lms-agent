namespace EasyConsole
{
    public abstract class MenuPage : Page
    {
        public Menu Menu { get; set; }

        protected MenuPage(string title, Program program, params Option[] options)
            : base(title, program)
        {
            Menu = new Menu();

            foreach (var option in options)
            {
                Menu.Add(option);
            }               
        }

        public override void Display()
        {
            base.Display();

            this.AddBackOption();

            Menu.Display();
        }
    }
}
