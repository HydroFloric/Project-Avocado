
using System.Collections.Generic;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;
using Label = UnityEngine.UIElements.Label;


public class SwarmUI : MonoBehaviour
{

    public Texture2D[] icons;
    public string[] dmgTypes = { "Magic", "Physical", "Explosive" };
    public UIDocument _uiDocument;


    private GameObject activeCam;
    private GameObject selectedObj;
    private VisualElement _close;
    private VisualElement _camView;
    private Label _id;
    private ListView _attributes;
    private List<string> items;
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
        items = new List<string>();
        activeCam = new GameObject("cam");
        _miniMap = new miniMap(GetComponent<MapManager>());
        _items = new List<gridItem>();
        _map = _uiDocument.rootVisualElement.Query("Map");
        _camView = _uiDocument.rootVisualElement.Q("cameraView");
        _close = _uiDocument.rootVisualElement.Q("close");
        _id = _uiDocument.rootVisualElement.Q<Label>("Identifier");
        _attributes = _uiDocument.rootVisualElement.Q<ListView>("Attributes");
        _infoPanel = _uiDocument.rootVisualElement.Query("InformationPanel");
        _gridIcon = Resources.Load< VisualTreeAsset >("Assets/UI Toolkit/Templates/GridIcon.uxml");
        _selectedUnitsLabel = (Label)_uiDocument.rootVisualElement.Query("SelectNum");
        _cameraPosition = _uiDocument.rootVisualElement.Q("cameraPosition");
        Debug.Log(_uiDocument.rootVisualElement.Query<Label>("SelectedNum"));
        _scrollView = _uiDocument.rootVisualElement.Query<ScrollView>("GridView");

        _infoPanel.visible = false;
        _close.visible = false;
        for (int i = 0; i < icons.Length; i++)
        {
            RenderTexture temp = new RenderTexture(32, 32, 24);
            RenderTexture.active = temp;
            Graphics.Blit(icons[i], temp);
            Texture2D output = new Texture2D(32, 32);
            output.ReadPixels(new Rect(0, 0, 32, 32), 0, 0);
            output.Apply();
            icons[i] = output;
        }

        _close.RegisterCallback<MouseDownEvent>(Hide);

        _attributes.makeItem = () => new Label { style = { color = Color.white, backgroundColor = Color.gray, borderBottomLeftRadius = 5, borderBottomRightRadius = 5, borderTopLeftRadius = 5, borderTopRightRadius = 5, paddingLeft = 5, paddingTop = 5 } };

        _attributes.bindItem = (e, i) => (e as Label).text = items[i];
        _attributes.itemsSource = items;
    }
    private void Update()
    {
        if (_infoPanel.visible)
        {
            UpdateCamView();
            UpdateInfoText();

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
        cam = new camera(0, 0);
    }
    public void UpdateCameraPosition(Vector2 movement)
    {
        if (cam.init != true)
        {
            intilizeMinimapIcon();
        }
        cam.updatePos(movement.x, movement.y);
        _cameraPosition.style.top = cam.current_y;
        _cameraPosition.style.left = cam.current_x;


    }

    public void Hide(MouseDownEvent evt)
    {
        _close.visible = false;
        _infoPanel.visible = false;
        activeCam.SetActive(false);
    }
    public void DisplayInfo(MouseDownEvent evt)
    {
        _infoPanel.visible = true;
        _close.visible = true;
        VisualElement i = (VisualElement)evt.currentTarget;
        EntityBase info = _items[int.Parse(i.name)].reference;
        selectedObj = info.gameObject;

        activeCam.transform.position = info.gameObject.transform.position;
        activeCam.transform.position += Vector3.up;
        activeCam.transform.parent = info.gameObject.transform;
        activeCam.transform.localPosition = new Vector3(0, 2.5f, 7.5f);
        activeCam.transform.localRotation = Quaternion.Euler(90, 0, 0);
        activeCam.AddComponent<Camera>();

        renderCam = new RenderTexture(256, 180, 16);
        activeCam.GetComponent<Camera>().targetTexture = renderCam;

        UpdateCamView();
        UpdateInfoText();
        _id.text = info.gameObject.name;

        activeCam.SetActive(true);
        //now for the fun part!



    }
    void UpdateInfoText()
    {
        var info = selectedObj.GetComponent<EntityBase>();
        items.Clear();
        items.Add("Position: " + "x: " + (int)info.x + " y: " + (int)info.y + " z: " + (int)info.z);
        items.Add("Health: " + info.health + "/" + info.maxHealth);
        items.Add("Dmg Type: " + dmgTypes[info.damageType - 1]);
        //items.Add("Dmg Res: " + dmgTypes[info.damageResist - 1]);
        items.Add("Speed: " + info.speed);
        items.Add("State: " + info.state);
        var pathing = info.toVec3();
        if(info.pathingTo != null)
        {
            pathing = info.pathingTo.Vec3Location();
        }
        items.Add("Pathing to: " + pathing);
        _attributes.RefreshItems();
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
            _items.Add(new gridItem(item.damageType, icons[item.damageType - 1], (int)item.health, (int)item.maxHealth, item));
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
    public bool generated = true;

    public miniMap(MapManager reference)
    {
        _mapManager = reference;
        maptile = Resources.Load<VisualTreeAsset>("Assets/UI Toolkit/Templates/miniMaptile.uxml");
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

        for (int i = temp.GetLength(0)-1; i > 0; i--)
        {
            var column = new VisualElement { style = { flexDirection = FlexDirection.Column } };
            for (int j = temp.GetLength(1)-1; j > 0; j--)
            {
                Color color = Color.grey;
                //DO better //this is gross
                if (temp[i,j].terrainDif == 115)
                {
                    color = Color.white;
                }
                if (temp[i, j].terrainDif == 1)
                {
                    color = Color.green;
                }
                if (temp[i,j].terrainDif == 2)
                {
                    color = Color.yellow;
                }
                if (temp[i, j].terrainDif == 3)
                {
                    color = Color.green;
                }
                if (temp[i,j].terrainDif == 110)
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