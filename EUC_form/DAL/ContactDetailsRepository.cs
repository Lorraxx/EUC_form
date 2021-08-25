using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq.Expressions;
using EUC_form.Data;
using EUC_form.DAL.Templates;
using EUC_form.Models;
using System.Globalization;

namespace EUC_form.DAL
{
    public class ContactDetailsRepository : EfRepository<ContactDetails>, IContactDetailsRepository
    {
        public ContactDetailsRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
