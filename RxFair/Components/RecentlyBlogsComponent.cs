using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RxFair.Service.Interface;

namespace RxFair.Components
{
    public class RecentlyBlogsViewComponent : ViewComponent
    {
        private readonly IBlogService _blog;
        public RecentlyBlogsViewComponent(IBlogService blog)
        {
            _blog = blog;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {

            var model = await _blog.GetRecentBlogs();
            
            return await Task.FromResult((IViewComponentResult)View("~/Views/Blogs/Components/_RecentlyBlogs.cshtml", model));
        }
    }
}
