using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalTrainerApi.Model.Dto.Product;
using PersonalTrainerApi.Services;
using System;
using System.Linq;

namespace PersonalTrainerApi.Controllers
{
    public class ProductController : AuthorizedController
    {
        private readonly IProductManagement productManagement;

        public ProductController(IProductManagement productManagement)
        {
            this.productManagement = productManagement;
        }

        /// <summary>
        /// Zwraca produkt o podanym identyfikatorze
        /// GET api/Product/Products/{id}
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            try
            {
                var product = productManagement.GetProduct(id);
                return Ok(product);
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        /// <summary>
        /// Modyfikuje produkt
        /// </summary>
        /// <param name="id">Identyfikator produktu</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public IActionResult Put([FromBody] ProductDto product)
        {
            try
            {
                productManagement.UpdateProduct(product);
                return Ok();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        /// <summary>
        /// Usuwa produkt
        /// </summary>
        /// <param name="id">Identyfikator produktu</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                productManagement.RemoveProduct(id);
                return Ok();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }


        /// <summary>
        /// Zwraca wszystkie produkty dodane przez użytkownika.
        /// </summary>
        /// <param name="userId">Identyfikator użytkownika</param>
        /// <returns></returns>
        [HttpGet("UserProducts/{userId}")]
        public IActionResult UserProducts(Guid userId)
        {
            try
            {
                var products = productManagement.GetUserProducts(userId);
                return Ok(products.ToArray());
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }


        /// <summary>
        /// Zwraca wszystkie produkty produkty
        /// </summary>
        /// <returns></returns>
        [HttpGet("Products")]
        public IActionResult Products()
        {
            try
            {
                var products = productManagement.GetProducts();
                return Ok(products);
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        /// <summary>
        /// Subskrybuje produkt
        /// </summary>
        /// <param name="productId">identyfikator produktu</param>
        /// <returns></returns>
        [HttpPost("Subscribe/{productId}")]
        public IActionResult Subscribe(Guid productId)
        {
            try
            {
                productManagement.SubscribeProduct(productId);
                return Ok();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        [Authorize("admin")]
        /// <summary>
        /// Potwierdza subskrypcję produktu.
        /// </summary>
        /// <param name="productId">Identyfikator produktu</param>
        /// <returns></returns>
        [HttpPost("AcceptSubscription/{productId}")]
        public IActionResult AcceptSubscription(Guid productId)
        {
            try
            {
                productManagement.AcceptSubscription(productId);
                return Ok();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        [Authorize("admin")]
        /// <summary>
        /// Odrzuca subskrypcję
        /// </summary>
        /// <param name="productId">Identyfikator produktu</param>
        /// <returns></returns>
        [HttpPost("DeclineSubscription/{productId}")]
        public IActionResult DeclineSubscription(Guid productId)
        {
            try
            {
                productManagement.DeclineSubscription(productId);
                return Ok();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }

        /// <summary>
        /// Anuluje subskrypcję
        /// </summary>
        /// <param name="productId">Identyfikator produktu</param>
        /// <returns></returns>
        [HttpPost("CancelSubscription/{productId}")]
        public IActionResult CancelSubscription(Guid productId)
        {
            try
            {
                productManagement.CancelSubscription(productId);
                return Ok();
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc);
            }
        }
    }
}
