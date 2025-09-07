using AutoMapper;
using Infrastructure.Configuration.AppSettings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Business
{
    public class ReportService : IReportProvider
    {
        protected readonly AppSettingsOptions _appSettingsOptions;
        protected readonly IMapper _mapper;

        public ReportService(IOptionsSnapshot<AppSettingsOptions> appSettingsOptions,
                            IMapper mapper)
        {
            _appSettingsOptions = appSettingsOptions.Value;
            _mapper = mapper;
        }

        public async Task<PaginatedReport<TDataDto>> PreparePaginatedReport<TData, TDataDto>(IQueryable<TData> query, PaginationParams pageParams, CancellationToken cancellationToken)
        {
            int itemsPerPage = pageParams.ItemsPerPage <= 0 || pageParams.ItemsPerPage > _appSettingsOptions.PaginatedReport.DefaultMaxAllowedItemsPerPage
                ? _appSettingsOptions.PaginatedReport.DefaultItemsPerPage
                : pageParams.ItemsPerPage;

            int itemsCount = await query.CountAsync();
            int totalPagesCount = Convert.ToInt32(Math.Ceiling((double)itemsCount / itemsPerPage));

            int pageNumber = pageParams.ItemsPerPage;
            if (pageParams.ItemsPerPage <= 0)
            {
                pageNumber = 1;
            }
            else if (pageParams.ItemsPerPage > totalPagesCount)
            {
                pageNumber = totalPagesCount;
            }

            IEnumerable<TData> data = await query
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage)
                .ToListAsync(cancellationToken);

            return new PaginatedReport<TDataDto>()
            {
                RequestedPageNumber = pageNumber,
                TotalPagesCount = totalPagesCount,
                ItemsPerPage = itemsPerPage,
                Data = _mapper.Map<IEnumerable<TDataDto>>(data)
            };
        }
    }
}