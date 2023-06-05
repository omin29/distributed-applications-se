using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Data.Context;
using Data.Entities;
using RestSharp;
using ApplicationService.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using SchoolInfoMVC.Models;
using SchoolInfoMVC.Utility;
using AutoMapper;
using Azure.Core;
using SchoolInfoMVC.Filters;
using SchoolInfoMVC.Models.IndexVMs;
using Microsoft.AspNetCore.Http.Features;

namespace SchoolInfoMVC.Controllers
{
    [Authenticated]
    public class TeachersController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RestClient _restClient;
        private readonly Mapper _mapper;

        public TeachersController(IConfiguration configuration)
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

        private async Task GetPageCount(TeacherIndexVM model, string? firstName = null, string? lastName = null)
        {
            RestRequest pageCountRequest = new RestRequest($"Teachers/PageCount/{model.Pager.ItemsPerPage}/" +
                (!string.IsNullOrEmpty(firstName) ? $"{firstName}" : "") +
                (!string.IsNullOrEmpty(lastName) ? $"_{lastName}" : ""), Method.Get);
            AddAuthorizationHeader(pageCountRequest);
            RestResponse pageCountResponse = await _restClient.ExecuteAsync(pageCountRequest);

            if (pageCountResponse.IsSuccessStatusCode && pageCountResponse.Content != null)
            {
                model.Pager.PagesCount = int.Parse(pageCountResponse.Content.ToString());
            }
        }

        // GET: Teachers
        public async Task<IActionResult> Index(TeacherIndexVM model, string? firstName = null, string? lastName = null)
        {
            RestRequest request = new RestRequest($"Teachers/{model.Pager.Page}/{model.Pager.ItemsPerPage}/" +
                (!string.IsNullOrEmpty(firstName)?$"{firstName}":"") +
                (!string.IsNullOrEmpty(lastName)?$"_{lastName}":""), Method.Get);
            AddAuthorizationHeader(request);

            RestResponse response = await _restClient.ExecuteAsync(request);
            List<TeacherVM> teachersVMs = new List<TeacherVM>();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                List<TeacherDTO> teacherDTOs = JsonConvert.DeserializeObject<List<TeacherDTO>>(response.Content)!;

                foreach (var teacherDTO in teacherDTOs)
                {
                    teachersVMs.Add(_mapper.Map<TeacherDTO, TeacherVM>(teacherDTO));
                }
            }

            model.Teachers = teachersVMs;
            await GetPageCount(model, firstName, lastName);

            return View(model);
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest requestForTeacher = new RestRequest($"Teachers/{id}", Method.Get);
            AddAuthorizationHeader(requestForTeacher);
            RestResponse responseForTeacher = await _restClient.ExecuteAsync(requestForTeacher);

            if(responseForTeacher.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            TeacherVM teacherVM = new TeacherVM();

            if (responseForTeacher.StatusCode == HttpStatusCode.OK && responseForTeacher.Content != null)
            {
                TeacherDTO teacherDTO = JsonConvert.DeserializeObject<TeacherDTO>(responseForTeacher.Content)!;
                teacherVM = _mapper.Map<TeacherDTO, TeacherVM>(teacherDTO);
            }

            RestRequest requestForTaughtClasses = new RestRequest($"TeachersClasses/ForTeacher/{teacherVM.Id}/{false}");
            AddAuthorizationHeader(requestForTaughtClasses);
            RestResponse responseForTaughtClasses = await _restClient.ExecuteAsync(requestForTaughtClasses);

            List<TeacherClassVM> taughtClasses = new List<TeacherClassVM>();

            if(responseForTaughtClasses.StatusCode == HttpStatusCode.OK && responseForTaughtClasses.Content != null)
            {
                List<TeacherClassDTO> teacherClassDTOs = JsonConvert
                    .DeserializeObject<List<TeacherClassDTO>>(responseForTaughtClasses.Content)!;

                foreach (var teacherClassDTO in teacherClassDTOs)
                {
                    taughtClasses.Add(_mapper.Map<TeacherClassDTO, TeacherClassVM>(teacherClassDTO));
                }
            }

            teacherVM.TaughtClasses = taughtClasses;

            return View(teacherVM);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TeacherVM teacher)
        {
            if (ModelState.IsValid)
            {
                RestRequest request = new RestRequest("Teachers", Method.Post);
                AddAuthorizationHeader(request);
                TeacherDTO teacherDTO = _mapper.Map<TeacherVM, TeacherDTO>(teacher);
                request.AddJsonBody(teacherDTO);
                RestResponse response = await _restClient.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to create teacher!";
                    return View(teacher);
                }
            }

