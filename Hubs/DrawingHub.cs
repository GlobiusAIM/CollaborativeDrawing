using Microsoft.AspNetCore.SignalR;
using CollaborativeDrawing.Models;
using System.Drawing;
using System.Collections.Concurrent;

namespace CollaborativeDrawing.Hubs
{
    public class DrawingHub : Hub
    {
        private static readonly ConcurrentDictionary<string, UserInfo> _connectedUsers = new();
        private static readonly List<DrawingPoint> _drawingHistory = new();
        private static readonly Random _random = new();

        public override async Task OnConnectedAsync()
        {
            var userName = Context.User?.Identity?.Name ?? $"User{_connectedUsers.Count + 1}";
            var userColor = Color.FromArgb(
                _random.Next(256),
                _random.Next(256),
                _random.Next(256)
            );

            var userInfo = new UserInfo
            {
                ConnectionId = Context.ConnectionId,
                UserName = userName,
                CursorColor = userColor,
                LastActivity = DateTime.UtcNow
            };

            _connectedUsers.TryAdd(Context.ConnectionId, userInfo);

            // Отправляем историю рисования новому пользователю
            await Clients.Caller.SendAsync("LoadDrawingHistory", _drawingHistory);

            // Уведомляем всех о новом пользователе
            await UpdateUsersList();

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connectedUsers.TryRemove(Context.ConnectionId, out _);
            await UpdateUsersList();
            await Clients.Others.SendAsync("UserDisconnected", Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        public async Task Draw(DrawingPoint point)
        {
            if (_connectedUsers.TryGetValue(Context.ConnectionId, out var user))
            {
                point.UserId = user.UserName;
                
                if (!point.IsStartOfStroke)
                {
                    _drawingHistory.Add(point);
                    // Ограничиваем историю последними 1000 точками для производительности
                    if (_drawingHistory.Count > 1000)
                    {
                        _drawingHistory.RemoveRange(0, 100);
                    }
                }

                await Clients.Others.SendAsync("ReceiveDraw", point);
            }
        }

        public async Task UpdateCursor(int x, int y)
        {
            if (_connectedUsers.TryGetValue(Context.ConnectionId, out var user))
            {
                user.CursorPosition = new Point(x, y);
                user.LastActivity = DateTime.UtcNow;

                var cursorInfo = new CursorPosition
                {
                    UserId = Context.ConnectionId,
                    UserName = user.UserName,
                    X = x,
                    Y = y,
                    Color = ColorTranslator.ToHtml(user.CursorColor)
                };

                await Clients.Others.SendAsync("UpdateCursor", cursorInfo);
            }
        }

        public async Task ClearCanvas()
        {
            _drawingHistory.Clear();
            await Clients.All.SendAsync("CanvasCleared");
        }

        private async Task UpdateUsersList()
        {
            var users = _connectedUsers.Values
                .Where(u => u.LastActivity > DateTime.UtcNow.AddSeconds(-30))
                .Select(u => new
                {
                    u.ConnectionId,
                    u.UserName,
                    Color = ColorTranslator.ToHtml(u.CursorColor)
                })
                .ToList();

            await Clients.All.SendAsync("UpdateUsersList", users);
        }
    }
}