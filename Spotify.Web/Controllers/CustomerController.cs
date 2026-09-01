using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Customer;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api/admin/customers")]
[Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<CustomerResponse>>> GetCustomers(CancellationToken cancellationToken)
        => Ok(await _customerService.GetCustomersAsync(cancellationToken));

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerResponse>> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id, cancellationToken);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CustomerResponse>> UpdateCustomer(
        Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var result = await _customerService.UpdateCustomerAsync(id, request, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return Ok(result.Customer);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(Guid id, CancellationToken cancellationToken)
    {
        var result = await _customerService.DeleteCustomerAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error);
            return ValidationProblem(ModelState);
        }

        return NoContent();
    }
}