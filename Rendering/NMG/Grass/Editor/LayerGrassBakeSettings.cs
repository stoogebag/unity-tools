// MIT License

// Copyright (c) 2021 NedMakesGames

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files(the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions :

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LayerGrassBakeSettings", menuName = "NedMakesGames/LayerGrassBakeSettings")]
public class LayerGrassBakeSettings : ScriptableObject {
    [Tooltip("The source mesh to build off of")]
    public Mesh sourceMesh;
    [Tooltip("The submesh index of the source mesh to use")]
    public int sourceSubMeshIndex;
    [Tooltip("A scale to apply to the source mesh before generating ")]
    public Vector3 scale;
    [Tooltip("A rotation to apply to the source mesh before generating . Euler angles, in degrees")]
    public Vector3 rotation;
    [Tooltip("The number of grass layers")]
    public int numGrassLayers;

    [Tooltip("grass height")] 
    public float grassHeight;
}
