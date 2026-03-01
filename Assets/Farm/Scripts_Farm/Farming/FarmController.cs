using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

[System.Serializable]
public class TileState
{
    public int x, y, z;
    public int state; // 0:đất đào đc nhưng chưa đào 1: Hố trống, 2: Cây 
    public string plantName;
    public int stage;
    public bool isWatered;
}
[System.Serializable]
public class SaveWrapper
{
    public List<TileState> allTiles = new List<TileState>();
}
    public class FarmController : MonoBehaviour
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

    public P_Action toolScript;

    bool isFarm = false;

    public PlantData selectedPlant;
    bool isStandingOnHole = false;
    [Header("Harvest UI")]
    public GameObject harvestPopupPrefab;


    [System.Serializable]
    public class PlantedCrop
    {
        public Vector3Int position; //vị trí cây 
        public PlantData plantInfo; //thông tin cây 
        public int currentStage = 0;
        public bool isWatered = false;
        public Coroutine growthCoroutine;  // Coroutine phát triển của cây trồng 
    }

    // danh sách cây trồng đang đc trồng trên bản đồ 
    private Dictionary<Vector3Int, PlantedCrop> activeCrops = new Dictionary<Vector3Int, PlantedCrop>();
    public ItemPickup pickup;

    [SerializeField] HotbarUI hotbar;

    private void Awake()
    {
        LoadGameData(PlayerRuntime.Instance.Player.Wrapper);
        Debug.Log("Load");
    }
    private void Start()
    {
    }
    private void Update()
    {
        if (toolScript == null)
        {
            toolScript = GameObject.Find("Player").GetComponent<P_Action>();
        }
        //UpdateSelectorTile();
        //CheckHole();
        //HandleFarmAction();
    }

    public void CheckHole()
    {        
        // Kiểm tra xem ô hiện tại có phải là hố không, khi soil là null 
        if (tm_Hole.HasTile(currentTargetCell) && tm_Soil.GetTile(currentTargetCell) == null)
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
        //if (seedMenuPanel != null)
        //{
        //    if (seedMenuPanel.activeSelf)
        //    {
        //        toolScript.isAction = false;
        //    }
        //    else
        //    {
        //        toolScript.isAction = true;
        //    }
        //}
    }
    public void UpdateSelectorTile(bool isFarm, Transform playerPos)
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
            Vector3Int playerCell = tm_Soil.WorldToCell(playerPos.position);

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

    void HandleFarmAction()
    {
        ItemRuntime item = hotbar.GetSelectedItem();
        if (item == null) return;

        if (seedMenuPanel.activeSelf) return;

        if (item.itemType == ItemType.Shovel && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Đào");
            Vector3Int cellPos = currentTargetCell;
            Debug.Log("CellPos: " + cellPos);
            TileBase currentTileBase = tm_Soil.GetTile(cellPos);
            if (currentTileBase == tb_Soil)
            {
                //Xóa tile soil
                tm_Soil.SetTile(cellPos, null);
                Debug.Log("Đã chuyển từ chưa đào hố sang đã có hố");
            }

        }
        else if (item.itemType == ItemType.WateringCan && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Tưới nước");
            Vector3Int cellPos = currentTargetCell;
            //kiểm tra xem ô này có cây không 
            if (activeCrops.ContainsKey(cellPos))
            {
                //lấy thông tin của cái cây đó (truyền vị trí vô thì sẽ biết đc đó là cây gì) 
                PlantedCrop crop = activeCrops[cellPos];
                if (!crop.isWatered && crop.currentStage < crop.plantInfo.growthStages.Count - 1) // nếu cây chưa tưới nước và chưa chín 
                {
                    //đánh dấu đc tưới nước 
                    crop.isWatered = true;

                    // Đổi hình hố đất thành đất ướt 
                    tm_HoleSeed.SetTile(cellPos, null);
                    //dùng này để tránh kẹt coroutin - khi coroutine cũ chưa chạy xong sẽ dễ kẹt nên tắt đi 
                    if (crop.growthCoroutine != null)
                    {
                        StopCoroutine(crop.growthCoroutine);
                    }
                    // Chạy coroutin đếm ngược thời gian lớn 
                    crop.growthCoroutine = StartCoroutine(GrowCropRoutine(crop));

                    Debug.Log("Tưới nước thành công! Đồng hồ thời gian bắt đầu chạy.");
                }
                else if (crop.currentStage >= crop.plantInfo.growthStages.Count - 1)
                {
                    Debug.Log("Cây đã chín rồi, không cần tưới nữa, mau thu hoạch thôi!");
                }
                else
                {
                    Debug.Log("Đất vẫn còn ướt, không cần tưới thêm!");
                }
            }
            else
            {
                Debug.Log("Ô này không có hạt giống, tưới tốn nước!");
            }
        }

    }
    public void Shovel()
    {
        ItemRuntime item = hotbar.GetSelectedItem();
        if (item == null) return;

        if (seedMenuPanel.activeSelf) return;
        if (item.itemType == ItemType.Shovel && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Đào");
            Vector3Int cellPos = currentTargetCell;
            Debug.Log("CellPos: " + cellPos);
            TileBase currentTileBase = tm_Soil.GetTile(cellPos);
            if (currentTileBase == tb_Soil)
            {
                //Xóa tile soil
                tm_Soil.SetTile(cellPos, null);
                Debug.Log("Đã chuyển từ chưa đào hố sang đã có hố");
            }

        }
    }
    public void Water()
    {
        ItemRuntime item = hotbar.GetSelectedItem();
        if (item == null) return;

        if (seedMenuPanel.activeSelf) return;
        if (item.itemType == ItemType.WateringCan && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Tưới nước");
            Vector3Int cellPos = currentTargetCell;
            //kiểm tra xem ô này có cây không 
            if (activeCrops.ContainsKey(cellPos))
            {
                //lấy thông tin của cái cây đó (truyền vị trí vô thì sẽ biết đc đó là cây gì) 
                PlantedCrop crop = activeCrops[cellPos];
                if (!crop.isWatered && crop.currentStage < crop.plantInfo.growthStages.Count - 1) // nếu cây chưa tưới nước và chưa chín 
                {
                    //đánh dấu đc tưới nước 
                    crop.isWatered = true;

                    // Đổi hình hố đất thành đất ướt 
                    tm_HoleSeed.SetTile(cellPos, null);
                    //dùng này để tránh kẹt coroutin - khi coroutine cũ chưa chạy xong sẽ dễ kẹt nên tắt đi 
                    if (crop.growthCoroutine != null)
                    {
                        StopCoroutine(crop.growthCoroutine);
                    }
                    // Chạy coroutin đếm ngược thời gian lớn 
                    crop.growthCoroutine = StartCoroutine(GrowCropRoutine(crop));

                    Debug.Log("Tưới nước thành công! Đồng hồ thời gian bắt đầu chạy.");
                }
                else if (crop.currentStage >= crop.plantInfo.growthStages.Count - 1)
                {
                    Debug.Log("Cây đã chín rồi, không cần tưới nữa, mau thu hoạch thôi!");
                }
                else
                {
                    Debug.Log("Đất vẫn còn ướt, không cần tưới thêm!");
                }
            }
            else
            {
                Debug.Log("Ô này không có hạt giống, tưới tốn nước!");
            }
        }
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
            if (!activeCrops.ContainsKey(cellPos)) // nếu không phải là activecrops có pos đc lưu trong dic thì lưu cái mới vô 
            {
                PlantedCrop newCrop = new PlantedCrop();
                newCrop.position = cellPos;
                newCrop.plantInfo = plantToPlant;
                newCrop.currentStage = 0;
                newCrop.isWatered = false; // Mới gieo chưa có nước

                activeCrops.Add(cellPos, newCrop);
            }

            Debug.Log($"Đã gieo thành công: {plantToPlant.seedName} tại {cellPos}");

            // Đóng menu sau khi gieo xong
            if (seedMenuPanel != null) seedMenuPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Không tìm thấy hố tại vị trí này để gieo");
        }
    }
    private IEnumerator GrowCropRoutine(PlantedCrop crop)
    {
        //lấy giai đoạn hiện tại của cây trồng 
        float timeToWait = crop.plantInfo.growthStages[crop.currentStage].growthTimeInSeconds;

        // chờ đúng số giây ở trong giai đoạn đó 
        yield return new WaitForSeconds(timeToWait);

        //tăng giai đoạn lên 1level 
        crop.currentStage++;

        // cập nhật tile mới theo giai đoạn 
        TileBase nextStageTile = crop.plantInfo.growthStages[crop.currentStage].stageTile;
        tm_Seed.SetTile(crop.position, nextStageTile);

        // cây cạn nước để tưới tiếp cho giai đoạn sau 
        crop.isWatered = false;

        // đặt lại cái đất cũ che đi cái nước 
        tm_HoleSeed.SetTile(crop.position, tb_HoleSeed);

        Debug.Log($"Cây ở {crop.position} đã lớn lên level {crop.currentStage}. Cần tưới nước tiếp!");

        // nếu cây trồng chín 
        if (crop.currentStage == crop.plantInfo.growthStages.Count - 1)
        {
            Debug.Log($"Cây ở {crop.position} ĐÃ CHÍN TỰ ĐỘNG!");
            Vector3 worldPos = tm_Seed.GetCellCenterWorld(crop.position);
            GameObject popup = Instantiate(harvestPopupPrefab, worldPos, Quaternion.identity);
            popup.GetComponent<HarvestPopupUI>().Setup(crop.position, this);
        }
    }
    public void HarvestCrop(Vector3Int pos)
    {
        // Kiểm tra xem ô đất này có cây đang trồng trong sổ không
        if (activeCrops.ContainsKey(pos))
        {
            PlantedCrop crop = activeCrops[pos];
            PlantData data = crop.plantInfo;

            // nhét đồ vào túi 
            if (data.harvestItemData != null)
            {
                ItemRuntime newItem = ItemRuntime.FromItem(data.harvestItemData);
                newItem.quantity = data.harvestQuantity; // Lấy số lượng thu hoạch từ PlantData - mặc định là 1 

                bool isAdded = false;
                var playerInventory = PlayerRuntime.Instance.Player.Inventory;


                List<ItemRuntime> items = playerInventory[InventoryContainerType.Farm].items;
                foreach (var invItem in items)
                {
                    if (invItem.itemID == newItem.itemID && invItem.isStackable)
                    {
                        invItem.quantity += newItem.quantity;
                        isAdded = true;
                        break;
                    }
                }
                if (!isAdded)
                {
                    PlayerRuntime.Instance.Player.Inventory[InventoryContainerType.Farm].items.Add(newItem);
                }



                Debug.Log($"Đã thu hoạch: {data.harvestItemName}");
            }

            //setup lại nông trại, tất cả đất về bình thường trừ cái seed phải là null 
            tm_Soil.SetTile(pos, tb_Soil);
            tm_Hole.SetTile(pos, tb_Hole);
            tm_Seed.SetTile(pos, null);
            tm_HoleSeed.SetTile(pos, tb_HoleSeed);
        }
    }
    public void SaveGameData()
    {        
        SaveWrapper wrapper = new SaveWrapper();
        // duyệt vị trí theo cái vùng đấy baseground (nằm ở đáy) để lấy tất cả vị trí có tile, nếu có cây thì lưu thông tin cây
        foreach (var pos in tm_GroundInvisible.cellBounds.allPositionsWithin)
        {
            if (!tm_GroundInvisible.HasTile(pos)) continue; //nếu dưới baseground k có tile thì khỏi lưu vì đó k phải là ô đất trồng được

            if (activeCrops.ContainsKey(pos)) // Trường hợp ô ĐANG CÓ CÂY đang được trồng 
            {
                //thêm vào list wrapper tất cả thông tin cần thiết để lưu lại trạng thái của ô đất đó, bao gồm: vị trí, trạng thái (có cây hay không), tên cây, giai đoạn phát triển, có được tưới nước hay không
                wrapper.allTiles.Add(new TileState
                {
                    x = pos.x,
                    y = pos.y,
                    z = pos.z,
                    state = 2, // state 2 là có cây
                    plantName = activeCrops[pos].plantInfo.seedName,
                    stage = activeCrops[pos].currentStage, //stage hiện tại của cây 
                    isWatered = activeCrops[pos].isWatered // trạng thái đã tưới hay chưa 
                });
            }
            else if (tm_Soil.GetTile(pos) == null) // Trường hợp HỐ TRỐNG
            {
                wrapper.allTiles.Add(new TileState
                {
                    x = pos.x,
                    y = pos.y,
                    z = pos.z,
                    plantName = "none",
                    state = 1
                });
            }
            else if(tm_Soil.GetTile(pos) == tb_Soil) // Trường hợp ĐẤT CHƯA ĐƯỢC ĐÀO HỐ
            {
                wrapper.allTiles.Add(new TileState
                {
                    x = pos.x,
                    y = pos.y,
                    z = pos.z,
                    plantName = "none",
                    state = 0
                });
            }
        }


        ///Sau khi đã có wrapper chứa tất cả thông tin cần thiết, ta sẽ lưu wrapper này vào PlayerRuntime để có thể truy cập lại sau khi load game
        PlayerRuntime.Instance.Player.Wrapper = wrapper.allTiles;

    }
    public void LoadGameData(List<TileState> savedTiles)
    {
        // Duyệt qua tất cả tile đã lưu trong savedTiles
        foreach (var tile in savedTiles)
        {
            Vector3Int pos = new Vector3Int(tile.x, tile.y, tile.z);
            if (tile.state == 0) // Đất chưa được đào hố
            {
                tm_Soil.SetTile(pos, tb_Soil);
                tm_Hole.SetTile(pos, tb_Hole);
                tm_Seed.SetTile(pos, null);
                tm_HoleSeed.SetTile(pos, null);
            }
            else if (tile.state == 1) // Hố trống
            {
                tm_Soil.SetTile(pos, null);
                tm_Hole.SetTile(pos, tb_Hole);
                tm_Seed.SetTile(pos, null);
                tm_HoleSeed.SetTile(pos, null);
            }
            else if (tile.state == 2) // Có cây
            {
                PlantData plantData = Resources.Load<PlantData>($"Plants/{tile.plantName}");
                if (plantData != null)
                {
                    PlantedCrop crop = new PlantedCrop
                    {
                        position = pos,
                        plantInfo = plantData,
                        currentStage = tile.stage,
                        isWatered = tile.isWatered
                    };
                    activeCrops[pos] = crop;
                    // Cập nhật Tilemap dựa trên giai đoạn phát triển của cây
                    TileBase stageTile = plantData.growthStages[tile.stage].stageTile;
                    tm_Seed.SetTile(pos, stageTile);
                    // Nếu cây đã được tưới nước ở giai đoạn trước, đặt tile đất ướt
                    if (tile.isWatered)
                    {
                        tm_HoleSeed.SetTile(pos, tb_HoleSeed);
                    }
                    else
                    {
                        tm_HoleSeed.SetTile(pos, null);
                    }
                }
                else
                {
                    Debug.LogWarning($"Không tìm thấy PlantData cho cây: {tile.plantName}");
                }
            }
        }
    }
}

