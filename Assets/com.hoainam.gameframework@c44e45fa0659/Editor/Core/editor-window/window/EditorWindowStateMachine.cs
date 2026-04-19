using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorWindowStateMachine : EditorWindow
{
    private Stack<EditorWindowState> listStates = new Stack<EditorWindowState>();

    //when unity kill and reopen window, it will serialize and restore
    //serializable variables, use this to determine last state to restore
    protected string lastState;
    
    //unitask rely on ticks to work, in editor window, there is no ticks every update,
    //so we force editor window send ticks if the window do use unitask.
    protected virtual bool windowUseUniTask => false;

    //this is called when:
    //  - open window from menu item
    //  - unity recompile script=>kill all windows=>reopen windows
    protected virtual void OnEnable()
    {
    }

	private void OnDestroy()
	{
        foreach (var i in listStates)
        {
            i.OnWindowClosed();
        }
	}

	void OnGUI()
    {
        if (listStates.Count > 0)
        {
            listStates.Peek().OnDraw();
        }
    }

    private void Update()
    {
        if (listStates.Count > 0)
        {
            var needDraw = listStates.Peek().Update();
            if (needDraw)
            {
                Repaint();
            }
        }

        if (windowUseUniTask)
        {
            EditorApplication.QueuePlayerLoopUpdate();
        }
    }

    public void PushState(EditorWindowState newState)
    {
        lastState = newState.GetType().Name;

        newState.FSM = this;
        newState.OnBeginState();
        listStates.Push(newState);

        Repaint();
    }

    public void PopState()
    {
        if (listStates.Count > 1)
        {
            listStates.Pop();

            lastState = listStates.Peek().GetType().Name;

            Repaint();
        }
        else
        {
            throw new Exception("[FSM] must have at least one state");
        }
    }

    public void SwitchState(EditorWindowState newState)
    {
        if (listStates.Count > 1)
        {
            throw new Exception("[FSM] cannot switch to new state while having multiple state active");
        }
        else
        {
            listStates.Clear();
            PushState(newState);
        }
    }

    public static void OpenWindow<T>(EditorWindowState defaultState) where T : EditorWindowStateMachine
    {
        var window = (T)GetWindow(typeof(T));
        window.titleContent = new GUIContent(typeof(T).Name);
        window.SwitchState(defaultState);
        window.Show();
    }
}
