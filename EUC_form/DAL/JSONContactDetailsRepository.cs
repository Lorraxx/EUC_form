using EUC_form.Data;
using EUC_form.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EUC_form.DAL.Templates
{
    public class JSONContactDetailsRepository : JSONRepository<ContactDetails>, IContactDetailsRepository
    {
        public JSONContactDetailsRepository() : base()
        {
        }
    }
}
