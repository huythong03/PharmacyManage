using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Diagnostics;

namespace PharmacyWeb.Hubs
{
	public class ChatHub : Hub
	{
		private static readonly ConcurrentDictionary<string, string> _clients = new ConcurrentDictionary<string, string>();

		// Khách hàng gửi tin nhắn đến dược sĩ
		public async Task SendMessageToPharmacist(string user, string message)
		{
			Debug.WriteLine($"[Server] Khách {user} (ID: {Context.ConnectionId}) gửi: {message}");
			_clients.TryAdd(Context.ConnectionId, user);
			await Clients.Group("Pharmacists").SendAsync("ReceiveMessage", user, message, Context.ConnectionId);
			Debug.WriteLine($"[Server] Đã gửi tin nhắn từ {user} đến nhóm Pharmacists");
			await Clients.Caller.SendAsync("ReceiveMessage", "Tôi", message);
			Debug.WriteLine($"[Server] Đã gửi lại tin nhắn cho khách {Context.ConnectionId}");
			await UpdateClientList();
		}

		// Dược sĩ trả lời khách hàng
		[Authorize(Roles = "Admin,Pharmacist")]
		public async Task SendMessageToClient(string clientId, string message)
		{
			Debug.WriteLine($"[Server] Dược sĩ (ID: {Context.ConnectionId}) gửi đến {clientId}: {message}");
			await Clients.Client(clientId).SendAsync("ReceiveMessage", "Dược Sĩ", message);
			Debug.WriteLine($"[Server] Đã gửi tin nhắn từ dược sĩ đến khách {clientId}");
			await Clients.Caller.SendAsync("ReceiveMessage", "Tôi", message);
			Debug.WriteLine($"[Server] Đã gửi lại tin nhắn cho dược sĩ {Context.ConnectionId}");
		}

		// Khi người dùng kết nối
		public override async Task OnConnectedAsync()
		{
			if (Context.User?.Identity?.IsAuthenticated == true &&
				(Context.User.IsInRole("Admin") || Context.User.IsInRole("Pharmacist")))
			{
				Debug.WriteLine($"[Server] Dược sĩ {Context.User.Identity.Name} (ID: {Context.ConnectionId}) kết nối");
				await Groups.AddToGroupAsync(Context.ConnectionId, "Pharmacists");
				Debug.WriteLine($"[Server] Đã thêm {Context.ConnectionId} vào nhóm Pharmacists");
				await UpdateClientList();
			}
			else
			{
				Debug.WriteLine($"[Server] Khách (ID: {Context.ConnectionId}) kết nối");
			}
			await base.OnConnectedAsync();
		}

		// Khi người dùng ngắt kết nối
		public override async Task OnDisconnectedAsync(Exception exception)
		{
			if (_clients.TryRemove(Context.ConnectionId, out string user))
			{
				Debug.WriteLine($"[Server] Khách {user} (ID: {Context.ConnectionId}) ngắt kết nối");
				await UpdateClientList();
			}
			await base.OnDisconnectedAsync(exception);
		}

		// Cập nhật danh sách khách
		private async Task UpdateClientList()
		{
			var clientList = _clients.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			Debug.WriteLine($"[Server] Cập nhật danh sách khách: {clientList.Count} khách");
			await Clients.Group("Pharmacists").SendAsync("UpdateClientList", clientList);
		}
	}
}