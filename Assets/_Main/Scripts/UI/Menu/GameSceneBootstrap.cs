using UnityEngine;
using VRGame.Audio;

namespace VRGame.UI.Menu
{
    /// <summary>
    /// Coloca este script en la escena del JUEGO (no en el menú).
    /// Se encarga de decirle al SoundManager que reproduzca la música del juego.
    ///
    /// SETUP EN LA ESCENA DE JUEGO:
    ///   1. Crear un GameObject vacío llamado "GameBootstrap".
    ///   2. Añadirle este script.
    ///   Eso es todo. El SoundManager persiste desde el menú y hará el fade in automáticamente.
    /// </summary>
    public class GameSceneBootstrap : MonoBehaviour
    {
        private void Start()
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayGameMusic();
            }
            else
            {
                Debug.LogWarning("[GameSceneBootstrap] SoundManager no encontrado. " +
                                 "Si empiezas el juego directamente desde esta escena (sin pasar por el menú), " +
                                 "añade también un SoundManager aquí o usa el menú como punto de entrada.");
            }
        }
    }
}
