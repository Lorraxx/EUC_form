using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EUC_form.Models.Templates;

namespace EUC_form.Models
{
    // Todo: Separate view model from EF model
    public class ContactDetails : BaseEntity
    {
        private const int minNameLength = 2;
        private const int maxNameLength = 50;

        public enum GenderEnum {
            Male,
            Female,
            Other
        }

        [Display(
            Name = nameof(Localization.ContactDetailResx.Email), 
            ResourceType = typeof(Localization.ContactDetailResx))]
        [Required(
            ErrorMessageResourceName = nameof(Localization.ContactDetailResx.ErrorMessage_Email_Required), 
            ErrorMessageResourceType = typeof(Localization.ContactDetailResx))]
        [EmailAddress(
            ErrorMessageResourceName = nameof(Localization.ContactDetailResx.ErrorMessage_Email_Invalid), 
            ErrorMessageResourceType = typeof(Localization.ContactDetailResx))]
        public string Email { get; set; }

        [Display(
            Name = nameof(Localization.ContactDetailResx.PIN), 
            ResourceType = typeof(Localization.ContactDetailResx))]
        //[Required(AllowEmptyStrings = false)]
        [MaxLength(11)]
        [MinLength(10)]
        [RegularExpression("[0-9]{6}/[0-9]{3,4}", 
            ErrorMessageResourceName = nameof(Localization.ContactDetailResx.ErrorMessage_PIN), 
            ErrorMessageResourceType = typeof(Localization.ContactDetailResx))]
        public string PersonalIdentificationNumber { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(
            Name = nameof(Localization.ContactDetailResx.LastName), 
            ResourceType = typeof(Localization.ContactDetailResx))]
        [StringLength(maxNameLength, MinimumLength = minNameLength)]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(
            Name = nameof(Localization.ContactDetailResx.FirstName), 
            ResourceType = typeof(Localization.ContactDetailResx))]
        [StringLength(maxNameLength, MinimumLength = minNameLength)]
        public string FirstName { get; set; }

        [Required]
        [Display(
            Name = nameof(Localization.ContactDetailResx.Nationality), 
            ResourceType = typeof(Localization.ContactDetailResx))]
        public int Nationality { get; set; } // FK to table with nations

        [Required]
        [Display(
            Name = nameof(Localization.ContactDetailResx.DOB), 
            ResourceType = typeof(Localization.ContactDetailResx))]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Display(
            Name = nameof(Localization.ContactDetailResx.Gender), 
            ResourceType = typeof(Localization.ContactDetailResx))]
        public GenderEnum Gender { get; set; }


        // Server side validation check
        internal bool IsValid()
        {
            return this.EmailIsValid()
                && this.PersonalIdentificationNumberIsValid()
                && this.LastNameIsValid()
                && this.FirstNameIsValid()
                && this.NationalityIsValid()
                && this.DateOfBirthIsValid()
                && this.GenderIsValid();
        }

        private bool GenderIsValid()
        {
            // enum
            return true;
        }

        private bool DateOfBirthIsValid()
        {
            return (this.DateOfBirth.Date <= DateTime.Today.Date);
        }

        private bool NationalityIsValid()
        {
            return (this.Nationality >= 0 && this.Nationality < Helpers.HelperFunctions.GetCountryList().Count);
        }

        private bool FirstNameIsValid()
        {
            return (this.FirstName.Length >= minNameLength && this.FirstName.Length <= maxNameLength);
        }

        private bool LastNameIsValid()
        {
            return (this.LastName.Length >= minNameLength && this.LastName.Length <= maxNameLength);
        }

        private bool PersonalIdentificationNumberIsValid()
        {
            if (String.IsNullOrEmpty(this.PersonalIdentificationNumber))
            {
                // NULL values are allowed
                this.PersonalIdentificationNumber = null;
                return true;
            }
            else
            {
                return Regex.IsMatch(this.PersonalIdentificationNumber, "[0-9]{6}/[0-9]{3,4}");
            }

            // todo: Validate with the PIN checksum.
        }

        private bool EmailIsValid()
        {
            try
            {
                MailAddress m = new MailAddress(this.Email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
