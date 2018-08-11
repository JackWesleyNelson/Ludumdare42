using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public PlanetGenerator Instance { get; private set; }
    public int Iterations { get; private set; } = 20;
    public int PlanetChildCount = 6;

    [SerializeField]
    private InventoryItemsDistributor Distributor;
    [SerializeField]
    private LayerMask planetLayerMask;
        
    private float minRadiusFromPlanets = 1.2f;
    private float maxRadiusFromLastPlanet;
    private float maxRadiusFromLastPlanetDefault = 4.2f;

    private float minSizeMult = .2f;
    private float maxSizeMult= .3f;

    private int maxValidationAttempts = 200;
    private int validationAttempts;

    private Planet originPlanet;

    private GameObject planetHierarchy;
    private Dictionary<Planet, GameObject> planetDict;

    private bool isSpawning = false;

    public void Start() {
        if (!Instance) {
            Instance = this;
        }
        if(this != Instance) {
            Destroy(this.gameObject);
        }
        planetDict = new Dictionary<Planet, GameObject>();
        validationAttempts = maxValidationAttempts;
        maxRadiusFromLastPlanet = maxRadiusFromLastPlanetDefault;

        StartCoroutine(RespawnOnTimer());

    }

    IEnumerator RespawnOnTimer() {
        while (true) {
            RespawnPlanets();
            yield return new WaitForSeconds(5f);
        }
    }

    private void RespawnPlanets() {
        if (isSpawning) {
            StopAllCoroutines();
        }
        maxRadiusFromLastPlanet = maxRadiusFromLastPlanetDefault;
        //Destroy whatever old planets existed.
        DestroyImmediate(planetHierarchy);
        planetDict.Clear();
        //Create a hierarchy for the planets to reside.
        planetHierarchy = new GameObject("Planets");
        //Make a starting planet at the origin.
        originPlanet = new Planet(new Vector2(0, 0), true, Distributor);
        //Generate a sphere of a random size, within constraints, at the origin. Name the planet, set it's layer and add to the dict.
        GameObject newPlanet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        newPlanet.name = planetDict.Count.ToString();
        newPlanet.transform.position = originPlanet.Position;
        newPlanet.transform.parent = planetHierarchy.transform;
        newPlanet.transform.localScale *= Random.Range(minSizeMult, maxSizeMult);
        newPlanet.layer = LayerMask.NameToLayer("Planet");
        planetDict.Add(originPlanet, newPlanet);
        //Using the origin as a parent, begin spawning new planets.
        StartCoroutine(SpawnPlanets(originPlanet, Iterations));
    }

    IEnumerator SpawnPlanets(Planet parent, int iterations) {
        isSpawning = true;
        yield return null;
        Queue<Planet> spawnerQueue = new Queue<Planet>();
        spawnerQueue.Enqueue(parent);
        do {
            for(int i = 0; i < PlanetChildCount; i++) {
                while(validationAttempts > 0) {
                    float radius = Random.Range(minRadiusFromPlanets + .0001f, maxRadiusFromLastPlanet);
                    float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
                    float x = Mathf.Cos(angle) * radius;
                    float y = Mathf.Sin(angle) * radius;

                    Vector2 pos = new Vector2(spawnerQueue.Peek().Position.x + x, spawnerQueue.Peek().Position.y + y);

                    if(Physics2D.OverlapCircle(pos, radius, planetLayerMask) == null) {
                        
                        Planet p = new Planet(pos, false, Distributor);
                        GameObject newPlanet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        DestroyImmediate(newPlanet.GetComponent<SphereCollider>());
                        newPlanet.AddComponent<CircleCollider2D>();
                        newPlanet.name = planetDict.Count.ToString();
                        newPlanet.transform.position = pos;
                        newPlanet.transform.parent = planetHierarchy.transform;
                        newPlanet.transform.localScale *= Random.Range(minSizeMult, maxSizeMult);
                        newPlanet.layer = LayerMask.NameToLayer("Planet");
                        planetDict.Add(p, newPlanet);
                        yield return new WaitForFixedUpdate();
                        spawnerQueue.Enqueue(p);
                        break;
                    }
                    else {
                        Debug.Log("Hit planet");
                    }
                }
                validationAttempts = maxValidationAttempts;
            }
            if(spawnerQueue.Count > 0) {
                spawnerQueue.Dequeue();
            }
            iterations--;
        }
        while (iterations > 0);
        isSpawning = false;
    }

    
    private void SpawnPlanetsRecursive(Planet parent, int iterations) {
        //We're deep enough into our iteration max, we can safely return.
        if(iterations <= 0) {
            return;
        }
        //Increase the max possible radius by a small amount
        maxRadiusFromLastPlanet *= 1.0125f;
        //We haven't hit our iteration cap, so we can try to generate each child planet.
        for (int i = 0; i < PlanetChildCount; i++) {
            //Attempt to generate a valid planet for this iteration attemps number of times before giving up.
            while (validationAttempts > 0) {
                //Generate a location along the circumfrence of a circle origin parent.pos, radius randomly generated between min and max exclusive.
                float radius = Random.Range(minRadiusFromPlanets+(maxSizeMult*2)+.0001f, maxRadiusFromLastPlanet);
                float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                Vector2 pos = new Vector2(parent.Position.x + x, parent.Position.y + y);
                //Validate that the planet is at least min distance from all other planets, continue to next attempt or fail out.
                if (Physics2D.OverlapCircle(pos, radius, planetLayerMask) == null) {
                    //We have vaildated, create a planet from the position.
                    Planet p = new Planet(pos, false, Distributor);
                    //Generate a planet with a random size, within constraints, give it a name and add to dict.
                    GameObject newPlanet = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    newPlanet.name = planetDict.Count.ToString();
                    newPlanet.transform.position = pos;
                    newPlanet.transform.parent = planetHierarchy.transform;
                    newPlanet.transform.localScale *= Random.Range(minSizeMult, maxSizeMult);
                    newPlanet.layer = LayerMask.NameToLayer("Planet");
                    planetDict.Add(p, newPlanet);
                    //Generate planets surrounding the validated planet.
                    SpawnPlanets(p, iterations - 1);
                    //Break from validation attempt loop.
                    break;
                }
                validationAttempts--;
            }
            validationAttempts = maxValidationAttempts;
        }
    }


}
