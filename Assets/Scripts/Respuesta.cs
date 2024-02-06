using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Respuesta
{
    public string jsonrpc;
    public int id;
    public Result result;
    public string error;
}
