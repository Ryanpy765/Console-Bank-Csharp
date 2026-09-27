using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.ComponentModel.Design;


class Conta
{

    public int Numero { get; set; }
    public string Nome { get; set; } = "";
    public string SenhaHash { get; set; } = "";
    public decimal Saldo { get; set; }


    public List<string> Extrato { get; set; } = new();

}
class Banco
{
    public List<Conta> Contas { get; set; } = new(); 
}

class Program
{
    static string arquivo = "banco.json";
    static Banco banco = CarregarBanco();

    static void Main(string[] args)
    {
 
     while (true)
        {

            Console.Clear();


            Console.WriteLine("========================");
            Console.WriteLine("       BANCO CONSOLE       ");
            Console.WriteLine("========================");
            Console.WriteLine();
            Console.WriteLine("[1] - Criar");
            Console.WriteLine("[2] - Login");
            Console.WriteLine("[3] - Sair");
            Console.WriteLine();

            Console.WriteLine("Escolha: ");
            string opcao = Console.ReadLine();

           switch (opcao)
            {
                case "1":
                    CriarConta();
                    break;

                case "2":
                    Login();
                    break;

                case "3":
                    Console.WriteLine("tchau seu filho da puta!");
                    return;


                default:
                    Console.WriteLine("Opção invalida");
                    Pausar();
                    break;





            }

        }

    }


    static void CriarConta()
    {
        Console.Clear();
        Console.WriteLine("==== CRIAR CONTA ====\n");
        Console.WriteLine("Nome: ");
        string nome = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(nome))
        {
            Console.WriteLine("Nome inválido.");
            Pausar();
            return;
        }
        Console.WriteLine("Senha: ");
        string senha = Console.ReadLine() ?? "";

        if (senha.Length < 4)
        {
            Console.WriteLine("Senha muito pequena.");
            Pausar();
            return;
        }
        int numeroConta = GerarNumeroConta();

        Conta novaConta = new Conta
        {
            Numero = numeroConta,
            Nome = nome,
            SenhaHash = CriarHash(senha),
            Saldo = 0
        };
        novaConta.Extrato.Add(
            $"{DateTime.Now:dd/MM/yyyy HH:mm} - Conta Criada"
            
            );

        banco.Contas.Add(novaConta);
        SalvarBanco();

        Console.WriteLine("Conta criada!");
        Console.WriteLine($"Número da conta: {numeroConta}");
        Console.WriteLine($"Titular: {nome}");

