using System.ComponentModel.DataAnnotations;

namespace Domain.Accounts;

/// <summary>
/// Represents the type of bank account
/// </summary>
public enum AccountType
{
    /// <summary>
    /// A basic savings account for personal banking
    /// </summary>
    [Display(Name = "Savings Account")]
    Savings = 1,

    /// <summary>
    /// A checking account for daily transactions
    /// </summary>
    [Display(Name = "Checking Account")]
    Checking = 2,

    /// <summary>
    /// A business account for commercial use
    /// </summary>
    [Display(Name = "Business Account")]
    Business = 3
}
