using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PerfilJugador
{
    private static int id = -1;
    private static int dinero = 0;
    private static short[] armas = new short[3];
    private static float sensitividad = 0.5f;


    public static int Id
    {
        get => id;
        set => id = value;
    }

    public static int Dinero
    {
        get => dinero;
        set => dinero = value;
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
