using FlightDex.SharedKernel.Paging;

namespace FlightDex.UnitTests.SharedKernel;

public sealed class PagedResultTests
{
    private static PagedResult<int> Page(int page, int pageSize, int total)
        => new([], page, pageSize, total);

    [Theory]
    [InlineData(57, 30, 2)]
    [InlineData(60, 30, 2)]
    [InlineData(61, 30, 3)]
    [InlineData(0, 30, 0)]
    public void TotalPages_is_ceiling_of_total_over_pagesize(int total, int pageSize, int expected)
        => Assert.Equal(expected, Page(1, pageSize, total).TotalPages);

    [Fact]
    public void TotalPages_is_zero_when_pagesize_is_non_positive()
        => Assert.Equal(0, Page(1, 0, 100).TotalPages);

    [Fact]
    public void HasPrevious_is_false_on_the_first_page_and_true_after()
    {
        Assert.False(Page(1, 30, 100).HasPrevious);
        Assert.True(Page(2, 30, 100).HasPrevious);
    }

    [Fact]
    public void HasNext_is_true_until_the_last_page()
    {
        Assert.True(Page(1, 30, 100).HasNext);   // 4 pages
        Assert.True(Page(3, 30, 100).HasNext);
        Assert.False(Page(4, 30, 100).HasNext);
    }
}
