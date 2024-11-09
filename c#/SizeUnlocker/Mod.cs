using GDWeave;

namespace SizeUnlocker;

public class Mod : IMod {

    public Mod(IModInterface modInterface) {
        modInterface.RegisterScriptMod(new PlayerScriptMod());
    }

    public void Dispose() {
        // Cleanup anything you do here
    }
}
