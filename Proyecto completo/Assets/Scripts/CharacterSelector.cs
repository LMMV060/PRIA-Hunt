using UnityEngine;
using UnityEngine.UI;

public class CharacterSelector : MonoBehaviour
{
    public RawImage iconoBoton;
    public Texture[] personajes;

    public static int personajeSeleccionado = 0;

    void Start()
    {
        personajeSeleccionado = 0;
        iconoBoton.texture = personajes[0];
    }

    public void CambiarPersonaje()
    {
        personajeSeleccionado++;

        if (personajeSeleccionado >= personajes.Length)
            personajeSeleccionado = 0;

        iconoBoton.texture = personajes[personajeSeleccionado];
    }
}