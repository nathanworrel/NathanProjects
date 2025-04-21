using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasShield : MonoBehaviour
{
    public Material mat3;
    public Material mat2;
    public Material mat1;
    MeshRenderer mesh;

    private void Start() {
        mesh = GetComponent<MeshRenderer>();
        mesh.enabled = false;
    }

    public void SetState(int i){
        if(i == 3){
            mesh.material = mat3;
        }else if(i == 2){
            mesh.material = mat2;
        }else{
            mesh.material = mat1;
        }
    }

    public void Render(bool state){
        mesh.enabled = state;
    }
}
