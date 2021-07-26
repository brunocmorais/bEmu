namespace bEmu.Core.GUI.Popups
{
    public class AboutPopup : MessagePopup
    {
        private static string title = "Sobre o bEmu";
        private static string text = "Desenvolvido com <3 por Bruno Costa de Morais - v0.1a";

        public AboutPopup(IMain game) : 
            base(game, PopupSize.Medium, text, title, new Button("OK", true))
        {
        }
    }
}