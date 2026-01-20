using AspireApp1.DbApi.DTOs;

namespace AspireApp1.Web;

public class DashboardApiClient
{
    private readonly HttpClient _httpClient;

    public DashboardApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<DashboardMetricDto?> GetCurrentMetricsAsync()
    {
        return await _httpClient.GetFromJsonAsync<DashboardMetricDto>("/api/dashboard/current");
    }

    public async Task<DashboardMetricDto?> GetCurrentMetricsForCustomerAsync(int customerId)
    {
        return await _httpClient.GetFromJsonAsync<DashboardMetricDto>($"/api/dashboard/current/customer/{customerId}");
    }

    public async Task<List<DashboardMetricDto>?> GetHistoricalMetricsAsync(int days = 30)
    {
        return await _httpClient.GetFromJsonAsync<List<DashboardMetricDto>>($"/api/dashboard/history?days={days}");
    }

    public async Task<List<DashboardMetricDto>?> GetHistoricalMetricsForCustomerAsync(int customerId, int days = 30)
    {
        return await _httpClient.GetFromJsonAsync<List<DashboardMetricDto>>($"/api/dashboard/history/customer/{customerId}?days={days}");
    }

    public async Task<HttpResponseMessage> GenerateSnapshotAsync()
    {
        return await _httpClient.PostAsync("/api/dashboard/snapshot", null);
    }
}
