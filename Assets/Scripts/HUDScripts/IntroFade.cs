using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroFade : MonoBehaviour
{
    public RawImage the3FacesLogo;
    public RawImage kidneyGamesLogo;
    public float fadeDuration = 1f;
    public float firstImageDuration = 2f;

    private bool isFading = false;

    private void Start()
    {
        StartCoroutine(PlayIntro());
    }

    private IEnumerator PlayIntro()
    {
        the3FacesLogo.gameObject.SetActive(true);
        kidneyGamesLogo.gameObject.SetActive(true);

        Color firstColor = the3FacesLogo.color;
        Color secondColor = kidneyGamesLogo.color;

        firstColor.a = 1f;
        secondColor.a = 0f;
        the3FacesLogo.color = firstColor;
        kidneyGamesLogo.color = secondColor;

        // Mantener la primera imagen visible un tiempo
        yield return new WaitForSeconds(firstImageDuration);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            firstColor.a = Mathf.Lerp(1f, 0f, t);
            the3FacesLogo.color = firstColor;

            secondColor.a = Mathf.Lerp(0f, 1f, t);
            kidneyGamesLogo.color = secondColor;

            yield return null;
        }
        firstColor.a = 0f;
        secondColor.a = 1f;

        the3FacesLogo.color = firstColor;
        kidneyGamesLogo.color = secondColor;

        the3FacesLogo.gameObject.SetActive(false);
    }
}
