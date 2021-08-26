using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EUC_form.Data;
using EUC_form.Models;
using EUC_form.DAL;
using System.Globalization;
using EUC_form.Helpers;
using System.Text.Json;
using System.Xml.Serialization;
using System.IO;
using System.Text.Json.Serialization;

namespace EUC_form.Controllers
{
    public class ContactDetailsController : Controller
    {
        private readonly IContactDetailsRepository _repository;

        public ContactDetailsController(IContactDetailsRepository repository)
        {
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _repository.GetAll());
        }

        public async Task<IActionResult> GetJson()
        {
            var toSerialize = await _repository.GetAll();
            JsonSerializerOptions serializerOptions = new()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };
            return Content(JsonSerializer.Serialize(
                toSerialize, 
                toSerialize.GetType(),
                serializerOptions));
        }

        public async Task<IActionResult> GetXML()
        {
            var toSerialize = await _repository.GetAll();
            XmlSerializer serializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter())
            {
                serializer.Serialize(textWriter, toSerialize);
                return Content(textWriter.ToString());
            }
        }

        public IActionResult Create()
        {
            ViewBag.CountryList = HelperFunctions.GetCountryList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
            "Email," +
            "PersonalIdentificationNumber," +
            "LastName,FirstName," +
            "Nationality,DateOfBirth," +
            "Gender,"
            )] 
            ContactDetails contactDetails)
        {
            //
            if (ModelState.IsValid)
            {
                if (contactDetails.IsValid())
                {
                    await _repository.Add(contactDetails);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.ServerSideValidationError = true;
                }
            }
            ViewBag.CountryList = HelperFunctions.GetCountryList();
            return View(contactDetails);
        }
    }
}