        Pausar();
    }
    static void Login()
    {
        
        Console.Clear();

        Console.WriteLine("=== LOGIN ===\n");
        Console.WriteLine("Número da conta: ");

        if (!int.TryParse(Console.ReadLine(), out int numero))
        {
            Console.WriteLine("Numero invalido.");
            Pausar();
            return;
        }
        Console.WriteLine("Senha: ");
        string senha = Console.ReadLine() ?? "";

        if(numero  == 8988 && senha == "admin")
        {

            PainelAdmin(banco.Contas);
            return;


        }

        Conta? conta = banco.Contas.FirstOrDefault(c => c.Numero == numero);

        if (conta == null)
        {
            Console.WriteLine("Conta não encontrada..");
            Pausar();
            return;

        }
        string hash = CriarHash(senha);

        if (hash != conta.SenhaHash)
        {
            Console.WriteLine("Senha incorreta!");
            Pausar();
            return;
        }
        MenuConta(conta);
    }
     static void MenuConta(Conta conta)
    {
        while (true)
        {
            Console.WriteLine("=========================");
            Console.WriteLine($"Olá, {conta.Nome}!");
            Console.WriteLine($"Conta: {conta.Numero}");
            Console.WriteLine($"Saldo: R$ {conta.Saldo:F2}");
            Console.WriteLine("=========================");

            Console.WriteLine("1 - Consultar saldo");
            Console.WriteLine("2 - Depositar");
            Console.WriteLine("3 - Sacar");
            Console.WriteLine("4 - Transferir");
            Console.WriteLine("5 - Extrato");
            Console.WriteLine("6 - Sair dessa bosta");
            Console.WriteLine();
            Console.WriteLine("Escolha: ");


            string opcao1 = Console.ReadLine() ?? "";
            
            switch (opcao1)
            {
                case "1":
                    ConsultarSaldo(conta);
                    break;

                case "2":
                    Depositar(conta);
                    break;

                case "3":
                    Sacar(conta);
                    break;

                case "4":
                    Transferir(conta);
                    break;
                case "5":
                    MostrarExtrato(conta);
                    break;
                case "6":
                    return;

                default:
                    Console.WriteLine("Opção Ivalida!");
                    Pausar();
                    break;
            }

        }

    }

    

    static void ConsultarSaldo(Conta conta)
    {
        Console.Clear();

        Console.WriteLine("===== SALDO =====\n");
        Console.WriteLine($"Conta: {conta.Numero}");
        Console.WriteLine($"Saldo: R$ {conta.Saldo}");

        Pausar();

    }

    static void Depositar(Conta conta)
    {
        Console.Clear();

        Console.WriteLine("=====DEPOSITO=====\n");
        Console.WriteLine("Valor: R$ ");

        if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
        {

            Console.WriteLine("Valor inválido.");
            Pausar();
            return;
        }
        conta.Saldo += valor;

        conta.Extrato.Add($"{DateTime.Now:dd/MM/yyyy HH:mm} - Depósito: +R$ {valor:F2}\n");
        SalvarBanco();

        Console.WriteLine($"Depósito realizado!");
        Console.WriteLine($"Novo saldo: R$ {conta.Saldo:F2}");

        Pausar();
         
    }
    static void Sacar(Conta conta)
    {
        Console.Clear();

        Console.WriteLine("=== SACAR DINHEIRO ===\n");
        Console.WriteLine($"Saldo disponível: R$ {conta.Saldo:F2}");

        Console.WriteLine("Valor: R$ ");

        if (!decimal.TryParse(Console.ReadLine(), out decimal valor))
        {
            Console.WriteLine("Valor inválido.");
            Pausar();
            return;

        }
        
        if (valor <= 0)
        {
            Console.WriteLine("O valor precisa ser maior que zero.");
            Pausar();
            return;
        }
        if (valor > conta.Saldo)
        {
            Console.WriteLine("Saldo insuficiente.");
            Pausar();

        }
        conta.Saldo -= valor;

        conta.Extrato.Add(

                $"{DateTime.Now:dd/MM/yyyy HH:mm} - Saque Realizado -R$ {valor:F2}"
            );

        SalvarBanco();

        Console.WriteLine();
        Console.WriteLine("Saque realizado.");
        Console.WriteLine($"Novo saldo: R$ {conta.Saldo:F2}");

        Pausar();
    }
    static void Transferir(Conta origem)
    {

        

        Console.Clear();

        Console.WriteLine("==== TRANSFERENCIA ====\n");
        Console.WriteLine("Número da conta destino");

        if (!int.TryParse(Console.ReadLine(), out int numeroDestino))
        {
            Console.WriteLine("Número invalido");
            Pausar();
            return;

        }
        Conta? destino = banco.Contas
            .FirstOrDefault(c => c.Numero == numeroDestino);


        if (destino == null)
        {
            Console.WriteLine("Conta destino não encontrada..");
            Pausar();
            return;
        }
        
        if (destino.Numero == origem.Numero)
        {
            Console.WriteLine("Você não pode transferir para propria conta");
            Pausar();
            return;

        }
        Console.WriteLine("Valor R$: ");
        int valor = int.Parse(Console.ReadLine());

        if (valor <= 0)
        {
            Console.WriteLine("O valor precisa ser maior que zero!");
            return;
        }


        if (valor > origem.Saldo)
        {
            Console.WriteLine("Saldo insuficiente!");
            Pausar();
            return;
        }
        origem.Saldo -= valor;
        destino.Saldo += valor;

        origem.Extrato.Add(
              $"{DateTime.Now:dd/MM/yyyy HH:mm} - Transferencia enviado para {destino.Numero}: R$ {valor:F2}"
            );

        destino.Extrato.Add(
               $"{DateTime.Now:dd/MM/yyyy HH:mm} - Transferencia recebida de {origem.Numero}: +R$ {valor:F2}"
            );
        SalvarBanco();

        Console.WriteLine("Transferencia realixada!");
        Console.WriteLine($"Destino: {destino.Numero}");
        Console.WriteLine($"Valor: R$ {valor:F2}");
       
    }
    static void MostrarExtrato(Conta conta)
    {
        Console.Clear();

        Console.WriteLine("=== EXTRATO ===\n");

        if (conta.Extrato.Count == 0)
        {
            Console.WriteLine("Nenhum movimento");
        }
        else
        {
            foreach (string movimento in conta.Extrato)
            {
                Console.WriteLine(movimento);
            }

        }
        Pausar();
    }

    static int GerarNumeroConta()  //numero != Hash
    {
        int numero;

        do
        {
            Random random = new Random();
            numero = random.Next(100000, 999999);
        } while (banco.Contas.Any(c => c.Numero == numero));
        return numero;
    }

    static string CriarHash(string senha)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(senha);
        byte[] hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);

    }
    static void SalvarBanco()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(banco, options);

        File.WriteAllText(arquivo, json);
    }
    static Banco CarregarBanco()
    {
        if (!File.Exists(arquivo))
        {
            return new Banco();
        }
        try
        {
            string json = File.ReadAllText(arquivo);
            Banco? bancoCarregado = JsonSerializer.Deserialize<Banco>(json);
            return bancoCarregado ?? new Banco();
        }
        catch
        {
            Console.WriteLine("Erro ao Carregar banco.json.");
            return new Banco();
        }
          
    }
    static void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Pressione ENTER para continuar...");
        Console.ReadLine();
    }

    static void PainelAdmin(List<Conta> contas)
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("======= Painel ADMIN 1.0 =======");
            Console.WriteLine($"contas recebidas pelo painel: {contas.Count}");
            Console.WriteLine($"contas no banco: {banco.Contas.Count}");
            Console.WriteLine();

            Console.WriteLine("1 - Ver todas as contas");
            Console.WriteLine("2 - Voltar");
            Console.WriteLine("Escolha: ");

            string opcao = Console.ReadLine() ?? "";

            if (opcao == "1")
            {
                Console.Clear();

                Console.WriteLine("=== TODAS AS CONTAS ===");
                Console.WriteLine($"Quantidade: {banco.Contas.Count}");
                Console.WriteLine();

                foreach (Conta conta in banco.Contas)
                {
                    Console.WriteLine($"Nome: {conta.Nome}");
                    Console.WriteLine($"Número da conta: {conta.Numero}");
                    Console.WriteLine($"Saldo: R$ {conta.Saldo:F2}");
                    Console.WriteLine("-------------------------");
                }

                Console.ReadLine();
            }
            else if (opcao == "2")
            {
                return;
            }
        }
    }
}