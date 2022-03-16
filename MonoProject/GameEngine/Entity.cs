namespace MonoProject.GameEngine;

public abstract class Entity
{
    protected Vector2 position;
    protected Vector2 velocity;
    protected float angle;
    protected Color color;

    public Entity()
    {

    }

    public virtual void Update(GameTime gameTime)
    {
        this.position += this.velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
    }

    public virtual void Draw(Shapes shapes)
    {

    }
}
