using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Delete planets that are not in the 2 by 2 grid space.
//Spawn tiled copies around current world space. When you are more than 1 world space away from a copy destroy the copy and instantiate another in the opposite direction.


public class PlanetGenerator : MonoBehaviour {
    public PlanetGenerator Instance { get; private set; }
    public int Iterations { get; private set; } = 6;
    public int PlanetChildCount = 7;

    [SerializeField]
    private InventoryItemsDistributor Distributor;
    [SerializeField]
    private LayerMask planetLayerMask;
    [SerializeField]
    private List<Material> planetMaterials;
    [SerializeField]
    private Transform ship;

    private float minRadiusFromPlanets;
    private float minRadiusFromPlanetsDefault = 1.15f;
    private float maxRadiusFromLastPlanet;
    private float maxRadiusFromLastPlanetDefault = 1.8f;

    private float minSizeMult = .225f;
    private float maxSizeMult= .4f;

    private float planetChildGrowthRate;
    private float planetChildGrowthRateDefault = 1.5f;
    private float planetChildGrowthRateMax = 5f;
    private float planetDistanceGrowthRate = 1.5f;

    public float WorldWidthRadius { get; private set; } = 10.0f;
    public float WorldHeightRadius{ get; private set; } = 10.0f;

    private int maxValidationAttempts = 4;
    private int validationAttempts;

    private Planet originPlanet;

    private List<GameObject> planetHierarchy;

    private List<Planet> planets;
    private Dictionary<Planet, GameObject> planetDict;

    public bool IsSpawning { get; private set; } = false;

    

    public void Start() {
        if (!Instance) {
            Instance = this;
        }
        if(this != Instance) {
            Destroy(this.gameObject);
        }
        planets = new List<Planet>();
        planetDict = new Dictionary<Planet, GameObject>();
        planetHierarchy = new List<GameObject>();
        validationAttempts = maxValidationAttempts;
        minRadiusFromPlanets = minRadiusFromPlanetsDefault;
        maxRadiusFromLastPlanet = maxRadiusFromLastPlanetDefault;
        planetChildGrowthRate = planetChildGrowthRateDefault;

        RespawnPlanets();

    }

    public void Update() {
        if (!IsSpawning) {
            ShiftHierarchyTiles(ship.transform.position);
        }
    }

    private void RespawnPlanets() {
        if (IsSpawning) {
            StopAllCoroutines();
        }
        minRadiusFromPlanets = minRadiusFromPlanetsDefault;
        maxRadiusFromLastPlanet = maxRadiusFromLastPlanetDefault;
        planetChildGrowthRate = planetChildGrowthRateDefault;

        //Destroy whatever old planets existed.
        planets.Clear();
        planetDict.Clear();
        planetHierarchy.Clear();
        //Create a hierarchy for the planets to reside.
        planetHierarchy.Add(new GameObject("Planets"));
        //Make a starting planet at the origin.
        originPlanet = CreatePlanet(new Vector2(0, 0), true);
        //Using the origin as a parent, begin spawning new planets.
        StartCoroutine(SpawnPlanets(originPlanet, Iterations));
    }

    IEnumerator SpawnPlanets(Planet parent, int iterations) {
        IsSpawning = true;
        int childrenToSpawn = PlanetChildCount;
        float xBounds = WorldWidthRadius - maxSizeMult;
        float yBounds = WorldHeightRadius - maxSizeMult;


        do {
            for (int i = 0; i < childrenToSpawn; i++) {
                if(validationAttempts > 0) {
                    float radius = Random.Range(minRadiusFromPlanets, maxRadiusFromLastPlanet);
                    float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
                    float x = Mathf.Cos(angle) * radius;
                    float y = Mathf.Sin(angle) * radius;
                    Vector2 pos = new Vector2(parent.Position.x + x, parent.Position.y + y);

                    //Attempt overlap detection to validate that planet is far enough from any other planet.
                    if(Physics2D.OverlapCircle(pos, minRadiusFromPlanetsDefault, planetLayerMask) == null && !(pos.x >= xBounds || pos.x <= -xBounds|| pos.y >= yBounds|| pos.y <= -yBounds)) {
                        CreatePlanet(pos);
                        validationAttempts = 0;
                    }
                    else {
                        validationAttempts--;
                    }
                    yield return null;
                }
                validationAttempts = maxValidationAttempts;
            }
            planetChildGrowthRate = Mathf.Lerp(planetChildGrowthRateDefault, planetChildGrowthRateMax, (float)(Iterations - iterations) / (float)(Iterations));
            childrenToSpawn = (int)(childrenToSpawn * planetChildGrowthRate);
            minRadiusFromPlanets *= planetDistanceGrowthRate;
            maxRadiusFromLastPlanet *= planetDistanceGrowthRate;
            iterations--;
        } while (iterations > 0);
        CreateHierarchyTiles();
        IsSpawning = false;
    }

    private Planet CreatePlanet(Vector2 pos, bool discovered = false) {
        Planet p = new Planet(pos, false, Distributor);
        GameObject newPlanet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        DestroyImmediate(newPlanet.GetComponent<SphereCollider>());
        newPlanet.AddComponent<CircleCollider2D>();
        newPlanet.name = planetDict.Count.ToString();
        newPlanet.transform.position = pos;
        newPlanet.transform.parent = planetHierarchy[0].transform;
        newPlanet.transform.localScale *= Random.Range(minSizeMult, maxSizeMult);
        newPlanet.layer = LayerMask.NameToLayer("Planet");
        newPlanet.GetComponent<MeshRenderer>().sharedMaterials = new Material[] { planetMaterials[Random.Range(0, planetMaterials.Count)] };
        planetDict.Add(p, newPlanet);
        planets.Add(p);
        return p;
    }

    private void CreateHierarchyTiles() {
        List<Vector2> directions = new List<Vector2>(8) {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(1, 0),
            new Vector2(0, -1),
            new Vector2(-1, 0),
            new Vector2(-1, -1),
            new Vector2(-1, 1),
            new Vector2(1, -1)
        };

        foreach (Vector2 dir in directions) {
            GameObject g = Instantiate(planetHierarchy[0]);
            g.name += dir.ToString();
            g.transform.position += new Vector3(dir.x * WorldWidthRadius * 2, dir.y * WorldHeightRadius * 2);
            planetHierarchy.Add(g);
        }
    }

    private void ShiftHierarchyTiles(Vector2 shipPosition) {
        float centerX = planetHierarchy[0].transform.position.x;
        float centerY = planetHierarchy[0].transform.position.y;
        float xDistance = Mathf.Abs(shipPosition.x - centerX);
        float yDistance = Mathf.Abs(shipPosition.y - centerY);

        foreach (GameObject g in planetHierarchy) {
            Vector3 newPos = g.transform.position;
            if (xDistance > 20) {
                if (shipPosition.x > centerX) {
                    newPos.x += 20;
                }
                else {
                    newPos.x -= 20;
                }
            }
            if (yDistance > 20) {
                if(shipPosition.y > centerY) {
                    newPos.y += 20;
                }
                else {
                    newPos.y -= 20;
                }
            }
            g.transform.position = newPos;
        }
    }

    public Planet GetPlanet(int index) {
        return planets[index];
    }

}
