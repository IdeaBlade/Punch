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
using System.Runtime.Serialization;
using Cocktail;

namespace DomainModel
{
    [DataContract(IsReference = true)]
    public class WorkExperienceItem : EntityBase, IHasRoot
    {
        internal WorkExperienceItem()
        {
        }

        /// <summary>Gets or sets the Id. </summary>
        [DataMember]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Required]
        public Guid Id { get; internal set; }

        /// <summary>Gets or sets the From. </summary>
        [DataMember]
        [Required]
        public DateTime From { get; set; }

        /// <summary>Gets or sets the To. </summary>
        [DataMember]
        [Required]
        public DateTime To { get; set; }

        /// <summary>Gets or sets the PositionTitle. </summary>
        [DataMember]
        [Required]
        public string PositionTitle { get; set; }

        /// <summary>Gets or sets the Company. </summary>
        [DataMember]
        [Required]
        public string Company { get; set; }

        /// <summary>Gets or sets the Location. </summary>
        [DataMember]
        [Required]
        public string Location { get; set; }

        /// <summary>Gets or sets the Description. </summary>
        [DataMember]
        [Required]
        public string Description { get; set; }

        /// <summary>Gets or sets the ResourceId. </summary>
        [DataMember]
        [Required]
        public Guid ResourceId { get; set; }

        /// <summary>Gets or sets the Resource. </summary>
        [DataMember]
        public Resource Resource { get; set; }

        #region IHasRoot Members

        public object Root
        {
            get { return Resource; }
        }

        #endregion

        internal static WorkExperienceItem Create()
        {
            return new WorkExperienceItem {Id = CombGuid.NewGuid()};
        }
    }
}