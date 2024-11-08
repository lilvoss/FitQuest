using System;
using UnityEngine;
using UnityEngine.UI;

public class legsExercices : MonoBehaviour
{

    [Serializable]
    public struct Exercice
    {
        public string name;
        public string description;
        public Sprite image;

    }
    [SerializeField] Exercice[] allExercices;
    // Start is called before the first frame update
    void Start()
    {
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;

        int N = allExercices.Length;
        for (int i = 0; i < 4; i++)
        {
            g = Instantiate(buttonTemplate, transform);
            g.transform.GetChild(0).GetComponent<Image>().sprite = allExercices[i].image;
            g.transform.GetChild(1).GetComponent<Text>().text = allExercices[i].name;
            g.transform.GetChild(2).GetComponent<Text>().text = allExercices[i].description;

        }
        Destroy(buttonTemplate);


    }


}
