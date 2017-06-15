using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;




namespace gameClient {


    enum GAME_STATE {
        START_MENU,
        PLAY,
        EXIT,
        LOST,
        WAIT,
        START
    };

    enum KEYS {
        RIGHT_PRESSED,
        LEFT_PRESSED,
        RIGHT_RELEASED,
        LEFT_RELEASED,
        NONE
    };

    class Game {

        IAsyncResult result;

        static int numPlayers = 2;

        Color[] colors;
        int[] directions;
        Vector2f[] positions;

        static int WIDTH = 800;
        static int HEIGHT = 500;
        RenderWindow window;
        Clock clock;
        GameTexture texture;
        MainPlayer player;
        public MainPlayer Player{
            get { return player; }
        }

        Menu menu;
        static GAME_STATE gameState;
        NetworkManager network;
        static RenderStates states =  RenderStates.Default;
        Color windowColor;
        Dictionary<byte, NonPlayer> nonPlayers;

        public Game() {

            clock = new Clock();

            network = new NetworkManager(this);
            ContextSettings contextSettings = new ContextSettings(); 
            contextSettings.DepthBits = 32;

            windowColor = new Color(0, 0, 0);

            window = new RenderWindow(new VideoMode((uint)WIDTH, (uint)HEIGHT), "Game", Styles.Default, contextSettings);


            window.SetActive(); 

            window.Closed += new EventHandler(OnClosed);
            window.KeyPressed += new EventHandler<KeyEventArgs>(OnKeyPressed);
            menu = new Menu(WIDTH, HEIGHT);

            colors = new Color[4];
            directions = new int[4];
            positions = new Vector2f[4];

            gameState = GAME_STATE.START_MENU;
            colors[0] = new Color(Color.Red);
            colors[1] = new Color(Color.Green);
            colors[2] = new Color(Color.Blue);
            colors[3] = new Color(Color.Yellow);
            directions[0] = 0;
            directions[1] = 90;
            directions[2] = -90;
            directions[3] = 180;
            positions[0] = new Vector2f(WIDTH / 4, HEIGHT / 2);
            positions[1] = new Vector2f(WIDTH / 2, HEIGHT / 4);
            positions[2] = new Vector2f(WIDTH / 2, HEIGHT - HEIGHT / 4);
            positions[3] = new Vector2f(WIDTH - WIDTH / 4, HEIGHT / 2);

            nonPlayers = new Dictionary<byte, NonPlayer>();

        }

        void init() {
           // Console.Write("IP: ");            
           // string ip = "";
            //ip = Console.ReadLine();
            //Console.Write("PORT: ");
            //string port = "";

            texture = new GameTexture(WIDTH, HEIGHT);
            gameState = GAME_STATE.PLAY;
            window.SetFramerateLimit(60);
            //player = new MainPlayer(positions[0], directions[0], colors[0], texture, 1);
            player = new MainPlayer(positions[network.Id], directions[network.Id], colors[network.Id], texture, network.Id);
            initPlayers();

            initialRender();

            Thread.Sleep(800); //zmienic na coś innego

            Task.Run(() => network.Send());

            clock.Restart();
        }


        void initPlayers() {

            for (byte i = 0; i < numPlayers; i++){
                if (i != network.Id)
                    nonPlayers.Add(i, new NonPlayer(positions[i], directions[i], colors[i], texture, (byte)i));
                
            }

        }

        public void updateNonPlayers(byte[] data) {
            byte id = data[2];
            if (nonPlayers.ContainsKey(id)) {
                nonPlayers[id].Gap = (data[0] == 0) ? false : true;
                nonPlayers[id].Key = (KEYS)data[1];
            }
        }


        public void run() {

            while (window.IsOpen) {

                window.DispatchEvents();

                switch (gameState) {
                    case GAME_STATE.START_MENU:
                        renderMenu();
                        break;
                    case GAME_STATE.PLAY:
                        update();
                        renderPlay();
                        break;
                    case GAME_STATE.LOST:
                        renderLostMenu();
                        break;
                    case GAME_STATE.WAIT:
                        renderWaitMenu();
                        if (result.IsCompleted)
                            init();
                        break;
                    case GAME_STATE.START:
                        
                        break;
                    default:
                        break;
                }

            }
        }

