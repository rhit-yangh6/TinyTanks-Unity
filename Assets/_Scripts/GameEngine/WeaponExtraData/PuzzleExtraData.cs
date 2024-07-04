using System.Collections.Generic;

namespace _Scripts.GameEngine.WeaponExtraData
{
    public class PuzzleExtraData : WeaponExtraData
    {
        private readonly HashSet<int> _puzzleStatus = new ();

        public bool AddStatus(int piece)
        {
            _puzzleStatus.Add(piece);
            
            if (_puzzleStatus.Count != 4) return false;
            
            _puzzleStatus.Clear();
            return true;
        }

        public HashSet<int> GetStatus()
        {
            return _puzzleStatus;
        }
    }
}