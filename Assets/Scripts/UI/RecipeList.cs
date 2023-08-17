using System.Collections.Generic;

public static class RecipeList
{
    private static List<Recipe> recipes = new List<Recipe>();
    static RecipeList()
    {
        // Flint axe
        int[] result = new int[] { ItemList.GetItemIDFromName("Flint Axe"), 1 };
        List<int[]> ingredients = new List<int[]>
        {
            new int[] { ItemList.GetItemIDFromName("Flint"), 2 },
            new int[] { ItemList.GetItemIDFromName("Stick"), 3 }
        };
        recipes.Add(new Recipe(result, ingredients));
        // Wood
        result = new int[] { ItemList.GetItemIDFromName("Wood"), 5 };
        ingredients = new List<int[]>
        {
            new int[] { ItemList.GetItemIDFromName("Flint"), 2 },
            new int[] { ItemList.GetItemIDFromName("Stick"), 3 },
            new int[] { ItemList.GetItemIDFromName("Flint Axe"), 12 }
        };
        recipes.Add(new Recipe(result, ingredients));
    }
    public static List<Recipe> Recipes { get => recipes; }
    public static Recipe GetRecipe(int id) => recipes[id];
}
