namespace Task2.Models
{
    public class Data
    {
        public int AccountId { get; set; }
        public decimal OpeningBalanceAssets { get; set; }
        public decimal OpeningBalanceLiabilities { get; set; }
        public decimal MoneyTurnoverDebit { get; set; }
        public decimal MoneyTurnoverCredit { get; set; }
        public decimal ClosingBalanceAssets { get; set; }
        public decimal ClosingBalanceLiabilities { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int AccountGroup { get; set; }
    }
}
