using System;
using System.Threading.Tasks;
using Life;
using Life.Network;
using Life.UI;
using Life.BizSystem;
using System.Net.Http;
using Newtonsoft.Json;

namespace AntiFreePunch
{
    public class Main : Plugin
    {
        public Main(IGameAPI gameAPI) : base(gameAPI) { }
        public class Ini
        {
            public string ini {  get; set; }
            public Ini()
            {
                ini = "MonWebhook";
            }
        }
        private async Task SendWebhookIni(string webhookUrl)
        {
            var embed = new
            {
                embeds = new[]
                {
                    new
                    {
                        title = "Initialisation du plugin **[AntiFP98]**",
                        description = $"",
                        color = 0xe72e2e,
                        fields = new[]
                        {
                            new { name = "**Nom du serveur en liste**", value = $"{Nova.serverInfo.serverListName} \n" ?? "Inconnu\n", inline = true },

                        },
                        footer = new
                        {
                            text = "AntiFP98",
                            icon_url = ""
                        }
                    }
                }
            };
            var content = JsonConvert.SerializeObject(embed);
            using (var httpClient = new HttpClient())
            {
                var requestContent = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
                await httpClient.PostAsync(webhookUrl, requestContent);
            }
        }
        public override async void OnPluginInit()
        {
            base.OnPluginInit();
            Nova.server.OnPlayerDamagePlayerEvent += new Action<Player, Player, int>(OnPlayerDamageOtherPlayer);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Le plugin [AntiFreePunch] est initialisée ! By Fenix98");
            Console.ResetColor();
            Ini init = new Ini();
            await SendWebhookIni(init.ini);
        }
        public void OnPlayerDamageOtherPlayer(Player Attaquant, Player Victime, int damage)
        {
            if (Attaquant.setup.areaId == 6 || Attaquant.setup.areaId == 185 || Attaquant.setup.areaId == 1 || Attaquant.setup.areaId == 170 || Attaquant.setup.areaId == 4)
            {
                if (!Attaquant.biz.IsActivity(Activity.Type.LawEnforcement) && !Attaquant.serviceMetier) 
                {
                    Victime.setup.Networkhealth =+ damage;
                    Victime.Notify("Information", "Votre vie vous a étais rendue", NotificationManager.Type.Info);
                    AttaquantWarningPanel(Attaquant);
                }
            }
        }
        public void AttaquantWarningPanel(Player Attaquant)
        {
           UIPanel panel = new UIPanel("<color=red>Anti-Free-Punch</color>", UIPanel.PanelType.Text);
           panel.SetText("Vous n'avez pas le droit de frapper quelqu'un dans cette zone !");
           panel.AddButton("Fermer", ui => Attaquant.ClosePanel(ui));
           Attaquant.ShowPanelUI(panel);
        }
    }
}
