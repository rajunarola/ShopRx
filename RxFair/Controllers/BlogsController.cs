using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Service.Interface;

namespace RxFair.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogService _blog;
        private readonly IBlogImageService _blogImage;

        public BlogsController(IBlogService blogService, IBlogImageService blogImageService)
        {
            _blog = blogService;
            _blogImage = blogImageService;
        }

        public IActionResult Index()
        {

            var blogDto = _blog.GetAll(x => x.IsActive).Select(x => new BlogDto
            {
                AuthorName = x.Author.FullName,
                BlogCategoryId = x.BlogCategoryId,
                BlogCategoryName = x.BlogCategory.CategoryName,
                BlogDate = x.BlogDate,
                FirstImageName = x.BlogImages.FirstOrDefault(y => y.BlogId == x.Id)?.ImageName ?? "",
                Id = x.Id,
                Descriptions = x.Descriptions,
                InternalName = x.InternalName,
                RelatedBlogs = x.RelatedBlogs,
                Tags = x.Tags,
                Title = x.Title,
                IsActive = x.IsActive
            }).ToList();

            return View(blogDto);
        }

        [HttpGet]
        public IActionResult BlogDetails(long id)
        {
            var blog = _blog.GetSingle(x => x.IsActive && x.Id == id);
            if (blog == null) return RedirectToAction("Index", "Blogs");

            var blogDtoView = new BlogDtoView
            {
                BlogDto = new BlogDto
                {
                    Id = blog.Id,
                    AuthorName = blog.Author.FullName,
                    BlogCategoryId = blog.BlogCategoryId,
                    BlogCategoryName = blog.BlogCategory.CategoryName,
                    BlogDate = blog.BlogDate,
                    BlogImage = blog.BlogImages.Select(y => new BlogImageDto
                    {
                        BlogId = y.BlogId,
                        ImageName = y.ImageName
                    }).ToList(),
                    Descriptions = blog.Descriptions,
                    InternalName = blog.InternalName,
                    IsActive = blog.IsActive,
                    Tags = blog.Tags,
                    Title = blog.Title,
                    FirstImageName = blog.BlogImages.FirstOrDefault(y => y.BlogId == blog.Id)?.ImageName ?? "",
                    BlogImageNameList = blog.BlogImages.Where(x=>x.BlogId==id).Select(x=>x.ImageName).ToList(),
                    RelatedBlogs = blog?.RelatedBlogs ?? ""
                }
            };
            var relatedBlogsIdList = new List<long>();
            if (!string.IsNullOrEmpty(blogDtoView.BlogDto.RelatedBlogs))
            {
                relatedBlogsIdList = blogDtoView.BlogDto.RelatedBlogs?.Split(',')?.Select(x => Convert.ToInt64(x)).ToList() ?? new List<long>();
            }
            var relatedBlogs = _blog.GetAll(x => relatedBlogsIdList.Contains(x.Id) && x.IsActive)
                .Select(x => new BlogDto
                {
                    Id = x.Id,
                    AuthorName = x.Author.FullName,
                    BlogCategoryId = x.BlogCategoryId,
                    BlogCategoryName = x.BlogCategory?.CategoryName ?? "",
                    BlogDate = x.BlogDate,
                    FirstImageName = x.BlogImages.FirstOrDefault(y => y.BlogId == x.Id)?.ImageName ?? "",
                    Descriptions = x.Descriptions,
                    InternalName = x.InternalName,
                    IsActive = x.IsActive,
                    RelatedBlogs = x?.RelatedBlogs ?? "",
                    Tags = x.Tags,
                    Title = x.Title
                }).ToList();

            blogDtoView.RelatedBlogs = relatedBlogs;

            return View(blogDtoView);
        }


    }
}