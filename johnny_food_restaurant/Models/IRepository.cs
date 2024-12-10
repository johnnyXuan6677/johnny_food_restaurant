
namespace johnny_food_restaurant.Models
{
    //此寫法為Respository模型，主要是將寫入DB的邏輯分離出controller，避免重複性動作，並將CRUD模組化以便管理
    //在這裡只有定義方法沒有實際邏輯操作
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id, QueryOption<T> option);
        Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOption<T> options);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

        //Task 為.NET當中異步操作的狀態與結果
        //Task 代表無回傳值
        //Task<Result> 代表一個回傳值的異步操作


    }
}

