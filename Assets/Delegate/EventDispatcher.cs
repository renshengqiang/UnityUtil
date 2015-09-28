using UnityEngine;
using System.Collections;

public delegate void VoidHandler();
public class EventDispatcher : MonoBehaviour{
    public static EventDispatcher dispatcher = null;
    public void Awake()
    {
        dispatcher = this;
    }

    public void RequestForSomething(VoidHandler completeHandler)
    {
        StartCoroutine(CompleteThread(completeHandler));
    }

    IEnumerator CompleteThread(VoidHandler handler)
    {
        yield return new WaitForSeconds(1f);
        
        if (null != handler && null != handler.Target)
        {
            Debug.Log("handler.Target: " + handler.Target.ToString());
            object target = handler.Target;
            MonoBehaviour mono = handler.Target as MonoBehaviour;
            Debug.Log("handler.Target as mono: " + mono);
            if (typeof(Object).IsAssignableFrom(target.GetType()))
            {
                if (mono != null && null != mono.gameObject)
                {
                    handler();
                }
            }
            else
            {
                handler();
            }
        }
    }
}
