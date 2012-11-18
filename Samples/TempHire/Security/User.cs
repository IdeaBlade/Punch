//====================================================================================================================
// Copyright (c) 2012 IdeaBlade
//====================================================================================================================
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS 
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
//====================================================================================================================
// USE OF THIS SOFTWARE IS GOVERENED BY THE LICENSING TERMS WHICH CAN BE FOUND AT
// http://cocktail.ideablade.com/licensing
//====================================================================================================================

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Runtime.Serialization;
using Cocktail;
using IdeaBlade.Aop;
using IdeaBlade.EntityModel;

namespace Security
{
    [ProvideEntityAspect]
    [DataContract(IsReference = true)]
    [ClientCanQuery(false)]
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
        public byte[] Password { get; set; }

        public static User Create()
        {
            return new User {Id = CombGuid.NewGuid()};
        }

        public void SetPassword(string password)
        {
            var key = CryptoHelper.GenerateKey(password);
            var stream = new MemoryStream();

            var length = CryptoHelper.EncryptToStream(
                stream, key, cs =>
                    {
                        var writer = new StreamWriter(cs);
                        writer.WriteLine(password);
                        writer.Flush();
                    });

            var encryptedPassword = stream.GetBuffer();
            Array.Resize(ref encryptedPassword, length);

            Password = encryptedPassword;
        }

        public bool Authenticate(string password)
        {
            try
            {
                var key = CryptoHelper.GenerateKey(password);
                var stream = new MemoryStream(Password);

                var decryptedPassword = CryptoHelper.DecryptFromStream(
                    stream, key, cs =>
                    {
                        var reader = new StreamReader(cs);
                        return reader.ReadLine();
                    });

                return password == decryptedPassword;
            }
            catch (Exception)
            {
                return false;                
            }
        }
    }
}