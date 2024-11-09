using GDWeave.Godot;
using GDWeave.Godot.Variants;
using GDWeave.Modding;

namespace SizeUnlocker
{
    internal class PlayerScriptMod : IScriptMod
    {
        public bool ShouldRun(string path) => path == "res://Scenes/Entities/Player/player.gdc";

        public IEnumerable<Token> Modify(string path, IEnumerable<Token> tokens)
        {
            // scale = clamp(animation_data["player_scale"], 0.6, 1.4) * Vector3.ONE
            var firstClamp = new MultiTokenWaiter([
                t => t is IdentifierToken { Name: "scale" },
                t => t.Type is TokenType.OpAssign,
                t => t is IdentifierToken { Name: "clamp" },
                t => t.Type is TokenType.ParenthesisOpen,
                t => t is IdentifierToken { Name: "animation_data" },
                t => t.Type is TokenType.BracketOpen,
                t => t.Type is TokenType.Constant, // player_scale
                t => t.Type is TokenType.BracketClose,
                t => t.Type is TokenType.Comma,
                t => t.Type is TokenType.Constant, // 0.6
            ], allowPartialMatch: false);

            // scale.y *= clamp(animation_data["player_scale_y"], 0.0, 2.0)
            var secondClamp = new MultiTokenWaiter([
                t => t is IdentifierToken { Name: "scale" },
                t => t.Type is TokenType.Period,
                t => t is IdentifierToken { Name: "y" },
                t => t.Type is TokenType.OpAssignMul,
                t => t is IdentifierToken { Name: "clamp" },
                t => t.Type is TokenType.ParenthesisOpen,
                t => t is IdentifierToken { Name: "animation_data" },
                t => t.Type is TokenType.BracketOpen,
                t => t.Type is TokenType.Constant, // player_scale_y
                t => t.Type is TokenType.BracketClose,
                t => t.Type is TokenType.Comma,
                t => t.Type is TokenType.Constant, // 0.0
            ], allowPartialMatch: false);

            int skip = 0;
            foreach (var token in tokens)
            {
                if (skip-- > 0) continue;

                if (firstClamp.Check(token))
                {
                    // -2.0, 10.0
                    yield return new ConstantToken(new RealVariant(-2.0));
                    yield return new Token(TokenType.Comma);
                    yield return new ConstantToken(new RealVariant(10.0));

                    skip = 2; // skip tokens ',' and '1.4'
                }
                else if (secondClamp.Check(token))
                {
                    // -2.0, 10.0
                    yield return new ConstantToken(new RealVariant(-2.0));
                    yield return new Token(TokenType.Comma);
                    yield return new ConstantToken(new RealVariant(10.0));

                    skip = 2; // skip tokens ',' and '2.0'
                }
                else
                {
                    // return the original token
                    yield return token;
                }
            }
        }
    }
}
