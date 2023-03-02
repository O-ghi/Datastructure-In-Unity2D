using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Maze : MonoBehaviour
{
    public static Maze instance;

    public GameObject wallprefab;
    public GameObject cellPrefab;
    public GameObject cellHolder;
    public int rows = 13;
    public int columns = 10;

    public GameObject[,] listCell;

    public Stack<Transform> pathList = new Stack<Transform>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        listCell = new GameObject[rows, columns];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject go = Instantiate(cellPrefab, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(cellHolder.transform, false);
                go.GetComponent<Cell>().pos = new Vector3(i, j, 0f);
                listCell[i, j] = go;
                // Debug.Log(go.GetComponent<Cell>().pos);
            }
        }

        GenerateMap();
        GameController.instance.SetBug();
        GameController.instance.SetRandomTarget();
        //FindTheWayToTarget();
    }

    private void DestroyNeighborWall(Cell currentCell, int way)
    {
        try
        {
            if (way == 1) // left
            {
                Cell neighborCell = listCell[(int)currentCell.pos.x, (int)currentCell.pos.y - 1].GetComponent<Cell>();
                neighborCell.DestroyWall(3);
            }
            else if (way == 2) // down
            {
                Cell neighborCell = listCell[(int)currentCell.pos.x + 1, (int)currentCell.pos.y].GetComponent<Cell>();
                neighborCell.DestroyWall(4);
            }
            else if (way == 3) // right
            {
                Cell neighborCell = listCell[(int)currentCell.pos.x, (int)currentCell.pos.y + 1].GetComponent<Cell>();
                neighborCell.DestroyWall(1);
            }
            else if (way == 4) // up
            {
                Cell neighborCell = listCell[(int)currentCell.pos.x - 1, (int)currentCell.pos.y].GetComponent<Cell>();
                neighborCell.DestroyWall(2);
            }
        }
        catch
        {

        }

    }

    private void DestroyWall(Cell cell, int way)
    {
        cell.DestroyWall(way);
        DestroyNeighborWall(cell, way);
    }

    private void GenerateMap()
    {
        Cell firstCell = listCell[0, 0].GetComponent<Cell>();
        DFS_DeleteWall(firstCell);
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Cell cell = listCell[x, y].GetComponent<Cell>();
                cell.gCost = 99999;
                cell.CalculateFCost();
                cell.cameFromCell = null;
            }
        }
    }

    private Cell getNextCell(Cell currentCell, int way)
    {
        try
        {
            if (way == 1) // left
            {
                return listCell[(int)currentCell.pos.x, (int)currentCell.pos.y - 1].GetComponent<Cell>();
            }
            else if (way == 2) // down
            {
                return listCell[(int)currentCell.pos.x + 1, (int)currentCell.pos.y].GetComponent<Cell>();
            }
            else if (way == 3) // right
            {
                return listCell[(int)currentCell.pos.x, (int)currentCell.pos.y + 1].GetComponent<Cell>();
            }
            else if (way == 4) // up
            {
                return listCell[(int)currentCell.pos.x - 1, (int)currentCell.pos.y].GetComponent<Cell>();
            }
        }
        catch
        {
            return null;
        }
        return null;
    }

    private int[] suffefArray()
    {
        int[] temp = new int[4];
        temp[0] = UnityEngine.Random.Range(1, 5);
        do
        {
            temp[1] = UnityEngine.Random.Range(1, 5);
        } while (temp[1] == temp[0]);
        do
        {
            temp[2] = UnityEngine.Random.Range(1, 5);
        } while (temp[2] == temp[1] || temp[2] == temp[0]);
        temp[3] = 10 - temp[0] - temp[1] - temp[2];
        return temp;
    }

    private void DFS_DeleteWall(Cell cell)
    {
        cell.GetComponent<Image>().color = Color.red;
        cell.isVisited = true;

        int[] randomWay = suffefArray();


        for (int i = 0; i < 4; i++)
        {
            Cell nextCell = getNextCell(cell, randomWay[i]);

            if (nextCell == null)
            {
                continue;
            }

            if (!nextCell.isVisited)
            {
                DestroyWall(cell, randomWay[i]);
                DFS_DeleteWall(nextCell);
            }

        }

        int randomDestroy = UnityEngine.Random.Range(1, 10);

        if (randomDestroy == 1)
        {
            DestroyWall(cell, UnityEngine.Random.Range(1, 4));
        }
    }

    private void ResetListCell()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                listCell[i, j].GetComponent<Cell>().isVisited = false;
            }
        }
    }

    public void FindTheWayToTarget()
    {
        ResetListCell();
        pathList.Clear();
        Cell firstCell = listCell[0, 0].GetComponent<Cell>();
        List<Cell> path = FindPath();
        foreach (Cell cell in path)
        {
            Debug.Log(cell.pos);
            pathList.Push(cell.transform);
        }
    }



    // Pathfinding
    private List<Cell> openList;
    private List<Cell> closedList;

    private List<Cell> FindPath()
    {
        Cell starCell = listCell[0, 0].GetComponent<Cell>();
        Cell endCell = GameController.instance.targetHolder.GetComponent<Cell>();
        // Debug.Log("End cell: " + endCell.pos);

        if (starCell == null || endCell == null)
        {
            return null;
        }

        openList = new List<Cell> { starCell };
        closedList = new List<Cell>();



        starCell.gCost = 0;
        starCell.hCost = CalculateDistanceCost(starCell, endCell);

        starCell.CalculateFCost();

        while (openList.Count > 0)
        {
            Cell currentCell = GetLowestFCostCell(openList);
            if (currentCell == endCell)
            {
                // Debug.Log("Reach endcell");
                return CalculatePath(endCell);
            }

            // Debug.Log("Current cell: " + currentCell.pos);

            openList.Remove(currentCell);
            closedList.Add(currentCell);

            foreach (Cell neighbourCell in GetNeighbourList(currentCell))
            {
                if (closedList.Contains(neighbourCell))
                {
                    continue;
                }
                // Debug.Log("NeighbourCell: " + neighbourCell.pos);

                int tentativeGCost = currentCell.gCost + CalculateDistanceCost(currentCell, neighbourCell);
                if (tentativeGCost < neighbourCell.gCost)
                {
                    neighbourCell.cameFromCell = currentCell;
                    neighbourCell.gCost = tentativeGCost;
                    neighbourCell.hCost = CalculateDistanceCost(neighbourCell, endCell);
                    neighbourCell.CalculateFCost();

                    if (!openList.Contains(neighbourCell))
                    {
                        openList.Add(neighbourCell);
                    }
                }
            }
        }
        foreach (Cell cell in closedList)
        {
            Debug.Log(cell.pos);
        }
        Debug.Log("Did not reach endcell");
        return null;


    }

    private List<Cell> GetNeighbourList(Cell currentCell)
    {
        List<Cell> neighbourList = new List<Cell>();

        //Left
        if (currentCell.pos.y - 1 >= 0 && currentCell.isWallDestroyed(1))
        {
            neighbourList.Add(GetCell((int)currentCell.pos.x, (int)currentCell.pos.y - 1));
        }
        //Right
        if (currentCell.pos.y + 1 < columns && currentCell.isWallDestroyed(3))
        {
            neighbourList.Add(GetCell((int)currentCell.pos.x, (int)currentCell.pos.y + 1));
        }
        //Down
        if (currentCell.pos.x + 1 < rows && currentCell.isWallDestroyed(2))
        {
            neighbourList.Add(GetCell((int)currentCell.pos.x + 1, (int)currentCell.pos.y));
        }
        //Up
        if (currentCell.pos.x - 1 >= 0 && currentCell.isWallDestroyed(4))
        {
            neighbourList.Add(GetCell((int)currentCell.pos.x - 1, (int)currentCell.pos.y));
        }

        return neighbourList;

    }

    private Cell GetCell(int x, int y)
    {
        return listCell[x, y].GetComponent<Cell>();
    }

    private List<Cell> CalculatePath(Cell endCell)
    {
        List<Cell> path = new List<Cell>();
        path.Add(endCell);
        Cell currentCell = endCell;
        while (currentCell.cameFromCell != null)
        {
            path.Add(currentCell.cameFromCell);
            currentCell = currentCell.cameFromCell;
        }
        return path;
    }

    private int CalculateDistanceCost(Cell cellA, Cell cellB)
    {
        int xDistance = (int)Mathf.Abs(cellA.pos.x - cellB.pos.x);
        int yDistance = (int)Mathf.Abs(cellA.pos.y - cellB.pos.y);
        int remaining = Mathf.Abs(xDistance + yDistance);
        return 10 * remaining;
    }

    private Cell GetLowestFCostCell(List<Cell> cellList)
    {
        Cell lowestFCostCell = cellList[0];
        for (int i = 1; i < cellList.Count; i++)
        {
            if (cellList[i].fCost < lowestFCostCell.fCost)
            {
                lowestFCostCell = cellList[i];
            }
        }

        return lowestFCostCell;
    }

}
