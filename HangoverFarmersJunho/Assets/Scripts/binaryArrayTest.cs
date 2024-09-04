using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class binaryArrayTest : MonoBehaviour
{

    #region Singleton

    // Declara uma instância estática da classe 
    public static binaryArrayTest instance;

    // Método chamado quando o script é inicializado
    private void Awake()
    {
        bits = new BitArray(initialBools);
        // Verifica se a instância é nula
        if (instance == null)
        {
            instance = this; // Define a instância para este objeto
         //   DontDestroyOnLoad(gameObject); // Não destrói o objeto ao carregar uma nova cena
        }
        else if (instance != this)
        {
            Destroy(gameObject); // Destroi o objeto se já houver uma instância existente
        }
    }

    #endregion

    [SerializeField] public int width = 1;
    [SerializeField] public int size = 1;
    [SerializeField] private bool[] initialBools;

    private BitArray bits;

    public bool[] GetInitialBools()
    {
        return initialBools;
    }
}