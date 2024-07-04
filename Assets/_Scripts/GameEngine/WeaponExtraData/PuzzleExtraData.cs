using System.Collections.Generic;

namespace _Scripts.GameEngine.WeaponExtraData
{
    public class PuzzleExtraData : WeaponExtraData
    {
        private readonly HashSet<int> _puzzleStatus = new ();

        public void AddStatus(int piece)
        {
            _puzzleStatus.Add(piece);
        }

        public bool IsComplete()
        {
            return _puzzleStatus.Count == 4;
        }

        public void ClearStatus()
        {
            _puzzleStatus.Clear();
        }

        public HashSet<int> GetStatus()
        {
            return _puzzleStatus;
        }
    }
}