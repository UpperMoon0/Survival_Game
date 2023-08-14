using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject slotPrefab;
    private bool isGrounded = true;
    private float
        camRotation = 0f,
        maxCamRotation = 90f,
        rotationSpeed = 5f,
        jumpForce = 5f;
    private Rigidbody rb;
    private GameObject
        cam,
        canvas,
        inventoryUI;
    private GameObject[] slotsUI;
    private Inventory 
        inv,
        cursorInv;
    private void Awake()
    {
        // Objects initialization
        cam = transform.GetChild(1).gameObject;
        canvas = transform.GetChild(2).gameObject;
        inventoryUI = canvas.transform.GetChild(0).gameObject;

        // Create the player's inventory
        inv = new Inventory(36);
        cursorInv = new Inventory(1);

        // UI initialization
        inventoryUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        slotsUI = new GameObject[inv.GetTotalSlots()];
        for (int i = 0; i < inv.GetTotalSlots(); i++)
        {
            slotsUI[i] = Instantiate(slotPrefab, inventoryUI.transform.GetChild(0));
        }

        // Initialize the inventory UI
        InitializeInventoryUI();

        // Freeze the player's rotation
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }
    private void Update()
    {
        UpdateInventory();
        UpdateMovement();
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
    private void InitializeInventoryUI()
    {
        for (int i = 0; i < inv.GetTotalSlots(); i++)
        {
            // Add a listener to the slot's button component
            int slotIndex = i;
            slotsUI[i].GetComponent<Button>().onClick.AddListener(() => OnSlotClick(slotIndex));
        }
    }
    private void UpdateInventory()
    {
        // Update the inventory UI
        for (int i = 0; i < inv.GetTotalSlots(); i++)
        {
            int itemId = inv.GetItemId(i);
            int quantity = inv.GetQuantity(i);
            if (itemId != -1)
            {
                slotsUI[i].transform.GetChild(0).gameObject.SetActive(true);
                slotsUI[i].transform.GetChild(0).GetComponent<Image>().sprite = ItemDict.GetItemTexture(itemId);
                slotsUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = quantity.ToString();
            }
            else
            {
                slotsUI[i].transform.GetChild(0).gameObject.SetActive(false);
                slotsUI[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        // Open the inventory
        if (Input.GetKeyDown(KeyCode.E))
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            inv.AddItem(0, 1);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            inv.AddItem(1, 1);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inv.AddItem(2, 1);
        }
    }
    private void OnSlotClick(int slotIndex)
    {
        int itemId = inv.GetItemId(slotIndex);
        int quantity = inv.GetQuantity(slotIndex);
        int cursorItemId = cursorInv.GetItemId(0);
        int cursorItemQuantity = cursorInv.GetQuantity(0);

        // Pick up the item from the slot
        if (cursorItemId == -1 && itemId != -1)
        {
            cursorInv.AddItem(itemId, quantity);
            inv.RemoveItemFromSlot(slotIndex, itemId, quantity);
        }
        // Place the item down in the slot
        else if (cursorItemId != -1 && (itemId == -1 || itemId == cursorItemId))
        { 
            bool success = inv.AddItemToSlot(slotIndex, cursorItemId, cursorItemQuantity);
            if (success)
            {
                cursorInv.RemoveItem(cursorItemId, cursorItemQuantity);
            }
        }
    }

}
