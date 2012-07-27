#region
using SFML.Window;
using SFMLStart;
using SFMLStart.Data;
using VeeBulletHell.Base;

#endregion
namespace VeeBulletHell
{
    public class Program
    {
        private static void Main()
        {
            Settings.Framerate.Limit = 60;
            Settings.Framerate.IsLimited = true;
            Settings.Frametime.IsStatic = false;
            Settings.Frametime.StaticValue = 1;

            var game = new BHGame();
            var gameWindow = new GameWindow(640, 480, 1);

            gameWindow.SetGame(game);
            gameWindow.Camera.Move(-new Vector2f(32, 16));
            gameWindow.Run();
        }
    }
}