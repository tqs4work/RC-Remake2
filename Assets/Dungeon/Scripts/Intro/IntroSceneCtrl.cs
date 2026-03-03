using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroSceneCtrl : MonoBehaviour
{

    [SerializeField] Image blackBg;
    [SerializeField] Image iconMess;
    [SerializeField] GameObject intro;
    [SerializeField] GameObject message1;
    [SerializeField] GameObject message2;
    [SerializeField] GameObject smallTalk1;
    [SerializeField] GameObject outside1;
    [SerializeField] GameObject outside2;
    [SerializeField] GameObject breakroll;
    [SerializeField] GameObject lightningdeco;
    [SerializeField] GameObject lightning1;
    [SerializeField] GameObject lightning2;
    [SerializeField] GameObject explosion;
    [SerializeField] GameObject fire;
    [SerializeField] GameObject portal;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject loadingText;


    bool isImageClicked;


    [Header("Audio")]
    [SerializeField] AudioSource BgMusic;
    [SerializeField] AudioSource BgMusic2;
    [SerializeField] AudioSource RainSound;
    [SerializeField] AudioSource MessageSound;
    [SerializeField] AudioSource ThunderSound;
    [SerializeField] AudioSource LightningSound;
    [SerializeField] AudioSource ExplosionSound;
    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator PlayIntro()
    {
        BgMusic.Play();
        blackBg.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        blackBg.CrossFadeAlpha(0, 12f, false);
        yield return new WaitForSeconds(4f);
        blackBg.CrossFadeAlpha(0, 1f, false);        
        RainSound.Play();
        yield return new WaitForSeconds(5f);
        MessageSound.Play();
        iconMess.gameObject.SetActive(true);
        yield return new WaitUntil(() => isImageClicked);
        iconMess.gameObject.SetActive(false);
        message1.SetActive(true);
        yield return new WaitUntil(() => message2.GetComponent<Dialog>().index > message2.GetComponent<Dialog>().lines.Length - 1
                                    && !message2.activeSelf);
        yield return new WaitForSeconds(2f);
        smallTalk1.SetActive(true);
        yield return new WaitForSeconds(3f); 
        smallTalk1.SetActive(false);
        blackBg.CrossFadeAlpha(1, 5f, false);
        yield return new WaitForSeconds(2f);
        blackBg.CrossFadeAlpha(1, 1f, false);
        yield return new WaitForSeconds(1f);
        outside1.SetActive(true);
        intro.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        blackBg.CrossFadeAlpha(0, 8f, false);
        yield return new WaitForSeconds(3f);
        blackBg.CrossFadeAlpha(0, 1f, false);
        BgMusic2.Play();
        yield return new WaitForSeconds(3f);
        breakroll.SetActive(true);
        lightningdeco.SetActive(true);
        yield return new WaitForSeconds(2f);
        lightning1.SetActive(true);
        //LightningSound.Play();
        ThunderSound.Play();
        yield return new WaitForSeconds(5/6f);
        lightning1.SetActive(false);
        outside2.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        lightning2.SetActive(true);
        LightningSound.Play();
        yield return new WaitForSeconds(1f);
        lightning2.SetActive(false);
        yield return new WaitForSeconds(3f);
        explosion.SetActive(true);
        ExplosionSound.Play();
        yield return new WaitForSeconds(0.5f);
        fire.SetActive(true);
        yield return new WaitForSeconds(4f);
        blackBg.CrossFadeAlpha(1, 12f, false);
        yield return new WaitForSeconds(4f);
        blackBg.CrossFadeAlpha(1, 1f, false);
        yield return new WaitForSeconds(2f);
        RainSound.Stop();
        BgMusic2.Stop();
        loadingText.SetActive(true);
        yield return new WaitUntil(() => loadingText.GetComponent<Dialog>().index > loadingText.GetComponent<Dialog>().lines.Length - 1
                                    && !loadingText.activeSelf);
        portal.SetActive(true);
        loading.SetActive(true);        
        yield return new WaitForSeconds(3f);
        StartCoroutine(NextScene());
    }


    IEnumerator NextScene()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Login");
    }    

    public void ImageClick()
    {
        isImageClicked = true;
    }

}
