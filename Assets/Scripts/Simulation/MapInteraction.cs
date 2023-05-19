using UnityEngine;

public class MapInteraction : MonoBehaviour
{
    public CoordsEvent emptyHandInteract;
    public InteractModeState interactMode;
    public MapObjectCollection mapObjects;
    public TileCollection tiles;
    public CurrencyState playerCurrency;

    public void OnClick(HexCoordinates coords)
    {
        if (interactMode.isSpellTargeting && interactMode.selectedSpell != null)
        {
            OnClickWithSpell(coords);
            return;
        }

        if (mapObjects.Get(coords).Count > 0)
        {
            OnClickWithMapObjects(coords);
            return;
        }

        OnClickEmptyTile(coords);
    }

    private void OnClickWithSpell(HexCoordinates coords)
    {
        interactMode.selectedSpell.SetTarget(coords);
        interactMode.selectedSpell.CastSpell();
    }

    private void OnClickWithMapObjects(HexCoordinates coords)
    {
        emptyHandInteract.Invoke(coords);
    }

    private void OnClickEmptyTile(HexCoordinates coords)
    {
        // Try to drain mana
        var tile = tiles.Get(coords);

        if (tile.miasma || !tile.explored) {return;}
        if (tile.Life == 0) {return;}

        tile.Life -= 1;
        playerCurrency.AddMana(1);

        var neighbors = coords.GetAllNeighbors();
        foreach (var n in neighbors)
        {
            tile = tiles.Get(n);
            if (tile.Life > 0)
            {
                tile.Life -= 1;
            }
        }
    }
}
