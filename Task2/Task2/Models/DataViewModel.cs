namespace Task2.Models
{
    public class DataViewModel
    {
        public List<Data> Data { get; set; }
        public decimal OpeningBalanceAssetsGeneralSum { get; set; }
        public decimal OpeningBalanceLiabilitiesGeneralSum { get; set; }
        public decimal MoneyTurnoverDebitGeneralSum { get; set; }
        public decimal MoneyTurnoverCreditGeneralSum { get; set; }
        public decimal ClosingBalanceAssetsGeneralSum { get; set; }
        public decimal ClosingBalanceLiabilitiesGeneralSum { get; set; }

        public string BankName { get; set; }
        public string DocumentTitle { get; set; }
        public string TimePeriod { get; set; }
        public string Option { get; set; }
        public string DateOfCreation { get; set; }
        public string Currency { get; set; }
    }
}
