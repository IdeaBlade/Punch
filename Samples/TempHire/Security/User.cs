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
        [RequiredValueVerifier(ErrorMessageResourceName = "User_Id")]
        public Guid Id { get; internal set; }

        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "User_Username")]
        public string Username { get; set; }

        [DataMember]
        [Required]
        [RequiredValueVerifier(ErrorMessageResourceName = "User_Password")]
        public string Password { get; set; }

        public static User Create()
        {
            return new User { Id = CombGuid.NewGuid() };
        }
    }
}