using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingObject : MonoBehaviour
{
    //Variable communicates if this is a superimposable mirror image or not. This variable is directly used to check if test subjects answers are correct. 
    public bool SuperimposableMirrorImage;

    //This holds the gameobject model used for visualising this model. When visualising this project, this model will be duplicated in a mirror image. 
    [SerializeField]
    private GameObject _modelPrefab;


    public GameObject Model1, Model2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ProduceObjects()
    {
        GameObject parent = GameObject.Find("MainObjectManager");
        Model1 = Instantiate(_modelPrefab, Vector3.zero, Quaternion.identity);
        Model2 = Instantiate(_modelPrefab, Vector3.zero, Quaternion.identity);
        Model1.transform.position += Vector3.left/2f;
        Model2.transform.position += Vector3.right/2f;
        Model2.transform.localScale = new Vector3(1, 1, -1f);
        Model1.transform.parent = parent.transform;
        Model2.transform.parent = parent.transform;
    }
}
