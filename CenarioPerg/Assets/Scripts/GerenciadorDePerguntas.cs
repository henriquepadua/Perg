using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GerenciadorDePerguntas : MonoBehaviour
{
    public TextMeshProUGUI textoPergunta;
    public TextMeshProUGUI textoFeedback;
    public Button[] botoesResposta;
    public Button botaoProximaPergunta;

    private BancoDePerguntas bancoDePerguntas;
    private Pergunta perguntaAtual;
    private int nivelAtual = 1;
    private int acertos = 0, erros = 0;
    private List<Pergunta> perguntasAtuais = new List<Pergunta>();
    private int maxNivel = 5; // Atualizado para o nível máximo 5

    void Start()
    {
        // Carregar banco de perguntas
        TextAsset jsonFile = Resources.Load<TextAsset>("perguntas");
        bancoDePerguntas = JsonUtility.FromJson<BancoDePerguntas>(jsonFile.text);

        // Configurar botões
        foreach (Button botao in botoesResposta)
        {
            botao.onClick.AddListener(() => VerificarResposta(botao));
        }
        botaoProximaPergunta.onClick.AddListener(ProximaPergunta);

        AtualizarPerguntasAtuais();
        ProximaPergunta();
    }

    void AtualizarPerguntasAtuais()
    {
        perguntasAtuais.Clear();
        foreach (var categoria in bancoDePerguntas.categorias)
        {
            perguntasAtuais.AddRange(categoria.perguntas.FindAll(p => p.dificuldade == nivelAtual));
        }

        if (perguntasAtuais.Count == 0)
        {
            Debug.LogWarning("Não há perguntas disponíveis para o nível " + nivelAtual);
        }
    }

    void ProximaPergunta()
    {
        textoFeedback.gameObject.SetActive(false);

        if (perguntasAtuais.Count == 0)
        {
            nivelAtual = AStarProximoNivel();
            if (nivelAtual > maxNivel)
            {
                Debug.Log("Todas as perguntas foram respondidas.");
                textoPergunta.text = "Parabéns! Você completou o jogo.";
                botaoProximaPergunta.gameObject.SetActive(false);
                return;
            }
            AtualizarPerguntasAtuais();
        }

        if (perguntasAtuais.Count == 0)
        {
            Debug.LogWarning("Não há perguntas disponíveis para o nível " + nivelAtual);
            return;
        }

        int indicePergunta = Random.Range(0, perguntasAtuais.Count);
        perguntaAtual = perguntasAtuais[indicePergunta];
        perguntasAtuais.RemoveAt(indicePergunta);

        // Atualizar UI
        textoPergunta.text = perguntaAtual.pergunta;
        for (int i = 0; i < botoesResposta.Length; i++)
        {
            if (i < perguntaAtual.alternativas.Count)
            {
                botoesResposta[i].GetComponentInChildren<TextMeshProUGUI>().text = perguntaAtual.alternativas[i];
                botoesResposta[i].gameObject.SetActive(true);
            }
            else
            {
                botoesResposta[i].gameObject.SetActive(false); // Esconde botões extras
            }
        }

        Debug.Log("Pergunta atualizada: " + perguntaAtual.pergunta + " | Nível: " + nivelAtual);
    }

    void VerificarResposta(Button botao)
    {
        string respostaSelecionada = botao.GetComponentInChildren<TextMeshProUGUI>().text;
        if (respostaSelecionada == perguntaAtual.correta)
        {
            textoFeedback.text = "Correto!";
            textoFeedback.color = Color.green;
            acertos++;
        }
        else
        {
            textoFeedback.text = "Errado!";
            textoFeedback.color = Color.red;
            erros++;
        }

        textoFeedback.gameObject.SetActive(true);
    }

    int AStarProximoNivel()
    {
        // Implementação simples do algoritmo A* para determinar o próximo nível
        int novoNivel = nivelAtual;
        float taxaDeAcertos = (float)acertos / (acertos + erros);

        // Definir a heurística (por exemplo, taxa de acertos)
        if (taxaDeAcertos > 0.8f && novoNivel < maxNivel) novoNivel++; // Aumentar nível
        else if (taxaDeAcertos < 0.5f && novoNivel > 1) novoNivel--; // Diminuir nível

        Debug.Log("Novo nível calculado pelo A*: " + novoNivel);
        return novoNivel;
    }
}