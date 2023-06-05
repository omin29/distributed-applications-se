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
using Azure.Core;
using SchoolInfoMVC.Models.IndexVMs;

namespace SchoolInfoMVC.Controllers
{
    [Authenticated]
    public class StudentsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly RestClient _restClient;
        private readonly Mapper _mapper;

        public StudentsController(IConfiguration configuration)
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

        private async Task GetPageCount(StudentIndexVM model, string? firstName = null, string? lastName = null)
        {
            RestRequest pageCountRequest = new RestRequest($"Students/PageCount/{model.Pager.ItemsPerPage}/" +
                (!string.IsNullOrEmpty(firstName) ? $"{firstName}" : "") +
                (!string.IsNullOrEmpty(lastName) ? $"_{lastName}" : ""), Method.Get);
            AddAuthorizationHeader(pageCountRequest);
            RestResponse pageCountResponse = await _restClient.ExecuteAsync(pageCountRequest);

            if (pageCountResponse.IsSuccessStatusCode && pageCountResponse.Content != null)
            {
                model.Pager.PagesCount = int.Parse(pageCountResponse.Content.ToString());
            }
        }

        // GET: Students
        public async Task<IActionResult> Index(StudentIndexVM model, string? firstName = null, string? lastName = null)
        {
            RestRequest request = new RestRequest($"Students/{model.Pager.Page}/{model.Pager.ItemsPerPage}/" +
                (!string.IsNullOrEmpty(firstName) ? $"{firstName}" : "") +
                (!string.IsNullOrEmpty(lastName) ? $"_{lastName}" : ""), Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);
            List<StudentVM> studentVMs = new List<StudentVM>();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                List<StudentDTO> studentDTOs = JsonConvert.DeserializeObject<List<StudentDTO>>(response.Content)!;

                foreach (var studentDTO in studentDTOs)
                {
                    studentVMs.Add(_mapper.Map<StudentDTO, StudentVM>(studentDTO));
                }
            }

            model.Students = studentVMs;
            await GetPageCount(model, firstName, lastName);

            return View(model);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest request = new RestRequest($"Students/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            StudentVM studentVM = new StudentVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                StudentDTO studentDTO = JsonConvert.DeserializeObject<StudentDTO>(response.Content)!;
                studentVM = _mapper.Map<StudentDTO, StudentVM>(studentDTO);
            }

            return View(studentVM);
        }

        // GET: Students/Create
        public async Task<IActionResult> Create()
        {
            List<ClassVM> classVMs = await GetClasses();

            ViewData["ClassId"] = new SelectList(classVMs, "Id", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentVM studentVM)
        {
            if (ModelState.IsValid)
            {
                RestRequest request = new RestRequest("Students", Method.Post);
                AddAuthorizationHeader(request);
                SaveStudentDTO saveStudentDTO = _mapper.Map<StudentVM, SaveStudentDTO>(studentVM);
                request.AddJsonBody(saveStudentDTO);
                RestResponse response = await _restClient.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to create student!";
                }
            }

            List<ClassVM> classVMs = await GetClasses();
            ViewData["ClassId"] = new SelectList(classVMs, "Id", "Name", studentVM.ClassId);

            return View(studentVM);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            RestRequest request = new RestRequest($"Students/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            StudentVM studentVM = new StudentVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                StudentDTO studentDTO = JsonConvert.DeserializeObject<StudentDTO>(response.Content)!;
                studentVM = _mapper.Map<StudentDTO, StudentVM>(studentDTO);
            }

            List<ClassVM> classVMs = await GetClasses();
            ViewData["ClassId"] = new SelectList(classVMs, "Id", "Name", studentVM.ClassId);
            return View(studentVM);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentVM studentVM)
        {
            if (ModelState.IsValid)
            {
                RestRequest request = new RestRequest("Students", Method.Post);
                AddAuthorizationHeader(request);
                SaveStudentDTO saveStudentDTO = _mapper.Map<StudentVM, SaveStudentDTO>(studentVM);
                request.AddJsonBody(saveStudentDTO);
                RestResponse response = await _restClient.ExecuteAsync(request);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewData["ErrorMessage"] = "Failed to edit student!";
                }
            }

            List<ClassVM> classVMs = await GetClasses();
            ViewData["ClassId"] = new SelectList(classVMs, "Id", "Name", studentVM.ClassId);
            return View(studentVM);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            RestRequest request = new RestRequest($"Students/{id}", Method.Get);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound();
            }

            StudentVM studentVM = new StudentVM();

            if (response.StatusCode == HttpStatusCode.OK && response.Content != null)
            {
                StudentDTO studentDTO = JsonConvert.DeserializeObject<StudentDTO>(response.Content)!;
                studentVM = _mapper.Map<StudentDTO, StudentVM>(studentDTO);
            }

            return View(studentVM);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            RestRequest request = new RestRequest($"Students/{id}", Method.Delete);
            AddAuthorizationHeader(request);
            RestResponse response = await _restClient.ExecuteAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                //It will be passed to redirected page
                TempData["ErrorMessage"] = "Failed to delete student!";
                return RedirectToAction(nameof(Delete));
            }
        }

        private async Task<List<ClassVM>> GetClasses()
        {
            //Getting all classes
            RestRequest requestForClasses = new RestRequest($"Classes/{1}/{int.MaxValue}", Method.Get);
            AddAuthorizationHeader(requestForClasses);
            RestResponse responseForClasses = await _restClient.ExecuteAsync(requestForClasses);
            List<ClassVM> classVMs = new List<ClassVM>();

            if (responseForClasses.StatusCode == HttpStatusCode.OK && responseForClasses.Content != null)
            {
                List<ClassDTO> classDTOs = JsonConvert.DeserializeObject<List<ClassDTO>>(responseForClasses.Content)!;

                foreach (var classDTO in classDTOs)
                {
                    classVMs.Add(_mapper.Map<ClassDTO, ClassVM>(classDTO));
                }
            }

            return classVMs;
        }
    }
}
