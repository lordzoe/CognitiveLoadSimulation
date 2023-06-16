using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingObject : MonoBehaviour
{
    //Variable communicates if this is a superimposable mirror image or not. This variable is directly used to check if test subjects answers are correct. 
    public bool Superimposable;

    //Is the model intending to be mirrored or not
    [SerializeField]
    private bool _mirrored;

    //This holds the gameobject primary model used for visualising this model. When visualising this project. 
    [SerializeField]
    private GameObject _modelPrefab1;

    //This holds the gameobject secondary optional model used for visualising this model. When visualising this project. 
    [SerializeField]
    private GameObject _modelPrefab2;

    //This holds the amount of rotation needed to apply i
    [SerializeField]
    private float _rotationToApply;

    //This holds the axis of rotation
    [SerializeField]
    public Vector3 _rotationVec;

    //This holds the rotation needed to apply in a Vec3 format (x,y,z)
    [SerializeField]
    private float _distance = .8f;

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
        if (_modelPrefab2 == null) // if there is only one molecule
        {
            GameObject parent = GameObject.Find("MainObjectManager");
            Model1 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);
            Model2 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);

            //Model1.transform.position += Vector3.left * _distance;
            // Model2.transform.rotation = Quaternion.Euler(_rotationToApply);
            Model2.transform.RotateAround(this.transform.parent.transform.position, _rotationVec, _rotationToApply);
            //Model2.transform.position += Vector3.right * _distance;
            if (_mirrored)
            {
                Model2.transform.localScale = new Vector3(1, 1, -1f);
            }
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;
        }
        else
        {
            GameObject parent = GameObject.Find("MainObjectManager");
            Model1 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);
            Model2 = Instantiate(_modelPrefab2, Vector3.zero, Quaternion.identity);

            //Model1.transform.position += Vector3.left * _distance;

            //Model2.transform.position += Vector3.right * _distance;
            if (_mirrored)
            {
                Model2.transform.localScale = new Vector3(1, 1, -1f);
            }
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;
        }
    }

    public void testRot()
    {
     //   Model2.transform.RotateAround(Model2.transform.position, _rotationVec, 1);
        Debug.Log("hello");
    }
}
