using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class vehicle
{
    public int node;
    public string name;

    public vehicle(int node, string name)
    {
        this.node = node;
        this.name = name;
    }
}

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused;
    private GameObject[] vehiclesGameObjects;

    //public vehicle Vehicle;
    public CarController RR;
    public GameObject startPosition;
    public Text currentPosition;
    public Slider nitrusSlider;
    public GameObject[] fullArray;


    [Header("racers list")]
    public GameObject uiList;
    public GameObject uiListFolder;
    public List<vehicle> presentVehicles;
    public List<GameObject> temporaryList;
    private GameObject[] temporaryArray;

    private int startPositionXvalue = -50 - 62;
    private bool arrarDisplayed = false, countdownFlag = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        uiListFolder = GameObject.Find("uiListFolder"); // Ensure "uiListFolder" matches the exact name in your Hierarchy

    }

    void Start()
    {
        RR = GameObject.FindGameObjectWithTag("Player").GetComponent<CarController>();

        vehiclesGameObjects = GameObject.FindGameObjectsWithTag("Enemy");

        presentVehicles = new List<vehicle>();
        foreach (GameObject R in vehiclesGameObjects)
            presentVehicles.Add(new vehicle(R.GetComponent<CarController>().currentNode, R.GetComponent<CarController>().carName));

        presentVehicles.Add(new vehicle(RR.gameObject.GetComponent<CarController>().currentNode, RR.carName));

        temporaryArray = new GameObject[presentVehicles.Count];

        temporaryList = new List<GameObject>();
        foreach (GameObject R in vehiclesGameObjects)
            temporaryList.Add(R);
        temporaryList.Add(RR.gameObject);

        fullArray = temporaryList.ToArray();
        displayArray();
        Debug.Log("Starting timedLoop...");
        StartCoroutine(timedLoop());
        pauseMenu.SetActive(false);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

    }
    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }
    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }
    public void Restart()
    {
        SceneManager.LoadScene(1);
    }
    public void MainMeniu()
    {
        SceneManager.LoadSceneAsync(0);
    }


    private void sortArray()
    {

        for (int i = 0; i < fullArray.Length; i++)
        {
            presentVehicles[i].name = fullArray[i].GetComponent<CarController>().carName;
            presentVehicles[i].node = fullArray[i].GetComponent<CarController>().currentNode;
        }

        for (int i = 0; i < presentVehicles.Count; i++)
        {
            for (int j = i + 1; j < presentVehicles.Count; j++)
            {
                if (presentVehicles[j].node < presentVehicles[i].node)
                {
                    vehicle QQ = presentVehicles[i];
                    presentVehicles[i] = presentVehicles[j];
                    presentVehicles[j] = QQ;
                }
            }
        }



        for (int i = 0; i < temporaryArray.Length; i++)
        {
            temporaryArray[i].transform.Find("VehicleNodeText").gameObject.GetComponent<Text>().text = presentVehicles[i].node.ToString();
            temporaryArray[i].transform.Find("VehicleNameText").gameObject.GetComponent<Text>().text = presentVehicles[i].name.ToString();
            if (RR.carName == presentVehicles[i].name)
                currentPosition.text = ((i + 1) + "/" + presentVehicles.Count).ToString();
        }
        presentVehicles.Reverse();
        for (int i = 0; i < temporaryArray.Length; i++)
        {
            if (RR.carName == presentVehicles[i].name)
                currentPosition.text = ((i + 1) + "/" + presentVehicles.Count).ToString();
        }



    }

    private void displayArray()
    {

        for (int i = 0; i < vehiclesGameObjects.Length + 1; i++)
        {
            generateList(i, presentVehicles[i].node, presentVehicles[i].name);
        }

        startPositionXvalue = -50;
    }

    private void generateList(int index, int num, string nameValue)
    {

        temporaryArray[index] = Instantiate(uiList);
        temporaryArray[index].transform.parent = uiListFolder.transform;
        //temporaryArray[index].gameObject.GetComponent<RectTransform>().localScale = new Vector3(2,2,2);
        temporaryArray[index].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, startPositionXvalue);
        //temporaryArray[index].transform.position = new Vector3(0,startPositionXvalue,0);
        temporaryArray[index].transform.Find("vehicle name").gameObject.GetComponent<Text>().text = nameValue.ToString();
        temporaryArray[index].transform.Find("vehicle node").gameObject.GetComponent<Text>().text = num.ToString();
        startPositionXvalue += 50;

    }

    private IEnumerator timedLoop()
    {
        Debug.Log("Starting timedLoop INSIDE...");
        while (true)
        {
            yield return new WaitForSeconds(.7f);
            sortArray();
            Debug.Log("sort");
        }
    }
}
