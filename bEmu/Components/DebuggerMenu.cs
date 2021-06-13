using System.Collections.Generic;
using System.Linq;
using System.Threading;
using bEmu.Core;
using bEmu.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Globalization;

namespace bEmu.Components
{
    public class DebuggerMenu : Menu
    {
        private const string CommandMarker = "> ";
        private string command = string.Empty;
        private readonly Keys[] validKeys = {
            Keys.A, Keys.B, Keys.C, Keys.D, Keys.E,
            Keys.F, Keys.G, Keys.H, Keys.I, Keys.J,
            Keys.K, Keys.L, Keys.M, Keys.N, Keys.O,
            Keys.P, Keys.Q, Keys.R, Keys.S, Keys.T,
            Keys.U, Keys.V, Keys.W, Keys.X, Keys.Y,
            Keys.Z, Keys.D0, Keys.D1, Keys.D2, 
            Keys.D3, Keys.D4, Keys.D5, Keys.D6, 
            Keys.D7, Keys.D8, Keys.D9, Keys.Space,
            Keys.Back, Keys.Enter
        };
        private IDebugger Debugger => game.GameSystem.System.Debugger;
        private List<MenuOption> items = new List<MenuOption>();

        public DebuggerMenu(IMainGame game) : base(game)
        {
            game.GameSystem.System.Debugger = new Debugger(game.GameSystem.System);
            UpdateMenuOptions();
        }

        public override string Title => "Debugger";

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            string lastCommand = command;
            bool executedCommand = false;
            
            foreach (var key in validKeys)
            {
                if (KeyboardStateExtensions.HasBeenReleased(key))
                {
                    switch (key)
                    {
                        case Keys.Space: 
                            command += " "; 
                            break;
                        case Keys.Back: 
                            if (command.Length > 0)
                                command = command.Substring(0, command.Length - 1); 
                            break;
                        case Keys.Enter:
                            ExecuteCommand();
                            executedCommand = true;
                            break;
                        default: 
                            string text = key.ToString().ToLower();
                            command += text[text.Length - 1];
                            break;
                    }
                }
            }

            if (lastCommand != command || executedCommand)
                UpdateMenuOptions();
        }

        public void ExecuteCommand()
        {
            AddLine(CommandMarker + command);

            if (string.IsNullOrWhiteSpace(command))
                return;

            string[] commandParts = command.Split(" ");

            switch (commandParts[0])
            {
                case "reg":
                    if (game.IsRunning)
                        GameIsRunning();
                    else
                        AddLine(Debugger.PrintRegisters());
                    break;
                case "print":
                    if (game.IsRunning)
                        GameIsRunning();
                    else
                        AddLine(Debugger.GetRegisterValue(commandParts[1]));
                    break;
                case "cls":
                    items.Clear();
                    break;
                case "listreg":
                    if (game.IsRunning)
                        GameIsRunning();
                    else
                        AddLine(string.Join('\n', Debugger.GetRegisters().Select(x => x.Name)));
                    break;
                case "help":
                    AddLine(PrintHelp());
                    break;
                case "quit":
                    game.Menu.CloseCurrentMenu();
                    break;
                case "go":
                    if (!game.IsRunning)
                        game.Pause();
                    break;
                case "stop":
                    if (game.IsRunning)
                        game.Pause();
                    break;
                case "m1":
                    if (game.IsRunning)
                        GameIsRunning();
                    else
                    {
                        byte value = Debugger.GetByteFromMemoryAddress(int.Parse(commandParts[1], NumberStyles.HexNumber));
                        AddLine(value.ToString("x"));
                    }
                    break;
                case "m2":
                    if (game.IsRunning)
                        GameIsRunning();
                    else
                    {
                        ushort value = Debugger.GetWordFromMemoryAddress(int.Parse(commandParts[1], NumberStyles.HexNumber));
                        AddLine(value.ToString("x"));
                    }
                    break;
                case "sm1":
                    if (game.IsRunning)
                        GameIsRunning();
                    else
                    {
                        int address = int.Parse(commandParts[1], NumberStyles.HexNumber);
                        byte value = byte.Parse(commandParts[2], NumberStyles.HexNumber);
                        Debugger.SetByteToMemoryAddress(address, value);
                        AddLine("Endereço " + address.ToString("x") + " definido com o valor " + value.ToString("x"));
                    }
                    break;
                default:
                    AddLine("Comando inválido: " + commandParts[0]);
                    break;
            }

            command = string.Empty;
            selectedOption = items.Count;
        }

        private void GameIsRunning()
        {
            AddLine("Jogo está rodando.");
        }

        public string PrintHelp()
        {
            return @"Lista de Comandos
 
help - mostrar esta ajuda
cls - limpar tela
quit - fechar debugger
s - executar próxima instrução
reg - imprimir registradores
listreg - listar registradores
print <x> - imprimir valor do registrador <x>
br <xxxx> - setar breakpoint em <xxxx>
cbr - limpar breakpoint
abr <xxxx> - setar breakpoint de acesso em <xxxx>
cabr - limpar breakpoint de acesso
go - continuar execução
stop - parar execução
m1 <xxxx> - obter byte da memória em <xxxx>
sm1 <xxxx> <yy> - setar endereço <xxxx> com o byte <yy>
m2 <xxxx> - obter word da memória em <xxxx>
sm2 <xxxx> <yyyy> - setar endereço <xxxx> com a word <yyyy>";
        }

        public void AddLine(string text)
        {
            foreach (var line in text.Split("\n"))
                items.Add(new MenuOption(line, line, typeof(int), (x) => {}, MenuOptionAlignment.Left));
        }

        public override IEnumerable<MenuOption> GetMenuOptions()
        {
            foreach (var item in items)
                yield return item;

            yield return new MenuOption(CommandMarker + command, CommandMarker + command, typeof(int), (x) => {}, MenuOptionAlignment.Left);
        }
    }
}