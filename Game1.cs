using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Intel8080
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D whiteRect;
        CPU cpu;
        Texture2D blackRect;
        const int tamanhoPixel = 2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 224 * tamanhoPixel;
            graphics.PreferredBackBufferHeight = 256 * tamanhoPixel;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            cpu = new CPU();
            cpu.State.LoadProgram("invaders");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            whiteRect = new Texture2D(GraphicsDevice, tamanhoPixel, tamanhoPixel);
            blackRect = new Texture2D(GraphicsDevice, tamanhoPixel, tamanhoPixel);

            Color[] whiteColor = new Color[tamanhoPixel * tamanhoPixel];
            Color[] blackColor = new Color[tamanhoPixel * tamanhoPixel];
            
            for(int i=0; i < whiteColor.Length; ++i) 
            {
                whiteColor[i] = Color.White;
                blackColor[i] = Color.Black;
            }
            
            whiteRect.SetData(whiteColor);
            blackRect.SetData(blackColor);

            Task t = new Task (() => 
            {
                for (;;) 
                {
                    cpu.EmularCiclo();
                    Thread.Sleep(0);
                }	
            });

            t.Start();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw (GameTime gameTime)
		{
			GraphicsDevice.Clear (Color.Black);
			spriteBatch.Begin ();
			
			for (int i = 0; i < 224; i++) 
			{
				for (int j = 0; j < 256; j += 8)
				{
					byte sprite = cpu.State.Memory [0x2400 + ((i * 256 / 8) + j / 8)];

					for (int pixel = 0; pixel < 8; pixel++) 
                    {
                        int x = (j + pixel) * tamanhoPixel;
                        int y = i * tamanhoPixel;

                        Vector2 coor = new Vector2(y, (256 * tamanhoPixel) - x);

						if ((sprite & (1 << pixel)) > 0) 
                            spriteBatch.Draw (whiteRect, coor, Color.White);
                        else 
                            spriteBatch.Draw (blackRect, coor, Color.Black);
					}
				}
			}

            //spriteBatch.Draw(whiteRect, new Vector2(100, 100), Color.White);

			spriteBatch.End ();

            base.Draw (gameTime);	
		}
    }
}