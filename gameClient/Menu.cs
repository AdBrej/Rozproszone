using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SFML;
using SFML.Graphics;
using SFML.Window;
using SFML.System;

namespace gameClient {
    class Menu {

        static int numItems = 2;
        int selectedItem;

        public int SelectedItem {
            get { return selectedItem; }
            set { selectedItem = value; }
        }

        Font font;
	    Text[] menuItems;
        public Text[] MenuItems {
            get { return menuItems; }
        }

	    Text lost;
        public Text Lost {
            get { return lost; }
        }

	    Text wait;
        public Text Wait {
            get { return wait; }
        }
    
        public Menu(int width, int height){

            font = new Font("Lato-Regular.ttf");
            menuItems = new Text[numItems];

            for (int i = 0; i < numItems; i++)
                menuItems[i] = new Text();
            lost = new Text();
            wait = new Text();

                /*for (sf::Text t : menuItems){
                    t.setFont(font);
                    t.setFillColor(sf::Color::White);
                }*/
            initStartMenu(width, height);
            initLostMenu(width, height);
            initWait(width, height);
        }

        void initStartMenu(int width, int height){
            
	        menuItems[0].Font = font;
	        menuItems[0].Color = Color.Red;
	        menuItems[0].DisplayedString ="Play";
            Vector2f pos0 = new Vector2f((float)(width/2.0- menuItems[0].GetGlobalBounds().Width/2.0), (float)(height/3.0));
	        menuItems[0].Position = pos0;
            
	        menuItems[1].Font = font;
	        menuItems[1].Color = Color.White;
	        menuItems[1].DisplayedString ="Exit";
            Vector2f pos1 = new Vector2f((float)(width/2.0- menuItems[1].GetGlobalBounds().Width/2.0), (float)(height/2.0));
	        menuItems[1].Position = pos1;   
	        selectedItem = 0;
        }

        void initWait(int width, int height){

            wait.Font = font;
	        wait.Color = Color.Yellow;
	        wait.DisplayedString ="Wait for other players\n(Write ip and port)";
            Vector2f pos = new Vector2f((float)(width/2.0- wait.GetGlobalBounds().Width/2.0) ,(float)(height/2.0));
	        wait.Position = pos; 
	       
        }

        void initLostMenu(int width, int height){
	        lost.Font = font;
	        lost.Color = Color.Red;
	        lost.Scale =new Vector2f(2,2);

	        lost.DisplayedString ="You lost";
            Vector2f pos = new Vector2f((float)(width/2.0- lost.GetGlobalBounds().Width/2.0) ,(float)(height/2.0));
	        lost.Position = pos; 
	       
        }

        //public void drawLost(RenderWindow window){
        //    window.Draw(lost);
        //}

        //public void draw(RenderWindow window){

        //    for (int i = 0; i < numItems; i++)
        //        window.Draw(menuItems[i]);
        //}

        //public void drawWait(RenderWindow window){
        //    window.Draw(wait);
        //}

        public void moveUp(){

	        if (selectedItem - 1 >= 0)
	        {
		        menuItems[selectedItem].Color = Color.White;
		        selectedItem--;
		        menuItems[selectedItem].Color = Color.Red;
	        }
        }

        public void moveDown(){

	        if (selectedItem + 1 < numItems)
	        {
		        menuItems[selectedItem].Color = Color.White;
		        selectedItem++;
                menuItems[selectedItem].Color = Color.Red;
	        }
        }
    
    
    }
}
