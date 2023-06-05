using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data.Context;
using Data.Entities;
using AutoMapper;
using RestSharp;
using SchoolInfoMVC.Utility;
using ApplicationService.DTOs;
using Newtonsoft.Json;
using SchoolInfoMVC.Models;
using System.Net;
using NuGet.DependencyResolver;
using SchoolInfoMVC.Filters;
using SchoolInfoMVC.Models.IndexVMs;

namespace SchoolInfoMVC.Controllers
{
    [Authenticated]
    public class ClassesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RestClient _restClient;
        private readonly Mapper _mapper;

        public ClassesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _restClient = new RestClient(_configuration.GetSection("ApiBaseUrl").Value!);
            _mapper = MapperConfig.InitializeAutomapper();
        }

        private void AddAuthorizationHeader(RestRequest request)
        {
            string token = Request.Cookies["JWT-Access-Token"]?.Replace("\"", "")!;
            request.AddHeader("Authorization", $"Bearer {token}");
        }

        private async Task GetPageCount(ClassIndexVM model, string? className = null)
        {
            RestRequest pageCountRequest = new RestRequest($"Classes/PageCount/{model.Pager.ItemsPerPage}" +
                (!string.IsNullOrEmpty(className)? $"/{className}" : ""), Method.Get);
            AddAuthorizationHeader(pageCountRequest);
            RestResponse pageCountResponse = await _restClient.ExecuteAsync(pageCountRequest);

            if (pageCountResponse.IsSuccessStatusCode && pageCountResponse.Content != null)
            {
                model.Pager.PagesCount = int.Parse(pageCountResponse.Content.ToString());
            }
        }

        // GET: Classes
        public async Task<IActionResult> Index(ClassIndexVM model, string? className = null)
        {
            RestRequest request = new RestRequest($"Classes/{model.Pager.Page}/{model.Pager.ItemsPerPage}" +
                (!string.IsNullOrEmpty(className) ? $"/{className}" : ""), Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);
            List<ClassVM> classVMs = new List<ClassVM>();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                List<ClassDTO> classDTOs = JsonConvert.DeserializeObject<List<ClassDTO>>(response.Content)!;

                foreach (var classDTO in classDTOs)
                {
                    classVMs.Add(_mapper.Map<ClassDTO, ClassVM>(classDTO));
                }
            }

            model.Classes = classVMs;
            await GetPageCount(model, className);

            return View(model);
        }

        // GET: Classes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest request = new RestRequest($"Classes/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            ClassVM classVM = new ClassVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                ClassDTO classDTO = JsonConvert.DeserializeObject<ClassDTO>(response.Content)!;
                classVM = _mapper.Map<ClassDTO, ClassVM>(classDTO);
            }

            return View(classVM);
        }

        // GET: Classes/Create
        public async Task<IActionResult> Create()
        {
            List<TeacherVM> teachersVMs = await GetTeachersWithoutClass();

            ViewData["FormTeacherId"] = new SelectList(teachersVMs, "Id", "GetFullName");
            return View();
        }

        // POST: Classes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClassVM classVM)
        {
            if (ModelState.IsValid)
            {
                RestRequest requestForClassCreation = new RestRequest("Classes", Method.Post);
                AddAuthorizationHeader(requestForClassCreation);
                SaveClassDTO saveClassDTO = _mapper.Map<ClassVM, SaveClassDTO>(classVM);
                requestForClassCreation.AddJsonBody(saveClassDTO);
                RestResponse responseForClassCreation = await _restClient.ExecuteAsync(requestForClassCreation);

                if (responseForClassCreation.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to create class!";
                }
            }

            List<TeacherVM> teachersVMs = await GetTeachersWithoutClass();

            ViewData["FormTeacherId"] = new SelectList(teachersVMs, "Id", "GetFullName", classVM.FormTeacherId);

            return View(classVM);
        }

        // GET: Classes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest request = new RestRequest($"Classes/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            ClassVM classVM = new ClassVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                ClassDTO classDTO = JsonConvert.DeserializeObject<ClassDTO>(response.Content)!;
                classVM = _mapper.Map<ClassDTO, ClassVM>(classDTO);
            }

            List<TeacherVM> teachersVMs = await GetTeachersWithoutClass();
            //Including the form teacher of the edited class in case we don't want to change them
            teachersVMs.Add(_mapper.Map<TeacherDTO, TeacherVM>(classVM.FormTeacher!));
            ViewData["FormTeacherId"] = new SelectList(teachersVMs, "Id", "GetFullName", classVM.FormTeacherId);

            return View(classVM);
        }

        // POST: Classes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClassVM classVM)
        {
            if (ModelState.IsValid)
            {
                RestRequest request = new RestRequest("Classes", Method.Post);
                AddAuthorizationHeader(request);
                SaveClassDTO saveClassDTO = _mapper.Map<ClassVM, SaveClassDTO>(classVM);
                request.AddJsonBody(saveClassDTO);
                RestResponse response = await _restClient.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to edit class!";
                }
            }

            List<TeacherVM> teachersVMs = await GetTeachersWithoutClass();
            /*Including the form teacher of the edited class in case we don't want to change them.
              In this case we pass only the id and the name as temp data from the view to this action.*/
            classVM.FormTeacher = new TeacherDTO()
            {
                Id = (int)TempData["CurrentFormTeacherId"]!,
                FirstName = TempData["CurrentFormTeacherFirstName"]!.ToString()!,
                LastName = TempData["CurrentFormTeacherLastName"]!.ToString()!,
                Phone = null!
            };
            teachersVMs.Add(_mapper.Map<TeacherDTO, TeacherVM>(classVM.FormTeacher));
            ViewData["FormTeacherId"] = new SelectList(teachersVMs, "Id", "GetFullName", classVM.FormTeacherId);

            return View(classVM);
        }

        // GET: Classes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest request = new RestRequest($"Classes/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            ClassVM classVM = new ClassVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                ClassDTO classDTO = JsonConvert.DeserializeObject<ClassDTO>(response.Content)!;
                classVM = _mapper.Map<ClassDTO, ClassVM>(classDTO);
            }

            return View(classVM);
        }

        // POST: Classes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            RestRequest request = new RestRequest($"Classes/{id}", Method.Delete);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //It will be passed to redirected page
                TempData["ErrorMessage"] = "Failed to delete class!";
                return RedirectToAction(nameof(Delete));
            }
        }

        private async Task<List<TeacherVM>> GetTeachersWithoutClass()
        {
            RestRequest requestForTeachers = new RestRequest("Teachers/WithoutClass", Method.Get);
            AddAuthorizationHeader(requestForTeachers);
            RestResponse responseForTeachers = await _restClient.ExecuteAsync(requestForTeachers);
            List<TeacherVM> teachersVMs = new List<TeacherVM>();

            if (responseForTeachers.StatusCode == HttpStatusCode.OK && responseForTeachers.Content != null)
            {
                List<TeacherDTO> teacherDTOs = JsonConvert.DeserializeObject<List<TeacherDTO>>(responseForTeachers.Content)!;

                foreach (var teacherDTO in teacherDTOs)
                {
                    teachersVMs.Add(_mapper.Map<TeacherDTO, TeacherVM>(teacherDTO));
                }
            }

            return teachersVMs;
        }
    }
}
