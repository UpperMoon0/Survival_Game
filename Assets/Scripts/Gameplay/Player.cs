using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject slotPrefab;
    public GameObject ingredientPrefab;
    public GameObject recipeSlotPrefab;

    private bool isGrounded = true;
    private int tab = 0;
    private int currentRecipeID = 0;
    private float camRotation = 0f;
    private float maxCamRotation = 90f;
    private float rotationSpeed = 5f;
    private float jumpForce = 5f;
    private float pickupDistance = 3f;
    private Rigidbody rb;
    private GameObject cam;
    private GameObject UICam;
    private GameObject canvas;
    private GameObject inventoryUI;
    private GameObject inventoryTab;
    private GameObject craftingTab;
    private GameObject inventoryTabButton;
    private GameObject craftingTabButton;
    private GameObject itemPreview;
    private GameObject craftButton;
    private GameObject itemImage;
    private GameObject itemName;
    private GameObject itemDescription;
    private GameObject recipePanel;
    private GameObject[] slotsUI;
    private Inventory inv;
    private Inventory cursorInv;
    private Recipe currentRecipe;

    public int CurrentRecipeID { get => currentRecipeID; set => currentRecipeID = value; }

    private void Awake()
    {
        // Objects initialization
        UICam = transform.GetChild(0).gameObject;
        cam = transform.GetChild(1).gameObject;
        canvas = transform.GetChild(2).gameObject;
        inventoryUI = canvas.transform.GetChild(0).gameObject;
        inventoryTab = inventoryUI.transform.GetChild(0).gameObject;
        craftingTab = inventoryUI.transform.GetChild(1).gameObject;
        inventoryTabButton = inventoryUI.transform.GetChild(2).gameObject;
        craftingTabButton = inventoryUI.transform.GetChild(3).gameObject;
        itemPreview = craftingTab.transform.GetChild(0).gameObject;
        itemImage = itemPreview.transform.GetChild(0).gameObject;
        itemName = itemPreview.transform.GetChild(1).gameObject;
        itemDescription = itemPreview.transform.GetChild(2).transform.GetChild(0).transform.GetChild(0).gameObject;
        craftButton = itemPreview.transform.GetChild(3).gameObject;
        recipePanel = craftingTab.transform.GetChild(1).transform.GetChild(0).transform.GetChild(0).transform.GetChild(0).gameObject;

        InitInventory();

        // Freeze the player's rotation
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
    }
    private void Update()
    {
        UpdateInventory();
        UpdateMovement();
        CheckForPickup();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Collider>() != null)
        {
            isGrounded = true;
        }
    }
    private void UpdateMovement()
    {
        // Move the player
        Vector3 moveDirection = Vector3.zero;
        if (isGrounded)
        {
            if (Input.GetKey(KeyCode.W))
            {
                moveDirection += transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                moveDirection -= transform.forward;
            }
            if (Input.GetKey(KeyCode.A))
            {
                moveDirection -= transform.right;
            }
            if (Input.GetKey(KeyCode.D))
            {
                moveDirection += transform.right;
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
                isGrounded = false;
            }

            rb.velocity = new Vector3(moveDirection.x * 10, rb.velocity.y, moveDirection.z * 10);
        }

        // Rotate the player
        if (!inventoryUI.activeSelf)
        {
            transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * rotationSpeed);
            camRotation += Input.GetAxis("Mouse Y") * -rotationSpeed;
            camRotation = Mathf.Clamp(camRotation, -maxCamRotation, maxCamRotation);
            cam.transform.localEulerAngles = new Vector3(camRotation, 0, 0);
        }
    }
    private void InitInventory()
    {
        // Set the the initial state of the inventory UI
        inv = new Inventory(36);
        cursorInv = new Inventory(1);
        inventoryUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ChangeTab(0);
        inventoryTabButton.GetComponent<Button>().onClick.AddListener(() => OnInventoryTabClick());
        craftingTabButton.GetComponent<Button>().onClick.AddListener(() => OnCraftingTabClick());
        InitInventoryTabUI();
        InitCraftingTabUI();
    }
    private void InitInventoryTabUI()
    {
        slotsUI = new GameObject[inv.GetTotalSlots()];
        for (int i = 9; i < inv.GetTotalSlots(); i++)
        {
            slotsUI[i] = Instantiate(slotPrefab, inventoryTab.transform.GetChild(0));
        }
        for (int i = 0; i < 9; i++)
        {
            slotsUI[i] = Instantiate(slotPrefab, inventoryTab.transform.GetChild(0));
        }
        for (int i = 0; i < inv.GetTotalSlots(); i++)
        {
            // Add a listener to the slot's button component
            int slotIndex = i;
            slotsUI[i].GetComponent<Button>().onClick.AddListener(() => OnSlotLeftClick(slotIndex));

            // Add a script that implements the IPointerClickHandler interface to the slot
            var rightClickHandler = slotsUI[i].AddComponent<RightClickHandler>();
            rightClickHandler.OnRightClick += () => OnSlotRightClick(slotIndex);
        }
    }
    private void InitCraftingTabUI()
    {
        for (int i = 0; i < RecipeList.Recipes.Count; i++)
        {
            GameObject recipeSlot = Instantiate(recipeSlotPrefab, recipePanel.transform);
            recipeSlot.GetComponent<RecipeSlot>().Id = i;
        }

        craftButton.GetComponent<Button>().onClick.AddListener(() => OnCraftButtonClick());
    }
    private void UpdateInventory()
    {
        // Resize the inventory UI
        ResizeInventoryUI();

        if (tab == 0)
        {
            UpdateInventoryTab();
        }
        if (tab == 1)
        {
            UpdateCraftingTab();
        }
        
        // Open the inventory
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryUI.SetActive(!inventoryUI.activeSelf);
            Cursor.visible = !Cursor.visible;
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }
    private void UpdateInventoryTab()
    {
        // Update the inventory UI
        for (int i = 0; i < inv.GetTotalSlots(); i++)
        {
            int itemId = inv.GetItemId(i);
            int quantity = inv.GetQuantity(i);
            if (itemId != -1)
            {
                slotsUI[i].transform.GetChild(0).gameObject.SetActive(true);
                slotsUI[i].transform.GetChild(0).GetComponent<Image>().sprite = ItemList.GetItemTexture(itemId);
                slotsUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = quantity.ToString();
            }
            else
            {
                slotsUI[i].transform.GetChild(0).gameObject.SetActive(false);
                slotsUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        // Update the cursor slot item display
        int cursorItemId = cursorInv.GetItemId(0);
        int cursorItemQuantity = cursorInv.GetQuantity(0);
        if (cursorItemId != -1)
        {
            inventoryTab.transform.GetChild(1).gameObject.SetActive(true);
            inventoryTab.transform.GetChild(1).GetComponent<Image>().sprite = ItemList.GetItemTexture(cursorItemId);
            inventoryTab.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = cursorItemQuantity.ToString();
        }
        else
        {
            inventoryTab.transform.GetChild(1).gameObject.SetActive(false);
            inventoryTab.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        // Make the cursor item image follow the mouse cursor
        Vector2 mousePosition = Input.mousePosition;
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(inventoryTab.transform.GetChild(1).parent as RectTransform, mousePosition, UICam.GetComponent<Camera>(), out localPosition);
        inventoryTab.transform.GetChild(1).GetComponent<RectTransform>().localPosition = localPosition;
    }

    private void UpdateCraftingTab()
    {
        foreach (Transform child in itemDescription.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        currentRecipe = RecipeList.GetRecipe(currentRecipeID);
        itemImage.GetComponent<Image>().sprite = ItemList.GetItemTexture(currentRecipe.Result[0]);
        string name = ItemList.GetItemNameFromID(currentRecipe.Result[0]);
        if (currentRecipe.Result[1] > 1)
        {
            name += " x" + currentRecipe.Result[1];
        }
        itemName.GetComponent<TextMeshProUGUI>().text = name;

        foreach (int[] i in currentRecipe.Ingredient)
        {
            GameObject ingredient = Instantiate(ingredientPrefab, itemDescription.transform);
            ingredient.transform.GetChild(0).GetComponent<Image>().sprite = ItemList.GetItemTexture(i[0]);
            ingredient.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = ItemList.GetItemNameFromID(i[0]) + " x" + i[1] + " (" + inv.GetTotalQuantity(i[0]) + ")";
        }
    }
    private void OnSlotLeftClick(int slotIndex)
    {
        int itemId = inv.GetItemId(slotIndex);
        int quantity = inv.GetQuantity(slotIndex);
        int capacity = inv.GetCapacity(slotIndex);
        int cursorItemId = cursorInv.GetItemId(0);
        int cursorItemQuantity = cursorInv.GetQuantity(0);

        // Pick up the item from the slot
        if (cursorItemId == -1 && itemId != -1)
        {
            cursorInv.AddItem(itemId, quantity);
            inv.RemoveItemFromSlot(slotIndex, itemId, quantity);
        }
        // Place as many items as possible in the slot
        else if (cursorItemId != -1 && (itemId == -1 || itemId == cursorItemId))
        {
            int spaceLeft = capacity - quantity;
            int quantityToPlace = Mathf.Min(spaceLeft, cursorItemQuantity);
            bool success = inv.AddItemToSlot(slotIndex, cursorItemId, quantityToPlace);
            if (success)
            {
                cursorInv.RemoveItem(cursorItemId, quantityToPlace);
            }
        }
    }
    private void OnSlotRightClick(int slotIndex)
    {
        int itemId = inv.GetItemId(slotIndex);
        int cursorItemId = cursorInv.GetItemId(0);

        // Pick up one item from the slot
        if ((cursorItemId == -1 || cursorItemId == itemId) && itemId != -1)
        {
            bool success = cursorInv.AddItem(itemId, 1);
            if (success)
            {
                inv.RemoveItemFromSlot(slotIndex, itemId, 1);
            }
        }
    }
    private void CheckForPickup()
    {
        // Raycast from the center of the screen
        RaycastHit hit;
        Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the hit object has the Pickup tag
            if (hit.transform.CompareTag("Pickup"))
            {
                // Check if the player is close enough
                if (Vector3.Distance(transform.position, hit.transform.position) <= pickupDistance)
                {
                    // Check if the player presses the pickup key
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        // Collect the item and destroy it
                        bool success = inv.AddItem(ItemList.GetItemIDFromPickable(hit.transform.GetComponent<PickableObject>().Type), 1);
                        if (success) Destroy(hit.transform.gameObject);
                    }
                }
            }
        }
    }
    private void ChangeTab(int tabID)
    {
        if (tabID == 0)
        {
            tab = 0;
            inventoryTab.SetActive(true);
            craftingTab.SetActive(false);
            inventoryTabButton.GetComponent<Image>().color = new Color(222 / 255.0f, 222 / 255.0f, 222 / 255.0f);
            craftingTabButton.GetComponent<Image>().color = new Color(1, 1, 1);
        }
        if (tabID == 1)
        {
            tab = 1;
            craftingTab.SetActive(true);
            inventoryTab.SetActive(false);
            craftingTabButton.GetComponent<Image>().color = new Color(222 / 255.0f, 222 / 255.0f, 222 / 255.0f);
            inventoryTabButton.GetComponent<Image>().color = new Color(1, 1, 1);
        }
    }
    private void OnInventoryTabClick() => ChangeTab(0);
    private void OnCraftingTabClick() => ChangeTab(1);
    private void OnCraftButtonClick()
    {
        if (currentRecipe != null)
        {
            bool success = inv.Craft(currentRecipe);
        }
    }
    void ResizeInventoryUI()
    {
        // Get the current screen size
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        // Calculate the new local scale for inventoryUI
        float newLocalScale = 2f * screenSize.x / 1920f;

        // Set the local scale of inventoryUI
        inventoryUI.transform.localScale = new Vector3(newLocalScale, newLocalScale, 1f);
    }
}
