using UnityEngine;

public class CurrentBiome : MonoBehaviour
{
    public static string[] CurrentsBioms;

    private float Delay = 1;


    private void Update()
    {
        Delay -= Time.deltaTime;

        if(Delay <= 0)
            VarCurrentBiome();
    }
    private void Start()
    {
        VarCurrentBiome();
    }

    private void VarCurrentBiome()
    {
        Delay = 1;

        CurrentsBioms = new string[4];
        int i = 0;


        Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f);

        foreach (var col in cols)
        {
            if(col != null)
            {
                string LayerName = LayerMask.LayerToName(col.gameObject.layer);

                if(LayerName.StartsWith("Biome"))
                {
                    CurrentsBioms[i] = LayerName;
                    i++;
                }
            }
        }
    }
}
