using Applications.YearView.DTO;
using MediatR;

namespace Applications.YearView.Querys;

public class GetYearViewquery : IRequest<YearViewDto>
{
    public int Year { get; }

    public GetYearViewquery(int year)
    {
        Year = year;
    }
}