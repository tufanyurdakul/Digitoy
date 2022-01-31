using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class SetTiles : MonoBehaviour
{
    private List<Tile> Tiles;
    private List<Tile> RandomTiles;
    private List<Player> Players;
    private List<Color> ColorsOfTile;
    //Text To Show Tiles
    private TextMeshProUGUI tmpTiles;
    private int[] playersPoint;
    public List<Transform> TransformOfTiles;
    public List<TextMeshProUGUI> ListOfTextPlayer;
    enum Colors
    {
        Red = 1,
        Black = 2,
        Blue = 3,
        Yellow = 4,
        Fake = 5
    }
    enum Sizes
    {
        ColorSize = 5,
        NumberSize = 14
    }

    private void Awake()
    {
        #region UnneccassaryDefinitions
        Tiles = new List<Tile>();
        RandomTiles = new List<Tile>();
        Players = new List<Player>();
        ColorsOfTile = new List<Color>();
        ColorsOfTile.Add(Color.red);
        ColorsOfTile.Add(Color.black);
        ColorsOfTile.Add(Color.blue);
        ColorsOfTile.Add(Color.yellow);
        playersPoint = new int[4];
        //Find My Text Mesh Object From File To Show Tile
        tmpTiles = (Resources.Load("Tile") as GameObject).GetComponent<TextMeshProUGUI>();
        #endregion
        //Add Tiles To Tile List Two Times
        AddTilesToList(0);
        AddTilesToList(53);
        AddTilesToFalse();
        //Create Random List For Tiles And New Array Is RandomTiles
        for (int i = 0; i < Tiles.Count; i++)
        {
            int randomNumber = Random.Range(0, Tiles.Count);
            RandomTiles.Add(Tiles[randomNumber]);
            Tiles.RemoveAt(randomNumber);
            i = -1;
        }
        #region SetGroupForRandom
        //Devide By 5 With Parameter Group
        for (int i = 0; i < 21; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                RandomTiles[(i * 5) + j].Group = i;
                RandomTiles[(i * 5) + j].Order = j;
            }
        }
        #endregion
        // Okey Tasini Al And To Dice
        Tile okey = new Tile();
        okey = GetOkey(ThrowDice());
        // if tile is fake okey again GetOkey with dices
        if (okey.Color == 5)
        {
            okey = GetOkey(ThrowDice());

        }
        #region ShowJoker
        TextMeshProUGUI tmpOkey = Instantiate(tmpTiles, new Vector3(TransformOfTiles[4].position.x, TransformOfTiles[4].position.y, TransformOfTiles[4].position.z), Quaternion.identity);
        tmpOkey.SetText("" + okey.Number);
        tmpOkey.color = ColorsOfTile[okey.Color - 1];
        tmpOkey.transform.SetParent(TransformOfTiles[4]);
        #endregion
        #region Define Player Tiles
        //Who Have 15 Tiles
        int firstOrderPlayer = Random.Range(0, 4);
        //Define 15 Tiles Players
        Player player = new Player();
        player.PlayerTiles = new List<Tile>();
        foreach (var item in RandomTiles)
        {
            if (item.Group == okey.Group + 1)
            {
                player.PlayerTiles.Add(item);
            }
            if (item.Group == okey.Group + 5)
            {
                player.PlayerTiles.Add(item);
            }
            if (item.Group == okey.Group + 9)
            {
                player.PlayerTiles.Add(item);
            }
        }
        player.PlayerId = 0;
        player.PlayerName = "Player" + firstOrderPlayer;
        Players.Add(player);
        //Define Others Tiles
        for (int i = 0; i < 3; i++)
        {
            player = new Player();
            player.PlayerTiles = new List<Tile>();
            for (int j = 0; j < RandomTiles.Count; j++)
            {
                if (RandomTiles[j].Group == okey.Group + 2 + i)
                    player.PlayerTiles.Add(RandomTiles[j]);

                if (RandomTiles[j].Group == okey.Group + 5 + i)
                    player.PlayerTiles.Add(RandomTiles[j]);

                if (RandomTiles[j].Group == okey.Group + 10 + i)
                {
                    if (i == 0 && RandomTiles[j].Order != i)
                        player.PlayerTiles.Add(RandomTiles[j]);

                    if (i > 0)
                    {
                        if (RandomTiles[j - 1].Order <= i)
                            player.PlayerTiles.Add(RandomTiles[j - 1]);
                        if (RandomTiles[j].Order > i + 1)
                            player.PlayerTiles.Add(RandomTiles[j]);
                    }
                }
            }
            player.PlayerId = i + 1;
            player.PlayerName = "Player" + ((firstOrderPlayer + 1 + i) % 4);
            Players.Add(player);
        }
        #endregion
        #region Calculate Point
        //Calculate Point Of Players
        for (int i = 0; i < Players.Count; i++)
        {
            //Win List For Players
            playersPoint[i] = SetPoint(i);
            foreach (var item in Players[i].PlayerTiles)
            {
                if (item.Id >= 0)
                {
                    if (okey.Number == item.Number && okey.Color == item.Color)
                    {
                        playersPoint[i] += 5;
                    }
                }
            }
            Debug.Log(Players[i].PlayerName + "-" + playersPoint[i]);
        }
        #endregion
        #region FindWinner
        int maxValue = 0;
        int winnerId = 0;
        for (int i = 0; i < playersPoint.Length; i++)
        {
            if (playersPoint[i] > maxValue)
            {
                maxValue = playersPoint[i];
                winnerId = int.Parse(Players[i].PlayerName.Replace("Player", ""));
            }
        }
        ListOfTextPlayer[winnerId].SetText(ListOfTextPlayer[winnerId].text + "(Winner)");
        #endregion
        //Copy Text To Screen
        #region Show Screen
        foreach (var item in Players)
        {
            int playerOrder = int.Parse(item.PlayerName.Replace("Player", ""));
            for (int i = 0; i < item.PlayerTiles.Count; i++)
            {
                TextMeshProUGUI copyTile = Instantiate(tmpTiles, new Vector3(TransformOfTiles[playerOrder].position.x + (i * 58), TransformOfTiles[playerOrder].position.y, TransformOfTiles[playerOrder].position.z), Quaternion.identity);
                if (item.PlayerTiles[i].Id < 0)
                {
                    copyTile.SetText("C" + item.PlayerTiles[i].Number);

                }
                else
                {
                    copyTile.SetText("" + item.PlayerTiles[i].Number);
                }
                copyTile.color = ColorsOfTile[item.PlayerTiles[i].Color - 1];
                copyTile.name = "" + item.PlayerTiles[i].Id;
                copyTile.transform.SetParent(TransformOfTiles[playerOrder]);
            }
        }
        #endregion
    }
    private void AddTilesToList(int id)
    {
        //Create Tiles And Add Tile To Tiles List(1-14,K,S,M,S)
        for (int i = 1; i < (int)Sizes.ColorSize; i++)
        {
            for (int j = 1; j < (int)Sizes.NumberSize; j++)
            {
                Tile tile = new Tile()
                {
                    Id = i + j + id,
                    Color = i,
                    Number = j
                };
                Tiles.Add(tile);
            }
        }
    }
    private void AddTilesToFalse()
    {
        //Add Fake Rummy Tile
        for (int i = 1; i < 3; i++)
        {
            Tile tile = new Tile()
            {
                Id = i * -1,
                Color = 5,
                Number = 0
            };
            Tiles.Add(tile);
        }
    }
    private int[] ThrowDice()
    {
        int[] dice = new int[2];
        for (int i = 0; i < 2; i++)
        {
            dice[i] = Random.Range(1, 7);
        }
        return dice;
    }
    private Tile GetOkey(int[] dices)
    {
        // Move Dice To Number Of The Group From First
        RandomTiles[RandomTiles.Count - 1].Group = dices[0];
        RandomTiles[RandomTiles.Count - 1].Order = 5;
        Tile Okey = new Tile();
        foreach (var item in RandomTiles)
        {
            if (item.Group == dices[0])
            {
                if (item.Order == dices[1] - 1)
                {
                    Okey = item;
                }
            }
        }
        if (Okey.Number < 13)
        {
            Okey.Number++;
        }
        else
        {
            Okey.Number = 1;
        }
        // Set False Okey To Okey Tile Value
        foreach (var item in RandomTiles)
        {
            if (item.Id < 0)
            {
                item.Color = Okey.Color;
                item.Number = Okey.Number;
            }
        }

        return Okey;
    }
    private int SetPoint(int orderNumber)
    {
        int point = 0;
        //Create List For Same Color
        List<int> Reds = new List<int>();
        List<int> Blue = new List<int>();
        List<int> Black = new List<int>();
        List<int> Yellow = new List<int>();
        for (int i = 0; i < Players[orderNumber].PlayerTiles.Count; i++)
        {
            if (Players[orderNumber].PlayerTiles[i].Color == (int)Colors.Red)
                Reds.Add(Players[orderNumber].PlayerTiles[i].Number);
            if (Players[orderNumber].PlayerTiles[i].Color == (int)Colors.Yellow)
                Yellow.Add(Players[orderNumber].PlayerTiles[i].Number);
            if (Players[orderNumber].PlayerTiles[i].Color == (int)Colors.Black)
                Black.Add(Players[orderNumber].PlayerTiles[i].Number);
            if (Players[orderNumber].PlayerTiles[i].Color == (int)Colors.Blue)
                Blue.Add(Players[orderNumber].PlayerTiles[i].Number);
        }
        //Order Same Colors
        Reds.Sort();
        Blue.Sort();
        Black.Sort();
        Yellow.Sort();
        //Control Thriples And Mores
        for (int i = 0; i < Reds.Count; i++)
        {
            if (i > 0 && i < Reds.Count - 1)
            {
                if (Reds[i - 1] == Reds[i] - 1 && Reds[i + 1] == Reds[i] + 1)
                    point += 3;
                else if (Reds[i + 1] == Reds[i] + 1 || Reds[i + 1] == Reds[i] + 2)
                    point += 1;
                else if (Reds[i] == 12 && Reds[0] == 1)
                    point += 1;
            }
            //12-13-1
            if (i == Reds.Count - 1 && Reds[i] == 13 && i > 0)
            {
                if (Reds[i - 1] == Reds[i] - 1 && Reds[0] == 1)
                    point += 3;
            }
            if (i == Reds.Count - 1 && i > 0)
            {
                if (Reds[i] == 12 && Reds[0] == 1)
                    point += 1;
            }
        }
        for (int i = 0; i < Yellow.Count; i++)
        {
            if (i > 0 && i < Yellow.Count - 1)
            {
                if (Yellow[i - 1] == Yellow[i] - 1 && Yellow[i + 1] == Yellow[i] + 1)
                    point += 3;
                else if (Yellow[i + 1] == Yellow[i] + 1 || Yellow[i + 1] == Yellow[i] + 2)
                    point += 1;
                else if (Yellow[i] == 12 && Yellow[0] == 1)
                    point += 1;
            }
            //12-13-1
            if (i == Yellow.Count - 1 && Yellow[i] == 13 && i > 0)
            {
                if (Yellow[i - 1] == Yellow[i] - 1 && Yellow[0] == 1)
                    point += 3;
            }
            if (i == Yellow.Count - 1 && i > 0)
            {
                if (Yellow[i] == 12 && Yellow[0] == 1)
                    point += 1;
            }
        }
        for (int i = 0; i < Black.Count; i++)
        {
            if (i > 0 && i < Black.Count - 1)
            {
                if (Black[i - 1] == Black[i] - 1 && Black[i + 1] == Black[i] + 1)
                    point += 3;
                else if (Black[i + 1] == Black[i] + 1 || Black[i + 1] == Black[i] + 2)
                    point += 1;
                else if (Black[i] == 12 && Black[0] == 1)
                    point += 1;
            }
            //12-13-1
            if (i == Black.Count - 1 && Black[i] == 13 && i > 0)
            {
                if (Black[i - 1] == Black[i] - 1 && Black[0] == 1)
                    point += 3;
            }
            if (i == Black.Count - 1 && i > 0)
            {
                if (Black[i] == 12 && Black[0] == 1)
                    point += 1;
            }
        }
        for (int i = 0; i < Blue.Count; i++)
        {
            if (i > 0 && i < Blue.Count - 1)
            {
                if (Blue[i - 1] == Blue[i] - 1 && Blue[i + 1] == Blue[i] + 1)
                    point += 3;
                else if (Blue[i + 1] == Blue[i] + 1 || Blue[i + 1] == Blue[i] + 2)
                    point += 1;
                else if (Blue[i] == 12 && Blue[0] == 1)
                    point += 1;
            }
            //12-13-1
            if (i == Blue.Count - 1 && Blue[i] == 13 && i > 0)
            {
                if (Blue[i - 1] == Blue[i] - 1 && Blue[0] == 1)
                    point += 3;
            }
            if (i == Blue.Count - 1 && i > 0)
            {
                if (Blue[i] == 12 && Blue[0] == 1)
                    point += 1;
            }
        }
        //Remove Repeat Values From List
        Reds = Reds.Distinct().ToList();
        Yellow = Yellow.Distinct().ToList();
        Black = Black.Distinct().ToList();
        Blue = Blue.Distinct().ToList();
        var SameValues1 = Reds.Intersect(Blue).ToList();
        var SameValues2 = Reds.Intersect(Yellow).ToList();
        var SameValues3 = Reds.Intersect(Black).ToList();
        var SameValues4 = Blue.Intersect(Yellow).ToList();
        var SameValues5 = Blue.Intersect(Black).ToList();
        var SameValues6 = Yellow.Intersect(Black).ToList();
        point += (SameValues1.Count + SameValues2.Count + SameValues3.Count + SameValues4.Count + SameValues5.Count + SameValues6.Count);
        return point;
    }
}
