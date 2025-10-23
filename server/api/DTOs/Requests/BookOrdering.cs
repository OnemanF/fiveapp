namespace api.DTOs.Requests;

public enum BookOrdering
{
    NameAsc = 0,
    NameDesc = 1,
    LongestTitle = 2,
    MostPages = 3,
    GenreName = 4,
    MostRecentlyCreated = 5,
    MostProductiveAuthor = 6
}