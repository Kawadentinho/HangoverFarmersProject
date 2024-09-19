using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{

    public FrutType frutType; // Tipo da fruta da peça
    public int x; // Posição X da peça no tabuleiro
    public int y; // Posição Y da peça no tabuleiro
    public Board board; // Referência ao tabuleiro
    public bool isInvisible; // Determina se a peça é invisível
    public bool isPowerUp; // Identifica se a peça é um power-up

    public void Init(int x, int y, Board board)
    {
        this.x = x;
        this.y = y;
        this.board = board;
        SetVisibility(!isInvisible); // Define a visibilidade ao inicializar
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        Piece otherPiece = collision.gameObject.GetComponent<Piece>();
        if (otherPiece != null && otherPiece.frutType != FrutType.Obstacle)
        {
            // Supondo que você saiba a direção da linha a ser destruída
            bool isHorizontal = DetermineLineDirection(); // Implementar essa função conforme a lógica do seu jogo

            if (IsPowerUp())
            {
                // Verifique se o power-up deve destruir a linha e forneça a direção correta
                board.DestroyLine(this, isHorizontal);
            }
        }
    }

    bool DetermineLineDirection()
    {
        // Implemente a lógica para determinar se a linha deve ser horizontal ou vertical
        return true; // Exemplo: retorne true para horizontal e false para vertical
    }




    void OnMouseDown()
    {
        if (!isInvisible && frutType != FrutType.Vazio) // Impede a seleção de peças vazias
        {
            board.SelectPiece(this);
        }
    }

    public void SetVisibility(bool isVisible)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = isVisible;
        }
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


    public bool IsPowerUp()
    {
        return frutType == FrutType.LinhaDestruidora;
    }


}

// Enumeração para os tipos de frutas disponíveis
public enum FrutType
{
    Abacaxi,
    Banana,
    Manga,
    Maca,
    Melancia,
    Pinha,
    Uva,
    Poder,
    Obstacle,
    PowerUp,
    LinhaDestruidora,
    PowerUpVertical,
    PowerUpHorizontal,
    Vazio
}
