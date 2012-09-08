#region
using SFML.Graphics;
using SFML.Window;
using SFMLStart.Data;
using SFMLStart.Utilities;
using VeeBulletHell.Base;
using VeeBulletHell.Data;

#endregion

namespace VeeBulletHell.Presets
{
    public static class BHPresetPlayers
    {
        public static BHEntity Reimu(BHGame mGame)
        {
            var fireTimeline = new Timeline();

            BHEntity result = BHPresetBase.Player(mGame, 4.ToUnits(), 2.ToUnits(), Assets.GetAnimation("playerleft"), Assets.GetAnimation("playerright"), Assets.GetAnimation("playerstill"));
            result.Sprite = Assets.GetTileset("p_reimu").GetSprite("s1", Assets.GetTexture("p_reimu"));

            fireTimeline.Action(() =>
                                {
                                    if (Keyboard.IsKeyPressed(Keyboard.Key.Z))
                                    {
                                        Assets.GetSound("se_plst00").Play();
                                        BHPresetBase.PlayerBullet(mGame, result.Position - new Vector2i(8.ToUnits(), 17.ToUnits()), 270, 27.ToUnits()).Sprite = new Sprite(Assets.GetTexture("b_player_reimu1"));
                                        BHPresetBase.PlayerBullet(mGame, result.Position - new Vector2i(-8.ToUnits(), 17.ToUnits()), 270, 27.ToUnits()).Sprite = new Sprite(Assets.GetTexture("b_player_reimu1"));
                                    }
                                });
            fireTimeline.Wait(4);
            fireTimeline.Goto();

            result.TimelinesUpdate.Add(fireTimeline);

            return result;
        }
    }
}