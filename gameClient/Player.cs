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
    class Player {

        private CircleShape circle;

        public CircleShape Circle {
            get { return circle; }
        }
	    protected RectangleShape rect;
	    protected Color color;
	    protected Vector2f position;
        public Transform transform;



	    protected int RADIUS = 4;
	    protected int RADIUS_ROTATION = 30;
	    protected int minGap = 200;
	    protected int maxGap = 600;
	    protected byte id;
	    protected float speed;

	    protected bool isPressed;
	    protected long beginTime;
	    protected bool isGap;
        protected bool gap;

        public bool Gap {
            get { return gap; }
            set { gap = value; }
        }

        protected KEYS key;

        public KEYS Key {
            get { return key; }
            set { key = value; }
        }

        protected Random rnd;


        public Player(Vector2f startPosition, int direction,  Color color, GameTexture texture, byte id) {
            rnd = new Random();
            //clock = new Clock();
            circle = new CircleShape(RADIUS);
            transform = Transform.Identity;

            rect = new RectangleShape();
            this.color = color;
            key = KEYS.NONE;

	        this.id = id;
	        speed = 0.1f;
	        gap = false;
            circle.FillColor = color;
            beginTime = 0;
	        circle.Origin = new Vector2f(RADIUS, RADIUS);
	        this.position = circle.Origin;
	        transform.Rotate(direction, startPosition); // początkowy kierunek
            circle.Position = startPosition; //początkowe położenie
	
	        isPressed = false;

	        beginTime = 0;
	        isGap = false;
	        setRect(color, new Vector2f(RADIUS*2, RADIUS*2), startPosition);
	
        }


        void setRect(Color color, Vector2f size, Vector2f startPosition){

	        rect.Origin = new Vector2f(size.X, size.Y / 2);
	        rect.FillColor = color;
	        rect.Size = size;
	        rect.Position = startPosition;
        }

        protected bool createGap(){	

	        if (!isGap){
		        int num = rnd.Next(1,80);
		        if (num == 1){
			        beginTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
			        isGap = true;
		        }
		
	        }
	        else{
		        long c = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

		        if ((c - beginTime) <= rnd.Next(minGap, maxGap)){

			        return true;
		        }
		        else{
			        isGap = false;
		        }
	        }

	        return false;
        }

    }
}
