using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System;
using System.Linq;

public enum BoardType
{
    NULL, Square2D
}

public class MainLibrary : MonoBehaviour
{
    

    XmlDocument pieceXML, boardXML;
    string externalFolderLocation = "./Assets/FutureExternalFolder"; // TEMPORAL FOLDER, CHANGE BEFORE COMPILING

    public Dictionary<string, PieceElement> pieceLibrary;
    public Dictionary<string, BoardElement> boardLibrary;

    void Start()
    {
        /*** INIT ***/
        pieceLibrary = new Dictionary<string, PieceElement>();
        boardLibrary = new Dictionary<string, BoardElement>();

        string
            piecesFolder = externalFolderLocation + "/Pieces",
            boardsFolder = externalFolderLocation + "/Boards";
        List<string> importLog = new List<string>();

        /*** LOADING PIECES ***/
        importLog.Add("╔════════════════════════════╗");
        importLog.Add("║      IMPORTING PIECES      ║");
        importLog.Add("╚════════════════════════════╝");
        importLog.Add("");
        string[] folders = System.IO.Directory.GetDirectories(piecesFolder);
        foreach (string folder in folders)
        {
            /* LOADING XML */
            string folderName = folder.Substring(piecesFolder.Length + 1);

            if (folderName.Equals("TemplatePiece")) continue;
            importLog.Add("═══ Importing folder \"" + folderName + "\" ═══");

            PieceElement newPiece = new PieceElement();
            string newID = "";

            XmlDocument piece = new XmlDocument();
            try
            {
                piece.Load(folder + "/piece.xml");
            }
            catch (XmlException e)
            {
                importLog.Add("[ERROR] Could not load piece.xml. Check that the file does exists and it is spelled correctly. Remember it is case sensitive.");
                continue;
            }

            /* READING METADATA */
            try
            {
                XmlNodeList metadataList = piece.GetElementsByTagName("metadata")[0].ChildNodes;
                foreach (XmlNode metadata in metadataList)
                {
                    switch (metadata.Name)
                    {
                        case "ID":
                            newID = metadata.InnerText;
                            break;
                        case "name":
                            newPiece.name = metadata.InnerText;
                            break;
                        case "version":
                            newPiece.version = metadata.InnerText;
                            break;
                        case "author":
                            newPiece.author = metadata.InnerText;
                            break;
                        case "boardType":
                            switch (metadata.InnerText)
                            {
                                case "Square2D":
                                    newPiece.boardType = BoardType.Square2D;
                                    break;
                                default:
                                    newPiece.boardType = BoardType.NULL;
                                    break;
                            }
                            break;
                        default:
                            importLog.Add("[WARNING] \"" + metadata.Name + "\" is not a valid metadata entry.");
                            break;
                    }
                }
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"metadata\" element in the file. Skipping piece.");
                continue;
            }

            /* READING MOVES */
            try
            {
                XmlNodeList movesList = piece.GetElementsByTagName("moves")[0].ChildNodes;
                foreach (XmlNode move in movesList)
                {
                    Move newMove = new Move();

                    switch (move.Name) // Style
                    {
                        case "infinite":
                            newMove.style = Style.INFINITE;
                            break;
                        case "finite":
                            newMove.style = Style.FINITE;
                            break;
                        case "jump":
                            newMove.style = Style.JUMP;
                            break;
                        case "infinitejump":
                            newMove.style = Style.INFINITEJUMP;
                            break;
                        default:
                            importLog.Add("[WARNING] \"" + move.Name + "\" is not a valid move style, skipping move.");
                            continue;
                    }

                    if (move.Attributes.Count == 1) // Type
                    {
                        if (move.Attributes[0].Name == "type")
                        {
                            switch (move.Attributes[0].InnerText)
                            {
                                case "move":
                                    newMove.type = Type.MOVE;
                                    break;
                                case "capture":
                                    newMove.type = Type.CAPTURE;
                                    break;
                                case "both":
                                    newMove.type = Type.BOTH;
                                    break;
                                default:
                                    importLog.Add("[WARNING] \"" + move.Attributes[0].InnerText + "\" is not a valid move type. Skipping move.");
                                    continue;
                            }
                        }
                        else
                        {
                            importLog.Add("[WARNING] \"" + move.Attributes[0].Name + "\" is not a valid attribute name, only \"type\" is accepted. Skipping move.");
                            continue;
                        }
                    }
                    else
                    {
                        importLog.Add("[WARNING] Wrong amount of atributes, only one can be used. Skipping move.");
                        continue;
                    }

                    if (move.InnerText.Length != 0) // Move Coords
                    {
                        string text = move.InnerText;
                        List<int> moveCoords = new List<int>();
                        bool possible = true, valid = true;
                        try
                        {
                            while (possible)
                            {
                                int newCoord;
                                if (text.Contains(","))
                                {
                                    newCoord = int.Parse(text.Substring(0, text.IndexOf(',')));
                                    text = text.Substring(text.IndexOf(',') + 1);
                                }
                                else
                                {
                                    newCoord = int.Parse(text);
                                    possible = false;
                                }

                                moveCoords.Add(newCoord);
                            }
                        }
                        catch
                        {
                            importLog.Add("[WARNING] Coordinates formatted incorrectly. Skipping move.");
                            valid = false;
                        }

                        if (valid)
                        {
                            switch (newPiece.boardType)
                            {
                                case BoardType.Square2D:
                                    if (moveCoords.Count == 2)
                                    {
                                        newMove.move = moveCoords.ToArray();
                                    }
                                    else
                                    {
                                        importLog.Add("[WARNING] Invalid amount of coordinates for the board type. Skipping move.");
                                        continue;
                                    }
                                    break;
                            }
                        }
                        else
                            continue;
                    }
                    else
                    {
                        importLog.Add("[WARNING] No coordinates specified. Skipping move.");
                        continue;
                    }

                    newPiece.moves.Add(newMove);
                }
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"moves\" element in the file. Skipping piece.");
                continue;
            }

