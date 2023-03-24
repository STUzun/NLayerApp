using AutoMapper;
using Core.Services;
using Core;
using Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Web.Controllers;
using Repository.Repositories;
using Microsoft.Extensions.Logging;
using Core.Repositories;
using Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Core.DTOs.Product;
using Core.DTOs.Custom;
using Core.DTOs.Category;
using System.Runtime.CompilerServices;
using Caching;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebUnitTest
{
    public class ProductsControllerUnitTest
    {
        ProductsController productsController;
        //Mock<ProductServiceWithCaching> productMock;
        //Mock<ICategoryService> categoryMock;
        //Mock<IMapper> mapperMock;
        ProductDto p1;
        ProductDto p2;
        ProductDto p3;
        ProductDto p4;
        IEnumerable<ProductDto> products;

        Category c1; 
        Category c2 ;
        Category c3 ;

        ProductWithCategoryDto productWithCategoryDto;
        List<ProductWithCategoryDto> responseDto;
        public ProductsControllerUnitTest()
        {
            p1 = new ProductDto
            {
                Id = 1,
                Name = "Dolma Kalemler",
                CategoryId = 1,
                Price = 10,
                Stock = 5,
                CreatedDate = DateTime.UtcNow

            };
            p2 = new ProductDto
            {
                Id = 2,
                Name = "Kurşun Kalemler",
                CategoryId = 1,
                Price = 50,
                Stock = 10,
                CreatedDate = DateTime.UtcNow

            };
            p3 = new ProductDto
            {
                Id = 3,
                Name = "Dolma Kalemler",
                CategoryId = 1,
                Price = 100,
                Stock = 20,
                CreatedDate = DateTime.UtcNow

            };
            p4 = new ProductDto
            {
                Id = 4,
                Name = "80 Yaprak Çizgili Defter",
                CategoryId = 2,
                Price = 50,
                Stock = 10,
                CreatedDate = DateTime.UtcNow
            };
            Category c1 = new Category { Id = 1, Name = "Kalemler" };
            Category c2 = new Category { Id = 2, Name = "Defterler" };
            Category c3 = new Category { Id = 3, Name = "Silgiler" };

            products = new List<ProductDto>
            {
                p1,
                p2,
                p3,
                p4
            };
            List<Category> categories = new List<Category>();
            categories.Add(c1);
            categories.Add(c2);
            categories.Add(c3);

            CategoryDto cdto1 = new CategoryDto { Id = 1, Name = "Kalemler" };
            CategoryDto cdto2 = new CategoryDto { Id = 2, Name = "Defterler" };
            CategoryDto cdto3 = new CategoryDto { Id = 3, Name = "Silgiler" };

            productWithCategoryDto = new ProductWithCategoryDto
            {
                Id = 5,
                Name = "77 Yaprak Çizgili Defter",
                CategoryId = 2,
                Price = 50,
                Stock = 10,
                CreatedDate = DateTime.UtcNow,
                Category= cdto2
            };
            responseDto = new List<ProductWithCategoryDto>
            {
                productWithCategoryDto
            };
            
            categoryMock = new Mock<ICategoryService>();
            mapperMock = new Mock<IMapper>();
        }
        public readonly Mock<IProductService> productMock = new();
        public readonly Mock<IMapper> mapperMock = new();
        public readonly Mock<ICategoryService> categoryMock = new();

        [Fact]
        public async void IndexTest()
        {
            //Arrange
            productsController = new ProductsController(productMock.Object, categoryMock.Object, mapperMock.Object);
            productMock.Setup( u =>  u.GetProducstWithCategory()).Returns(Task.FromResult (CustomResponseDto<List<ProductWithCategoryDto>>.Success(200,responseDto)));
            //Act
            var result = productsController.Index().Result;
            //Assert
            Assert.IsType<ViewResult>(result);

        }

        [Fact]
        public async void SaveValidTest()
        {
            IEnumerable<Category> categories = new List<Category>
            {
                c1,
                c2,
                c3
            };
            productsController = new ProductsController(productMock.Object, categoryMock.Object, mapperMock.Object);
            categoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(categories));

            var saveResult = productsController.Save(p4).Result;

            Assert.NotNull(saveResult);
            categoryMock.Verify(x => x.GetAllAsync(), Times.Never);
            mapperMock.Verify(y => y.Map<Product>(It.IsAny<ProductDto>()), Times.Once);
            mapperMock.Verify(y => y.Map<List<CategoryDto>>(It.IsAny<List<Category>>()), Times.Never);
            productMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);

            //var result = await saveResultç.Index().ConfigureAwait(false) as ViewResult;
            //Assert.Contains("Index", saveResult.ViewName);
            Assert.IsType<RedirectToActionResult>(saveResult);

            var red = (RedirectToActionResult)saveResult;
            Assert.Equal("Index",red.ActionName);
            
        }
        [Fact]
        public void SaveInValidTest()
        {
            //Arrange
            IEnumerable<Category> categories = new List<Category>
            {
                c1,
                c2,
                c3
            };

            productsController = new ProductsController(productMock.Object, categoryMock.Object, mapperMock.Object);
            productsController.ModelState.AddModelError("Region", "Region is mandatory");

            categoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(categories));
            //Act
            var saveResult = productsController.Save(p4);
            //Assert
            categoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            mapperMock.Verify(y => y.Map<Product>(It.IsAny<ProductDto>()), Times.Never);
            mapperMock.Verify(y => y.Map<List<CategoryDto>>(It.IsAny<List<Category>>()), Times.Once);
            productMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Never);
            Assert.NotNull(saveResult);
        }
        [Fact]
        public void UpdateByIdTest() 
        {
            //Arrange
            Category c1 = new Category { Id = 1, Name = "Kalemler" };
            Category c2 = new Category { Id = 2, Name = "Defterler" };
            Category c3 = new Category { Id = 3, Name = "Silgiler" };


            IEnumerable<Category> categories = new List<Category>()
            {
                c1,
                c2,
                c3
            };

            CategoryDto cdto1 = new CategoryDto { Id = 1, Name = "Kalemler" };
            CategoryDto cdto2 = new CategoryDto { Id = 2, Name = "Defterler" };
            CategoryDto cdto3 = new CategoryDto { Id = 3, Name = "Silgiler" };
            List<CategoryDto> categoriesDto = new List<CategoryDto>();
            categoriesDto.Add(cdto1);
            categoriesDto.Add(cdto2);
            categoriesDto.Add(cdto3);



            Product pro = new Product
            {
                Id = 3,
                Name = "Dolma Kalemler",
                CategoryId = 1,
                Price = 100,
                Stock = 20,
                CreatedDate = DateTime.UtcNow

            };
            long id = 2;

            
            productsController = new ProductsController(productMock.Object, categoryMock.Object, mapperMock.Object);
            var product = productMock.Setup(u => u.GetByIdAsync(id)).Returns(Task.FromResult(pro));
            categoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(categories));
            mapperMock.Setup(u => u.Map<ProductDto>(pro)).Returns(p3);
            mapperMock.Setup(u => u.Map<List<CategoryDto>>(categories)).Returns(categoriesDto);
            //Act
            var result = productsController.Update(id).Result as ViewResult;

            //Assert
            categoryMock.Verify(u => u.GetAllAsync(), Times.Once);
            productMock.Verify(u => u.GetByIdAsync(id), Times.Once);
            mapperMock.Verify(u => u.Map<ProductDto>(pro), Times.Once);
            mapperMock.Verify(u => u.Map<List<CategoryDto>>(categories), Times.Once);
            Assert.NotNull(result);
            Assert.IsAssignableFrom<ViewResult>(result);

        }
        [Fact]
        public void UpdateByObjectInvalidTest() 
        {
            //Arrange
            IEnumerable<Category> categories = new List<Category>
            {
                c1,
                c2,
                c3
            };
            CategoryDto cdto1 = new CategoryDto { Id = 1, Name = "Kalemler" };
            CategoryDto cdto2 = new CategoryDto { Id = 2, Name = "Defterler" };
            CategoryDto cdto3 = new CategoryDto { Id = 3, Name = "Silgiler" };
            List<CategoryDto> categoriesDto = new List<CategoryDto>();
            categoriesDto.Add(cdto1);
            categoriesDto.Add(cdto2);
            categoriesDto.Add(cdto3);


            categoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(categories));
            mapperMock.Setup(x => x.Map<List<CategoryDto>>(categories)).Returns(categoriesDto);
            mapperMock.Setup(x => x.Map<Product>(p1));
            productsController = new ProductsController(productMock.Object, categoryMock.Object, mapperMock.Object);
            productsController.ModelState.AddModelError("Region", "Region is mandatory");
            //Act
            var result=productsController.Update(p1).Result;

            //Assert
            categoryMock.Verify(x => x.GetAllAsync(), Times.Once);
            mapperMock.Verify(x => x.Map<List<CategoryDto>>(categories), Times.Once);
            mapperMock.Verify(x => x.Map<Product>(p1), Times.Never);
            Assert.IsAssignableFrom<ViewResult>(result);
        }

        [Fact]
        public void UpdateByObjectValidTest()
        {
            //Arrange
            IEnumerable<Category> categories = new List<Category>
            {
                c1,
                c2,
                c3
            };
            CategoryDto cdto1 = new CategoryDto { Id = 1, Name = "Kalemler" };
            CategoryDto cdto2 = new CategoryDto { Id = 2, Name = "Defterler" };
            CategoryDto cdto3 = new CategoryDto { Id = 3, Name = "Silgiler" };
            List<CategoryDto> categoriesDto = new List<CategoryDto>();
            categoriesDto.Add(cdto1);
            categoriesDto.Add(cdto2);
            categoriesDto.Add(cdto3);


            categoryMock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(categories));
            mapperMock.Setup(x => x.Map<List<CategoryDto>>(categories)).Returns(categoriesDto);
            mapperMock.Setup(x => x.Map<Product>(p1));
            productsController = new ProductsController(productMock.Object, categoryMock.Object, mapperMock.Object);
            //Act
            var result = productsController.Update(p1).Result;

            //Assert
            categoryMock.Verify(x => x.GetAllAsync(), Times.Never);
            mapperMock.Verify(x => x.Map<List<CategoryDto>>(categories), Times.Never);
            mapperMock.Verify(x => x.Map<Product>(p1), Times.Once);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);
        }

        [Fact]
        public void RemoveTest()
        {
            Product p = new Product
            {
                Id = 1,
                Name = "Dolma Kalemler",
                CategoryId = 1,
                Price = 10,
                Stock = 5,
                CreatedDate = DateTime.UtcNow

            };
            //Arrange
            productMock.Setup(x => x.GetByIdAsync(1)).Returns(Task.FromResult(p));
            productMock.Setup(x => x.RemoveAsync(p));
            long id = 1;
            productsController = new ProductsController(productMock.Object, categoryMock.Object, mapperMock.Object);

            //Act
            var result = productsController.Remove(id).Result;

            //Assert
            productMock.Verify(x => x.GetByIdAsync(1), Times.Once);
            productMock.Verify(x => x.RemoveAsync(p), Times.Once);
            Assert.IsAssignableFrom<RedirectToActionResult>(result);
        }

    }
}
