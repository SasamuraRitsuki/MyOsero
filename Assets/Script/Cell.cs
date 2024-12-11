using UnityEngine;
using UnityEngine.UI;

public enum CellState
{
    None = 0,
    Black = 1,
    White = 2
    //Choiced = 3
}
public class Cell : MonoBehaviour
{
    [SerializeField]
    private Image _view = null;

    [SerializeField]
    private CellState _cellState = CellState.None;
    public CellState CellState
    {
        get => _cellState;
        set
        {
            _cellState = value;
            OnCellStateChanged();
        }
    }

    private void OnValidate()
    {
        OnCellStateChanged();
    }

    private void OnCellStateChanged()
    {
        if (_view == null) { return; }

        if (_cellState == CellState.None)
        {
            _view.color = Color.clear;

        }
        if (_cellState == CellState.Black)
        {
            _view.color = Color.black;

        }
        if (_cellState == CellState.White)
        {
            _view.color = Color.white;

        }
        //else if (_cellState == CellState.Choiced)
        //{
        //    _view.color = Color.yellow;
        //}
    }
}