            /* LOADING IMAGE */
            bool isValid = true;

            try
            {
                Texture2D tmpTexture = new Texture2D(2, 2);
                tmpTexture.LoadImage(File.ReadAllBytes(folder + "/icon.png"));
                newPiece.image = Sprite.Create(tmpTexture, new Rect(0.0f, 0.0f, tmpTexture.width, tmpTexture.height), new Vector2(0.5f, 0.5f));
            }
            catch (Exception e)
            {
                Debug.Log(e);
                isValid = false;
                importLog.Add("[ERROR] Could not load icon.png. Check that the file does exists and it is spelled correctly. Remember it is case sensitive.");
            }

            /* PIECE CHECKING */
            if (newID.Length == 0) // ID is empty?
            {
                importLog.Add("[ERROR] The ID cannot be empty.");
                isValid = false;
            }
            else if (pieceLibrary.ContainsKey(newID)) // ID in use?
            {
                PieceElement otherPiece;
                pieceLibrary.TryGetValue(newID, out otherPiece);
                importLog.Add("[ERROR] The ID \"" + newID + "\" is already used by the piece with the name \"" + otherPiece.name + "\".");
                isValid = false;
            }
            if (newPiece.name.Length == 0) // Has a name?
            {
                importLog.Add("[ERROR] The name cannot be empty.");
                isValid = false;
            }
            if (newPiece.boardType == BoardType.NULL) // Valid BoardType?
            {
                importLog.Add("[ERROR] The boardType is invalid. Remember it is case sensitive.");
                isValid = false;
            }
            if (newPiece.moves.Count == 0) // Has moves?
            {
                importLog.Add("[ERROR] No moves were saved.");
                isValid = false;
            }

