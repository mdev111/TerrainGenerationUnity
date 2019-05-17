using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corrosion : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        
    }
    public ParticleSystem part;
    public List<ParticleCollisionEvent> collisionEvents;

    void Start()
    {
        part = GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
    }

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = part.GetCollisionEvents(other, collisionEvents);

        Mesh mesh = other.GetComponent<Mesh>();
        int i = 0;
        Debug.Log("collision");
        while (i < numCollisionEvents)
        {
            if (mesh)
            {
                Vector3 pos = collisionEvents[i].intersection;
                Vector3 force = collisionEvents[i].velocity * 10;
                CorrosionMapGenerator.ReevaluateMap(20f);
                //EndlessTerrain.upda
                //MapGenerator.dr
                //mesh.uv[0].AddForce(force);
                //MapGenerator.mes
                //mesh.uv[ = mesh.uv * 0.1;
            }
            i++;
        }
    }
}
