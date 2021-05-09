using System.Collections.Generic;

namespace Farseer.Net.DataDemo
{
    public class UserCoinsAgent
    {
        private readonly TestContext _context;

        public UserCoinsAgent(TestContext context)
        {
            _context = context;
        }

        public UserCoinsAgent()
        {
            _context = TestContext.Data;
        }

        public List<UserCoinsPO> ToCreditList()
        {
            return _context.UserCoins.SetName("lottery_company_dt", "account_user_coins").Where(o => o.CoinsType == EumCoinsType.Credit && o.Quota != o.Coins).ToList();
        }
    }
}