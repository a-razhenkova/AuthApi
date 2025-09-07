namespace Business
{
    public interface IReportProvider
    {
        Task<PaginatedReport<TDataDto>> PreparePaginatedReport<TData, TDataDto>(IQueryable<TData> query, PaginationParams pageParams, CancellationToken cancellationToken);
    }
}