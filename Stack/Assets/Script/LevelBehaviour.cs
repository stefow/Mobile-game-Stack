using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;
public class LevelBehaviour : MonoBehaviour
{

    public Transform pivot, cam_pivot, spawner;
    public Transform[] spawnerPoints;
    public GameObject cube, prevCube;

    public Material[] skyboxes;
    public Camera cam;
    public Canvas startCanv, endCanv, mainCanv, settingsCanv;

    //UI
    public Scrollbar sound, music;
    public Text soundPercentage, musicPercentage, scoreDisplay, highScoreDisplay, newScore;

    //Sounds
    public AudioSource endSound, musicSound, blockSound;


    public float camDistance = 0.5f, camSpeed = 1.0f, cubeSpeed = 1f;
    public float clippingDistance = 0.05f;

    float distance;
    bool startposition = true, swap = true, finished = false, start = false;
    int score = 0, highScore = 0;
    int color;

    private void Awake()
    {
        Settings settings = SaveSystem.LoadSettings();
        HighScore hs = SaveSystem.LoadHighScore();
        highScore = hs.getScore();
        highScoreDisplay.text = highScore.ToString();
        sound.value = settings.getSoundLevel();
        music.value = settings.getmusicLevel();
        System.Random r = new System.Random();
        color = r.Next(1, 100);
        prevCube.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((color / 100f) % 1f, 1f, 1f));
        color++;
        cube.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((color / 100f) % 1f, 1f, 1f));
        color++;
        RenderSettings.skybox = skyboxes[r.Next(0, skyboxes.Length)];
        musicSound.volume = music.value;
        blockSound.volume = sound.value;
    }

    void Update()
    {
        SetVolume();
        if (start && !finished)
        {
            scoreDisplay.text = score.ToString();
            MoveObject(cam.transform, cam_pivot, camSpeed);
            SlideCubeBetweenPoints();
            if (Input.GetMouseButtonDown(0))
            {
                StackCube();
                CheackIfGameEnded();
            }

        }
    }
    private void StackCube()
    {
        distance = Vector3.Distance(new Vector3(0, 0, 0), spawner.localPosition);

        if (distance > prevCube.transform.localScale.x && startposition == true ||
            distance > prevCube.transform.localScale.z && startposition == false)
        {
            endSound.Play();
            musicSound.Stop();
            finished = true;
            endCanv.enabled = true;
            cube.AddComponent<Rigidbody>();
        }
        if (distance < clippingDistance && finished == false)
        {
            blockSound.Play();
            score++;
            cube.transform.position = new Vector3(prevCube.transform.position.x, cube.transform.position.y, prevCube.transform.position.z);
            var temp = cube;
            cube.transform.SetParent(null);
            cube = Instantiate(cube, spawner);
            cube.transform.localPosition = new Vector3(0, 0, 0);
            prevCube = temp;
        }
        else if (finished == false)
        {
            SliceCube();
        }
        cube.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.HSVToRGB((color / 100f) % 1f, 1f, 1f));
        color++;
    }
    private void CheackIfGameEnded()
    {
        if (finished == false)
        {
            pivot.position = pivot.position + new Vector3(0, camDistance, 0);
            pivot.position = new Vector3(prevCube.transform.position.x, pivot.position.y, prevCube.transform.position.z);
            if (startposition)
            {
                spawner.position = spawnerPoints[2].position;
                swap = true;
                startposition = false;
            }
            else
            {
                spawner.position = spawnerPoints[0].position;
                swap = true;
                startposition = true;
            }
        }
        if (score > highScore)
        {
            highScoreDisplay.text = score.ToString();
            if (finished)
            {
                SaveSystem.SaveScore(new HighScore(score));
                newScore.enabled = true;
            }
        }
        if (score <= 100) cubeSpeed = cubeSpeed + 0.01f;
    }
    private void SliceCube()
    {
        GameObject leftCube = null, rightCube = null;
        if (spawner.localPosition.x < clippingDistance && startposition == true)
        {
            leftCube = Instantiate(cube, new Vector3(pivot.position.x + (-prevCube.transform.localScale.x - distance) / 2, pivot.localPosition.y, pivot.localPosition.z), Quaternion.identity);
            leftCube.transform.localScale = new Vector3(distance, 1, prevCube.transform.localScale.z);
            rightCube = Instantiate(cube, new Vector3(pivot.position.x + (-distance / 2), pivot.localPosition.y, pivot.localPosition.z), Quaternion.identity);
            rightCube.transform.localScale = new Vector3(prevCube.transform.localScale.x - distance, 1, prevCube.transform.localScale.z);
            prevCube = rightCube;
            leftCube.AddComponent<Rigidbody>();
            Destroy(cube);
            cube = Instantiate(rightCube, spawner);
            cube.transform.localPosition = new Vector3(0, 0, 0);
        }
        if (spawner.localPosition.x > clippingDistance && startposition == true)
        {
            leftCube = Instantiate(cube, new Vector3(pivot.position.x + (distance / 2), pivot.localPosition.y, pivot.localPosition.z), Quaternion.identity);
            leftCube.transform.localScale = new Vector3(prevCube.transform.localScale.x - distance, 1, prevCube.transform.localScale.z);
            rightCube = Instantiate(cube, new Vector3(pivot.position.x + (prevCube.transform.localScale.x + distance) / 2, pivot.localPosition.y, pivot.localPosition.z), Quaternion.identity);
            rightCube.transform.localScale = new Vector3(distance, 1, prevCube.transform.localScale.z);
            rightCube.AddComponent<Rigidbody>();
            prevCube = leftCube;
            Destroy(cube);
            cube = Instantiate(leftCube, spawner);
            cube.transform.localPosition = new Vector3(0, 0, 0);
        }
        if (spawner.localPosition.z < clippingDistance && startposition == false)
        {
            leftCube = Instantiate(cube, new Vector3(pivot.localPosition.x, pivot.localPosition.y, pivot.position.z + (-distance / 2)), Quaternion.identity);
            leftCube.transform.localScale = new Vector3(prevCube.transform.localScale.x, 1, prevCube.transform.localScale.z - distance);
            rightCube = Instantiate(cube, new Vector3(pivot.localPosition.x, pivot.localPosition.y, pivot.position.z + (-prevCube.transform.localScale.z - distance) / 2), Quaternion.identity);
            rightCube.transform.localScale = new Vector3(prevCube.transform.localScale.x, 1, distance);
            rightCube.AddComponent<Rigidbody>();
            prevCube = leftCube;
            Destroy(cube);
            cube = Instantiate(leftCube, spawner);
            cube.transform.localPosition = new Vector3(0, 0, 0);
        }
        if (spawner.localPosition.z > clippingDistance && startposition == false)
        {
            leftCube = Instantiate(cube, new Vector3(pivot.localPosition.x, pivot.localPosition.y, pivot.position.z + (distance + prevCube.transform.localScale.z) / 2), Quaternion.identity);
            leftCube.transform.localScale = new Vector3(prevCube.transform.localScale.x, 1, distance);
            rightCube = Instantiate(cube, new Vector3(pivot.localPosition.x, pivot.localPosition.y, pivot.position.z + (distance / 2)), Quaternion.identity);
            rightCube.transform.localScale = new Vector3(prevCube.transform.localScale.x, 1, prevCube.transform.localScale.z - distance);
            leftCube.AddComponent<Rigidbody>();
            prevCube = rightCube;
            Destroy(cube);
            cube = Instantiate(rightCube, spawner);
            cube.transform.localPosition = new Vector3(0, 0, 0);
        }
        leftCube.name = "leftCube";
        rightCube.name = "rightCUbe";
        cube.name = "OriginalCube";
        blockSound.Play();
        score++;
    }
    private void SlideCubeBetweenPoints()
    {
        if (startposition == true && finished == false)
        {
            if (spawner.position.x > spawnerPoints[1].position.x && swap == true)
            {
                MoveObject(spawner, spawnerPoints[1], cubeSpeed);
            }
            else { swap = false; }
            if (spawner.position.x < spawnerPoints[0].position.x && swap == false)
            {
                MoveObject(spawner, spawnerPoints[0], cubeSpeed);
            }
            else { swap = true; }
        }
        if (startposition == false && finished == false)
        {
            if (spawner.position.z > spawnerPoints[3].position.z && swap == true)
            {
                MoveObject(spawner, spawnerPoints[3], cubeSpeed);
            }
            else { swap = false; }
            if (spawner.position.z < spawnerPoints[2].position.z && swap == false)
            {
                MoveObject(spawner, spawnerPoints[2], cubeSpeed);
            }
            else { swap = true; }
        }
    }
    private void MoveObject(Transform ob, Transform target, float speed)
    {
        var step = speed * Time.deltaTime;
        ob.position = Vector3.MoveTowards(ob.position, target.position, step);
    }

    private void SetVolume()
    {
        if (settingsCanv.enabled)
        {
            soundPercentage.text = ((int)(sound.value * 100)).ToString() + "%";
            musicPercentage.text = ((int)(music.value * 100)).ToString() + "%";
            endSound.volume = sound.value;
            musicSound.volume = music.value;
            blockSound.volume = sound.value;
        }
    }
    public void startStack()
    {
        cube.GetComponent<MeshRenderer>().enabled = true;
        start = true;
        startCanv.enabled = false;
        mainCanv.enabled = true;
    }
    public void chageToScene(int n)
    {
        SceneManager.LoadScene(n);
    }
    public void showCanvas(Canvas can)
    {
        can.enabled = true;
    }
    public void hideCanvas(Canvas can)
    {
        can.enabled = false;
    }
    public void quit()
    {
        Application.Quit();
    }
    public void saveSettings()
    {
        SaveSystem.SaveSettings(new Settings(sound.value, music.value));
    }
}
