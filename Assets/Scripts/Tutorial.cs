using UnityEngine;
using Utilities.Singletons;
using UnityEngine.UI;
using TMPro;

public class Tutorial : Singleton<Tutorial> {
    public Sprite[] gallery;
    public string[] galleryDescriptions;

    public Image displayImage;
    public TextMeshProUGUI descriptionText;

    public Button nextImg;
    public Button prevImg;
    public TextMeshProUGUI indexText;
    public int i = 0;

    public void BtnNext () {
        i++;
        i = i % gallery.Length;
    }

    public void BtnPrev () {
        i--;
        if (i < 0) i = gallery.Length - 1;
    }

    void Update () {
        displayImage.sprite = gallery[i];
        descriptionText.text = galleryDescriptions[i];
        indexText.text = (i + 1) + "/" + gallery.Length;
    }
}
