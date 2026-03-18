using System.Drawing;

namespace CollaborativeDrawing.Models
{
    public class UserInfo
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public Point CursorPosition { get; set; }
        public Color CursorColor { get; set; }
        public DateTime LastActivity { get; set; }
    }

    public class DrawingPoint
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; } = "#000000";
        public int LineWidth { get; set; } = 3;
        public string UserId { get; set; } = string.Empty;
        public bool IsStartOfStroke { get; set; }
    }

    public class CursorPosition
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public string Color { get; set; } = "#000000";
    }

    public class LoginModel
    {
        public string Username { get; set; } = string.Empty;
        public string FavoriteColor { get; set; } = "#000000";
    }
}