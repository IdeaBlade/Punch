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

using IdeaBlade.EntityModel;

namespace DomainModel
{
    public class TempHireEntities : EntityManager
    {
        static TempHireEntities()
        {
            // Disable TransactionScope. SQL Compact doesn't support TransactionScopes.
            TransactionSettings.Default = new TransactionSettings(TransactionSettings.Default.IsolationLevel,
                                                                  TransactionSettings.Default.Timeout, false);
        }

        public TempHireEntities(EntityManagerContext context) : base(context)
        {
        }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for StaffingResource entities. </summary>
        public EntityQuery<StaffingResource> StaffingResources { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for Address entities. </summary>
        public EntityQuery<Address> Addresses { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for PhoneNumber entities. </summary>
        public EntityQuery<PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for AddressType entities. </summary>
        public EntityQuery<AddressType> AddressTypes { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for PhoneNumberType entities. </summary>
        public EntityQuery<PhoneNumberType> PhoneNumberTypes { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for Rate entities. </summary>
        public EntityQuery<Rate> Rates { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for RateType entities. </summary>
        public EntityQuery<RateType> RateTypes { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for WorkExperienceItem entities. </summary>
        public EntityQuery<WorkExperienceItem> WorkExperienceItems { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for Skill entities. </summary>
        public EntityQuery<Skill> Skills { get; set; }

        /// <summary>Gets the <see cref="T:EntityQuery"/> for State entities. </summary>
        public EntityQuery<State> States { get; set; }
    }
}