using System.Collections.Generic;

public class Recipe
{
    private int[] result;
    private List<int[]> ingredient = new List<int[]>();
    public Recipe(int[] result, List<int[]> ingredient)
    {
        this.result = result;
        this.ingredient = ingredient;
    }
    public int[] Result { get => result; }
    public List<int[]> Ingredient { get => ingredient; }
}