using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PharmacyWeb.Data;
using PharmacyWeb.Interfaces;
using PharmacyWeb.Data.Repositories;
using PharmacyWeb.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
using Microsoft.AspNetCore.Localization;
using System.Globalization;
using PharmacyWeb.Hubs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Register DbContext with Identity
builder.Services.AddDbContext<PharmacyWebContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("PharmacyWebConnection")));

// Register Identity with custom options
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
	options.SignIn.RequireConfirmedAccount = false;
	options.Password.RequireDigit = true;
	options.Password.RequiredLength = 6;
	options.Password.RequireNonAlphanumeric = false;
})
	.AddEntityFrameworkStores<PharmacyWebContext>()
	.AddDefaultTokenProviders()
	.AddDefaultUI();

// Register repositories
builder.Services.AddScoped<IMedicineRepository, MedicineRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();

// Register email service
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register SignalR with custom options
builder.Services.AddSignalR(options =>
{
	options.MaximumReceiveMessageSize = 102400; // Tăng giới hạn kích thước tin nhắn
	options.EnableDetailedErrors = true; // Bật lỗi chi tiết để debug
	options.KeepAliveInterval = TimeSpan.FromSeconds(15); // Gửi tín hiệu keep-alive mỗi 15 giây
	options.HandshakeTimeout = TimeSpan.FromSeconds(30); // Timeout cho handshake
	options.ClientTimeoutInterval = TimeSpan.FromSeconds(60); // Timeout cho client
});

// Register Localization
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages()
	.AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
	.AddDataAnnotationsLocalization();

// Add logging
builder.Services.AddLogging(logging =>
{
	logging.AddConsole();
	logging.AddDebug();
	logging.SetMinimumLevel(LogLevel.Debug); // Đặt mức log tối thiểu là Debug
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}
else
{
	app.UseDeveloperExceptionPage();
	app.UseHttpsRedirection();
}

app.UseStaticFiles();

app.UseRequestLocalization(new RequestLocalizationOptions
{
	DefaultRequestCulture = new RequestCulture("vi-VN"),
	SupportedCultures = new[] { new CultureInfo("vi-VN") },
	SupportedUICultures = new[] { new CultureInfo("vi-VN") }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Map routes
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Map SignalR Hub
app.MapHub<ChatHub>("/chatHub");

// Seed data
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var context = services.GetRequiredService<PharmacyWebContext>();
		var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
		var logger = services.GetRequiredService<ILogger<Program>>();

		context.Database.Migrate();

		string[] roleNames = { "Admin", "Pharmacist" };
		foreach (var roleName in roleNames)
		{
			if (!await roleManager.RoleExistsAsync(roleName))
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}

		var adminEmail = "admin@pharmacyweb.com";
		var adminPassword = "Admin123!";
		var adminUser = await userManager.FindByEmailAsync(adminEmail);
		if (adminUser == null)
		{
			adminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
			var result = await userManager.CreateAsync(adminUser, adminPassword);
			if (result.Succeeded)
			{
				await userManager.AddToRoleAsync(adminUser, "Admin");
				logger.LogInformation("Admin user created successfully.");
			}
			else
			{
				logger.LogError("Failed to create admin user: {Errors}", string.Join(", ", result.Errors));
			}
		}
	}
	catch (Exception ex)
	{
		var logger = services.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred while seeding the database.");
	}
}

app.Run();

// EmailSender implementation
public interface IEmailSender
{
	Task SendEmailAsync(string email, string subject, string htmlMessage);
}

public class EmailSender : IEmailSender
{
	private readonly IConfiguration _config;

	public EmailSender(IConfiguration config)
	{
		_config = config;
	}

	public async Task SendEmailAsync(string email, string subject, string htmlMessage)
	{
		var settings = _config.GetSection("EmailSettings");
		var mailMessage = new MailMessage
		{
			From = new MailAddress(settings["SenderEmail"]),
			Subject = subject,
			Body = htmlMessage,
			IsBodyHtml = true
		};
		mailMessage.To.Add(email);

		using var smtpClient = new SmtpClient(settings["SmtpServer"])
		{
			Port = int.Parse(settings["SmtpPort"]),
			Credentials = new System.Net.NetworkCredential(settings["SenderEmail"], settings["SenderPassword"]),
			EnableSsl = true
		};

		await smtpClient.SendMailAsync(mailMessage);
	}
}