            return View(teacher);
        }

        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest request = new RestRequest($"Teachers/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            TeacherVM teacherVM = new TeacherVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                TeacherDTO teacherDTO = JsonConvert.DeserializeObject<TeacherDTO>(response.Content)!;
                teacherVM = _mapper.Map<TeacherDTO, TeacherVM>(teacherDTO);
            }

            return View(teacherVM);
        }

        // POST: Teachers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TeacherVM teacher)
        {
            if (ModelState.IsValid)
            {
                RestRequest request = new RestRequest("Teachers", Method.Post);
                AddAuthorizationHeader(request);
                TeacherDTO teacherDTO = _mapper.Map<TeacherVM, TeacherDTO>(teacher);
                request.AddJsonBody(teacherDTO);
                RestResponse response = await _restClient.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to edit teacher!";
                    return View(teacher);
                }
            }
            return View(teacher);
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest request = new RestRequest($"Teachers/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            TeacherVM teacherVM = new TeacherVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                TeacherDTO teacherDTO = JsonConvert.DeserializeObject<TeacherDTO>(response.Content)!;
                teacherVM = _mapper.Map<TeacherDTO, TeacherVM>(teacherDTO);
            }

            return View(teacherVM);
        }

        // POST: Teachers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            RestRequest request = new RestRequest($"Teachers/{id}", Method.Delete);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //It will be passed to redirected page
                TempData["ErrorMessage"] = "Failed to delete teacher!";
                return RedirectToAction(nameof(Delete));
            }
        }

        public async Task<IActionResult> AddClass(int? teacherId, string? teacherName)
        {
            if(teacherId == null || teacherName == null)
            {
                return BadRequest();
            }

            RestRequest requestForTeachableClasses = new RestRequest($"TeachersClasses/ForTeacher/{teacherId}/{true}");
            AddAuthorizationHeader(requestForTeachableClasses);
            RestResponse responseForTeachableClasses = await _restClient.ExecuteAsync(requestForTeachableClasses);

            List<TeacherClassVM> teachableClasses = new List<TeacherClassVM>();

            if (responseForTeachableClasses.StatusCode == HttpStatusCode.OK && responseForTeachableClasses.Content != null)
            {
                List<TeacherClassDTO> teacherClassDTOs = JsonConvert
                    .DeserializeObject<List<TeacherClassDTO>>(responseForTeachableClasses.Content)!;

                foreach (var teacherClassDTO in teacherClassDTOs)
                {
                    teachableClasses.Add(_mapper.Map<TeacherClassDTO, TeacherClassVM>(teacherClassDTO));
                }
            }

            ViewData["TeacherId"] = teacherId;
            ViewData["TeacherName"] = teacherName;
            ViewData["ClassId"] = new SelectList(teachableClasses, "ClassId", "GetClassName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddClass(TeacherClassVM teacherClassVM, string? teacherName)
        {
            if (ModelState.IsValid)
            {
                RestRequest request = new RestRequest("TeachersClasses", Method.Post);
                AddAuthorizationHeader(request);
                SaveTeacherClassDTO saveTeacherClassDTO = _mapper.Map<TeacherClassVM, SaveTeacherClassDTO>(teacherClassVM);
                request.AddJsonBody(saveTeacherClassDTO);
                RestResponse response = await _restClient.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Details), new {id = teacherClassVM.TeacherId});
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to add class for teacher!";
                }
            }

            RestRequest requestForTeachableClasses = new RestRequest(
                $"TeachersClasses/ForTeacher/{teacherClassVM.TeacherId}/{true}");
            AddAuthorizationHeader(requestForTeachableClasses);
            RestResponse responseForTeachableClasses = await _restClient.ExecuteAsync(requestForTeachableClasses);

            List<TeacherClassVM> teachableClasses = new List<TeacherClassVM>();

            if (responseForTeachableClasses.StatusCode == HttpStatusCode.OK && responseForTeachableClasses.Content != null)
            {
                List<TeacherClassDTO> teacherClassDTOs = JsonConvert
                    .DeserializeObject<List<TeacherClassDTO>>(responseForTeachableClasses.Content)!;

                foreach (var teacherClassDTO in teacherClassDTOs)
                {
                    teachableClasses.Add(_mapper.Map<TeacherClassDTO, TeacherClassVM>(teacherClassDTO));
                }
            }

            ViewData["TeacherId"] = teacherClassVM.TeacherId;
            ViewData["TeacherName"] = teacherName;
            ViewData["ClassId"] = new SelectList(teachableClasses, "ClassId", "GetClassName", teacherClassVM.ClassId);

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveClass(int id, int teacherId)
        {
            RestRequest request = new RestRequest($"TeachersClasses/{id}", Method.Delete);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                //It will be passed to redirected page
                TempData["ErrorMessage"] = "Failed to remove class for teacher!";
            }

            return RedirectToAction(nameof(Details), new { id = teacherId });
        }
    }
}
