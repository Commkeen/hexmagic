using UnityEngine.Events;

public class MapObjectEventListener : IEventListener
{
    public MapObjectEvent gameEvent;

    public UnityEvent<MapObject> onEvent;


    public override void OnEvent()
    {

    }

    public void OnEvent(MapObject obj)
    {
        onEvent.Invoke(obj);
    }

    protected override GameEvent GetEvent()
    {
        return gameEvent;
    }
}
