using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class FarmController_TA : MonoBehaviour
{
    [Header("Tilemap")]
    public Tilemap tm_Soil;
    public Tilemap tm_Hole;
    public Tilemap tm_Seed;
    public Tilemap tm_HoleSeed;
    public Tilemap tm_WaterSoil;
    public Tilemap tm_BaseGround;
    public Tilemap tm_GroundInvisible;

    public TileBase tb_Soil;
    public TileBase tb_Hole;
    public TileBase tb_Seed;
    public TileBase tb_HoleSeed;
    public TileBase tb_WaterSoil;
    public TileBase tb_BaseGround;
    public TileBase tb_GroundInvisible;

    [Header("Selector")]
    public Tilemap tm_Selector;
    public TileBase tb_Selector;
    private Vector3Int currentTargetCell;
    private Vector3Int previousTargetCell;

    [Header("UI Shop")]
    public GameObject seedMenuPanel;

    public P_UseTools toolScript;

    bool isFarm = false;

    public PlantData selectedPlant;
    bool isStandingOnHole = false;
    private void Start()
    {
        toolScript = GetComponent<P_UseTools>();
    }
    private void Update()
    {
        UpdateSelectorTile();
        // Kiểm tra xem ô hiện tại có phải là hố không, khi soil là null 
        if(tm_Hole.HasTile(currentTargetCell) && tm_Soil.GetTile(currentTargetCell) == null)
        {
            // Nếu có hố và không có soil thì coi như đang đứng trên hố
            isStandingOnHole = true;
        }
        else
        {
            // Nếu không có hố hoặc có soil thì coi như không đứng trên hố
            isStandingOnHole = false;
        }
        if (isStandingOnHole)
        {
            // Nếu đứng trên hố và chưa mở menu thì hiện lên
            if (!seedMenuPanel.activeSelf)
            {
                ShowSeedMenu(true);
            }
        }
        else
        {
            // Nếu đi ra khỏi hố thì tự đóng menu 
            if (seedMenuPanel.activeSelf)
            {
                ShowSeedMenu(false);
            }
        }
        if (seedMenuPanel != null)
        {
            if (seedMenuPanel.activeSelf)
            {
                toolScript.enabled = false;
            }
            else
            {
                toolScript.enabled = true;
            }
        }
        HandleFarmAction();
    }

    void UpdateSelectorTile()
    {
        if (isFarm == false)
        {
            if (previousTargetCell != null)
            {
                tm_Selector.SetTile(previousTargetCell, null);
            }
            return;
        }
        else
        {
            // Lấy ô player đang đứng
            Vector3Int playerCell = tm_Soil.WorldToCell(transform.position);

            // Ô target = ô player 
            currentTargetCell = playerCell;

            if (tm_GroundInvisible.HasTile(currentTargetCell))
            {
                // Nếu ô target thay đổi, vẽ lại
                if (currentTargetCell != previousTargetCell)
                {
                    tm_Selector.SetTile(previousTargetCell, null); // Xóa ô selector cũ
                    tm_Selector.SetTile(currentTargetCell, tb_Selector); // Vẽ ô selector mới
                    previousTargetCell = currentTargetCell; // Lưu lại vị trí
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Farm"))
        {
            isFarm = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Farm"))
        {
            isFarm = false;
            // Xóa ngay lập tức khi vừa bước chân ra khỏi ruộng
            tm_Selector.SetTile(previousTargetCell, null);
        }
    }

    void HandleFarmAction()
    {
        if (seedMenuPanel.activeSelf) return;
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Vừa click chuột! Tool hiện tại đang là số: " + toolScript.numberTool);
        }
        if (toolScript.numberTool == 3 &&  Input.GetMouseButtonDown(0))
        {
            Debug.Log("Đào");
            Vector3Int cellPos = currentTargetCell;
            Debug.Log("CellPos: " + cellPos);
            TileBase currentTileBase = tm_Soil.GetTile(cellPos);
            if(currentTileBase == tb_Soil)
            {
                //Xóa tile soil
                tm_Soil.SetTile(cellPos, null);
                Debug.Log("Đã chuyển từ chưa đào hố sang đã có hố");
            }
             
        }
        //if(toolScript.numberTool == 2 && Input.GetMouseButtonDown(0))
        //{
        //    if (selectedPlant == null || selectedPlant.growthStages.Count == 0)
        //    {
        //        Debug.Log("Chưa chọn hạt giống hoặc dữ liệu cây bị trống!");
        //        return;
        //    }
        //    Debug.Log("Gieo hạt");
        //    Vector3Int cellPos = currentTargetCell;
        //    Debug.Log("CellPos: " + cellPos);
        //    TileBase currentTileBase = tm_Hole.GetTile(cellPos);
        //    if (currentTileBase != tb_Hole)
        //    {
        //        Debug.Log("Không thể gieo hạt ở đây, vì không phải là đất đã được đào");
        //    }
        //    else if (currentTileBase == tb_Hole)
        //    {
        //        // Xóa hole để lộ ra chỗ gieo hạt 
        //        tm_Hole.SetTile(cellPos, null);
        //        // Lấy Stage đầu tiên (Hạt giống vừa gieo)
        //        TileBase seedTile = selectedPlant.growthStages[0].stageTile;
        //        tm_Seed.SetTile(cellPos, seedTile);
        //        Debug.Log("Đã gieo hạt thành công!");
        //    }
        //    else
        //    {
        //        Debug.Log("Không thể gieo hạt ở đây, vì đã có hạt hoặc đất chưa được đào");
        //    }
        //}

    }
    

    void ShowSeedMenu(bool isShow)
    {
        seedMenuPanel.SetActive(isShow);
    }
    //hàm hỗ trợ  script choose seed
    public Vector3Int GetCurrentTargetCell()
    {
        return currentTargetCell;
    }
    public void PlantSeedAtCurrentPos(PlantData plantToPlant)
    {
        if (plantToPlant == null || plantToPlant.growthStages.Count == 0)
        {
            Debug.LogWarning("Dữ liệu cây trồng bị trống!");
            return;
        }

        Vector3Int cellPos = GetCurrentTargetCell();
        TileBase currentHoleTile = tm_Hole.GetTile(cellPos);

        // Kiểm tra xem tại ô đó có hố để gieo không
        if (currentHoleTile == tb_Hole)
        {
            //Xóa hố đất
            tm_Hole.SetTile(cellPos, null);

            // Đặt hạt giống (Stage 0) xuống Tilemap Seed
            TileBase seedTile = plantToPlant.growthStages[0].stageTile;
            tm_Seed.SetTile(cellPos, seedTile);

            Debug.Log($"Đã gieo thành công: {plantToPlant.seedName} tại {cellPos}");

            // Đóng menu sau khi gieo xong
            if (seedMenuPanel != null) seedMenuPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Không tìm thấy hố tại vị trí này để gieo");
        }
    }
}
