using UnityEngine;
using System.Collections.Generic;

public class TabuleiroGrafoTematico : MonoBehaviour
{
    public GameObject casaPrefab; // Prefab da casa
    public GameObject linhaPrefab; // Prefab da linha/caminho entre as casas
    public int colunas = 7; // Quantidade de colunas do tabuleiro
    public int linhas = 15; // Quantidade de linhas
    public float distanciaX = 2.5f; // Espaçamento horizontal entre casas
    public float distanciaY = 2.5f; // Espaçamento vertical entre casas

    private List<List<GameObject>> mapa = new List<List<GameObject>>(); // Lista de casas por linha
    private Dictionary<GameObject, List<GameObject>> conexoes = new Dictionary<GameObject, List<GameObject>>(); // Armazena as conexões
    private HashSet<GameObject> caminhoValido = new HashSet<GameObject>(); // Armazena os vértices de caminhos válidos

    private string[] temas = { "Ciência", "Arte", "Esporte", "Geografia", "História" }; // Temas disponíveis
    private Dictionary<string, Color> coresTemas = new Dictionary<string, Color>()
    {
        { "Ciência", Color.blue },
        { "Arte", Color.red },
        { "Esporte", Color.green },
        { "Geografia", Color.yellow },
        { "História", Color.magenta }
    };

    void Start()
    {
        GerarTabuleiro();
        GerarCaminhos();
        RemoverBecosSemSaida();
        AtribuirTemas();
    }

    // Gera o tabuleiro com casas dispostas em uma grade 7x15
    void GerarTabuleiro()
    {
        for (int y = 0; y < linhas; y++)
        {
            List<GameObject> linhaCasas = new List<GameObject>();
            for (int x = 0; x < colunas; x++)
            {
                // Define a posição da casa
                Vector2 posicao = new Vector2(x * distanciaX, y * distanciaY);
                GameObject casa = Instantiate(casaPrefab, posicao, Quaternion.identity);
                casa.transform.SetParent(transform);
                casa.name = $"Casa {x},{y}";

                linhaCasas.Add(casa);
                conexoes[casa] = new List<GameObject>(); // Inicia lista de conexões
            }
            mapa.Add(linhaCasas);
        }
    }

    // Gera os caminhos aleatórios da primeira linha até a última
    void GerarCaminhos()
    {
        foreach (GameObject casaInicial in mapa[0]) // Para cada vértice da primeira linha
        {
            // Gera um caminho aleatório até a última linha
            GameObject casaAtual = casaInicial;
            caminhoValido.Add(casaAtual); // Marca a casa inicial como parte de um caminho válido

            for (int y = 1; y < linhas; y++)
            {
                List<GameObject> proximasOpcoes = mapa[y]; // Obtém as casas da próxima linha

                // Escolhe uma casa aleatória na próxima linha
                GameObject proximaCasa = proximasOpcoes[Random.Range(0, proximasOpcoes.Count)];

                // Conecta a casa atual com a próxima casa
                CriarCaminho(casaAtual, proximaCasa);

                casaAtual = proximaCasa; // Atualiza a casa atual para continuar o caminho
                caminhoValido.Add(casaAtual); // Marca a casa como parte de um caminho válido
            }
        }
    }

    // Cria um caminho entre duas casas
    void CriarCaminho(GameObject casaA, GameObject casaB)
    {
        Vector3 posA = casaA.transform.position;
        Vector3 posB = casaB.transform.position;

        // Instancia uma linha/caminho entre as casas
        GameObject linha = Instantiate(linhaPrefab);
        linha.transform.SetParent(transform);

        // Define a posição e rotação da linha para conectá-las
        linha.transform.position = (posA + posB) / 2; // Posição no meio
        linha.transform.up = (posB - posA).normalized; // Ajusta a orientação
        float fatorDeEscala = 0.6f; // Por exemplo, 50% do tamanho original
    linha.transform.localScale = new Vector3(0.1f, Vector3.Distance(posA, posB) * fatorDeEscala, 1); // Ajusta o tamanho

        // Armazena a conexão
        conexoes[casaA].Add(casaB);
    }

    // Remove as casas que não fazem parte de um caminho válido
    void RemoverBecosSemSaida()
    {
        for (int y = 0; y < linhas; y++)
        {
            foreach (GameObject casa in mapa[y])
            {
                if (!caminhoValido.Contains(casa)) // Se a casa não faz parte de um caminho válido
                {
                    Destroy(casa); // Remove a casa do tabuleiro
                }
            }
        }
    }

    // Atribui temas aleatórios às casas que fazem parte de um caminho válido
    void AtribuirTemas()
    {
        foreach (GameObject casa in caminhoValido)
        {
            // Escolhe um tema aleatório
            string tema = temas[Random.Range(0, temas.Length)];
            // Muda a cor da casa de acordo com o tema
            SpriteRenderer sr = casa.GetComponent<SpriteRenderer>();
            sr.color = coresTemas[tema];
            // Renomeia a casa com o tema
            casa.name = $"{casa.name} - {tema}";
        }
    }
}
