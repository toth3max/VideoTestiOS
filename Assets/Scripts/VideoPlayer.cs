using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.IO;

public class VideoPlayer : MonoBehaviour
{
    public GameObject downloadButton;
    public GameObject playButton;
    public Text downloadText;
    public Text playText;
    public Slider progressBar;

    public string videoUrl;
    public string videoFilename;
    string localSavePath;

    private bool errorOccured;

    void StartVideo()
    {
        // Will attach a VideoPlayer to the main camera.
        GameObject camera = GameObject.Find("Main Camera");

        // VideoPlayer automatically targets the camera backplane when it is added
        // to a camera object, no need to change videoPlayer.targetCamera.
        var videoPlayer = camera.AddComponent<UnityEngine.Video.VideoPlayer>();

        // Play on awake defaults to true. Set it to false to avoid the url set
        // below to auto-start playback since we're in Start().
        videoPlayer.playOnAwake = false;

        // By default, VideoPlayers added to a camera will use the far plane.
        // Let's target the near plane instead.
        videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;

        // This will cause our Scene to be visible through the video being played.
        videoPlayer.targetCameraAlpha = 0.5F;

        // Set the video to play. URL supports local absolute or relative paths.
        // Here, using absolute.
        videoPlayer.url = localSavePath;

        // Skip the first 100 frames.
        //videoPlayer.frame = 100;

        // Restart from beginning when done.
        videoPlayer.isLooping = true;

        // Each time we reach the end, we slow down the playback by a factor of 10.
        //videoPlayer.loopPointReached += EndReached;

        // Start playback. This means the VideoPlayer may have to prepare (reserve
        // resources, pre-load a few frames, etc.). To better control the delays
        // associated with this preparation one can use videoPlayer.Prepare() along with
        // its prepareCompleted event.
        videoPlayer.Play();
    }

    public void StartVideoPlayback()
    {
        Debug.Log("Starting playing video");
        StartVideo();
    }


    public void StartDownload() {
        Debug.Log("Starting download coroutine");
        StartCoroutine(DownloadIndividualFile());
    }

    IEnumerator DownloadIndividualFile()
    {

        Debug.Log("Starting Download");

        localSavePath = Application.persistentDataPath + "/" + videoFilename;
        Debug.Log("localSavePath: " + localSavePath);

        UnityWebRequest filewww = new UnityWebRequest(videoUrl, UnityWebRequest.kHttpVerbGET);
        
        filewww.downloadHandler = new DownloadHandlerFile(localSavePath);

        filewww.SendWebRequest();

        while (!filewww.isDone)
        {
            progressBar.value = filewww.downloadProgress;
            yield return null;
        }

        if (filewww.isNetworkError || filewww.isHttpError)
        {
            Debug.LogError(filewww.error);
            errorOccured = true;
        }
        else
        {
            Debug.Log("File successfully downloaded and saved to " + localSavePath);
        }

    }
}
