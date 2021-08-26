using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
        [DataType(DataType.Date)] 
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
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

            // Fail - PIN isn't 9-10 numbers separated by '/'
            if (!Regex.IsMatch(this.PersonalIdentificationNumber, "[0-9]{6}/[0-9]{3,4}"))
                return false;

            bool
                RC  = false, // PIN
                ECP = false; // registration number of the insured

            // Separate numbers to YYMMDD/CCC(C)

            UInt32
                YY,     // Year
                MM,     // Month
                DD,     // Day of month
                CCCC;   // Check sum

            int CCCC_length = this.PersonalIdentificationNumber.Length - 7;

            try
            {
                YY      = Convert.ToUInt32(this.PersonalIdentificationNumber.Substring(0, 2));
                MM      = Convert.ToUInt32(this.PersonalIdentificationNumber.Substring(2, 2));
                DD      = Convert.ToUInt32(this.PersonalIdentificationNumber.Substring(4, 2));
                CCCC    = Convert.ToUInt32(this.PersonalIdentificationNumber.Substring(7, CCCC_length));
            }
            catch (Exception)
            {
                throw;
            }

            // Fail - Last three digits can't be zeroes
            if (CCCC == 0)
                return false;

            // Determine month and gender
            if (MM > 50)
            {
                // Female
                if (this.Gender != GenderEnum.Female)
                    return false;

                MM -= 50;
            }
            else
            {
                // Male
                if (this.Gender != GenderEnum.Male)
                    return false;
            }
            if (MM > 20)
            {
                RC = true;
                MM -= 20;
            }

            // Calculate day
            if (DD > 40)
            {
                ECP = true;
                DD -= 40;
            }

            // Invalid RČ / EČP month +20 and at the same time day +40.
            if (ECP && RC)
                return false;

            // Determine year
            if (CCCC_length == 3)
            {
                if (YY > 53)
                {
                    YY = 1800 + YY;
                }
                else
                {
                    YY = 1900 + YY;
                }
            }
            if (CCCC_length == 4)
            {
                if (YY > 53)
                {
                    YY = 1900 + YY;
                }
                else
                {
                    YY = 2000 + YY;
                }
            }

            // Validate date
            DateTime dateOfBirth;
            string dayOfBirthAsString = YY.ToString() + "/" + MM.ToString() + "/" + DD.ToString();
            if (DateTime.TryParseExact( dayOfBirthAsString, 
                                        "yyyy/MM/dd", 
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out dateOfBirth )
               )
            {
                // DD/MM/YYYY is a valid date
                if (!dateOfBirth.Date.Equals(this.DateOfBirth))
                {
                    // Dates of birth are different
                    return false;
                }
            }
            else
            {
                // Data are not a valid date
                return false;
            }

            // Inadmissible ending of EČP.
            if (ECP)
            {
                if (this.PersonalIdentificationNumber.Length == 10
                    && CCCC < 600)
                    return false;

                if (this.PersonalIdentificationNumber.Length == 11
                    && CCCC < 6000)
                    return false;
            }

            // RC has to be divisible by 11
            if (this.PersonalIdentificationNumber.Length == 10)
            {
                string noSlashPIN = this.PersonalIdentificationNumber.Substring(0, 6) 
                    + this.PersonalIdentificationNumber.Substring(7, CCCC_length);
                UInt32 uIntRC;
                try
                {
                    uIntRC = Convert.ToUInt32(noSlashPIN);
                    if ((uIntRC % 11) != 0)
                    {
                        return false;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }


            return true;
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
