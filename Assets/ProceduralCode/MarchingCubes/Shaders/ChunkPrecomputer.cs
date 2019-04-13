using UnityEngine;

public class ChunkPrecomputer : MonoBehaviour
{
    public int xOffsets = 0;
    public int yOffsets = 0;
    public int zOffsets = 0;

    public ComputeShader lerpShader;
    public RenderTexture lerpField_texture;
    public Material lerpField_material;

    public ComputeShader caseShader;
    public RenderTexture caseNumber_texture;
    public Material caseNumber_material;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        PrecomputeChunk();
    }

    private PrecomputedChunkCollection PrecomputeChunk()
    {
        ///---Shader setup---
        //--Kernals
        int lerpShaderHandler = lerpShader.FindKernel("PrecomputeChunkLerp");
        int caseShaderHandler = caseShader.FindKernel("PrecomputeChunkCase");
        //--Textures
        //-Lerp

        lerpField_texture = CreateTexture(18, 18 * 18);
        lerpShader.SetTexture(lerpShaderHandler, "LerpField_Texture", lerpField_texture);
        //-Case
        caseNumber_texture = CreateTexture(16, 16 * 16);
        caseShader.SetTexture(caseShaderHandler, "CaseNumber_Texture", caseNumber_texture);

        //--Float Arrays
        //-Lerp
        BufferPair XField = SetupComputeBuffer(lerpShaderHandler, lerpShader, "LerpFloats_X", 18*18*18);
        BufferPair YField = SetupComputeBuffer(lerpShaderHandler, lerpShader, "LerpFloats_Y", 18*18*18);
        BufferPair ZField = SetupComputeBuffer(lerpShaderHandler, lerpShader, "LerpFloats_Z", 18*18*18);
        //-Case
        BufferPair caseNumbers = SetupComputeBuffer(caseShaderHandler, caseShader, "CaseNumber_Floats", 16 * 16 * 16);

        //--Setting Offsets
        SetupShaderOffsets(lerpShader);
        SetupShaderOffsets(caseShader);

        //--Dispatch
        lerpShader.Dispatch(lerpShaderHandler, 18 / 6, 18 / 6, 18 / 6);
        caseShader.Dispatch(lerpShaderHandler, 16 / 8, 16 / 8, 16 / 8);

        //--Returning Textures
        lerpField_material.mainTexture = lerpField_texture;
        caseNumber_material.mainTexture = caseNumber_texture;

        //--Fetch data for method return
        FetchArrayData(XField);
        FetchArrayData(YField);
        FetchArrayData(ZField);
        FetchArrayData(caseNumbers);
        PrecomputedChunkCollection collection = new PrecomputedChunkCollection();
        collection.lerpValues_X = XField.array;
        collection.lerpValues_Y = YField.array;
        collection.lerpValues_Z = ZField.array;
        collection.caseNumbers = caseNumbers.array;

        return collection;
    }

    public struct PrecomputedChunkCollection
    {
        public float[] lerpValues_X;
        public float[] lerpValues_Y;
        public float[] lerpValues_Z;
        public float[] caseNumbers;
    }

    private static float[] FetchArrayData(BufferPair pair)
    {
        pair.buffer.GetData(pair.array);
        pair.buffer.Dispose();
        return pair.array;
    }

    private RenderTexture CreateTexture(int width, int height )
    {
        RenderTexture new_texture = new RenderTexture(width, height, 24);
        new_texture.enableRandomWrite = true;
        new_texture.Create();
        return new_texture;
    }

    struct BufferPair
    {
        public float[] array;
        public ComputeBuffer buffer;
    }
    private BufferPair SetupComputeBuffer(int shaderHandler, ComputeShader shader, string arrayName, int arraySize)
    {
        BufferPair result = new BufferPair();
        result.array = new float[arraySize];
        result.buffer = new ComputeBuffer(result.array.Length, sizeof(float));
        shader.SetBuffer(shaderHandler, arrayName, result.buffer);
        return result;
    }

    private void SetupShaderOffsets(ComputeShader shader)
    {
        shader.SetInt("xOffset", xOffsets);
        shader.SetInt("yOffset", yOffsets);
        shader.SetInt("zOffset", zOffsets);
    }
}
