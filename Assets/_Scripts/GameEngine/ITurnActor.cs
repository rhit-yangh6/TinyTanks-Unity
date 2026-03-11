using System.Collections;

namespace _Scripts.GameEngine
{
    public interface ITurnActor
    {
        IEnumerator MakeMove();
    }
}
