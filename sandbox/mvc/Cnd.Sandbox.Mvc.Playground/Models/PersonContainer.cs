using Cnd.Core.Common;

namespace Cnd.Sandbox.Mvc.Playground.Models
{
    public class Person : IEventEntity
    {
        public Person()
        {

        }
        public int Id {get;set;} = 1;
        public string Name { get; set; }
        public int Age {get;set;} = 10;
        public string Gender {get;set;} = "Female";
        public bool Updated {get;set;}
    }

    public static class PersonContainer
    {
        public static readonly Person user = new Person{ Id= 1, Name ="Firat", Age = 35 , Gender = "Male" };

    }

    public class PersonViewModel
    {
        public int Id {get;set;} =1;
        public string Name { get; set; }
        public int Age {get;set;} = 10;
        public string Gender {get;set;} = "Female";
    }

}