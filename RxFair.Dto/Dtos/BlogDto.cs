using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace RxFair.Dto.Dtos
{
    public class BlogDtoView
    {
        public BlogDto BlogDto { get; set; }
        public List<BlogDto> RelatedBlogs { get; set; }

    }

    public class BlogDto : BaseModel
    {
        public string Title { get; set; }

        public string InternalName { get; set; }

        public long BlogCategoryId { get; set; }

        public string BlogCategoryName { get; set; }

        public List<BlogImageDto> BlogImage { get; set; }
        public List<string> BlogImageNameList { get; set; }

        public string FirstImageName { get; set; }

        public long AuthorId { get; set; }
        public string AuthorName { get; set; }

        public DateTime BlogDate { get; set; }
        public string DateBlog { get; set; }

        public string BlogContent { get; set; }

        public string Tags { get; set; }
        public List<string> BlogTag { get; set; }

        public string Descriptions { get; set; }

        public string RelatedBlogs { get; set; }
        public List<string> RelatedBlog { get; set; }

    }

    public class BlogImageDto : BaseModel
    {
        public long BlogId { get; set; }

        public string ImageName { get; set; }

        public IFormFileCollection BlogImage { get; set; }
    }

    public class RecentlyBlogsView
    {
        public long BlogId { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
    }


}
