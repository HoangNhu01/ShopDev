namespace ShopDev.Utils.DataUtils
{
    public static class CompareObjectUtils
    {
        /// <summary>
        /// So sánh thông tin giữa hai đối tượng
        /// </summary>
        public static bool CompareObjects<T1, T2>(T1 obj1, T2 obj2)
        {
            // Lấy thông tin về tất cả các trường public của cả hai lớp
            var properties1 = typeof(T1).GetProperties().Select(p => p.Name);
            var properties2 = typeof(T2).GetProperties().Select(p => p.Name);

            // Tìm ra các trường chung giữa hai lớp
            var commonProperties = properties1.Intersect(properties2);

            // Kiểm tra giá trị của các trường chung
            foreach (var propertyName in commonProperties)
            {
                var value1 = typeof(T1).GetProperty(propertyName)?.GetValue(obj1);
                var value2 = typeof(T2).GetProperty(propertyName)?.GetValue(obj2);

                if (!Equals(value1, value2))
                {
                    // Nếu có ít nhất một trường có giá trị khác nhau, trả về false
                    return false;
                }
            }

            // Nếu không có trường chung hoặc tất cả các trường giống nhau, trả về true
            return commonProperties.Any();
        }
    }
}
