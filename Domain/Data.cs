namespace Domain;
public abstract class Data {
    public static readonly Data Empty = new EmptyData();
}

public class EmptyData : Data;