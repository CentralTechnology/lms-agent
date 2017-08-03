namespace Service.Menu
{
    using System;
    using EasyConsole;
    using Pages;
    using Pages.Options;
    using Pages.Run;

    class ClientProgram : Program
    {
        public ClientProgram(Guid adminAccess)
            : base("License Monitoring System", true)
        {
            AddPage(new MainPage(this, adminAccess));

            AddPage(new RunPage(this));

            AddPage(new OptionsPage(this));
            AddPage(new GeneralPage(this));
            AddPage(new UsersPage(this));
            AddPage(new VeeamPage(this));

            SetPage<MainPage>();
        }

        public override void Run()
        {
            Console.Title = Title;

            CurrentPage.Display();
        }
    }
}