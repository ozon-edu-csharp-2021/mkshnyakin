using System.Threading;
using System.Threading.Tasks;

namespace OzonEdu.MerchandiseService.Domain.Contracts
{
    /// <summary>
    /// Базовый интерфейс репозитория
    /// </summary>
    /// <typeparam name="TAggregationRoot">Объект сущности для управления</typeparam>
    public interface IRepository<TAggregationRoot>
    {
        /// <summary>
        /// Создать новую сущность
        /// </summary>
        /// <param name="itemToCreate">Объект для создания</param>
        /// <param name="cancellationToken">Токен для отмены операции. <see cref="CancellationToken"/></param>
        /// <returns>Созданная сущность</returns>
        Task<TAggregationRoot> CreateAsync(TAggregationRoot itemToCreate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Получить сущность по Id
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <param name="cancellationToken">Токен для отмены операции. <see cref="CancellationToken"/></param>
        /// <returns>Существующая сущность</returns>
        Task<TAggregationRoot> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обновить существующую сущность
        /// </summary>
        /// <param name="itemToUpdate">Объект для обновления</param>
        /// <param name="cancellationToken">Токен для отмены операции. <see cref="CancellationToken"/></param>
        /// <returns>Обновленная сущность сущность</returns>
        Task<TAggregationRoot> UpdateAsync(TAggregationRoot itemToUpdate, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Удалить существующую сущность
        /// </summary>
        /// <param name="itemToDelete">Объект для удаления</param>
        /// <param name="cancellationToken">Токен для отмены операции. <see cref="CancellationToken"/></param>
        /// <returns>Обновленная сущность сущность</returns>
        Task<TAggregationRoot> DeleteAsync(TAggregationRoot itemToDelete, CancellationToken cancellationToken = default);
    }
}