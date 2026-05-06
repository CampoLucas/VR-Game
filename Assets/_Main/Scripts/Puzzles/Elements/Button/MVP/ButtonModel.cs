using System;
using VRGame.DesignPatterns.Observers;

namespace VRGame.Puzzles.Elements.Button
{
    /// <summary>
    /// Has the button's data and state machine
    /// </summary>
    public class ButtonModel : IDisposable
    {
        #region Properties

        public int Id { get; private set; }
        
        /// <summary> If true, it sets itself to disable after being pressed. </summary>
        public bool IsOnce { get; private set; }
        
        #endregion

        #region Public Methods

        public void Setup(int id, bool isOnce)
        {
            Id = id;
            IsOnce = isOnce;
        }

        public void Dispose()
        {
            
        }
        
        #endregion
    }
}