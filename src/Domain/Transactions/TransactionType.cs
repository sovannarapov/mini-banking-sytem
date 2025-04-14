using System.ComponentModel.DataAnnotations;

namespace Domain.Transactions;

/// <summary>
/// Represents the type of banking transaction
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// Adding money to an account
    /// </summary>
    [Display(Name = "Deposit")]
    Deposit = 1,

    /// <summary>
    /// Removing money from an account
    /// </summary>
    [Display(Name = "Withdraw")]
    Withdraw = 2,

    /// <summary>
    /// Moving money between accounts
    /// </summary>
    [Display(Name = "Transfer")]
    Transfer = 3
}
