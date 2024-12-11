using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardCell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private int _rows = 8;

    [SerializeField]
    private int _columns = 8;

    [SerializeField]
    private GridLayoutGroup _gridLayoutGroup = null;

    [SerializeField]
    private Cell _cellPrefab = null;

    //�\������e�L�X�g
    public TextMeshProUGUI turnText;
    public TextMeshProUGUI ScoreText;

    private int _selectedRow = 0;
    private int _selectedColumn = 0;

    private Cell[,] cells;

    private int _flipCount = 0;

    private int _turnCount = 1;

    private bool _blackturn = true;

    private bool _turned = false;

    private int _blackScore = 0;
    private int _whiteScore = 0;


    private CellState MyCellState = CellState.Black;
    private CellState EnemyCellState = CellState.White;

    void Start()
    {
        var parent = _gridLayoutGroup.gameObject.transform;
        cells = new Cell[_rows, _columns];
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                var cell = Instantiate(_cellPrefab);
                cell.transform.SetParent(parent);
                cells[r, c] = cell;

                if (r == 3 && c == 3) cell.CellState = CellState.Black;
                if (r == 4 && c == 4) cell.CellState = CellState.Black;
                if (r == 3 && c == 4) cell.CellState = CellState.White;
                if (r == 4 && c == 3) cell.CellState = CellState.White;
                //if (r == 5 && c == 4) cell.CellState = CellState.White;
                //if (r == 6 && c == 4) cell.CellState = CellState.White;
                //if (r == 5 && c == 5) cell.CellState = CellState.Black;
                //if (r == 4 && c == 5) cell.CellState = CellState.Black;
            }
        }

        CellScoreCount();
    }

    private void Update()
    {
        if (_blackturn)
        {
            turnText.color = Color.black;
            turnText.text = "Black Turn";

        }
        else
        {
            turnText.color = Color.white;
            turnText.text = "White Turn";
        }
        ScoreText.text = $"{_blackScore}  -  {_whiteScore}";
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        MyCellState = CellState.Black;
        EnemyCellState = CellState.White;
        if (!_blackturn)
        {
            MyCellState = CellState.White;
            EnemyCellState = CellState.Black;
        }

        var obj = eventData.pointerCurrentRaycast.gameObject;
        var par = obj.transform.parent.gameObject;
        var image = par.GetComponent<Cell>();

        bool click_obj = false;
        //�N���b�N�����Z�����ǂ��̍��W���T��
        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                if (image == cells[r, c])
                {
                    click_obj = true;
                    _selectedRow = r;
                    _selectedColumn = c;
                }
            }
        }

        //���������ɉ��������āA�Q�[����ʓ����^�b�`������
        if (cells[_selectedRow, _selectedColumn].CellState == CellState.None && click_obj)
        {
            //Debug.Log($"_selectedRow = {_selectedRow}");
            //Debug.Log($"_selectedColumn = {_selectedColumn}");

            _flipCount = 0;


            //1 = �^��
            if (TryCellCheck(_selectedRow + 1, _selectedColumn))
            {
                _flipCount = 0;
                if (TryCanPutCell(1, _selectedRow + 1, _selectedColumn))
                {
                    CellFlip(1);
                }
            }
            //2 = �^��
            if (TryCellCheck(_selectedRow - 1, _selectedColumn))
            {
                _flipCount = 0;
                if (TryCanPutCell(2, _selectedRow - 1, _selectedColumn))
                {
                    CellFlip(2);
                }
            }
            //3 = �^�E
            if (TryCellCheck(_selectedRow , _selectedColumn + 1))
            {
                _flipCount = 0;
                //Debug.Log($"�܉E�ɓ��钼�O count {_flipCount}");
                if (TryCanPutCell(3, _selectedRow, _selectedColumn + 1))
                {
                    CellFlip(3);
                }
            }
            //4 = �^�E
            if (TryCellCheck(_selectedRow, _selectedColumn - 1))
            {
                _flipCount = 0;
                if (TryCanPutCell(4, _selectedRow, _selectedColumn - 1))
                {
                    CellFlip(4);
                }
            }
            //5 = �E��
            if (TryCellCheck(_selectedRow + 1, _selectedColumn + 1))
            {
                _flipCount = 0;
                if (TryCanPutCell(5, _selectedRow + 1, _selectedColumn + 1))
                {
                    CellFlip(5);
                }
            }
            //6 = �E��
            if (TryCellCheck(_selectedRow - 1, _selectedColumn + 1))
            {
                _flipCount = 0;
                if (TryCanPutCell(6, _selectedRow - 1, _selectedColumn + 1))
                {
                    CellFlip(6);
                }
            }
            //7 = ����
            if (TryCellCheck(_selectedRow + 1, _selectedColumn - 1))
            {
                _flipCount = 0;
                if (TryCanPutCell(7, _selectedRow + 1, _selectedColumn - 1))
                {
                    CellFlip(7);
                }
            }
            //8 = ����
            if (TryCellCheck(_selectedRow - 1, _selectedColumn - 1))
            {
                _flipCount = 0;
                //Debug.Log($"�e�X�g�@{_flipCount}");
                if (TryCanPutCell(8, _selectedRow - 1, _selectedColumn - 1))
                {
                    //Debug.Log("������߂���");
                    CellFlip(8);
                }
                //Debug.Log($"�e�X�g��@{_flipCount}");

            }

            if (_turned)
            {
                cells[_selectedRow,_selectedColumn].CellState = MyCellState;
                _turnCount++;
                _blackturn = _blackturn == true ? false : true;
                _turned = false;
                CellScoreCount();
            }
        }

    }

    void CellScoreCount()
    {

        _blackScore = 0;
        _whiteScore = 0;

        for (var r = 0; r < _rows; r++)
        {
            for (var c = 0; c < _columns; c++)
            {
                if (cells[r, c].CellState == CellState.Black)
                {
                    _blackScore++;
                }
                if (cells[r, c].CellState == CellState.White)
                {
                    _whiteScore++;
                }
            }
        }
    }
    bool TryCellCheck(int row, int column)
    {
        //�w�肳�ꂽ�ꏊ�ɃZ�������邩�ǂ����`�F�b�N
        if (row > _rows - 1 || row < 0 || column > _columns - 1 || column < 0)
        {
            return false;
        }
        else return true;
    }

    void CellFlip(int pattern)
    {
        switch (pattern)
        {
            case 1:
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow + i, _selectedColumn].CellState =
                        cells[_selectedRow + i, _selectedColumn].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;

            case 2:
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow - i, _selectedColumn].CellState =
                        cells[_selectedRow - i, _selectedColumn].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;

            case 3:
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow, _selectedColumn + i].CellState =
                        cells[_selectedRow, _selectedColumn + i].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;
            case 4:
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow, _selectedColumn - i].CellState =
                        cells[_selectedRow, _selectedColumn - i].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;
            case 5:
                
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow + i, _selectedColumn + i].CellState =
                        cells[_selectedRow + i, _selectedColumn + i].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;
            case 6:
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow - i, _selectedColumn + i].CellState =
                        cells[_selectedRow - i, _selectedColumn + i].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;
            case 7:
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow + i, _selectedColumn - i].CellState =
                        cells[_selectedRow + i, _selectedColumn - i].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;
            case 8:
                for (int i = 1; i <= _flipCount; i++)
                {
                    cells[_selectedRow - i, _selectedColumn - i].CellState =
                        cells[_selectedRow - i, _selectedColumn - i].CellState == MyCellState
                        ? EnemyCellState : MyCellState;
                }
                break;
        }
        //Debug.Log($"_flipCount = {_flipCount}");
        _flipCount = 0;
        _turned = true;
    }
    bool TryCanPutCell(int pattern, int row, int column)
    {
        bool canFlip = false;
        bool reachingTheEdge = false;

        while ((cells[row, column].CellState == EnemyCellState) && !reachingTheEdge)
        {
            canFlip = true;
            _flipCount++;
            switch (pattern)
            {
                case 1://���֌���
                    if (TryCellCheck(row + 1, column))
                    {
                        //Debug.Log("case 1");
                        row++;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
                case 2://��֌���
                    if (TryCellCheck(row - 1, column))
                    {
                        //Debug.Log("case 2");

                        row--;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
                case 3://�E�֌���
                    if (TryCellCheck(row, column + 1))
                    {
                        //Debug.Log("case 3");
                        column++;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
                case 4://���֌���
                    if (TryCellCheck(row, column - 1))
                    {
                        //Debug.Log("case 4");
                        column--;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
                case 5:
                    //�E���`�F�b�N
                    if (TryCellCheck(row + 1, column + 1))
                    {
                        //Debug.Log("case 5");
                        row++;
                        column++;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
                case 6:
                    //�E��
                    if (TryCellCheck(row - 1, column + 1))
                    {
                        //Debug.Log("case 6");
                        row--;
                        column++;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
                case 7:
                    //����
                    if (TryCellCheck(row + 1, column - 1))
                    {
                        //Debug.Log("case 7");
                        row++;
                        column--;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
                case 8:
                    //����
                    if (TryCellCheck(row - 1, column - 1))
                    {
                        //Debug.Log($"case 8 {_flipCount}");
                        row--;
                        column--;
                        break;
                    }
                    else
                    {
                        reachingTheEdge = true;
                        break;
                    }
            }

        }
        if (cells[row, column].CellState == MyCellState)
        {
            if (canFlip)
            {
                return true;
            }
        }

        return false;
    }
}