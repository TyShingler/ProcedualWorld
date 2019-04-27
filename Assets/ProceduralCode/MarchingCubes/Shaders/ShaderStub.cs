using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ChunkPrecomputer;


public class ShaderStub
{

    int Indexing(Vector3 id)
    {
        return (int)(id.x + (18 * id.y) + (18 * 18 * id.z));
    }

    public PrecomputedChunkCollection SamplerSetONE()
    {
        PrecomputedChunkCollection collection = new PrecomputedChunkCollection();
        collection.lerpValues_X = SetONE_X_Z();
        collection.lerpValues_Y = SetONE_Y();
        collection.lerpValues_Z = SetONE_X_Z();
        collection.caseNumbers = SetONE_CASE();
        return collection;
    }

    private float[] SetONE_X_Z()
    {
        float[] r = new float[18*18*18];
        for(int i = 0; i < r.Length; i++)
        {
            r[i] = 1.0f;
        }
        return r;
    }

    private float[] SetONE_Y()
    {
        float[] r = new float[18 * 18 * 18];
        for (int i = 0; i < r.Length; i++)
        {
            if(i < Indexing(new Vector3(0,8,0)))
            {
                r[i] = 1.0f;
            } else if (i > Indexing(new Vector3(0, 9, 0)))
            {
                r[i] = -1.0f;
            }else
            {
                r[i] = 0.5f;
            }
        }
        return r;
    }

    private float[] SetONE_CASE()
    {
        float[] r = new float[18 * 18 * 18];
        for (int i = 0; i < r.Length; i++)
        {
            if (i < Indexing(new Vector3(0, 0, 8)))
            {
                r[i] = 255.0f;
            }
            else if (i > Indexing(new Vector3(0, 0, 9)))

            {
                r[i] = 153.0f;
            }
            else
            {
                r[i] = 0.0f;
            }
        }
        return r;
    }

    // case = 10011001 = 153
}
