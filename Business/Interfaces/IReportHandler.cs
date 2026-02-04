namespace Business
{
    public interface IReportHandler
    {
        Task<PaginatedReport<TDataDto>> PreparePaginatedReport<TData, TDataDto>(IQueryable<TData> query, PaginationParams pageParams, CancellationToken cancellationToken);
    }
}