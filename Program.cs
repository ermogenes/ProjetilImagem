using System.Drawing;

// Recebe os parâmetros
const double gravidade = 9.80665;
double velocidade, anguloGraus, anguloRadianos, altura, alcance;

Console.WriteLine("-- Projétil --\n");

Console.Write("Entre com a velocidade, em m/s..: ");
velocidade = Convert.ToDouble(Console.ReadLine());

Console.Write("Entre com o ângulo, em graus....: ");
anguloGraus = Convert.ToDouble(Console.ReadLine());

// Calcula alcance e altura máxima
anguloRadianos = anguloGraus * Math.PI / 180;

altura = Math.Pow(velocidade * Math.Sin(anguloRadianos), 2) / (2 * gravidade);
alcance = (Math.Pow(velocidade, 2) * Math.Sin(anguloRadianos * 2)) / gravidade;

Console.WriteLine($"\nAlcance........: {alcance:N2} m");
Console.WriteLine($"Altura máxima..: {altura:N2} m");

Console.WriteLine("\nRenderizando imagem...");

// Define pontos de origem
int xOrigem = 10;
int yOrigem = (int)altura + 10;
int margem = 20;

// Calcula tamanho da imagem
int larguraImagem = (int)alcance + 2 * xOrigem + 2 * margem;
int alturaImagem = yOrigem + 3 * margem;

// Prepara a imagem inicial
using Bitmap fundo = new(larguraImagem, alturaImagem);
using Graphics gfx = Graphics.FromImage(fundo);

gfx.Clear(Color.White);

// Configura anti-aliasing
gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
gfx.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

// Cores dos traços
Pen canetaParabola = new Pen(Color.Red, 2);
Pen canetaRetas = new Pen(Color.GreenYellow);
// Pen canetaCurvaSpLine = new Pen(Color.Yellow);

// Desenha curvas de referência
Point pontoInicial = new Point(xOrigem, yOrigem);
Point pontoCulminante = new Point(xOrigem + (int)alcance / 2, yOrigem - (int)altura);
Point pontoFinal = new Point(xOrigem + (int)alcance, yOrigem);

var pontos = new[] {
    pontoInicial,
    pontoCulminante,
    pontoFinal,
};

// gfx.DrawCurve(canetaCurvaSpLine, pontos);
gfx.DrawLines(canetaRetas, pontos);

// Decompõe a velocidades em vertical e horizontal
double velocidadeX = velocidade * Math.Cos(anguloRadianos);
double velocidadeY = velocidade * Math.Sin(anguloRadianos);

// Tempo para atingir o ponto mais alto
double tempoCulminante = velocidadeY / gravidade;

// Escreve resultados do programa
Point pontoResultado = new Point(xOrigem + (int)alcance / 2, yOrigem - (int)altura / 2);
gfx.DrawString($"Velocidade = {velocidade:N2}m/s\nÂngulo = {anguloGraus:N2}°\nAltura = {altura:N2} em {tempoCulminante:N2}s\nAlcance = {alcance:N2}\nt = 1s, 1px = 1m", SystemFonts.DefaultFont, Brushes.Blue, pontoResultado);

// Desenho da parábola

// t em s
// Escala 1px = 1m

// Tempo e posição iniciais
int t = 1;
double x = 0;
double xAnterior = 0;
double y = 0;
double yAnterior = 0;

// Termina quando atingir o solo
while (y >= 0)
{
    // Calcula o ponto no tempo atual
    x = velocidadeX * t;
    y = velocidadeY * t - (gravidade * Math.Pow(t, 2)) / 2;

    // Pontos da reta, a partir da origem
    Point inicio = new Point((int)(xOrigem + xAnterior), (int)(yOrigem - yAnterior));
    Point fim;

    // Se passar do solo, usa o ponto final
    if (y < 0)
    {
        fim = new Point((int)(xOrigem + alcance), (int)(yOrigem));
    }
    else
    {
        fim = new Point((int)(xOrigem + x), (int)(yOrigem - y));
    }

    // Desenha a reta
    gfx.DrawLine(canetaParabola, inicio, fim);

    // Referências das coordenadas
    Point tempo;
    if (y > 0)
    {
        tempo = new Point(fim.X, xOrigem + (int)altura + margem);
    }
    else
    {
        // No último, mostra no topo
        tempo = new Point(fim.X, xOrigem);
    }

    Point valorY = new Point(fim.X, fim.Y);
    gfx.DrawString($"t={t}\n{x:N2}", SystemFonts.DefaultFont, Brushes.Black, tempo);
    gfx.DrawString($"{y:N2}", SystemFonts.DefaultFont, Brushes.Gray, valorY);

    // Log de ajuda no terminal
    Console.WriteLine($"t={t}, ({xAnterior:N2}, {yAnterior:N2}) => ({x:N2}, {y:N2})");

    // Passa para o próximo tempo
    t += 1;

    // Guarda o ponto atual para a próxima execução
    xAnterior = x;
    yAnterior = y;
}

// Salva a imagem
string nomeArquivo = $"lancamento-{velocidade:N2}mps-{anguloGraus:N2}°.png";
fundo.Save(nomeArquivo);

Console.WriteLine($"\nGerada imagem: {nomeArquivo}.");
