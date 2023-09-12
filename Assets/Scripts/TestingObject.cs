/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingObject : MonoBehaviour
{
    /// <summary>
    ///Variable communicates if this is a superimposable mirror image or not. This variable is directly used to check if test subjects answers are correct.  
    /// </summary>
    public bool Superimposable;

    /// <summary>
    ///This is for the tutorial to check if it needs to be matched to progress to next portion 
    /// </summary>
    public bool ToBeMatched;

    /// <summary>
    ///the matching vectors specify which elements of symmetry the model has, ie NH3 would have 1 matching vector where a chiral compound would have 3 
    /// </summary>
    public Vector3 MatchingVectors;

    /// <summary>
    ///Is the model intending to be mirrored or not 
    /// </summary>
    [SerializeField]
    private bool _mirrored;

    /// <summary>
    ///Which axis the object needed to be mirrored 
    /// </summary>
    [SerializeField]
    private Vector3 _mirroringVector;

    /// <summary>
    ///This holds the gameobject primary model used for visualising this model. When visualising this project.  
    /// </summary>
    [SerializeField]
    private GameObject _modelPrefab1;

    /// <summary>
    ///This holds the gameobject secondary optional model used for visualising this model. When visualising this project. Used mainly for constiutional isomers 
    /// </summary>
    [SerializeField]
    private GameObject _modelPrefab2;

    /// <summary>
    ///This holds the amount of rotation needed to apply to make the two models different visually 
    /// </summary>
    [SerializeField]
    private float _rotationToApply;

    /// <summary>
    ///This holds the axis of rotation
    /// </summary>
    [SerializeField]
    public Vector3 _rotationVec;

    /// <summary>
    ///This holds the distance to space the molecules from eachother
    /// </summary>
    [SerializeField]
    private float _distance = .8f;

    /// <summary>
    /// Holds the models that are often molecules
    /// </summary>
    public GameObject Model1, Model2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Method <c>ProduceObjects</c> Generates the models from the stores models 
    /// </summary>
    public void ProduceModels()
    {
        Debug.Log(this.name);
        if (_modelPrefab2 == null) // if there is only one molecule
        {
            GameObject parent = GameObject.Find("MainObjectManager"); //find the MainObjectManager (MOM)

            //produce two objects and parent them to MOM
            Model1 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);
            Model2 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;

            //Rotate the molecule as defined by the earlier variables
            Model2.transform.RotateAround(Model2.transform.parent.transform.position, _rotationVec, _rotationToApply);

            //Seperate the molecules
            Model1.transform.position += Vector3.left * _distance;
            Model2.transform.position += Vector3.right * _distance;

            if (_mirrored) //if it needs to be mirrored
            {
                Model2.transform.localScale = _mirroringVector; //mirror it along the mirroring vector
            }
        }
        else
        {
            GameObject parent = GameObject.Find("MainObjectManager"); //find the MainObjectManager (MOM)

            //produce two objects and parent them to MOM
            Model1 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);
            Model2 = Instantiate(_modelPrefab2, Vector3.zero, Quaternion.identity);
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;

            //Rotate the molecule as defined by the earlier variables
            Model2.transform.RotateAround(Model2.transform.parent.transform.position, _rotationVec, _rotationToApply);

            //Seperate the molecules
            Model1.transform.position += Vector3.left * _distance;
            Model2.transform.position += Vector3.right * _distance;

            if (_mirrored) //if it needs to be mirrored
            {
                Model2.transform.localScale = _mirroringVector; //mirror it along the mirroring vector
            }
            
        }
        //grab the first part of the molecule name and rename them left and right
        Model1.name = Helper.GetUntilOrEmpty(this.name) + "_Left"; 
        Model2.name = Helper.GetUntilOrEmpty(this.name) + "_Right";
    }
}
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingObject : MonoBehaviour
{
    /// <summary>
    ///Variable communicates if this is a superimposable mirror image or not. This variable is directly used to check if test subjects answers are correct.  
    /// </summary>
    public bool Superimposable;

    /// <summary>
    ///This is for the tutorial to check if it needs to be matched to progress to next portion
    /// </summary>
    public bool ToBeMatched;

    /// <summary>
    ///the matching vectors specify which elements of symmetry the model has, ie NH3 would have 1 matching vector where a chiral compound would have 3
    /// </summary>
    public Vector3 MatchingVectors;

    /// <summary>
    ///Is the model intending to be mirrored or not
    /// </summary>
    [SerializeField]
    private bool _mirrored;

    /// <summary>
    ///Which axis the object needed to be mirrored
    /// </summary>
    [SerializeField]
    private Vector3 _mirroringVector;

    /// <summary>
    ///This holds the gameobject primary model used for visualising this model. When visualising this project.  
    /// </summary>
    [SerializeField]
    private GameObject _modelPrefab1;

    /// <summary>
    ///This holds the gameobject secondary optional model used for visualising this model. When visualising this project. Used mainly for constiutional isomers
    /// </summary>
    [SerializeField]
    private GameObject _modelPrefab2;

    /// <summary>
    ///This holds the amount of rotation needed to apply to make the two models different visually
    /// </summary>
    [SerializeField]
    private float _rotationToApply;

    /// <summary>
    ///This holds the axis of rotation
    /// </summary>
    [SerializeField]
    public Vector3 _rotationVec;

    /// <summary>
    ///This holds the distance to space the molecules from eachother
    /// </summary>
    [SerializeField]
    private float _distance;

    /// <summary>
    /// Camera, for positioning the models, make sure this is the vr cam
    /// </summary>
    [SerializeField]
    private Camera _cam;

    /// <summary>
    /// How much space to make between the camera and the produce models
    /// </summary>
    [SerializeField]
    private float _camSpacing;

    /// <summary>
    /// Holds the models that are often molecules
    /// </summary>
    public GameObject Model1, Model2;


    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    /// <summary>
    /// Method <c>ProduceObjects</c> Generates the models from the stores models
    /// </summary>
    public void ProduceModels()
    {
        _cam = Camera.main;

        Debug.Log(this.name);

        Vector3 camPos = _cam.transform.position;
        Vector3 camLookDir = _cam.transform.forward;
        Vector3 basePos = camPos + camLookDir * _camSpacing;
        Debug.Log(camPos + "  " + camLookDir+"  " + _camSpacing+"  " + basePos);

        GameObject tempParent = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tempParent.name = "TEMP PARENT (IF THIS ISNT DELETED BY END OF SCRIPT YOU HAV PROBLEM)";


        if (_modelPrefab2 == null) // if there is only one molecule
        {
            GameObject parent = GameObject.Find("MainObjectManager"); //find the MainObjectManager (MOM)

            //produce two objects and parent them to MOM
            Model1 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);
            Model2 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);

            //set transform to be that of the parent (MOM)
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;

            //Rotate the molecule as defined by the earlier variables
            Model2.transform.RotateAround(Model2.transform.parent.transform.position, _rotationVec, _rotationToApply);

            //Change parent to that of tempParent for adjustment to camera
            Model1.transform.parent = tempParent.transform;
            Model2.transform.parent = tempParent.transform;

            //adjust position and rotation to camera
            tempParent.transform.position = basePos;
            tempParent.transform.LookAt(basePos * 2 - camPos);

            //Seperate the models from eachother
            Model1.transform.position += tempParent.transform.right * -_distance;
            Model2.transform.position += tempParent.transform.right * _distance;

            //Reparent to correct parent
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;
           

            if (_mirrored) //if it needs to be mirrored
            {
                Model2.transform.localScale = new Vector3(Model2.transform.localScale.x * _mirroringVector.x, Model2.transform.localScale.y * _mirroringVector.y, Model2.transform.localScale.z * _mirroringVector.z) ; //mirror it along the mirroring vector
            }
        }
        else
        {
            GameObject parent = GameObject.Find("MainObjectManager"); //find the MainObjectManager (MOM)

            //produce two objects and parent them to MOM
            Model1 = Instantiate(_modelPrefab1, Vector3.zero, Quaternion.identity);
            Model2 = Instantiate(_modelPrefab2, Vector3.zero, Quaternion.identity);

            //set transform to be that of the parent (MOM)
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;

            //Rotate the molecule as defined by the earlier variables
            Model2.transform.RotateAround(Model2.transform.parent.transform.position, _rotationVec, _rotationToApply);

            //Change parent to that of tempParent for adjustment to camera
            Model1.transform.parent = tempParent.transform;
            Model2.transform.parent = tempParent.transform;

            //adjust position and rotation to camera
            tempParent.transform.position = basePos;
            tempParent.transform.LookAt(basePos * 2 - camPos);

            //Seperate the models from eachother
            Model1.transform.position += tempParent.transform.right * -_distance;
            Model2.transform.position += tempParent.transform.right * _distance;

            //Reparent to correct parent
            Model1.transform.parent = parent.transform;
            Model2.transform.parent = parent.transform;


            if (_mirrored) //if it needs to be mirrored
            {
                Model2.transform.localScale = new Vector3(Model2.transform.localScale.x * _mirroringVector.x, Model2.transform.localScale.y * _mirroringVector.y, Model2.transform.localScale.z * _mirroringVector.z); //mirror it along the mirroring vector
            }

        }
        //grab the first part of the molecule name and rename them left and right
        Model1.name = Helper.GetUntilOrEmpty(this.name) + "_Left";
        Model2.name = Helper.GetUntilOrEmpty(this.name) + "_Right";
        GameObject.Destroy(tempParent);
    }
}
