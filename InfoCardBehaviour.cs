using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoCardBehaviour : MonoBehaviour
{
    [Header("Information Cards")]
    [SerializeField] private List<GameObject> InfoCardList;

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator ShowInfoCard(string cardName)
    {
        foreach (GameObject go in InfoCardList)
        {
            if (go.name.Substring(0, 2).ToLower().Equals(cardName.Substring(0, 2).ToLower()))
            {
                go.SetActive(true);
            }
        }

        yield return null;
    }
}
