using Core.DTOs.Category;
using Core.DTOs.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface ICategoryService:IService<Category>
    {
        public Task<CustomResponseDto<CategoryWithProductDto>> getSingleCategoryByIDWithProductAsync(long categoryId);
    }
}
