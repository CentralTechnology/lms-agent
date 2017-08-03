namespace EasyConsole
{
    using System;

    public class Option
    {
        public Option(string name, Action callback)
        {
            Name = name;
            Callback = callback;
        }

        public Action Callback { get; private set; }
        public string Name { get; private set; }

        public override string ToString()
        {
            return Name;
        }
    }
}