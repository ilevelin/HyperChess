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
    [SerializeField] Sprite defaultBoardSprite;
    XmlDocument pieceXML, boardXML;
    string externalFolderLocation;

    public Dictionary<string, PieceElement> pieceLibrary;
    public Dictionary<string, BoardElement> boardLibrary;

    void Start()
    {
        /*** DUPLICATION CHECK *********************************************************************************************************************/
        /*******************************************************************************************************************************************/
        if (GameObject.FindGameObjectsWithTag("MainLibrary").Length != 1) GameObject.Destroy(gameObject);

        GameObject.DontDestroyOnLoad(gameObject);

        /*** INIT **********************************************************************************************************************************/
        /*******************************************************************************************************************************************/
        pieceLibrary = new Dictionary<string, PieceElement>();
        boardLibrary = new Dictionary<string, BoardElement>();

        if (Application.isEditor)
            externalFolderLocation = ".\\Assets\\ExternalFolder";
        else
            externalFolderLocation = ".\\UserContent";

        string
            piecesFolder = externalFolderLocation + "\\Pieces",
            boardsFolder = externalFolderLocation + "\\Boards";
        List<string> importLog = new List<string>();

        /*** LOADING PIECES ************************************************************************************************************************/
        /*******************************************************************************************************************************************/
        importLog.Add("╔════════════════════════════╗");
        importLog.Add("║      IMPORTING PIECES      ║");
        importLog.Add("╚════════════════════════════╝");
        importLog.Add("");
        string[] folders = System.IO.Directory.GetDirectories(piecesFolder);
        foreach (string folder in folders)
        {
            /*** LOADING XML ***********************************************************************************************************************/
            string folderName = folder.Substring(piecesFolder.Length + 1);

            if (folderName.Equals("TemplatePiece")) continue;
            importLog.Add("");
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

            /*** READING METADATA ******************************************************************************************************************/
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

            /*** READING MOVES *********************************************************************************************************************/
            try
            {
                XmlNodeList movesList = piece.GetElementsByTagName("moves")[0].ChildNodes;
                foreach (XmlNode move in movesList)
                {
                    Move newMove = new Move();

                    // Style ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    switch (move.Name) 
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

                    // Type /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (move.Attributes.Count == 1) 
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

                    // Move Coords //////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    if (move.InnerText.Length != 0) 
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

                                moveCoords.Insert(0, newCoord);
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

            /*** LOADING IMAGE(S) ******************************************************************************************************************/
            bool validBoard = true;
            bool hasDefault = false;

            foreach (String filepath in Directory.GetFiles(folder)) {
                if (filepath.EndsWith(".png"))
                {
                    try
                    {
                        Texture2D tmpTexture = new Texture2D(2, 2);
                        Sprite tmpSprite = null;
                        bool validImage = true;
                        tmpTexture.LoadImage(File.ReadAllBytes(filepath));
                        if (tmpTexture.width != 500 && tmpTexture.height != 500)
                        {
                            importLog.Add($"[WARNING] \"{filepath.Substring(folder.Length+1)}\" size is not valid. Atleast one of its sides must me 500 pixels in size.");
                            validImage = false;
                        }
                        if (tmpTexture.width > 500 || tmpTexture.height > 500)
                        {
                            importLog.Add($"[WARNING] \"{filepath.Substring(folder.Length+1)}\" size is not valid. None of it's sides can be greater than 500 pixels in size.");
                            validImage = false;
                        }
                        if (validImage)
                        {
                            tmpSprite = Sprite.Create(tmpTexture, new Rect(0.0f, 0.0f, tmpTexture.width, tmpTexture.height), new Vector2(0.5f, 0.5f));
                            /*
                            if (filepath.EndsWith("default.png"))
                                hasDefault = true;
                            */
                            if (filepath.Substring(folder.Length + 1).Substring(0, filepath.Substring(folder.Length + 1).LastIndexOf('.')) == "default")
                                hasDefault = true;
                            newPiece.sprites.Add(filepath.Substring(folder.Length+1).Substring(0, filepath.Substring(folder.Length+1).LastIndexOf('.')), tmpSprite);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log($"Error: {e}");
                        importLog.Add($"[ERROR] An error occurred while loading \"{filepath.Substring(folder.Length+1)}\".");
                    }
                }
            }

            /*** PIECE CHECKING ********************************************************************************************************************/

            // Has default image?
            if (!hasDefault)
            {
                importLog.Add("[ERROR] There is no valid \"default.png\" image. Remember that the name is case sensitive.");
                validBoard = false;
            }

            // ID is empty?
            if (newID.Length == 0) 
            {
                importLog.Add("[ERROR] The ID cannot be empty.");
                validBoard = false;
            }

            // ID in use?
            else if (pieceLibrary.ContainsKey(newID)) 
            {
                PieceElement otherPiece;
                pieceLibrary.TryGetValue(newID, out otherPiece);
                importLog.Add("[ERROR] The ID \"" + newID + "\" is already used by the piece with the name \"" + otherPiece.name + "\".");
                validBoard = false;
            }

            // Has a name?
            if (newPiece.name.Length == 0)
            {
                importLog.Add("[ERROR] The name cannot be empty.");
                validBoard = false;
            }

            // Valid BoardType?
            if (newPiece.boardType == BoardType.NULL) 
            {
                importLog.Add("[ERROR] The boardType is invalid. Remember it is case sensitive.");
                validBoard = false;
            }

            // Has moves?
            if (newPiece.moves.Count == 0) 
            {
                importLog.Add("[ERROR] No moves were saved.");
                validBoard = false;
            }

            /*** SAVING PIECE **********************************************************************************************************************/

            // ALL OK
            if (validBoard) 
            {
                pieceLibrary.Add(newID, newPiece);
                importLog.Add("[INFO] Piece added to the library successfully.");
            }
        }
        
        /*** LOADING BOARDS ************************************************************************************************************************/
        /*******************************************************************************************************************************************/
        importLog.Add("");
        importLog.Add("");
        importLog.Add("╔════════════════════════════╗");
        importLog.Add("║      IMPORTING BOARDS      ║");
        importLog.Add("╚════════════════════════════╝");
        importLog.Add("");
        folders = System.IO.Directory.GetDirectories(boardsFolder);
        foreach (string folder in folders)
        {
            /*** LOADING XML ***********************************************************************************************************************/
            string folderName = folder.Substring(piecesFolder.Length + 1);

            if (folderName.Equals("TemplateBoard")) continue;
            importLog.Add("");
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

            /*** LOADING METADATA ******************************************************************************************************************/
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
                newBoard.image = defaultBoardSprite;
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"metadata\" element in the file. Skipping board.");
                continue;
            }

            /*** LOADING PLAYERS *******************************************************************************************************************/
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

                    // Import information
                    PlayerImport newPlayer = new PlayerImport();
                    foreach (XmlNode playerInfo in player.ChildNodes)
                    {
                        switch (playerInfo.Name)
                        {
                            // TEAM /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

                            // COLOR ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                                    importLog.Add("[WARNING] A player's color is formated incorrectly.");
                                    continue;
                                }

                                break;

                            // DIRECTION ////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

                            // INTERFACE COLOR //////////////////////////////////////////////////////////////////////////////////////////////////////
                            case "interfacecolor":
                                try
                                {
                                    int
                                        r = int.Parse(playerInfo.InnerText.Substring(0, playerInfo.InnerText.IndexOf(','))),
                                        g = int.Parse(playerInfo.InnerText.Substring(playerInfo.InnerText.IndexOf(',') + 1, playerInfo.InnerText.LastIndexOf(',') - playerInfo.InnerText.IndexOf(',') - 1)),
                                        b = int.Parse(playerInfo.InnerText.Substring(playerInfo.InnerText.LastIndexOf(',') + 1));
                                    newPlayer.interfaceColor = new Color(r / 255.0f, g / 255.0f, b / 255.0f);
                                }
                                catch
                                {
                                    importLog.Add("[WARNING] A player's interfacecolor is formated incorrectly.");
                                    continue;
                                }
                                break;

                            // IMAGE VARIANT ////////////////////////////////////////////////////////////////////////////////////////////////////////
                            case "imagevariant":
                                try
                                {
                                    newPlayer.pieceVariant = playerInfo.InnerText;
                                }
                                catch
                                {
                                    importLog.Add("[WARNING] An error occurred while reading the imagevariant of a player.");
                                    continue;
                                }
                                break;

                            // ERROR ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                            default:
                                importLog.Add("[WARNING] \"" + playerInfo.Name + "\" is not a valid entry for a player.");
                                continue;
                        }
                    }
                    // Fill optional info
                    if (newPlayer.interfaceColor == null)
                    {
                        importLog.Add("[INFO] Couldn't find an interfacecolor value for a player. Defaulting to the same as color.");
                        newPlayer.interfaceColor = newPlayer.color;
                    }
                    if (newPlayer.pieceVariant == null)
                    {
                        importLog.Add("[INFO] Couldn't find a piecevariant value for a player. Defaulting to \"default\".");
                        newPlayer.pieceVariant = "default";
                    }


                    // Check all info is on place
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
           
            /*** LOADING PIECES ********************************************************************************************************************/
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
                        else
                        {
                            PieceElement tmp;
                            pieceLibrary.TryGetValue(newPiece.ID, out tmp);
                            if (tmp.boardType != newBoard.boardType)
                            {
                                importLog.Add("[WARNING] Specified piece is not of the same board type of the board. Skipping.");
                            }
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
                    importLog.Add("[ERROR] At least one piece entry has failed. Skipping board.");
                    continue;
                }
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"pieces\" element in the file. Skipping board.");
                continue;
            }
           
            /*** LOADING POSITION ******************************************************************************************************************/
            try
            {
                XmlNodeList positionRows = board.GetElementsByTagName("position")[0].ChildNodes;
                failed = false;
                switch (newBoard.boardType)
                {
                    case BoardType.Square2D:
                        // Import Strings ///////////////////////////////////////////////////////////////////////////////////////////////////////////
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
                        // Checking Strings /////////////////////////////////////////////////////////////////////////////////////////////////////////
                        foreach (List<string> row in initialPosition)
                            foreach (string cell in row)
                            {
                                if (cell[0] != '_')
                                {
                                    if (cell[0] != '0')
                                    {
                                        try
                                        {

                                            int owner = int.Parse(cell[0].ToString());
                                            if (owner - 1 >= newBoard.players.Count)
                                            {
                                                importLog.Add("[ERROR] Player owner in a cell is outside the player list.");
                                                failed = true;
                                                continue;
                                            }
                                            else if (!newBoard.pieceIDs.Keys.ToArray().Contains(cell[1]))
                                            {
                                                importLog.Add("[ERROR] Piece in a cell is not in the imported piece list.");
                                                failed = true;
                                                continue;
                                            }
                                        }
                                        catch
                                        {
                                            importLog.Add("[ERROR] Invalid character detected in a cell of the initial position.");
                                            failed = true;
                                            continue;
                                        }
                                    }
                                    if (cell.Contains(':'))
                                    {
                                        for (int i = cell.IndexOf(':'); i < cell.Length; i = i + 2)
                                        {
                                            if (cell[i] != ':')
                                            {
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
                        BoardSquare2D init = new BoardSquare2D();
                        init.board = new string[initialPosition.Count][];
                        for (int i = 0; i < initialPosition.Count; i++)
                            init.board[i] = initialPosition[i].ToArray();
                        newBoard.board = init;
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

            /*** LOADING CUSTOM COLORS *************************************************************************************************************/
            try
            {
                XmlNodeList customcolors = board.GetElementsByTagName("customcolors")[0].ChildNodes;
                bool failedboard = false;
                foreach(XmlNode list in customcolors)
                {
                    switch (list.Name)
                    {
                        case "colorlist":
                            foreach (XmlNode colorNode in list.ChildNodes)
                            {
                                if (colorNode.Name == "color")
                                {
                                    int
                                        r = int.Parse(colorNode.InnerText.Substring(0, colorNode.InnerText.IndexOf(','))),
                                        g = int.Parse(colorNode.InnerText.Substring(colorNode.InnerText.IndexOf(',') + 1, colorNode.InnerText.LastIndexOf(',') - colorNode.InnerText.IndexOf(',') - 1)),
                                        b = int.Parse(colorNode.InnerText.Substring(colorNode.InnerText.LastIndexOf(',') + 1));
                                    newBoard.colorList.Add(new Color(r / 255.0f, g / 255.0f, b / 255.0f));
                                }
                                else
                                {
                                    importLog.Add("[WARNING] There's something insithe the color list that is not a color. Ignoring.");
                                    continue;
                                }
                            }
                            break;
                        case "board":
                            if (newBoard.colorList.Count == 0)
                            {
                                importLog.Add("[ERROR] In the customcolor element, there must be a colorlist element and must be placed before the board element to be able to use it. Skipping board.");
                                failedboard = true;
                                break;
                            }
                            newBoard.hasCustomColorPlacement = true;
                            switch (newBoard.boardType)
                            {
                                case BoardType.Square2D:
                                    List<string> rowStrings = new List<string>();
                                    foreach(XmlNode row in list.ChildNodes)
                                    {
                                        if (row.Name == "row")
                                            rowStrings.Add(row.InnerText);
                                        else
                                        {
                                            importLog.Add("[ERROR] There is something in the customcolor board that is not a row. Skipping board.");
                                            failedboard = true;
                                            break;
                                        }
                                    }
                                    if (failedboard)
                                        break;
                                    ((BoardSquare2D)newBoard.board).colors = new int[rowStrings.Count][];
                                    for (int i = 0; i < rowStrings.Count; i++)
                                    {
                                        try
                                        {
                                            string auxstring = rowStrings[i];
                                            List<int> auxlist = new List<int>();
                                            for (int j = 0; auxstring.Contains(','); j++)
                                            {
                                                auxlist.Add(int.Parse(auxstring.Substring(0, auxstring.IndexOf(','))));
                                                auxstring = auxstring.Substring(auxstring.IndexOf(',') + 1);
                                            }
                                            auxlist.Add(int.Parse(auxstring));
                                            ((BoardSquare2D)newBoard.board).colors[i] = auxlist.ToArray();
                                        }
                                        catch
                                        {
                                            importLog.Add("[ERROR] The customcolor board is formated incorrectly. Skipping board.");
                                            failedboard = true;
                                            break;
                                        }
                                    }
                                    int sizecheck = ((BoardSquare2D)newBoard.board).colors[0].Length;
                                    foreach (int[] aux in ((BoardSquare2D)newBoard.board).colors)
                                    {
                                        if (aux.Length != sizecheck)
                                        {
                                            importLog.Add("[ERROR] Row size in customcolor board is inconsistent. Skipping board.");
                                            failedboard = true;
                                            break;
                                        }
                                        else
                                            foreach (int value in aux)
                                                if (value <= 0)
                                                {
                                                    importLog.Add("[ERROR] A value in the customcolor board is invalid. Skipping board.");
                                                    failedboard = true;
                                                    break;
                                                }
                                    }
                                    break;
                            }

                            break;
                        default:
                            importLog.Add($"[ERROR] \"{list.Name}\" is not a valid entry inside \"customcolors\". Ignoring.");
                            break;
                    }
                }
                if (failedboard) continue;
            }
            catch
            {
                importLog.Add("[WARNING] An error occourred while loading the custom colors. Maybe does not exist? Ignoring.");
            }

            /*** LOADING SPECIALS ******************************************************************************************************************/
            try
            {
                bool skipboard = false;
                XmlNodeList specialsList = board.GetElementsByTagName("specials")[0].ChildNodes;
                foreach (XmlNode move in specialsList)
                {
                    if (move.Name.CompareTo("move") != 0)
                    {
                        importLog.Add("[WARNING] There is something in \"specials\" that is not a move. Ignoring.");
                        continue;
                    }
                    
                    XmlNodeList moveElements = move.ChildNodes;
                    bool hasMovePiece = false, hasCondition = false, error = false;
                    SpecialMove resultMove = new SpecialMove();

                    int coordAmount = 0;
                    switch (newBoard.boardType)
                    {
                        case BoardType.Square2D:
                            coordAmount = 2;
                            break;
                    }
                    
                    foreach (XmlNode element in moveElements)
                    {
                        switch (element.Name)
                        {
                            case "check":
                                try
                                {
                                    string conditionText = element.InnerText;
                                    SpecialConditionCheckType condition = SpecialConditionCheckType.NULL;
                                    char piece = ' ';
                                    int player = 0;
                                    string cellText = "";

                                    List<int> cellAux = new List<int>();
                                    if (element.Attributes.Count == 1)
                                    {
                                        if (element.Attributes[0].Name == "cell")
                                            cellText = element.Attributes[0].InnerText;
                                        else
                                        {
                                            importLog.Add("[ERROR] The attribute must be named \"cell\" in a \"check\" element.");
                                            error = true;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        importLog.Add("[ERROR] The amount of attributes in a \"check\" element is incorrect.");
                                        error = true;
                                        continue;
                                    }

                                    while (cellText.Contains(','))
                                    {
                                        cellAux.Insert(0,int.Parse(cellText.Substring(0, cellText.IndexOf(',')))-1);
                                        cellText = cellText.Substring(cellText.IndexOf(',') + 1);
                                    }
                                    cellAux.Insert(0, int.Parse(cellText)-1);

                                    if (cellAux.Count != coordAmount)
                                    {
                                        importLog.Add("[ERROR] The amount of coordinates is incorrect for a board of this type.");
                                        error = true;
                                        continue;
                                    }

                                    if (conditionText.StartsWith("ispiece"))
                                    {
                                        piece = conditionText[7];
                                        conditionText = "ispiece";
                                    }
                                    if (conditionText.StartsWith("isplayer"))
                                    {
                                        player = int.Parse(conditionText[8].ToString());
                                        conditionText = "isplayer";
                                    }
                                    if (conditionText.StartsWith("isteam"))
                                    {
                                        player = int.Parse(conditionText.Substring(6));
                                        conditionText = "isteam";
                                    }

                                    switch (conditionText)
                                    {
                                        case "hasmoved":
                                            condition = SpecialConditionCheckType.HASMOVED;
                                            break;
                                        case "hasnotmoved":
                                            condition = SpecialConditionCheckType.HASNOTMOVED;
                                            break;
                                        case "ispiece":
                                            condition = SpecialConditionCheckType.ISPIECE;
                                            break;
                                        case "isplayer":
                                            condition = SpecialConditionCheckType.ISPLAYER;
                                            break;
                                        case "isteam":
                                            condition = SpecialConditionCheckType.ISTEAM;
                                            break;
                                        case "isempty":
                                            condition = SpecialConditionCheckType.ISEMPTY;
                                            break;
                                        case "isnotempty":
                                            condition = SpecialConditionCheckType.ISNOTEMPTY;
                                            break;
                                        case "isattacked":
                                            condition = SpecialConditionCheckType.ISATTACKED;
                                            break;
                                        case "issafe":
                                            condition = SpecialConditionCheckType.ISSAFE;
                                            break;
                                        default:
                                            importLog.Add("[ERROR] The condition of a \"check\" element is incorrect.");
                                            error = true;
                                            continue;
                                    }
                                    hasCondition = true;
                                    if (player == 0)
                                        resultMove.conditions.Add(new SpecialConditionCheck(condition, cellAux.ToArray(), piece));
                                    else
                                        resultMove.conditions.Add(new SpecialConditionCheck(condition, cellAux.ToArray(), player));
                                }
                                catch (Exception e)
                                {
                                    importLog.Add("[ERROR] An exception ocurred while checking a \"check\" element. Error: " + e);
                                }
                                break;
                            case "lastmove":
                                try
                                {
                                    string fromText, toText;
                                    List<int> aux = new List<int>();
                                    int[] from, to;
                                    int fromPlayer = 0;
                                    
                                    toText = element.InnerText.Substring(element.InnerText.IndexOf('-') + 1);
                                    fromText = element.InnerText.Substring(0, element.InnerText.IndexOf('-'));
                                    while (toText.Contains(','))
                                    {
                                        aux.Insert(0, int.Parse(toText.Substring(0, toText.IndexOf(',')))-1);
                                        toText = toText.Substring(toText.IndexOf(',') + 1);
                                    }
                                    aux.Insert(0, int.Parse(toText)-1);
                                    if (aux.Count == coordAmount)
                                        to = aux.ToArray();
                                    else
                                    {
                                        importLog.Add("[ERROR] The amount of coordinates is incorrect for a board of this type.");
                                        error = true;
                                        continue;
                                    }
                                    aux.Clear();
                                    
                                    while (fromText.Contains(','))
                                    {
                                        aux.Insert(0, int.Parse(fromText.Substring(0, fromText.IndexOf(',')))-1);
                                        fromText = fromText.Substring(fromText.IndexOf(',') + 1);
                                    }
                                    aux.Insert(0, int.Parse(fromText)-1);
                                    if (aux.Count == coordAmount)
                                        from = aux.ToArray();
                                    else
                                    {
                                        importLog.Add("[ERROR] The amount of coordinates is incorrect for a board of this type.");
                                        error = true;
                                        continue;
                                    }
                                    
                                    if (element.Attributes.Count == 1)
                                    {
                                        if (element.Attributes[0].Name == "player")
                                            fromPlayer = int.Parse(element.Attributes[0].InnerText);
                                        else
                                        {
                                            importLog.Add("[ERROR] The attribute must be named \"player\" in a \"lastmove\" element.");
                                            error = true;
                                            continue;
                                        }
                                    }
                                    else if (element.Attributes.Count != 0)
                                    {

                                        importLog.Add("[ERROR] The amount of attributes in a \"lastmove\" element is incorrect.");
                                        error = true;
                                        continue;
                                    }
                                    
                                    hasCondition = true;
                                    if (fromPlayer == 0)
                                        resultMove.conditions.Add(new SpecialConditionLastMove(from, to));
                                    else
                                        resultMove.conditions.Add(new SpecialConditionLastMove(from, to, fromPlayer));
                                }
                                catch (Exception e)
                                {
                                    importLog.Add("[ERROR] An exception ocurred while checking a \"lastmove\" element. Error: " + e);
                                }
                                break;
                            case "movepiece":
                                try
                                {
                                    string fromText2, toText2;
                                    List<int> aux2 = new List<int>();
                                    int[] from2, to2;

                                    toText2 = element.InnerText.Substring(element.InnerText.IndexOf('-') + 1);
                                    fromText2 = element.InnerText.Substring(0, element.InnerText.IndexOf('-'));
                                    while (toText2.Contains(','))
                                    {
                                        aux2.Insert(0, int.Parse(toText2.Substring(0, toText2.IndexOf(',')))-1);
                                        toText2 = toText2.Substring(toText2.IndexOf(',') + 1);
                                    }
                                    aux2.Insert(0, int.Parse(toText2)-1);
                                    if (aux2.Count == coordAmount)
                                        to2 = aux2.ToArray();
                                    else
                                    {
                                        importLog.Add("[ERROR] The amount of coordinates is incorrect for a board of this type.");
                                        error = true;
                                        continue;
                                    }
                                    
                                    aux2.Clear();
                                    while (fromText2.Contains(','))
                                    {
                                        aux2.Insert(0, int.Parse(fromText2.Substring(0, fromText2.IndexOf(',')))-1);
                                        fromText2 = fromText2.Substring(fromText2.IndexOf(',') + 1);
                                    }
                                    aux2.Insert(0, int.Parse(fromText2)-1);
                                    if (aux2.Count == coordAmount)
                                        from2 = aux2.ToArray();
                                    else
                                    {
                                        importLog.Add("[ERROR] The amount of coordinates is incorrect for a board of this type.");
                                        error = true;
                                        continue;
                                    }
                                    aux2.Clear();
                                    hasMovePiece = true;
                                    resultMove.results.Add(new SpecialResultMovePiece(from2, to2));
                                }
                                catch (Exception e)
                                {
                                    importLog.Add("[ERROR] An exception ocurred while checking a \"movepiece\" element. Error: " + e);
                                }
                                break;
                            case "createpiece":
                                try
                                {
                                    if (element.InnerText[1] == '-')
                                    {
                                        if (newBoard.pieceIDs.Keys.Contains(element.InnerText[0]))
                                        {
                                            string cellText3;
                                            List<int> aux4 = new List<int>();
                                            int[] cell3;

                                            cellText3 = element.InnerText.Substring(2);
                                            while (cellText3.Contains(','))
                                            {
                                                aux4.Insert(0, int.Parse(cellText3.Substring(0, cellText3.IndexOf(',')))-1);
                                                cellText3 = cellText3.Substring(cellText3.IndexOf(',') + 1);
                                            }
                                            aux4.Insert(0, int.Parse(cellText3)-1);
                                            if (aux4.Count == coordAmount)
                                            {
                                                cell3 = aux4.ToArray();
                                                resultMove.results.Add(new SpecialResultCreatePiece(cell3, element.InnerText[0]));
                                            }
                                            else
                                            {
                                                importLog.Add("[ERROR] The amount of coordinates is incorrect for a board of this type.");
                                                error = true;
                                                continue;
                                            }

                                        }
                                        else
                                        {
                                            importLog.Add("[ERROR] A \"createpiece\" tries to create a piece that is not in the piece list.");
                                            error = true;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        importLog.Add("[ERROR] A \"createpiece\" is formated incorrectly.");
                                        error = true;
                                        continue;
                                    }
                                }
                                catch (Exception e)
                                {
                                    importLog.Add("[ERROR] An exception ocurred while checking a \"createpiece\" element. Error: " + e);
                                }
                                break;
                            case "removepiece":
                                try
                                {
                                    string cellText2 = element.InnerText;
                                    List<int> aux3 = new List<int>();
                                    int[] cell2;
                                    
                                    while (cellText2.Contains(','))
                                    {
                                        aux3.Insert(0, int.Parse(cellText2.Substring(0, cellText2.IndexOf(',')))-1);
                                        cellText2 = cellText2.Substring(cellText2.IndexOf(',') + 1);
                                    }
                                    aux3.Insert(0, int.Parse(cellText2) - 1);
                                    if (aux3.Count == coordAmount)
                                    {
                                        cell2 = aux3.ToArray();
                                        resultMove.results.Add(new SpecialResultRemovePiece(cell2));
                                    }
                                    else
                                    {
                                        importLog.Add("[ERROR] The amount of coordinates is incorrect for a board of this type.");
                                        error = true;
                                        continue;
                                    }
                                }
                                catch (Exception e)
                                {
                                    importLog.Add("[ERROR] An exception ocurred while checking a \"removepiece\" element. Error: " + e);
                                }
                                break;
                            default:
                                importLog.Add($"[ERROR] \"{element.Name}\" is not a valid element for a special move.");
                                error = true;
                                continue;
                        }
                    }
                    
                    if (hasMovePiece && hasCondition && !error)
                    {
                        newBoard.specials.Add(resultMove);
                    }
                    else
                    {
                        skipboard = true;
                        break;
                    }
                    
                }
                if (skipboard) continue;
            }
            catch
            {
                importLog.Add("[ERROR] Could not find a \"specials\" element in the file. Skipping board.");
                continue;
            }
           
            /*** LOADING IMAGE *********************************************************************************************************************/
            bool isValid = true;
           
            try
            {
                Texture2D tmpTexture = new Texture2D(2, 2);
                tmpTexture.LoadImage(File.ReadAllBytes(folder + "/icon.png"));
                if (tmpTexture.width != 1280 || tmpTexture.height != 720)
                {
                    importLog.Add("[WARNING] Board image size is not valid. It must be 1280 pixels wide and 720 pixels tall exactly. Loading default image.");
                    throw new Exception();
                }
                newBoard.image = Sprite.Create(tmpTexture, new Rect(0.0f, 0.0f, tmpTexture.width, tmpTexture.height), new Vector2(0.5f, 0.5f));
            }
            catch
            {
                newBoard.image = defaultBoardSprite;
            }
           
            /*** BOARD CHECKING ********************************************************************************************************************/
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
           
            /*** SAVING BOARD **********************************************************************************************************************/
            if (isValid)
            {
                boardLibrary.Add(newName, newBoard);
                importLog.Add("[INFO] Board added to the library successfully.");
            }

        }

        /*** LAST INIT *****************************************************************************************************************************/
        /*******************************************************************************************************************************************/
        DateTime time = DateTime.Now;
        string logsRoute = "";

        if (Application.isEditor)
            logsRoute = "./Assets/ExternalLogs";
        else
            logsRoute = "./Logs/";

        if (!System.IO.Directory.Exists(logsRoute))
            System.IO.Directory.CreateDirectory(logsRoute);
        
        System.IO.File.WriteAllLines(String.Format("{0}/LOG_{1,4:0000}_{2,2:00}_{3,2:00}_{4,2:00}_{5,2:00}_{6,2:00}.txt", logsRoute, time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second), importLog);
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