            /* SAVING PIECE */
            if (isValid) // ALL OK
            {
                pieceLibrary.Add(newID, newPiece);
                importLog.Add("[INFO] Piece added to the library successfully.");
            }
        }

        /*** LOADING BOARDS ***/
        importLog.Add("");
        importLog.Add("");
        importLog.Add("╔════════════════════════════╗");
        importLog.Add("║      IMPORTING BOARDS      ║");
        importLog.Add("╚════════════════════════════╝");
        importLog.Add("");
        folders = System.IO.Directory.GetDirectories(boardsFolder);
        foreach (string folder in folders)
        {
            /* LOADING XML */
            string folderName = folder.Substring(piecesFolder.Length + 1);

            if (folderName.Equals("TemplateBoard")) continue;
            importLog.Add("═══ Importing folder \"" + folderName + "\" ═══");

            BoardElement newBoard = new BoardElement();
            string newName = "";

            XmlDocument board = new XmlDocument();
            try
            {
                board.Load(folder + "/board.xml");
            }
            catch (XmlException e)
            {
                importLog.Add("[ERROR] Could not load board.xml. Check that the file does exists and it is spelled correctly. Remember it is case sensitive.");
                continue;
            }

            /* LOADING METADATA */
            try
            {
                XmlNodeList metadataList = board.GetElementsByTagName("metadata")[0].ChildNodes;
                foreach (XmlNode metadata in metadataList)
                {
                    switch (metadata.Name)
                    {
                        case "name":
                            newName = metadata.InnerText;
                            break;
                        case "version":
                            newBoard.version = metadata.InnerText;
                            break;
                        case "author":
                            newBoard.author = metadata.InnerText;
                            break;
                        case "boardType":
                            switch (metadata.InnerText)
                            {
                                case "Square2D":
                                    newBoard.boardType = BoardType.Square2D;
                                    break;
                                default:
                                    newBoard.boardType = BoardType.NULL;
                                    break;
                            }
                            break;
                        default:
                            importLog.Add("[WARNING] \"" + metadata.Name + "\" is not a valid metadata entry.");
                            break;
                    }
                }
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"metadata\" element in the file. Skipping board.");
                continue;
            }

            /* LOADING PLAYERS */
            bool failed = false;
            try
            {
                XmlNodeList playerList = board.GetElementsByTagName("players")[0].ChildNodes;
                int soloTeam = -1;
                foreach (XmlNode player in playerList)
                {
                    if (player.Name != "player")
                    {
                        importLog.Add("[WARNING] There's something in the players list that is not a player. Check the spelling and remember it is case sensitive.");
                        continue;
                    }

                    PlayerImport newPlayer = new PlayerImport();
                    foreach (XmlNode playerInfo in player.ChildNodes)
                    {
                        switch (playerInfo.Name)
                        {
                            case "team":
                                int team = 0;
                                if (playerInfo.InnerText == "-")
                                {
                                    team = soloTeam;
                                    soloTeam--;
                                }
                                else try
                                    {
                                        team = int.Parse(playerInfo.InnerText);
                                        if (team < -1)
                                        {
                                            importLog.Add("[WARNING] A player's team cannot be negative.");
                                            continue;
                                        }
                                    }
                                    catch
                                    {
                                        importLog.Add("[WARNING] A player's team must be a integer number or a hyphen (-).");
                                        continue;
                                    }
                                newPlayer.team = team;
                                break;
                            case "color":
                                try
                                {
                                    int
                                        r = int.Parse(playerInfo.InnerText.Substring(0, playerInfo.InnerText.IndexOf(','))),
                                        g = int.Parse(playerInfo.InnerText.Substring(playerInfo.InnerText.IndexOf(',') + 1, playerInfo.InnerText.LastIndexOf(',') - playerInfo.InnerText.IndexOf(',') - 1)),
                                        b = int.Parse(playerInfo.InnerText.Substring(playerInfo.InnerText.LastIndexOf(',') + 1));
                                    newPlayer.color = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
                                }
                                catch (Exception e)
                                {
                                    importLog.Add("[WARNING] Player color is formated incorrectly.");
                                    continue;
                                }

                                break;
                            case "direction":
                                try
                                {
                                    int dir = int.Parse(playerInfo.InnerText);
                                    newPlayer.direction = dir;
                                }
                                catch
                                {
                                    importLog.Add("[WARNING] A player's direction must be a number.");
                                    continue;
                                }
                                break;
                            default:
                                importLog.Add("[WARNING] \"" + playerInfo.Name + "\" is not a valid entry for a player.");
                                continue;
                        }
                    }
                    if (newPlayer.direction == null)
                    {
                        importLog.Add("[ERROR] A player doesn't have a direction. Skipping board.");
                        failed = true;
                        break;
                    }
                    else if (newPlayer.color == null)
                    {
                        importLog.Add("[ERROR] A player doesn't have a color. Skipping board.");
                        failed = true;
                        break;
                    }
                    else if (newPlayer.team == null)
                    {
                        importLog.Add("[ERROR] A player doesn't have a team. Skipping board.");
                        failed = true;
                        break;
                    }
                    else
                    {
                        newBoard.players.Add(newPlayer);
                    }
                }
                if (failed)
                {
                    importLog.Add("[ERROR] At least one player entry is not complete. Skipping board.");
                    continue;
                }
                else if (playerList.Count < 2 || playerList.Count > 9)
                {
                    importLog.Add("[ERROR] Number of players is out of accepted values. It MUST be between 2 and 9. Skipping board.");
                    continue;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e);
                importLog.Add("[ERROR] Could not find a \"players\" element in the file. Skipping board.");
                continue;
            }

            /* LOADING PIECES */
            try
            {
                XmlNodeList pieceList = board.GetElementsByTagName("pieces")[0].ChildNodes;
                failed = false;
                foreach (XmlNode piece in pieceList)
                {
                    if (piece.Name != "piece")
                    {
                        importLog.Add("[WARNING] There's something in the piece list that is not a piece. Check the spelling and remember it is case sensitive.");
                        continue;
                    }
                    else
                    {
                        PieceImport newPiece = new PieceImport();
                        char pieceChar = ' ';
                        foreach (XmlAttribute attribute in piece.Attributes)
                        {
                            if (attribute.Name == "type")
                            {
                                switch (attribute.InnerText)
                                {
                                    case "king":
                                        newPiece.type = PieceType.KING;
                                        break;
                                    case "pawn":
                                        newPiece.type = PieceType.PAWN;
                                        break;
                                    case "upgrade":
                                        newPiece.type = PieceType.UPGRADE;
                                        break;
                                    default:
                                        importLog.Add("[WARNING] \"" + attribute.InnerText + "\" is not a valid piece type. Ignoring.");
                                        break;
                                }
                            }
                            else
                            {
                                importLog.Add("[WARNING] \"" + attribute.InnerText + "\" is not a valid attribute for a piece. Ignoring.");
                            }
                        }
                        foreach (XmlNode pieceData in piece.ChildNodes)
                        {
                            switch (pieceData.Name)
                            {
                                case "ID":
                                    newPiece.ID = pieceData.InnerText;
                                    break;
                                case "value":
                                    newPiece.value = int.Parse(pieceData.InnerText);
                                    break;
                                case "char":
                                    if (pieceData.InnerText.Length == 1)
                                        pieceChar = pieceData.InnerText[0];
                                    else
                                        importLog.Add("[WARNING] Piece character field must be a single character. Ignoring.");
                                    break;
                                default:
                                    importLog.Add("[WARNING] \"" + pieceData.Name + "\" is not a valid field in a piece. Skipping.");
                                    continue;
                            }
                        }

                        string invalidCharacters = " _:0123456789";

                        if (newPiece.ID == null)
                        {
                            importLog.Add("[WARNING] Piece ID is obligatory. Skipping. ");
                            failed = true;
                            continue;
                        }
                        else if (!pieceLibrary.Keys.ToArray().Contains(newPiece.ID))
                        {
                            importLog.Add("[WARNING] Piece ID does not exist in the library. Skipping. ");
                            failed = true;
                            continue;
                        }
                        if (newPiece.value < 0)
                        {
                            importLog.Add("[WARNING] Piece value cannot be negative. Skipping.");
                            failed = true;
                            continue;
                        }

                        if (invalidCharacters.Contains(pieceChar))
                        {
                            importLog.Add("[WARNING] Piece character not indicated or invalid. Skipping.");
                            failed = true;
                            continue;
                        }

                        newBoard.pieceIDs.Add(pieceChar, newPiece);
                    }
                }
                if (failed)
                {
                    importLog.Add("[ERROR] At least one piece entry is not complete. Skipping board.");
                    continue;
                }
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"pieces\" element in the file. Skipping board.");
                continue;
            }

            /* LOADING POSITION */
            try
            {
                XmlNodeList positionRows = board.GetElementsByTagName("position")[0].ChildNodes;
                failed = false;
                switch (newBoard.boardType)
                {
                    case BoardType.Square2D:
                        // Import Strings
                        List<List<string>> initialPosition = new List<List<string>>();
                        int columns = -1;
                        foreach (XmlNode row in positionRows)
                        {
                            if (row.Name != "row")
                            {
                                importLog.Add("[ERROR] There's something that is not a row inside the intial position. Skipping board.");
                                failed = true;
                                break;
                            }
                            else
                            {
                                bool possible = true;
                                List<string> newRow = new List<string>();
                                string stringRow = row.InnerText;
                                while (possible)
                                {
                                    if (stringRow.IndexOf(',') != -1)
                                    {
                                        newRow.Add(stringRow.Substring(0, stringRow.IndexOf(',')));
                                        stringRow = stringRow.Substring(stringRow.IndexOf(',') + 1);
                                    }
                                    else
                                    {
                                        newRow.Add(stringRow);
                                        possible = false;
                                    }
                                }
                                
                                if (columns == -1)
                                {
                                    initialPosition.Add(newRow);
                                    columns = newRow.Count;
                                }
                                else if (columns != newRow.Count)
                                {
                                    importLog.Add("[ERROR] Columns amount in the initial position is not the same in all the rows. Skipping board.");
                                    failed = true;
                                    break;
                                }
                                else
                                    initialPosition.Add(newRow);
                            }
                        }
                        // Checking Strings
                        foreach (List<string> row in initialPosition)
                            foreach (string cell in row)
                            {
                                if (cell[0] != '_' && cell[0] != '0')
                                    try
                                    {
                                        int owner = int.Parse(cell[0].ToString());
                                        if (owner - 1 >= newBoard.players.Count)
                                        {
                                            importLog.Add("[ERROR] Player owner in a cell is outside the player list.");
                                            failed = true;
                                            continue;
                                        }
                                        else
                                        {
                                            if (!newBoard.pieceIDs.Keys.ToArray().Contains(cell[1]))
                                            {
                                                importLog.Add("[ERROR] Piece in a cell is not in the imported piece list.");
                                                failed = true;
                                                continue;
                                            }
                                            else
                                            {
                                                if (cell.Length > 2)
                                                {
                                                    for (int i = 2; i < cell.Length; i = i + 2)
                                                    {
                                                        if (cell[i] != ':')
                                                        {
                                                            Debug.Log("643");
                                                            importLog.Add("[ERROR] Invalid character detected after the cell description.");
                                                            failed = true;
                                                            continue;
                                                        }
                                                        else
                                                        {
                                                            try
                                                            {
                                                                int promote = int.Parse(cell[i + 1].ToString());
                                                                if (promote - 1 >= newBoard.players.Count)
                                                                {
                                                                    importLog.Add("[ERROR] Promote player in a cell is outside the player list.");
                                                                    failed = true;
                                                                    continue;
                                                                }
                                                            }
                                                            catch
                                                            {
                                                                importLog.Add("[ERROR] Invalid character detected as player to asign a cell as promotable.");
                                                                failed = true;
                                                                continue;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        importLog.Add("[ERROR] Invalid character detected in a cell of the initial position.");
                                        failed = true;
                                        continue;
                                    }
                            }
                        BoardSquare2D init = new BoardSquare2D();
                        init.board = new string[initialPosition.Count][];
                        for (int i = 0; i < initialPosition.Count; i++)
                            init.board[i] = initialPosition[i].ToArray();
                        newBoard.initialState = init;
                        break;
                }
                if (failed)
                {
                    continue;
                }
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"position\" element in the file. Skipping board.");
                continue;
            }

            /* LOADING SPECIALS */
            // TODO

            /* LOADING IMAGE */
            bool isValid = true;

            try
            {
                Texture2D tmpTexture = new Texture2D(2, 2);
                tmpTexture.LoadImage(File.ReadAllBytes(folder + "/icon.png"));
                newBoard.image = Sprite.Create(tmpTexture, new Rect(0.0f, 0.0f, tmpTexture.width, tmpTexture.height), new Vector2(0.5f, 0.5f));
            }
            catch
            {
                isValid = false;
                importLog.Add("[ERROR] Could not load icon.png. Check that the file does exists and it is spelled correctly. Remember it is case sensitive.");
            }

            /* BOARD CHECKING */
            if (newName.Length == 0) // Has name?
            {
                importLog.Add("[ERROR] The ID cannot be empty.");
                isValid = false;
            }
            else if (boardLibrary.ContainsKey(newName)) // Name exists in board library?
            {
                PieceElement otherBoard;
                pieceLibrary.TryGetValue(newName, out otherBoard);
                importLog.Add("[ERROR] The ID \"" + newName + "\" is already used by another board in the library.");
                isValid = false;
            }

            /* SAVING BOARD */
            if (isValid)
            {
                boardLibrary.Add(newName, newBoard);
                importLog.Add("[INFO] Board added to the library successfully.");
            }

        }

        /*** LAST INIT ***/
        GameObject.DontDestroyOnLoad(gameObject);
        foreach (string message in importLog) Debug.Log(message); // Temporal. Exportar a archivo en un futuro.
    }

    public PieceElement GetPiece(string id)
    {
        PieceElement element;
        pieceLibrary.TryGetValue(id, out element);
        return element;
    }

    public BoardElement GetBoard(string id)
    {
        BoardElement element;
        boardLibrary.TryGetValue(id, out element);
        return element;
    }
}
