using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour{
    public static InventoryManager instance;
    public int maxStackedItems = 10;
    public GameObject InventoryItemPrefab;
    public InventorySlot[] inventorySlots;
    int selectedSlot = -1;
    public RectTransform selectedSlotTransform;
    public bool open = false;
    public GameObject otherInventoryOpen = null;
    public GameObject inventoryGO;
    public Transform currentSelectedItem = null;
    public GameObject inventoryGroup;
    private GameObject hoverableInfo;
    private UnityEngine.UI.Image currentHeldItemImage;
    public Material[] materials;

    // starting items
    public List<Item> startingItems;

    public void Awake(){
        instance = this;
    }
    private void Start(){
        hoverableInfo = GameObject.Find("InventoryCanvas").transform.Find("HoverableInfoContainer").gameObject;
        currentHeldItemImage = GameObject.Find("InventoryCanvas").transform.Find("CurrentSelectedItem").GetComponent<UnityEngine.UI.Image>();
        ChangeSelectedSlot(0);
        inventoryGO.SetActive(false);
        
        foreach (Item item in startingItems){
            AddItem(item);
        }
    }

    private void Update(){
        if(Input.inputString != null){
            bool isNumber = int.TryParse(Input.inputString, out int number);
            if(isNumber && number > 0 && number < 10){
                ChangeSelectedSlot(number - 1);
            }
        }
        hoverableInfo.transform.position = Input.mousePosition;

        // make the currenly selected item follow the mouses position
        if(currentSelectedItem){
            currentSelectedItem.position = Input.mousePosition;
        }

        if(GetSelectedItem(false) != null){
            if (GetSelectedItem(false).Image != currentHeldItemImage.sprite){
                currentHeldItemImage.sprite = GetSelectedItem(false).Image;
                if(GameObject.FindWithTag("Player")){
                    ChangeHeldItemMesh();
                }
            } 
        } else {
            GameObject.FindWithTag("Player").transform.GetChild(0).GetComponentInChildren<MeshFilter>().mesh.Clear();
            currentHeldItemImage.sprite = null;
        }
    }

    private void ChangeHeldItemMesh()
    {
        Transform playerItemTransform = GameObject.FindWithTag("Player").transform.GetChild(0).GetChild(0);

        if (GetSelectedItem(false) is BlockItem blockItem)
        {
            MeshData meshData = BlockHelper.GetFullMeshData(new MeshData(true), blockItem.blockType);

            // Get the MeshFilter and MeshRenderer components
            MeshFilter meshFilter = playerItemTransform.GetComponentInChildren<MeshFilter>();
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();
            Mesh mesh = meshFilter.mesh;

            // Set up the mesh
            mesh.Clear();
            mesh.subMeshCount = 2;
            mesh.vertices = meshData.vertices.Concat(meshData.waterMesh.vertices).ToArray();
            mesh.SetTriangles(meshData.triangles.ToArray(), 0);  // Main block
            mesh.SetTriangles(meshData.waterMesh.triangles.Select(val => val + meshData.vertices.Count).ToArray(), 1);  // Water block
            mesh.uv = meshData.uv.Concat(meshData.waterMesh.uv).ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // Adjust the position and scale for the held item
            playerItemTransform.localPosition = new Vector3(0.772f, -0.52f, 0.357f);
            playerItemTransform.localRotation = Quaternion.Euler(0, 0, 0);
            playerItemTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            // Assign materials
            Material[] materials = meshRenderer.materials;
            materials[0] = this.materials[0];  // Block material
            meshRenderer.materials = materials;
        }
        else
        {
            // Handle non-block items (tools, etc.)
            ExtrudeSpriteMesh extrudeSpriteMesh = playerItemTransform.GetComponentInChildren<ExtrudeSpriteMesh>();
            extrudeSpriteMesh.sprite = currentHeldItemImage.sprite;
            extrudeSpriteMesh.CreateExtrudedMesh();

            MeshFilter meshFilter = playerItemTransform.GetComponentInChildren<MeshFilter>();
            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if(GetSelectedItem(false) is ToolItem toolItem){
                // Assign materials
                Material[] materials = meshRenderer.materials;
                this.materials[2].SetTexture("_BaseMap", toolItem.Image.texture);
                materials[0] = this.materials[2];  // Non-block material
                meshRenderer.materials = materials;
            } else {
                // Assign materials
                Material[] materials = meshRenderer.materials;
                materials[0] = this.materials[1];  // Non-block material
                meshRenderer.materials = materials;
            }
            // Adjust position and scale for non-block items
            playerItemTransform.localPosition = new Vector3(0.322f, -0.288f, 0.193f);
            playerItemTransform.localRotation = Quaternion.Euler(-11.571f, -73.275f, 27.262f);
            playerItemTransform.localScale = new Vector3(0.02f, 0.02f, 0.015f);
        }
    }


    public void ToggleInventoryOpen(){
        if(open) {
            Cursor.lockState = CursorLockMode.Locked;
            if(otherInventoryOpen){
                otherInventoryOpen.SetActive(false);
                otherInventoryOpen = null; 
                inventoryGroup.SetActive(true);
            }
            hoverableInfo.SetActive(false);
        } else {
            Cursor.lockState = CursorLockMode.None;
        }
        open = !open;
        GameObject.FindWithTag("Player").GetComponent<PlayerInput>().inventoryOpen = open;
        inventoryGO.SetActive(open);
    }
    void ChangeSelectedSlot(int newValue){
        selectedSlotTransform.anchoredPosition = new Vector2(-720 + (newValue * 180), selectedSlotTransform.anchoredPosition.y);
        selectedSlot = newValue;
    }
    public bool AddItem(Item item){
        for (int i = 0; i < inventorySlots.Length; i++){
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot != null && itemInSlot.item == item && itemInSlot.count < maxStackedItems && itemInSlot.item.IsStackable){
                itemInSlot.count++;
                itemInSlot.RefreshCount();
                return true;
            }
        }
        for (int i = 0; i < inventorySlots.Length; i++){
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if(itemInSlot == null){
                SpawnNewItem(item, slot);
                return true;
            }
        }
        return false; // if no free slot
    }
    // spawns a new InventoryItem. If slot is given, assigns it to that slot. If not, sets as currentSelectedItem
    public InventoryItem SpawnNewItem(Item item, InventorySlot slot = null)
    {
        GameObject newItemGo = Instantiate(InventoryItemPrefab);
        InventoryItem inventoryItem = newItemGo.GetComponent<InventoryItem>();
        inventoryItem.InitializeItem(item);
        if(slot){
            newItemGo.transform.SetParent(slot.transform);            
        } else {
            currentSelectedItem = inventoryItem.transform;
        }
        return inventoryItem;
    }

    public Item GetSelectedItem(bool use) {
        InventorySlot slot = inventorySlots[selectedSlot];
        InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
        if(itemInSlot != null){
            Item item = itemInSlot.item;
            if(use){
                itemInSlot.count--;
                if(itemInSlot.count <= 0){
                    Destroy(itemInSlot.gameObject);
                } else {
                    itemInSlot.RefreshCount();
                }
            }
            return item;
        }
        return null;
    }
}


