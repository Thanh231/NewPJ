public abstract class EditorWindowState
{
    public EditorWindowStateMachine FSM;
    public virtual void OnBeginState() { }
    public abstract void OnDraw();
    public virtual bool Update()
    {
        return false;
    }
    public virtual void OnWindowClosed() { }
}
