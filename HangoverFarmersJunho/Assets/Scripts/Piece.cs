using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    /*public FrutType frutType; // Tipo da fruta da peça
    public int x; // Posição X da peça no tabuleiro
    public int y; // Posição Y da peça no tabuleiro
    public Board board; // Referência ao tabuleiro

    public void Init(int x, int y, Board board) // Inicializa a peça com posição e referência ao tabuleiro
    {
        this.x = x;
        this.y = y;
        this.board = board;
    }

    void OnMouseDown() // Chamado quando a peça é clicada
    {
        board.SelectPiece(this); // Seleciona a peça no tabuleiro
    }

    public void AnimateScale(Vector3 targetScale, float duration) // Anima a escala da peça
    {
        StartCoroutine(ScaleCoroutine(targetScale, duration)); // Inicia a rotina de animação de escala
    }

    private IEnumerator ScaleCoroutine(Vector3 targetScale, float duration) // Rotina de animação de escala
    {
        Vector3 startScale = transform.localScale; // Escala inicial da peça
        float time = 0; // Tempo de animação

        while (time < duration) // Enquanto o tempo de animação não atingir a duração
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration); // Interpola a escala da peça
            time += Time.deltaTime; // Incrementa o tempo com base no tempo real do jogo
            yield return null; // Aguarda o próximo quadro
        }

        transform.localScale = targetScale; // Garante que a escala final seja exatamente a desejada
    }
}

// Enumeração para os tipos de frutas disponíveis
public enum FrutType
{
    Abacaxi,
    banana,
    manga,
    maça,
    melancia,
    pinha,
    uva,
    poder
}*/
    public FrutType frutType; // Tipo da fruta da peça
    public int x; // Posição X da peça no tabuleiro
    public int y; // Posição Y da peça no tabuleiro
    public Board board; // Referência ao tabuleiro
    public bool isInvisible; // Determina se a peça é invisível

    public void Init(int x, int y, Board board) // Inicializa a peça com posição e referência ao tabuleiro
    {
        this.x = x;
        this.y = y;
        this.board = board;
    }

    void OnMouseDown() // Chamado quando a peça é clicada
    {
        if (!isInvisible) // Somente seleciona a peça se não for invisível
        {
            board.SelectPiece(this);
        }
    }

    public void SetVisibility(bool isVisible) // Define a visibilidade da peça
    {
        GetComponent<Renderer>().enabled = isVisible;
    }

    public void AnimateScale(Vector3 targetScale, float duration) // Anima a escala da peça
    {
        StartCoroutine(ScaleCoroutine(targetScale, duration)); // Inicia a rotina de animação de escala
    }

    private IEnumerator ScaleCoroutine(Vector3 targetScale, float duration) // Rotina de animação de escala
    {
        Vector3 startScale = transform.localScale; // Escala inicial da peça
        float time = 0; // Tempo de animação

        while (time < duration) // Enquanto o tempo de animação não atingir a duração
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration); // Interpola a escala da peça
            time += Time.deltaTime; // Incrementa o tempo com base no tempo real do jogo
            yield return null; // Aguarda o próximo quadro
        }

        transform.localScale = targetScale; // Garante que a escala final seja exatamente a desejada
    }
}

// Enumeração para os tipos de frutas disponíveis
public enum FrutType
{
    Abacaxi,
    banana,
    manga,
    maça,
    melancia,
    pinha,
    uva,
    poder,
    Obstacle
}

