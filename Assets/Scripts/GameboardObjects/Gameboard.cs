using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Gameboard : MonoBehaviour {
    public GameObject HexPrefab;

    [Header("Map Settings")]
    public int boardRadius = 12;
    private Dictionary<string, Hex> hexes;
    private Dictionary<Hex, GameObject> hexToGameObjectMap;

    private Ray ray;

    #nullable enable
    private Hex? selectedHex;
    #nullable disable

    private List<Hex> highlightedHexes;

    public bool ShowDebugCoordsOnHexes = false;

    bool isRaycastValidMovement = true;

    // Start is called before the first frame update
    void Start() {
        Initialize();
    }

    void Update() {
        GetRayCastHit();
    }

    public void Initialize() {
        BuildMap();

        highlightedHexes = new List<Hex>();
    }

    public void GetRayCastHit() {
        ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitData;
        isRaycastValidMovement = true; // default to true for frame

        if (Physics.Raycast(ray, out hitData)) {
            // GameObject hitParent = hitData.collider.transform.parent.gameObject;

            // if (hitParent.name.Contains("Hex")) {
            //     // Check if target tile is occupied
            //     Hex hex = GetHexByName(hitParent.name);
            //     if (hex == null) return;

            //     // if (selectedHex == null) {
            //     //     if (hex.GetGameboardObject() is Creature) HandleCreatureMovement(hex);
            //     //     else if (hex.GetGameboardObject() is Permanent) HandlePermanentMovement(hex);
            //     // } else {
            //     //     if (selectedHex.GetGameboardObject() is Creature) HandleCreatureMovement(hex);
            //     //     else if (selectedHex.GetGameboardObject() is Permanent) HandlePermanentMovement(hex);
            //     // }
            // }
        }
    }

    // private void HandleCreatureMovement(Hex target) {
    //     int distanceToHex = Cube.GetDistanceToHex(selectedHex, target);
    //     bool isTargetWithinRangeMovement = selectedHex != null ? ((Creature)selectedHex.GetGameboardObject()).attributes.speed >= distanceToHex : true;
    //     bool isTargetWithinRangeAttack = selectedHex != null ? ((Creature)selectedHex.GetGameboardObject()).attributes.range >= distanceToHex : false;

    //     HighlightRaycastHitSpot(target, isTargetWithinRangeMovement, 0);

    //     // Toggle selection of occupied gameboard piece
    //     if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON) && !IsRaycastTargetUI()) {
    //         // We're already targeting, so select the target game object
    //         if (selectedHex != null) {
    //             if (isTargetWithinRangeMovement) {
    //                 // if we're already targetting but we're now wanting to target a new creature
    //                 if (target.IsOccupied()) {
    //                     if (this.selectedHex != target && isTargetWithinRangeAttack) {
    //                         gboManager.Attack(selectedHex.GetGameboardObject(), target.GetGameboardObject());
    //                         this.selectedHex = null;
    //                         ClearHighlightedSpaces();
    //                     }
    //                 // else we're targeting an empty cell to move the currently selected creature to
    //                 } else {
    //                     gboManager.Move(selectedHex.GetGameboardObject(), target);
    //                     ClearHighlightedSpaces();
    //                 }

    //                 this.selectedHex = null;
    //                 gboManager.DeselectAll();
    //             } else {
    //                 // If attempting to click on a spot that's not in range of currently selected creature, then deselect
    //                 this.selectedHex = null;
    //                 gboManager.DeselectAll();
    //                 ClearHighlightedSpaces();
    //             }
                
    //         } else if (target.IsOccupied()) {
    //             if (gboManager.Select(target.GetGameboardObject())) this.selectedHex = target;
    //         }
    //     }
    // }

    private void ClearHighlightedSpaces() {
        HighlightRaycastHitSpot(null, false, 0);
    }

    // private void HandlePermanentMovement(Hex target) {
    //     int radius = 0;
    //     bool isWithinRange = true;

    //     if (selectedHex != null && selectedHex.GetGameboardObject() != null) {
    //         Permanent p = selectedHex.GetGameboardObject() as Permanent;
    //         radius = p.GetRadius();
            
    //         // Clamp castle on each players side
    //         if (p.GetHex().R < 0 && target.R >= 0 || p.GetHex().R > 0 && target.R <= 0) isWithinRange = false;
    //     }

    //     HighlightRaycastHitSpot(target, isWithinRange, radius);

    //     if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON) && !IsRaycastTargetUI()) {
    //         if (selectedHex != null) {
    //             // Move it
    //             if ((!target.IsOccupied() || target.GetGameboardObject() == selectedHex.GetGameboardObject()) && isWithinRange && isRaycastValidMovement) {
    //                 // we can move there
    //                 gboManager.Move(selectedHex.GetGameboardObject(), target);
    //                 selectedHex = null;
    //                 gboManager.DeselectAll();
    //             } else {
    //                 this.selectedHex = null;
    //                 gboManager.DeselectAll();
    //                 ClearHighlightedSpaces();
    //             }
    //         } else {
    //             // Select it
    //             if (target.IsOccupied()) {
    //                 if (gboManager.Select(target.GetGameboardObject())) selectedHex = target;
    //             }
    //         }
    //     }
    // }

    public void HighlightRaycastHitSpot(Hex target, bool isTargetWithinRange, int radius = 0) {
        // List<Hex> hexesInRange = new List<Hex>();

        // // Show preview of spaces for movement if available
        // if (selectedHex != null && selectedHex.GetGameboardObject() is Creature) {
        //     Creature selectedCreature = selectedHex.GetGameboardObject() as Creature;
        //     if (selectedCreature.HasMovementActionAvailable())
        //         hexesInRange.AddRange(HighlightHexesWithinRange(selectedHex, selectedCreature.attributes.speed, HexColors.AVAILABLE_MOVES));
        // }

        // // Highlight a tile where the cursor is while a creature is targeted to move
        // if (selectedHex != null && target != null && (selectedHex != target || selectedHex.GetGameboardObject() is Permanent)) {
        //     Color color = target.IsOccupied() || !isTargetWithinRange ? HexColors.INVALID_MOVE : HexColors.VALID_MOVE;
        //     if (selectedHex.GetGameboardObject() is Creature) {
        //         // TODO: This should be probably turned RED

        //         Creature creature = selectedHex.GetGameboardObject() as Creature;
        //         if (!creature.HasMovementActionAvailable() && !target.IsOccupied()) color = HexColors.INVALID_MOVE;
        //     }

        //     if (selectedHex.GetGameboardObject() is Permanent && selectedHex.GetGameboardObject() == target.GetGameboardObject())
        //         color = HexColors.VALID_MOVE;

        //     List<Hex> hexes = HighlightHexesWithinRange(target, radius, color);
            
        //     hexesInRange.AddRange(hexes);
        // }

        // // Remove all highlights where raycast is not hitting
        // // or is highlighted
        // if (highlightedHexes.Count > 0) {
        //     foreach (Hex h in highlightedHexes) {
        //         if (selectedHex == null) {
        //             RemoveHighlightFromHex(h);
        //         } else if (
        //             h != target // not raycast target
        //             && (
        //                 // not highlighted hexes in range
        //                 (hexesInRange != null && !hexesInRange.Contains(h))
        //                 || hexesInRange == null
        //                 ) 
        //         ) {
        //             RemoveHighlightFromHex(h);
        //         }
        //     }

        //     if (selectedHex == null) {
        //         highlightedHexes.RemoveAll(h => true);
        //     } else {
        //         highlightedHexes.RemoveAll(h => (
        //             h != target
        //             && selectedHex != h
        //             && ((hexesInRange != null && !hexesInRange.Contains(h)) || hexesInRange == null))
        //         );
        //     }
        // }
    }

    private List<Hex> HighlightHexesWithinRange(Hex center, int range, Color color) {
        List<Hex> hexesInRange = new List<Hex>();

        // for (int q = -range; q <= range; q++) {
        //     for (int r = -range; r <= range; r++) {
        //         for (int s = -range; s <= range; s++) {
        //             if (q + r + s == 0) {
        //                 Vector3 loc = Cube.CubeAdd(center, new Vector3(q, r, s));
        //                 Hex h = GetHexAt((int)loc.x, (int)loc.y);

        //                 // Breakaway because at least one space is not valid in target ray cast hit
        //                 if (center != selectedHex) {
        //                     if (h == null || (h.IsOccupied() && h.GetGameboardObject() != selectedHex.GetGameboardObject()))
        //                         isRaycastValidMovement = false;
        //                 }

        //                 if (h != null) hexesInRange.Add(h);
        //             }
        //         }
        //     }
        // }

        // foreach (Hex hex in hexesInRange) {
        //     if (isRaycastValidMovement && (!hex.IsOccupied() || hex.GetGameboardObject() == selectedHex.GetGameboardObject())) HighlightHex(hex, color);
        //     else if (isRaycastValidMovement && hex.GetGameboardObject() == null && selectedHex.GetGameboardObject() is Permanent) HighlightHex(hex, color);
        //     else HighlightHex(hex, HexColors.INVALID_MOVE);
        // }

        return hexesInRange;
    }

    public void HighlightHex(Hex hex, Color color) {
        try {
            GameObject go = hexToGameObjectMap[hex];
            MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();

            highlightedHexes.Add(hex);
            mr.material.SetColor("_Color", color);
        } catch {
            // noop
        }
        
    }

    private void RemoveHighlightFromHex(Hex hex) {
        if (!highlightedHexes.Contains(hex)) return;

        GameObject go = hexToGameObjectMap[hex];
        MeshRenderer mr = go.GetComponentInChildren<MeshRenderer>();

        mr.material.SetColor("_Color", HexColors.DEFAULT_COLOR);
    }

    private void BuildMap() {
        hexes = new Dictionary<string, Hex>();
        hexToGameObjectMap = new Dictionary<Hex, GameObject>();

         for (int q = -boardRadius; q <= boardRadius; q++) {
            int r1 = (int)Mathf.Max(-boardRadius, -q - boardRadius);
            int r2 = (int)Mathf.Min(boardRadius, -q + boardRadius);

            for (int r = r1; r <= r2; r++) {
                Hex h = new Hex(q, r);
                GameObject hexGO = Instantiate(HexPrefab, h.Position(), Quaternion.identity, this.transform) as GameObject;

                hexGO.name = "Hex_" + q + "_" + r;
                hexes.Add(hexGO.name, h);
                if (ShowDebugCoordsOnHexes) hexGO.GetComponentInChildren<TextMesh>().text = string.Format("{0},{1}", q, r);

                // MeshRenderer mr = hexGO.GetComponentInChildren<MeshRenderer>();
                // mr.material = MatGrasslands;

                hexToGameObjectMap[h] = hexGO;
            }
        }
    }

    public Vector2Int GetCoordinateByName(string name) {
        string[] subs = name.Split('_');
        return new Vector2Int(int.Parse(subs[1]), int.Parse(subs[2]));
    }

    public Hex GetHexByName(string name) {
        if (hexes == null) {
            Debug.LogError("Hexes array not yet instantiated.");
            return null;
        }

        try {
            return hexes[name];
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
    public Hex ReturnHexofRayCastHit()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitData;
        isRaycastValidMovement = true; // default to true for frame

        if (Physics.Raycast(ray, out hitData))
        {
            GameObject hitParent = hitData.collider.transform.parent.gameObject;

            if (hitParent.name.Contains("Hex"))
            {
                // Check if target tile is occupied
                return GetHexByName(hitParent.name);
            }
        }
        return null;
    }

    public bool IsRaycastTargetUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
