using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Utilities.Singletons;

public class Gameboard : Singleton<Gameboard> {
    public GameObject HexPrefab;

    [Header("Map Settings")]
    public int boardRadius = 12;
    private Dictionary<string, Hex> _hexes;
    private Dictionary<Hex, GameObject> _hexToGameObjectMap;

    private Ray ray;

    #nullable enable
    private Hex? _currentRayCastHitSpot;
    #nullable disable

    private List<Hex> _highlightedHexes;

    public bool showDebugCoordsOnHexes = false;
    private bool _isRaycastRangeValid = true;

    // TODO: We can make these getters / settings
    private GameObject _playerOneDeckArea;
    private GameObject _playerOneGraveArea;
    private GameObject _playerTwoDeckArea;
    private GameObject _playerTwoGraveArea;

    // Start is called before the first frame update
    void Start() {
        Initialize();
    }

    void Update() {
        GetRayCastHit();
        HighlightRaycastHitSpot(_currentRayCastHitSpot, HexColors.VALID_MOVE, 1);
    }

    public void Initialize() {
        BuildMap();

        _highlightedHexes = new List<Hex>();
    }

    public Hex GetRayCastHit() {
        ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitData;
        _isRaycastRangeValid = true; // default to true for frame

        if (Physics.Raycast(ray, out hitData)) {
            if (hitData.collider != null && hitData.collider.transform != null && hitData.collider.transform.parent != null) {
                GameObject hitParent = hitData.collider.transform.parent.gameObject;

                if (hitParent.name.Contains("Hex")) {
                    // Check if target tile is occupied
                    _currentRayCastHitSpot = GetHexByName(hitParent.name);

                    return _currentRayCastHitSpot;
                }
            }
            
        }

        return null;
    }

    private void ClearHighlightedSpaces() {
        HighlightRaycastHitSpot(null, HexColors.DEFAULT_COLOR, 0);
    }

    public void HighlightRaycastHitSpot(Hex target, Color color, int radius = 0) {
        List<Hex> hexesInRange = new List<Hex>();

        List<Hex> hexes = HighlightHexesWithinRange(target, radius, color);
        hexesInRange.AddRange(hexes);

        // Remove all highlights where raycast is not hitting
        // or is highlighted
        if (_highlightedHexes.Count > 0) {
            foreach (Hex h in _highlightedHexes) {
                if (_currentRayCastHitSpot == null) {
                    RemoveHighlightFromHex(h);
                } else if (
                    h != target // not raycast target
                    && (
                        // not highlighted hexes in range
                        (hexesInRange != null && !hexesInRange.Contains(h))
                        || hexesInRange == null
                        ) 
                ) {
                    RemoveHighlightFromHex(h);
                }
            }

            if (_currentRayCastHitSpot == null) {
                _highlightedHexes.RemoveAll(h => true);
            } else {
                _highlightedHexes.RemoveAll(h => (
                    h != target
                    && _currentRayCastHitSpot != h
                    && ((hexesInRange != null && !hexesInRange.Contains(h)) || hexesInRange == null))
                );
            }
        }
    }

    private List<Hex> HighlightHexesWithinRange(Hex center, int range, Color color) {
        List<Hex> hexesInRange = new List<Hex>();
        if (center == null) return hexesInRange;

        for (int q = -range; q <= range; q++) {
            for (int r = -range; r <= range; r++) {
                for (int s = -range; s <= range; s++) {
                    if (q + r + s == 0) {
                        Vector3 loc = Cube.CubeAdd(center, new Vector3(q, r, s));
                        Hex h = GetHexAt((int)loc.x, (int)loc.y);

                        // Breakaway because at least one space is not valid in target ray cast hit
                        if (h == null) _isRaycastRangeValid = false;

                        if (h != null) hexesInRange.Add(h);
                    }
                }
            }
        }

        foreach (Hex hex in hexesInRange) {
            if (_isRaycastRangeValid) HighlightHex(hex, color);
            else HighlightHex(hex, HexColors.INVALID_MOVE);
        }

        return hexesInRange;
    }

    public void HighlightHex(Hex hex, Color color) {
        try {
            GameObject go = _hexToGameObjectMap[hex];
            MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();

            _highlightedHexes.Add(hex);
            mr.materials[1].color = color;
        } catch {
            // noop
        }
        
    }

