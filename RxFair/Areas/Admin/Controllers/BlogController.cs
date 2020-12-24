using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RxFair.Data.DbModel;
using RxFair.Dto.Dtos;
using RxFair.Models;
using RxFair.Service.Exceptions;
using RxFair.Service.Interface;
using RxFair.Utility.Common;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using RxFair.Dto.Enum;
using RxFair.Utility;
using RxFair.Utility.Extension;

namespace RxFair.Areas.Admin.Controllers
{
    [Authorize(Roles = AuthorizeRoles.Admin), Area("Admin")]
    public class BlogController : BaseController<BlogController>
    {
        #region Fields
        private readonly IUserService _user;
        private readonly IBlogService _blog;
        private readonly IBlogImageService _blogImage;
        private readonly IBlogTagService _blogTag;
        private readonly IBlogCategoryService _blogCategory;
        #endregion

        #region Ctor
        public BlogController(IUserService user, IBlogService blog, IBlogCategoryService blogCategory, IBlogImageService blogImage, IBlogTagService blogTag)
        {
            _blog = blog;
            _blogCategory = blogCategory;
            _blogImage = blogImage;
            _blogTag = blogTag;
            _user = user;

        }
        #endregion

        #region Methods

        #region Blog Category
        public IActionResult ManageBlogCategory()
        {
            return View();
        }

        public IActionResult AddEditBlogCategory(long id)
        {
            if (id == 0) return View(@"Components/_AddEditBlogCategory", new BlogDto { Id = id });
            var result = _blogCategory.GetSingle(x => x.Id == id);

            return View(@"Components/_AddEditBlogCategory", new BlogDto { Id = result.Id, BlogCategoryName = result.CategoryName });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditBlogCategory(BlogDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }

                    if (model.Id == 0)
                    {
                        model.BlogCategoryName = model.BlogCategoryName.Trim();
                        var isExist = _blogCategory.GetCount(x => x.CategoryName.ToLower().Equals(model.BlogCategoryName.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Blog Category already exist");
                        }
                        var blogCategory = new BlogCategory
                        {
                            CategoryName = model.BlogCategoryName,
                            IsActive = true
                        };
                        await _blogCategory.InsertAsync(blogCategory, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Blog Category inserted successfully", blogCategory.Id);
                    }
                    else
                    {
                        model.BlogCategoryName = model.BlogCategoryName.Trim();
                        var isExist = _blogCategory.GetCount(x => x.CategoryName.ToLower().Equals(model.BlogCategoryName.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Blog Category already exist");
                        }
                        var result = _blogCategory.GetSingle(x => x.Id == model.Id);
                        result.CategoryName = model.BlogCategoryName;
                        await _blogCategory.UpdateAsync(result, Accessor, User.GetUserId());
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Blog Category updated successfully", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditBlogCategory");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        public async Task<IActionResult> GetBlogCategoryList(JQueryDataTableParamModel param)
        {
            try
            {
                var searchRecords = new SqlParameter() { ParameterName = "@SearchRecords", SqlDbType = SqlDbType.Int, Direction = ParameterDirection.Output };
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@iDisplayStart",SqlDbType.Int){Value = param.iDisplayStart},
                    new SqlParameter("@iDisplayLength",SqlDbType.Int){Value = param.iDisplayLength},
                    new SqlParameter("@SortColumn",SqlDbType.VarChar){Value = GetSortingColumnName(param.iSortCol_0)},
                    new SqlParameter("@SortDir",SqlDbType.VarChar){Value = param.sSortDir_0 ?? ""},
                    new SqlParameter("@Search",SqlDbType.NVarChar){Value = param.sSearch ?? ""},
                    searchRecords
                };


                var allList = await _blogCategory.GetBlogCategoryList(parameters.ToArray());
                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });
            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetBlogCategoryList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        [HttpPost]
        public IActionResult RemoveBlog(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var isExist = _blog.GetCount(x => x.BlogCategoryId == id);
                    if (isExist > 0)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, $@"This Blog Category is in use. You can't perform this action.");
                    }
                    var result = _blogCategory.GetSingle(x => x.Id == id);
                    _blogCategory.Delete(result);
                    _blogCategory.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Blog Category Deleted successfully");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveBlog");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageBlogStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var isExist = _blog.GetCount(x => x.BlogCategoryId == id);
                    if (isExist > 0)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, $@"This Blog Category is in use. You can't perform this action.");
                    }
                    var result = _blogCategory.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _blogCategory.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Blog Category {(result.IsActive ? "Activated" : "Deactivated")} successfully", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageBlogStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }
        #endregion

        #region Blog
        public IActionResult ManageBlog()
        {
            return View();
        }