        void initialRender() {
            window.Clear();

            states.Transform = player.transform;
            window.Draw(player.Circle, states);
            foreach (KeyValuePair<byte, NonPlayer> np in nonPlayers) {
                states.Transform = np.Value.transform;
                window.Draw(np.Value.Circle, states);
            }
            window.Display();
        }

        void renderMenu() {
            window.Clear(windowColor);
            foreach (Text item in menu.MenuItems)
                window.Draw(item);

            window.Display();
        }

        void renderLostMenu() {
            window.Clear(windowColor);
            window.Draw(menu.Lost);
            window.Display();
        }

        void renderPlay() {
            window.Clear();
            window.Draw(texture.Sprite);

            states.Transform = player.transform;
            window.Draw(player.Circle, states);
            foreach (KeyValuePair<byte, NonPlayer> np in nonPlayers) {
                states.Transform = np.Value.transform;
                window.Draw(np.Value.Circle, states);
            }
            window.Display();
        }

        void renderWaitMenu() {
            window.Clear(windowColor);
            window.Draw(menu.Wait);
            window.Display();
        }

        static void OnClosed(object sender, EventArgs e) {
            Window window = (Window)sender;
            window.Close();
        }

        void OnKeyPressed(object sender, KeyEventArgs e) {

            switch (gameState) {
                case GAME_STATE.START_MENU:
                    menuEvents(sender, e);
                    break;
                case GAME_STATE.PLAY:
                    playEvents(sender, e);
                    break;
                case GAME_STATE.LOST:
                    lostEvents(sender, e);
                    break;
                default:
                    break;
            }

        }


        void menuEvents(object sender, KeyEventArgs e) {

            switch (e.Code) {
                case Keyboard.Key.Up:
                    menu.moveUp();
                    break;
                case Keyboard.Key.Down:
			        menu.moveDown();
			        break;
                case Keyboard.Key.Return:
                    if (menu.SelectedItem == 0){
				        gameState = GAME_STATE.WAIT;
				        renderWaitMenu();
                       
                        //Task<Tuple<String, String>> res = Task.Run(() => getIPPort());
                        //Tuple<String, String> ip_port = await res;
                        Action del = network.init;
                        result = del.BeginInvoke(null, null);
			        }	
			        else if (menu.SelectedItem == 1){				
				        window.Close();
			        }
                    break;
                default:
                    break;
            }

        }

        //Tuple<String, String> getIPPort() {
        //    Console.WriteLine("IP: ");
        //    String ip = Console.ReadLine();
        //    Console.WriteLine("PORT: ");
        //    String port = Console.ReadLine();
        //    return Tuple.Create(ip, port);
        //}

        static void playEvents(object sender, KeyEventArgs e) {
  
        }

        static void lostEvents(object sender, KeyEventArgs e){
            Window window = (Window)sender;
            switch (e.Code) {
	            case Keyboard.Key.Return:
			        window.Close();
		            break;
	            default:
		            break;
	        }
        }
 
        void update(){

            Time time = clock.ElapsedTime;
            if (!player.move(texture, time.AsMilliseconds())) {
                //network.StopReceiving();
                gameState = GAME_STATE.LOST;
            }
            //player.move(texture, time.AsMilliseconds());
            //clock.restart().asMilliseconds();
            List<byte> toRemove = new List<byte>();
            foreach (KeyValuePair<byte, NonPlayer> np in nonPlayers) {
                //np.Value.move(texture, time.AsMilliseconds());
                if (!np.Value.move(texture, time.AsMilliseconds()))
                    toRemove.Add(np.Key);                 
            }

            foreach (byte i in toRemove)
                nonPlayers.Remove(i);

            clock.Restart().AsMilliseconds();
        }
    
    }
}
