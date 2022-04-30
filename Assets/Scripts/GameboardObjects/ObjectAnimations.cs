using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Singletons;
using TMPro;
using Cinemachine;

public class ObjectAnimations : Singleton<ObjectAnimations>
{
    private CinemachineVirtualCamera playerCamera;
    //damage shake
    private float animationTimer;
    Vector3 oldPosition;
    private bool shaking;
    private float duration = 1;
    private GameboardObject shookObject;

    //damage text
    public GameObject damageText;
    private bool[] textActive;
    private GameObject[] damageGO;
    private Vector3 targetPos;
    private float[] textTimer;
    private float damageTextCount;
    // Update is called once per frame
    private void Awake()
    {
        textActive = new bool[10];
        damageGO = new GameObject[10];
        textTimer = new float[10];

    }
    void Update()
    {
        for( int i=0; i<10; i++)
        {
            textTimer[i] += Time.deltaTime / 1.5f;
        }
        animationTimer += Time.deltaTime / duration;
        ObjectIsShaking();
        ManageDamageText();
    }
    public void ObjectIsShaking()
    {
        if (shookObject == null) shaking = false;
        if (shaking)
        {
            Vector3 newPos = Random.insideUnitSphere * (Time.deltaTime * 20);
            newPos.y = shookObject.transform.position.y;
            newPos.z = shookObject.transform.position.z;
            newPos.x += shookObject.transform.position.x;
            shookObject.transform.position = newPos;

            if (animationTimer > duration)
            {
                shaking = false;
                shookObject.transform.position = oldPosition;
            }
        }
    }
    public void ShakeMe( GameboardObject x)
    {
        if (x != null)
        {
            x._defaultDeferredAttackTimerAmount = duration;
            shookObject = x;
            animationTimer = 0;
            oldPosition = shookObject.transform.position;
            shaking = true;
        }
    }
    public void SpawnDamageText(Transform transformPosition, float damageDone)
    {
        damageTextCount++;
        Vector3 position = transformPosition.position;
        for (int x = 0; x < 10; x++)
        {
            if (!textActive[x] && damageTextCount>0)
            {
                damageTextCount--;
                damageGO[x] = Instantiate(damageText, position, Quaternion.identity);
                playerCamera = CameraController.Instance._virtualCamera;
                damageGO[x].transform.LookAt(playerCamera.transform);
                damageGO[x].transform.rotation = Quaternion.LookRotation(playerCamera.transform.forward);

                damageGO[x].GetComponent<TextMeshPro>().SetText(damageDone.ToString());
                targetPos = damageGO[x].transform.position + new Vector3(0, 3f);
                textActive[x] = true;
                textTimer[x] = 0;
            }

        }

    }
    public void ManageDamageText()
    {
        for (int x = 0; x < 10; x++)
        {
            if (textActive[x])
            {
                damageGO[x].transform.position += new Vector3(0, 1.5f) * Time.deltaTime;
                if (damageGO[x].transform.position.y > targetPos.y || textTimer[x] > 1.5f)
                {
                    textActive[x] = false;
                    Destroy(damageGO[x]);
                }
            }
        }
    }
}
