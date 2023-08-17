using UnityEngine;
using UnityEngine.UI;

public class RecipeSlot : MonoBehaviour
{
    private int id;
    private bool selected = false;
    private Player player;
    public int Id { get => id; set => id = value; }
    private void Start()
    {
        player = transform.root.GetComponent<Player>();
        var recipeList = RecipeList.Recipes;
        transform.GetChild(0).GetComponent<Image>().sprite = ItemList.GetItemTexture(recipeList[id].Result[0]);
        GetComponent<Button>().onClick.AddListener(() => OnButtonClicked());
    }
    private void Update()
    {
        if (player.CurrentRecipeID == id) 
            selected = true; 
        else 
            selected = false;

        if (selected) 
            GetComponent<Image>().color = new Color(180 / 255f, 180 / 255f, 180 / 255f); 
        else 
            GetComponent<Image>().color = new Color(222 / 255f, 222 / 255f, 222 / 255f);
    }
    private void OnButtonClicked()
    {
        player.CurrentRecipeID = id;
    }
}
