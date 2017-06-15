using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using SFML;
using SFML.Graphics;
using SFML.Window;

namespace gameClient {
    static class Program {   

        static void Main() {

            Game game = new Game();
            game.run();
            
        }
    } 
}