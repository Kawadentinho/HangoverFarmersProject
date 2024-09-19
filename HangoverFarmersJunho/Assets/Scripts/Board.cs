using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Unity.VisualScripting;
using System.IO;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int MaxJogadas = 10;
    private int currentJogadas;
    public int maxObejetivo = 10;
    private int currentObjective;


    public GameObject powerUpPrefab; // Referência ao prefab do power-up
    public GameObject obstaclePrefab;
    public GameObject[] piecePrefab;
    public GameManager gameManager;
    public GameObject linhaDestruidoraPrefab;


    public Piece[,] pieces;
    private Piece selectedPiece;
    public Vector3 vector3Base = new Vector3(1, 1, 1); // Valor padrão para a escala


    public SpriteRenderer objetivo;
    public SpriteRenderer objetivoImage;


    public TextMeshProUGUI obejectiveText;
    public TextMeshProUGUI JogadasText;


    public bool cabo;
    private bool isRefilling = false; // Variável para controlar o estado de refill
    public bool aumentei = false;


    void Start()
    {
        currentJogadas = MaxJogadas;
        pieces = new Piece[width, height];
        InitializeBoard();
    }

    private void Update()
    {
        UpdateJogadaText();
        UpdateObjectiveText();

        if (currentJogadas <= 0 && !cabo)
        {
            cabo = true;
            gameManager.gameOver();

            Debug.Log("Game Over");
        }
    }

    void AumentarVida()
    {
        currentJogadas++;
    }

    void InitializeBoard()
    {
        binaryArrayTest binaryArray = GetComponent<binaryArrayTest>(); // Obtém o componente do BinaryArrayTest
        bool[] initialBools = binaryArray.GetInitialBools(); // Obtém a matriz binária

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + y * width;
                if (index < initialBools.Length && initialBools[index])
                {
                    // Se a matriz binária indica que há um espaço vazio aqui
                    pieces[x, y] = CreateEmptyPiece(x, y);
                }
                else
                {
                    // Caso contrário, crie uma peça normal
                    GameObject newPiece = Instantiate(piecePrefab[RandomFrut()], new Vector3(x, y, 0), Quaternion.identity);
                    if (newPiece != null)
                    {
                        pieces[x, y] = newPiece.GetComponent<Piece>();
                        if (pieces[x, y] != null)
                        {
                            pieces[x, y].Init(x, y, this);
                        }
                    }
                }
            }
        }

        // Após a inicialização, verifique se há matches e os processe
        List<Piece> piecesDestroyed = CheckForMatches(out int totalDestroyed);
        CheckObjective(piecesDestroyed);
    }

    Piece CreateEmptyPiece(int x, int y)
    {
        GameObject emptyObject = new GameObject("EmptyPiece");
        Piece emptyPiece = emptyObject.AddComponent<Piece>();
        emptyPiece.frutType = FrutType.Vazio;
        emptyPiece.Init(x, y, this);
        emptyPiece.SetVisibility(false); // Peça invisível
        return emptyPiece;
    }

    int RandomFrut()
    {
        return UnityEngine.Random.Range(0, piecePrefab.Length);
    }

    private void UpdateJogadaText()
    {
        JogadasText.text = currentJogadas.ToString();
    }

    private void UpdateObjectiveText()
    {
        obejectiveText.text = $"{currentObjective}/{maxObejetivo}";
    }

    public void SelectPiece(Piece piece)
    {
        if (currentJogadas <= 0 || isRefilling) return; // Impede a seleção durante o refill
        if (piece.frutType == FrutType.Obstacle) return; // Impede a seleção de peças de obstáculo

        if (selectedPiece == null)
        {
            selectedPiece = piece;
            selectedPiece.AnimateScale(vector3Base * 1.2f, 0.2f);
        }
        else
        {
            if (IsAdjacent(selectedPiece, piece))
            {
                selectedPiece.AnimateScale(vector3Base, 0.2f);
                piece.AnimateScale(vector3Base, 0.2f);
                SwapPieces(selectedPiece, piece);
            }
            else
            {
                selectedPiece.AnimateScale(vector3Base, 0.2f);
                selectedPiece = piece;
                selectedPiece.AnimateScale(vector3Base * 1.2f, 0.2f);
            }
        }
    }

    bool IsAdjacent(Piece piece1, Piece piece2)
    {
        return (Mathf.Abs(piece1.x - piece2.x) == 1 && piece1.y == piece2.y) ||
               (Mathf.Abs(piece1.y - piece2.y) == 1 && piece1.x == piece2.x);
    }

    void SwapPieces(Piece piece1, Piece piece2)
    {
        if (piece1.frutType == FrutType.Obstacle || piece2.frutType == FrutType.Obstacle) return;

        int tempX = piece1.x;
        int tempY = piece1.y;

        pieces[piece1.x, piece1.y] = piece2;
        pieces[piece2.x, piece2.y] = piece1;

        piece1.Init(piece2.x, piece2.y, this);
        piece2.Init(tempX, tempY, this);

        Vector3 tempPosition = piece1.transform.position;
        piece1.transform.position = piece2.transform.position;
        piece2.transform.position = tempPosition;

        piece1.AnimateScale(vector3Base, 0.2f);
        piece2.AnimateScale(vector3Base, 0.2f);
        selectedPiece = null;
        currentJogadas--;

        List<Piece> piecesDestroyed = CheckForMatches(out int totalDestroyed);

        if (piecesDestroyed.Exists(p => p.IsPowerUp()))
        {
            ActivatePowerUp(piecesDestroyed);
        }
        else
        {
            if (totalDestroyed >= 4)
            {
                // Gera o power-up no local da peça movida (piece1)
                CreatePowerUp(piece1.x, piece1.y, true); // Ajustar para a direção correta se necessário
            }
            CheckObjective(piecesDestroyed);
        }
    }




    List<Piece> CheckForMatches(out int totalDestroyed)
    {
        List<Piece> piecesToDestroy = new List<Piece>();
        totalDestroyed = 0;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null) continue;

                // Verifica linha horizontal
                if (x < width - 2)
                {
                    int matchLength = 1;
                    FrutType currentType = pieces[x, y].frutType;
                    for (int k = 1; k < width - x; k++)
                    {
                        if (pieces[x + k, y] != null && pieces[x + k, y].frutType == currentType)
                        {
                            matchLength++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (matchLength == 3)
                    {
                        for (int k = 0; k < matchLength; k++)
                        {
                            piecesToDestroy.Add(pieces[x + k, y]);
                            totalDestroyed++;
                        }
                    }
                    else if (matchLength >= 4)
                    {
                        // Cria o power-up horizontal
                        int powerUpX = x + 1; // Ajuste a posição conforme necessário
                        if (powerUpX < width && pieces[powerUpX, y] == null)
                        {
                            CreatePowerUp(powerUpX, y, true); // Power-up horizontal
                        }
                        for (int k = 0; k < matchLength; k++)
                        {
                            piecesToDestroy.Add(pieces[x + k, y]);
                            totalDestroyed++;
                        }
                    }
                }

                // Verifica coluna vertical
                if (y < height - 2)
                {
                    int matchLength = 1;
                    FrutType currentType = pieces[x, y].frutType;
                    for (int k = 1; k < height - y; k++)
                    {
                        if (pieces[x, y + k] != null && pieces[x, y + k].frutType == currentType)
                        {
                            matchLength++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (matchLength == 3)
                    {
                        for (int k = 0; k < matchLength; k++)
                        {
                            piecesToDestroy.Add(pieces[x, y + k]);
                            totalDestroyed++;
                        }
                    }
                    else if (matchLength >= 4)
                    {
                        // Cria o power-up vertical
                        int powerUpY = y + 1; // Ajuste a posição conforme necessário
                        if (powerUpY < height && pieces[x, powerUpY] == null)
                        {
                            CreatePowerUp(x, powerUpY, false); // Power-up vertical
                        }
                        for (int k = 0; k < matchLength; k++)
                        {
                            piecesToDestroy.Add(pieces[x, y + k]);
                            totalDestroyed++;
                        }
                    }
                }
            }
        }
        // Destruir as peças da lista e processar o refill
        foreach (Piece piece in piecesToDestroy)
        {
            if (piece != null)
            {
                pieces[piece.x, piece.y] = null;
                Destroy(piece.gameObject);
            }
        }

        DestroyAdjacentObstacles(piecesToDestroy);
        StartCoroutine(RefillBoard());

        return piecesToDestroy;
    }






    void CheckObjective(List<Piece> piecesDestroyed)
    {
        FrutType objetivoFrutType = objetivo.GetComponent<Piece>().frutType;
        int destroyedCount = 0;

        foreach (Piece piece in piecesDestroyed)
        {
            if (piece.frutType == objetivoFrutType)
            {
                destroyedCount++;
            }
        }

        if (destroyedCount > 0)
        {
            currentObjective += destroyedCount;
            // Verifica se o objetivo foi alcançado
            if (currentObjective >= maxObejetivo)
            {
                //gameManager.LevelComplete(); // Descomentei para ativar a conclusão de nível
            }
        }
    }



    void DestroyAdjacentObstacles(List<Piece> piecesToDestroy)
    {
        foreach (Piece piece in piecesToDestroy)
        {
            if (piece != null && piece.frutType == FrutType.Obstacle)
            {
                for (int x = piece.x - 1; x <= piece.x + 1; x++)
                {
                    for (int y = piece.y - 1; y <= piece.y + 1; y++)
                    {
                        if (x >= 0 && x < width && y >= 0 && y < height && pieces[x, y] != null)
                        {
                            if (pieces[x, y].frutType != FrutType.Obstacle)
                            {
                                piecesToDestroy.Add(pieces[x, y]);
                            }
                        }
                    }
                }
            }
        }
    }



    IEnumerator AnimatePieceMovement(Piece piece, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = piece.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            piece.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        piece.transform.position = targetPosition;
    }




    IEnumerator RefillBoard()
    {
        yield return new WaitForSeconds(0.2f);

        List<IEnumerator> animations = new List<IEnumerator>();

        // Move as peças existentes para os espaços vazios abaixo dos espaços vazios
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null)
                {
                    for (int k = y + 1; k < height; k++)
                    {
                        if (pieces[x, k] != null)
                        {
                            animations.Add(AnimatePieceMovement(pieces[x, k], new Vector3(x, y, 0), 0.3f));
                            pieces[x, y] = pieces[x, k];
                            pieces[x, k] = null;
                            pieces[x, y].Init(x, y, this);
                            break;
                        }
                    }
                }
            }
        }

        // Executa todas as animações de queda em paralelo
        foreach (IEnumerator animation in animations)
        {
            StartCoroutine(animation);
        }
        yield return new WaitForSeconds(0.3f);

        animations.Clear();

        // Agora faz o refill para os espaços vazios, exceto os espaços reservados para power-ups
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (pieces[x, y] == null)
                {
                    GameObject newPiece = Instantiate(piecePrefab[RandomFrut()], new Vector3(x, height, 0), Quaternion.identity); // Cria a peça fora da tela
                    pieces[x, y] = newPiece.GetComponent<Piece>();
                    if (pieces[x, y] != null)
                    {
                        pieces[x, y].Init(x, y, this);
                        animations.Add(AnimatePieceMovement(pieces[x, y], new Vector3(x, y, 0), 0.3f)); // Anima o movimento da peça até o destino
                    }
                }
            }
        }

        // Executa todas as animações de refill em paralelo
        foreach (IEnumerator animation in animations)
        {
            StartCoroutine(animation);
        }
        yield return new WaitForSeconds(0.3f);

        // Após o refill, verifica por novos matches
        List<Piece> piecesDestroyed = CheckForMatches(out int totalDestroyed);
        if (piecesDestroyed.Count > 0)
        {
            CheckObjective(piecesDestroyed);
            yield return StartCoroutine(RefillBoard()); // Certifica que checa por matches repetidamente até não restar mais matches
        }
    }











    void CreatePowerUp(int x, int y, bool isHorizontal)
    {
        // Verifique se o espaço está vazio antes de criar o power-up
        if (x >= 0 && x < width && y >= 0 && y < height && pieces[x, y] == null)
        {
            if (powerUpPrefab != null)
            {
                GameObject powerUpObject = Instantiate(powerUpPrefab, new Vector3(x, y, 0), Quaternion.identity);
                Piece powerUpPiece = powerUpObject.GetComponent<Piece>();
                if (powerUpPiece != null)
                {
                    powerUpPiece.Init(x, y, this);
                    powerUpPiece.frutType = FrutType.PowerUp; // Ajuste o tipo do power-up
                    pieces[x, y] = powerUpPiece; // Coloca o power-up no grid
                }
            }
            else
            {
                Debug.LogError("Prefab de PowerUp não atribuído.");
            }
        }
        else
        {
            Debug.LogWarning($"Não é possível criar o power-up na posição ({x}, {y}) porque o espaço está ocupado.");
        }
    }













    void ActivatePowerUp(List<Piece> piecesDestroyed)
    {
        Piece powerUpPiece = piecesDestroyed.Find(p => p.IsPowerUp());

        if (powerUpPiece != null)
        {
            bool isHorizontal = true; // Ajuste conforme a necessidade

            // Destruir linha com base na direção
            DestroyLine(powerUpPiece, isHorizontal);
            pieces[powerUpPiece.x, powerUpPiece.y] = null;
            Destroy(powerUpPiece.gameObject);
        }
    }






    public void DestroyLine(Piece piece, bool isHorizontal)
    {
        int x = piece.x;
        int y = piece.y;

        if (isHorizontal)
        {
            // Destruir linha horizontal
            for (int i = 0; i < width; i++)
            {
                if (pieces[i, y] != null)
                {
                    Destroy(pieces[i, y].gameObject);
                    pieces[i, y] = null;
                }
            }
        }
        else
        {
            // Destruir linha vertical
            for (int i = 0; i < height; i++)
            {
                if (pieces[x, i] != null)
                {
                    Destroy(pieces[x, i].gameObject);
                    pieces[x, i] = null;
                }
            }
        }
    }




    bool DetermineLineDirection()
    {
        // Exemplo básico: Se o power-up foi ativado verticalmente, retorna false; horizontalmente, retorna true
        return this.transform.position.x > this.transform.position.y;
    }






}


