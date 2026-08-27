namespace UniConnect.Domain.Enums;
public enum UserType
{
    Student = 1,
    Alumni = 2,
    Business = 3,
    Admin = 4
}

public enum VerificationStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public enum OpportunityStatus
{
    Draft = 1,
    PendingApproval = 2,
    Published = 3,
    Closed = 4
}

public enum ConnectionStatus
{
    Pending = 1,
    Accepted = 2,
    Declined = 3
}

public enum MediaType
{
    Image = 1,
    Video = 2,
    Document = 3
}
