namespace api.DTOs.Loan
    {
    public class LoanValidationResult
        {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public static LoanValidationResult Success()
            {
            return new LoanValidationResult { IsValid = true };
            }

        public static LoanValidationResult Failure(string errorMessage)
            {
            return new LoanValidationResult { IsValid = false, ErrorMessage = errorMessage };
            }
        }
    }
