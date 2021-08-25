using EUC_form.Models;
using EUC_form.DAL.Templates;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EUC_form.DAL
{
    public interface IContactDetailsRepository : IAsyncRepository<ContactDetails>
    {
    }
}
