using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerfilJugador
{
    private static int id = 0;
    private static short[] armas = new short[3];
    private static float sensitividad = 0.5f;


    public static int Id
    {
        get => id;
        set => id = value;
    }

    public static short[] Armas
    {
        get => armas;
        set => armas = value;
    }

    public static float Sensitividad
    {
        get => sensitividad;
        set => sensitividad = value;
    }
}
