using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LevelBlueprintManager : MonoBehaviour
{
    public float minTimeBetweenBuildSpawns = 1f;
    public float maxTimeBetweenBuildSpawns = 5f;
    public int maxActiveBuildSites = 3;

    public string nextSceneName;

    public UnityEvent onLevelEnd;

    public GameObject buildSitePrefab;
    
    private List<BuildableBlueprint> buildables;
    private List<BuildSite> activeBuildSites;
    void Start()
    {
        buildables = new List<BuildableBlueprint>(transform.GetComponentsInChildren<BuildableBlueprint>());
        activeBuildSites = new List<BuildSite>();

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
    }

    public void StartLevel()
    {
        StartCoroutine(SpawnBuildables());
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

    private IEnumerator SpawnBuildables()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minTimeBetweenBuildSpawns, maxTimeBetweenBuildSpawns));

            activeBuildSites.RemoveAll(buildSite => buildSite == null);

            var unbuilt = buildables.FindAll(buildable =>
                !buildable.gameObject.activeInHierarchy);

            if (unbuilt.Count == 0 && activeBuildSites.Count == 0)
            {
                yield return new WaitForSeconds(1f);
                onLevelEnd.Invoke();
            }

            if (activeBuildSites.Count >= maxActiveBuildSites)
                continue;

            var validBuildables = unbuilt.FindAll(buildable =>
                !activeBuildSites.Find(buildSite => buildSite.objToBuild == buildable)
                && buildable.prerequisites.All(prereq => prereq.gameObject.activeInHierarchy));

            if (validBuildables.Count == 0)
                continue;

            var objToBuild = validBuildables[Random.Range(0, validBuildables.Count)];

            Debug.Log($"spawning build site for {objToBuild.gameObject} [{objToBuild.transform.position}]");

            var buildSite = Instantiate(buildSitePrefab, objToBuild.transform.position, objToBuild.transform.rotation)
                .GetComponent<BuildSite>();
            
            buildSite.objToBuild = objToBuild;
            
            activeBuildSites.Add(buildSite);
        }
    }
}
