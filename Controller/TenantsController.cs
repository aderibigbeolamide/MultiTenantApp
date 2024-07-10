[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly IMultiTenantStore<TenantInfo> _tenantStore;

    public TenantsController(IConfiguration configuration, IMultiTenantStore<TenantInfo> tenantStore)
    {
        _configuration = configuration;
        _tenantStore = tenantStore;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TenantInfo>>> GetTenants()
    {
        var tenants = await _tenantStore.GetAllAsync();
        return Ok(tenants);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TenantInfo>> GetTenant(string id)
    {
        var tenant = await _tenantStore.TryGetAsync(id);
        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    [HttpPost]
    public async Task<ActionResult<TenantInfo>> CreateTenant(TenantInfo tenantInfo)
    {
        if (await _tenantStore.TryGetAsync(tenantInfo.Identifier) != null)
        {
            return BadRequest("Tenant identifier already exists.");
        }

        await _tenantStore.TryAddAsync(tenantInfo);
        return CreatedAtAction(nameof(GetTenant), new { id = tenantInfo.Id }, tenantInfo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTenant(string id, TenantInfo tenantInfo)
    {
        if (id != tenantInfo.Id)
        {
            return BadRequest();
        }

        var existingTenant = await _tenantStore.TryGetAsync(id);
        if (existingTenant == null)
        {
            return NotFound();
        }

        await _tenantStore.TryUpdateAsync(tenantInfo);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTenant(string id)
    {
        var tenant = await _tenantStore.TryGetAsync(id);
        if (tenant == null)
        {
            return NotFound();
        }

        await _tenantStore.TryRemoveAsync(id);
        return NoContent();
    }
}
