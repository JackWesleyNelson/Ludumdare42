using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour {
    public PlanetGenerator Instance { get; private set; }
    public int Iterations { get; private set; } = 5;
    public int PlanetChildCount = 3;

    [SerializeField]
    private InventoryItemsDistributor Distributor;
    [SerializeField]
    private LayerMask planetLayerMask;
        
    private float minRadiusFromPlanets = .8f;
    private float maxRadiusFromLastPlanet;
    private float maxRadiusFromLastPlanetDefault = 1.5f;

    private float minSizeMult = .2f;
    private float maxSizeMult= .3f;

    private int maxValidationAttempts = 200;
    private int validationAttempts;

    private Planet originPlanet;

    private GameObject planetHierarchy;
    private Dictionary<Planet, GameObject> planetDict;

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
            yield return new WaitForSeconds(2.5f);
        }
    }

    private void RespawnPlanets() {
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
        SpawnPlanets(originPlanet, Iterations);
    }

    private void SpawnPlanets(Planet parent, int iterations) {
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
                float radius = Random.Range(minRadiusFromPlanets+.0001f, maxRadiusFromLastPlanet);
                float angle = Random.Range(0f, 1f) * Mathf.PI * 2;
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;

                Vector2 pos = new Vector2(parent.Position.x + x, parent.Position.y + y);
                //Validate that the planet is at least min distance from all other planets, continue to next attempt or fail out.
                Collider[] colliders = Physics.OverlapSphere(pos, radius, planetLayerMask);

                if (colliders.Length <= 1) {
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