    private void RemoveHighlightFromHex(Hex hex) {
        if (!_highlightedHexes.Contains(hex)) return;

        GameObject go = _hexToGameObjectMap[hex];
        MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();

        mr.materials[1].color = HexColors.DEFAULT_COLOR;
    }

    private void BuildMap() {
        _hexes = new Dictionary<string, Hex>();
        _hexToGameObjectMap = new Dictionary<Hex, GameObject>();

         for (int q = -boardRadius; q <= boardRadius; q++) {
            int r1 = (int)Mathf.Max(-boardRadius, -q - boardRadius);
            int r2 = (int)Mathf.Min(boardRadius, -q + boardRadius);

            for (int r = r1; r <= r2; r++) {
                Hex h = new Hex(q, r);
                GameObject hexGO = Instantiate(HexPrefab, h.Position(), Quaternion.identity, this.transform) as GameObject;
                MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                mr.materials[1].color = HexColors.DEFAULT_COLOR;

                hexGO.name = "Hex_" + q + "_" + r;
                _hexes.Add(hexGO.name, h);
                if (showDebugCoordsOnHexes) hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", q, r);

                _hexToGameObjectMap[h] = hexGO;
            }
        }

        AddDeckLocations();
    }

    private void AddDeckLocations() {
        Hex hexP1Deck = new Hex(4, boardRadius - 1);
        _playerOneDeckArea = Instantiate(HexPrefab, hexP1Deck.Position(), Quaternion.Euler(0, 30.0f, 0), this.transform) as GameObject;
        _playerOneDeckArea.transform.localScale = new Vector3(2.5f, 1.0f, 2.5f);
        _playerOneDeckArea.name = "Deck_p1";
        MeshRenderer mr = _playerOneDeckArea.GetComponentInChildren<MeshRenderer>();
        mr.materials[1].color = HexColors.DEFAULT_COLOR;

        Hex hexP1Grave = new Hex(8, boardRadius - 5);
        _playerOneGraveArea = Instantiate(HexPrefab, hexP1Grave.Position(), Quaternion.Euler(0, 30.0f, 0), this.transform) as GameObject;
        _playerOneGraveArea.transform.localScale = new Vector3(2.5f, 1.0f, 2.5f);
        _playerOneGraveArea.name = "Grave_p1";
        mr = _playerOneGraveArea.GetComponentInChildren<MeshRenderer>();
        mr.materials[1].color = HexColors.DEFAULT_COLOR;

        Hex hexP2Deck = new Hex(-4, -(boardRadius - 1));
        _playerTwoDeckArea = Instantiate(HexPrefab, hexP2Deck.Position(), Quaternion.Euler(0, 30.0f, 0), this.transform) as GameObject;
        _playerTwoDeckArea.transform.localScale = new Vector3(2.5f, 1.0f, 2.5f);
        mr = _playerTwoDeckArea.GetComponentInChildren<MeshRenderer>();
        mr.materials[1].color = HexColors.DEFAULT_COLOR;

        Hex hexP2Grave = new Hex(-8, -(boardRadius - 5));
        _playerTwoGraveArea = Instantiate(HexPrefab, hexP2Grave.Position(), Quaternion.Euler(0, 30.0f, 0), this.transform) as GameObject;
        _playerTwoGraveArea.transform.localScale = new Vector3(2.5f, 1.0f, 2.5f);
        mr = _playerTwoGraveArea.GetComponentInChildren<MeshRenderer>();
        mr.materials[1].color = HexColors.DEFAULT_COLOR;
    }

    public Vector2Int GetCoordinateByName(string name) {
        string[] subs = name.Split('_');
        return new Vector2Int(int.Parse(subs[1]), int.Parse(subs[2]));
    }

    public Hex GetHexByName(string name) {
        if (_hexes == null) {
            Debug.LogError("Hexes array not yet instantiated.");
            return null;
        }

        try {
            return _hexes[name];
        } catch {
            return null;
        }
    }

    public Hex GetHexAt(Vector2 coord) {
        string name = "Hex_" + coord.x + "_" + coord.y;
        return GetHexByName(name);
    }

    public Hex GetHexAt(int x, int y) {
        string name = "Hex_" + x + "_" + y;
        return GetHexByName(name);
    }

    public bool IsRaycastTargetUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