        public async Task<IActionResult> GetBlogList(JQueryDataTableParamModel param)
        {
            try
            {
                var parameters = CommonMethod.GetJQueryDatatableParamList(param, GlobalRxFair.Value.CurrentTimeZoneId, GetSortingColumnName(param.iSortCol_0)).Parameters;
                //var parameters = CommonMethod.GetJQueryDatatableParamList(param, GetSortingColumnName(param.iSortCol_0)).Parameters;

                var allList = await _blog.GetBlogList(parameters.ToArray());
                var total = allList.FirstOrDefault()?.TotalRecords ?? 0;
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = total,
                    iTotalDisplayRecords = total,
                    aaData = allList
                });

            }
            catch (Exception ex)
            {
                ErrorLog.AddErrorLog(ex, "GetBlogList");
                return Json(new
                {
                    param.sEcho,
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0,
                    aaData = ""
                });
            }
        }

        public IActionResult AddEditBlog(long id)
        {
            BindDropdownList(id);
            if (id == 0) return View(new BlogDto { Id = id });
            var blog = _blog.GetById(id);
            return View(new BlogDto
            {
                Id = blog.Id,
                Title = blog.Title,
                InternalName = blog.InternalName,
                BlogCategoryId = blog.BlogCategoryId,
                AuthorId = blog.AuthorId,
                DateBlog = blog.BlogDate.ToDefaultDateTime("MMM-dd-yyyy"),
                Tags = string.Join(",", blog.BlogTags.Select(x => x.TagName).ToList()),
                BlogTag = blog.BlogTags.Select(x => x.TagName).ToList(),
                BlogContent = blog.Descriptions,
                RelatedBlogs = blog?.RelatedBlogs ?? "",
                RelatedBlog = blog.RelatedBlogs.Split(",").ToList()

            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEditBlog(BlogDto model)
        {
            model.BlogTag = model.BlogTag ?? new List<string>();
            model.RelatedBlog = model.RelatedBlog ?? new List<string>();
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }

                    if (model.Id == 0)
                    {
                        var isExist = _blog.GetCount(x => x.Title.ToLower().Equals(model.Title.ToLower()));
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Blog with same title exists.");
                        }

                        var blog = Mapper.Map<BlogDto, Blog>(model);
                        blog.IsActive = true;
                        blog.AuthorId = model.AuthorId;
                        blog.Descriptions = model.BlogContent;
                        blog.BlogDate = Convert.ToDateTime(model.DateBlog);
                        blog.RelatedBlogs = string.Join(",", model.RelatedBlog ?? new List<string>());

                        await _blog.InsertAsync(blog, Accessor, User.GetUserId());

                        var addNewTag = new List<BlogTag>();

                        foreach (var item in model.BlogTag)
                        {
                            addNewTag.Add(new BlogTag
                            {
                                BlogId = blog.Id,
                                TagName = item,
                                IsActive = true
                            });
                        }

                        var blogtags = _blogTag.GetAll().Where(x => x.TagName == addNewTag.ToString());

                        if (!blogtags.Any())
                        {
                            await _blogTag.InsertRangeAsync(addNewTag, Accessor, User.GetUserId());
                        }
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Blog inserted successfully", blog.Id);
                    }
                    else
                    {
                        var isExist = _blog.GetCount(x => x.Title.ToLower().Equals(model.Title.ToLower()) && x.Id != model.Id);
                        if (isExist > 0)
                        {
                            txscope.Dispose();
                            return JsonResponse.GenerateJsonResult(0, "Blog with same title exists.");
                        }
                        var result = _blog.GetSingle(x => x.Id == model.Id);
                        result.Title = model.Title;
                        result.InternalName = model.InternalName;
                        result.BlogCategoryId = model.BlogCategoryId;
                        result.AuthorId = model.AuthorId;
                        result.BlogDate = Convert.ToDateTime(model.DateBlog);
                        result.Descriptions = model.BlogContent;
                        result.RelatedBlogs = string.Join(",", model.RelatedBlog);
                        await _blog.UpdateAsync(result, Accessor, User.GetUserId());

                        var existTags = result.BlogTags.Select(x => x.TagName).ToList();

                        var removeTag = existTags.Except(model.BlogTag).ToList();
                        if (removeTag.Any())
                        {
                            var deleBlogTags = result.BlogTags.Where(x => removeTag.Contains(x.TagName)).ToList();
                            _blogTag.DeleteRange(deleBlogTags);
                            _blogTag.Save();
                        }

                        var newTagList = model.BlogTag.Except(existTags).ToList();
                        var editNewTag = new List<BlogTag>();
                        foreach (var item in newTagList)
                        {
                            editNewTag.Add(new BlogTag
                            {
                                BlogId = result.Id,
                                TagName = item,
                                IsActive = true
                            });
                        }
                        if (editNewTag.Any())
                        {
                            await _blogTag.InsertRangeAsync(editNewTag, Accessor, User.GetUserId());
                        }
                        txscope.Complete();
                        return JsonResponse.GenerateJsonResult(1, "Blog updated successfully", result.Id);
                    }
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/AddEditBlog");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageBlogListStatus(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _blog.GetSingle(x => x.Id == id);
                    result.IsActive = !result.IsActive;
                    await _blog.UpdateAsync(result, Accessor, User.GetUserId());
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, $@"Blog {(result.IsActive ? "Activated" : "Deactivated")} successfully", result.Id);
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/ManageBlogListStatus");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult RemoveBlogList(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _blog.GetSingle(x => x.Id == id);
                    _blog.Delete(result);
                    _blog.Save();
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, @"Blog deleted successfully");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/RemoveBlog");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        #endregion

        #region BlogImages

        public IActionResult ManageBlogImages(long id)
        {
            var model = _blogImage.GetAll(x => x.BlogId == id && x.IsActive).Select(x => new BlogImageDto()
            {
                Id = x.Id,
                ImageName = $@"\{FilePathList.BlogImage}\{x.ImageName}"
            }).ToList();
            return View(model);
        }

        public IActionResult AddEditBlogImage(long id)
        {
            return View(@"Components/_AddEditBlogImage", new BlogImageDto() { BlogId = id });
        }

        [HttpPost, ValidateAntiForgeryToken, DisableRequestSizeLimit]
        public async Task<IActionResult> AddEditBlogImage(BlogImageDto model)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var newBlogFile = new List<string>();
                model.BlogImage = Accessor.HttpContext.Request.Form.Files;
                try
                {
                    if (!ModelState.IsValid)
                    {
                        txscope.Dispose();
                        return JsonResponse.GenerateJsonResult(0, string.Join(",", ModelState.GetModelError()));
                    }

                    var newblogImage = new List<BlogImage>();
                    if (model.BlogImage == null)
                        return JsonResponse.GenerateJsonResult(0, "Please select file !");
                    foreach (var item in model.BlogImage)
                    {
                        var newFileName = CommonMethod.GetFileName(item.FileName);
                        newBlogFile.Add(newFileName);
                        await CommonMethod.UploadFileAsync(HostingEnvironment.WebRootPath, FilePathList.BlogImage, newFileName, item);
                        var blogImage = Mapper.Map<BlogImageDto, BlogImage>(model);
                        blogImage.BlogId = model.BlogId;
                        blogImage.ImageName = newFileName;
                        blogImage.IsActive = true;
                        newblogImage.Add(blogImage);
                    }

                    await _blogImage.InsertRangeAsync(newblogImage, Accessor, User.GetUserId());
                    txscope.Complete();

                    return JsonResponse.GenerateJsonResult(1, "Blog image inserted successfully");

                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    foreach (var item in newBlogFile)
                    {
                        CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.BlogImage, item), true);
                    }
                    ErrorLog.AddErrorLog(ex, "Post/AddEditBlogImage");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteBlogImages(long id)
        {
            using (var txscope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var result = _blogImage.GetSingle(x => x.Id == id);

                    string fileName = result.ImageName;
                    _blogImage.Delete(result);

                    CommonMethod.DeleteFile(CommonMethod.CheckServerPath(HostingEnvironment.WebRootPath, FilePathList.BlogImage, fileName), true);
                    txscope.Complete();
                    return JsonResponse.GenerateJsonResult(1, "Blog Image deleted successfully");
                }
                catch (Exception ex)
                {
                    txscope.Dispose();
                    ErrorLog.AddErrorLog(ex, "Post/DeleteBlogImages");
                    return JsonResponse.GenerateJsonResult(0, GlobalConstant.SomethingWrong);

                }

            }

        }
        #endregion

        #endregion

        #region Controller Common
        private void BindDropdownList(long blogId = 0)
        {
            ViewBag.BlogTagList = _blogTag.GetAll(x => x.BlogId == blogId && x.IsActive).Select(x => new SelectListItem { Text = x.TagName, Value = x.TagName }).OrderBy(x => x.Text).ToList();
            ViewBag.RelatedBlogList = _blog.GetAll(x => x.Id != blogId && x.IsActive).Select(x => new SelectListItem { Text = x.InternalName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.categoryList = _blogCategory.GetAll(x => x.IsActive).Select(x => new SelectListItem { Text = x.CategoryName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            ViewBag.authorList = _user.GetAll(x => x.EmailConfirmed && x.IsActive).Where(x => x.FirstName != null && x.LastName != null).Select(x => new SelectListItem { Text = x.FullName, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
        }
        #endregion
    }
}