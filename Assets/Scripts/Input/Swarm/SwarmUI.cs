using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;


public class SwarmUI : MonoBehaviour
{
    public Texture2D[] icons;
    public UIDocument _uiDocument;
    private miniMap _miniMap;
    private VisualTreeAsset _gridIcon;
    private VisualElement _map;
    private VisualElement _mapHolder;

    private VisualElement _cameraPosition;
    private camera cam;
    private Label _selectedUnitsLabel;
    private ScrollView _scrollView;

    private int _hItems = 4;
    private List<gridItem> _items;

    private void Start()
    {
        _miniMap = new miniMap(GetComponent<MapManager>());

    }

    void Awake()
    {
        _items = new List<gridItem>();
        _map = _uiDocument.rootVisualElement.Query("Map");
        _gridIcon = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/GridIcon.uxml");
        _selectedUnitsLabel = (Label)_uiDocument.rootVisualElement.Query("SelectNum");
        _cameraPosition = _uiDocument.rootVisualElement.Q("cameraPosition");
        Debug.Log(_uiDocument.rootVisualElement.Query<Label>("SelectedNum"));
        _scrollView = _uiDocument.rootVisualElement.Query<ScrollView>("GridView");
        

        for(int i = 0; i < icons.Length; i++)
        {
            RenderTexture temp = new RenderTexture(32, 32, 24);
            RenderTexture.active = temp;
            Graphics.Blit(icons[i], temp);
            Texture2D output = new Texture2D(32, 32);
            output.ReadPixels(new Rect(0, 0, 32, 32), 0, 0);
            output.Apply();
            icons[i] = output;
        }


    }
    void test()
    {
        for(int i = 0; i < 100; i++)
        {
            _items.Add(new gridItem("", icons[Random.Range(0, icons.Length)], 10, 10)); 
        }
        UpdateSelectedUnitsLabel();
        CreateUnitGrid();
        ShowMap();
    }

    void UpdateSelectedUnitsLabel()
    {
        _selectedUnitsLabel.text = "Selected Units: " + _items.Count.ToString();
    }
    public void ShowMap()
    {
        if (!_miniMap.generated)
        {
            _miniMap.generate();
           
        }
        _map.Add(_miniMap.getMiniMap());
        //UpdateCameraPosition(new Vector2(_map.resolvedStyle.left, _map.resolvedStyle.top));

    }
    void CreateUnitGrid()
    {
        _scrollView.contentContainer.Clear();

        for (int i = 0; i < _items.Count; i = i + _hItems)
        {
            var row = new VisualElement { style = { flexDirection = FlexDirection.Row, marginLeft = 5, marginRight = 5, marginTop = 5 } };
            row.style.flexGrow = 1;
            for (int j = 0; j < _hItems; j++)
            {
                if ((i + j) < _items.Count)
                {
                    var unitElement = _gridIcon.Instantiate();
                    unitElement.Q<VisualElement>("Icon").style.backgroundImage = (StyleBackground)_items[i + j].spriteImage;
                    unitElement.Q<Label>("Info").text = _items[i + j].health.ToString() + "/" + _items[i + j].maxHealth.ToString();
                    row.Add(unitElement);
                }
            }

            _scrollView.contentContainer.Add(row);
        }
    }
    public void intilizeCam()
    {
        
        _cameraPosition.style.left = 0;
        _cameraPosition.style.top = 0;
        cam = new camera(0,0);
    }
    public void UpdateCameraPosition(Vector2 movement)
    {
        if(cam.init != true)
        {
            intilizeCam();
        }
        cam.updatePos(movement.x,movement.y);
        _cameraPosition.style.top = cam.current_y;
        _cameraPosition.style.left = cam.current_x;
       

    }

    public void UpdateUI(EntityBase[] e)
    {
        _items.Clear();
        foreach (var item in e)
        {
            _items.Add(new gridItem("", icons[item.type], (int)item.health, (int)item.maxHealth));
        }
        UpdateSelectedUnitsLabel();
        CreateUnitGrid();
    }
}
class miniMap
{
    MapManager _mapManager;
    VisualTreeAsset maptile; 
    public VisualElement grid;
    public bool generated;

    public miniMap(MapManager reference)
    {
        _mapManager = reference;
        maptile = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/miniMaptile.uxml");
        grid = new VisualElement { style = { flexDirection = FlexDirection.Row } };
        var temp = _mapManager.getMap();
        if (temp == null) generated = false;
    }
    public void generate()
    {
        var temp = _mapManager.getMap();
        if (temp == null) { generated = false; return; }
        if (temp != null) generated = true; 

        for (int i = 0; i < temp.GetLength(0); i++)
        {
            var column = new VisualElement { style = { flexDirection = FlexDirection.Column } };
            for (int j = 0; j < temp.GetLength(1); j++)
            {
                Color color;
                if (temp[i, j].terrainDif < 100)
                {
                    color = Color.green;
                }
                else
                {
                    color = Color.red;
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
    
    public string type;
    public Texture spriteImage;
    public int health;
    public int maxHealth;

    public gridItem(string type, Texture spriteImage, int health, int maxHealth)
    {
        this.type = type;
        this.spriteImage = spriteImage;
        this.health = health;
        this.maxHealth = maxHealth;
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
        max_x = 220;
        max_y = 140;
        current_x = 220;
        current_y = 140;
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
