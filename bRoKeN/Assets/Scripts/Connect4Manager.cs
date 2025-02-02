using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connect4Manager : MonoBehaviour
{
    // Board dimensions (typically 7 columns and 6 rows)
    public int numColumns = 7;
    public int numRows = 6;

    // Disc types (Empty = 0, Player = 1, Computer = 2)
    public enum DiscType { Empty = 0, Player = 1, Computer = 2 };

    // A serializable class representing one column of board slots.
    // In the Inspector, assign each column’s slots (index 0 = bottom slot, highest index = top slot).
    [System.Serializable]
    public class Column
    {
        public Transform[] slots;
    }

    // Array of columns – assign these in the Inspector.
    public Column[] columns;

    // Disc prefabs for the player and computer. Assign your 3D disc models here.
    public GameObject playerDiscPrefab;
    public GameObject computerDiscPrefab;

    // Range for the computer’s move delay (in seconds).
    public float minDelay = 1.0f;
    public float maxDelay = 2.0f;

    // Audio clips for various events.
    [Tooltip("Sound played when the player places a disc.")]
    public AudioClip playerPlaceSound;
    [Tooltip("Sound played when the computer places a disc.")]
    public AudioClip computerPlaceSound;
    [Tooltip("Sound played when the puzzle resets.")]
    public AudioClip resetSound;
    [Tooltip("Sound played when the player wins.")]
    public AudioClip playerWinSound;
    [Tooltip("Sound played when the computer wins.")]
    public AudioClip computerWinSound;

    // Reference to an AudioSource component.
    private AudioSource audioSource;

    // Internal board state where board[col, row] stores:
    // 0 = empty, 1 = player disc, 2 = computer disc.
    private int[,] board;

    // Keep track of instantiated disc GameObjects so we can clear them when resetting.
    private List<GameObject> activeDiscs = new List<GameObject>();

    // Flag to disable player input while the computer is moving or after a win.
    private bool inputEnabled = true;

    void Start()
    {
        board = new int[numColumns, numRows];
        InitializeBoard();
        Debug.Log("board on");
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogWarning("No AudioSource found on Connect4Manager object. Please add one.");
        }
    }

    /// <summary>
    /// Resets the board state and clears all disc objects.
    /// Plays the reset sound effect.
    /// </summary>
    void InitializeBoard()
    {
        
        for (int col = 0; col < numColumns; col++)
        {
            for (int row = 0; row < numRows; row++)
            {
                board[col, row] = (int)DiscType.Empty;
            }
        }
        foreach (GameObject disc in activeDiscs)
        {
            Destroy(disc);
        }
        activeDiscs.Clear();
        inputEnabled = true;

        // Play the reset sound effect.
        if (audioSource != null && resetSound != null)
        {
            audioSource.PlayOneShot(resetSound);
        }
    }

    /// <summary>
    /// Inserts a disc into a column, returning the row index where the disc was placed.
    /// Returns -1 if the column is full.
    /// </summary>
    int InsertDisc(int column, DiscType discType)
    {
        if (column < 0 || column >= numColumns)
        {
            Debug.LogError("Invalid column index: " + column);
            return -1;
        }

        // Start from the bottom slot (index 0) and move upward.
        for (int row = 0; row < numRows; row++)
        {
            if (board[column, row] == (int)DiscType.Empty)
            {
                board[column, row] = (int)discType;
                // Instantiate the disc prefab at the slot's position.
                Transform slotTransform = columns[column].slots[row];
                GameObject discPrefab = (discType == DiscType.Player) ? playerDiscPrefab : computerDiscPrefab;
                GameObject discInstance = Instantiate(discPrefab, slotTransform.position, slotTransform.rotation);
                activeDiscs.Add(discInstance);

                // Play the appropriate disc placement sound effect.
                if (audioSource != null)
                {
                    if (discType == DiscType.Player && playerPlaceSound != null)
                        audioSource.PlayOneShot(playerPlaceSound);
                    else if (discType == DiscType.Computer && computerPlaceSound != null)
                        audioSource.PlayOneShot(computerPlaceSound);
                }
                return row;
            }
        }
        // Column is full.
        return -1;
    }

    /// <summary>
    /// Checks if the specified column is full (i.e. the top-most slot is occupied).
    /// </summary>
    bool IsColumnFull(int column)
    {
        return board[column, numRows - 1] != (int)DiscType.Empty;
    }

    /// <summary>
    /// Called by your physical button script to trigger a player move.
    /// </summary>
    public void OnColumnButtonPressed(int column)
    {
        if (!inputEnabled)
            return;

        int row = InsertDisc(column, DiscType.Player);
        if (row != -1)
        {
            // Check if the player's move completes 4 in a row.
            if (CheckWin(column, row, (int)DiscType.Player))
            {
                Debug.Log("Player won!");
                if (audioSource != null && playerWinSound != null)
                    audioSource.PlayOneShot(playerWinSound);
                inputEnabled = false;  // Freeze the board so no further moves can be made.
                return;
            }

            // If the player's move filled the top-most slot in that column, reset the board.
            if (IsColumnFull(column))
            {
                Debug.Log("Column " + column + " is full. Resetting board.");
                StartCoroutine(ResetBoardAfterDelay());
                return;
            }

            // Disable input while the computer makes its move.
            inputEnabled = false;
            StartCoroutine(ComputerMoveCoroutine());
        }
        else
        {
            Debug.Log("Column " + column + " is already full. Please choose a different column.");
        }
    }

    /// <summary>
    /// Handles the computer move after a delay.
    /// </summary>
    IEnumerator ComputerMoveCoroutine()
    {
        float delay = Random.Range(minDelay, maxDelay);
        yield return new WaitForSeconds(delay);

        List<int> validColumns = new List<int>();
        for (int col = 0; col < numColumns; col++)
        {
            if (!IsColumnFull(col))
                validColumns.Add(col);
        }

        if (validColumns.Count > 0)
        {
            int chosenColumn = validColumns[Random.Range(0, validColumns.Count)];
            int row = InsertDisc(chosenColumn, DiscType.Computer);
            if (row != -1)
            {
                if (CheckWin(chosenColumn, row, (int)DiscType.Computer))
                {
                    Debug.Log("Computer won!");
                    if (audioSource != null && computerWinSound != null)
                        audioSource.PlayOneShot(computerWinSound);
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(ResetBoardAfterDelay());
                    yield break;
                }

                if (IsColumnFull(chosenColumn))
                {
                    yield return new WaitForSeconds(0.5f);
                    StartCoroutine(ResetBoardAfterDelay());
                    yield break;
                }
            }
        }
        // Re-enable input if no win or reset occurred.
        inputEnabled = true;
    }

    /// <summary>
    /// Waits briefly before resetting the board.
    /// </summary>
    IEnumerator ResetBoardAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        InitializeBoard();
    }

    /// <summary>
    /// Checks if there are four consecutive discs of the same type, starting from the disc placed at (column, row).
    /// It checks horizontally, vertically, and in both diagonal directions.
    /// </summary>
    bool CheckWin(int column, int row, int discType)
    {
        // Direction vectors: horizontal, vertical, diagonal (up-right), and diagonal (down-right).
        int[][] directions = new int[][] {
            new int[] { 1, 0 },
            new int[] { 0, 1 },
            new int[] { 1, 1 },
            new int[] { 1, -1 }
        };

        foreach (var dir in directions)
        {
            int dx = dir[0];
            int dy = dir[1];
            int count = 1; // Count the current disc.

            // Check in the positive direction.
            int c = column + dx;
            int r = row + dy;
            while (c >= 0 && c < numColumns && r >= 0 && r < numRows && board[c, r] == discType)
            {
                count++;
                c += dx;
                r += dy;
            }

            // Check in the negative direction.
            c = column - dx;
            r = row - dy;
            while (c >= 0 && c < numColumns && r >= 0 && r < numRows && board[c, r] == discType)
            {
                count++;
                c -= dx;
                r -= dy;
            }

            if (count >= 4)
                return true;
        }
        return false;
    }
}
