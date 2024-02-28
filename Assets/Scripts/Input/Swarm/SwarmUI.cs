using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;


public class SwarmUI : MonoBehaviour
{

    public Texture2D[] icons;
    public UIDocument _uiDocument;

    
    private GameObject activeCam;
    private VisualElement _camView;
    private Label _id;
    private GroupBox _attributes;
    private RenderTexture renderCam;


    private miniMap _miniMap;
    private VisualTreeAsset _gridIcon;
    private VisualElement _infoPanel;
    private VisualElement _map;
    private VisualElement _mapHolder;

    private VisualElement _cameraPosition;
    private camera cam;
    private Label _selectedUnitsLabel;
    private ScrollView _scrollView;

    private int _hItems = 4;
    private List<gridItem> _items;


    void Awake()
    {
        activeCam = new GameObject("cam");
        _miniMap = new miniMap(GetComponent<MapManager>());
        _items = new List<gridItem>();
        _map = _uiDocument.rootVisualElement.Query("Map");
        _camView = _uiDocument.rootVisualElement.Q("cameraView");
        _id = _uiDocument.rootVisualElement.Q<Label>("Identifier");
        _attributes = _uiDocument.rootVisualElement.Q<GroupBox>("Attributes");
        _infoPanel = _uiDocument.rootVisualElement.Query("InformationPanel");
        _gridIcon = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI Toolkit/Templates/GridIcon.uxml");
        _selectedUnitsLabel = (Label)_uiDocument.rootVisualElement.Query("SelectNum");
        _cameraPosition = _uiDocument.rootVisualElement.Q("cameraPosition");
        Debug.Log(_uiDocument.rootVisualElement.Query<Label>("SelectedNum"));
        _scrollView = _uiDocument.rootVisualElement.Query<ScrollView>("GridView");
        
        _infoPanel.visible = false;
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
    private void Update()
    {
        if (activeCam != null)
        {
            UpdateCamView();
        }
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
                    unitElement.name = (i + j).ToString();
                    unitElement.RegisterCallback<MouseDownEvent>(DisplayInfo);
                    unitElement.style.backgroundColor = _items[i + j].color;
                    unitElement.Q<VisualElement>("Icon").style.backgroundImage = (StyleBackground)_items[i + j].spriteImage;
                    unitElement.Q<Label>("Info").text = _items[i + j].health.ToString() + "/" + _items[i + j].maxHealth.ToString();
                    row.Add(unitElement);
                }
            }

            _scrollView.contentContainer.Add(row);
        }
    }
    public void intilizeMinimapIcon()
    {
        
        _cameraPosition.style.left = 0;
        _cameraPosition.style.top = 0;
        cam = new camera(0,0);
    }
    public void UpdateCameraPosition(Vector2 movement)
    {
        if(cam.init != true)
        {
            intilizeMinimapIcon();
        }
        cam.updatePos(movement.x,movement.y);
        _cameraPosition.style.top = cam.current_y;
        _cameraPosition.style.left = cam.current_x;
       

    }
    public void DisplayInfo(MouseDownEvent evt)
    {

        VisualElement i = (VisualElement)evt.currentTarget;
        EntityBase info = _items[int.Parse(i.name)].reference;
         
        
        var selectedObject = info.gameObject;

        _id.text = info.gameObject.name;

        _attributes.Clear();
        _attributes.Add(new Label { text =  ("Position: " + "x: " + (int)info.x + " y: " + (int)info.y + " z: " + (int)info.z), style = { color = Color.white, backgroundColor = Color.gray} }) ;
        _attributes.Add(new Label { text = ("Health: " + info.health + "/" + info.maxHealth), style = { color = Color.white, backgroundColor = Color.gray } });
        _attributes.Add(new Label { text = ("Speed: " + info.speed), style = { color = Color.white , backgroundColor = Color.gray } });
        _attributes.Add(new Label { text = ("State: " + info.state), style = { color = Color.white , backgroundColor = Color.gray } });
        _attributes.Add(new Label { text = ("Pathing to: " + info.pathingTo.Vec3Location()), style = { color = Color.white , backgroundColor = Color.gray } });

        //now for the fun part!
        activeCam.transform.parent = info.gameObject.transform;
        activeCam.transform.localPosition = new Vector3(0, 1, 5);
        activeCam.transform.localRotation = Quaternion.Euler(90, 0, 0);
        activeCam.AddComponent<Camera>();

        renderCam = new RenderTexture(256, 180, 16);
        activeCam.GetComponent<Camera>().targetTexture = renderCam;
        UpdateCamView();

        _infoPanel.visible = true;
       
    }
    void UpdateCamView()
    {
        RenderTexture.active = renderCam;
        Texture2D output = new Texture2D(253, 180);
        output.ReadPixels(new Rect(0, 0, 253, 180), 0, 0);
        output.Apply();
        _camView.style.backgroundImage = output;
    }

    public void UpdateUI(EntityBase[] e)
    {
        _items.Clear();
        foreach (var item in e)
        {
            _items.Add(new gridItem(item.type, icons[item.type], (int)item.health, (int)item.maxHealth, item));
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
                    color = Color.grey;
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
        if(type == 0)
        {
            color = Color.green;
        }
        if (type == 1)
        {
            color = Color.blue;
        }
        if (type == 2)
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
