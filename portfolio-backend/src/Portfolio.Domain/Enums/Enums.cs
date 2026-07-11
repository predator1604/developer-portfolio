namespace Portfolio.Domain.Enums;

public enum MessageStatus
{
    New = 0,
    Read = 1,
    Replied = 2,
    Archived = 3,
}

public enum ChatMessageRole
{
    User = 0,
    Assistant = 1,
    System = 2,
}
