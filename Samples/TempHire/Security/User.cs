using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Cocktail;
using IdeaBlade.Aop;
using IdeaBlade.Validation;

namespace Security
{
    [ProvideEntityAspect]
    [DataContract(IsReference = true)]
    public class User
    {
        internal User()
        {
        }

        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; internal set; }

        [DataMember]
        [Required]
        public string Username { get; set; }

        [DataMember]
        [Required]
        public string Password { get; set; }

        public static User Create()
        {
            return new User { Id = CombGuid.NewGuid() };
        }
    }
}