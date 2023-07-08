using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelBlueprintManager : MonoBehaviour
{
    public float minTimeBetweenBuildSpawns = 1f;
    public float maxTimeBetweenBuildSpawns = 5f;
    public int maxActiveBuildSites = 3;

    public GameObject buildSitePrefab;
    
    private List<BuildableBlueprint> buildables;
    private List<BuildSite> buildSites;
    void Start()
    {
        buildables = new List<BuildableBlueprint>(transform.GetComponentsInChildren<BuildableBlueprint>());

        for (int i = 0; i < buildables.Count;)
        {
            if (buildables[i].startBuilt)
            {
                buildables.RemoveAt(i);
                continue;
            }
            
            buildables[i].gameObject.SetActive(false);
            
            i++;
        }

        StartCoroutine(SpawnBuildables());
    }

    private IEnumerator SpawnBuildables()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenBuildSpawns, maxTimeBetweenBuildSpawns));

            var validBuildables = buildables.FindAll(buildable =>
                buildable.prerequisites.All(prereq => prereq.gameObject.activeInHierarchy));

            var objToBuild = validBuildables[Random.Range(0, validBuildables.Count)];

            var buildSite = Instantiate(buildSitePrefab, objToBuild.transform.position, objToBuild.transform.rotation);

            buildSite.GetComponent<BuildSite>().objToBuild = objToBuild;
            
            
        }
    }
}
