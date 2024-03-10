using UnityEngine.UIElements;
using UnityEngine;

class miniMap
{
    MapManager _mapManager;
    VisualTreeAsset maptile;
    public VisualElement grid;
    public bool generated = true;

    public miniMap(MapManager reference)
    {
        _mapManager = reference;
        maptile = Resources.Load<VisualTreeAsset>("Templates/miniMaptile");
        grid = new VisualElement { style = { flexDirection = FlexDirection.Row } };
        var temp = _mapManager.getMap();
        if (temp == null) generated = false;
        if (generated) generate();
    }
    public void generate()
    {
        var temp = _mapManager.getMap();
        if (temp == null) { generated = false; return; }
        if (temp != null) generated = true;

        for (int i = temp.GetLength(0) - 1; i > 0; i--)
        {
            var column = new VisualElement { style = { flexDirection = FlexDirection.Column } };
            for (int j = temp.GetLength(1) - 1; j > 0; j--)
            {
                Color color = Color.grey;
                //DO better //this is gross
                if (temp[i, j].terrainDif == 115)
                {
                    color = Color.white;
                }
                if (temp[i, j].terrainDif == 1)
                {
                    color = Color.green;
                }
                if (temp[i, j].terrainDif == 2)
                {
                    color = Color.yellow;
                }
                if (temp[i, j].terrainDif == 3)
                {
                    color = Color.green;
                }
                if (temp[i, j].terrainDif == 110)
                {
                    color = Color.blue;
                }

                var tile = maptile.Instantiate();
                tile.style.backgroundColor = color;
                column.Add(tile);
            }
            grid.Add(column);
        }
    }
    public VisualElement getMiniMap()
    {
        return grid;
    }
}
class gridItem
{

    public int type;
    public Texture spriteImage;
    public int health;
    public int maxHealth;
    public Color color;
    public EntityBase reference;
    public gridItem(int type, Texture spriteImage, int health, int maxHealth, EntityBase reference)
    {
        this.type = type;
        this.spriteImage = spriteImage;
        this.health = health;
        this.maxHealth = maxHealth;
        this.reference = reference;
        if (type == 1)
        {
            color = Color.green;
        }
        if (type == 2)
        {
            color = Color.blue;
        }
        if (type == 3)
        {
            color = Color.red;
        }
    }
}

class horizontalItem
{
    public int type; public Texture spriteImage; public int cost; public Color color; public GameObject reference;

    public horizontalItem(int type, Texture spriteImage, int cost, GameObject reference)
    {
        this.type = type;
        this.spriteImage = spriteImage;
        this.cost = cost;
        this.reference = reference;
        if (type == 1)
        {
            color = Color.green;
        }
        if (type == 2)
        {
            color = Color.blue;
        }
        if (type == 3)
        {
            color = Color.red;
        }
    }
}
struct camera
{

    float max_x;
    float max_y;

    public float current_x;
    public float current_y;
    public bool init;
    public camera(float _x, float _y)
    {
        init = true;
        max_x = 160;
        max_y = 80;
        current_x = 160;
        current_y = 80;
    }
    public void updatePos(float _x, float _y)
    {
        _y = -_y;
        if ((current_x + _x) > 0 && (current_x + _x) < max_x)
        {
            current_x = current_x + _x;
        }
        if ((current_y + _y) > 0 && (current_y + _y) < max_y)
        {
            current_y = current_y + _y;
        }
    }
}