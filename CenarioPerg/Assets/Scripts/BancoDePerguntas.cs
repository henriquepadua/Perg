using System.Collections.Generic;

[System.Serializable]
public class Pergunta
{
    public string pergunta;
    public int dificuldade;
    public List<string> alternativas;
    public string correta;
}

[System.Serializable]
public class Categoria
{
    public string categoria;
    public List<Pergunta> perguntas;
}

[System.Serializable]
public class BancoDePerguntas
{
    public List<Categoria> categorias;
}