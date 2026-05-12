using System.ComponentModel.DataAnnotations;

namespace Sales_Management_System_API.DTO.CategoryDtos
{
    // Trả về client
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? ParentId { get; set; }
        public string? ParentName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Trả về dạng cây cha-con
    public class CategoryTreeDto
    {
        public int CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public List<CategoryTreeDto> Children { get; set; } = [];
    }

    // Tạo mới
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }

    // Cập nhật
    public class UpdateCategoryDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }
    }

    // Phân trang + tìm kiếm
    public class CategoryQueryDto
    {
        public string? Search { get; set; }

        public int? ParentId { get; set; }  // lọc theo cha

        public bool? IsActive { get; set; } // lọc theo trạng thái

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 100)]
        public int PageSize { get; set; } = 10;
    }
}
