using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(Animator))]
public class Transition : MonoBehaviour
{
    public enum LOADTYPE { Scene, UI }

    public static Transition instance;
    Canvas canvas;
    Animator anim;

    object target;
    object current;
    LOADTYPE loadType;


    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        anim = GetComponent<Animator>();

        if(instance != null)
            Destroy(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartTransition(object target = null, object current = null, LOADTYPE loadType = LOADTYPE.Scene)
    {
        if (!anim.GetBool("Run"))
        {
            canvas.enabled = true;
            anim.SetBool("Run", true);
            this.target = target;
            this.loadType = loadType;
            this.current = current;
        }
    }
    public void InitialRun()
    {
        StartCoroutine(Load());
        IEnumerator Load()
        {
            switch (loadType)
            {
                case LOADTYPE.Scene:
                    var async = SceneManager.LoadSceneAsync(target.ToString());
                    while(!async.isDone)
                    {
                        yield return null;
                    }
                    break;
                case LOADTYPE.UI:

                    if (current != null)
                    {
                        var cur = current as GameObject;
                        cur.SetActive(false);
                    }

                    var tar = target as GameObject;
                    tar.SetActive(true);
                    yield return new WaitForSeconds(.2f);
                    break;
            }
            anim.SetBool("Run", false);
            yield return null;
        }
    }

    public void FinishTransition() => canvas.enabled = false;
}
