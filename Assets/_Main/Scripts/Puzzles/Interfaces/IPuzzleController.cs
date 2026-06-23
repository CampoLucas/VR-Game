

using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.Interfaces
{
    public interface IPuzzleController : ISubject<int, bool>, IObserver<int, bool>
    {
        /// <summary>
        /// The current state of the puzzle
        /// </summary>
        bool SolvedState { get; }

        /// <summary>
        /// Subject for when the puzzle is enabled and disabled
        /// </summary>
        ISubject<int, bool> OnEnabledSubject { get; }
        
        /// <summary>
        /// Disables and enables the puzzle
        /// </summary>
        /// <param name="isEnabled"></param>
        void SetEnabled(bool isEnabled);

    }
}
