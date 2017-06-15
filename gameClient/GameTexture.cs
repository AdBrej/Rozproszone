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
    class GameTexture {
        
        RenderTexture renderTexture;
        RenderStates states = RenderStates.Default;
        Sprite sprite;

        public Sprite Sprite {
            get { return sprite; }
           
        }

        uint[,] pixels;
        int width;
        int height;


        public GameTexture(int width, int height) {

            renderTexture = new RenderTexture((uint)width, (uint)height);

            renderTexture.Smooth = true;
            sprite = new Sprite();
            sprite.Texture = renderTexture.Texture;
            renderTexture.Clear();
            //sprite.Color = new Color(Color.Red);
	        this.width = width;
	        this.height = height;

	        pixels = new uint[width, height];
      
        }

        public void drawToTexture(RectangleShape rect, Transform t, int size, Vector2f point){

	        setPixels(size, point);
            states.Transform = t;
            renderTexture.Draw(rect, states);
            renderTexture.Display();

        }

        void setPixels(int radius, Vector2f origin){
	        for (int y = -radius; y <= radius; y++){
		        for (int x = -radius; x <= radius; x++){
			        if (x*x + y*y <= radius*radius){
				        int tempx = (int)origin.X + x;
				        int tempy = (int)origin.Y + y;
				        if (tempx >=0 && tempx <width && tempy >=0 && tempy <height)
					        pixels[tempx, tempy] = 1;				
			        }
		        }
	        }

        }


        public bool checkCollision(Vector2f position){
	        if (position.X >= width || position.Y >= height || position.X <= 0 || position.Y <= 0 
		        || pixels[(uint)position.X, (uint)position.Y] == 1){
                
                //Console.WriteLine("KOLIZJA");
		        return true;
	        }
	        return false;
	
        }
    }
}
