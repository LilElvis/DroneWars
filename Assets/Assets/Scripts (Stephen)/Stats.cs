using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    Text text;
    int collisionNum = 0;
    float currentSpeed = 0.0f;
    float averageSpeed = 0.0f;
    List<float> oldSpeeds = new List<float>();
    int numOfSpeeds = 0;
    float topSpeed = 0.0f;
    float windSpeed = 0.0f;
    float inputLag = 0.0f;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
    }



    

    public void addCollision()
    {
        ++collisionNum;
    }

    public void resetCollision()
    {
        collisionNum = 0;
    }

    public void setSpeed(float speed)
    {
        if (speed > topSpeed)
            topSpeed = speed;

        //oldSpeeds.Add(currentSpeed);
        ++numOfSpeeds;
        averageSpeed = (averageSpeed * numOfSpeeds + speed) / (numOfSpeeds + 1);

        currentSpeed = speed;
    }

    public void resetTopSpeed()
    {
        topSpeed = 0.0f;
    }

    public void setWindSpeed(float speed)
    {
        windSpeed = speed;
    }

    public void setInputLag(float time)
    {
        inputLag = time;
    }

    // Update is called once per frame
    void Update ()
    {
        windSpeed += Random.Range(-0.1f, 0.11f);

        setSpeed(Random.Range(0.0f, 1.0f) + Time.time * 0.1f);

        text.text =
            collisionNum + " \n\n" +
            currentSpeed.ToString("00.00") + " Km/h\n\n" +
            averageSpeed.ToString("00.00") + " Km/h\n\n" +
            topSpeed.ToString("00.00") + " Km/h\n\n" +
            windSpeed.ToString("00.00") + " Km/h\n\n" +
            inputLag.ToString("00.00") + " ms";


    }
}
