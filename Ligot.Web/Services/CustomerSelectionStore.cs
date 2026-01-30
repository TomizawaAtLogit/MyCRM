namespace Ligot.Web.Services;

/// <summary>
/// Shared state management service for customer and order selection across PreSales, Projects, and Cases.
/// Implements lifecycle hooks to clear orders when the customer changes.
/// </summary>
public class CustomerSelectionStore
{
    private int? _selectedCustomerId;
    private int? _selectedOrderId;
    
    /// <summary>
    /// Event triggered when the selected customer changes.
    /// </summary>
    public event Action? OnCustomerChanged;
    
    /// <summary>
    /// Event triggered when the selected order changes.
    /// </summary>
    public event Action? OnOrderChanged;
    
    /// <summary>
    /// Gets the currently selected customer ID.
    /// </summary>
    public int? SelectedCustomerId => _selectedCustomerId;
    
    /// <summary>
    /// Gets the currently selected order ID.
    /// </summary>
    public int? SelectedOrderId => _selectedOrderId;
    
    /// <summary>
    /// Sets the selected customer and clears the selected order (lifecycle hook).
    /// </summary>
    /// <param name="customerId">The customer ID to select, or null to clear.</param>
    public void SetSelectedCustomer(int? customerId)
    {
        if (_selectedCustomerId != customerId)
        {
            _selectedCustomerId = customerId;
            
            // Lifecycle hook: Clear order when customer changes
            if (_selectedOrderId.HasValue)
            {
                _selectedOrderId = null;
                OnOrderChanged?.Invoke();
            }
            
            OnCustomerChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// Sets the selected order.
    /// </summary>
    /// <param name="orderId">The order ID to select, or null to clear.</param>
    public void SetSelectedOrder(int? orderId)
    {
        if (_selectedOrderId != orderId)
        {
            _selectedOrderId = orderId;
            OnOrderChanged?.Invoke();
        }
    }
    
    /// <summary>
    /// Clears both customer and order selections.
    /// </summary>
    public void Clear()
    {
        var customerChanged = _selectedCustomerId.HasValue;
        var orderChanged = _selectedOrderId.HasValue;
        
        _selectedCustomerId = null;
        _selectedOrderId = null;
        
        if (orderChanged)
            OnOrderChanged?.Invoke();
        if (customerChanged)
            OnCustomerChanged?.Invoke();
    }
}

