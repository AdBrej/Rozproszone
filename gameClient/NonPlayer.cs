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
    class NonPlayer : Player {

        

        public NonPlayer(Vector2f startPosition, int direction, Color color, GameTexture texture, byte id)
            : base(startPosition, direction, color, texture, id) { 
        
            key = KEYS.NONE;
            gap = false;
        }

        public bool move(GameTexture texture, int time){

	        if (key == KEYS.RIGHT_PRESSED) {
                if (!isPressed) {

                    position.X = (float)(Circle.Position.X + Math.Cos((45 - Circle.Rotation) * Math.PI / 180));
                    position.Y = (float)(Circle.Position.Y + Math.Sin((45 - Circle.Rotation) * Math.PI / 180) + RADIUS_ROTATION);

                    isPressed = true;
                    
                }

                transform.Rotate(speed * time * 2, position);
                
            }
	        else if (key == KEYS.LEFT_PRESSED){
                if (!isPressed) {

                    position.X = (float)(Circle.Position.X + Math.Cos((45 - Circle.Rotation) * Math.PI / 180));
                    position.Y = (float)(Circle.Position.Y + Math.Sin((45 - Circle.Rotation) * Math.PI / 180) - RADIUS_ROTATION);

                    isPressed = true;
                }
                
                transform.Rotate(-speed * time * 2, position);
            }
	        else{
                float dX = (float)Math.Cos(Circle.Rotation * Math.PI / 180) * speed * time;
                float dY = (float)Math.Sin(Circle.Rotation * Math.PI / 180) * speed * time;

                transform.Translate(dX, dY);
                isPressed = false;
                
	        }


            int tempx = (int)(Circle.Position.X + Math.Cos(Circle.Rotation * Math.PI / 180) * (RADIUS + 1)); 
            int tempy = (int)(Circle.Position.Y + Math.Sin(Circle.Rotation * Math.PI / 180) * (RADIUS + 1));

            Vector2f col = transform.TransformPoint(tempx, tempy);

            if (texture.checkCollision(col))
                return false;

            Vector2f point = transform.TransformPoint(rect.Position);


            if (!gap)
                texture.drawToTexture(rect, transform, RADIUS, point);


	        return true;
        }
    }
}
