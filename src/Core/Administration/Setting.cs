namespace Core.Administration
{
    using System.ComponentModel.DataAnnotations;
    using Abp.Domain.Entities;

    public class Setting : Entity
    {
        public Setting()
        {
        }

        public Setting(string name, string value)
        {
            Name = name;
            Value = value;
        }

        [Required]
        public string Name { get; set; }

        public string Value { get; set; }
    }
}