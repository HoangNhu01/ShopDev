using ShopDev.Inventory.ApplicationServices.ProductModule.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopDev.Inventory.ApplicationServices.ProductModule.Abstract
{
	public interface IProductService
	{
        void Create(ProductCreateDto input);
		ProductDetailDto FindById(string id);
		void Update(ProductUpdateDto input);
	}
}
